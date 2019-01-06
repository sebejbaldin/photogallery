//var images;
var gallery = document.getElementById('gallery');

async function getPhotos() {
    fetch('/api/v1/gallery')
        .then(res => {
            res.json()
                .then(jsonRes => {
                    writeCards(jsonRes);
                });
        });
}

function writeCards(fileList) {
    //gallery.empty();
    gallery.innerHTML = '';
    fileList.forEach(item => {
        let card = getCard(item.url, item.id, item.name, '', item.isVoted, item.rating, item.votes);
        //gallery.innerHTML = card;
        let child = document.createElement('div');
        child.setAttribute('class', 'col-md-4');
        child.innerHTML = card;
        gallery.appendChild(child);
    });
}

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
                card.innerHTML = hasVoted(true, id, obj.average, obj.count);
            else
                getPhotos();
        })
        .catch(err => {
            console.log(err);
        });
    //await getPhotos();
}

function getCard(img_url, id, title, descr, isVoted, average, votes) {
    let tmp = `<div class="card" style="width: 18rem;">
      <img class="card-img-top" src="${img_url}" alt="Card image cap">
      <div class="card-body">
            <!--<h5 class="card-title">${title}</h5>-->
            <!--<p class="card-text">${descr}</p>-->
            <div id="card_board_${id}">`;



    tmp += hasVoted(isVoted, id, average, votes);

    tmp += `</div></div></div>`;
    return tmp;
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