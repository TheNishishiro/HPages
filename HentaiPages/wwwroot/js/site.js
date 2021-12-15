document.addEventListener("DOMContentLoaded", function(event) {
   document.querySelectorAll('img').forEach(function(img){
  	img.onerror = function(){this.style.display='none';};
   })
});

function SetSelectBackgroundColor(select) {
    let selectedValue = select.options[select.selectedIndex].value;
    if (selectedValue === "")
    {
        select.style.background = "#FFFFFF";
    }
    else if (selectedValue === "true")
    {
        select.style.background = "#ADFFA5";
    }
    else if (selectedValue === "false")
    {
        select.style.background = "#FF7F7F";
    }
}