<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="News.aspx.cs" Inherits="ClubbyBook.Web.Common.News"  %>
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

  <script id="newsTemplate" type="text/x-jquery-tmpl">

  <li id="news_${id}" class="DataListItem">
    <div class="Data OnlyData">
      <ul class="List">
        <li class="Item Title">
          <a href="${viewNewsLink}">
            <strong>${title}</strong>
          </a>
        </li>
        <li class="Item Label">
          ${createdDateFull}
        </li>
        <li class="Item Description">
          <p>${restrictedMessage}</p>
        </li>
      </ul>
    </div>
    <div class="Actions Hidden">
      <ul>
        <% if (AccessManagement.CanEditNews) { %>
        <li class="Edit">
          <a class="ImageLink" href="${editNewsLink}">
            <img title="Редактировать" alt="Редактировать" src='<%= ResolveUrl("~/Images/Common/Edit.png") %>' />
          </a>
        </li>
        <% } %>
        <% if (AccessManagement.CanRemoveNews) { %>
        <li class="Remove">
          <a class="ImageLink" href="javascript: void(0)" onclick="removeNews(${id}); return false;">
            <img title="Удалить новость" alt="Удалить новость" src='<%= ResolveUrl("~/Images/Common/Remove.png") %>' />
          </a>
        </li>
        <% } %>
      </ul>
    </div>
    <div class="Clear"></div>
  </li>

  </script>

  <script language="javascript" type="text/javascript">

    var newsList = null;

    $(document).ready(function() {

      $.backtotop({
        text: "Вверх"
      });

      newsList = new UIList(
        "#newsList", 
        "#newsTemplate", 
        '<%= ResolveUrl("~/Services/NewsService.asmx/GetNews") %>',
        function () { return []; },
        { listItemSelector: ".DataListItem" }
      );
    });

    <% if (AccessManagement.CanRemoveNews) { %>
    function removeNews(id) {

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/NewsService.asmx/RemoveNews") %>',
        params: { "newsId": id },
        success: function (data) {

          if (engine.utilities.checkServerResult(data.d)) {

            var newsListItem = $("li#news_" + id);

            $(newsListItem).slideUp(500, function() {

              $(newsListItem).remove();

              engine.eventHandlers.onServerSuccess("Новость удалена.");
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

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Новости из мира книг" />

  <div class="ContentDataList">
    <ul id="newsList"></ul>
  </div>

</asp:Content>
