<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="ClubbyBook.Web.Common.Error" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Ошибка" />

  <div class="ErrorContent">
    <p><%= ErrorMessage %></p>
  </div>

</asp:Content>
