<%@ Page EnableViewState="true" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="EditorTools.aspx.cs" Inherits="ClubbyBook.Web.Editor.EditorTools" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Инструменты редактора" />

  <ul class="SimpleList">
    <li>
      <a href='<%= RedirectHelper.ResolveUrl(RedirectDirection.PrePostValidation) %>'>Валидация и трансформация текста перед постом</a>
    </li>
  </ul>

</asp:Content>
