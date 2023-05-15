<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ViewBook.aspx.cs" Inherits="ClubbyBook.Web.Common.ViewBook" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>
<%@ Import Namespace="ClubbyBook.Web.Controls" %>
<%@ Import Namespace="ClubbyBook.Common" %>
<%@ Import Namespace="ClubbyBook.Common.Enums" %>
<%@ Import Namespace="ClubbyBook.Common.Utilities" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>
<%@ Import Namespace="ClubbyBook.UI" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/InfoMessageControl.ascx" TagName="InfoMessageControl" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/RatingControl.ascx" TagName="RatingControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/lightlist.css") %>' type="text/css" />
  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/backtotop.css") %>' type="text/css" />

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightlist.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tmpl.min.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.backtotop.js") %>'></script>

  <script id="bookAuthorsTemplate" type="text/x-jquery-tmpl">
    <ul>
      {{each authors}}
        <li>
          {{if confirmed}}<a href="${viewAuthorLink}">{{/if}}${fullName}{{if confirmed}}</a>{{/if}}{{if $index !== authors.length - 1 }},&nbsp;{{/if}}
        </li>
      {{/each}}
    </ul>
  </script>

  <script id="bookGenresTemplate" type="text/x-jquery-tmpl">
    <ul>
      {{each genres}}
        <li>
          <a href="${viewGenreBooksLink}">${name}</a>{{if $index !== genres.length - 1 }},&nbsp;{{/if}}
        </li>
      {{/each}}
    </ul>
  </script>

  <script id="bookTypeTemplate" type="text/x-jquery-tmpl">
    {{if totalCount > 0 }}
      {{if partialCount > 0 }}
        <div><a href="${partialLink}">в вашем городе (${partialCount})</a></div>
      {{/if}}
      <div><a href="${totalLink}">всего (${totalCount})</a></div>
    {{/if}}
  </script>

  <script id="offerTemplate" type="text/x-jquery-tmpl">
    {{if totalCount > 0 }}
      {{if partialCount > 0 }}
        <div><a href="${partialLink}">в вашем городе (${partialCount})</a></div>
      {{/if}}
      <div><a href="${totalLink}">всего (${totalCount})</a></div>
    {{/if}}
  </script>

  <script id="commentTemplate" type="text/x-jquery-tmpl">

  <li id="comment_${id}" class="DataListItem">
    <div class="Image">
      <a class="ImageLink" href="${viewUserLink}">
        <img src="${userPhotoPath}" title="${seoImageAlt}" alt="${seoImageAlt}" />
      </a>
    </div>
    <div class="Data">
      <ul class="List">
        <li class="Item Title">
          <a href="${viewUserLink}">
            <strong>${userFullName}</strong>
          </a>
        </li>
        <li class="Item Label">
          ${createdDate}
        </li>
        <li class="Item Description">
          {{html message}}
        </li>
    </div>
    <div class="Actions Hidden">
      <% if (UserManagement.IsAuthenticated) { %>
      <ul>
        {{if isUserComment}}
        <li class="Remove">
          <a class="ImageLink" href="javascript: void(0)" onclick="removeBookComment(${id}); return false;">
            <img title="Удалить комментарий" alt="Удалить комментарий" src='<%= ResolveUrl("~/Images/Common/Remove.png") %>' />
          </a>
        </li>
        {{/if}}
      </ul>
      <% } %>
    </div>
    <div class="Clear"></div>
  </li>

  </script>

  <script type="text/javascript" language="javascript">

    var commentList = null;

    $(document).ready(function () {

      $.backtotop({
        text: "Вверх"
      });

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/BooksService.asmx/GetBookInfo") %>',
        params: { 'bookId': <%= Entity.Id %> },
        success: function (data) {

          updateUserBookStatus();

          var lists = new Object();
          lists.authors = new Array();
          lists.genres = new Array();
          lists.confirmed = true;

          var bookInfo = data.d;
          if (isDefinedAndNotNull(bookInfo)) {

            lists.authors = bookInfo.authors;
            lists.genres = bookInfo.genres;
            lists.confirmed = bookInfo.confirmed;

            updateUserBookStatus(bookInfo.containsInUserLibrary, bookInfo);
          }

          if (lists.authors.length > 0) {

            $(".BookAuthorsList").empty();
            $("#bookAuthorsTemplate").tmpl(lists).appendTo(".BookAuthorsList");
          }

          if (lists.genres.length > 0) {

            $(".BookGenresList").empty();
            $("#bookGenresTemplate").tmpl(lists).appendTo(".BookGenresList");
          }
        },
        error: function (error) {

          engine.eventHandlers.onServerFailed(error);
        }
      });

      commentList = new UIList(
        "#commentList", 
        "#commentTemplate", 
        '<%= ResolveUrl("~/Services/BooksService.asmx/GetBookComments") %>',
        function () {
          return [ { "paramName": "bookId", "paramValue": <%= Entity.Id %> } ];
        },
        {
          listItemSelector: ".DataListItem",
          emptyListText: "Комментарии отсутствуют. Добавьте первый!"
        }
      );

      function updateUserBookStatus(containsInUserLibrary, bookInfo) {

        if (containsInUserLibrary) {

          $(".InUserLibrary").removeClass("Hidden");
          $(".NotInUserLibrary").addClass("Hidden");

          if (bookInfo) {

            $("#ddlBookStatus").val(bookInfo.status);

            updateUserBookComment(bookInfo.userComment);

            if ((bookInfo.type & <%= (int)UserBookType.PaperBook %>) === <%= (int)UserBookType.PaperBook %>)
              $("#chkBookTypePaperBook").attr("checked", "checked");
            if ((bookInfo.type & <%= (int)UserBookType.EBook %>) === <%= (int)UserBookType.EBook %>)
              $("#chkBookTypeEBook").attr("checked", "checked");
            if ((bookInfo.type & <%= (int)UserBookType.Audiobook %>) === <%= (int)UserBookType.Audiobook %>)
              $("#chkBookTypeAudiobook").attr("checked", "checked");

            if ((bookInfo.offer & <%= (int)UserBookOfferType.Buy %>) === <%= (int)UserBookOfferType.Buy %>)
              $("#chkOfferBuy").attr("checked", "checked");
            if ((bookInfo.offer & <%= (int)UserBookOfferType.Sell %>) === <%= (int)UserBookOfferType.Sell %>)
              $("#chkOfferSell").attr("checked", "checked");
            if ((bookInfo.offer & <%= (int)UserBookOfferType.Barter %>) === <%= (int)UserBookOfferType.Barter %>)
              $("#chkOfferBarter").attr("checked", "checked");
            if ((bookInfo.offer & <%= (int)UserBookOfferType.WillGiveRead %>) === <%= (int)UserBookOfferType.WillGiveRead %>)
              $("#chkOfferWillGiveRead").attr("checked", "checked");  
            if ((bookInfo.offer & <%= (int)UserBookOfferType.WillGrantGratis %>) === <%= (int)UserBookOfferType.WillGrantGratis %>)
              $("#chkOfferWillGrantGratis").attr("checked", "checked");  
          }
        }
        else {

          $("li.InUserLibrary").addClass("Hidden");
          $("li.NotInUserLibrary").removeClass("Hidden");

          $("#ddlBookStatus").val(<%= (int)UserBookStatusType.None %>);
          $("#lstBookType input[type=checkbox]").removeAttr("checked");
          $("#lstOffer input[type=checkbox]").removeAttr("checked");
        }

        if (bookInfo) {

          if (bookInfo.collection)
            $("#lbCollection").text("Да");

          updateUserBookTypeInternal(bookInfo.paperBookType, "#lbPaperBookType");
          updateUserBookTypeInternal(bookInfo.eBookType, "#lbEBookType");
          updateUserBookTypeInternal(bookInfo.audiobookType, "#lbAudiobookType");

          updateUserBookOfferInternal(bookInfo.sellOffer, "#lbSellOffer");
          updateUserBookOfferInternal(bookInfo.buyOffer, "#lbBuyOffer");
          updateUserBookOfferInternal(bookInfo.barterOffer,"#lbBarterOffer");
          updateUserBookOfferInternal(bookInfo.willGiveReadOffer, "#lbWillGiveReadOffer");
          updateUserBookOfferInternal(bookInfo.willGrantGratisOffer, "#lbWillGrantGratisOffer");
        }
      }

      function updateUserBookTypeInternal(bookType, bookTypeSpan) {

        if (isDefinedAndNotNull(bookType) && isDefinedAndNotNull(bookTypeSpan) && bookType.totalCount > 0) {

          $(bookTypeSpan).empty();
          $("#bookTypeTemplate").tmpl(bookType).appendTo($(bookTypeSpan));
        }
      }

      function updateUserBookOfferInternal(offer, offerSpan) {

        if (isDefinedAndNotNull(offer) && isDefinedAndNotNull(offerSpan) && offer.totalCount > 0) {

          $(offerSpan).empty();
          $("#offerTemplate").tmpl(offer).appendTo($(offerSpan));
        }
      }

      <% if (UserManagement.IsAuthenticated) { %>

      $("#lstBookType input[type=checkbox]").bind("click", function () {

        var bType = <%= (int)UserBookType.None %>;

        $("#lstBookType input[type=checkbox]:checked").each(function() {
          bType |= $(this).val();
        });

        engine.books.changeUserBookType(<%= UserManagement.CurrentUser.Id %>, <%= Entity.Id %>, bType);
      });

      $("#lstOffer input[type=checkbox]").bind("click", function () {

        var offer = <%= (int)UserBookOfferType.None %>;

        $("#lstOffer input[type=checkbox]:checked").each(function() {
          offer |= $(this).val();
        });

        engine.books.changeUserBookOffer(<%= UserManagement.CurrentUser.Id %>, <%= Entity.Id %>, offer);
      });

      $("#ddlBookStatus").bind("change", function () {

        engine.books.changeUserBookStatus(<%= UserManagement.CurrentUser.Id %>, <%= Entity.Id %>, $("#ddlBookStatus").val());
      });

      $("#lnkAddBookToLibrary").bind("click", function () {

        engine.books.addToUserLibrary(<%= UserManagement.CurrentUser.Id %>, <%= Entity.Id %>, function () {

          updateUserBookStatus(true);
        });

        return false;
      });

      $("#lnkRemoveBookFromLibrary").bind("click", function () {

        engine.books.removeFromUserLibrary(<%= UserManagement.CurrentUser.Id %>, <%= Entity.Id %>, function () {

          updateUserBookStatus(false);
        });

        return false;
      });

      <% } %>
    });

    <% if (UserManagement.IsAuthenticated) { %>
    function changeUserBookComment() {

      var lightwindow = $(".ChangeUserBookCommentContent").lightwindow({
        title: "Изменить комментарий"
      });
      $("#tbUserBookComment").val($("#lbUserBookComment").text());
      $("#btnChangeUserBookComment").bind("click", function () {

        var userBookComment = $("#tbUserBookComment").val();
        engine.books.changeUserBookComment(<%= UserManagement.CurrentUser.Id %>, <%= Entity.Id %>, userBookComment, function() {

          updateUserBookComment(userBookComment);
        });

        lightwindow.close();
      });
    }

    function updateUserBookComment(comment) 
    {
      if (isDefinedAndNotNull(comment) && comment !== "") {

        $("#lbUserBookComment").css("margin-right", "5px");
        $("#lbUserBookComment").text(comment);
      }
      else {

        $("#lbUserBookComment").css("margin-right", "");
        $("#lbUserBookComment").text("");
      }
    }

    function addBookComment() {

      if ($("#tbNewComment").val() === '')
        return;

      engine.books.addBookComment(<%= Entity.Id %>, $("#tbNewComment").val(), function (newComment) {

        commentList.add(newComment);
        $("#tbNewComment").val("");
      });
    }

    function removeBookComment(commentId) {

      engine.books.removeBookComment(<%= Entity.Id %>, commentId, function() {

        var commentItemSelector = "li#comment_" + commentId;
        $(commentItemSelector).slideUp(500, function() {
          $(commentItemSelector).remove();
        });
      });
    }

    <% } %>

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:InfoMessageControl ID="infoMessage" runat="server" />

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" />

  <div class="ViewRows">
    <div class="ViewLeftSide">
      <div class="ViewImage">
        <img src="<%= ResolveUrl(UIUtilities.ValidateImagePath(Entity.CoverPath, Settings.EmptyBookCoverPath)) %>" 
          title="<%= UIUtilities.GetBookImageAltForSEO(Entity.Title) %>"
          alt="<%= UIUtilities.GetBookImageAltForSEO(Entity.Title) %>" />
      </div>
      <div class="ViewRate">
        <clubbybook:RatingControl ID="rating" runat="server" 
          GetCommonRatingServiceMethod="GetCommonBookRating"
          GetUserRatingServiceMethod="GetUserBookRating"
          SetUserRatingServiceMethod="SetUserBookRating">
        </clubbybook:RatingControl>
      </div>
      <div class="ViewActions">
        <ul>
          <% if (UserManagement.IsAuthenticated) { %>
          <li class="Hidden BottomLine NotInUserLibrary">
            <a id="lnkAddBookToLibrary" href="javascript: void(0)">Добавить в мою библиотеку</a>
          </li>
          <li class="Hidden BottomLine InUserLibrary">
            <a id="lnkRemoveBookFromLibrary" href="javascript: void(0)">Убрать из моей библиотеки</a>
          </li>
          <li class="Hidden BottomLine InUserLibrary">
            <select id="ddlBookStatus" class="DropDownList">
              <option value="<%= (int)UserBookStatusType.None %>">
                <%= AttributeHelper.GetEnumValueDescription(UserBookStatusType.None) %>
              </option>
              <option value="<%= (int)UserBookStatusType.WantToRead %>">
                <%= AttributeHelper.GetEnumValueDescription(UserBookStatusType.WantToRead) %>
              </option>
              <option value="<%= (int)UserBookStatusType.ReadingNow %>">
                <%= AttributeHelper.GetEnumValueDescription(UserBookStatusType.ReadingNow) %>
              </option>
              <option value="<%= (int)UserBookStatusType.AlreadyRead %>">
                <%= AttributeHelper.GetEnumValueDescription(UserBookStatusType.AlreadyRead) %>
              </option>
            </select>
          </li>
          <li class="Hidden BottomLine InUserLibrary">
            <ul id="lstBookType">
              <li class="CheckBox">
                <input id="chkBookTypePaperBook" type="checkbox" value="<%= (int)UserBookType.PaperBook %>" />
                <label for="chkBookTypePaperBook"><%= AttributeHelper.GetEnumValueDescription(UserBookType.PaperBook)%></label>
              </li>
              <li class="CheckBox">
                <input id="chkBookTypeEBook" type="checkbox" value="<%= (int)UserBookType.EBook %>" />
                <label for="chkBookTypeEBook"><%= AttributeHelper.GetEnumValueDescription(UserBookType.EBook)%></label>
              </li>
              <li class="CheckBox">
                <input id="chkBookTypeAudiobook" type="checkbox" value="<%= (int)UserBookType.Audiobook %>" />
                <label for="chkBookTypeAudiobook"><%= AttributeHelper.GetEnumValueDescription(UserBookType.Audiobook)%></label>
              </li>
            </ul>
          </li>
          <li class="Hidden BottomLine InUserLibrary">
            <ul id="lstOffer">
              <li class="CheckBox">
                <input id="chkOfferBuy" type="checkbox" value="<%= (int)UserBookOfferType.Buy %>" />
                <label for="chkOfferBuy"><%= AttributeHelper.GetEnumValueDescription(UserBookOfferType.Buy)%></label>
              </li>
              <li class="CheckBox">
                <input id="chkOfferSell" type="checkbox" value="<%= (int)UserBookOfferType.Sell %>" />
                <label for="chkOfferSell"><%= AttributeHelper.GetEnumValueDescription(UserBookOfferType.Sell)%></label>
              </li>
              <li class="CheckBox">
                <input id="chkOfferBarter" type="checkbox" value="<%= (int)UserBookOfferType.Barter %>" />
                <label for="chkOfferBarter"><%= AttributeHelper.GetEnumValueDescription(UserBookOfferType.Barter)%></label>
              </li>
              <li class="CheckBox">
                <input id="chkOfferWillGiveRead" type="checkbox" value="<%= (int)UserBookOfferType.WillGiveRead %>" />
                <label for="chkOfferWillGiveRead"><%= AttributeHelper.GetEnumValueDescription(UserBookOfferType.WillGiveRead)%></label>
              </li>
              <li class="CheckBox">
                <input id="chkOfferWillGrantGratis" type="checkbox" value="<%= (int)UserBookOfferType.WillGrantGratis %>" />
                <label for="chkOfferWillGrantGratis"><%= AttributeHelper.GetEnumValueDescription(UserBookOfferType.WillGrantGratis)%></label>
              </li>
            </ul>
          </li>
          <% } %>
        </ul>
      </div>
    </div>
    <div class="ViewData">
      <ul class="ViewDataList">
        <li class="ViewDataListItem">
          <div class="Label">Авторы:</div>
          <div class="Value"><span class="BookAuthorsList"><%= UIUtilities.NotSpecifiedString %></span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Жанр:</div>
          <div class="Value"><span class="BookGenresList"><%= UIUtilities.NotSpecifiedString %></span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Название оригинала:</div>
          <div class="Value"><%= UIUtilities.ValidateStringValue(Entity.OriginalTitle) %></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem BottomLine">
          <div class="Label">Сборник:</div>
          <div class="Value"><span id="lbCollection">Нет</span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Продают:</div>
          <div class="Value"><span id="lbSellOffer">-</span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Покупают:</div>
          <div class="Value"><span id="lbBuyOffer">-</span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Обменивают:</div>
          <div class="Value"><span id="lbBarterOffer">-</span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem ">
          <div class="Label">Дадут прочесть:</div>
          <div class="Value"><span id="lbWillGiveReadOffer">-</span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem BottomLine">
          <div class="Label">Дарят:</div>
          <div class="Value"><span id="lbWillGrantGratisOffer">-</span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem">
          <div class="Label">Печатные книги:</div>
          <div class="Value"><span id="lbPaperBookType">-</span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem ">
          <div class="Label">Электронные книги:</div>
          <div class="Value"><span id="lbEBookType">-</span></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem BottomLine">
          <div class="Label">Аудиокниги:</div>
          <div class="Value"><span id="lbAudiobookType">-</span></div>
          <div class="Clear"></div>
        </li>
        <% if (UserManagement.IsAuthenticated) { %>
        <li class="ViewDataListItem BottomLine InUserLibrary">
          <div class="Label">Ваше примечание:</div>
          <div class="Value"><span id="lbUserBookComment" class="Description"></span><a href="javascript: changeUserBookComment();" class="SmallLink">(редактировать)</a></div>
          <div class="Clear"></div>
        </li>
        <% } %>
        <li class="ViewDataListItem Description">
          <%= Entity.Confirmed ? UIUtilities.PrepareTextContent(Entity.Description) : string.Format("Книга \"{0}\" не проверена.", Entity.Title) %>
        </li>
      </ul>
    </div>
    <div class="Clear"></div>
  </div>

  <div class="SpaceBetween"></div>

  <clubbybook:ContentHeaderControl ID="contentHeaderComments" runat="server" Title="Комментарии" SmallTitle="True" />

  <div class="Comments">
  <% if (UserManagement.IsAuthenticated) { %>
    <div class="NewComment">
      <textarea class="TextBox" id="tbNewComment" rows="3" cols="90"></textarea>
      <a href="javascript: void(0);" onclick="addBookComment(); return false;" class="Button">Добавить</a>
    </div>
  <% } else {%>
    <clubbybook:InfoMessageControl ID="addingCommentInfoMessage" runat="server" />
  <% } %>
    <div class="Clear BottomLine"></div>
    <div class="ContentDataList">
      <ul id="commentList"></ul>
    </div>
  </div>

  <% if (UserManagement.IsAuthenticated) { %>
  <div class="ChangeUserBookCommentContent Hidden">
    <div class="ChangeUserBookCommentContentBlock">
      <textarea class="TextBox" id="tbUserBookComment"  rows="4" cols="70"></textarea>
    </div>
    <div class="Buttons">
      <input type="button" class="Button" id="btnChangeUserBookComment" title="Изменить" value="Изменить" />
      <input type="button" class="Button lightwindow_close" title="Отмена" value="Отмена" />
    </div>
  </div>
  <% } %>
</asp:Content>
