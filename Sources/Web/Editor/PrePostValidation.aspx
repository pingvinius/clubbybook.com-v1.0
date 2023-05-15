<%@ Page EnableViewState="true" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="PrePostValidation.aspx.cs" Inherits="ClubbyBook.Web.Editor.PrePostValidation" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Валидация текста перед постом" />

  <% if (IsPostBack) { %>
  <div class="PrePostValidationDoneBlock">
    <ul>
      <li><a href='<%= RedirectHelper.ResolveUrl(RedirectDirection.PrePostValidation) %>'>Проверить что-то еще мой господин(госпожа)?</a></li>
      <li><a href='<%= RedirectHelper.ResolveUrl(RedirectDirection.EditBook) %>' target="_blank">Добавление книги</a></li>
      <li><a href='<%= RedirectHelper.ResolveUrl(RedirectDirection.EditAuthor) %>' target="_blank">Добавление автора</a></li>
      <li><a href='<%= RedirectHelper.ResolveUrl(RedirectDirection.EditNews) %>' target="_blank">Добавление новости</a></li>
    </ul>
    <div class="BottomLine"></div>
    <asp:Literal runat="server" ID="lbPreparedText"></asp:Literal>
  </div>
  <% } else { %>
  <div class="PrePostValidationBlock">
    <asp:TextBox ID="tbPrePostText" CssClass="TextBox" TextMode="MultiLine" runat="server" Rows="40"></asp:TextBox>
    <asp:Button ID="btnTransform" runat="server" CssClass="Button" Text="Преобразовать" OnClick="btnTransform_Click" />
  </div>
  <% } %>

</asp:Content>
