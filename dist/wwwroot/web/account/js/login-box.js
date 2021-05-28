// 弹出重新登录窗口
window.setTimeout(function() {
    if ($page().client.width > 400 && $page().client.height > 400) {
        var windowObject = window.self;
    } else {
        var windowObject = window.parent;
    }

    if (window.location.pathname.indexOf('main.html') > - 1) {
        try {
            window.location.href = '/';
            } catch(e) {}
    }

    if (window.self.$id('login-box-popup-window') != null || window.parent.$id('login-box-popup-window') != null) {
        return;
    }

    windowObject.fastui.windowPopup('login-box', '', '/web/account/login.html', 350, 350);
    }, 200);