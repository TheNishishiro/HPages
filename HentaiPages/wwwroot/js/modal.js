var modal = document.getElementById("myModal");
var btn = document.getElementById("myBtn");
var span = document.getElementsByClassName("close")[0];
var displayImage = document.getElementById("displayImage");
var displayImageFull = document.getElementById("displayImageFull");
var likeButton = document.getElementById("likeButton");
var deleteButton = document.getElementById("deleteButton");
var downloadButton = document.getElementById("downloadButton");
var modalVideoDiv = document.getElementById("modalVideoDiv");
var modalVideoPlayer = document.getElementById("modalVideoPlayer");
var video = document.getElementById("video");

var uploadDate = document.getElementById("uploadDate");
var headerId = document.getElementById("headerId");

var currentImageId = 0;
var currentArrayId = 0;
var currentlyAvailableIds = [];

async function openModal(availableIds, currentId) {
    currentArrayId = currentId;
    currentlyAvailableIds = availableIds;

    await setImage();
    modal.style.display = "block";
}

async function ExecuteAPICall(constructedUri) {
    const response = await fetch(constructedUri);
    const d = await response.json();
    console.log(d);
    return d;
}

async function getImage() {
    const response = await fetch("/api/images/GetImageById/" + currentImageId);
    return response.json();
}

function ISODateToDateString(date) {
    date = new Date(date);
    return date.getFullYear()+'-' + (date.getMonth()+1) + '-'+date.getDate();
}

async function setRandomImage()
{
    const response = await fetch("/api/images/Random");
    currentImageId = await response.json();

    let src = "/api/images/GetImageDataById/" + currentImageId;
    displayImageFull.style.backgroundImage = "url('" + src + "')";

    let image = await getImage();

    console.log(src);
    console.log(image);

    headerId.innerHTML = image["imageId"];

    if (image["favourite"]) {
        likeButton.textContent = "💔";
    }
    else {
        likeButton.textContent = "💗";
    }
}

async function setImage() {
    currentImageId = currentlyAvailableIds[currentArrayId];

    let src = "/api/images/GetImageDataById/" + currentImageId;
    displayImage.style.backgroundImage = "url('" + src + "')";
    let image = await getImage();
    
    console.log(modalVideoDiv.style.display);
    modalVideoDiv.style.display = "none";
    
    if (image["contentType"] === "video/mp4") {
        modalVideoDiv.style.display = "";
        video.pause();
        modalVideoPlayer.src = "api/images/GetImageDataById/" + currentImageId;
        video.load();
        video.play();
    }

    console.log(src);
    console.log(image);

    uploadDate.innerHTML = "Upload date: " + ISODateToDateString(image["uploadDate"]);
    headerId.innerHTML = image["imageId"];

    if (image["favourite"]) {
        likeButton.textContent = "💔";
    }
    else {
        likeButton.textContent = "💗";
    }
}

function hideModal() {
    modal.style.display = "none";
}

span.onclick = function() {
    hideModal();
}

window.onclick = function(event) {
  if (event.target === modal) {
      hideModal();
  }
}

document.onkeydown = async function(evt) {
    evt = evt || window.event;
    if (evt.keyCode === 27) {
        window.event.returnValue = false;
        window.event.cancelBubble = true;
        modal.style.display = "none";
    }
    else if (evt.keyCode === 37) {
        if (currentArrayId > 0) {
            currentArrayId--;
        }
        await setImage();
    }
    else if (evt.keyCode === 39) {
        if (currentArrayId < currentlyAvailableIds.length) {
            currentArrayId++;
        }
        await setImage();
    }
};

async function setFavourite() {
    let constructedUri = "/api/images/ToggleLikeImage/" + currentImageId;
    await ExecuteAPICall(constructedUri);
    alert("Image state toggled!");
    await setImage();
}

async function deleteImage() {
    let constructedUri = "/api/images/DeleteImage/" + currentImageId;

    if (confirm('Are you sure you want to delete this image?')) {
        await ExecuteAPICall(constructedUri);
        hideModal();
    }
}

function download() {
    let a = document.createElement("a");
    a.href = "/api/images/GetImageDataById/" + currentImageId;
    a.setAttribute("download", currentImageId + ".png");
    a.click();
}