<%@ Page EnableViewState="true" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="EditAccount.aspx.cs" Inherits="ClubbyBook.Web.Account.EditAccount" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightvalidation.js") %>'></script>

  <script type="text/javascript" language="javascript">

    $(document).ready(function () {

      $("#aspnetForm").validate({
        requiredFields: {
          "#<%= tbEmail.ClientID %>": "Почта является обьязательным полем. Заполните пожалуйста."
        },
        emailFields: {
          "#<%= tbEmail.ClientID %>": "Почта должна быть в формате name@domine.name."
        },
        customValidation: function () {

          var errors = [];
          if ($("#<%= tbNewPassword.ClientID %>").val() !== $("#<%= tbNewPassword2.ClientID %>").val())
            errors.push("Новый пароль повторен не правильно.");
          return errors;
        }
      });
    });

  </script>

</asp:Content>

<asp:Content ID="сontent" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" />

  <div class="EditRows">
    <div class="Error"></div>
    <ul class="EditList">
      <li class="EditListItem">
        <div class="Label">Почта:</div>
        <div class="Value">
          <asp:TextBox ID="tbEmail" runat="server" 
            TextMode="SingleLine" 
            CssClass="TextBox" 
            Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Новый пароль:</div>
        <div class="Value">
          <asp:TextBox ID="tbNewPassword" runat="server" 
            TextMode="Password" 
            CssClass="TextBox" 
            Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Еще раз:</div>
        <div class="Value">
          <asp:TextBox ID="tbNewPassword2" runat="server" 
            TextMode="Password" 
            CssClass="TextBox" 
            Width="250px" />
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
