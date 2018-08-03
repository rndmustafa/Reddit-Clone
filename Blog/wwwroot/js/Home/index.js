﻿$(".arrow").click(function (eventObject) {
    let dir = 0;
    if ($(this).hasClass("up") && !$(this).hasClass("active")) {
        dir = 1;
    }
    else if ($(this).hasClass("down") && !$(this).hasClass("active")) {
        dir = -1;
    }
    let postId = $(this).parents(".PostBlock").attr("id");
    vote($(this), postId,dir);
});

function vote(jqueryArrow, postId, dir) {
    $.ajax({
        type: "GET",
        url: "Home/Vote",
        data: {postId, dir},
        contentType: "application/json",
        dataType: "json",
        success: function (data) {
            changeArrowColors(jqueryArrow);
            jqueryArrow.siblings(".score").text(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            $("#VoteAuthWarning").removeClass("opacity-0");
        }
    });
}

function changeArrowColors(jqueryArrow) {
    if (jqueryArrow.hasClass("up") && !jqueryArrow.hasClass("active")) {
        jqueryArrow.addClass("active");
        jqueryArrow.siblings(".arrow").removeClass("active");
    }
    else if (jqueryArrow.hasClass("down") && !jqueryArrow.hasClass("active")) {
        jqueryArrow.addClass("active");
        jqueryArrow.siblings(".arrow").removeClass("active");
    }
    else {
        jqueryArrow.removeClass("active");
    }
}

function closeWarningBox() {
    $("#VoteAuthWarning").addClass("opacity-0");
}