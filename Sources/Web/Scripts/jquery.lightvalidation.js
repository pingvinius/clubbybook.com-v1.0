/**
 * jQuery Light Validation Plugin
 * 
 * @author  Oleg Kashpur
 * @email pingvinius@gmail.com
 * @twitter pingvinius
 * @version 1.00
 * 
 */

(function ($) {

  $.fn.validate = function (options) {

    var defaults = {
      errorsContainer: "Error",
      requiredFields: {},
      emailFields: {},
      customValidation: function () { },
      onlyFirstError: false,
      svMethod: "",
      svGetParams: function () { return ""; },
      svType: "POST",
      svContentType: "application/json; charset=utf-8",
      svDataType: "json",
      svOnSuccess: function (data) { },
      svFailedMessage: "An error has been occured during the server validation."
    };

    var opts = $.extend({}, defaults, options);

    this.each(function () {
      validateInternal($(this), opts);
    });

    return this;
  };

  //////////////////////////////////////////////////////////////////

  const emailRegularExpression = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;

  //////////////////////////////////////////////////////////////////

  function validateInternal(form, options) {

    $(form).bind("submit", function (event) {

      clearErrors(options);

      try {

        if (!performRequiredFieldsValidation(options))
          return false;

        if (!performEmailFieldsValidation(options))
          return false;

        if (!performCustomValidation(options))
          return false;

        if (!performServerValidation(options))
          return false;
      }
      catch (error) {
        addError(options, "An error has been occured during the validation: " + error + ".");
      }
      finally {

        if (hasErrors(options)) {

          showErrors(options);
          return false;
        }
      }

      return true;
    });
  }

  //////////////////////////////////////////////////////////////////

  function showErrors(options) {

    getErrorsContainer(options).fadeIn(300);
  }

  function clearErrors(options) {

    getErrorsList(options).empty();
    getErrorsContainer(options).hide();
  }

  function hasErrors(options) {

    return getErrorsList(options).find("li").length > 0;
  }

  function addError(options, message) {

    if (options.onlyFirstError && getErrorsList(options).find("li").length >= 1)
      return;
    getErrorsList(options).append("<li>" + message + "</li>");
  }

  function getErrorsContainer(options) {

    var errorsContainer = $("." + options.errorsContainer);
    if (!errorsContainer)
      throw "Could not find errors container."
    return $(errorsContainer);
  }

  function getErrorsList(options) {

    var errorsContainer = getErrorsContainer(options);
    var errorsList = errorsContainer.find("ul");
    if (!errorsList || errorsList.length === 0) {
      errorsContainer.append("<ul></ul>");
      errorsList = errorsContainer.find("ul");
    }

    return $(errorsList);
  }

  //////////////////////////////////////////////////////////////////

  function performRequiredFieldsValidation(options) {

    var isFirstError = true;

    $.each(options.requiredFields, function (fieldId, errorMessage) {

      if (!checkRequiredValid(fieldId)) {

        if (isFirstError) {
          $(fieldId).focus();
          isFirstError = false;
        }

        addError(options, errorMessage);
      }
    });

    return !hasErrors(options);
  }

  function performEmailFieldsValidation(options) {

    var isFirstError = true;

    $.each(options.emailFields, function (fieldId, errorMessage) {

      if (!checkEmailValid(fieldId)) {

        if (isFirstError) {
          $(fieldId).focus();
          isFirstError = false;
        }

        addError(options, errorMessage);
      }
    });

    return !hasErrors(options);
  }

  function performCustomValidation(options) {

    var customErrors = options.customValidation();
    if (customErrors) {

      $.each(customErrors, function (index, value) {
        addError(options, value);
      });
    }

    return !hasErrors(options);
  }

  function performServerValidation(options) {

    if (options.svMethod !== "") {

      var params = options.svGetParams();

      $.ajax({
        cache: false,
        async: false,
        type: options.svType,
        url: options.svMethod,
        data: params,
        contentType: options.svContentType,
        dataType: options.svDataType,
        success: function (data, textStatus, jqXHR) {

          var serverErrors = options.svOnSuccess(data);
          if (serverErrors) {

            if (typeof(serverErrors).toLowerCase() === "string")
              serverErrors = [serverErrors]; // transform to array

            $.each(serverErrors, function (index, value) {
              addError(options, value);
            });
          }
        },
        error: function (jqXHR, textStatus, errorThrown) {

          addError(options, options.svFailedMessage);
        }
      });
    }

    return !hasErrors(options);
  }

  //////////////////////////////////////////////////////////////////

  function checkEmailValid(fieldId) {

    if (fieldId)
      return emailRegularExpression.test($(fieldId).val());

    return true;
  }

  function checkRequiredValid(fieldId) {

    if (fieldId)
      return $(fieldId).val().length !== 0;

    return true;
  }

})(jQuery);