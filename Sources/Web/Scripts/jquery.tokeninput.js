/*
 * jQuery Plugin: Tokenizing Autocomplete Text Entry
 * Version 1.4.2
 *
 * Copyright (c) 2009 James Smith (http://loopj.com)
 * Licensed jointly under the GPL and MIT licenses,
 * choose which one suits your project best!
 *
 */

(function ($) {

  // Default settings
  var DEFAULT_SETTINGS = {
    hintText: "Type in a search term",
    noResultsText: "No results",
    searchingText: "Searching...",
    deleteText: "&times;",
    searchDelay: 200,
    minChars: 1,
    tokenLimit: null,
    jsonContainer: null,
    method: "POST",
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    queryParam: "query",
    tokenDelimiter: ",",
    preventDuplicates: false,
    maxItemsCount: 100,
    prePopulate: null,
    animateDropdown: true,
    onResult: null,
    onAdd: null,
    onDelete: null
  };

  // Default classes to use when theming
  var DEFAULT_CLASSES = {
    tokenList: "token-input-list",
    token: "token-input-token",
    tokenDelete: "token-input-delete-token",
    selectedToken: "token-input-selected-token",
    highlightedToken: "token-input-highlighted-token",
    dropdown: "token-input-dropdown",
    dropdownItem: "token-input-dropdown-item",
    dropdownItem2: "token-input-dropdown-item2",
    selectedDropdownItem: "token-input-selected-dropdown-item",
    inputToken: "token-input-input-token"
  };

  // Input box position "enum"
  var POSITION = {
    BEFORE: 0,
    AFTER: 1,
    END: 2
  };

  // Expose the .tokenInput function to jQuery as a plugin
  $.fn.tokenInput = function (url_or_data, options) {

    return this.each(function () {
      new $.TokenList(this, url_or_data, options);
    });
  };

  // TokenList class for each input
  $.TokenList = function (input, url_or_data, options) {

    var settings = $.extend({}, DEFAULT_SETTINGS, options || {});

    //
    // Initialization
    //

    // Configure the data source
    if (typeof (url_or_data) === "string") {

      // Set the url to query against
      settings.url = url_or_data;

      // Make a smart guess about cross-domain if it wasn't explicitly specified
      if (settings.crossDomain === undefined) {

        if (settings.url.indexOf("://") === -1)
          settings.crossDomain = false;
        else
          settings.crossDomain = (location.href.split(/\/+/g)[1] !== settings.url.split(/\/+/g)[1]);
      }
    }
    else if (typeof (url_or_data) === "object") // Set the local data to search through
      settings.local_data = url_or_data;

    // Build class names
    if (settings.classes) // Use custom class names
      settings.classes = $.extend({}, DEFAULT_CLASSES, settings.classes);
    else if (settings.theme) { // Use theme-suffixed default class names

      settings.classes = {};
      $.each(DEFAULT_CLASSES, function (key, value) {
        settings.classes[key] = value + "-" + settings.theme;
      });
    }
    else
      settings.classes = DEFAULT_CLASSES;

    var saved_tokens = []; // Save the tokens
    var token_count = 0; // Keep track of the number of tokens in the list
    var cache = new $.TokenList.Cache(); // Basic cache to save on db hits

    // Keep track of the timeout, old vals
    var timeout;
    var input_val;

    // Create a new text input an attach keyup events
    var input_box = $("<input type=\"text\"  autocomplete=\"off\">")
      .css({
        outline: "none"
      })
      .focus(function () {

        if (settings.tokenLimit === null || settings.tokenLimit !== token_count)
          show_dropdown_hint();
      })
      .blur(function () {

        hide_dropdown();
      })
      .bind("keyup keydown blur update", resize_input)
      .keypress(function (event) {

        var key;
        if (window.event)
          key = window.event.keyCode; // IE
        else
          key = e.which; // firefox

        return (key != 13);
      })
      .keydown(function (event) {

        var previous_token;
        var next_token;

        switch (event.keyCode) {
          case Keys.Left:
          case Keys.Right:
          case Keys.Up:
          case Keys.Down:
            if (!$(this).val()) {
              previous_token = input_token.prev();
              next_token = input_token.next();

              if ((previous_token.length && previous_token.get(0) === selected_token) || (next_token.length && next_token.get(0) === selected_token)) {

                // Check if there is a previous/next token and it is selected
                if (event.keyCode === Keys.Left || event.keyCode === Keys.Up)
                  deselect_token($(selected_token), POSITION.BEFORE);
                else
                  deselect_token($(selected_token), POSITION.AFTER);
              }
              else if ((event.keyCode === Keys.Left || event.keyCode === Keys.Up) && previous_token.length)
                select_token($(previous_token.get(0))); // We are moving left, select the previous token if it exists
              else if ((event.keyCode === Keys.Right || event.keyCode === Keys.Down) && next_token.length)
                select_token($(next_token.get(0))); // We are moving right, select the next token if it exists
            }
            else {

              var dropdown_item = null;

              if (event.keyCode === Keys.Down || event.keyCode === Keys.Right)
                dropdown_item = $(selected_dropdown_item).next();
              else
                dropdown_item = $(selected_dropdown_item).prev();

              if (dropdown_item.length)
                select_dropdown_item(dropdown_item);

              return false;
            }
            break;

          case Keys.Backspace:
            previous_token = input_token.prev();

            if (!$(this).val().length) {

              if (selected_token)
                delete_token($(selected_token));
              else if (previous_token.length)
                select_token($(previous_token.get(0)));

              return false;
            }
            else if ($(this).val().length === 1)
              hide_dropdown();
            else
              setTimeout(function () { do_search(); }, 5); // set a timeout just long enough to let this function finish.
            break;

          case Keys.Tab:
          case Keys.Enter:
          case Keys.NumpadEnter:
          case Keys.Comma:
            if (selected_dropdown_item) {
              add_token($(selected_dropdown_item));
              return false;
            }
            break;

          case Keys.Escape:
            hide_dropdown();
            return true;

          default:
            if (String.fromCharCode(event.which))
              setTimeout(function () { do_search(); }, 5); // set a timeout just long enough to let this function finish.
            break;
        }
      });

    // Keep a reference to the original input box
    var hidden_input = $(input)
      .hide()
      .val("")
      .focus(function () {
        input_box.focus();
      })
      .blur(function () {
        input_box.blur();
      });

    // Keep a reference to the selected token and dropdown item
    var selected_token = null;
    var selected_token_index = 0;
    var selected_dropdown_item = null;

    // The list to store the token items in
    var token_list = $("<ul />")
        .addClass(settings.classes.tokenList)
        .css("width", $(input).css("width"))
        .click(function (event) {

          var li = $(event.target).closest("li");
          if (li && li.get(0) && $.data(li.get(0), "tokeninput"))
            toggle_select_token(li);
          else {

            if (selected_token)
              deselect_token($(selected_token), POSITION.END); // Deselect selected token

            // Focus input box
            input_box.focus();
          }
        })
        .mouseover(function (event) {

          var li = $(event.target).closest("li");
          if (li && selected_token !== this)
            li.addClass(settings.classes.highlightedToken);
        })
        .mouseout(function (event) {

          var li = $(event.target).closest("li");
          if (li && selected_token !== this)
            li.removeClass(settings.classes.highlightedToken);
        })
        .insertBefore(hidden_input);

    // The token holding the input box
    var input_token = $("<li />")
      .addClass(settings.classes.inputToken)
      .appendTo(token_list)
      .append(input_box);

    // The list to store the dropdown items in
    var dropdown = $("<div>")
      .addClass(settings.classes.dropdown)
      .css("margin-left", 3)
      .css("width", $(input).css("width"))
      .appendTo("body")
      .hide();

    // Magic element to help us resize the text input
    var input_resizer = $("<tester/>")
      .insertAfter(input_box)
      .css({
        position: "absolute",
        top: -9999,
        left: -9999,
        width: "auto",
        fontSize: input_box.css("fontSize"),
        fontFamily: input_box.css("fontFamily"),
        fontWeight: input_box.css("fontWeight"),
        letterSpacing: input_box.css("letterSpacing"),
        whiteSpace: "nowrap"
      });

    // Pre-populate list if items exist
    hidden_input.val("");
    li_data = settings.prePopulate || hidden_input.data("pre");
    if (li_data && li_data.length) {
      $.each(li_data, function (index, value) {
        insert_token(value.value, value.text);
      });
    }

    //
    // Public functions
    //

    this.clearTokens = clearTokensInternal;

    function clearTokensInternal() {

      while ($(input_token).prev().length > 0)
        delete_token($(input_token).prev());

      hide_dropdown();
    }

    //
    // Private functions
    //

    function resize_input() {

      if (input_val === (input_val = input_box.val())) { return; }

      // Enter new content into resizer and resize input accordingly
      var escaped = input_val.replace(/&/g, '&amp;').replace(/\s/g, ' ').replace(/</g, '&lt;').replace(/>/g, '&gt;');
      input_resizer.html(escaped);
      input_box.width(input_resizer.width() + 30);
    }

    function is_printable_character(keycode) {

      return ((keycode >= 48 && keycode <= 90) ||     // 0-1a-z
                (keycode >= 96 && keycode <= 111) ||    // numpad 0-9 + - / * .
                (keycode >= 186 && keycode <= 192) ||   // ; = , - . / ^
                (keycode >= 219 && keycode <= 222));    // ( \ ) '
    }

    // Inner function to a token to the list
    function insert_token(value, text) {

      var this_token = $("<li><p>" + text + "</p></li>")
        .addClass(settings.classes.token)
        .insertBefore(input_token);

      // The 'delete token' button
      $("<span>" + settings.deleteText + "</span>")
        .addClass(settings.classes.tokenDelete)
        .appendTo(this_token)
        .click(function () {
          delete_token($(this).parent());
          return false;
        });

      // Store data on the token
      var token_data = { "value": value, "text": text };
      $.data(this_token.get(0), "tokeninput", token_data);

      // Save this token for duplicate checking
      saved_tokens = saved_tokens.slice(0, selected_token_index).concat([token_data]).concat(saved_tokens.slice(selected_token_index));
      selected_token_index++;

      // Update the hidden input
      var token_values = $.map(saved_tokens, function (el) {
        return el.value;
      });
      hidden_input.val(token_values.join(settings.tokenDelimiter));

      token_count += 1;

      return this_token;
    }

    // Add a token to the token list based on user input
    function add_token(item) {

      var li_data = $.data(item.get(0), "tokeninput");
      var callback = settings.onAdd;

      if (token_count >= settings.maxItemsCount) {

        input_box.focus();
        return;
      }

      // See if the token already exists and select it if we don't want duplicates
      if (token_count > 0 && settings.preventDuplicates) {

        var found_existing_token = null;
        token_list.children().each(function () {

          var existing_token = $(this);
          var existing_data = $.data(existing_token.get(0), "tokeninput");
          if (existing_data && existing_data.value === li_data.value) {

            found_existing_token = existing_token;
            return false;
          }
        });

        if (found_existing_token) {

          select_token(found_existing_token);
          input_token.insertAfter(found_existing_token);
          input_box.focus();
          return;
        }
      }

      // Insert the new tokens
      insert_token(li_data.value, li_data.text);

      // Check the token limit
      if (settings.tokenLimit !== null && token_count >= settings.tokenLimit) {

        input_box.hide();
        hide_dropdown();
        return;
      }
      else
        input_box.focus();

      // Clear input box
      input_box.val("");

      // Don't show the help dropdown, they've got the idea
      hide_dropdown();

      // Execute the onAdd callback if defined
      if ($.isFunction(callback))
        callback(li_data);
    }

    // Select a token in the token list
    function select_token(token) {

      token.addClass(settings.classes.selectedToken);
      selected_token = token.get(0);

      // Hide input box
      input_box.val("");

      // Hide dropdown if it is visible (eg if we clicked to select token)
      hide_dropdown();
    }

    // Deselect a token in the token list
    function deselect_token(token, position) {

      token.removeClass(settings.classes.selectedToken);
      selected_token = null;

      if (position === POSITION.BEFORE) {

        input_token.insertBefore(token);
        selected_token_index--;
      }
      else if (position === POSITION.AFTER) {

        input_token.insertAfter(token);
        selected_token_index++;
      }
      else {

        input_token.appendTo(token_list);
        selected_token_index = token_count;
      }

      // Show the input box and give it focus again
      input_box.focus();
    }

    // Toggle selection of a token in the token list
    function toggle_select_token(token) {

      var previous_selected_token = selected_token;

      if (selected_token)
        deselect_token($(selected_token), POSITION.END);

      if (previous_selected_token === token.get(0))
        deselect_token(token, POSITION.END);
      else
        select_token(token);
    }

    // Delete a token from the token list
    function delete_token(token) {

      // Remove the value from the saved list
      var token_data = $.data(token.get(0), "tokeninput");
      var callback = settings.onDelete;

      var index = token.prevAll().length;
      if (index > selected_token_index) index--;

      // Delete the token
      token.remove();
      selected_token = null;

      // Show the input box and give it focus again
      input_box.focus();

      // Remove this token from the saved list
      saved_tokens = saved_tokens.slice(0, index).concat(saved_tokens.slice(index + 1));
      if (index < selected_token_index) selected_token_index--;

      // Update the hidden input
      var token_values = $.map(saved_tokens, function (el) {
        return el.value;
      });
      hidden_input.val(token_values.join(settings.tokenDelimiter));

      token_count -= 1;

      if (settings.tokenLimit !== null)
        input_box.show().val("").focus();

      // Execute the onDelete callback if defined
      if ($.isFunction(callback))
        callback(token_data);
    }

    // Hide and clear the results dropdown
    function hide_dropdown() {

      dropdown.hide().empty();
      selected_dropdown_item = null;
    }

    function show_dropdown() {

      dropdown.css({
        position: "absolute",
        top: $(token_list).offset().top + $(token_list).outerHeight(),
        left: $(token_list).offset().left,
        zindex: 999
      })
        .show();
    }

    function show_dropdown_searching() {

      if (settings.searchingText) {
        dropdown.html("<p>" + settings.searchingText + "</p>");
        show_dropdown();
      }
    }

    function show_dropdown_hint() {

      if (settings.hintText) {
        dropdown.html("<p>" + settings.hintText + "</p>");
        show_dropdown();
      }
    }

    // Highlight the query part of the search term
    function highlight_term(value, term) {

      return value.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + term + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<b>$1</b>");
    }

    // Populate the results dropdown with some results
    function populate_dropdown(query, results) {

      if (results && results.length) {

        dropdown.empty();
        var dropdown_ul = $("<ul>")
          .appendTo(dropdown)
          .mouseover(function (event) {
            select_dropdown_item($(event.target).closest("li"));
          })
          .mousedown(function (event) {
            add_token($(event.target).closest("li"));
            return false;
          })
          .hide();

        $.each(results, function (index, value) {

          var this_li = $("<li>" + highlight_term(value.text, query) + "</li>")
                                  .appendTo(dropdown_ul);

          if (index % 2)
            this_li.addClass(settings.classes.dropdownItem);
          else
            this_li.addClass(settings.classes.dropdownItem2);

          if (index === 0)
            select_dropdown_item(this_li);

          $.data(this_li.get(0), "tokeninput", { "value": value.value, "text": value.text });
        });

        show_dropdown();

        if (settings.animateDropdown)
          dropdown_ul.slideDown("fast");
        else
          dropdown_ul.show();
      }
      else {

        if (settings.noResultsText) {
          dropdown.html("<p>" + settings.noResultsText + "</p>");
          show_dropdown();
        }
      }
    }

    // Highlight an item in the results dropdown
    function select_dropdown_item(item) {

      if (item) {

        if (selected_dropdown_item)
          deselect_dropdown_item($(selected_dropdown_item));

        item.addClass(settings.classes.selectedDropdownItem);
        selected_dropdown_item = item.get(0);
      }
    }

    // Remove highlighting from an item in the results dropdown
    function deselect_dropdown_item(item) {

      item.removeClass(settings.classes.selectedDropdownItem);
      selected_dropdown_item = null;
    }

    // Do a search and show the "searching" dropdown if the input is longer
    // than settings.minChars
    function do_search() {

      var query = input_box.val().toLowerCase();

      if (query && query.length) {

        if (selected_token)
          deselect_token($(selected_token), POSITION.AFTER);

        if (query.length >= settings.minChars) {

          show_dropdown_searching();
          clearTimeout(timeout);

          timeout = setTimeout(function () { run_search(query); }, settings.searchDelay);
        }
        else
          hide_dropdown();
      }
    }

    // Do the actual search
    function run_search(query) {

      var cached_results = cache.get(query);
      if (cached_results)
        populate_dropdown(query, cached_results);
      else {

        // Are we doing an ajax search or local data search?
        if (settings.url) {

          $.ajax({
            url: settings.url,
            data: "{ '" + settings.queryParam + "': '" + query + "' }",
            type: settings.method,
            dataType: settings.crossDomain ? "jsonp" : settings.dataType,
            contentType: settings.contentType,
            success: function (data, textStatus, jqXHR) {

              var results = data.d;
              if ($.isFunction(settings.onResult)) {
                results = settings.onResult.call(this, results);
              }
              cache.add(query, settings.jsonContainer ? results[settings.jsonContainer] : results);

              // only populate the dropdown if the results are associated with the active search query
              if (input_box.val().toLowerCase() === query)
                populate_dropdown(query, settings.jsonContainer ? results[settings.jsonContainer] : results);
            },
            error: function (jqXHR, textStatus, errorThrown) {

              if (input_box.val().toLowerCase() === query)
                populate_dropdown(query, []);
            }
          });

        }
        else if (settings.local_data) {

          // Do the search through local data
          var results = $.grep(settings.local_data, function (row) {
            return row.text.toLowerCase().indexOf(query.toLowerCase()) > -1;
          });

          if ($.isFunction(settings.onResult))
            results = settings.onResult.call(this, results);

          cache.add(query, results);
          populate_dropdown(query, results);
        }
      }
    }

    return this;
  };

  // Really basic cache for the results
  $.TokenList.Cache = function (options) {

    var settings = $.extend({
      max_size: 500
    }, options);

    var data = {};
    var size = 0;

    var flush = function () {
      data = {};
      size = 0;
    };

    this.add = function (query, results) {

      if (size > settings.max_size)
        flush();

      if (!data[query])
        size += 1;

      data[query] = results;
    };

    this.get = function (query) {
      return data[query];
    };
  };

} (jQuery));
