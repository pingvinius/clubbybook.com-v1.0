/**
* jQuery Light Carousel Plugin
* 
* @author  Oleg Kashpur
* @email pingvinius@gmail.com
* @twitter pingvinius
* @version 1.00
* 
*/

(function ($) {

  var defaults = {
    itemTemplateSelector: "",
    carouselClass: "lightcarousel",
    prevClass: "lc_prev",
    nextClass: "lc_next",
    loadingClass: "lc_loading",
    listClass: "lc_ul",
    navClass: "lc_nav",
    emptyClass: "lc_empty",
    errorClass: "lc_error",
    emptyText: "Empty",
    defaultErrorMessage: "The unknown error occurred."
  };

  $.lightcarousel = function (sender, options) {

    if (!isDefinedAndNotNull(sender))
        throw Errors.ArgumentNullException;

    var settings = $.extend({}, defaults, options);
    var itemsCount = 0;

    $(sender).empty();
    $(sender).addClass(settings.carouselClass);
    $("<div></div>").addClass(settings.loadingClass).append($("<img />")).appendTo($(sender));


    this.add = function (list) {

      if (!isDefinedAndNotNull(list))
        list = new Array();

      if (list.length > 0) {

        if (itemsCount > 0) {

          $.each ($(settings.itemTemplateSelector).tmpl(list), function (index, value) {

            raiseCarouselEvent("insertItem", [$(value), 0, true]);
          });
        }
        else
          this.update(list);
      }

      itemsCount += list.length;

      updateNavBarVisible();
    }

    this.update = function (list) {

      if (!isDefinedAndNotNull(list))
        list = new Array();

      destroyUI();

      if (list.length > 0) { // Create new carousel

        var listControl = $("<ul></ul>").addClass(settings.listClass);
        var navigationControl = $("<div></div>").addClass(settings.navClass);
        var prevControl = $("<a></a>").addClass(settings.prevClass).attr("href", "#").appendTo(navigationControl);
        var nextControl = $("<a></a>").addClass(settings.nextClass).attr("href", "#").appendTo(navigationControl);

        $(settings.itemTemplateSelector).tmpl(list).appendTo(listControl);

        $(sender).append(listControl);
        $(sender).append($("<div></div>").addClass("clear"));
        $(sender).append(navigationControl);

        $(listControl).carouFredSel({
          auto: false,
          prev: getSelectorFor("a." + settings.prevClass),
          next: getSelectorFor("a." + settings.nextClass),
          scroll: {
            items: 7
          }
        });
      }
      else
        $("<div></div>").addClass(settings.emptyClass).text(settings.emptyText).appendTo(sender);

      itemsCount = list.length;

      updateNavBarVisible();
    }

    this.error = function (errorMessage, errorType, stackTrace) {

      if (!isDefinedAndNotNull(errorMessage))
        errorMessage = settings.defaultErrorMessage;

      destroyUI();

      $("<div></div>").addClass(settings.errorClass).html(errorMessage).appendTo(sender);
    }


    function getSelectorFor(childElementSelector) {

      var selector = "#" + $(sender).attr("id");
      if (isDefinedAndNotNull(childElementSelector))
        selector += " " + childElementSelector;
      return selector;
    }

    function raiseCarouselEvent(eventName, params) {

      return $(getSelectorFor("ul." + settings.listClass)).trigger(eventName, params);
    }

    function getCarouselConfiguration(paramName) {

      var val = null;
      raiseCarouselEvent("configuration", [paramName, function( value ) {

        val = value;
      }]);

      return val;
    }

    function updateNavBarVisible() {

      var nav = $(getSelectorFor("." + settings.navClass));
      if (nav.length > 0) {

        var currentVisibleItemsCount = getCarouselConfiguration("items.visible");
        if (!isNaN(currentVisibleItemsCount) && itemsCount > currentVisibleItemsCount)
          $(nav.get(0)).show();
        else
          $(nav.get(0)).hide();
      }
    }

    function destroyUI() {

      raiseCarouselEvent("destroy");

      itemsCount = 0;

      $(sender).empty();
    }

    return this;
  };

  $.fn.lightcarousel = function (options) {

    if (this.length !== 1)
      throw Errors.ArgumentException;

    return new $.lightcarousel($(this).get(0), options);
  };

})(jQuery);