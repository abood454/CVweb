function fun2() {
    var sum = 0;
    var hi = parseInt(document.getElementById("hid").innerHTML);
    var checkboxes = document.getElementsByClassName("che");
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {

            sum += parseInt(checkboxes[i].value);
        }


    }
    document.getElementById("kk").innerHTML = String(sum + hi);
}