$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    if (typeof(jsonDataDepartment) == 'undefined') {
        $id('department-select').parentNode.parentNode.style.display = 'none';
    } else {
        fastui.listTreeSelect('department-select', jsonDataDepartment, 'dbs_id', 'dbs_departmentid', 'dbs_name', 0, 0);
    }

    if (typeof(jsonDataRole) == 'undefined') {
        $id('role-select').parentNode.parentNode.style.display = 'none';
    } else {
        fastui.listSelect('role-select', jsonDataRole, 'dbs_id', 'dbs_name');
    }

    fastui.valueTips('username', lang.user.tips.value['username']);
    fastui.valueTips('password', lang.user.tips.value['password-add']);
    fastui.valueTips('password-confirm', lang.user.tips.value['password-confirm']);
    fastui.valueTips('code', lang.user.tips.value['code']);
    fastui.valueTips('position', lang.user.tips.value['position']);
    fastui.valueTips('email', lang.user.tips.value['email']);
    fastui.valueTips('phone', lang.user.tips.value['phone']);
    fastui.valueTips('tel', lang.user.tips.value['tel']);
    fastui.sideTips('username', lang.user.tips.side['username']);
    fastui.sideTips('password', lang.user.tips.side['password']);
}


// 用户添加表单提交
function userAdd() {
    var department = $id('department');
    var role = $id('role');
    var username = $id('username').value;
    var password = $id('password').value;
    var code = $id('code').value;
    var position = $id('position').value;
    var email = $id('email').value;
    var phone = $id('phone').value;
    var tel = $id('tel').value;
    var admin = $id('admin').checked;
    var send = $id('send').checked;
    var data = '';

    if (typeof(jsonDataDepartment) == 'undefined') {
        data += 'departmentid=0&';
    } else {
        data += 'departmentid=' + department.value + '&';
    }

    if (typeof(jsonDataRole) == 'undefined') {
        data += 'roleid=0&';
    } else {
        data += 'roleid=' + role.value + '&';
    }

    if (fastui.testInput(true, 'username', /^[\d]+$/) == true) {
        fastui.inputTips('username', lang.user.tips.input['username-pure-number-error']);
        return;
    }

    if (fastui.testInput(true, 'username', /^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\"\\\|\,\.\<\>\/\?]{2,16}$/) == false) {
        fastui.inputTips('username', lang.user.tips.input['username']);
        return;
    } else {
        data += 'username=' + escape(username) + '&';
    }

    if (password.length == 0) {
        data += 'password=' + escape(randomPassword()) + '&';
    } else {
        if (fastui.testInput(true, 'password', /^[\S]{6,16}$/) == false) {
            fastui.inputTips('password', lang.user.tips.input['password']);
            return;
        } else {
            data += 'password=' + escape(password) + '&';
        }

        if (fastui.testInput(true, 'password-confirm', /^[\S]{6,16}$/) == false) {
            fastui.inputTips('password-confirm', lang.user.tips.input['password-confirm']);
            return;
        }

        if (fastui.testCompare('password', 'password-confirm') == false) {
            fastui.inputTips('password-confirm', lang.user.tips.input['password-confirm-error']);
            return;
        }
    }

    if (fastui.testInput(false, 'code', /^[\w\-]{2,16}$/) == false) {
        fastui.inputTips('code', lang.user.tips.input['code']);
        return;
    } else if (code.length > 0) {
        data += 'code=' + escape(code) + '&';
    }

    if (fastui.testInput(false, 'position', /^[\s\S]{2,32}$/) == false) {
        fastui.inputTips('position', lang.user.tips.input['position']);
        return;
    } else if (position.length > 0) {
        data += 'position=' + escape(position) + '&';
    }

    if (fastui.testInput(false, 'email', /^[\w\-]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$/) == false) {
        fastui.inputTips('email', lang.user.tips.input['email']);
        return;
    } else if (email.length > 0) {
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

    data += 'admin=' + admin + '&';
    data += 'send=' + send + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/admin/user/user-action.ashx?action=add', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.dataListActionCallback('complete');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('user-add');
                    }, 500);
            } else if (data == 'username-existed') {
                fastui.inputTips('username', lang.user.tips['username-existed']);

                $id('username').focus();
            } else if (data == 'email-existed') {
                fastui.inputTips('email', lang.user.tips['email-existed']);

                $id('email').focus();
            } else if (data == 'phone-existed') {
                fastui.inputTips('phone', lang.user.tips['phone-existed']);

                $id('phone').focus();
            } else {
                fastui.textTips(lang.user.tips['operation-failed']);
            }
            }
        });
}


// 获取随机密码
function randomPassword() {
    var chars = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    var key = '';
    var i;

    for (i = 0; i < 6; i++) {
        key += chars.charAt(Math.floor(Math.random() * chars.length));
    }

    return key;
}