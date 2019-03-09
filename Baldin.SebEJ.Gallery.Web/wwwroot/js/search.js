
document.getElementById('searchBox').addEventListener('input', async (event) => {
    let query = event.target.value;
    if (query.length > 2) {
        let data = await fetch(`/api/v1/search/1?query=${query}`, {
                method: 'GET',
                credentials: 'include'
            })
            .then(json => {
                if (json.ok)
                    return json.json();
            })
            .then(data => data);
        if (data) {
            writeCards(data.photos);
        }
    }
});