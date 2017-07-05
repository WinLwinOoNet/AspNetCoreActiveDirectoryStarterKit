/*!
 * ASP.NET Core Active Directory Starter Kit 
 */
function displayGridError(e) {
    if (e.errors) {
        if ((typeof e.errors) == 'string') {
            //single error
            //display the message
            window.alerts.error(e.errors);
        } else {
            //array of errors
            //source: http://docs.telerik.com/kendo-ui/getting-started/using-kendo-with/aspnet-mvc/helpers/grid/faq#how-do-i-display-model-state-errors
            var message = "The following errors have occurred:";
            //create a message containing all errors.
            $.each(e.errors, function (key, value) {
                if (value.errors) {
                    message += "\n";
                    message += value.errors.join("\n");
                }
            });
            //display the message
            window.alerts.error(message);
        }
    } else {
        alert("An error occurred while processing your request. " +
            "If these issue persists, then please contact the customer service.");
    }
}

function menuSelect(e) {
    // Show loading icon
    kendo.ui.progress($(".content-wrapper"), true);
}

$(function () {

    $("input[data-val-length-max]").each(function (index, element) {
        var length = parseInt($(this).attr("data-val-length-max"));
        $(this).prop("maxlength", length);
    });

    $(".show-progress").click(function () {
        kendo.ui.progress($(".content-wrapper"), true);
    });

    // Display * although field itself doesn't have client-side validation; we later validate those fields at server-side.
    $("label.field-required-label").append(" <span class=\"field-required\">*</span>");

    $("input:text[data-val-required],input:password[data-val-required],select[data-val-required],textarea[data-val-required]").each(function () {
        var label = $("label[for=\"" + $(this).attr("id") + "\"]");
        if (label.text().length > 0) {
            label.append(" <span class=\"field-required\">*</span>");
        }
    });

    var showAlert = function (alert) {
        var template = _.template("<div class='alert <%= alertClass %> alert-dismissable'>" +
            "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>" +
            "<%= message %></div>");
        var alertElement = $(template(alert));
        $(".alert-container").append(alertElement);
        window.setTimeout(function () { alertElement.fadeOut(); }, 10000);
    };

    window.alerts = {
        showAlert: showAlert,
        success: function (message) { showAlert({ alertClass: "alert-success", message: message }); },
        info: function (message) { showAlert({ alertClass: "alert-info", message: message }); },
        warning: function (message) { showAlert({ alertClass: "alert-warning", message: message }); },
        error: function (message) { showAlert({ alertClass: "alert-danger", message: message }); }
    };

    // http://stackoverflow.com/questions/12107263/why-is-validationsummarytrue-displaying-an-empty-summary-for-property-errors
    if ($(".validation-summary-errors li:visible").length === 0) {
        $(".validation-summary-errors").hide();
    }
});
