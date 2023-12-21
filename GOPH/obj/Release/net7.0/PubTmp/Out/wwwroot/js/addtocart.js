
var timeout = null;
function increaseCount(id, object) {
    var input = object.previousElementSibling;

    var value = parseInt(input.value, 10);
    value = isNaN(value) ? 0 : value;
    value++;
    input.value = value;

    clearTimeout(timeout);

    timeout = setTimeout(function () {
        updatecart(id, input.value);
    }, 500);


}
function resetData() {
    count = 0;
    isAlreadyInProcess = false;
}


function decreaseCount(id, object) {
    var input = object.nextElementSibling;

    var value = parseInt(input.value, 10);
    if (value > 1) {
        value = isNaN(value) ? 0 : value;
        value--;
        input.value = value;
        clearTimeout(timeout);

        timeout = setTimeout(function () {
            updatecart(id, input.value);
        }, 500);

    }
}



function deletCart(object, id) {

    var load = document.getElementById('modal-all');
    load.style.display = "block";
    debugger
    $.ajax({
        url: domain + '/Cart/deletecart',
        contentType: 'application/html; charset=utf-8',
        data: { id: id },
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            debugger

            var c = response.countItem;
            $(".icon-item-count-cart").text(c);
            $('#shopping-cart').empty();
            $('#shopping-cart').html(response.returnHtml);

            load.style.display = "none";

            debugger
        },
        error: function (response) {
            debugger
            alert("lỗi liên hệ admin");
        }
    });
}

function updatecart(id, count) {
    var load = document.getElementById('modal-all');
    load.style.display = "block";
    $.ajax({
        url: domain + '/Cart/AddCountCart',
        contentType: 'application/html; charset=utf-8',
        data: { id: id, count: count },
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            debugger

            var c = response.countItem;
            $(".icon-item-count-cart").text(c);
            $('#shopping-cart').empty();
            $('#shopping-cart').html(response.returnHtml);
            load.style.display = "none";
            //myAlertBottom(response.message);
        },
        error: function (response) {
            debugger
            alert(response);
        }
    });

}

function addtocart(object, id) {
    var load = document.getElementById('modal-all');
    load.style.display = "block";
    debugger
    $.ajax({
        url: domain + '/Cart/addtocart',
        contentType: 'application/html; charset=utf-8',
        data: { id: id},
        type: 'GET',
        dataType: 'json',
        success: function (response) {
        
            $(".icon-item-count-cart").text(response.countItem);
            load.style.display = "none";
            myAlertBottom(response.message);
        },
        error: function (response) {
          
            alert(response);
        }
    });
}
function myAlertTop() {
    $(".myAlert-top").show();
    setTimeout(function () {
        $(".myAlert-top").hide();
    }, 2000);
}

function myAlertBottom(text) {
    debugger
    $(".myAlert-bottom").show();
    $(".myAlert-bottom").text(text);
    setTimeout(function () {
        $(".myAlert-bottom").hide();
    }, 2000);
}
