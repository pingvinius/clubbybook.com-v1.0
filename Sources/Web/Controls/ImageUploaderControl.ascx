<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="ImageUploaderControl.ascx.cs" Inherits="ClubbyBook.Web.Controls.ImageUploaderControl" %>

<link rel="stylesheet" href='<%= ResolveUrl("~/Styles/imgareaselect.css") %>' type="text/css" />

<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.ajaxfileupload.js") %>'></script>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery.imgareaselect.min.js") %>'></script>

<script type="text/javascript" language="javascript">

  var maxImageWidth = 450;
  var maxImageHeight = 450;

  var errorMessages = [
    "Укажите изображение для загрузки!",
    "Размер изображения не должен превышать 1 Mb.",
    "Доступные расширения: \".jpg\", \".jpeg\", \".png\", \".bmp\".",
    "При загрузке изображения произошла непредвиденная ошибка."
  ];

  $(document).ready(function () {

    updateImage();

    var ias = $("#imgUploaded").imgAreaSelect({
      handles: true,
      instance: true,
      aspectRatio: "192:300",
      onSelectChange: updateThumbnail
    });

    $("#lnkUploadImage").bind("click", function () {

      $(".ImageUploader div.UploadError").hide();

      var currentImageSrc = $("#<%= hfImageTempPath.ClientID %>").val();
      var isFirstUpload = !currentImageSrc || currentImageSrc == "";

      $.ajaxFileUpload({
        url: '<%= ResolveUrl("~/Services/ImageUploaderService.asmx/Upload") %>',
        secureUri: false,
        id: "<%= fuUploader.ClientID %>",
        onSuccess: function (responseData, status) {

          if (isDefinedAndNotNull(responseData) && responseData.result === "0") {

            var imageWidth, imageHeight;
            var responseWidth = parseInt(responseData.width, 10);
            var responseHeight = parseInt(responseData.height, 10);
            var koef = responseWidth / responseHeight;

            if (responseWidth >= responseHeight) {

              imageWidth = maxImageWidth;
              imageHeight = Math.round(maxImageWidth / koef);
            }
            else {

              imageWidth = Math.round(maxImageHeight * koef);
              imageHeight = maxImageHeight;
            }

            $("#<%= hfImageWidth.ClientID %>").val(imageWidth);
            $("#<%= hfImageHeight.ClientID %>").val(imageHeight);
            $("#<%= hfImageTempPath.ClientID %>").val(responseData.filePath);

            updateImage();
          }
          else
            handleUploadError(responseData ? responseData.result : 4);

          updateImgAreaSelect(isFirstUpload);
        },
        onError: function (exception, status) {

          handleUploadError(4);
          updateImgAreaSelect(isFirstUpload);
        }
      });

      return false;
    });

    function updateImage() {

      var uploadedImage = $("#imgUploaded");

      var newImageSrc = $("#<%= hfImageTempPath.ClientID %>").val();

      if (newImageSrc && newImageSrc != "") {

        var imageWidth = $("#<%= hfImageWidth.ClientID %>").val();
        if (!imageWidth || imageWidth == "")
          imageWidth = 0;

        var imageHeight = $("#<%= hfImageHeight.ClientID %>").val();
        if (!imageHeight || imageHeight == "")
          imageHeight = 0;

        $(uploadedImage).attr("width", imageWidth);
        $(uploadedImage).attr("height", imageHeight);
        $(uploadedImage).attr("src", newImageSrc);

        $(".ImageUploader div.Images").show();

        var thumbnailSize = getThumbnailSize();

        var thumbnailScale = thumbnailSize.width / thumbnailSize.height;
        var imageScale = imageWidth / imageHeight;
        var selection = new Object();

        if (imageScale > thumbnailScale) {

          selection.height = imageHeight;
          selection.width = thumbnailScale * imageHeight;
        }
        else {
          selection.height = imageWidth / thumbnailScale;
          selection.width = imageWidth;
        }

        selection.x1 = imageWidth / 2 - selection.width / 2;
        selection.y1 = imageHeight / 2 - selection.height / 2;
        selection.x2 = imageWidth / 2 + selection.width / 2;
        selection.y2 = imageHeight / 2 + selection.height / 2;

        updateThumbnail(uploadedImage, selection);

        if (ias)
          ias.setSelection(selection.x1, selection.y1, selection.x2, selection.y2);
      }
      else
        $(".ImageUploader div.Images").hide();
    }

    function updateThumbnail(img, selection) {

      var thumbnailImage = $("#imgThumbnail");

      $(thumbnailImage).attr("src", $(img).attr("src"));

      if ($(img).attr("src") == "" || !selection || !selection.width || !selection.height)
        return;

      var imageWidth = $(img).innerWidth();
      var imageHeight = $(img).innerHeight();

      var thumbnailSize = getThumbnailSize();

      var scaleX = thumbnailSize.width / selection.width;
      var scaleY = thumbnailSize.height / selection.height;

      $(thumbnailImage).css({
        width: Math.round(scaleX * imageWidth) + "px",
        height: Math.round(scaleY * imageHeight) + "px",
        marginLeft: "-" + Math.round(scaleX * selection.x1) + "px",
        marginTop: "-" + Math.round(scaleY * selection.y1) + "px"
      });

      $("#<%= hfThumbnailX.ClientID %>").val(selection.x1);
      $("#<%= hfThumbnailY.ClientID %>").val(selection.y1);
      $("#<%= hfThumbnailWidth.ClientID %>").val(selection.width);
      $("#<%= hfThumbnailHeight.ClientID %>").val(selection.height);
    }

    function handleUploadError(errorCode, exception) {

      if (errorCode >= 1 && errorCode <= 4) {

        $(".ImageUploader div.UploadError p").empty();
        $(".ImageUploader div.UploadError p").text(errorMessages[errorCode - 1]);
        $(".ImageUploader div.UploadError").show();
      }
    }

    function updateImgAreaSelect(isFirstUpload) {

      if (!isFirstUpload && ias)
        ias.update();
    }

    function getThumbnailSize() {

      return { width: $(".ImageUploader div.Images div.Thumbnail").width(),
        height: $(".ImageUploader div.Images div.Thumbnail").height()
      };
    }
  });

