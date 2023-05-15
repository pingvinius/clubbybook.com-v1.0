/**
 * jQuery Ajax helper Plugin
 * 
 * @author  Oleg Kashpur
 * @email pingvinius@gmail.com
 * @twitter pingvinius
 * @version 1.00
 * 
 */

(function ($) {

  $.extend({

    sendAjax: function (options) {

      var defaults = {
        url: "",
        params: {},
        async: true,
        success: function (data) { },
        error: function (error) { }
      };

      var opts = $.extend({}, defaults, options);

      var ajaxDataArray = new Array();
      $.each(opts.params, function (key, value) {
        if (value instanceof Array)
          ajaxDataArray.push("'" + key + "': " + JSON.stringify(value) + "");
        else
          ajaxDataArray.push("'" + key + "': '" + value + "'");
      });
      var ajaxData = "{" + ajaxDataArray.join(',') + "}";

      $.ajax({
        type: "POST",
        url: opts.url,
        data: ajaxData,
        async: opts.async,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, textStatus, jqXHR) {

          opts.success(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {

          // Don't process error with code 0
          if (jqXHR.status === 0) 
            return;

          var statusCode = jqXHR.status;
          var message = statusCode + " - " + errorThrown;
          var type = "";
          var stackTrace = "";

          if (isJSON(jqXHR.responseText)) {

            var errorObj = $.parseJSON(jqXHR.responseText);
            if (errorObj !== null) {

              message += "<br />" + errorObj.Message;
              type = errorObj.ExceptionType;
              stackTrace = errorObj.StackTrace.replace("\r\n", "<br />");
            }
          }

          opts.error({
            code: statusCode,
            message: message,
            type: type,
            stackTrace: stackTrace
          });
        }
      });

      return null;
    }
  });

})(jQuery);