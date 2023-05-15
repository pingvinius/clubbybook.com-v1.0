<%@ Page EnableViewState="true" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="EditAuthor.aspx.cs" Inherits="ClubbyBook.Web.Editor.EditAuthor" %>
<%@ Import Namespace="ClubbyBook.Common.Enums" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/EnumDropDownList.ascx" TagName="EnumDropDownList" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/ImageUploaderControl.ascx" TagName="ImageUploaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightvalidation.js") %>'></script>

  <script type="text/javascript" language="javascript">

    $(document).ready(function () {

      $("#aspnetForm").validate({
        requiredFields: {
          "#<%= tbFullName.ClientID %>": "Имя автора является обьязательным полем. Заполните пожалуйста.",
          "#<%= tbShortDescription.ClientID %>": "Краткое описание является обьязательным полем. Заполните пожалуйста.",
          "#<%= tbBiography.ClientID %>": "Биография является обьязательным полем. Заполните пожалуйста."
        }
      });

      $($("select[id*='<%= ddlType.ClientID %>']").get(0)).bind("change", function (e) {

        validateFieldSetByAuthorType();
      });

      $($("select[id*='<%= ddlGender.ClientID %>']").get(0)).bind("change", function (e) {

        validateGenderEnding();

        if ($($("select[id*='<%= ddlGender.ClientID %>']").get(0)).val() !== "<%= GenderType.NotSpecified.ToString() %>") {

          $($("select[id*='<%= ddlType.ClientID %>']").get(0)).val("<%= AuthorType.Man.ToString() %>");
        }
      });

      validateFieldSetByAuthorType();
    });

    function validateFieldSetByAuthorType() {

      if ($($("select[id*='<%= ddlType.ClientID %>']").get(0)).val() === "<%= AuthorType.PublishingHouse.ToString() %>") {
        $(".OnlyManField").hide();
      }
      else {
        $(".OnlyManField").show();

        validateGenderEnding();
      }
    }

    function validateGenderEnding() {

      if ($($("select[id*='<%= ddlGender.ClientID %>']").get(0)).val() === "<%= GenderType.Female.ToString() %>") {

        $($("#<%= tbBirthdayYear.ClientID %>").parents(".EditListItem").children(".Label").get(0)).text("Родилась:");
        $($("#<%= tbDeathYear.ClientID %>").parents(".EditListItem").children(".Label").get(0)).text("Умерла:");
      }
      else {

        $($("#<%= tbBirthdayYear.ClientID %>").parents(".EditListItem").children(".Label").get(0)).text("Родился:");
        $($("#<%= tbDeathYear.ClientID %>").parents(".EditListItem").children(".Label").get(0)).text("Умер:");
      }
    }

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Добавление нового автора" />

  <div class="EditRows">
    <div class="Error"></div>
    <ul class="EditList">
      <li class="EditListItem">
        <div class="Label">Полное имя:</div>
        <div class="Value">
          <asp:TextBox ID="tbFullName" runat="server" TextMode="SingleLine" CssClass="TextBox" Width="400px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Тип:</div>
        <div class="Value">
          <clubbybook:EnumDropDownList ID="ddlType" runat="server" Width="150px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem OnlyManField">
        <div class="Label">Пол:</div>
        <div class="Value">
          <clubbybook:EnumDropDownList ID="ddlGender" runat="server" Width="150px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem OnlyManField">
        <div class="Label">Родился:</div>
        <div class="Value">
          <asp:TextBox ID="tbBirthdayYear" runat="server" TextMode="SingleLine" CssClass="TextBox" Width="50px" /> г.
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem OnlyManField">
        <div class="Label">Умер:</div>
        <div class="Value">
          <asp:TextBox ID="tbDeathYear" runat="server" TextMode="SingleLine" CssClass="TextBox" Width="50px" /> г.
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Краткое описание:</div>
        <div class="Value">
          <asp:TextBox ID="tbShortDescription" runat="server" TextMode="MultiLine" Rows="10" CssClass="TextBox" Width="560px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Биография:</div>
        <div class="Value">
          <asp:TextBox ID="tbBiography" runat="server" TextMode="MultiLine" Rows="25" CssClass="TextBox" Width="560px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Фото:</div>
        <div class="Value">
          <clubbybook:ImageUploaderControl ID="imageUploader" runat="server" />
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
