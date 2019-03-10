async function voteImage(id) {
    let payload = {
        Picture_Id: id,
        //img_name: document.getElementById('img_name').value,
        Rating: document.getElementById('vote_' + id).value
    };
    let card = document.getElementById('card_board_' + id);
    card.innerHTML = hasVoted(true, id, 'N.D.', 'N.D.');
    await fetch('/api/v1/gallery', {
        method: 'POST',
        mode: 'cors',
        cache: 'no-cache',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        },
        body: JSON.stringify(payload)
    }).then(res => {
        console.log('voted');
        return res.json();
    })
        .then(obj => {
            if (obj && obj.average && obj.count)
                card.innerHTML = hasVoted(true, id, Math.round10(obj.average, -1), obj.count);
        })
        .catch(err => {
            console.log(err);
        });
}