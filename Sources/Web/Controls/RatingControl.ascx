<%@ Control Language="C#" EnableViewState="true" AutoEventWireup="true" CodeBehind="RatingControl.ascx.cs" Inherits="ClubbyBook.Web.Controls.RatingControl" %>

<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.raty.min.js") %>'></script>

<script type="text/javascript" language="javascript">

  $(document).ready(function () {

    $(".RatingControl .Rating").raty({
      path: '<%= ResolveUrl("~/Images/Common/") %>',
      number: 5,
      readOnly: <% if (IsReadOnly) { %> true <% } else { %> false <% } %>,
      starOff: "StarOff.png",
      starOn: "StarOn.png",
      width: "120px",
      hintList: [":(((", ":(", ":!", ":)", ":)))"],
      noRatedMsg: "Еще никто не голосовал :(",
      click: function (score) {

        $.sendAjax({
          url: '<%= ResolveUrl(string.Format("~/Services/RatingService.asmx/{0}", SetUserRatingServiceMethod)) %>',
          params: {
            id: <%= EntityIdInt %>,
            newValue: score
          },
          success: function (data) {
            updateCommonRatingValue();
            <% if (!System.String.IsNullOrEmpty(InfoMessageControlIdToRemove)) { %>$("#<%= InfoMessageControlIdToRemove %>").hide();<% } %>
          }
        });

        updateUserRatingValue(score);
      }
    });

    updateCommonRatingValue();
    updateUserRatingValue();


    function updateCommonRatingValue() {

      var commonAuthorRating = getCommonAuthorRating();
      $.fn.raty.start(commonAuthorRating, ".RatingControl .Rating");
    }

    function updateUserRatingValue(value) {

      if (!value)
        value = 0;

      <% if (ClubbyBook.Controllers.UserManagement.IsAuthenticated) { %>
      if (value === 0)
        value = getUserAuthorRating();
      <% } %>

      var userVoiceTextControl = $("#lbUserVoice");
      var isReadOnly = "<%= IsReadOnly %>".toLowerCase() == "true";

      if (value > 0 && !isReadOnly) {
        userVoiceTextControl.show();
        userVoiceTextControl.text("Ваша оценка " + value);
      }
      else
        userVoiceTextControl.hide();
    }

    function getCommonAuthorRating() {

      var ratingValue = 0;

      $.sendAjax({
        url: '<%= ResolveUrl(string.Format("~/Services/RatingService.asmx/{0}", GetCommonRatingServiceMethod)) %>',
        params: { id: <%= EntityIdInt %> },
        async: false,
        success: function (data) {
          if (isDefinedAndNotNull(data.d))
            ratingValue = data.d;
        }
      });

      return ratingValue;
    }

    function getUserAuthorRating() {

      var userRatingValue = 0;

      $.sendAjax({
        url: '<%= ResolveUrl(string.Format("~/Services/RatingService.asmx/{0}", GetUserRatingServiceMethod)) %>',
        params: { id: <%= EntityIdInt %> },
        async: false,
        success: function (data) {
          if (isDefinedAndNotNull(data.d))
            userRatingValue = data.d;
        }
      });

      return userRatingValue;
    }
  });

</script>

<div class="RatingControl">
  <ul>
    <li class="Control">
      <div class="Rating"></div>
    </li>
    <li class="Text">
      <label id="lbUserVoice"></label>
    </li>
  </ul>
</div>