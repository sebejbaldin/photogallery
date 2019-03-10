var currentSearch = null;
var currentPage = 1;

window.onpopstate = async (event) => {
    let params = event.state ? event.state.split('-') : null;
    if (params) {
        let data = await getPaginatedPhotos(params[0], params[1]);
        writeCards(data.photos);
        writePagination(data.pageIndex, data.pageCount);
    }
};

async function navigateToPage(index) {
    if (currentSearch) {
        let data = await getPaginatedPhotos(index, currentSearch);
        writePagination(data.pageIndex, data.pageCount);
        writeCards(data.photos);
        history.pushState(`${index}-${currentSearch}`, 'Search ' + index, `/Gallery/Search/${index}?query=${currentSearch}`);
        currentPage = index;
    }
}

async function getPaginatedPhotos(index, query) {
    let url = `/api/v1/search/${index}`;
    if (query) {
        url += `?query=${query}`;
    }
    let data = await fetch(url, {
        method: 'GET',
        credentials: 'include'
    })
        .then((res) => {
            if (res.ok)
                return res.json();
        })
        .catch(console.warn)
        .then((json) => json);
    data.pageIndex = index;
    return data;
}

document.getElementById('searchBox').addEventListener('input', async (event) => {
    let query = event.target.value;
    currentSearch = query;
    if (query.length > 2) {
        history.replaceState(`${currentPage}-${query}`, query, `/Gallery/Search?query=${query}`);
        let data = await getPaginatedPhotos(currentPage, query);
        if (data) {
            writeCards(data.photos);
            writePagination(currentPage, data.pageCount);
        }
    }
});