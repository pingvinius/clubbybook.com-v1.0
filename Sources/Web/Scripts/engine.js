////////////////////////////////////
// Constants
////////////////////////////////////

var BooksSearchParam =
{
  Users: 0,
  Authors: 1,
  Genres: 2,
  Cities: 3
};

var UsersSearchParam =
{
  Books: 0,
  Cities: 1,
  Offers: 2,
  BookTypes: 3
};

var ServicePaths =
{
  GetNewNotificationCount: "/Services/NotificationsService.asmx/GetNewNotificationCount",
  SendConversationMessage: "/Services/NotificationsService.asmx/SendConversationMessage",
  GetBookInfo: "/Services/BooksService.asmx/GetBookInfo",
  GetBooksInfo: "/Services/BooksService.asmx/GetBooksInfo",
  AddBookToUserLibrary: "/Services/BooksService.asmx/AddBookToLibrary",
  RemoveBookFromUserLibrary: "/Services/BooksService.asmx/RemoveBookFromLibrary",
  ChangeUserBookOffer: "/Services/BooksService.asmx/ChangeUserBookOffer",
  ChangeUserBookType: "/Services/BooksService.asmx/ChangeUserBookType",
  ChangeUserBookStatus: "/Services/BooksService.asmx/ChangeUserBookStatus",
  ChangeUserBookComment: "/Services/BooksService.asmx/ChangeUserBookComment",
  LeaveFeedback: "/Services/GeneralService.asmx/LeaveFeedback",
  ValidateTextBeforePost: "/Services/GeneralService.asmx/ValidateTextBeforePost",
  AddUserUnprovenBook: "/Services/BooksService.asmx/AddUserUnprovenBook",
  AddBookComment: "/Services/BooksService.asmx/AddBookComment",
  RemoveBookComment: "/Services/BooksService.asmx/RemoveBookComment",
  AddAuthorComment: "/Services/AuthorsService.asmx/AddAuthorComment",
  RemoveAuthorComment: "/Services/AuthorsService.asmx/RemoveAuthorComment",
};

var UITextConstants =
{
  ServerFailed: "Сбой сервера.",
  ServerReturnedError: "Сервис вернул ошибку.",
  ListEmpty: "Список пуст."
};

var TooltipTypeTitlesDict = new Array();
TooltipTypeTitlesDict[TooltipType.Information] = "Информация";
TooltipTypeTitlesDict[TooltipType.Error] = "Ошибка";

