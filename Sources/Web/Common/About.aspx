<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="ClubbyBook.Web.Common.About" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="О проекте" />

  <p>
    <strong>ClubbyBook.com</strong> - это социальная сеть любителей книг, которая ориентирована на покупку, продажу, 
    обмен книгами между пользователями. Если Вы любите читать книги - Вы попали туда, куда нужно :)
  </p>
  <br />
  <p>
    Наш сайт позволит Вам выполнять три самых необходимых действия: 
  </p>
  <p class="LargeIndented">
    1) вести свою библиотеку онлайн, принимать участие в формировании рейтинга книг, 
    получать рекомендации интересных Вам книг; 
  </p>
  <p class="LargeIndented">
    2) продавать и обменивать книги, которые Вам уже не нужны, отдавая их в хорошие руки;
  </p>
  <p class="LargeIndented">
    3) позаимствовать или купить за меньшую цену книгу, о которой давно мечтали.
  </p>
  <p>
    Помимо всех вкусных возможностей сайта, Вы сможете просто повстречать много интересных людей.
    И это еще не все! У нас есть огромное количество других предложений!
    <a href='<%= RedirectHelper.ResolveUrl(RedirectDirection.Registration) %>'>Зарегистрируйся</a> сейчас и проверь!
  </p>
  <br />
  <p>
    Все это и многое другое ждет Вас на сайте лучшей социальной сети книголюбов ClubbyBook.com. 
    Почему мы лучшие?! Сайт постоянно развивается. Сообщество растет. Мы работаем только ради пользователя. 
    Напиши пожелание - и мы его обязательно выполним. Мы пользуемся только новыми технологиями. 
    А если Вы уже зашли сюда, то это значит, что мы далеко не последние в результатах поисковика. 
    Хотя, впрочем, судить Вам.
  </p>
  <br />
  <p>
    <i>Книга - чистейшая сущность человеческой души. Чарлз Лэм</i>
  </p>
  <br />
  <p>
    Читайте! Общайтесь! Делитесь! Наслаждайтесь! Всегда рады видеть Вас на нашем сайт! 
    Книги в Ваших ругах могут сделать этот мир добрее!
  </p>
  <br />
  <p>
    <i>Дом, в котором нет книг, подобен телу, лишенному души. Марк Туллий Цицерон</i>
  </p>
</asp:Content>
