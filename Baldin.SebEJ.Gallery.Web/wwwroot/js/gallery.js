﻿//var images;
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

async function getCurrentPagePhotos() {
    let currentUrl = location.pathname;
    currentUrl = currentUrl.substring(currentUrl.lastIndexOf('/') + 1);
    let id = parseInt(currentUrl);
    if (!id) {
        id = 1;
    }
    let data = await getPaginatedPhotos(id);

    writeCards(data.photos);
}

async function getPaginatedPhotos(pageIndex) {
    let data = await fetch(`/api/v1/gallery/${pageIndex}`, {
        method: 'GET',
        credentials: 'include'
    })
    .then((res) => {
        if (res.ok)
            return res.json();
    })
    .catch(console.warn)
    .then((json) => {
        return json;
    });
    data.pageIndex = pageIndex;
    return data;
}

window.onpopstate = async (event) => {
    let index = event.state ? event.state : 1;
    let data = await getPaginatedPhotos(index);
    writeCards(data.photos);
    writePagination(data.pageIndex, data.pageCount);
};

async function navigateToPage(index) {
    let data = await getPaginatedPhotos(index);
    writePagination(data.pageIndex, data.pageCount);
    writeCards(data.photos);
    history.pushState(index, 'Gallery ' + index, `/Gallery/${index}`);
}

function writeCards(fileList) {
    //gallery.empty();
    gallery.innerHTML = '';
    fileList.forEach(item => {
        let card = getCard(item.thumbnail_Url ? item.thumbnail_Url : item.url, item.id, item.name, '', item.isVoted, item.rating, item.votes);
        //gallery.innerHTML = card;
        let child = document.createElement('div');
        child.setAttribute('class', 'col-md-4 col-sm-6');
        child.innerHTML = card;
        gallery.appendChild(child);
    });
}

function writePagination(pageIndex, pageNumber) {
    let hasPrevious = pageIndex > 1;
    let hasNext = pageIndex < pageNumber;

    let pages = [];
    let cnt = pageIndex - 2;
    while (cnt <= pageNumber && cnt <= 5) {
        if (cnt <= 0) {
            cnt++;
            continue;
        }
        pages.push(cnt);
        cnt++;
    }

    let pageSection = document.getElementById('pagination');
    pageSection.innerHTML = getPagination(pageIndex, hasPrevious, hasNext, pages);
}



function getCard(img_url, id, title, descr, isVoted, average, votes) {
    let tmp = `<div class="card" style="margin-bottom: 25px;">
<a href="/Gallery/Photo/${id}">
      <img class="card-img-top" style="height: 300px;" src="${img_url}" alt="Card image cap">
</a>
      <div class="card-body">
            <!--<h5 class="card-title">${title}</h5>-->
            <!--<p class="card-text">${descr}</p>-->
            <div id="card_board_${id}">`;

    tmp += hasVoted(isVoted, id, Math.round10(average, -1), votes);

    tmp += `</div></div></div>`;
    return tmp;
}

function getPagination(current, hasPrev, hasNext, innerPages) {
    let pagTemp = `<nav aria-label="gallery pagination">
        <ul class="pagination justify-content-center">
            <li id="prevPage" class="page-item ${hasPrev ? "" : "disabled"}">
                <button class="page-link" onclick="navigateToPage(${current - 1});" aria-label="Previous">
                    <span aria-hidden="true">&laquo;</span>
                    <span class="sr-only">Previous</span>
                </button>
            </li>`;

    innerPages.forEach(index => {
        pagTemp += `<li class="page-item ${index === current ? "active" : ""}"><button class="page-link" onclick="navigateToPage(${index});">${index}</button></li>`;
    });
            

    pagTemp += `<li id="nexPage" class="page-item ${hasNext ? "" : "disabled"}">
                <button class="page-link" onclick="navigateToPage(${(current*1)+1});" aria-label="Next">
                    <span aria-hidden="true">&raquo;</span>
                    <span class="sr-only">Next</span>
                </button>
            </li>
        </ul>
    </nav>`;

    return pagTemp;
}

