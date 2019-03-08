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
            else
                getPhotos();
        })
        .catch(err => {
            console.log(err);
        });
}

function hasVoted(hasVoted, id, average, votes) {
    if (!hasVoted) {
        return `<form>
            <div class="row">
            <div class="col">
                  <select name="vote" id="vote_${id}" class="form-control">
                        <option value="1">1</option> 
                        <option value="2">2</option> 
                        <option value="3">3</option> 
                        <option value="4">4</option> 
                        <option value="5">5</option> 
                  </select>
                  </div>

                  <div class="col">

                  <button type="button" onclick="voteImage(${id});" class="btn btn-primary btn-block">Vote</button>
                  </div>
                  </div>
                  </form>`;
    } else {
        return `<div class="row">
                  <div class="col-6">
                        <i class="fas fa-star"></i> ${average}
                  </div>
                  <div class="col-6">
                        <i class="fas fa-poll"></i> ${votes}
                  </div>
            </div>`;
    }
}