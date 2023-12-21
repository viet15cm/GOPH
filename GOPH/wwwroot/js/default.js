var domain = document.getElementById("domain").value;

function myAlertBottom(text) {
    debugger
    $(".myAlert-bottom").show();
    $(".myAlert-bottom").text(text);
    setTimeout(function () {
        $(".myAlert-bottom").hide();
    }, 2000);
}
