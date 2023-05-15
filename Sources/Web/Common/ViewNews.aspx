<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ViewNews.aspx.cs" Inherits="ClubbyBook.Web.Common.ViewNews" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>
<%@ Import Namespace="ClubbyBook.UI" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" />

  <div class="ViewRows">
    <div class="ViewData OnlyData">
      <ul class="ViewDataList">
        <li class="ViewDataListItem BottomLine">
          <div class="Label">Дата создания:</div>
          <div class="Value"><%= UIUtilities.GetFullDateString(Entity.CreatedDate) %></div>
          <div class="Clear"></div>
        </li>
        <li class="ViewDataListItem Description">
          <%= UIUtilities.PrepareTextContent(Entity.Message) %>
        </li>
      </ul>
    </div>
    <div class="Clear"></div>
  </div>

</asp:Content>
