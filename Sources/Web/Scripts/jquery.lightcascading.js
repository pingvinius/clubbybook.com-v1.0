/**
 * jQuery Light Cascading Plugin
 * 
 * @author  Oleg Kashpur
 * @email pingvinius@gmail.com
 * @twitter pingvinius
 * @version 1.00
 * 
 */

(function ($) {

  $.fn.cascading = function (options) {

    var defaults = {
      url: "",
      params: [],
      getValues: function () {
        return [];
      },
      getSelectedValue: function () {
        return options.noValue;
      },
      parentId: "",
      loadingText: "loading",
      promptText: "(select value)",
      errorText: "error",
      noValue: -1,
      supportDisable: true,
      getItem: function (object) {
        return wrapItem(options.errorText, options.noValue);
      }
    };

    var opts = $.extend({}, defaults, options);

    this.each(function () {
      cascadingInternal($(this), opts);
    });

    return this;
  };


  function cascadingInternal(element, options) {

    if (options.parentId && options.parentId.length > 0) {

      var parentElement = $("#" + options.parentId);

      if (options.supportDisable)
        element.attr("disabled", "disabled");

      parentElement.bind("change", function () {

        loading(element, options);
      });

      // if parent control has value, process loading immediately
      if (parentElement.val() !== options.noValue)
        loading(element, options);
    }
    else
      loading(element, options);
  }

  function loading(element, options) {

    fill(element, [wrapItem(options.loadingText, options.noValue)]);

    var ajaxParams = getAjaxParams(options);

    $.ajax({
      type: "POST",
      url: options.url,
      data: ajaxParams,
      async: false,
      cache: false,
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (data, textStatus, jqXHR) {

        var itemsArray = new Array();
        $.each(data.d, function (index, object) {
          itemsArray.push(options.getItem(object));
        });

        if (itemsArray.length > 0)
          itemsArray.unshift(wrapItem(options.promptText, options.noValue));

        fill(element, itemsArray, options.getSelectedValue());
      },
      error: function (jqXHR, textStatus, errorThrown) {

        fill(element, [wrapItem(options.errorText, options.noValue)], options.noValue);
      }
    });
  }

  function wrapItem(txt, val) {

    return { text: txt, value: val };
  }

  function fill(element, array, selectedValue) {

    element.empty();

    $.each(array, function (index, item) {

      var selectedStr = "";
      if (selectedValue && selectedValue === item.value)
        selectedStr = " selected='selected'";

      element.append("<option value='" + item.value + "'" + selectedStr + ">" + item.text + "</option>");
    });

    element.removeAttr("disabled");
  }

  function getAjaxParams(options) {

    var values = options.getValues();

    var ajaxDataArray = new Array();
    for (i = 0; i < options.params.length; i++) {
      ajaxDataArray.push("'" + options.params[i] + "': '" + values[i] + "'");
    }

    return "{" + ajaxDataArray.join(',') + "}";
  }

})(jQuery);