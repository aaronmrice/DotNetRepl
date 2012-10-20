"use strict";

$(function () {
    var executeCode = function () {
        var code = $("#input").val();
        $("div.output").spin({ lines: 13 });
        $.ajax({
            url: "/Execute",
            data: { session: sessionID, inp: code },
            dataType: "json",
            success: function (returned) {
                $("#output").val(returned);

                var lines = returned.split("\r\n").length;
                var rows = $("#output").attr("rows");
                if (lines > rows) {
                    $("#output").attr("rows", lines + 1);
                }

                $("div.output .spinner").remove();
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