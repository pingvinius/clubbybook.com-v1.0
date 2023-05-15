/**
* jQuery Light Tooltip Plugin
* 
* @author  Oleg Kashpur
* @email pingvinius@gmail.com
* @twitter pingvinius
* @version 1.00
* 
*/

var TooltipType = {
  None: 0,
  Information: 1,
  Error: 2
};

(function ($) {

  var defaults = {
    text: "jQuery Light Tooltip Plugin",
    timeout: 2000,
    type: TooltipType.None,
    tooltipClass: "lighttooltip",
    titleClass: "lighttooltip_title",
    contentClass: "lighttooltip_content",
    customWidth: 400,
    getTypeToTitleDict: function() {

      var arr = new Array();

      arr[TooltipType.None] = "";
      arr[TooltipType.Information] = "Information";
      arr[TooltipType.Error] = "Error";

      return arr;
    }
  };

  function TooltipItem(settings) {

    var tooltipUI = null;

    createTooltipUI();
    showInternal();

    function showInternal() {

      if (isVisible(tooltipUI))
        throw Errors.InvalidOperationException;

      updateTooltipLayout();

      $(tooltipUI).fadeIn(500);

      subscribeEvents();
      setupTimer();
    }

    function closeInternal() {

      if (tooltipUI === null)
        return;

      describeEvents();

      $(tooltipUI).fadeOut(1000);
      $(tooltipUI).remove();

      tooltipUI = null;
    }

    function createTooltipUI() {

      tooltipUI = $("<div></div>").addClass(settings.tooltipClass)

      if (settings.type !== TooltipType.None) {

        tooltipUI.append($("<div></div>")
          .addClass(settings.titleClass)
          .append("<h1>" + settings.getTypeToTitleDict()[settings.type] + "</h1>"));
      }

      tooltipUI.append($("<div></div>")
        .addClass(settings.contentClass)
        .html(settings.text))
        .appendTo($("form"));
    }

    function updateTooltipLayout() {

      var documentSize = getDocumentSize();
      var documentScrollOffset = getDocumentScrollOffset();

      var windowHeight = $(tooltipUI).height();
      var windowWidth = isDefinedAndNotNull(settings.customWidth) ? settings.customWidth : $(tooltipUI).width();

      var showedTooltips = $("div." + settings.tooltipClass);
      var lastShowedTooltip = showedTooltips.length > 1 ? showedTooltips.get(showedTooltips.length - 2) : null;

      var topOffset = lastShowedTooltip !== null ? 
        lastShowedTooltip.offsetTop + lastShowedTooltip.offsetHeight + 2 : // based on last showed tooltip
        documentSize.height / 4 + documentScrollOffset.top; // based on the display height

      if (isDefinedAndNotNull(settings.customWidth))
        $(tooltipUI).css("width", settings.customWidth);

      $(tooltipUI).css("top", topOffset);
      $(tooltipUI).css("left", documentSize.width / 2 - windowWidth / 2 + documentScrollOffset.left);
    }

    function setupTimer() {

      var timer = setTimeout(function () { 

        closeInternal(); 
        clearTimeout(timer);

      }, settings.timeout);
    }

    function subscribeEvents() {

      $(window).bind("scroll", function () {
        updateTooltipLayout();
      });

      $(window).bind("resize", function () {
        updateTooltipLayout();
      });

      $("." + settings.tooltipClass).bind("click", function (e) {

        e.preventDefault();
        closeInternal();
      });
    }

    function describeEvents() {

      $(window).unbind("scroll");
      $(window).unbind("resize");

      $("." + settings.tooltipClass).unbind("click");
    }
  }

  $.lighttooltip = function (options) {

    return new TooltipItem($.extend({}, defaults, options));
  };

})(jQuery);