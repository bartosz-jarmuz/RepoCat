$(document).ready(function () {

    $.get("/Home/NavHeaderStats", function (data) {
        $("#NavHeaderStatsContainer").replaceWith(function () {
            return $(data).hide().fadeIn(500);
        });
    });

    

    $('.sidebar-minimizer').on('click', function () {
        if ($('body').hasClass('brand-minimized sidebar-minimized')) {
            setCookie('sidebarOpen', 'false');
        } else {
            setCookie('sidebarOpen', 'true');
        }
    });
});


function getOrganizationFromKey(key) {
    return key.split(":")[0];
}
function getRepositoryFromKey(key) {
    return key.split(":")[1];
}

function isItemInArray(input, separator, itemToFind) {
    if (input) {
        var split = input.split(separator);
        var exists = split.includes(itemToFind);
        return exists;
    }
    return false;
}

function trimStringMidsection(text) {
    if (text.length > 45) {
        var middlePoint = text.length / 2;
        var firstPart = text.substring(0, middlePoint);
        var secondPart = text.substring(middlePoint);
        return firstPart.substring(0, 20) + '(...)' + secondPart.substring(secondPart.length - 20);
    }
}

