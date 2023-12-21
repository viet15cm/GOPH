
    window.onscroll = function () {myFunction()};
    var navbar = document.getElementById("navbar-search");
    var menuSmallGroups = document.getElementById("menuSmallGroups");
    var mainhtm = document.getElementById("main");
    var sticky = navbar.offsetTop;

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

            }, _throttleDelay);
        }

