// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function goToLocation(loc) {
    document.location = loc;
}

function _get(url, callback) {
    $.ajax({
        type: 'get',
        url: url,
        success: function (data) {
            callback(data);
        }
    });
}

function getPartialViewHtml(url, selector, callback) {
    var element = $(selector);
    element.html('<span><i class="fas fa-spinner fa-spin" style="font-size: 64px; left: 50%;"></i></span>');
    _get(url,
        function (data) {
            if (data) {
                element.html(data);
            } else {
                toastr.error(`Error loading resource from ${url}`);
                element.html('<span><i class="fas fa-exclamation-triangle" style="font-size: 64px; left: 50%;"></i> Error</span>');
            }
            if (callback) {
                callback();
            }
        });
}

function activateModal(url, selector, reload) {
    const element = $(selector);
    getPartialViewHtml(url, selector, function () {
        element.modal();

        if (reload) {
            element.on('hidden.bs.modal',
                () => {
                    window.location.reload(true);
                });
        }
    });
}


