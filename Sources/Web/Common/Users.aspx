<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="ClubbyBook.Web.Common.Users"  %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>
<%@ Import Namespace="ClubbyBook.Common" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/lightlist.css") %>' type="text/css" />
  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/backtotop.css") %>' type="text/css" />

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightlist.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tmpl.min.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.backtotop.js") %>'></script>

  <script id="userTemplate" type="text/x-jquery-tmpl">

  <li id="user_${userId}" class="DataListItem">
    <div class="Image">
      <a class="ImageLink" href="${viewProfileLink}">
        <img src="${photoPath}" title="${seoImageAlt}" alt="${seoImageAlt}" />
      </a>
    </div>
    <div class="Data">
      <ul class="List">
        <li class="Item Title">
          <a href="${viewProfileLink}">
            <strong>${fullName}</strong>
          </a>
        </li>
        <li class="Item Label">
          Псевдоним: ${nickname}
        </li>
        <li class="Item Label">
          Пол: ${genderName}
        </li>
        <li class="Item Label">
          Город: ${cityName}
        </li>
        <li class="Item Label">
          Всего книг: ${userBookCount}
        </li>
      </ul>
    </div>
    <div class="Actions Hidden">
      <% if (UserManagement.IsAuthenticated) { %>
      <ul>
        <li class="SendMessage">
          <a class="ImageLink" href="javascript: void(0)" onclick="sendMessage(${userId}); return false;">
            <img title="Отправить сообщение" alt="Отправить сообщение" src='<%= ResolveUrl("~/Images/Common/SendMessage.png") %>' />
          </a>
        </li>
      </ul>
      <% } %>
    </div>
    <div class="Clear"></div>
  </li>

  </script>

  <script language="javascript" type="text/javascript">

    var usersList = null;

    $(document).ready(function() {

      $.backtotop({
        text: "Вверх"
      });

      usersList = new UIList(
        "#usersList", 
        "#userTemplate", 
        '<%= ResolveUrl("~/Services/UsersService.asmx/GetUsers") %>',
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

          usersList.update();
          e.preventDefault();
        }
      });
    });

    function getSearchParameters() {

      var searchParameters = [
        { "paramName": "searchText", "paramValue": $("#tbSearch").val() }
      ];

      <% if (AccessManagement.CanAdvancedUsersSearch) { %>
      $("#lstRoles input[type=checkbox]:checked").each(function() {
        searchParameters.push({ "paramName": "role", "paramValue": $(this).val() });
      });
      <% } %>

      ids = getSearchParameterIds(UsersSearchParam.Books);
      $.each(ids, function (index, value) {
        searchParameters.push({ "paramName": "bookId", "paramValue": value });
      });

      ids = getSearchParameterIds(UsersSearchParam.Cities);
      $.each(ids, function (index, value) {
        searchParameters.push({ "paramName": "cityId", "paramValue": value });
      });

      ids = getSearchParameterIds(UsersSearchParam.Offers);
      $.each(ids, function (index, value) {
        searchParameters.push({ "paramName": "offer", "paramValue": value });
      });

      ids = getSearchParameterIds(UsersSearchParam.BookTypes);
      $.each(ids, function (index, value) {
        searchParameters.push({ "paramName": "bookType", "paramValue": value });
      });

      return searchParameters;
    }

    function getSearchParameterIds(parameter) {

      var idsArray = new Array();

      switch (parameter) {

        case UsersSearchParam.Books:

          var bookIdFromRequest = <%= BookId %>;
          if (bookIdFromRequest !== -1)
            idsArray.push(bookIdFromRequest);

          break;

        case UsersSearchParam.Cities:

          var cityIdFromRequest = <%= CityId %>;
          if (cityIdFromRequest !== -1)
            idsArray.push(cityIdFromRequest);

          break;

        case UsersSearchParam.Offers:

          var offerFromRequest = <%= Offer %>;
          if (offerFromRequest !== -1)
            idsArray.push(offerFromRequest);

          break;

        case UsersSearchParam.BookTypes:

          var bookTypeFromRequest = <%= BookType %>;
          if (bookTypeFromRequest !== -1)
            idsArray.push(bookTypeFromRequest);

          break;
      }

      return idsArray;
    }

    <% if (AccessManagement.CanAdvancedUsersSearch) { %>
    function expandAdvancedSearchBlock() {

      if (isVisible($(".SearchBlock .Advanced")))
        $(".SearchBlock .Advanced").slideUp(500);
      else
        $(".SearchBlock .Advanced").slideDown(500);
    }
    <% } %>

    <% if (UserManagement.IsAuthenticated) { %>
    function sendMessage(toUserId) {

      engine.notifications.sendMessage(<%= UserManagement.CurrentUser.Id %>, toUserId);
    }
    <% } %>

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Читатели" />

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
    <% if (AccessManagement.CanAdvancedUsersSearch) { %>
    <div class="Advanced">
      <ul class="FilterList">
        <li class="FilterItem">
          <div class="Label">Показывать только:</div>
          <div class="Field">
            <ul id="lstRoles">
              <li class="CheckBox">
                <input id="chkAdminRole" type="checkbox" value="<%= UserManagement.AdminRoleName %>" />
                <label for="chkAdminRole">Администраторов</label>
              </li>
              <li class="CheckBox">
                <input id="chkEditorRole" type="checkbox" value="<%= UserManagement.EditorRoleName %>" />
                <label for="chkEditorRole">Редакторов</label>
              </li>
              <li class="CheckBox">
                <input id="chkAccountRole" type="checkbox" value="<%= UserManagement.AccountRoleName %>" />
                <label for="chkAccountRole">Пользователей</label>
              </li>
            </ul>
          </div>
          <div class="Clear"></div>
        </li>
        <li class="Button">
          <a class="Button" href="javascript: void(0)" onclick="usersList.update(); return false;">Поиск</a>
        </li>
      </ul>
    </div>
    <% } %>
  </div>

  <div class="SearchParametersBlock">
    <p id="lbTotalItemCount"></p>
    <p id="lbSearchParameters"></p>
  </div>

  <div class="ContentDataList">
    <ul id="usersList"></ul>
  </div>

  <% if (UserManagement.IsAuthenticated) { %>
  <div class="SendMessageContent Hidden">
    <div class="SendMessageContentMessageBlock">
      <textarea class="TextBox" id="tbMessage" rows="4" cols="70"></textarea>
    </div>
    <div class="Buttons">
      <input type="button" class="Button" id="btnSend" title="Отправить" value="Отправить" />
      <input type="button" class="Button lightwindow_close" title="Отмена" value="Отмена" />
    </div>
  </div>
  <% } %>

</asp:Content>
