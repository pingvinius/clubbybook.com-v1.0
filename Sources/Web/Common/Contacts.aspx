<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Contacts.aspx.cs" Inherits="ClubbyBook.Web.Common.Contacts" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Контакты" />

  <p>
    Местонахождение: Украина, Харьков
  </p>
  <br />
  <p>
    В случае возникновения вопросов или проблем, или у Вас есть пожелания, 
    Вы можете сообщить нам на почту <a href="mailto:support@clubbybook.com">support@clubbybook.com</a>
    или воспользоваться ссылкой «Обратная связь» внизу страницы.
  </p>
  <p>
    Если у Вас есть какие-либо предложения по поводу партнерства или размещения рекламы просьба 
    написать нам на почту <a href="mailto:admin@clubbybook.com">admin@clubbybook.com</a>.
  </p>

</asp:Content>
