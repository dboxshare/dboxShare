// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('password', lang.user.tips.value['password']);
    fastui.valueTips('password-confirm', lang.user.tips.value['password-confirm']);
    fastui.valueTips('vericode', lang.user.tips.value['vericode']);
    fastui.sideTips('password', lang.user.tips.side['password']);
    fastui.sideTips('vericode', lang.user.tips.side['vericode']);
}


// 重设登录密码表单提交
function resetPassword() {
    var password = $id('password').value;
    var vericode = $id('vericode').value;
    var data = '';

    if (fastui.testInput(true, 'password', /^[^\s\'\"\%\*\<\>\=]{6,16}$/) == false) {
        fastui.inputTips('password', lang.user.tips.input['password']);
        return;
    } else {
        data += 'password=' + escape(password) + '&';
    }

    if (fastui.testInput(true, 'password-confirm', /^[^\s\'\"\%\*\<\>\=]{6,16}$/) == false) {
        fastui.inputTips('password-confirm', lang.user.tips.input['password-confirm']);
        return;
    }

    if (fastui.testCompare('password', 'password-confirm') == false) {
        fastui.inputTips('password-confirm', lang.user.tips.input['password-confirm-error']);
        return;
    }

    if (fastui.testInput(true, 'vericode', /^[\d]{6}$/) == false) {
        fastui.inputTips('vericode', lang.user.tips.input['vericode']);
        return;
    } else {
        data += 'vericode=' + vericode + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/account/user-forgot/reset-password', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.iconTips('tick');

                window.setTimeout(function() {
                    window.parent.fastui.textTips(lang.user.tips['reset-success'], 0, 8);

                    window.parent.fastui.windowClose('forgot-password');
                    }, 500);
            } else if (data == 'user-unexist') {
                fastui.textTips(lang.user.tips['user-unexist']);
            } else if (data == 'vericode-error') {
                fastui.textTips(lang.user.tips['vericode-error']);
            } else {
                fastui.textTips(lang.user.tips['operation-failed']);
            }
            }
        });
}