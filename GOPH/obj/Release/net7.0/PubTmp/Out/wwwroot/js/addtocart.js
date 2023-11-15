function addtocart(object, id) {

    $.ajax({
        url: domain + '/home/addtocart',
        contentType: 'application/html; charset=utf-8',
        data: { id: id },
        type: 'GET',
        dataType: 'json',
        success: function (response) {
        
            $(".icon-item-count-cart").text(response.countItem);

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
