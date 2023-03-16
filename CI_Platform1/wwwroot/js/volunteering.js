let slideIndex = 1;
showSlides(slideIndex);

// Next/previous controls
function plusSlides(n) {
    showSlides(slideIndex += n);
}

// Thumbnail image controls
function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    let i;
    let slides = document.getElementsByClassName("mySlides");
    let dots = document.getElementsByClassName("demo");
    let captionText = document.getElementById("caption");
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    slides[slideIndex - 1].style.display = "block";
    dots[slideIndex - 1].className += " active";
    // captionText.innerHTML = dots[slideIndex-1].alt;
}



// Tabs js
const tabs = document.querySelectorAll('.tab');
const tabContents = document.querySelectorAll('.tab-content-item');

tabs.forEach(tab => {
    tab.addEventListener('click', function () {
        // remove active class from all tabs and tab content
        tabs.forEach(tab => tab.classList.remove('active'));
        tabContents.forEach(content => content.classList.remove('active'));

        // add active class to current tab and its corresponding tab content
        this.classList.add('active');
        document.getElementById(this.dataset.tabContent).classList.add('active');
    });
});



// Add to Favourites

function Fav(id) {
    var url = '@Url.Action("_Fav", "Home")';

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            id: id
        },
        success: function (data) {
            if (data.length == 0) // No errors
                alert("Fave success!");
        },
        error: function (jqXHR) { // Http Status is not 200
        },
        complete: function (jqXHR, status) { // Whether success or error it enters here
        }
    });
};

function UnFav(id) {
    var url = '@Url.Action("_UnFav", "Home")';

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            id: id
        },
        success: function (data) {
            if (data.length == 0) // No errors
                alert("Unfave success!");
        },
        error: function (jqXHR) { // Http Status is not 200
        },
        complete: function (jqXHR, status) { // Whether success or error it enters here
        }
    });
};

function ratemission(starId, missionId, id) {
    $.ajax({
        url: '/Home/AddRating',
        type: 'POST',
        data: { missionId: missionId, id: id, rating: starId },
        success: function (result) {
            if (parseInt(result.ratingExists.rating),10) {
                // Update the heart icon to show that the mission has been liked
                for (i = 1; i <= parseInt(result.ratingExists.rating, 10); i++) {

                    var starbtn = document.querySelector('.star-' + i);
                    starbtn.style.color = "#F88634";
                }
                for (i = parseInt(result.ratingExists.rating, 10) + 1; i <= 5; i++) {

                    var starbtn = document.querySelector('.star-' + i);
                    starbtn.style.color = "black";
                }
            } else {
                // Update the heart icon to show that the mission has been unliked
                for (i = 1; i <= parseInt(result.newRating.rating, 10); i++) {

                    var starbtn = document.getElementById(String(i));

                    starbtn.style.color = "#F88634";
                }
                for (i = parseInt(result.newRating.rating, 10) + 1; i <= 5; i++) {

                    var starbtn = document.getElementById(String(i));

                    starbtn.style.color = "black";
                }
            }
        },
        error: function () {
            // Handle error response from the server, e.g. show an error message to the user
            alert('Error: Could not like mission.');
        }
    });
}
//function Fav() {
//    const params = new URLSearchParams(window.location.search);
//    const query = params.get("missionid")

//    $.ajax({
//        url: "/Home/Fav",
//        data: { missionId: query },
//        success: function (result) {
//            alert("successs");
//            console.log(result)
//        }
//    });
//}