(function ($) {

  $.extend({

    ajaxFileUpload: function (options) {

      var defaultOptions = {
        url: "",
        id: "",
        secureUri: false,
        onStart: function () { },
        onEnd: function () { },
        onSend: function () { },
        onSuccess: function (responseData, status) { },
        onCompleted: function (responseData, status) { },
        onError: function (exception, status) { }
      };

      options = $.extend({}, $.ajaxSettings, defaultOptions, options);

      var id = new Date().getTime();
      var form = createUploadForm(id, options.id);
      var io = createUploadIframe(id, options.secureUri);

      var frameId = "jUploadFrame" + id;
      var formId = "jUploadForm" + id;

      // Watch for a new set of requests
      if (!jQuery.active++)
        options.onStart();

      var requestDone = false;
      var responseData = {};

      options.onSend();

      // Wait for a response to come back
      var uploadCallback = function (isTimeout) {

        var io = document.getElementById(frameId);
        try {
          if (io.contentWindow) {

            responseData.responseText = io.contentWindow.document.body ? io.contentWindow.document.body.innerHTML : null;
            responseData.responseXML = io.contentWindow.document.XMLDocument ? io.contentWindow.document.XMLDocument : io.contentWindow.document;
          } else if (io.contentDocument) {

            responseData.responseText = io.contentDocument.document.body ? io.contentDocument.document.body.innerHTML : null;
            responseData.responseXML = io.contentDocument.document.XMLDocument ? io.contentDocument.document.XMLDocument : io.contentDocument.document;
          }
        }
        catch (e) {
          options.onError(e);
        }

        if (responseData || isTimeout === "timeout") {

          requestDone = true;

          var status = "success";
          var results = null;

          try {

            if (isTimeout === "timeout")
              throw "timeout";

            results = convertResult(options, responseData);
            options.onSuccess(results, status);
          }
          catch (e) {

            status = "error";
            options.onError(e, status);
          }

          if (! --jQuery.active)
            options.onEnd();

          // Process result
          options.onCompleted(results, status);

          $(io).unbind()

          setTimeout(function () {

            try {
              $(io).remove();
              $(form).remove();
            }
            catch (e) {
              options.onError(e);
            }

          }, 100);

          responseData = null;
        }
      };

      // Timeout checker
      if (options.timeout > 0) {

        setTimeout(function () {
          // Check to see if the request is still happening
          if (!requestDone)
            uploadCallback("timeout");
        }, options.timeout);
      }

      try {

        var form = jQuery("#" + formId);
        $(form).attr("action", options.url);
        $(form).attr("method", "POST");
        $(form).attr("target", frameId);
        if (form.encoding)
          $(form).attr("encoding", "multipart/form-data");
        else
          $(form).attr("enctype", "multipart/form-data");

        $(form).submit();

      }
      catch (e) {
        options.onError(e);
      }

      $("#" + frameId).load(uploadCallback);

      return { abort: function () { } };
    }
  });


  function createUploadIframe(id, uri) {

    var frameId = "jUploadFrame" + id;
    var iframeHtml = '<iframe id="' + frameId + '" name="' + frameId + '" style="position:absolute; top:-9999px; left:-9999px"';
    if (window.ActiveXObject) {

      if (typeof uri === "boolean")
        iframeHtml += ' src="' + 'javascript:false' + '"';
      else if (typeof uri === "string")
        iframeHtml += ' src="' + uri + '"';
    }
    iframeHtml += " />";
    $(iframeHtml).appendTo(document.body);

    return $("#" + frameId).get(0);
  }

  function createUploadForm(id, fileElementId) {

    var formId = "jUploadForm" + id;
    var fileId = "jUploadFile" + id;
    var form = $('<form  action="" method="POST" name="' + formId + '" id="' + formId + '" enctype="multipart/form-data"></form>');
    var oldElement = $("#" + fileElementId);
    var newElement = $(oldElement).clone();

    $(oldElement).attr("id", fileId);
    $(oldElement).before(newElement);
    $(oldElement).appendTo(form);

    $(form).css("position", "absolute");
    $(form).css("top", "-1200px");
    $(form).css("left", "-1200px");
    $(form).appendTo("body");

    return form;
  }

  function convertResult(options, responseData) {

    if (responseData) {

      var firstChildren = $(responseData.responseXML).children().get(0);
      if (isDefined(firstChildren)) {

        var data = null;
        if (isDefined(firstChildren.textContent))
          data = firstChildren.textContent;
        else
          data = firstChildren.innerText;

        return eval("(" + data + ")");
      }
    }

    return "";
  }

})(jQuery);