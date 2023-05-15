<%@ Page EnableViewState="true" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ResolveDuplicateBook.aspx.cs" Inherits="ClubbyBook.Web.Editor.ResolveDuplicateBook" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/token-input.css") %>' type="text/css" />

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightvalidation.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tokeninput.js") %>'></script>

  <script type="text/javascript" language="javascript">

    $(document).ready(function () {

      $("#aspnetForm").validate({
        requiredFields: {
          "#<%= hfDuplicateBook.ClientID %>": "Выберите основную книгу для замены.",
        },
        customValidation: function () {

          choosedId = parseInt($("#<%= hfDuplicateBook.ClientID %>").val(), 10);
          if (isDefinedAndNotNull(choosedId) && choosedId != NaN && choosedId === <%= Entity.Id %>)
            return ["Вы должны выбрать книгу которая является оригиналом, а не ту же книгу что и текущая."];
          return null;
        }
      });

      $("#tbBooksAndAuthors").tokenInput('<%= ResolveUrl("~/Services/BooksService.asmx/GetAutoCompleteBooksAndAuthors") %>', {
        hintText: "Введите книгу",
        noResultsText: "Ничего не найдено, добавте ее самостоятельно",
        searchingText: "Поиск книги...",
        preventDuplicates: true,
        maxItemsCount: 1,
        queryParam: "prefixText",
        onAdd: function (data) {

          if (data)
            $("#<%= hfDuplicateBook.ClientID %>").val(data.value);
        },
        onDelete: function (data) {

          if (data)
            $("#<%= hfDuplicateBook.ClientID %>").val("");
        }
      });
    });

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" />

  <div class="EditRows">
    <div class="Error"></div>
    <ul class="EditList">
      <li class="EditListItem">
        <div class="Label">Сослаться на книгу:</div>
        <div class="Value">
          <input type="text" id="tbBooksAndAuthors" class="TextBox" style="width: 400px;" />
          <asp:HiddenField ID="hfDuplicateBook" runat="server" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Buttons">
          <asp:Button ID="btnSave" runat="server" Text="Исправить" CssClass="Button" OnClick="btnResolve_Click" />
          <asp:Button ID="btnCancel" runat="server" Text="Отмена" CssClass="Button" UseSubmitBehavior="false" OnClick="btnCancel_Click" />
        </div>
        <div class="Clear"></div>
      </li>
    </ul>
  </div>

  <div class="EditInformation">
    <p>
      Примечание: нельзя сопоставить книгу с более чем одной.
    </p>
  </div>

</asp:Content>
