/**
* jQuery Light Window Plugin (dialog boxes)
* 
* @author  Oleg Kashpur
* @email pingvinius@gmail.com
* @twitter pingvinius
* @version 1.00
* 
*/

(function ($) {

  var defaults = {
    windowClass: "lightwindow_window",
    maskClass: "lightwindow_mask",
    closeClass: "lightwindow_close",
    titleClass: "lightwindow_title",
    contentClass: "lightwindow_content",
    closeByClick: false,
    closeByEsc: true,
    title: "Title",
    focusElementSelector: null,
    customWidth: null,
    corruptIdTemplate: "lightbox_corrupted_id_"
  };

  function corruptIds(thiswindow, parentElement, currCorruptedNumber) { // returns corrupted count

    if (!isDefinedAndNotNull(thiswindow) || !isDefinedAndNotNull(parentElement) || !isDefinedAndNotNull(currCorruptedNumber))
      throw Errors.ArgumentNullException;

    $(parentElement).children().each(function (index, value) {

      var oldId = $(value).attr("id");
      if (oldId !== "") {

        var newId = thiswindow.settings.corruptIdTemplate + currCorruptedNumber++;

        thiswindow.corruptedIdsDict[newId] = oldId;

        $(value).attr("id", newId);
      }

      currCorruptedNumber = corruptIds(thiswindow, value, currCorruptedNumber);
    });

    return currCorruptedNumber;
  }

  function restoreIds(thiswindow, parentElement) {

    if (!isDefinedAndNotNull(thiswindow) || !isDefinedAndNotNull(parentElement))
      throw Errors.ArgumentNullException;

    for (var key in thiswindow.corruptedIdsDict) {

      var elem = $(parentElement).find("#" + key).get(0);
      if (isDefinedAndNotNull(elem))
        $(elem).attr("id", thiswindow.corruptedIdsDict[key]);
    }

    thiswindow.corruptedIdsDict = {};
  }

  function showInternal(thiswindow) {

    if (!isDefinedAndNotNull(thiswindow))
      throw Errors.ArgumentNullException;

    thiswindow.corruptedIdsDict = {}; // clear to prevent bugs
    corruptIds(thiswindow, thiswindow.senderElement, 0);

    $(thiswindow.wrapper).fadeIn(1000, function () {

      if (isDefinedAndNotNull(thiswindow.settings.focusElementSelector))
        $(thiswindow.settings.focusElementSelector).focus();
    });
    $(thiswindow.mask).show();

    subscribeEvents(thiswindow);
  }

  function closeInternal(thiswindow) {

    if (!isDefinedAndNotNull(thiswindow))
      throw Errors.ArgumentNullException;

    describeEvents(thiswindow);

    restoreIds(thiswindow, thiswindow.senderElement);

    $(thiswindow.wrapper).hide();
    $(thiswindow.mask).hide();

    $(thiswindow.wrapper).remove();
    $(thiswindow.mask).remove();
  }

  function createWindowUI(thiswindow) {

    if (!isDefinedAndNotNull(thiswindow))
      throw Errors.ArgumentNullException;

    var form = $("form");

    var content = $("<div></div>")
      .addClass(thiswindow.settings.contentClass)
      .append($(thiswindow.senderElement).html());

    var title = $("<div></div>")
      .addClass(thiswindow.settings.titleClass)
      .append("<h1>" + thiswindow.settings.title + "</h1>");

    thiswindow.wrapper = $("<div></div>")
      .addClass(thiswindow.settings.windowClass)
      .append(title)
      .append(content)
      .append($("<div class=\"Clear\"></div>"))
      .appendTo(form);

    thiswindow.mask = $("<div></div>")
      .addClass(thiswindow.settings.maskClass)
      .appendTo(form);

    updateMaskAndBoxLayout(thiswindow);
  }

  function updateMaskAndBoxLayout(thiswindow) {

    if (!isDefinedAndNotNull(thiswindow))
      throw Errors.ArgumentNullException;

    var documentSize = getDocumentSize();
    var documentScrollOffset = getDocumentScrollOffset();

    thiswindow.mask.css("width", documentSize.width)
               .css("height", documentSize.height)
               .css("top", documentScrollOffset.top)
               .css("left", documentScrollOffset.left);

    var windowHeight = $(thiswindow.wrapper).height();
    var windowWidth = isDefinedAndNotNull(thiswindow.settings.customWidth) ? thiswindow.settings.customWidth : $(thiswindow.wrapper).width();

    if (isDefinedAndNotNull(thiswindow.settings.customWidth))
      $(thiswindow.wrapper).css("width", thiswindow.settings.customWidth);
    $(thiswindow.wrapper).css("top", documentSize.height / 2 - windowHeight / 2 + documentScrollOffset.top);
    $(thiswindow.wrapper).css("left", documentSize.width / 2 - windowWidth / 2 + documentScrollOffset.left);
  }

  function subscribeEvents(thiswindow) {

    if (!isDefinedAndNotNull(thiswindow))
      throw Errors.ArgumentNullException;

    window.onscroll = function () {

      updateMaskAndBoxLayout(thiswindow);
    }

    window.onresize = function () {

      updateMaskAndBoxLayout(thiswindow);
    }

    $("." + thiswindow.settings.closeClass).bind("click", function (e) {

      e.preventDefault();
      thiswindow.close();
    });

    if (thiswindow.settings.closeByClick) {

      $(mask).bind("click", function (e) {

        e.preventDefault();
        thiswindow.close();
      });
    }

    if (thiswindow.settings.closeByEsc) {

      $(document).bind("keydown.fb", function (e) {

        if (e.keyCode === Keys.Escape) {

          e.preventDefault();
          thiswindow.close();
        }
      });
    }
  }

  function describeEvents(thiswindow) {

    if (!isDefinedAndNotNull(thiswindow))
      throw Errors.ArgumentNullException;

    $("." + thiswindow.settings.closeClass).unbind("click");

    if (thiswindow.settings.closeByClick)
      $(thiswindow.mask).unbind("click");

    if (thiswindow.settings.closeByEsc)
      $(thiswindow.document).unbind("keydown.fb");
  }


  $.lightwindow = function (sender, options) {

    if (!isDefinedAndNotNull(sender))
      throw Errors.ArgumentNullException;

    var thiswindow = this;
    this.senderElement = sender;
    this.settings = $.extend({}, defaults, options);
    this.corruptedIdsDict = {};
    this.wrapper = null;
    this.mask = null;

    createWindowUI(thiswindow);
    showInternal(thiswindow);


    this.show = function () { showInternal(thiswindow); }

    this.close = function () { closeInternal(thiswindow); };


    return this;
  };


  $.fn.lightwindow = function (options) {

    if (this.length !== 1)
      throw Errors.ArgumentException;

    return new $.lightwindow($(this).get(0), options);
  };

})(jQuery);