////////////////////////////////////
// Engine
////////////////////////////////////
var engine = {
  utilities: {
    checkServerResult: function (dataResult) {
      if (!isDefinedAndNotNull(dataResult) || typeof dataResult !== "number")
        return false;

      return dataResult === 0;
    },

    leaveFeedback: function (message, userId) {
      $.sendAjax({
          url: ServicePaths.LeaveFeedback,
          params: {
            "userId": userId,
            "message": message
          },
          success: function (data) {
            if (engine.utilities.checkServerResult(data.d))
              engine.eventHandlers.onServerSuccess("Ваше пожелание будет учтено.");
            else
              engine.eventHandlers.onServerReturnedError();
          },
          error: function (error) {
            engine.eventHandlers.onServerFailed(error);
          }
        });
    },
  },

  eventHandlers: {
    onListError: function (list, error) {
      if (!isDefinedAndNotNull(list) && !isDefinedAndNotNull(error))
        throw Errors.ArgumentNullException;

      list.error(error.message, error.type, error.stackTrace);
    },

    onCarouselError: function (carouselList, error) {
      if (!isDefinedAndNotNull(carouselList) && !isDefinedAndNotNull(error))
        throw Errors.ArgumentNullException;

      carouselList.error(error.message, error.type, error.stackTrace);
    },

    onServerSuccess: function (text) {
      if (!isDefinedAndNotNull(text))
        return;

      $.lighttooltip({
        text: text,
        type: TooltipType.None,
        getTypeToTitleDict: function () {
          return TooltipTypeTitlesDict;
        }
      });
    },

    onServerFailed: function (error) {
      var text = UITextConstants.ServerFailed;
      if (isDefinedAndNotNull(error) && isDefinedAndNotNull(error.message))
        text += " (" + error.message + ")";

      $.lighttooltip({
        text: text,
        type: TooltipType.Error,
        getTypeToTitleDict: function () {
          return TooltipTypeTitlesDict;
        }
      });
    },

    onServerReturnedError: function (additionalText) {
      var text = UITextConstants.ServerReturnedError;
      if (isDefinedAndNotNull(additionalText))
        text += " (" + additionalText + ")";

      $.lighttooltip({
        text: text,
        type: TooltipType.Error,
        getTypeToTitleDict: function () {
          return TooltipTypeTitlesDict;
        }
      });
    },
  },

  notifications: {
    update: function (userId) {
      if (!isDefinedAndNotNull(userId))
        throw Errors.ArgumentNullException;

      function updateInternal(count) {
        if (!isDefinedAndNotNull(count))
          count = 0;

        var title = "Уведомления";
        if (count > 0)
          title = "<strong>" + title + " (" + count + ")</strong>";
        $("#lnkNotifications").html(title);
      }

      updateInternal(0);

      $.sendAjax({
        url: ServicePaths.GetNewNotificationCount,
        async: true,
        params: { "userId": userId },
        success: function (data) {
          if (isDefinedAndNotNull(data.d))
            updateInternal(data.d);
        }
      });
    },

    initialize: function (userId) {
      if (!isDefinedAndNotNull(userId))
        throw Errors.ArgumentNullException;

      engine.notifications.update(userId);

      setInterval(function () {
        engine.notifications.update(userId);
      }, 120000);
    },

    sendMessage: function (fromUserId, toUserId, preSelector, onSuccess) {
      if (!isDefinedAndNotNull(fromUserId) || !isDefinedAndNotNull(toUserId))
        throw Errors.ArgumentNullException;

      var selector = ".SendMessageContent";
      if (isDefinedAndNotNull(preSelector))
        selector = preSelector + " " + selector;

      var lightwindow = $(selector).lightwindow({
        title: "Напишите сообщение",
        focusElementSelector: "tbMessage"
      });
      $("#btnSend").bind("click", function () {
        $.sendAjax({
          url: ServicePaths.SendConversationMessage,
          params: {
            "fromUserId": fromUserId,
            "toUserId": toUserId,
            "message": $("#tbMessage").val()
          },
          success: function (data) {
            if (engine.utilities.checkServerResult(data.d)) {
              engine.eventHandlers.onServerSuccess("Сообщение отправлено.");

              if (isDefinedAndNotNull(onSuccess))
                onSuccess();
            }
            else
              engine.eventHandlers.onServerReturnedError();
          },
          error: function (error) {
            engine.eventHandlers.onServerFailed(error);
          }
        });

        lightwindow.close();
      });
    }
  },

  books: {
    getBookInfo: function (bookId, successFunc) {
      if (!isDefinedAndNotNull(bookId))
        throw Errors.ArgumentNullException;

      $.sendAjax({
        url: ServicePaths.GetBookInfo,
        async: true,
        params: { "bookId": bookId },
        success: function (data) {
          if (isDefinedAndNotNull(successFunc))
            successFunc(data.d);
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    getBooksInfo: function (booksIds, successFunc) {
      if (!isDefinedAndNotNull(booksIds))
        throw Errors.ArgumentNullException;

      $.sendAjax({
        url: ServicePaths.GetBooksInfo,
        async: true,
        params: { "booksIds": booksIds },
        success: function (data) {
          if (isDefinedAndNotNull(successFunc)) {
            if (isDefinedAndNotNull(data.d))
              successFunc(data.d.items);
            else
              successFunc(new Array());
          }
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    addToUserLibrary: function (userId, bookId, successFunc) {
      if (!isDefinedAndNotNull(userId) || !isDefinedAndNotNull(bookId))
        throw Errors.ArgumentNullException;

      $.sendAjax({
        url: ServicePaths.AddBookToUserLibrary,
        async: true,
        params: {
          "userId": userId,
          "bookId": bookId
        },
        success: function (data) {
          if (isDefinedAndNotNull(data.d)) {
            if (data.d === 0) {
              engine.eventHandlers.onServerSuccess("Книга добавлена в личную библиотеку.");

              if (isDefinedAndNotNull(successFunc))
                successFunc();

              return;
            }
            else if (data.d === 1) {
              engine.eventHandlers.onServerSuccess("Книга уже добавлена в Вашей библиотеке.");
              return;
            }
          }

          engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    removeFromUserLibrary: function (userId, bookId, successFunc) {
      if (!isDefinedAndNotNull(userId) || !isDefinedAndNotNull(bookId))
        throw Errors.ArgumentNullException;

      $.sendAjax({
        url: ServicePaths.RemoveBookFromUserLibrary,
        params: {
          "userId": userId,
          "bookId": bookId
        },
        success: function (data) {
          if (engine.utilities.checkServerResult(data.d)) {
            engine.eventHandlers.onServerSuccess("Книга удалена из личной библиотеки.");

            if (isDefinedAndNotNull(successFunc))
              successFunc();
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    changeUserBookOffer: function (userId, bookId, offer, successFunc) {
      $.sendAjax({
        url: ServicePaths.ChangeUserBookOffer,
        params: {
          userId: userId,
          bookId: bookId,
          offer: offer
        },
        success: function (data) {
          if (engine.utilities.checkServerResult(data.d)) {
            engine.eventHandlers.onServerSuccess("Изменения приняты.");

            if (isDefinedAndNotNull(successFunc))
              successFunc();
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    changeUserBookType: function (userId, bookId, type, successFunc) {
      $.sendAjax({
        url: ServicePaths.ChangeUserBookType,
        params: {
          userId: userId,
          bookId: bookId,
          type: type
        },
        success: function (data) {
          if (engine.utilities.checkServerResult(data.d)) {
            engine.eventHandlers.onServerSuccess("Изменения приняты.");

            if (isDefinedAndNotNull(successFunc))
              successFunc();
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    changeUserBookStatus: function (userId, bookId, status, successFunc) {
      $.sendAjax({
        url: ServicePaths.ChangeUserBookStatus,
        params: {
          userId: userId,
          bookId: bookId,
          status: status
        },
        success: function (data) {
          if (engine.utilities.checkServerResult(data.d)) {
            engine.eventHandlers.onServerSuccess("Изменения приняты.");

            if (isDefinedAndNotNull(successFunc))
              successFunc();
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    changeUserBookComment: function (userId, bookId, comment, successFunc) {
      $.sendAjax({
        url: ServicePaths.ChangeUserBookComment,
        params: {
          userId: userId,
          bookId: bookId,
          comment: comment
        },
        success: function (data) {
          if (engine.utilities.checkServerResult(data.d)) {
            engine.eventHandlers.onServerSuccess("Изменения приняты.");

            if (isDefinedAndNotNull(successFunc))
              successFunc();
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    addUserUnprovenBook: function (userId, title, authors, comment, markAsRead, successFunc) {
      $.sendAjax({
        url: ServicePaths.AddUserUnprovenBook,
        params: {
          userId: userId,
          title: title,
          authors: authors,
          comment: comment,
          markAsRead: markAsRead
        },
        success: function (data) {
          if (isDefinedAndNotNull(data.d) && data.d > 0) {
            engine.eventHandlers.onServerSuccess("Книга добавлена на проверку.");

            if (isDefinedAndNotNull(successFunc))
              successFunc(data.d);
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    addBookComment: function (bookId, comment, successFunc) {
      $.sendAjax({
        url: ServicePaths.AddBookComment,
        params: {
          bookId: bookId,
          comment: comment
        },
        success: function (data) {
          if (isDefinedAndNotNull(data.d)) {
            if (isDefinedAndNotNull(successFunc))
              successFunc(data.d);

            engine.eventHandlers.onServerSuccess("Комментарий добавлен!");
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    removeBookComment: function (bookId, commentId, successFunc) {
      $.sendAjax({
        url: ServicePaths.RemoveBookComment,
        params: {
          bookId: bookId,
          commentId: commentId
        },
        success: function (data) {
          if (isDefinedAndNotNull(data.d) && data.d === 0) {
            if (isDefinedAndNotNull(successFunc))
              successFunc(data.d);

            engine.eventHandlers.onServerSuccess("Комментарий удален!");
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },
  },

  authors: {
    addAuthorComment: function (authorId, comment, successFunc) {
      $.sendAjax({
        url: ServicePaths.AddAuthorComment,
        params: {
          authorId: authorId,
          comment: comment
        },
        success: function (data) {
          if (isDefinedAndNotNull(data.d)) {
            if (isDefinedAndNotNull(successFunc))
              successFunc(data.d);

            engine.eventHandlers.onServerSuccess("Комментарий добавлен!");
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },

    removeAuthorComment: function (authorId, commentId, successFunc) {
      $.sendAjax({
        url: ServicePaths.RemoveAuthorComment,
        params: {
          authorId: authorId,
          commentId: commentId
        },
        success: function (data) {
          if (isDefinedAndNotNull(data.d) && data.d === 0) {
            if (isDefinedAndNotNull(successFunc))
              successFunc(data.d);

            engine.eventHandlers.onServerSuccess("Комментарий удален!");
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {
          engine.eventHandlers.onServerFailed(error);
        }
      });
    },
  },
};

////////////////////////////////////
// UIList
////////////////////////////////////
var AdditionalUIListOptionDefaults = {
  listItemSelector: null,
  searchParametersBlockContainerSelector: null,
  totalItemCountContainerSelector: null,
  searchParametersContainerSelector: null,
  emptyListText : null,
};
function UIList(listSelector, itemTemplateSelector, ajaxUrl, getSearchParametersFunc, options) {
  if (!isDefinedAndNotNull(listSelector) || !isDefinedAndNotNull(itemTemplateSelector) || !isDefinedAndNotNull(ajaxUrl) || !isDefinedAndNotNull(getSearchParametersFunc))
    throw Errors.ArgumentNullException;

  var additionalOptions = $.extend({}, AdditionalUIListOptionDefaults, options);
  var lightListOptions = {
    itemTemplateSelector: itemTemplateSelector,
    showMoreClick: function () {
      updateInternal(false);
    }
  };
  if (isDefinedAndNotNull(additionalOptions.emptyListText)) {
    lightListOptions = $.extend({}, lightListOptions, { emptyText: additionalOptions.emptyListText });
  }
  var list = $(listSelector).lightlist(lightListOptions);

  var cachedIDs = new Array();
  var isInitializeLoad = true;

  updateInternal(true);
  isInitializeLoad = false;

  //
  // Private Methods
  //
  function updateInternal(fullUpdate) {
    if (!isDefinedAndNotNull(fullUpdate))
      throw Errors.ArgumentNullException;

    var searchParams = getSearchParametersFunc();
    if (!fullUpdate) {
      $.each(cachedIDs, function (index, value) {
        searchParams.push({ "paramName": "existentId", "paramValue": value });
      });
    }
    var ajaxParams = { "searchParameters": searchParams };

    if (fullUpdate && isDefinedAndNotNull(additionalOptions.searchParametersBlockContainerSelector))
      $(additionalOptions.searchParametersBlockContainerSelector).hide();

    $.sendAjax({
      url: ajaxUrl,
      async: !isInitializeLoad,
      params: ajaxParams,
      success: function (data) {
        if (isDefinedAndNotNull(data.d)) {
          cachedIDs = cachedIDs.concat(data.d.ids);

          list.update(data.d.items, fullUpdate, data.d.moreItems);

          if (fullUpdate && isDefinedAndNotNull(additionalOptions.totalItemCountContainerSelector)) {
            if (data.d.totalItemCountString !== "") {
              $(additionalOptions.totalItemCountContainerSelector).text(data.d.totalItemCountString);
              $(additionalOptions.totalItemCountContainerSelector).show();

              if (isDefinedAndNotNull(additionalOptions.searchParametersBlockContainerSelector))
                $(additionalOptions.searchParametersBlockContainerSelector).show();
            }
            else {
              $(additionalOptions.totalItemCountContainerSelector).text("");
              $(additionalOptions.totalItemCountContainerSelector).hide();
            }
          }

          if (fullUpdate && isDefinedAndNotNull(additionalOptions.searchParametersContainerSelector)) {
            if (data.d.searchParametersString !== "") {
              $(additionalOptions.searchParametersContainerSelector).text(data.d.searchParametersString);
              $(additionalOptions.searchParametersContainerSelector).show();

              if (isDefinedAndNotNull(additionalOptions.searchParametersBlockContainerSelector))
                $(additionalOptions.searchParametersBlockContainerSelector).show();
            }
            else {
              $(additionalOptions.searchParametersContainerSelector).text("");
              $(additionalOptions.searchParametersContainerSelector).hide();
            }
          }
        }
        else {
          // TODO: check this place. search item count should be hid here
          cachedIDs = new Array();

          list.update();
        }

        updateActions();
      },
      error: function (error) {
        engine.eventHandlers.onListError(list, error);
      }
    });
  }

  function updateActions() {
    if (isDefinedAndNotNull(additionalOptions.listItemSelector)) {
      $(additionalOptions.listItemSelector).hover(
        function (event) {
          $(this).find(".Actions").show();
        },
        function (event) {
          $(this).find(".Actions").hide();
        }
      );
    }
  }

  //
  // Public Methods
  //
  this.update = function () {
    list.reset();
    cachedIDs = new Array();

    updateInternal(true);
  }

  this.add = function (newItem) {
    cachedIDs.push(newItem);
    list.add(newItem);
    updateActions();
  }
};

////////////////////////////////////
// UICarousel
////////////////////////////////////
function UICarousel(selector, itemTemplateSelector, ajaxUrl, getAjaxMethodParamsFunc, emptyText) {
  if (!isDefinedAndNotNull(selector) || !isDefinedAndNotNull(itemTemplateSelector) || !isDefinedAndNotNull(ajaxUrl) || !isDefinedAndNotNull(getAjaxMethodParamsFunc))
    throw Errors.ArgumentNullException;

  var carouselSettings = { itemTemplateSelector: itemTemplateSelector };
  if (isDefinedAndNotNull(emptyText))
    $.extend(carouselSettings, { emptyText: emptyText });

  var carousel = $(selector).lightcarousel(carouselSettings);

  var isInitializeLoad = true;
  updateInternal();
  isInitializeLoad = false;

  function updateInternal(async) {
    $.sendAjax({
      url: ajaxUrl,
      async: isDefinedAndNotNull(async) ? async : true, //!isInitializeLoad,
      params: getAjaxMethodParamsFunc(),
      success: function (data) {
        if (isDefinedAndNotNull(data.d))
          carousel.update(data.d.items);
        else
          carousel.update();
      },
      error: function (error) {
        engine.eventHandlers.onCarouselError(carousel, error);
      }
    });
  }

  //
  // Public Methods
  //
  this.update = function (async) {
    updateInternal(async);
  }

  this.addItem = function (newItem) {
    if (!isDefinedAndNotNull(newItem))
      throw Errors.ArgumentNullException;

    carousel.add([newItem]);
  }
};