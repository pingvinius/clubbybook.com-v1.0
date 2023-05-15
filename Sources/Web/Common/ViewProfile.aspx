<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ViewProfile.aspx.cs" Inherits="ClubbyBook.Web.Common.ViewProfile" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="ClubbyBook.Common" %>
<%@ Import Namespace="ClubbyBook.Common.Enums" %>
<%@ Import Namespace="ClubbyBook.Common.Utilities" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>
<%@ Import Namespace="ClubbyBook.UI" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/InfoMessageControl.ascx" TagName="InfoMessageControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/lightcarousel.css") %>' type="text/css" />
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tmpl.min.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.caroufredsel.min.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightcarousel.js") %>'></script>

  <script id="bookTemplate" type="text/x-jquery-tmpl">
    <li>
      <a class="ImageLink" href="${viewBookLink}">
        <img src="${coverPath}" title="${authorsAndTitle}" alt="${authorsAndTitle}" width="80" height="125" />
      </a>
    </li>
  </script>

  <script id="commonScript" type="text/javascript" language="javascript">

    var bookTypeFilters = [
      { "paramName": "bookType", "paramValue": <%= (int)UserBookType.PaperBook %>},
      { "paramName": "bookType", "paramValue": <%= (int)UserBookType.EBook %>},
      { "paramName": "bookType", "paramValue": <%= (int)UserBookType.Audiobook %>}
    ];

    var bookOfferFilters = [
      { "paramName": "offer", "paramValue": <%= (int)UserBookOfferType.Sell %>},
      { "paramName": "offer", "paramValue": <%= (int)UserBookOfferType.Buy %>},
      { "paramName": "offer", "paramValue": <%= (int)UserBookOfferType.Barter %>},
      { "paramName": "offer", "paramValue": <%= (int)UserBookOfferType.WillGiveRead %>},
      { "paramName": "offer", "paramValue": <%= (int)UserBookOfferType.WillGrantGratis %>}
    ];

    var bookStatusFilters = [
      { "paramName": "status", "paramValue": <%= (int)UserBookStatusType.AlreadyRead %>},
      { "paramName": "status", "paramValue": <%= (int)UserBookStatusType.ReadingNow %>},
      { "paramName": "status", "paramValue": <%= (int)UserBookStatusType.WantToRead %>},
    ];

    var currentBookTypeFilterIndex = -1;
    var currentBookOfferFilterIndex = -1;
    var currentBookStatusFilterIndex = -1;

    var booksCarousel = null;

    $(document).ready(function () {

      booksCarousel = new UICarousel(
        "#profileBooks", 
        "#bookTemplate", 
        '<%= ResolveUrl("~/Services/BooksService.asmx/GetBooks") %>',
        function () {

          var filter = [
            { "paramName": "userId", "paramValue": <%= Entity.UserId %>},
            { "paramName": "returnall", "paramValue": 1}
          ];
          if (validateFilterIndexes(currentBookTypeFilterIndex, currentBookOfferFilterIndex, currentBookStatusFilterIndex)) {

            if (currentBookTypeFilterIndex !== -1)
              filter.push(bookTypeFilters[currentBookTypeFilterIndex]);
            if (currentBookOfferFilterIndex !== -1)
              filter.push(bookOfferFilters[currentBookOfferFilterIndex]);
            if (currentBookStatusFilterIndex !== -1)
              filter.push(bookStatusFilters[currentBookStatusFilterIndex]);
          }

          return { "searchParameters": filter };
        },
        <% if (IsThatYou) { %>
          "В Вашей библиотеке нет книг соответствующих указаному фильтру."
        <% } else { %>
          "В библиотеке пользователя нет книг соответствующих указаному фильтру." 
        <% } %>
      );
    });

    function changeBooksFilter(bookTypeIndex, bookOfferIndex, bookStatusIndex, updateImmediately) {

      if (!isDefined(bookTypeIndex) || !isDefined(bookOfferIndex) || !isDefined(bookStatusIndex))
        throw Errors.ArgumentNullException;

      if (validateFilterIndexes(bookTypeIndex, bookOfferIndex, bookStatusIndex)) {

        if (bookTypeIndex !== null)
          currentBookTypeFilterIndex = bookTypeIndex;
        if (bookOfferIndex !== null)
          currentBookOfferFilterIndex = bookOfferIndex;
        if (bookStatusIndex !== null)
          currentBookStatusFilterIndex = bookStatusIndex;

        $(".ProfileBooksFilter span").removeClass("Current");

        if (currentBookTypeFilterIndex !== -1)
          $($(".ProfileBooksFilter span.BookType").get(currentBookTypeFilterIndex)).addClass("Current");
        if (currentBookOfferFilterIndex !== -1)
          $($(".ProfileBooksFilter span.BookOffer").get(currentBookOfferFilterIndex)).addClass("Current");
        if (currentBookStatusFilterIndex !== -1)
          $($(".ProfileBooksFilter span.BookStatus").get(currentBookStatusFilterIndex)).addClass("Current");

        if (currentBookTypeFilterIndex === -1 && currentBookOfferFilterIndex === -1 && currentBookStatusFilterIndex === -1)
          $($(".ProfileBooksFilter span.All").get(0)).addClass("Current");

        booksCarousel.update(updateImmediately);
      }
    }

    function validateFilterIndexes(bookTypeIndex, bookOfferIndex, bookStatusIndex) {

      if (!isDefined(bookTypeIndex) || !isDefined(bookOfferIndex) || !isDefined(bookStatusIndex))
        throw Errors.ArgumentNullException;

      var bResult = true;
      if (bookTypeIndex !== null)
        bResult &= bookTypeIndex >= -1 && bookTypeIndex <= 2;
      if (bookOfferIndex !== null)
        bResult &= bookOfferIndex >= -1 && bookOfferIndex <= 4;
      if (bookStatusIndex !== null)
        bResult &= bookStatusIndex >= -1 && bookStatusIndex <= 2;
      return bResult;
    }

  </script>

  <% if (IsThatYou) { %>
  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/token-input.css") %>' type="text/css" />
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tokeninput.js") %>'></script>
  <script id="MyProfileOnlyScript" type="text/javascript" language="javascript">

    $(document).ready(function () {

      var tokenInput = $.TokenList("#tbBooksAndAuthors", '<%= ResolveUrl("~/Services/BooksService.asmx/GetAutoCompleteBooksAndAuthors") %>', {
        hintText: "Введите книгу",
        noResultsText: "Ничего не найдено, добавте ее самостоятельно",
        searchingText: "Поиск книги...",
        preventDuplicates: true,
        queryParam: "prefixText",
        onAdd: function (data) {

          if (data) {

            var ids = $("#hfBooksId").val().toArrayList();
            ids.push(data.value);
            $("#hfBooksId").val(ids.join(","));
          }
        },
        onDelete: function (data) {

          if (data) {

            var ids = $("#hfBooksId").val().toArrayList();
            var index = ids.indexOf(data.value);
            if (index >= 0)
              ids.splice(index, 1);
            $("#hfBooksId").val(ids.join(","));
          }
        }
      });

      $("#lnkApplyNewBooks").bind("click", function () {

        changeBooksFilter(-1, -1, -1, false);

        var booksIds = $("#hfBooksId").val().toArrayList();
        $.each(booksIds, function (index, value) {

          engine.books.addToUserLibrary(<%= Entity.UserId %>, value, function () {

            engine.books.getBookInfo(value, function (item) {

              booksCarousel.addItem(item);
            });
          });
        });

        tokenInput.clearTokens();

        $("#hfBooksId").val("");
      });

      $("#lnkCancelNewBooks").bind("click", function () {

        tokenInput.clearTokens();
        $("#hfBooksId").val("");
      });
    });

    function addUserUnprovenBook() {

      var lightwindow = $(".AddUserUnprovenBookContent").lightwindow({
        title: "Добавление книги вручную"
      });
      $("#btnAddUnprovenBook").bind("click", function () {

        changeBooksFilter(-1, -1, -1, false);

        engine.books.addUserUnprovenBook(<%= Entity.UserId %>, $("#tbUnprovenBookTitle").val(), $("#tbUnprovenBookAuthors").val(), 
          $("#tbUnprovenBookComment").val(), parseInt($("#chkUnprovenBookMarkAsRead").val(), 10) === 1, function (newBookId) {

            if (isDefinedAndNotNull(newBookId) && newBookId > 0) {

              engine.books.getBookInfo(newBookId, function (item) {

                booksCarousel.addItem(item);
              });
            }
          });

        lightwindow.close();
      });
    }
  </script>
  <% } %>
  <% if (IsThatYou || UserManagement.IsAdminAuthenticated) { %>
  <script id="RemoveAccountScript" type="text/javascript" language="javascript">
    function removeUserAccount() {

      var lightwindow = $(".RemoveProfileContent").lightwindow({
        title: "Удаление профайла"
      });
      $("#btnRemoveProfileYes").bind("click", function () {

        $.sendAjax({
          url: '<%= ResolveUrl("~/Services/UsersService.asmx/RemoveUserAccount") %>',
          params: { "userId": <%= Entity.UserId %> },
          async: false,
          success: function (data) { 

            if (engine.utilities.checkServerResult(data.d))
              redirectToPage('<%= RedirectHelper.ResolveUrl(RedirectDirection.Home) %>');
            else
              redirectToPage('<%= RedirectHelper.ResolveUrl(RedirectDirection.Error, AttributeHelper.GetEnumValueUrlRewrite(ErrorCodeType.RemovingUserAccountFailed)) %>');
          },
          error: function () {

            redirectToPage('<%= RedirectHelper.ResolveUrl(RedirectDirection.Error, AttributeHelper.GetEnumValueUrlRewrite(ErrorCodeType.RemovingUserAccountFailed)) %>');
          }
        });

        lightwindow.close();
      });
    }
  </script>
  <% } %>

  <% if (UserManagement.IsAuthenticated && !IsThatYou) { %>
  <script id="SendMessageScript" type="text/javascript" language="javascript">

    function sendMessage(toUserId) {

      engine.notifications.sendMessage(<%= UserManagement.CurrentUser.Id %>, toUserId);
    }

  </script>
  <% } %>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <% if (ShowFillMessageToUser) { %>
  <clubbybook:InfoMessageControl ID="infoMessage" runat="server" />
  <% } %>
  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" />

  <div class="ViewRows">
    <div class="ViewLeftSide">
      <div class="ViewImage">
        <img src="<%= ResolveUrl(UIUtilities.ValidateImagePath(Entity.ImagePath, Settings.EmptyProfileAvatarPath)) %>" 
          title="<%= UIUtilities.GetProfileImageAltForSEO(UIUtilities.GetProfileFullName(Entity)) %>"
          alt="<%= UIUtilities.GetProfileImageAltForSEO(UIUtilities.GetProfileFullName(Entity)) %>" />
      </div>
      <div class="ViewActions">
        <ul>
          <% if (UserManagement.IsAuthenticated && !IsThatYou) { %>
          <li>
            <a href="javascript: void(0)" onclick="sendMessage(<%= Entity.UserId %>); return false;">Отправить сообщение</a>
          </li>
          <% } %>
        </ul>
      </div>
    </div>
    <div class="ViewData">
      <ul class="ViewDataList">
        <% if (AccessManagement.CanSeeEmails || IsThatYou) { %>
        <li class="ViewDataListItem">
          <div class="Label">Почта:</div>
          <div class="Value"><%= Entity.User.EMail%></div>
          <div class="Clear"></div>
        </li>
        <% } %>
        <li class="ViewDataListItem">
          <div class="Label">Псевдоним:</div>
          <div class="Value"><%= UIUtilities.ValidateStringValue(Entity.Nickname) %></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Пол:</div>
          <div class="Value"><%= UIUtilities.ValidateStringValue(AttributeHelper.GetEnumValueDescription(Entity.Gender)) %></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Город:</div>
          <div class="Value"><%= ValidateProfileCity(Entity.City) %></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Прочитанных книг:</div>
          <div class="Value"><%= UserReadBookCount %></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Всего книг:</div>
          <div class="Value"><%= UserLibraryBookCount %></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem BottomLine">
          <div class="Label">Дата регистрации:</div>
          <div class="Value"><%= UIUtilities.GetShortDateString(Entity.User.CreatedDate) %></div>
          <div class="Clear"></div>
        </li>
      </ul>
    </div>
    <div class="Clear"></div>
  </div>

  <div class="SpaceBetween"></div>

  <clubbybook:ContentHeaderControl ID="profileBooksContentHeader" runat="server" SmallTitle="True" />
  <div class="ProfileBooksFilter">
    <span class="All Current"><a href="javascript: changeBooksFilter(-1, -1, -1);">все</a></span>
    <span class="Separator">|</span>
    <span class="BookOffer"><a href="javascript: changeBooksFilter(null, 0, null);"><% if (IsThatYou) { %>продам<% } else { %>продаст<% } %></a></span>
    <span class="Separator">|</span>
    <span class="BookOffer"><a href="javascript: changeBooksFilter(null, 1, null);"><% if (IsThatYou) { %>куплю<% } else { %>купит<% } %></a></span>
    <span class="Separator">|</span>
    <span class="BookOffer"><a href="javascript: changeBooksFilter(null, 2, null);"><% if (IsThatYou) { %>обменяю<% } else { %>обменяет<% } %></a></span>
    <span class="Separator">|</span>
    <span class="BookOffer"><a href="javascript: changeBooksFilter(null, 3, null);"><% if (IsThatYou) { %>дам почитать<% } else { %>даст прочесть<% } %></a></span>
    <span class="Separator">|</span>
    <span class="BookOffer"><a href="javascript: changeBooksFilter(null, 4, null);"><% if (IsThatYou) { %>подарю<% } else { %>подарит<% } %></a></span>
    <span class="Separator">|</span>
    <span class="BookStatus"><a href="javascript: changeBooksFilter(null, null, 0);"><% if (IsThatYou) { %>читал(а)<% } else { %>читал(а)<% } %></a></span>
    <span class="Separator">|</span>
    <span class="BookStatus"><a href="javascript: changeBooksFilter(null, null, 1);"><% if (IsThatYou) { %>читаю<% } else { %>читает<% } %></a></span>
    <span class="Separator">|</span>
    <span class="BookStatus"><a href="javascript: changeBooksFilter(null, null, 2);"><% if (IsThatYou) { %>хочу прочесть<% } else { %>хочет прочесть<% } %></a></span>
  </div>
  <div class="ProfileBooksFilter">
    <span class="BookType"><a href="javascript: changeBooksFilter(0, null, null);">печатные</a></span>
    <span class="Separator">|</span>
    <span class="BookType"><a href="javascript: changeBooksFilter(1, null, null);">электронные</a></span>
    <span class="Separator">|</span>
    <span class="BookType"><a href="javascript: changeBooksFilter(2, null, null);">аудиокниги</a></span>
  </div>
  <div id="profileBooks"></div>

  <% if (IsThatYou) { %>
  <div class="SpaceBetween"></div>

  <clubbybook:ContentHeaderControl ID="profileAddBooksContentHeader" runat="server" Title="Добавление книг" SmallTitle="True" />
  <div class="ViewAddNewBook">
    <div class="Fields">
      <input type="text" id="tbBooksAndAuthors" class="TextBox" />
      <input type="hidden" id="hfBooksId" />
    </div>
    <div class="Actions">
      <a id="lnkApplyNewBooks" class="ImageLink" href="javascript: void(0)">
        <img src='<%= ResolveUrl("~/Images/Common/Apply.png") %>' alt="Добавить" title="Добавить" />
      </a>
      <a id="lnkCancelNewBooks" class="ImageLink" href="javascript: void(0)">
        <img src='<%= ResolveUrl("~/Images/Common/Cancel.png") %>' alt="Отмена" title="Отмена" />
      </a>
    </div>
    <div class="Clear"></div>
    <div>
      <p>
        Примечание: при добавлении книг, текущий выбранный фильтр будет сброшен на "все", 
        а вновь добавленные книги появятся в начале списка. В случае, когда Вы не смогли найти книгу, 
        Вы можете воспользоваться <a href="javascript: addUserUnprovenBook();">формой добавления книги вручную</a>. 
        В этом случае книга будет добавлена в Вашу библиотеку, но не будет видна в поиске для других пользователей, 
        до одобрения модератором. После одобрения модератором, у книги появиться обложка и описание.
      </p>
    </div>
  </div>
  <% } %>
  <% if (IsThatYou || UserManagement.IsAdminAuthenticated) { %>
  <div class="SpaceBetween"></div>

  <clubbybook:ContentHeaderControl ID="profileRemoveProfileContentHeader" runat="server" Title="Удаление профайла" SmallTitle="True" />
  <div>
    <p>
      <% if (IsThatYou) { %>
      Внимание! При удалении аккаунта, Ваши данные сохраняются: библиотека книг, рейтинги, сообщения и т.п.
      Если после удаления, Вы вновь захотите пользоваться данным аккаунтом, мы восстановим данные.
      В случае если Вы желаете полностью удалить свой аккаунт, 
      Вам необходимо написать письмо в свободной форме на <a href="mailto:support@clubbybook.com">support@clubbybook.com</a>,
      с того почтового адреса, под каким зарегистрированы в социальной сети ClubbyBook.
      <% } %>
      Для того что бы удалить аккаунт пройдите по <a href="javascript: removeUserAccount();">ссылке</a>.
    </p>
  </div>
  <% } %>
  <% if (UserManagement.IsAuthenticated && !IsThatYou) { %>
  <div class="SendMessageContent Hidden">
    <div class="SendMessageContentMessageBlock">
      <textarea class="TextBox" id="tbMessage"  rows="4" cols="70"></textarea>
    </div>
    <div class="Buttons">
      <input type="button" class="Button" id="btnSend" title="Отправить" value="Отправить" />
      <input type="button" class="Button lightwindow_close" title="Отмена" value="Отмена" />
    </div>
  </div>
  <% } %>
  <% if (IsThatYou) { %>
  <div class="AddUserUnprovenBookContent Hidden">
    <div class="AddUserUnprovenBookContentBlock">
      <ul>
        <li>
          <div class="Label">Название:</div>
          <div class="Value"><input type="text" id="tbUnprovenBookTitle" class="TextBox" /></div>
          <div class="Clear"></div>
        </li>
        <li>
          <div class="Label">Авторы:</div>
          <div class="Value"><input type="text" id="tbUnprovenBookAuthors" class="TextBox" /></div>
          <div class="Clear"></div>
        </li>
        <li>
          <div class="Label">Примечание:</div>
          <div class="Value"><textarea class="TextBox" id="tbUnprovenBookComment"  rows="4" cols="70"></textarea></div>
          <div class="Clear"></div>
        </li>
        <li>
          <div class="Label"></div>
          <div class="Value CheckBox">
            <input type="checkbox" id="chkUnprovenBookMarkAsRead" checked="checked" value="1" />
            <label for="chkUnprovenBookMarkRead">Пометить книгу как прочитанную</label>
          </div>
          <div class="Clear"></div>
        </li>
      </ul>
    </div>
    <div class="Buttons">
      <input type="button" class="Button" id="btnAddUnprovenBook" title="Добавить книгу" value="Добавить книгу" />
      <input type="button" class="Button lightwindow_close" title="Отмена" value="Отмена" />
    </div>
  </div>
  <% } %>
  <% if (IsThatYou || UserManagement.IsAdminAuthenticated) { %>
  <div class="RemoveProfileContent Hidden">
    <div class="RemoveProfileContentMessageBlock">
      Вы действительно хотите удалить <% if (IsThatYou) { %>свой<% } else { %>этот<% } %> профайл?
    </div>
    <div class="Buttons">
      <input type="button" class="Button" id="btnRemoveProfileYes" title="Да" value="Да" />
      <input type="button" class="Button lightwindow_close" title="Нет" value="Нет" />
    </div>
  </div>
  <% } %>

</asp:Content>
