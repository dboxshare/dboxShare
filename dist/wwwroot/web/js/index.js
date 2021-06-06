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
    if ($html5() == false) {
        fastui.coverTips(lang.login.tips['html5-browser-note']);
        return;
    }

    if ($cookie('user-login-id').length == 0) {
        $id('loginid').focus();
    } else {
        $id('loginid').value = $cookie('user-login-id');
        $id('password').focus();
    }

    languageView();
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

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/account/user-login', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                $location('/web/main.html');
            } else if (data == 'login-failed') {
                fastui.textTips(lang.login.tips['login-failed']);
            } else if (data == 'login-lock-ip') {
                fastui.textTips(lang.login.tips['login-lock-ip']);
            } else if (data == 'logged-in') {
                fastui.textTips(lang.login.tips['logged-in']);
            } else {
                fastui.textTips(lang.login.tips['operation-failed']);
            }
            }
        });
}


// 多国语言列表视图显示
function languageView() {
    var html = '';

    for (var i in lang.json) {
        html += '<label onClick="languageChange(\'' + i + '\');">' + lang.json[i] + '</label>';
    }

    $class('language-view')[0].innerHTML = html;
}


// 默认语言切换
function languageChange(value) {
    for (var i in lang.json) {
        if (i == value) {
            $cookie('app-language', value, 365);
            break;
        }
    }

    window.location.reload(true);
}