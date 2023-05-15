<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InfoMessageControl.ascx.cs" Inherits="ClubbyBook.Web.Controls.InfoMessageControl" %>
<%@ Import Namespace="ClubbyBook.Web.Controls" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>

<% if (Type != InfoMessageType.None) { %>

  <div id="<%= ClientID %>" class="InfoMessageControlContent">
  <% if (Type == InfoMessageType.ViewBookRegistrationMessage) { %>
    <p>
      Добро пожаловать читатель! Для того, что бы купить книгу, продать книгу, обменять книгу или добавить книгу в свою онлайн библиотеку Вам необходимо 
      <a href="<%= RedirectHelper.ResolveUrl(RedirectDirection.Registration, true) %>">зарегистрироваться</a>. Регистрация не займет много времени (почта и пароль). 
      Будем рады видеть Вас в <a href="<%= RedirectHelper.ResolveUrl(RedirectDirection.About) %>"> нашем сообществе</a>!
    </p>
  <% } else if (Type == InfoMessageType.ViewAuthorRegistrationMessage) { %>
    <p>
      Добро пожаловать читатель! Для того, что бы Вам открылись все возможности сайта необходимо пройти несложную форму <a href="<%= RedirectHelper.ResolveUrl(RedirectDirection.Registration, true) %>">регистрации</a>. 
      Будем рады видеть Вас в <a href="<%= RedirectHelper.ResolveUrl(RedirectDirection.About) %>"> нашем сообществе</a>!
    </p>
  <% } else if (Type == InfoMessageType.ViewProfileFillFieldsMessage) { %>
    <p>
      Добро пожаловать читатель! Для того, что бы Вам открылись все возможности сайта необходимо заполнить Ваш город проживания. 
      А для того, что бы другие читатели больше доверяли - свое имя, фамилию или псевдоним.
    </p>
  <% } else if (Type == InfoMessageType.ViewBookPleaseRateBook) { %>
    <p>
      Для того, что бы получать качественные рекомендации "что бы почитать?", поставьте оценку книге!
    </p>
  <% } else if (Type == InfoMessageType.ViewAuthorPleaseRateAuthor) { %>
    <p>
      Для того, что бы получать качественные рекомендации "что бы почитать?", поставьте оценку автору!
    </p>
  <% } else if (Type == InfoMessageType.AddCommentRequirements) { %>
  <p>
    Для того, что бы добавить свой комментарий, необходимо <a href="<%= RedirectHelper.ResolveUrl(RedirectDirection.Registration) %>">зарегистрироватся</a>! 
    Это - просто! Не стой в стороне, учавствуй в обсуждении!
  </p>
  <% } else if (Type == InfoMessageType.CustomText) { %>
    <p>
      <%= CustomText %>
    </p>
  <% } %>
  </div>
<% } %>
