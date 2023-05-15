<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ViewAuthor.aspx.cs" Inherits="ClubbyBook.Web.Common.ViewAuthor" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>
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
          <a class="ImageLink" href="javascript: void(0)" onclick="removeAuthorComment(${id}); return false;">
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

      commentList = new UIList(
        "#commentList", 
        "#commentTemplate", 
        '<%= ResolveUrl("~/Services/AuthorsService.asmx/GetAuthorComments") %>',
        function () {
          return [ { "paramName": "authorId", "paramValue": <%= Entity.Id %> } ];
        },
        {
          listItemSelector: ".DataListItem",
          emptyListText: "Комментарии отсутствуют. Добавьте первый!"
        }
      );
    });

    <% if (UserManagement.IsAuthenticated) { %>

    function addAuthorComment() {

      if ($("#tbNewComment").val() === '')
        return;

      engine.authors.addAuthorComment(<%= Entity.Id %>, $("#tbNewComment").val(), function (newComment) {

        commentList.add(newComment);
        $("#tbNewComment").val("");
      });
    }

    function removeAuthorComment(commentId) {

      engine.authors.removeAuthorComment(<%= Entity.Id %>, commentId, function() {

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
        <img src="<%= ResolveUrl(UIUtilities.ValidateImagePath(Entity.PhotoPath, Settings.EmptyAuthorPhotoPath)) %>" 
          title="<%= UIUtilities.GetAuthorImageAltForSEO(Entity.FullName) %>"
          alt="<%= UIUtilities.GetAuthorImageAltForSEO(Entity.FullName) %>" />
      </div>
      <div class="ViewRate">
        <clubbybook:RatingControl ID="rating" runat="server"
          GetCommonRatingServiceMethod="GetCommonAuthorRating"
          GetUserRatingServiceMethod="GetUserAuthorRating"
          SetUserRatingServiceMethod="SetUserAuthorRating">
        </clubbybook:RatingControl>
      </div>
    </div>
    <div class="ViewData">
      <ul class="ViewDataList">
        <li class="ViewDataListItem BottomLine">
          <div class="Label">Тип:</div>
          <div class="Value"><%= AttributeHelper.GetEnumValueDescription(Entity.Type)%></div>
          <div class="Clear"></div>
        </li>
        <% if (Entity.Type != AuthorType.PublishingHouse) { %>
        <li class="ViewDataListItem">
          <div class="Label">Пол:</div>
          <div class="Value"><%= AttributeHelper.GetEnumValueDescription(Entity.Gender)%></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem BottomLine">
          <div class="Label">Годы жизни:</div>
          <div class="Value"><%= UIUtilities.GetAuthorLifeYearsString(Entity.Gender, Entity.BirthdayYear, Entity.DeathYear)%></div>
          <div class="Clear"></div>
        </li>
        <% } %>
        <li class="ViewDataListItem Description">
          <%= UIUtilities.PrepareTextContent(Entity.ShortDescription) %>
        </li>
      </ul>
    </div>
    <div class="Clear"></div>
  </div>

  <div class="SpaceBetween"></div>

  <clubbybook:ContentHeaderControl ID="contentBiography" runat="server" Title="Биография" />

  <div class="Biography">
    <%= UIUtilities.PrepareTextContent(Entity.Biography) %>
  </div>

  <div class="SpaceBetween"></div>

  <clubbybook:ContentHeaderControl ID="contentHeaderComments" runat="server" Title="Комментарии" SmallTitle="True" />

  <div class="Comments">
  <% if (UserManagement.IsAuthenticated) { %>
    <div class="NewComment">
      <textarea class="TextBox" id="tbNewComment" rows="3" cols="90"></textarea>
      <a href="javascript: void(0);" onclick="addAuthorComment(); return false;" class="Button">Добавить</a>
    </div>
  <% } else {%>
    <clubbybook:InfoMessageControl ID="addingCommentInfoMessage" runat="server" />
  <% } %>
    <div class="Clear BottomLine"></div>
    <div class="ContentDataList">
      <ul id="commentList"></ul>
    </div>
  </div>

</asp:Content>
