function writeCards(fileList) {
    let gallery = document.getElementById('gallery');

    if (fileList) {
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
    else {
        gallery.innerHTML = '';
    }
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
        pagTemp += `<li class="page-item ${index == current ? "active" : ""}"><button class="page-link" onclick="navigateToPage(${index});">${index}</button></li>`;
    });


    pagTemp += `<li id="nexPage" class="page-item ${hasNext ? "" : "disabled"}">
                <button class="page-link" onclick="navigateToPage(${(current * 1) + 1});" aria-label="Next">
                    <span aria-hidden="true">&raquo;</span>
                    <span class="sr-only">Next</span>
                </button>
            </li>
        </ul>
    </nav>`;

    return pagTemp;
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