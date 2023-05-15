<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="true" CodeBehind="ContentHeaderControl.ascx.cs" Inherits="ClubbyBook.Web.Controls.ContentHeaderControl" %>

<div class="ContentHeader">

  <div class="<% if (SmallTitle) { %>SmallTitle<% } else { %>Title<% } %>">
    <% if (AllowExpandCollapse) { %>
    <a class="HiddenLink" title="Свернуть/Развернуть" href="javascript: void(0)" onclick="<%= ExpandCollapseClientFunc %>; return false;">
    <% } %>
    <% if (!SmallTitle) { %><h1><% } else { %><h2><% } %>
    <%= Title%>
    <% if (!SmallTitle) { %></h1><% } else { %></h2><% } %>
    <% if (AllowExpandCollapse) { %></a><% } %>
  </div>

  <div class="Actions">

    <asp:Repeater ID="rpActions" runat="server" OnItemDataBound="rpActions_ItemDataBound">
      <HeaderTemplate>
        <ul>
      </HeaderTemplate>
      <ItemTemplate>
        <li>
          <a id="lnkAction" runat="server"></a>
        </li>
      </ItemTemplate>
      <FooterTemplate>
        </ul>
      </FooterTemplate>
    </asp:Repeater>

  </div>

  <div class="Clear"></div>

</div>