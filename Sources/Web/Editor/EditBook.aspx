<%@ Page EnableViewState="true" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="EditBook.aspx.cs" Inherits="ClubbyBook.Web.Editor.EditBook" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>

<%@ Register Assembly="CustomControls" Namespace="ClubbyBook.CustomControls" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/ImageUploaderControl.ascx" TagName="ImageUploaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/token-input.css") %>' type="text/css" />

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightvalidation.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tokeninput.js") %>'></script>

  <script type="text/javascript" language="javascript">

    $(document).ready(function () {

      $("#aspnetForm").validate({
        requiredFields: {
          "#<%= tbTitle.ClientID %>": "Заголовок книги является обьязательным полем. Заполните пожалуйста.",
          "#<%= tbDescription.ClientID %>": "Описание книги является обьязательным полем. Заполните пожалуйста.",
          "#<%= hfAuthors.ClientID %>": "Автор книги является объязательным полем. Заполните пожалуйста."
        }
      });

      $("#tbAuthors").tokenInput('<%= ResolveUrl("~/Services/AuthorsService.asmx/GetAutoCompleteAuthors") %>',
        {
          hintText: "Введите автора",
          noResultsText: "Ничего не найдено",
          searchingText: "Поиск автора...",
          preventDuplicates: true,
          queryParam: "prefixText",
          prePopulate: <%= AuthorsJavascriptArray %>,
          onAdd: function (data) {

            if (data) {

              var ids = $("#<%= hfAuthors.ClientID %>").val().toArrayList();
              ids.push(data.value);
              $("#<%= hfAuthors.ClientID %>").val(ids.join(","));
            }
          },
          onDelete: function (data) {

            if (data) {

              var ids = $("#<%= hfAuthors.ClientID %>").val().toArrayList();
              var index = ids.indexOf(data.value);
              if (index >= 0)
                ids.splice(index, 1);
              $("#<%= hfAuthors.ClientID %>").val(ids.join(","));
            }
          }
        }
      );
    });

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Добавление новой книги" />

  <div class="EditRows">
    <div class="Error"></div>
    <ul class="EditList">
      <% if (CanModifyConfirmedState) { %>
      <li class="EditListItem BottomLine">
        <div class="Label">Книга проверена:</div>
        <div class="Value" style="margin-top: 6px;">
          <asp:CheckBox ID="chkConfirmedBook" runat="server" />
        </div>
        <div class="Clear"></div>
      </li>
      <% } %>
      <li class="EditListItem">
        <div class="Label">Название:</div>
        <div class="Value">
          <asp:TextBox ID="tbTitle" runat="server" TextMode="SingleLine" CssClass="TextBox" Width="400px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Название оригинала:</div>
        <div class="Value">
          <asp:TextBox ID="tbOriginalTitle" runat="server" TextMode="SingleLine" CssClass="TextBox" Width="400px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Авторы:</div>
        <div class="Value">
          <input type="text" id="tbAuthors" class="TextBox" style="width: 400px;" />
          <asp:HiddenField ID="hfAuthors" runat="server" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Жанр:</div>
        <div class="Value">
          <clubbybook:FixedDropDownList ID="ddlGenres" runat="server" CssClass="DropDownList" Width="410px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Описание:</div>
        <div class="Value">
          <asp:TextBox ID="tbDescription" runat="server" TextMode="MultiLine" Rows="25" CssClass="TextBox" Width="560px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Сборник:</div>
        <div class="Value">
          <asp:CheckBox ID="chkCollection" runat="server" CssClass="CheckBox" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Обложка:</div>
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
