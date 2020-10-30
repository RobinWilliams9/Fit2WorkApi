// Global page load
$(document).ready(function () {
    if (window.location.href.indexOf("Client/Details") > -1) {
        initialiseClientDetails();
    }
});
// Client/Details page
function initialiseClientDetails() {
    console.log("initialiseClientDetails")
    var userCount = $("#clientUserCount").text();
    if (userCount === "0") {
        $("#noUsersNotice").show();
    } else {
        $("#sendSMSNotice").show();
    }
    // Page events    
    $("#btnSelectNoUsers").click(function () {
        $('#tableUserList').find('input[type="checkbox"]').each(function () {
            $(this).prop('checked', false);
        });
        updateSendSmsButton(this);
    });
    $("#btnSelectAllUsers").click(function () {
        $('#tableUserList').find('input[type="checkbox"]').each(function () {
            $(this).prop('checked', true);
        });
        updateSendSmsButton(this);
    });
    $("#btnSelectAllRegisteredUsers").click(function () {
        $('#tableUserList tr').filter(':has(td[data-user-registered="True"])').each(function () {
            $(this).find("input[type=checkbox]").prop('checked', true);
        });
        updateSendSmsButton(this);
    });
    $("#btnSelectAllNoSMSUsers").click(function () {
        $('#tableUserList tr').filter(':has(td[data-user-sms="True"])').each(function () {
            $(this).find("input[type=checkbox]").prop('checked', true);
        });
        updateSendSmsButton(this);
    });
    $("input[data-user-select]").change(function () {
        updateSendSmsButton(this);
    });
    $("input[data-userid]").change(function () {
        updateSendSmsButton(this);
    });
    // Page functions
    function updateSendSmsButton(sender) {
        var all = $('#tableUserList').find('input[type="checkbox"]');
        var selected = $('#tableUserList').find('input[type="checkbox"]:checked');
        if (selected.length > 0) {
            $("#btnSendSms").removeClass("disabled");
            $("#btnSendSms").removeAttr("disabled");
            $("#btnSendSms").text("Send " + selected.length + " SMS");
        } else {
            $("#btnSendSms").addClass("disabled");
            $("#btnSendSms").attr("disabled","disabled");
            $("#btnSendSms").text("Send SMS");
        }
        if (all.length === selected.length) {
            $("#allUsersSMSNotice").show();
            $("#btnSendSms").addClass("btn-warning");
        } else {
            $("#allUsersSMSNotice").hide();
            $("#nonRegisteredUsersSMSNotice").hide()
            $("#btnSendSms").removeClass("btn-warning");
        }
        if (sender.id === "btnSelectAllRegisteredUsers") {
            if (selected.length > 0) {
                $("#nonRegisteredUsersSMSNotice").show();
            } else {
                $("#nonRegisteredUsersSMSNotice").hide();
            }
        }
    }
}
// Global alert handling
function showGlobalError(msg) {
    $("#globalAlertMessage").text(msg);
    $("#globalAlert").removeClass("alert-info");
    $("#globalAlert").removeClass("alert-success");
    $("#globalAlert").removeClass("alert-warning");
    $("#globalAlert").addClass("alert-danger");
    $("#globalAlert").fadeIn(300).delay(5000).fadeOut(300);
}
function showGlobalInfo(msg) {
    $("#globalAlertMessage").text(msg);
    $("#globalAlert").removeClass("alert-success");
    $("#globalAlert").removeClass("alert-warning");
    $("#globalAlert").removeClass("alert-danger");
    $("#globalAlert").addClass("alert-info");
    $("#globalAlert").fadeIn(300).delay(7000).fadeOut(300);
}
function showGlobalSuccess(msg) {
    $("#globalAlertMessage").text(msg);
    $("#globalAlert").removeClass("alert-info");
    $("#globalAlert").removeClass("alert-danger");
    $("#globalAlert").removeClass("alert-warning");
    $("#globalAlert").addClass("alert-success");
    $("#globalAlert").fadeIn(300).delay(5000).fadeOut(300);
}