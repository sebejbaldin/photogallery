﻿'use strict';

var comments = document.getElementById('comments_box');
var commentText = document.getElementById('commentText');
var charCounter = document.getElementById('charCount');

commentText.addEventListener('input', () => {
    charCounter.innerText = commentText.value.length;
});
//var postId = document.getElementById('postId').value;

var connection = new signalR.HubConnectionBuilder().withUrl('/Comments').build();

connection.on('ReceiveComment', (email, text, insertDate, comment_id) => {
    if (username !== email) {
        appendComment(email, text, new Date(insertDate).toLocaleString(), comment_id);
    }
});

connection.on('EraseComment', (comment_id) => {
    document.getElementById('comment_' + comment_id).remove();
});

connection.on('ModifyComment', (comment_id, body) => {
    if (document.getElementById(`comText_${comment_id}`).innerText !== body) {
        let upBox = document.getElementById(`updateBox_${comment_id}`);
        upBox.innerHTML = getNormalCommentHTML(comment_id, normalizeInput(body));
    }
});

document.getElementById('sendComment').addEventListener('click', async () => {
    let date = new Date();
    let payload = {
        picture_Id: postId,
        email: username,
        text: commentText.value
    };
    commentText.value = '';
    charCounter.innerText = '0';
    await fetch('/api/v1/comments', {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        mode: 'cors',
        cache: 'no-cache',
        body: JSON.stringify(payload)
    })
        .then(resCommentId => {
            return resCommentId.json();
        })
        .then(commentId => {
            console.log('Id Commento: ' + commentId);
            connection.invoke('SendComment', username, payload.text, date, commentId, postId);
            appendComment(username, payload.text, date.toLocaleString(), commentId, true);
        })
    .catch(err => {
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

function appendComment(email, text, creationDate, comment_id, isAuthor) {
    let container = document.createElement('div');
    text = normalizeInput(text);
    container.setAttribute('class', 'col-12 mb-2');
    container.setAttribute('id', `comment_${comment_id}`);
    container.innerHTML = getComment(email, text, creationDate, comment_id, isAuthor);
    comments.appendChild(container);
}

function getComment(email, text, creationDate, comment_id, isMine) {
    let comm = `<div class="row">
                    <div class="col-1">
                        <i class="fas fa-user fa-2x"></i>
                    </div>
                    <div class="col-11">
                        <div class="card" style="width: 100%;">
                            <div class="card-body">
                            <h5 class="card-title">${email}, ${creationDate}</h5>
                            <div id="updateBox_${comment_id}">
                                    <p class="card-text" style="max-height: 40px; overflow-x: auto;" id="comText_${comment_id}">${text}</p>`;
    if (isMine) {
        comm += `<button class="btn btn-link" style="font-family: 'Comic Sans MS', cursive, sans-serif;" onclick="updateComment(${comment_id});">Modify</button>
                 <button class="btn btn-link" style="font-family: 'Comic Sans MS', cursive, sans-serif;" onclick="deleteComment(${comment_id});">Delete</button>`;
    }

    comm += `</div></div></div></div></div>`;
    
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

function updateComment(comment_id) {
    let previousText = document.getElementById(`comText_${comment_id}`).innerText;
    let upBox = document.getElementById(`updateBox_${comment_id}`);
    upBox.innerHTML = getUpdateCommentHTML(comment_id, previousText);
}

function saveUpdatedComment(comment_id) {
    let payload = {
        commentId: comment_id,
        textBody: document.getElementById(`textarea_${comment_id}`).value
    };
    let upBox = document.getElementById(`updateBox_${comment_id}`);
    upBox.innerHTML = getNormalCommentHTML(comment_id, payload.textBody, true);
    connection.invoke('UpdateComment', comment_id, payload.textBody, postId);
    fetch(`/api/v1/comments/${comment_id}`, {
        method: 'PATCH',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json; charset=UTF-8'
        },
        referrer: 'client',
        body: JSON.stringify(payload)
    }).catch(err => {
        console.error(err);
    });
}

function getNormalCommentHTML(itemId, text, withButtons) {
    let temp = ` 
        <p class="card-text" style="max-height: 40px; overflow-x: auto;" id="comText_${itemId}">${text}</p>`
    if (withButtons)
        temp += `<button class="btn btn-link" style="font-family: 'Comic Sans MS', cursive, sans-serif;" onclick="updateComment(${itemId});">Modify</button>
        <button class="btn btn-link" style="font-family: 'Comic Sans MS', cursive, sans-serif;" onclick="deleteComment(${itemId});">Delete</button>`;
    return temp;
}

function getUpdateCommentHTML(itemId, text) {
    return `<textarea id="textarea_${itemId}" class="form-control">${text}</textarea>
        <button type="button" class="btn btn-link" style="font-family: 'Comic Sans MS', cursive, sans-serif;" onclick="saveUpdatedComment(${itemId})">Save</button>`;
}

function normalizeInput(text) {
    return text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}