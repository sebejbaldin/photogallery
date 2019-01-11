'use strict';

var comments = document.getElementById('comments_box');
var commentText = document.getElementById('commentText');

commentText.addEventListener('input', () => {
    document.getElementById('charCount').innerText = commentText.value.length;
});
//var postId = document.getElementById('postId').value;

var connection = new signalR.HubConnectionBuilder().withUrl('/Comments').build();

connection.on('ReceiveComment', (email, text, insertDate) => {
    appendComment(email, text, new Date(insertDate).toLocaleString());
});

connection.on('EraseComment', (comment_id) => {
    document.getElementById('comment_' + comment_id).remove();
});

document.getElementById('sendComment').addEventListener('click', () => {
    connection.invoke('SendComment', username, commentText.value, new Date(), postId);
    let payload = {
        picture_Id: postId,
        email: username,
        text: commentText.value
    };
    commentText.value = '';
    fetch('/api/v1/comments', {
        method: 'POST',
        credentials: 'include',
        headers: {
            "Content-Type": 'application/json'
        },
        mode: "cors",
        cache: "no-cache",
        body: JSON.stringify(payload)
    }).catch(err => {
        console.error(err);
    });
});

connection.start()
    .then(function () {
        connection.invoke('AddToGroup', postId);
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

function appendComment(email, text, creationDate) {
    let container = document.createElement('div');
    text = text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    container.setAttribute('class', 'col-12');
    container.innerHTML = getComment(email, text, creationDate);
    comments.appendChild(container);
}

function getComment(email, text, creationDate, isMine) {
    let comm = `<div class="row">
                <div class="col-1">
                    <i class="fas fa-user fa-2x"></i>
                </div>
                <div class="col-11">
                    <div class="card" style="width: 100%;">
                        <div class="card-body">
                            <h5 class="card-title">${email}, ${creationDate}</h5>
                            <p class="card-text">${text}</p>`;
    if (isMine) {
        comm += `<button class="btn btn-link mod" style="font-family: 'Comic Sans MS', cursive, sans-serif;" onclick="updateComment(@item.Id);">Modify</button>
                                    <button class="btn btn-link del" style="font-family: 'Comic Sans MS', cursive, sans-serif;" onclick="deleteComment(@item.Id);">Delete</button>`;
    }

    comm += `</div></div></div></div>`;
    return comm;
}

function deleteComment(comment_id) {
    connection.invoke('DeleteComment', comment_id, postId);
    fetch(`/api/v1/comments/${comment_id}`, {
        method: 'DELETE',
        credentials: 'include',
        mode: 'cors'
    }).catch(err => {
        console.error(err);
    });
}