// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('password', lang.user.tips.value['password-modify']);
    fastui.valueTips('password-confirm', lang.user.tips.value['password-confirm']);
    fastui.valueTips('name', lang.user.tips.value['name']);
    fastui.valueTips('email', lang.user.tips.value['email']);
    fastui.valueTips('phone', lang.user.tips.value['phone']);
    fastui.valueTips('tel', lang.user.tips.value['tel']);
    fastui.sideTips('password', lang.user.tips.side['password']);
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/api/account/user-profile-data-json', 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.user.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('username').value = jsonData.ds_username;
                $id('name').value = jsonData.ds_name;
                $id('email').value = jsonData.ds_email;
                $id('phone').value = jsonData.ds_phone;
                $id('tel').value = jsonData.ds_tel;
            }

            dataInit();

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 用户修改表单提交
function userModify() {
    var password = $id('password').value;
    var name = $id('name').value;
    var email = $id('email').value;
    var phone = $id('phone').value;
    var tel = $id('tel').value;
    var data = '';

    if (fastui.testInput(false, 'password', /^[^\s\'\"\%\*\<\>\=]{6,16}$/) == false) {
        fastui.inputTips('password', lang.user.tips.input['password']);
        return;
    } else if (password.length > 0) {
        if (fastui.testInput(false, 'password-confirm', /^[^\s\'\"\%\*\<\>\=]{6,16}$/) == false) {
            fastui.inputTips('password-confirm', lang.user.tips.input['password-confirm']);
            return;
        }

        if (fastui.testCompare('password', 'password-confirm') == false) {
            fastui.inputTips('password-confirm', lang.user.tips.input['password-confirm-error']);
            return;
        }

        data += 'password=' + escape(password) + '&';
    }

    if (fastui.testInput(false, 'name', /^[\s\S]{2,24}$/) == false) {
        fastui.inputTips('name', lang.user.tips.input['name']);
        return;
    } else if (name.length > 0) {
        data += 'name=' + escape(name) + '&';
    }

    if (fastui.testInput(true, 'email', /^[\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$/) == false) {
        fastui.inputTips('email', lang.user.tips.input['email']);
        return;
    } else {
        data += 'email=' + escape(email) + '&';
    }

    if (fastui.testInput(false, 'phone', /^\+?([\d]{2,4}\-?)?[\d]{6,11}$/) == false) {
        fastui.inputTips('phone', lang.user.tips.input['phone']);
        return;
    } else if (phone.length > 0) {
        data += 'phone=' + escape(phone) + '&';
    }

    if (fastui.testInput(false, 'tel', /^\+?([\d]{2,4}\-?){0,2}[\d]{6,8}(\-?[\d]{2,8})?$/) == false) {
        fastui.inputTips('tel', lang.user.tips.input['tel']);
        return;
    } else if (tel.length > 0) {
        data += 'tel=' + escape(tel) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/account/user-profile', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.iconTips('tick');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('profile');
                    }, 500);
            } else if (data == 'email-existed') {
                fastui.inputTips('email', lang.user.tips['email-existed']);
            } else if (data == 'phone-existed') {
                fastui.inputTips('phone', lang.user.tips['phone-existed']);
            } else {
                fastui.textTips(lang.user.tips['operation-failed']);
            }
            }
        });
}