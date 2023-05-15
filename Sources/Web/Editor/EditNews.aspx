<%@ Page EnableViewState="true" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="EditNews.aspx.cs" Inherits="ClubbyBook.Web.Editor.EditNews" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightvalidation.js") %>'></script>

  <script type="text/javascript" language="javascript">

    $(document).ready(function () {

      $("#aspnetForm").validate({
        requiredFields: {
          "#<%= tbTitle.ClientID %>": "Заголовок новости является обьязательным полем. Заполните пожалуйста.",
          "#<%= tbMessage.ClientID %>": "Текст новости является обьязательным полем. Заполните пожалуйста."
        }
      });
    });

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Добавление новости" />

  <div class="EditRows">
    <div class="Error"></div>
    <ul class="EditList">
      <li class="EditListItem">
        <div class="Label">Заголовок:</div>
        <div class="Value">
          <asp:TextBox ID="tbTitle" runat="server" TextMode="SingleLine" CssClass="TextBox" Width="560px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Текст:</div>
        <div class="Value">
          <asp:TextBox ID="tbMessage" runat="server" TextMode="MultiLine" Rows="30" CssClass="TextBox" Width="560px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Buttons">
          <asp:Button ID="btnSave" runat="server" Text="Сохранить" CssClass="Button" OnClick="btnSave_Click" />
          <asp:Button ID="btnCancel" runat="server" Text="Отмена" CssClass="Button" UseSubmitBehavior="false" OnClick="btnCancel_Click" />
        </div>
        <div class="Clear"></div>
      </li>
    </ul>
  </div>

</asp:Content>
