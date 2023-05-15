<%@ Page EnableViewState="false" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClubbyBook.Web.Common.Default" %>
<%@ Import Namespace="ClubbyBook.Web.Utilities" %>
<%@ Import Namespace="ClubbyBook.Common" %>
<%@ Import Namespace="ClubbyBook.Controllers" %>

<%@ Register Src="~/Controls/ContentHeaderControl.ascx" TagName="ContentHeaderControl" TagPrefix="clubbybook" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="cphHead" runat="server">

  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/lightlist.css") %>' type="text/css" />
  <link rel="stylesheet" href='<%= ResolveUrl("~/Styles/lightcarousel.css") %>' type="text/css" />

  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.tmpl.min.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.caroufredsel.min.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightlist.js") %>'></script>
  <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.lightcarousel.js") %>'></script>

  <script id="topNewsTemplate" type="text/x-jquery-tmpl">

  <li id="topnews_${id}" class="DataListItem TopNews">
    <div class="Data OnlyData">
      <ul class="List">
        <li class="Item Label">
          ${createdDateShort} - <a href="${viewNewsLink}">${title}</a>
        </li>
      </ul>
    </div>
    <div class="Clear"></div>
  </li>

  </script>

  <script id="bookTemplate" type="text/x-jquery-tmpl">
    <li>
      <a class="ImageLink" href="${viewBookLink}">
        <img src="${coverPath}" title="${authorsAndTitle}" alt="${authorsAndTitle}" width="80" height="125" />
      </a>
    </li>
  </script>

  <script language="javascript" type="text/javascript">

    var topNewsList = null;
    <% if (UserManagement.IsAuthenticated) { %>var recommendedBooksCarousel = null;<% } %>
    var mostPopularBooksOnSaleCarousel = null;
    var mostPopularBooksInExchangeCarousel = null;
    var latestBooksCarousel = null;

    $(document).ready(function () {

      topNewsList = new UIList(
        "#topNewsList",
        "#topNewsTemplate",
        '<%= ResolveUrl("~/Services/NewsService.asmx/GetTopNews") %>',
        function () { return []; },
        ".DataListItem .TopNews"
      );

      <% if (UserManagement.IsAuthenticated) { %>
      recommendedBooksCarousel = new UICarousel(
        "#recommendedBooks",
        "#bookTemplate",
        '<%= ResolveUrl("~/Services/BooksService.asmx/GetRecommendedBooks") %>',
        function() { 
          return { "userId": <%= UserManagement.CurrentUser.Id %> };
        },
        "Вам необходимо добавить книги в библиотеку и выставить им рейтинг."
      );
      <% } %>

      mostPopularBooksOnSaleCarousel = new UICarousel(
        "#mostPopularBooksOnSale",
        "#bookTemplate",
        '<%= ResolveUrl("~/Services/BooksService.asmx/GetMostPopularBooksOnSale") %>',
        function() { return {}; },
        UITextConstants.ListEmpty
      );

      mostPopularBooksInExchangeCarousel = new UICarousel(
        "#mostPopularBooksInExchange",
        "#bookTemplate",
        '<%= ResolveUrl("~/Services/BooksService.asmx/GetMostPopularBooksInExchange") %>',
        function() { return {}; },
        UITextConstants.ListEmpty
      );

      latestBooksCarousel = new UICarousel(
        "#latestBooks",
        "#bookTemplate",
        '<%= ResolveUrl("~/Services/BooksService.asmx/GetLatestAddedBooks") %>',
        function() { return {}; },
        UITextConstants.ListEmpty
      );
    });

  </script>

</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="cphBody" runat="server">

  <clubbybook:ContentHeaderControl ID="contentHeader" runat="server" Title="Добро пожаловать в наше сообщество" />
  <p>
    ClubbyBook.com - это не просто новая социальная сеть для тех, кто любит читать книги, но и возможность обмениваться,
    продавать, и покупать интересные книги, общаться с людьми близкими Вам по духу. 
    А также это первая украинская социальная сеть любителей книг.
    Вы сможете получить рекомендованный список книг,  которые стоит прочесть, вести свою online библиотеку и 
    никогда не забывать сюжеты любимых книг, которые уже когда-то прочитали.
  </p>
  <p>
    Здесь Вы найдете интересных собеседников, а с ними – новые и интересные Вам книги. Сможете найти хозяина книг, 
    которые Вам уже не нужны, но кому-то могут пригодиться!
  </p>

  <div class="SpaceBetween"></div>
  <clubbybook:ContentHeaderControl ID="topNewsContentHeader" runat="server" Title="Новости" />
  <div class="ContentDataList TopNews">
    <ul id="topNewsList"></ul>
  </div>

  <% if (UserManagement.IsAuthenticated) { %>
  <div class="SpaceBetween"></div>
  <clubbybook:ContentHeaderControl ID="recommendedBooksContentHeader" runat="server" Title="Рекомендованные книги:" />
  <div id="recommendedBooks"></div>
  <% } %>

  <div class="SpaceBetween"></div>
  <clubbybook:ContentHeaderControl ID="latestBooksHeader" runat="server" Title="Последние добавленные книги:" />
  <div id="latestBooks"></div>

  <div class="SpaceBetween"></div>
  <clubbybook:ContentHeaderControl ID="mostPopularBooksOnSaleContentHeader" runat="server" Title="Лучшее в продаже:" />
  <div id="mostPopularBooksOnSale"></div>

  <div class="SpaceBetween"></div>
  <clubbybook:ContentHeaderControl ID="mostPopularBooksInExchangeContentHeader" runat="server" Title="Лучшее на обмен:" />
  <div id="mostPopularBooksInExchange"></div>

</asp:Content>
