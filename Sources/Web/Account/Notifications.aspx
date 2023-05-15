<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="ClubbyBook.Web.Account.Notifications" %>
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

  <% if (AccessManagement.CanExploitSystemNotifications) { %>
  <script id="systemNotificationTemplate" type="text/x-jquery-tmpl">

  <li id="systemnotification_${id}" class="DataListItem{{if isNew}} Highlight{{/if}}">
    <div class="Image">
      <a class="ImageLink" href="${ownerUserLink}">
        <img title="${ownerUserFullName}" alt="${ownerUserFullName}" src="${ownerUserPhotoPath}" />
      </a>
    </div>
    <div class="Data">
      <ul class="List">
        <li class="Item Title">
          <a href="${ownerUserLink}">
            <strong>${ownerUserFullName}</strong>
          </a>
        </li>
        <li class="Item Title">
          ${typeString} (${createdDate})
        </li>
        <li class="Item Description">
          {{html content}}
        </li>
      </ul>
    </div>
    <div class="Actions Hidden">
      <ul>
        {{if isNew}}
        <li class="MarkAsRead">
          <a class="ImageLink" href="javascript: void(0)" onclick="markAsReadSystemNotification(${id}); return false;">
            <img title="Пометить как прочитаное" alt="Пометить как прочитаное" src='<%= ResolveUrl("~/Images/Common/MarkNotificationAsRead.png") %>' />
          </a>
        </li>
        {{/if}}
        <li class="Remove">
          <a class="ImageLink" href="javascript: void(0)" onclick="removeSystemNotification(${id}); return false;">
            <img title="Удалить" alt="Удалить" src='<%= ResolveUrl("~/Images/Common/Remove.png") %>' />
          </a>
        </li>
      </ul>
    </div>
    <div class="Clear"></div>
  </li>

  </script>

  <script id="systemNotificationsScript" language="javascript" type="text/javascript">

    var systemNotificationsList = null;

    function initializeSystemNotifications() {

      systemNotificationsList = new UIList(
        "#systemNotificationsList", 
        "#systemNotificationTemplate", 
        '<%= ResolveUrl("~/Services/NotificationsService.asmx/GetSystemNotifications") %>',
        getSystemNotificationsSearchParameters,
        {
          listItemSelector: ".System .DataListItem",
          searchParametersBlockContainerSelector: ".System .SearchParametersBlock",
          totalItemCountContainerSelector: "#lbSystemNotificationsTotalItemCount"
        }
      );

      $("#tbSystemNotificationsSearch").bind("keypress", function (e) {

        var code = e.keyCode || e.which;
        if (code === Keys.Enter) {

          systemNotificationsList.update();
          e.preventDefault();
        }
      });
    }

    function getSystemNotificationsSearchParameters() {

      return [{ "paramName": "searchText", "paramValue": $("#tbSystemNotificationsSearch").val() }];
    }

    function markAsReadSystemNotification(id) {

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/NotificationsService.asmx/MarkAsReadSystemNotification") %>',
        params: { "systemNotificationId": id },
        success: function (data) {

          if (engine.utilities.checkServerResult(data.d)) {

            $("li#systemnotification_" + id).removeClass("Highlight");
            $("li#systemnotification_" + id + " div.Actions ul li.MarkAsRead").remove();

            engine.notifications.update(<%= UserManagement.CurrentUser.Id %>);
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {

          engine.eventHandlers.onServerFailed(error);
        }
      });
    }

    function removeSystemNotification(id) {

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/NotificationsService.asmx/RemoveSystemNotification") %>',
        params: { "systemNotificationId": id },
        success: function (data) {

          if (engine.utilities.checkServerResult(data.d)) {

            var listItem = $("li#systemnotification_" + id);

            $(listItem).slideUp(500, function () {

              $(listItem).remove();

              engine.eventHandlers.onServerSuccess("Уведомление удалено.");

              engine.notifications.update(<%= UserManagement.CurrentUser.Id %>);
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

  </script>
  <% } %>
  <% if (AccessManagement.CanExploitFeedbackNotifications) { %>
  <script id="feedbackNotificationTemplate" type="text/x-jquery-tmpl">

  <li id="feedbacknotification_${id}" class="DataListItem{{if isNew}} Highlight{{/if}}">
    <div class="Image">
      {{if !isAnonymous}}<a class="ImageLink" href="${fromUserLink}">{{/if}}
        <img title="${fromUserFullName}" alt="${fromUserFullName}" src="${fromUserPhotoPath}" />
      {{if !isAnonymous}}</a>{{/if}}
    </div>
    <div class="Data">
      <ul class="List">
        <li class="Item Title">
          {{if !isAnonymous}}
            <a href="${fromUserLink}">
          {{else}}
            <span class="LinkSubstitute">
          {{/if}}
            <strong>${fromUserFullName}</strong>
          {{if !isAnonymous}}
            </a>
          {{else}}
            </span>
          {{/if}}
        </li>
        <li class="Item Title">
          ${typeString} (${createdDate})
        </li>
        <li class="Item Description">
          {{html content}}
        </li>
      </ul>
    </div>
    <div class="Actions Hidden">
      <ul>
        {{if isNew}}
        <li class="MarkAsRead">
          <a class="ImageLink" href="javascript: void(0)" onclick="markAsReadFeedbackNotification(${id}); return false;">
            <img title="Пометить как прочитаное" alt="Пометить как прочитаное" src='<%= ResolveUrl("~/Images/Common/MarkNotificationAsRead.png") %>' />
          </a>
        </li>
        {{/if}}
        <li class="Remove">
          <a class="ImageLink" href="javascript: void(0)" onclick="removeFeedbackNotification(${id}); return false;">
            <img title="Удалить" alt="Удалить" src='<%= ResolveUrl("~/Images/Common/Remove.png") %>' />
          </a>
        </li>
      </ul>
    </div>
    <div class="Clear"></div>
  </li>

  </script>

  <script id="feedbackNotificationsScript" language="javascript" type="text/javascript">

    var feedbackNotificationsList = null;

    function initializeFeedbackNotifications() {

      feedbackNotificationsList = new UIList(
        "#feedbackNotificationsList", 
        "#feedbackNotificationTemplate", 
        '<%= ResolveUrl("~/Services/NotificationsService.asmx/GetFeedbackNotifications") %>',
        getFeedbackNotificationsSearchParameters,
        {
          listItemSelector: ".Feedback .DataListItem",
          searchParametersBlockContainerSelector: ".Feedback .SearchParametersBlock",
          totalItemCountContainerSelector: "#lbFeedbackNotificationsTotalItemCount"
        }
      );

      $("#tbFeedbackNotificationsSearch").bind("keypress", function (e) {

        var code = e.keyCode || e.which;
        if (code === Keys.Enter) {

          feedbackNotificationsList.update();
          e.preventDefault();
        }
      });
    }

    function getFeedbackNotificationsSearchParameters() {

      return [{ "paramName": "searchText", "paramValue": $("#tbFeedbackNotificationsSearch").val() }];
    }

    function markAsReadFeedbackNotification(id) {

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/NotificationsService.asmx/MarkAsReadFeedbackNotification") %>',
        params: { "feedbackNotificationId": id },
        success: function (data) {

          if (engine.utilities.checkServerResult(data.d)) {

            $("li#feedbacknotification_" + id).removeClass("Highlight");
            $("li#feedbacknotification_" + id + " div.Actions ul li.MarkAsRead").remove();

            engine.notifications.update(<%= UserManagement.CurrentUser.Id %>);
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function (error) {

          engine.eventHandlers.onServerFailed(error);
        }
      });
    }

    function removeFeedbackNotification(id) {

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/NotificationsService.asmx/RemoveFeedbackNotification") %>',
        params: { "feedbackNotificationId": id },
        success: function (data) {

          if (engine.utilities.checkServerResult(data.d)) {

            var listItem = $("li#feedbacknotification_" + id);

            $(listItem).slideUp(500, function () {

              $(listItem).remove();

              engine.eventHandlers.onServerSuccess("Уведомление о фидбеке удалено.");

              engine.notifications.update(<%= UserManagement.CurrentUser.Id %>);
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

  </script>
  <% } %>
  <% if (AccessManagement.CanExploitConversationNotifications) { %>
  <script id="conversationNotificationTemplate" type="text/x-jquery-tmpl">

  <li id="conversationnotification_${id}" class="DataListItem{{if isNew}} Highlight{{/if}}">
    <div class="Image">
      <a class="ImageLink" href="${fromUserLink}">
        <img title="${fromUserFullName}" alt="${fromUserFullName}" src="${fromUserPhotoPath}" />
      </a>
    </div>
    <div class="Data">
      <ul class="List">
        <li class="Item Title">
          <a href="${fromUserLink}"><strong>${fromUserFullName}</strong></a> - 
          <a href="${toUserLink}"><strong>${toUserFullName}</strong></a> 
          (${directionString})
        </li>
        <li class="Item Title">
          ${typeString} (${createdDate})
        </li>
        <li class="Item Description">
          {{html content}}
        </li>
      </ul>
    </div>
    <div class="Actions Hidden">
      <ul>
        {{if isNew}}
        <li class="MarkAsRead">
          <a class="ImageLink" href="javascript: void(0)" onclick="markAsReadConversationNotification(${id}); return false;">
            <img title="Пометить как прочитаное" alt="Пометить как прочитаное" src='<%= ResolveUrl("~/Images/Common/MarkNotificationAsRead.png") %>' />
          </a>
        </li>
        {{/if}}
        {{if canReply}}
          <li class="Reply">
            <a class="ImageLink" href="javascript: void(0)" onclick="replyToConversationNotification(${id}, ${fromUserId}); return false;">
              <img title="Ответить" alt="Ответить" src='<%= ResolveUrl("~/Images/Common/NotificationReply.png") %>' />
            </a>
          </li>
        {{/if}}
        <li class="Remove">
          <a class="ImageLink" href="javascript: void(0)" onclick="removeConversationNotification(${id}); return false;">
            <img title="Удалить" alt="Удалить" src='<%= ResolveUrl("~/Images/Common/Remove.png") %>' />
          </a>
        </li>
      </ul>
    </div>
    <div class="Clear"></div>
  </li>

  </script>

  <script id="conversationNotificationsScript" language="javascript" type="text/javascript">

    var conversationNotificationsList = null;

    function initializeConversationNotifications() {

      conversationNotificationsList = new UIList(
        "#conversationNotificationsList", 
        "#conversationNotificationTemplate", 
        '<%= ResolveUrl("~/Services/NotificationsService.asmx/GetConversationNotifications") %>',
        getConversationNotificationsSearchParameters,
        {
          listItemSelector: ".Conversation .DataListItem",
          searchParametersBlockContainerSelector: ".Conversation .SearchParametersBlock",
          totalItemCountContainerSelector: "#lbConversationNotificationsTotalItemCount"
        }
      );

      $("#tbConversationNotificationsSearch").bind("keypress", function (e) {

        var code = e.keyCode || e.which;
        if (code === Keys.Enter) {

          conversationNotificationsList.update();
          e.preventDefault();
        }
      });
    }

    function getConversationNotificationsSearchParameters() {

      return [
        { "paramName": "searchText", "paramValue": $("#tbConversationNotificationsSearch").val() },
        { "paramName": "userid", "paramValue": <%= UserManagement.CurrentUser.Id %> }
      ];
    }

    function markAsReadConversationNotification(id) {

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/NotificationsService.asmx/MarkAsReadConversationNotification") %>',
        params: { "conversationNotificationId": id },
        success: function (data) {

          if (engine.utilities.checkServerResult(data.d)) {

            $("li#conversationnotification_" + id).removeClass("Highlight");
            $("li#conversationnotification_" + id + " div.Actions ul li.MarkAsRead").remove();

            engine.notifications.update(<%= UserManagement.CurrentUser.Id %>);
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function(error) {

          engine.eventHandlers.onServerFailed(error);
        }
      });
    }

    function removeConversationNotification(id) {

      $.sendAjax({
        url: '<%= ResolveUrl("~/Services/NotificationsService.asmx/RemoveConversationNotification") %>',
        params: { "conversationNotificationId": id },
        success: function (data) {

          if (engine.utilities.checkServerResult(data.d)) {

            var listItem = $("li#conversationnotification_" + id);

            $(listItem).slideUp(500, function () {

              $(listItem).remove();

              engine.eventHandlers.onServerSuccess("Сообщение удалено.");

              engine.notifications.update(<%= UserManagement.CurrentUser.Id %>);
            });
          }
          else
            engine.eventHandlers.onServerReturnedError();
        },
        error: function(error) {

          engine.eventHandlers.onServerFailed(error);
        }
      });
    }

    function replyToConversationNotification(id, toUserId) {

      engine.notifications.sendMessage(
        <%= UserManagement.CurrentUser.Id %>, 
        toUserId, 
        ".Conversation", 
        function() { 
          conversationNotificationsList.update();
        }); 

      markAsReadConversationNotification(id);
    }

  </script>
  <% } %>

  <script language="javascript" type="text/javascript">

    $(document).ready(function () {

      $.backtotop({
        text: "Вверх"
      });

      <% if (AccessManagement.CanExploitSystemNotifications) { %>
      initializeSystemNotifications();
      <% } %>
      <% if (AccessManagement.CanExploitFeedbackNotifications) { %>
      initializeFeedbackNotifications();
      <% } %>
      <% if (AccessManagement.CanExploitConversationNotifications) { %>
      initializeConversationNotifications();
      <% } %>
    });

    <% if (AccessManagement.CanExploitSystemNotifications) { %>
    function expandCollapseSystemNotifications() {

      if (isVisible($(".System")))
        $(".System").hide();
      else
        $(".System").show();
    }
    <% } %>
    <% if (AccessManagement.CanExploitFeedbackNotifications) { %>
    function expandCollapseFeedbackNotifications() {

      if (isVisible($(".Feedback")))
        $(".Feedback").hide();
      else
        $(".Feedback").show();
    }
    <% } %>
    <% if (ConversationNotificationExpandable) { %>
    function expandCollapseConversationNotifications() {

      if (isVisible($(".Conversation")))
        $(".Conversation").hide();
      else
        $(".Conversation").show();
    }
    <% } %>
  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <% if (AccessManagement.CanExploitSystemNotifications) { %>
  <clubbybook:ContentHeaderControl ID="systemNotificationsContentHeader" runat="server" 
    Title="Уведомления"
    ExpandCollapseClientFunc="expandCollapseSystemNotifications()" />

  <div class="System">
    <div class="SearchBlock">
      <div class="Simple">
        <input type="text" id="tbSystemNotificationsSearch" class="TextBox Find" 
          onfocus='javascript: $(".System span.Prompt").hide();'
          onblur='javascript: $("#tbSystemNotificationsSearch").val() === "" ? $(".System span.Prompt").show() : $(".System span.Prompt").hide();' />
        <span class="Prompt">Поиск</span>
        <div class="Cancel">
          <a class="ImageLink" href="javascript: void(0)" onclick='$("#tbSystemNotificationsSearch").val(""); $("#tbSystemNotificationsSearch").focus();'>
            <img src="<%= ResolveUrl("~/Images/Common/Cancel.png") %>" alt="Очистить ввод" title="Очистить ввод" />
          </a>
        </div>
      </div>
    </div>
    <div class="SearchParametersBlock">
      <p id="lbSystemNotificationsTotalItemCount"></p>
    </div>
    <div class="ContentDataList">
      <ul id="systemNotificationsList"></ul>
    </div>
  </div>

  <div class="SpaceBetween"></div>
  <% } %>
  <% if (AccessManagement.CanExploitFeedbackNotifications) { %>
  <clubbybook:ContentHeaderControl ID="feedbackNotificationsContentHeader" runat="server" 
    Title="Обратная связь"
    ExpandCollapseClientFunc="expandCollapseFeedbackNotifications()" />

  <div class="Feedback">
    <div class="SearchBlock">
      <div class="Simple">
        <input type="text" id="tbFeedbackNotificationsSearch" class="TextBox Find" 
          onfocus='javascript: $(".Feedback span.Prompt").hide();'
          onblur='javascript: $("#tbFeedbackNotificationsSearch").val() === "" ? $(".Feedback span.Prompt").show() : $(".Feedback span.Prompt").hide();' />
        <span class="Prompt">Поиск</span>
        <div class="Cancel">
          <a class="ImageLink" href="javascript: void(0)" onclick='$("#tbFeedbackNotificationsSearch").val(""); $("#tbFeedbackNotificationsSearch").focus();'>
            <img src="<%= ResolveUrl("~/Images/Common/Cancel.png") %>" alt="Очистить ввод" title="Очистить ввод" />
          </a>
        </div>
      </div>
    </div>
    <div class="SearchParametersBlock">
      <p id="lbFeedbackNotificationsTotalItemCount"></p>
    </div>
    <div class="ContentDataList">
      <ul id="feedbackNotificationsList"></ul>
    </div>
  </div>

  <div class="SpaceBetween"></div>
  <% } %>
  <% if (AccessManagement.CanExploitConversationNotifications) { %>
  <clubbybook:ContentHeaderControl ID="conversationNotificationsContentHeader" runat="server" Title="Общение" />

  <div class="Conversation">
    <div class="SearchBlock">
      <div class="Simple">
        <input type="text" id="tbConversationNotificationsSearch" class="TextBox Find" 
          onfocus='javascript: $(".Conversation span.Prompt").hide();'
          onblur='javascript: $("#tbConversationNotificationsSearch").val() === "" ? $(".Conversation span.Prompt").show() : $(".Conversation span.Prompt").hide();' />
        <span class="Prompt">Поиск</span>
        <div class="Cancel">
          <a class="ImageLink" href="javascript: void(0)" onclick='$("#tbConversationNotificationsSearch").val(""); $("#tbConversationNotificationsSearch").focus();'>
            <img src="<%= ResolveUrl("~/Images/Common/Cancel.png") %>" alt="Очистить ввод" title="Очистить ввод" />
          </a>
        </div>
      </div>
    </div>
    <div class="SearchParametersBlock">
      <p id="lbConversationNotificationsTotalItemCount"></p>
    </div>
    <div class="ContentDataList">
      <ul id="conversationNotificationsList"></ul>
    </div>
    <div class="SendMessageContent Hidden">
      <div class="SendMessageContentMessageBlock">
        <textarea class="TextBox" id="tbMessage" rows="4" cols="70"></textarea>
      </div>
      <div class="Buttons">
        <input type="button" class="Button" id="btnSend" title="Отправить" value="Отправить" />
        <input type="button" class="Button lightwindow_close" title="Отмена" value="Отмена" />
      </div>
    </div>
  </div>
  <% } %>

</asp:Content>
