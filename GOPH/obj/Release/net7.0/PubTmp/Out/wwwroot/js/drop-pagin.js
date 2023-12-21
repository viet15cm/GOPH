function showDropHot(object) {

    var numberpage = object.value;
    debugger
    $.ajax({
        url: "/Home/ShowDropPartial",
        contentType: 'application/html; charset=utf-8',
        data: { "PageNumber": numberpage },
        type: 'GET',
        dataType: 'html',
        success: function (response) {

            $("#list-card-hot").append(response);

            object.parentNode.parentNode.removeChild(object.parentNode);
        },
        error: function (response) {
            debugger
            alert(response);
        }
    });
}