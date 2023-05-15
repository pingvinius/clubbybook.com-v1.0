/**
* jQuery Back to Top Plugin
* 
* @author  Oleg Kashpur
* @email pingvinius@gmail.com
* @twitter pingvinius
* @version 1.00
* 
*/

(function ($) {

  var defaults = {
    text: "Back to Top",
    styleClass: "backtotop",
    showFrom: 300,
    fullShowFrom: 2000
  };

  function createUI(instance) {

    if (!isDefinedAndNotNull(instance))
      throw Errors.ArgumentNullException;

    var link = $("<a></a>")
      .text(instance.settings.text)
      .bind("click", function () { linkClick(instance); });

    instance.ui = $("<div></div>")
      .addClass(instance.settings.styleClass)
      .append(link)
      .appendTo($("form"));
  }

  function linkClick(instance) {

    if (!isDefinedAndNotNull(instance))
      throw Errors.ArgumentNullException;

    window.scroll(0, 0);
  }

  function updateVisibility(instance) {

    if (!isDefinedAndNotNull(instance))
      throw Errors.ArgumentNullException;

    var documentScrollOffset = getDocumentScrollOffset();

    if (documentScrollOffset.top > instance.settings.showFrom) {

      var opacity = (documentScrollOffset.top - instance.settings.showFrom) /
        (instance.settings.fullShowFrom - instance.settings.showFrom);
      if (opacity > 1.0)
        opacity = 1.0;

      $(instance.ui).css("opacity", opacity);
      $(instance.ui).css("-moz-opacity", opacity);
      $(instance.ui).css("filter", "alpha(opacity = " + (opacity * 100) | 0 + ")");

      $(instance.ui).show();
    }
    else
      $(instance.ui).hide();
  }

  function subscribeEvents(instance) {

    if (!isDefinedAndNotNull(instance))
      throw Errors.ArgumentNullException;

    window.onscroll = function () {

      updateVisibility(instance);
    }

    window.onresize = function () {

      updateVisibility(instance);
    }
  }


  $.backtotop = function (options) {

    this.settings = $.extend({}, defaults, options);

    if (this.settings.showFrom > this.settings.fullShowFrom)
      throw Errors.ArgumentException;

    createUI(this);
    subscribeEvents(this);
    updateVisibility(this);
  };

})(jQuery);