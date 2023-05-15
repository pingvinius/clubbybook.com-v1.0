<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Authors.aspx.cs" Inherits="ClubbyBook.Web.Common.Authors"  %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>
<%@ Import Namespace="ClubbyBook.Common" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/lightlist.css") %>' type="text/css" />
  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/backtotop.css") %>' type="text/css" />

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tmpl.min.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightlist.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.backtotop.js") %>'></script>

  <script id="authorTemplate" type="text/x-jquery-tmpl">

  <li id="author_${id}" class="DataListItem">
    <div class="Image">
      <a class="ImageLink" href="${viewAuthorLink}">
        <img src="${photoPath}" title="${seoImageAlt}" alt="${seoImageAlt}" />
      </a>
    </div>
    <div class="Data">
      <ul class="List">
        <li class="Item Title">
          <a href="${viewAuthorLink}">
            <strong>${fullName}</strong>{{if isPublishingHouse}} (${typeString}){{/if}}
          </a>
        </li>
        <li class="Item Label">
          Годы жизни: ${yearsString}
        </li>
        <li class="Item Label">
          Количество книг в наличии: ${booksCount}
        </li>
        <li class="Item Description">
          <p>${restrictedDescription}</p>
        </li>
      </ul>
    </div>
    <div class="Actions Hidden">
      <ul>
        <li class="ShowAuthorBooks">
          <a class="ImageLink" href="${viewAuthorBooksLink}">
            <img title="Просмотреть список книг" alt="Просмотреть список книг" src='<%= ResolveUrl("~/Images/Common/Library.png") %>' />
          </a>
        </li>
        <% if (AccessManagement.CanEditAuthor) { %>
        <li class="Edit">
          <a class="ImageLink" href="${editAuthorLink}">
            <img title="Редактировать" alt="Редактировать" src='<%= ResolveUrl("~/Images/Common/Edit.png") %>' />
          </a>
        </li>
        <% } %>
        <% if (AccessManagement.CanRemoveAuthor) { %>
        <li class="Remove">
          <a class="ImageLink" href="javascript: void(0)" onclick="removeAuthor(${id}); return false;">
            <img title="Удалить автора" alt="Удалить автора" src='<%= ResolveUrl("~/Images/Common/Remove.png") %>' />
          </a>
        </li>
        <% } %>
      </ul>
    </div>
    <div class="Clear"></div>
  </li>

  </script>

  <script language="javascript" type="text/javascript">

    var authorsList = null;

    $(document).ready(function() {

      $.backtotop({
        text: "Вверх"
      });

      authorsList = new UIList(
        "#authorsList", 
        "#authorTemplate", 
        '<%= ResolveUrl("~/Services/AuthorsService.asmx/GetAuthors") %>',
        getSearchParameters,
        {
          listItemSelector: ".DataListItem",
          searchParametersBlockContainerSelector: ".SearchParametersBlock",
          totalItemCountContainerSelector: "#lbTotalItemCount"
        }
      );

      $("#tbSearch").bind("keypress", function(e) {

        var code = e.keyCode || e.which;
        if(code === Keys.Enter) {

          authorsList.update();
          e.preventDefault();
        }
      });
    });

    function getSearchParameters() {

      return [{ "paramName": "searchText", "paramValue": $("#tbSearch").val() }];
    }

    <% if (AccessManagement.CanRemoveAuthor) { %>
    function removeAuthor(id) {

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/AuthorsService.asmx/RemoveAuthor") %>',
        params: { "authorId": id },
        success: function (data) {

          if (engine.utilities.checkServerResult(data.d)) {

            var authorListItem = $("li#author_" + id);

            $(authorListItem).slideUp(500, function() {

              $(authorListItem).remove();

              engine.eventHandlers.onServerSuccess("Автор удален.");
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

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Авторы" />

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
  </div>

  <div class="SearchParametersBlock">
    <p id="lbTotalItemCount"></p>
  </div>

  <div class="ContentDataList">
    <ul id="authorsList"></ul>
  </div>

</asp:Content>