</script>

<div class="ImageUploader">

  <div class="Info">
    <p>
      Для того, чтобы загрузить изображение, необходимо выбрать файл и нажать "Загрузить". 
      Доступные расширения: ".jpg", ".jpeg", ".png", ".bmp".
      Доступный размер изображения не должен превышать 1 Mb.
    </p>
  </div>
  <div class="UploadError"><p></p></div>
  <div class="Uploader">
    <div class="FileUploader">
      <asp:FileUpload ID="fuUploader" runat="server" />
    </div>
    <div class="UploaderLink">
      <a id="lnkUploadImage" href="javascript: void(0)">Загрузить</a>
    </div>
    <div class="Clear"></div>
    <asp:HiddenField ID="hfImageTempPath" runat="server" />
    <asp:HiddenField ID="hfImageHeight" runat="server" />
    <asp:HiddenField ID="hfImageWidth" runat="server" />
    <asp:HiddenField ID="hfThumbnailX" runat="server" />
    <asp:HiddenField ID="hfThumbnailY" runat="server" />
    <asp:HiddenField ID="hfThumbnailWidth" runat="server" />
    <asp:HiddenField ID="hfThumbnailHeight" runat="server" />
  </div>
  <div class="Images">
    <div class="OrigImage"><img id="imgUploaded" src="" alt="Оригинал" /></div>
    <div class="Thumbnail"><img id="imgThumbnail" src="" alt="Миниатюра" /></div>
    <div class="Clear"></div>
  </div>
  <div class="Clear"></div>

</div>