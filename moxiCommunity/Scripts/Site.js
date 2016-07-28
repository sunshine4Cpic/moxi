$(function () {
    aPostInit();

    $("#pagination-from").submit(function () {
        location.href = '?page=' + $('#fase-input').val();
        return false;
    })
});

function aPostInit(obja)
{

    if (obja == undefined)
        obja = $("a[method=post]");

    obja.click(function () {
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
}

