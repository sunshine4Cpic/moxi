$(function () {
    $("a[method=post]").click(function () {
        var cf = true;
        if (this.getAttribute("confirm"))
            cf = confirm(this.getAttribute("confirm"));
        if (cf) {
            var temp = document.createElement("form");
            temp.action = this.href;
            temp.method = "post";
            temp.style.display = "none";
            document.body.appendChild(temp);
            temp.submit();
        }
        return false;
    });
});

