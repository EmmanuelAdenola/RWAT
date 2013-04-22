$(function () {
    $(document).on("submit", "#registerform", function () { return false; });
    $(document).on("submit", "#loginform", function () { return false; });

    $(document).on("click", "#register", function () {
        var self = this;
        $.ajax({
            type: "POST",
            url: $(this).attr('data-url'),
            data: $("#registerform").serialize(),
            success: function (data) {
                if (data["success"] !== undefined) {
                    window.location = $(self).attr('data-redirecturl');
                }
                else {
                    $("#registerwrapper").html(data);
                }
            }
        });

    });

    $(document).on("click", "#login", function () {
        var self = this;
        $.ajax({
            type: "POST",
            url: $(this).attr('data-url'),
            data: $("#loginform").serialize(),
            success: function (data) {
                if (data["success"] !== undefined) {
                    window.location = $(self).attr('data-redirecturl');
                }
                else {
                    $("#loginwrapper").html(data);
                }
            }
        });

    });
});