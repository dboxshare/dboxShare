// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('username', lang.forgot.tips.value['username']);
    fastui.valueTips('email', lang.forgot.tips.value['email']);
}


// 获取验证码表单提交
function getVericode() {
    var username = $id('username').value;
    var email = $id('email').value;
    var data = '';

    if (fastui.testInput(true, 'username', /^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\"\\\|\,\.\<\>\/\?]{2,16}$/) == false) {
        fastui.inputTips('username', lang.forgot.tips.input['username']);
        return;
    } else {
        data += 'username=' + escape(username) + '&';
    }

    if (fastui.testInput(true, 'email', /^[\w\-]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$/) == false) {
        fastui.inputTips('email', lang.forgot.tips.input['email']);
        return;
    } else {
        data += 'email=' + escape(email) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/user/forgot-password.ashx?action=get-vericode', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                $location('/web/user/forgot-password-reset.html');
            } else if (data == 'user-info-error') {
                fastui.textTips(lang.forgot.tips['user-info-error']);
            } else if (data == 'forgot-lock-ip') {
                fastui.textTips(lang.forgot.tips['forgot-lock-ip']);
            } else {
                fastui.textTips(lang.forgot.tips['operation-failed']);
            }
            }
        });
}