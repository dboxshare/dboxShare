// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('password', lang.forgot.tips.value['password']);
    fastui.valueTips('password-confirm', lang.forgot.tips.value['password-confirm']);
    fastui.valueTips('vericode', lang.forgot.tips.value['vericode']);
    fastui.sideTips('password', lang.forgot.tips.side['password']);
    fastui.sideTips('vericode', lang.forgot.tips.side['vericode']);
}


// 重设登录密码表单提交
function resetPassword() {
    var password = $id('password').value;
    var vericode = $id('vericode').value;
    var data = '';

    if (fastui.testInput(true, 'password', /^[\S]{6,16}$/) == false) {
        fastui.inputTips('password', lang.forgot.tips.input['password']);
        return;
    } else {
        data += 'password=' + escape(password) + '&';
    }

    if (fastui.testInput(true, 'password-confirm', /^[\S]{6,16}$/) == false) {
        fastui.inputTips('password-confirm', lang.forgot.tips.input['password-confirm']);
        return;
    }

    if (fastui.testCompare('password', 'password-confirm') == false) {
        fastui.inputTips('password-confirm', lang.forgot.tips.input['password-confirm-error']);
        return;
    }

    if (fastui.testInput(true, 'vericode', /^[\d]{6}$/) == false) {
        fastui.inputTips('vericode', lang.forgot.tips.input['vericode']);
        return;
    } else {
        data += 'vericode=' + vericode + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/user/forgot-password.ashx?action=reset-password', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.textTips(lang.forgot.tips['reset-success']);

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('forgot-password');
                    }, 500);
            } else if (data == 'user-unexist') {
                fastui.textTips(lang.forgot.tips['user-unexist']);
            } else if (data == 'vericode-error') {
                fastui.textTips(lang.forgot.tips['vericode-error']);
            } else {
                fastui.textTips(lang.forgot.tips['operation-failed']);
            }
            }
        });
}