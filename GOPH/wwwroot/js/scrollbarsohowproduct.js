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
            object = object + 1;
            document.getElementById("number-page").value = object;
            showDrop(object)
        }

    }, _throttleDelay);
}



function showDrop(object) {

    var numberpage = object;

    var group = document.getElementById("router-id").value;

    debugger
    $.ajax({
        url: domain + '/home/showCardProductPartial',
        contentType: 'application/html; charset=utf-8',
        data: { pageNumber: numberpage, groupId: group },
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            debugger
            $("#list-card-product").append(response);
            document.getElementById("loader-scroll-img").style.display = "none";
        },
        error: function (response) {
            debugger
            alert(response);
        }
    });
}