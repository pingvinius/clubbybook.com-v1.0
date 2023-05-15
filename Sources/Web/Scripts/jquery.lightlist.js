/**
* jQuery Light List Plugin
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
    showMoreText: "Далее",
    emptyText: "По данному запросу ничего не найдено",
    defaultErrorMessage: "Произошла неизвестная ошибка",
    showMoreItemClass: "LightListShowMore",
    loadingItemClass: "LightListLoading",
    errorMessageItemClass: "LightListErrorMessage",
    errorTypeItemClass: "LightListErrorType",
    errorStackTraceItemClass: "LightListErrorStackTrace",
    emptyItemClass: "LightListEmpty",
    showMoreClick: function () { }
  };

  $.lightlist = function (sender, options) {

    var settings = $.extend({}, defaults, options);

    this.update = function (list, clearOldItems, moreItems) {

      if (!isDefinedAndNotNull(sender))
        throw Errors.ArgumentNullException;

      if (!isDefinedAndNotNull(list))
        list = Array();
      if (!isDefinedAndNotNull(clearOldItems))
        clearOldItems = true;
      if (!isDefinedAndNotNull(moreItems))
        moreItems = false;

      // clearup
      if (performCompleteCleaning || clearOldItems) {

        $(sender).empty();
        performCompleteCleaning = false;
      }
      else {

        $(sender).find("." + settings.showMoreItemClass).remove();
        $(sender).find("." + settings.loadingItemClass).remove();
      }

      // add new items
      if (list.length > 0) {

        $(settings.itemTemplateSelector).tmpl(list).appendTo(sender);

        if (moreItems) {

          var showMoreLink = $("<a></a>").attr("href", "javascript: void(0);").text(settings.showMoreText);
          var loadingImage = $("<img />");
          var showModeListItem = $("<li></li>").addClass(settings.showMoreItemClass).append(showMoreLink).appendTo(sender);
          var loadingListItem = $("<li></li>").addClass(settings.loadingItemClass).append(loadingImage).appendTo(sender);

          $(showMoreLink).bind("click", function () {

            showModeListItem.hide();
            loadingListItem.show();

            settings.showMoreClick();

            return false;
          });
        }
      }
      else if (getItemsCount() === 0) {

        $("<li></li>").addClass(settings.emptyItemClass).text(settings.emptyText).appendTo(sender);
        performCompleteCleaning = true;
      }

      itemsCount += list.length;

      return this;
    };

    this.add = function (newItem) {

      if (!isDefinedAndNotNull(sender))
        throw Errors.ArgumentNullException;

      $(sender).find("." + settings.emptyItemClass).remove();
      $(sender).prepend($(settings.itemTemplateSelector).tmpl([newItem]));
    }

    this.error = function (errorMessage, errorType, stackTrace) {

      if (!isDefinedAndNotNull(sender))
        throw Errors.ArgumentNullException;

      if (!isDefinedAndNotNull(errorMessage))
        errorMessage = settings.defaultErrorMessage;

      performCompleteCleaning = true;

      $(sender).empty();

      $("<li></li>").addClass(settings.errorMessageItemClass).html(errorMessage).appendTo(sender);

      if (errorType)
        $("<li></li>").addClass(settings.errorTypeItemClass).html(errorType).appendTo(sender);

      if (stackTrace)
        $("<li></li>").addClass(settings.errorStackTraceItemClass).html(stackTrace).appendTo(sender);
    }

    this.count = function () {

      return getItemsCount();
    }

    this.reset = function () {

      setItemsCount(0);
    }

    var itemsCount = 0;
    var performCompleteCleaning = true;

    function getItemsCount() {

      return itemsCount;
    }

    function setItemsCount(count) {

      itemsCount = count;
    }

    return this;
  };


  $.fn.lightlist = function (options) {

    if (this.length !== 1)
      throw Errors.ArgumentException;

    return new $.lightlist($(this).get(0), options);
  };

})(jQuery);