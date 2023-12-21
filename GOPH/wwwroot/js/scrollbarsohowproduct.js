window.onscroll = function () { myFunction() };
var navbar = document.getElementById("navbar-search");
var menuSmallGroups = document.getElementById("menuSmallGroups");
var mainhtm = document.getElementById("main");
var sticky = navbar.offsetTop;

document.getElementById("loader-scroll-img").style.display = "none";
function myFunction() {

    if (window.pageYOffset >= sticky) {
        navbar.classList.add("sticky");
        menuSmallGroups.classList.add("mt-5")
        mainhtm.classList.add("magin");

    } else {
        navbar.classList.remove("sticky");
        menuSmallGroups.classList.remove("mt-5")
        mainhtm.classList.remove("magin");
    }
}


var _throttleTimer = null;
var _throttleDelay = 100;
var $window = $(window);
var $document = $(document);

var object = Number(document.getElementById("number-page").value);

var object_2 = Number(document.getElementById("number-page-isprice").value);
$document.ready(function () {

    $window
        .off('scroll', ScrollHandler)
        .on('scroll', ScrollHandler);

});

function ScrollHandler(e) {
    //throttle event:
    clearTimeout(_throttleTimer);
    _throttleTimer = setTimeout(function () {
        console.log('scroll');

        //do work
        if ($window.scrollTop() + $window.height() > $document.height() - 100) {

            document.getElementById("loader-scroll-img").style.display = "block";
            object_2 = Number(document.getElementById("number-page-isprice").value)
            object = object + 1;
            object_2 = object_2 + 1;
            document.getElementById("number-page").value = object;
            document.getElementById("number-page-isprice").value = object_2;
            showDrop(object, object_2)
        }

    }, _throttleDelay);
}



function showDrop(object, object_2) {

    var numberpage = object;

    var pageIsPireNumber = object_2;
    debugger

    var group = document.getElementById("router-id").value;

    debugger

    $.ajax({
        url: domain + '/home/showCardProductPartial',
        contentType: 'application/html; charset=utf-8',
        data: { pageNumber: numberpage, groupId: group, pageIsPireNumber: pageIsPireNumber },
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            debugger
            $("#list-card-product").append(response.returnHtml);
            if (response.isPrice === true) {

                document.getElementById("number-page-isprice").value = response.pageNumber;
                debugger
            }
            document.getElementById("loader-scroll-img").style.display = "none";
        },
        error: function (response) {
            debugger
            alert(response);
        }
    });
}

