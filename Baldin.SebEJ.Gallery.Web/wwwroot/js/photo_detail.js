'use strict';

var comments = document.getElementById('comments_box');
var commentText = document.getElementById('commentText');
//var postId = document.getElementById('postId').value;

var connection = new signalR.HubConnectionBuilder().withUrl('/Comments').build();

connection.on('ReceiveComment', (username, text, insertDate) => {
    appendComment(username, text, insertDate);
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

function appendComment(username, text, creationDate) {
    let container = document.createElement('div');
    text = text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    container.setAttribute('class', 'col-12');
    container.innerHTML = getComment(username, text, creationDate);
    comments.appendChild(container);
}


function getComment(username, text, creationDate) {
    return `<div class="row">
                <div class="col-2">
                    <i class="fas fa-users fa-3x"></i>
                </div>
                <div class="col-10">
                    <div class="card" style="width: 100%;">
                        <div class="card-body">
                            <h5 class="card-title">${username}, ${creationDate}</h5>
                            <p class="card-text">${text}</p>
                            <button class="btn btn-link">Modify</button>
                            <button class="btn btn-link">Delete</button>
                        </div>
                    </div>
                </div>
            </div>`;
}