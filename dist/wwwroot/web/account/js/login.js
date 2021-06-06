// 回车键提交登录表单
document.onkeydown = function(e) {
    var event = e || window.event;

    if (event.keyCode == 13) {
        var cover = $class('page-cover')[0];

        if (cover == undefined || cover.style.display == 'none') {
            userLogin();
            return;
        }
    }
};


// 页面初始化事件调用函数
function init() {
    pageInit();
}


// 页面初始化
function pageInit() {
    if ($cookie('user-login-id').length == 0) {
        $id('loginid').focus();
    } else {
        $id('loginid').value = $cookie('user-login-id');
        $id('password').focus();
    }
}


// 登录表单提交
function userLogin() {
    var loginId = $id('loginid').value;
    var password = $id('password').value;
    var data = '';

    if (fastui.testInput(true, 'loginid', /^([^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\"\\\|\,\.\<\>\/\?]{2,16}|[\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?|\+?([\d]{2,4}\-?)?[\d]{6,11})$/) == false) {
        fastui.inputTips('loginid', lang.login.tips.input['login-id']);
        return;
    } else {
        data += 'loginid=' + escape(loginId) + '&';
    }

    if (fastui.testInput(true, 'password', /^[^\s\'\"\%\*\<\>\=]{6,16}$/) == false) {
        fastui.inputTips('password', lang.login.tips.input['password']);
        return;
    } else {
        data += 'password=' + escape(password) + '&';
    }

    data = data.substring(0, data.length - 1);

    window.parent.fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/account/user-login', 
        async: true, 
        data: data, 
        callback: function(data) {
            window.parent.fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.location.reload();

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('login');
                    }, 500);
            } else if (data == 'login-failed') {
                window.parent.fastui.textTips(lang.login.tips['login-failed']);
            } else if (data == 'login-lock-ip') {
                window.parent.fastui.textTips(lang.login.tips['login-lock-ip']);
            } else if (data == 'logged-in') {
                window.parent.fastui.textTips(lang.login.tips['logged-in']);
            } else {
                window.parent.fastui.textTips(lang.login.tips['operation-failed']);
            }
            }
        });
}