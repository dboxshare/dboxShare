// 弹出重新登录窗口
window.setTimeout(function() {
    if ($page().client.width > 400 && $page().client.height > 400) {
        var windowObject = window.self;
    } else {
        var windowObject = window.parent;
    }

    $ajax({
        type: 'GET', 
        url: '/web/user/logout.ashx', 
        async: true
        });

    if (window.location.pathname.indexOf('main.html') > - 1) {
        window.location.href = '/';
    }

    if ($id('re-login-popup-window') != null) {
        return;
    }

    windowObject.fastui.windowPopup('re-login', '', '/web/user/login.html', 350, 350);
    }, 0);