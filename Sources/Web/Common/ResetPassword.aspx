<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="ClubbyBook.Web.Common.ResetPassword" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightvalidation.js") %>'></script>

  <script type="text/javascript" language="javascript">

    $(document).ready(function () {

      var errorString = "Ваши почта неопознаны системой, возможно Вы еще не регистрировались?";

      $("#aspnetForm").validate({
        requiredFields: {
          "#<%= tbEmail.ClientID %>": "Почта не может быть пустой, заполните пожалуйста."
        },
        emailFields: {
          "#<%= tbEmail.ClientID %>": "Почта должна быть в правильно формате."
        },
        onlyFirstError: true,
        svMethod: '<%= ResolveUrl("~/Services/ValidateService.asmx/CheckResetPassword") %>',
        svGetParams: function () {
          return "{ email: '" + $("#<%= tbEmail.ClientID %>").val() + "'}"
        },
        svOnSuccess: function (data) {
          return !engine.utilities.checkServerResult(data.d) ? 
            "Ваша почта не опознана системой. Пользователя с такой почтой не существует." : null;
        }
      });
    });

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Сброс пароля" />

  <% if (!IsPostBack) { %>
  <p>
Если Вы забыли свой пароль, введите свой почтовый ящик, и нажмите "Cбросить" и 
Вам на почту будет выслан новый пароль, с помощью которого Вы сможете войти в систему.
  </p>
  <p>
В случае возникновения сложностей напишите нам на почту 
<a href="mailto:support@clubbybook.com">support@clubbybook.com</a>.
  </p>

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
        <div class="Buttons">
          <asp:Button ID="btnReset" runat="server" Text="Сбросить" CssClass="Button" OnClick="btnReset_Click" />
        </div>
        <div class="Clear"></div>
      </li>
    </ul>
  </div>
  <% } else { %>
  <p>
Вам на почту был выслан новый пароль. Дождитесь письма и 
<a href='<%= RedirectHelper.ResolveUrl(RedirectDirection.Login) %>'>войдите на сайт</a> с помощью нового пароля. 
После чего Вы можете изменить пароль в настройках своего аккаунта.
  </p>
  <p>
В случае возникновения сложностей напишите нам на почту 
<a href="mailto:support@clubbybook.com">support@clubbybook.com</a>.
  </p>
  <% } %>

</asp:Content>