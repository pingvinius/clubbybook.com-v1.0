<%@ Page EnableViewState="true" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="EditProfile.aspx.cs" Inherits="ClubbyBook.Web.Account.EditProfile" %>

<%@ Register Assembly="CustomControls" Namespace="ClubbyBook.CustomControls" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/EnumDropDownList.ascx" TagName="EnumDropDownList" TagPrefix="clubbybook" %>
<%@ Register Src="~/Controls/ImageUploaderControl.ascx" TagName="ImageUploaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightvalidation.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightcascading.js") %>'></script>

  <script type="text/javascript" language="javascript">

    $(document).ready(function () {

      $("#<%= ddlDistrict.ClientID %>").cascading({
        url: '<%= ResolveUrl("~/Services/LocationService.asmx/GetCountryDistricts") %>',
        params: ["countryId"],
        getValues: function () {
          return [-1];
        },
        getSelectedValue: function () {
          return <%= Entity.City != null ? Entity.City.DistrictId : -1 %>;
        },
        loadingText: "Загрузка",
        promptText: "(Выбирите область)",
        errorText: "Ошибка передачи данных",
        getItem: function (object) {
          return { text: object.text, value: object.value };
        }
      });


      $("#<%= ddlCity.ClientID %>").cascading({
        url: '<%= ResolveUrl("~/Services/LocationService.asmx/GetDistrictCities") %>',
        params: ["districtId"],
        getValues: function () {
          return [$("#<%= ddlDistrict.ClientID %>").val()];
        },
        getSelectedValue: function () {
          return <%= Entity.CityId %>;
        },
        parentId: "<%= ddlDistrict.ClientID %>",
        loadingText: "Загрузка",
        promptText: "(Выбирите город)",
        errorText: "Ошибка передачи данных",
        getItem: function (object) {
          return { text: object.text, value: object.value };
        }
      });

      $("#<%= ddlCity.ClientID %>").change(function (element) {

        if (isDefinedAndNotNull(element.currentTarget))
          $("#<%= hfCity.ClientID %>").val($(element.currentTarget).val());
      });
    });

  </script>


</asp:Content>

<asp:Content ID="сontent" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" />

  <div class="EditRows">
    <ul class="EditList">
      <li class="EditListItem">
        <div class="Label">Имя:</div>
        <div class="Value">
          <asp:TextBox ID="tbName" runat="server" TextMode="SingleLine" CssClass="TextBox" Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Фамилия:</div>
        <div class="Value">
          <asp:TextBox ID="tbSurname" runat="server" TextMode="SingleLine" CssClass="TextBox" Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Псевдоним:</div>
        <div class="Value">
          <asp:TextBox ID="tbNickname" runat="server" TextMode="SingleLine" CssClass="TextBox" Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Пол:</div>
        <div class="Value">
          <clubbybook:EnumDropDownList ID="ddlGender" runat="server" Width="150px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Область:</div>
        <div class="Value">
          <clubbybook:FixedDropDownList ID="ddlDistrict" runat="server" CssClass="DropDownList" Width="250px" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Город:</div>
        <div class="Value">
          <clubbybook:FixedDropDownList ID="ddlCity" runat="server" CssClass="DropDownList" Width="250px" />
          <asp:HiddenField ID="hfCity" runat="server" Value="-1" />
        </div>
        <div class="Clear"></div>
      </li>
      <li class="EditListItem">
        <div class="Label">Аватарка:</div>
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
