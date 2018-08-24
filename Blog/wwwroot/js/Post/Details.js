let topCommentPage = 1;

function loadTopComments() {
    topCommentPage += 1;
    $.ajax({
        url: "/Post/LoadTopComments/",
        data: {
            postId: $(".PostBlock").attr("id"),
            topCommentPage
        },
        success: function (data) {
            let commentsList = data["comments"].replace(/&quot;/ig, '"');
            commentsList = commentsList.replace(/'/gi, '"');
            commentsList = JSON.parse(commentsList);

            AddComments(commentsList, data["hasNextPage"]);
        },
        error: function (jqXHR, textStatus) {
            console.log(jqXHR);
            console.log(textStatus);
        }
    });
}

function AddComments(commentsList, hasNextPage) {
    if (hasNextPage === "False") {
        $("#loadmore").remove();
    }

    for (comment of commentsList) {
        let domObject = `
          <div class="CommentBlock d-flex flex-row border p-2" id="${comment.commentId}">
            <div class="mr-5">
                <p class="arrow up">&uarr;</p>
                <p class="score">0</p>
                <p class="arrow down">&darr;</p>
            </div>
            <div class="d-flex flex-column">
                <small>${comment.userName} ${comment.whenPosted}</small>
                <p>${comment.content}</p>
                <small class="text-muted">parent reply</small>
            </div>
        </div>
            `;

        $(".comments").append(domObject);
    }
}