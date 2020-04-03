function fun2() {

    var sum = 0;
    var hi = parseInt(document.getElementById("hid").value);
    var checkboxes = document.getElementsByClassName("che");
    var val = document.getElementsByClassName("To");
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {

            sum += parseInt(val[i].value);
        }


    }

    document.getElementById("kk").innerHTML = String(sum + hi);
}