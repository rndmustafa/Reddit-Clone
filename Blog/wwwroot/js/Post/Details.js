let topCommentPage = 1;

function loadTopComments() {
    topCommentPage += 1;
    fetch(`/Post/LoadTopComments/?postId=${$(".PostBlock").attr("id")}&topCommentPage=${topCommentPage}`)
        .then(res => res.json())
        .then((data) => {
            let commentsList = JSON.parse(data["comments"]);
            AddComments(commentsList, data["hasNextPage"]);
        })
        .catch(error => {
            console.log(error);
        })
}

function AddComments(commentsList, hasNextPage) {
    if (hasNextPage === "False") {
        $("#loadmore").remove();
    }

    for (comment of commentsList) {
        let domObject = getDomComment(comment);

        $(".comments").append(domObject);
    }
}

function getDomComment(comment) {
    let domComment = `
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
        </div>`;

    if (comment.childComments.length) {
        domComment += "<ul>"
        for (let child of comment.childComments) {
            domComment += "<li>"
            domComment += getDomComment(child);
            domComment += "</li>"
        }
        domComment += "</ul>"
    }

    return domComment;
}