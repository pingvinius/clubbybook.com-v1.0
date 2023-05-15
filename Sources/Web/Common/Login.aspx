<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ClubbyBook.Web.Common.Login" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightvalidation.js") %>'></script>

  <script type="text/javascript" language="javascript">

    $(document).ready(function () {

      var errorString = "Ваши почта и пароль не опознаны системой авторизации.";

      $("#aspnetForm").validate({
        requiredFields: {
          "#<%= tbEmail.ClientID %>": errorString,
          "#<%= tbPassword.ClientID %>": errorString
        },
        emailFields: {
          "#<%= tbEmail.ClientID %>": errorString
        },
        onlyFirstError: true,
        svMethod: '<%= ResolveUrl("~/Services/ValidateService.asmx/CheckLogin") %>',
        svGetParams: function () {
          return "{ email: '" + $("#<%= tbEmail.ClientID %>").val() + "', password: '" + $("#<%= tbPassword.ClientID %>").val() + "' }"
        },
        svOnSuccess: function (data) {
          return !engine.utilities.checkServerResult(data.d) ? errorString : null;
        }
      });
    });

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Вход" />

  <div class="EditRows">

    <div class="Error"></div>

    <ul class="EditList">
      <li class="EditListItem">
        <div class="Label">Почта:</div>
        <div class="Value">
          <asp:TextBox ID="tbEmail" runat="server" 
            MaxLength = "50" 
            TextMode="SingleLine" 
            CssClass="TextBox" 
            Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Пароль:</div>
        <div class="Value">
          <asp:TextBox ID="tbPassword" runat="server" 
            MaxLength = "30" 
            TextMode="Password" 
            CssClass="TextBox" 
            Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label"></div>
        <div class="Value">
          <asp:CheckBox ID="chkRememberMe" runat="server" Text="Запомнить меня" CssClass="CheckBox" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label"></div>
        <div class="Value">
          <a href='<%= RedirectHelper.ResolveUrl(RedirectDirection.ResetPassword) %>'>Забыли пароль?</a>
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Buttons">
          <asp:Button ID="btnSignIn" runat="server" Text="Вход" CssClass="Button" OnClick="btnSignIn_Click" />
        </div>
        <div class="Clear"></div>
      </li>
    </ul>

  </div>

</asp:Content>