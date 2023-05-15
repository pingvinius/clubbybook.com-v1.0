<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="ClubbyBook.Web.Common.Registration" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightvalidation.js") %>'></script>

  <script type="text/javascript" language="javascript">

    $(document).ready(function () {

      $("#aspnetForm").validate({
        requiredFields: {
          "#<%= tbEmail.ClientID %>": "Почта является обьязательным полем. Заполните пожалуйста.",
          "#<%= tbPassword.ClientID %>": "Пароль является обьязательным полем. Заполните пожалуйста."
        },
        emailFields: {
          "#<%= tbEmail.ClientID %>": "Почта должна быть в формате name@domine.name."
        },
        customValidation: function () {

          if (!$("#chkAgreeWithAgreement").is(":checked"))
            return ["Для того что бы зарегистрироватся вам необходимо согласится с условиями пользовательского соглашения."];
          return null;
        },
        svMethod: '<%= ResolveUrl("~/Services/ValidateService.asmx/CheckRegistration") %>',
        svGetParams: function () {
          return "{ email: '" + $("#<%= tbEmail.ClientID %>").val() + "', password: '" + $("#<%= tbPassword.ClientID %>").val() + "' }"
        },
        svOnSuccess: function (data) {

          if (data.d === 1)
            return "Все поля должны быть заполнены.";
          else if (data.d === 2)
            return "Такой пользователь уже существует.";
          else if (data.d === 3)
            return "Пользователь был удален. Свяжитесь с нами и мы восcтановим Ваш аккаунт (см. контакты).";

          return null;
        }
      });
    });

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Регистрация" />

  <div class="EditRows">

    <div class="Error"></div>

    <ul class="EditList">
      <li class="EditListItem">
        <div class="Label">Почта:<span class="RequaredField">*</span></div>
        <div class="Value">
          <asp:TextBox ID="tbEmail" runat="server" MaxLength = "50" TextMode="SingleLine" CssClass="TextBox" Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Пароль:<span class="RequaredField">*</span></div>
        <div class="Value">
          <asp:TextBox ID="tbPassword" runat="server" MaxLength = "30" TextMode="Password" CssClass="TextBox" Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label"></div>
        <div class="Value CheckBox">
            <input type="checkbox" id="chkAgreeWithAgreement" />
            <label>Я прочел и согласен с условиями <a href='<%= RedirectHelper.ResolveUrl(RedirectDirection.UserAgreement) %>'>пользовательского соглашения.</a></label>
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Buttons">
          <asp:Button ID="btnRegistration" runat="server" Text="Регистрация" CssClass="Button" onclick="btnRegistration_Click" />
        </div>
        <div class="Clear"></div>
      </li>
    </ul>

  </div>

</asp:Content>
