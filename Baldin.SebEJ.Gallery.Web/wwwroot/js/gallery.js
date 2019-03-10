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

async function getCurrentPagePhotos() {
    let id = getCurrentPageIndex();
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



