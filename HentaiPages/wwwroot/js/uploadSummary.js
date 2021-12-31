async function ExecuteAPICall(constructedUri) {
    const response = await fetch(constructedUri);
    const d = await response.json();
    console.log(d);
    return d;
}

async function deleteImage(id) {
    let constructedUri = "/api/images/DeleteImage/" + id;

    if (confirm('Are you sure you want to delete this image?')) {
        await ExecuteAPICall(constructedUri);
    }
}