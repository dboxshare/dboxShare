// 弹出重新登录窗口
window.setTimeout(function() {
    if ($page().client.width > 400 && $page().client.height > 400) {
        var windowObject = window.self;
    } else {
        var windowObject = window.parent;
    }

    if (window.location.pathname.indexOf('main.html') > - 1) {
        window.location.href = '/';
    }

    if (window.self.$id('re-login-popup-window') != null || window.parent.$id('re-login-popup-window') != null) {
        return;
    }

    windowObject.fastui.windowPopup('re-login', '', '/web/user/login.html', 350, 350);
    }, 200);