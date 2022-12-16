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
var saveTagsButton = document.getElementById("saveTagsButton");
var tagPickerDiv = document.getElementById("tagPicker");
var filterTagsPickerDiv = document.getElementById("filterTagsPicker");

var uploadDate = document.getElementById("uploadDate");
var headerId = document.getElementById("headerId");

var currentImageId = 0;
var currentArrayId = 0;
var currentlyAvailableIds = [];
var allTags = ["Feet", "Thighs", "Legwear", "Butt", "Creampie", "Sex", "Masturbation", 
    "Boobies", "Pantsu", "Yuri", "Catgirl", "Gif", "Irl", "Ecchi", "Wet", "POV", "Handjob", "Group", "Schoolgirl",
    "Secretary", "Condom", "Grayscale", "Casual", "Inviting", "Cowgirl", "LegsSpreading", "Maid", "Blowjob", "Lingerie",
    "Tattoo", "Pussy", "Blushing", "Happy", "Boobjob", "PussyEating", "Cumshot", "Toys", "Fingering", "Video", "Footjob", "Hiding", "Dialogs",
    "Doggy", "NotHuman", "Milf", "Nurse"
]

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
    console.log(response);
    return response.json();
}

async function getImagTags() {
    const response = await fetch("/api/images/GetImageTagsById/" + currentImageId);
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
    modalVideoDiv.style.display = "none";
    
    if (image["contentType"] === "video/mp4") {
        modalVideoDiv.style.display = "";
        video.pause();
        modalVideoPlayer.src = "api/images/GetImageDataById/" + currentImageId;
        video.load();
        video.play();
    }

    uploadDate.innerHTML = "Upload date: " + ISODateToDateString(image["uploadDate"]);
    headerId.innerHTML = image["imageId"];
    if (image["favourite"]) {
        likeButton.textContent = "💔";
    }
    else {
        likeButton.textContent = "💗";
    }

    let tags = await getImagTags();
    if (tags === null)
        tags = [];
    for (const tag of tags) {
        let checkboxId = "checkbox"+tag;
        document.getElementById(checkboxId).checked = true;
    }
    let difference = allTags.filter(x => !tags.includes(x));
    for (const tag of difference) {
        let checkboxId = "checkbox"+tag;
        document.getElementById(checkboxId).checked = false;
    }

    sortTagsPicker();
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

function toggleTags() {
    if (tagPickerDiv.style.display === "none")
    {
        saveTagsButton.style.display = "";
        tagPickerDiv.style.display = "";
    }
    else
    {
        saveTagsButton.style.display = "none";
        tagPickerDiv.style.display = "none"
    }
}

function toggleFilterTags() {
    if (filterTagsPickerDiv.style.display === "none")
    {
        filterTagsPickerDiv.style.display = "";
    }
    else
    {
        filterTagsPickerDiv.style.display = "none"
    }
}

function sortTagsPicker() {
    $('#tagPicker label').sort(function(a, b) {
        var $a = $(a).find(':checkbox'),
            $b = $(b).find(':checkbox');

        if ($a.is(':checked') && !$b.is(':checked'))
            return -1;
        else if (!$a.is(':checked') && $b.is(':checked'))
            return 1;

        if ($a.val() < $b.val())
            return -1;
        else if ($a.val() > $b.val())
            return 1;
        
        return 0;
    }).appendTo('#tagPicker');

    $('#tagPicker .default:last, #tagPicker :checked:last').closest('label').after('');
}

async function saveTagsSettings() {
    let requestUrl = "/api/images/SetTags/" + currentImageId;
    let activeTags = [];

    for (const tag of allTags) {
        let checkboxId = "checkbox"+tag;
        if (document.getElementById(checkboxId).checked === true)
        {
            activeTags.push(tag);
        }
    }
    
    let body = JSON.stringify(activeTags);
    console.log(body);
    await fetch(requestUrl,
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: body
        });
}