"use strict";

$(function () {
    var executeCode = function () {
        var code = $("#input").val();
        $.ajax({
            url: "/Home/Execute",
            data: { session: sessionID, inp: code },
            dataType: "json",
            success: function (returned) {
                $("#output").val(returned);
            }
        });
    };
    $("#input").bind("keypress", function (e) {
        if (e.keyCode == 13) {
            executeCode();
            return false;
        }
    });
    $("#form").submit(function () {
        executeCode();
        return false;
    });
});