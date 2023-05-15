<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="SiteMap.aspx.cs" Inherits="ClubbyBook.Web.Common.SiteMap" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Карта сайта" />

  <% if (UserManagement.IsAdminAuthenticated || UserManagement.IsEditorAuthenticated) { %>
    <div class="UpdateSiteMapBlock">
      <asp:Button ID="btnUpdateSiteMap" runat="server" Text="Обновить карту сайта" CssClass="Button" OnClick="btnUpdateSiteMap_Click" />
      <asp:Button ID="btnUpdateBooksLastModifiedDate" runat="server" Text="Обновить даты книг" CssClass="Button" OnClick="btnUpdateBooksLastModifiedDate_Click" />
      <asp:Button ID="btnUpdateAuthorsLastModifiedDate" runat="server" Text="Обновить даты авторов" CssClass="Button" OnClick="btnUpdateAuthorsLastModifiedDate_Click" />
    </div>
  <% } %>

  <asp:SiteMapDataSource ID="siteMapDataSource" runat="server" ShowStartingNode="true" />
  <asp:TreeView ID="tvSiteMap" runat="server" DataSourceID="siteMapDataSource" />

</asp:Content>
