<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Books.aspx.cs" Inherits="ClubbyBook.Web.Common.Books"  %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>
<%@ Import Namespace="ClubbyBook.Common" %>
<%@ Import Namespace="ClubbyBook.Common.Enums" %>
<%@ Import Namespace="ClubbyBook.Common.Utilities" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>
<%@ Import Namespace="ClubbyBook.UI" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/lightlist.css") %>' type="text/css" />
  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/token-input.css") %>' type="text/css" />
  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/backtotop.css") %>' type="text/css" />

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tokeninput.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightlist.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tmpl.min.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.backtotop.js") %>'></script>

  <script id="bookTemplate" type="text/x-jquery-tmpl">

  <li id="book_${id}" class="DataListItem">
    <div class="Image">
      <a class="ImageLink" href="${viewBookLink}">
        <img src="${coverPath}" title="${seoImageAlt}" alt="${seoImageAlt}" />
      </a>
    </div>
    <div class="Data">
      <ul class="List">
        <li class="Item Title">
          <a href="${viewBookLink}">
            <strong>${title}</strong>{{if collection}} (сборник){{/if}}
          </a>
        </li>
        <li class="Item Label">
          <div class="BookAuthorsList">
            <ul>
              {{each authors}}
                <li>
                  {{if confirmed}}<a href="${viewAuthorLink}">{{/if}}${fullName}{{if confirmed}}</a>{{/if}}{{if $index !== authors.length - 1}},&nbsp;{{/if}}
                </li>
              {{/each}}
            </ul>
          </div>
        </li>
        <li class="Item Label">
          <div class="BookGenresList">
            <ul>
              {{each genres}}
                <li>
                  <a href="${viewGenreBooksLink}">${name}</a>{{if $index !== genres.length - 1}},&nbsp;{{/if}}
                </li>
              {{/each}}
            </ul>
          </div>
        </li>
        <li class="Item Description">
          <p>${restrictedDescription}</p>
        </li>
      </ul>
    </div>
    <% if (UserManagement.IsAuthenticated) { %>
    <div class="Markers">
      <ul>
        {{if containsInOtherUserLibraryFromSameCity }}
        <li>
          <a class="ImageLink" href="javascript: void(0)" onclick="goToUsersFromSameCityList(${id}); return false;">
            <img title="Книга ${title} есть у других пользователей с твоего города. Нажмите для того что бы перейти." alt="Книга ${title} есть у других пользователей с твоего города. Нажмите для того что бы перейти." src='<%= ResolveUrl("~/Images/Common/ContainsInOtherUserLibrary.png") %>' />
          </a>
        </li>
        {{/if}}
      </ul>
    </div>
    <% } %>
    <div class="Actions Hidden">
      <% if (UserManagement.IsAuthenticated) { %>
      <ul>
        <li class="AddBookToLibrary" {{if containsInUserLibrary }}style="display: none;"{{/if}}>
          <a class="ImageLink" href="javascript: void(0)" onclick="addBookToLibrary(${id}); return false;">
            <img title="Добавить в мою библиотеку" alt="Добавить в мою библиотеку" src='<%= ResolveUrl("~/Images/Common/AddToLibrary.png") %>' />
          </a>
        </li>
        <li class="RemoveBookFromLibrary" {{if !containsInUserLibrary }}style="display: none;"{{/if}}>
          <a class="ImageLink" href="javascript: void(0)" onclick="removeBookFromLibrary(${id}); return false;">
            <img title="Удалить из моей библиотеки" alt="Удалить из моей библиотеки" src='<%= ResolveUrl("~/Images/Common/RemoveFromLibrary.png") %>' />
          </a>
        </li>
        <% if (AccessManagement.CanEditBook) { %>
        <li class="Edit">
          <a class="ImageLink" href="${editBookLink}">
            <img title="Редактировать" alt="Редактировать" src='<%= ResolveUrl("~/Images/Common/Edit.png") %>' />
          </a>
        </li>
        <% } %>
        <% if (AccessManagement.CanRemoveBook) { %>
        <li class="Remove">
          <a class="ImageLink" href="javascript: void(0)" onclick="removeBook(${id}); return false;">
            <img title="Удалить книгу" alt="Удалить книгу" src='<%= ResolveUrl("~/Images/Common/Remove.png") %>' />
          </a>
        </li>
        <% } %>
      </ul>
      <% } %>
    </div>
    <div class="Clear"></div>
  </li>

  </script>

  <script language="javascript" type="text/javascript">

    var booksList = null;

    $(document).ready(function() {

      $.backtotop({
        text: "Вверх"
      });

      booksList = new UIList(
        "#booksList", 
        "#bookTemplate", 
        '<%= ResolveUrl("~/Services/BooksService.asmx/GetBooks") %>',
        getSearchParameters,
        {
          listItemSelector: ".DataListItem",
          searchParametersBlockContainerSelector: ".SearchParametersBlock",
          totalItemCountContainerSelector: "#lbTotalItemCount",
          searchParametersContainerSelector: "#lbSearchParameters"
        }
      );

      $("#tbSearch").bind("keypress", function(e) {

        var code = e.keyCode || e.which;
        if(code === Keys.Enter) {

          booksList.update();
          e.preventDefault();
        }
      });

      initializeCitySearch();
      initializeAuthorsSearch();
      initializeGenresSearch();
    });


    function initializeCitySearch() {

      var cityId = <%= SearchCityId %>;
      if (cityId === -1) {

        $("#tbCities").removeAttr("disabled");

        $("#tbCities").tokenInput('<%= ResolveUrl("~/Services/LocationService.asmx/GetAutoCompleteCities") %>',
        {
          hintText: "Введите город",
          noResultsText: "Такого города нет в базе данных",
          searchingText: "Поиск города...",
          preventDuplicates: true,
          queryParam: "prefixText",
          prePopulate: [],
          onAdd: function (data) {

            if (data) {

              var ids = $("#hfCities").val().toArrayList();
              ids.push(data.value);
              $("#hfCities").val(ids.join(","));
            }
          },
          onDelete: function (data) {

            if (data) {

              var ids = $("#hfCities").val().toArrayList();
              var index = ids.indexOf(data.value);
              if (index >= 0)
                ids.splice(index, 1);
              $("#hfCities").val(ids.join(","));
            }
          }
        });
      }
      else
        $("#tbCities").attr("disabled", "disabled");
    }

    function initializeAuthorsSearch() {

      var authorId = <%= SearchAuthorId %>;
      if (authorId === -1) {

        $("#tbAuthors").removeAttr("disabled");

        $("#tbAuthors").tokenInput('<%= ResolveUrl("~/Services/AuthorsService.asmx/GetAutoCompleteAuthors") %>',
        {
          hintText: "Введите автора",
          noResultsText: "Такого автора нет в базе данных",
          searchingText: "Поиск автора...",
          preventDuplicates: true,
          queryParam: "prefixText",
          prePopulate: [],
          onAdd: function (data) {

            if (data) {

              var ids = $("#hfAuthors").val().toArrayList();
              ids.push(data.value);
              $("#hfAuthors").val(ids.join(","));
            }
          },
          onDelete: function (data) {

            if (data) {

              var ids = $("#hfAuthors").val().toArrayList();
              var index = ids.indexOf(data.value);
              if (index >= 0)
                ids.splice(index, 1);
              $("#hfAuthors").val(ids.join(","));
            }
          }
        });
      }
      else
        $("#tbAuthors").attr("disabled", "disabled");
    }

    function initializeGenresSearch() {

      var genreId = <%= SearchGenreId %>;
      if (genreId === -1) {

        $("#ddlGenres").removeAttr("disabled");

        $.sendAjax({
          url: '<%= ResolveUrl("~/Services/BooksService.asmx/GetGenres") %>',
          async: true,
          params: {},
          success: function (data) {

            if (isDefinedAndNotNull(data.d)) {

              $.each(data.d, function (index, value) {

                $("#ddlGenres").append($("<option></option>").attr("value", value.value).text(value.text));
              });

              $("#ddlGenres").selectedIndex = 0;
            }
          },
          error: function(error) { }
        });
      }
      else
        $("#ddlGenres").attr("disabled", "disabled");
    }

    function getSearchParameters() {

      var searchParameters = [
        { "paramName": "searchText", "paramValue": $("#tbSearch").val() }
      ];

      var offer = <%= (int)UserBookOfferType.None %>;
      $("#lstOffer input[type=checkbox]:checked").each(function() {
        offer |= $(this).val();
      });
      searchParameters.push({ "paramName": "offer", "paramValue": offer });

      var bookType = <%= (int)UserBookOfferType.None %>;
      $("#lstBookType input[type=checkbox]:checked").each(function() {
        bookType |= $(this).val();
      });
      searchParameters.push({ "paramName": "bookType", "paramValue": bookType });

      searchParameters.push({ "paramName": "bookcollectionsornot", "paramValue": parseInt($("#dllBookCollectionsOrNot").val(), 10) });

      <% if (UserManagement.IsEditorAuthenticated || UserManagement.IsAdminAuthenticated) { %>
      searchParameters.push({ "paramName": "bookprogress", "paramValue": parseInt($("#ddlBookProgress").val(), 10) });
      <% } %>

      var ids;

      ids = getSearchParameterIds(BooksSearchParam.Users);
      $.each(ids, function (index, value) {
        searchParameters.push({ "paramName": "userId", "paramValue": value });
      });

      ids = getSearchParameterIds(BooksSearchParam.Cities);
      $.each(ids, function (index, value) {
        searchParameters.push({ "paramName": "cityId", "paramValue": value });
      });

      ids = getSearchParameterIds(BooksSearchParam.Authors);
      $.each(ids, function (index, value) {
        searchParameters.push({ "paramName": "authorId", "paramValue": value });
      });

      ids = getSearchParameterIds(BooksSearchParam.Genres);
      $.each(ids, function (index, value) {
        searchParameters.push({ "paramName": "genreId", "paramValue": value });
      });

      return searchParameters;
    }


    function getSearchParameterIds(parameter) {

      var idsArray = new Array();

      switch (parameter) {

        case BooksSearchParam.Authors:

          var authorIdFromRequest = <%= SearchAuthorId %>;
          var authorsIdsChoosed = $("#hfAuthors").val().toArrayList();
          if (authorIdFromRequest !== -1 && authorsIdsChoosed.indexOf(authorIdFromRequest) === -1)
            authorsIdsChoosed.push(authorIdFromRequest);
          $.each(authorsIdsChoosed, function (index, value) {
            idsArray.push(parseInt(value, 10));
          });

          break;

        case BooksSearchParam.Genres:

          if (<%= SearchGenreId %> !== -1)
            idsArray.push(<%= SearchGenreId %>);
          else if($("#ddlGenres").val() != -1)
            idsArray.push(parseInt($("#ddlGenres").val(), 10));

          break;

        case BooksSearchParam.Users:

          if (<%= SearchUserId %> !== -1) 
            idsArray.push(<%= SearchUserId %>);

          break;

        case BooksSearchParam.Cities:

          var cityIdFromRequest = <%= SearchCityId %>;
          var citiesIdsChoosed = $("#hfCities").val().toArrayList();
          if (cityIdFromRequest !== -1 && citiesIdsChoosed.indexOf(cityIdFromRequest) === -1)
            citiesIdsChoosed.push(cityIdFromRequest);
          $.each(citiesIdsChoosed, function (index, value) {
            idsArray.push(parseInt(value, 10));
          });

          break;
      }

      return idsArray;
    }


    function expandAdvancedSearchBlock() {

      if (isVisible($(".SearchBlock .Advanced")))
        $(".SearchBlock .Advanced").slideUp(500);
      else
        $(".SearchBlock .Advanced").slideDown(500);
    }

    <% if (UserManagement.IsAuthenticated) { %>

    function addBookToLibrary(id) {

      engine.books.addToUserLibrary(<%= UserManagement.CurrentUser.Id %>, id, function () {

        $("li#book_" + id + " div.Actions ul li.AddBookToLibrary").hide();
        $("li#book_" + id + " div.Actions ul li.RemoveBookFromLibrary").show();
      });
    }

    function removeBookFromLibrary(id) {

      engine.books.removeFromUserLibrary(<%= UserManagement.CurrentUser.Id %>, id, function () {

        $("li#book_" + id + " div.Actions ul li.RemoveBookFromLibrary").hide();
        $("li#book_" + id + " div.Actions ul li.AddBookToLibrary").show();
      });
    }

    <% if (UserManagement.IsAuthenticated) { %>
    function goToUsersFromSameCityList(bookId) {

      if (!isDefinedAndNotNull(bookId) || bookId <= 0)
        throw Errors.ArgumentException;

      engine.books.getBookInfo(bookId, function(item) {

        if (isDefinedAndNotNull(item)) {

          var avaliableOffers = [];

          if (isDefinedAndNotNull(item.anyOffer) && item.anyOffer.partialCount > 0) {

            avaliableOffers.push({ 
              element: "a#cuobAnyOffer",
              link: item.anyOffer.partialLink,
              count: item.anyOffer.partialCount
            });
          }

          if (isDefinedAndNotNull(item.sellOffer) && item.sellOffer.partialCount > 0) {

            avaliableOffers.push({ 
              element: "a#cuobSellOffer",
              link: item.sellOffer.partialLink,
              count: item.sellOffer.partialCount
            });
          }

          if (isDefinedAndNotNull(item.buyOffer) && item.buyOffer.partialCount > 0) {

            avaliableOffers.push({ 
              element: "a#cuobBuyOffer",
              link: item.buyOffer.partialLink,
              count: item.buyOffer.partialCount
            });
          }

          if (isDefinedAndNotNull(item.barterOffer) && item.barterOffer.partialCount > 0) {

            avaliableOffers.push({ 
              element: "a#cuobBarterOffer",
              link: item.barterOffer.partialLink,
              count: item.barterOffer.partialCount
            });
          }

          if (isDefinedAndNotNull(item.willGiveReadOffer) && item.willGiveReadOffer.partialCount > 0) {

            avaliableOffers.push({ 
              element: "a#cuobWillGiveReadOffer",
              link: item.willGiveReadOffer.partialLink,
              count: item.willGiveReadOffer.partialCount
            });
          }

          if (isDefinedAndNotNull(item.willGrantGratisOffer) && item.willGrantGratisOffer.partialCount > 0) {

            avaliableOffers.push({ 
              element: "a#cuobWillGrantGratisOffer",
              link: item.willGrantGratisOffer.partialLink,
              count: item.willGrantGratisOffer.partialCount
            });
          }

          if (avaliableOffers.length === 1) {

            redirectToPage(avaliableOffers[0].link);
          }
          else if (avaliableOffers.length > 1) {

            var lightwindow = $(".ChooseUsersOffersContent").lightwindow({
              title: "Выберите интересующее предложение"
            });

            $.each(avaliableOffers, function (index, value) {

              var imgElement = $(value.element).find("img").get(0);
              $(value.element).attr("href", value.link);
              $(imgElement).attr("alt", $(imgElement).attr("alt") + " (" + value.count + ")");
              $(imgElement).attr("title", $(imgElement).attr("title") + " (" + value.count + ")");
              $(value.element).parent().removeClass("Hidden");

              $(value.element).bind("click", function(e) {

                lightwindow.close();
                $(value.element).unbind("click");
                return true;
              });
            });
          }
        }
      });
    }
    <% } %>

    <% if (AccessManagement.CanRemoveBook) { %>
    function removeBook(id) {

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/BooksService.asmx/RemoveBook") %>',
        params: { "bookId": id },
        success: function (data) {

          if (engine.utilities.checkServerResult(data.d)) {

            var bookListItem = $("li#book_" + id);

            $(bookListItem).slideUp(500, function() {

              $(bookListItem).remove();

              engine.eventHandlers.onServerSuccess("Книга удалена.");
            });
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {

          engine.eventHandlers.onServerFailed(error);
        }
      });
    }
    <% } %>

    <% } %>

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Книги" />

  <div class="SearchBlock">
    <div class="Simple">
      <input type="text" id="tbSearch" class="TextBox Find" 
        onfocus='javascript: $("span.Prompt").hide();'
        onblur='javascript: $("#tbSearch").val() === "" ? $("span.Prompt").show() : $("span.Prompt").hide();' />
      <span class="Prompt">Поиск</span>
      <div class="Cancel">
        <a class="ImageLink" href="javascript: void(0)" onclick='$("#tbSearch").val(""); $("#tbSearch").focus();'>
          <img src="<%= ResolveUrl("~/Images/Common/Cancel.png") %>" alt="Очистить ввод" title="Очистить ввод" />
        </a>
      </div>
    </div>
    <div class="Advanced">
      <ul class="FilterList">
        <li class="FilterItem">
          <div class="Label">Города:</div>
          <div class="Field">
            <input type="text" id="tbCities" class="TextBox" />
            <input type="hidden" id="hfCities" />
          </div>
          <div class="Clear"></div>
        </li>
        <li class="FilterItem">
          <div class="Label">Авторы:</div>
          <div class="Field">
            <input type="text" id="tbAuthors" class="TextBox" />
            <input type="hidden" id="hfAuthors" />
          </div>
          <div class="Clear"></div>
        </li>
        <li class="FilterItem">
          <div class="Label">Жанр:</div>
          <div class="Field">
            <select id="ddlGenres" class="DropDownList">
              <option value="-1"><%= UIUtilities.SelectString %></option>
            </select>
          </div>
          <div class="Clear"></div>
        </li>
        <li class="FilterItem">
          <div class="Label">Сборник:</div>
          <div class="Field">
            <select id="dllBookCollectionsOrNot" class="DropDownList">
              <option value="<%= (int)BookCollectionsOrNotType.DoesnotMatter %>" selected="selected"><%= AttributeHelper.GetEnumValueDescription(BookCollectionsOrNotType.DoesnotMatter)%></option>
              <option value="<%= (int)BookCollectionsOrNotType.CollectionsOnly %>"><%= AttributeHelper.GetEnumValueDescription(BookCollectionsOrNotType.CollectionsOnly)%></option>
              <option value="<%= (int)BookCollectionsOrNotType.ExcludeCollections %>"><%= AttributeHelper.GetEnumValueDescription(BookCollectionsOrNotType.ExcludeCollections)%></option>
            </select>
          </div>
          <div class="Clear"></div>
        </li>
        <% if (UserManagement.IsEditorAuthenticated || UserManagement.IsAdminAuthenticated) { %>
        <li class="FilterItem">
          <div class="Label">Прогресс:</div>
          <div class="Field">
            <select id="ddlBookProgress" class="DropDownList">
              <option value="<%= (int)BookProgressType.Proven %>"><%= AttributeHelper.GetEnumValueDescription(BookProgressType.Proven)%></option>
              <option value="<%= (int)BookProgressType.All %>" selected="selected"><%= AttributeHelper.GetEnumValueDescription(BookProgressType.All)%></option>
              <option value="<%= (int)BookProgressType.Unproven %>"><%= AttributeHelper.GetEnumValueDescription(BookProgressType.Unproven)%></option>
            </select>
          </div>
          <div class="Clear"></div>
        </li>
        <% } %>
        <li class="FilterItem">
          <div class="Label">Хочу:</div>
          <div class="Field">
            <ul id="lstOffer">
              <li class="CheckBox">
                <input id="chkOfferBuy" type="checkbox" value="<%= (int)UserBookOfferType.Sell %>" />
                <label for="chkOfferBuy">Купить</label>
              </li>
              <li class="CheckBox">
                <input id="chkOfferSell" type="checkbox" value="<%= (int)UserBookOfferType.Buy %>" />
                <label for="chkOfferSell">Продать</label>
              </li>
              <li class="CheckBox">
                <input id="chkOfferBarter" type="checkbox" value="<%= (int)UserBookOfferType.Barter %>" />
                <label for="chkOfferBarter">Обменять</label>
              </li>
              <li class="CheckBox">
                <input id="chkOfferWillGiveRead" type="checkbox" value="<%= (int)UserBookOfferType.WillGiveRead %>" />
                <label for="chkOfferWillGiveRead">Взять прочесть</label>
              </li>
              <li class="CheckBox">
                <input id="chkOfferWillGrantGratis" type="checkbox" value="<%= (int)UserBookOfferType.WillGrantGratis %>" />
                <label for="chkOfferWillGrantGratis">Получить подарок</label>
              </li>
            </ul>
            <ul id="lstBookType">
              <li class="CheckBox">
                <input id="chkBookTypePaperBook" type="checkbox" value="<%= (int)UserBookType.PaperBook %>" />
                <label for="chkBookTypePaperBook">Печатную книгу</label>
              </li>
              <li class="CheckBox">
                <input id="chkBookTypeEBook" type="checkbox" value="<%= (int)UserBookType.EBook %>" />
                <label for="chkBookTypeEBook">Электронную книгу</label>
              </li>
              <li class="CheckBox">
                <input id="chkBookTypeAudiobook" type="checkbox" value="<%= (int)UserBookType.Audiobook %>" />
                <label for="chkBookTypeAudiobook">Аудиокнигу</label>
              </li>
            </ul>
          </div>
          <div class="Clear"></div>
        </li>
        <li class="Button">
          <a class="Button" href="javascript: void(0)" onclick="booksList.update(); return false;">Поиск</a>
        </li>
      </ul>
    </div>
  </div>

  <div class="SearchParametersBlock">
    <p id="lbTotalItemCount"></p>
    <p id="lbSearchParameters"></p>
  </div>

  <div class="ContentDataList">
    <ul id="booksList"></ul>
  </div>

  <% if (UserManagement.IsAuthenticated) { %>
  <div class="ChooseUsersOffersContent Hidden">
    <div class="ChooseUsersOffersBlock">
      <ul>
        <li class="Hidden">
          <a id="cuobAnyOffer" class="ImageLink" href="javascript: void(0)">
            <img src='<%= ResolveUrl("~/Images/Common/AnyOffer.png") %>' alt="Любое" title="Любое" />
          </a>
        </li>
        <li class="Hidden">
          <a id="cuobSellOffer" class="ImageLink" href="javascript: void(0)">
            <img src='<%= ResolveUrl("~/Images/Common/Sell.png") %>' alt="Продажа" title="Продажа" />
          </a>
        </li>
        <li class="Hidden">
          <a id="cuobBuyOffer" class="ImageLink" href="javascript: void(0)">
            <img src='<%= ResolveUrl("~/Images/Common/Buy.png") %>' alt="Купля" title="Купля" />
          </a>
        </li>
        <li class="Hidden">
          <a id="cuobBarterOffer" class="ImageLink" href="javascript: void(0)">
            <img src='<%= ResolveUrl("~/Images/Common/Barter.png") %>' alt="Обмен" title="Обмен" />
          </a>
        </li>
        <li class="Hidden">
          <a id="cuobWillGiveReadOffer" class="ImageLink" href="javascript: void(0)">
            <img src='<%= ResolveUrl("~/Images/Common/WillGiveRead.png") %>' alt="Дадут прочесть" title="Дадут прочесть" />
          </a>
        </li>
        <li class="Hidden">
          <a id="cuobWillGrantGratisOffer" class="ImageLink" href="javascript: void(0)">
            <img src='<%= ResolveUrl("~/Images/Common/WillGrantGratis.png") %>' alt="Подарок" title="Подарок" />
          </a>
        </li>
      </ul>
    </div>
    <div class="Clear"></div>
    <div class="Buttons">
      <input type="button" class="Button lightwindow_close" title="Закрыть" value="Закрыть" />
    </div>
  </div>
  <% } %>

</asp:Content>
