$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    if (typeof(departmentDataJson) == 'undefined') {
        $id('department-select').parentNode.parentNode.style.display = 'none';
    } else {
        fastui.listTreeSelect('department-select', departmentDataJson, 'ds_id', 'ds_departmentid', 'ds_name', 0, 0);
    }

    if (typeof(roleDataJson) == 'undefined') {
        $id('role-select').parentNode.parentNode.style.display = 'none';
    } else {
        fastui.listSelect('role-select', roleDataJson, 'ds_id', 'ds_name');
    }

    fastui.valueTips('username', lang.user.tips.value['username']);
    fastui.valueTips('password', lang.user.tips.value['password-add']);
    fastui.valueTips('password-confirm', lang.user.tips.value['password-confirm']);
    fastui.valueTips('name', lang.user.tips.value['name']);
    fastui.valueTips('code', lang.user.tips.value['code']);
    fastui.valueTips('title', lang.user.tips.value['title']);
    fastui.valueTips('email', lang.user.tips.value['email']);
    fastui.valueTips('phone', lang.user.tips.value['phone']);
    fastui.valueTips('tel', lang.user.tips.value['tel']);
    fastui.sideTips('username', lang.user.tips.side['username']);
    fastui.sideTips('password', lang.user.tips.side['password']);
    fastui.sideTips('uploadsize', lang.user.tips.side['upload-size']);
    fastui.sideTips('downloadsize', lang.user.tips.side['download-size']);
}


// 用户添加表单提交
function userAdd() {
    var department = $id('department');
    var role = $id('role');
    var username = $id('username').value;
    var password = $id('password').value;
    var name = $id('name').value;
    var code = $id('code').value;
    var title = $id('title').value;
    var email = $id('email').value;
    var phone = $id('phone').value;
    var tel = $id('tel').value;
    var uploadSize = $id('uploadsize').value;
    var downloadSize = $id('downloadsize').value;
    var admin = $id('admin').checked;
    var send = $id('send').checked;
    var data = '';

    if (typeof(departmentDataJson) == 'undefined') {
        data += 'departmentid=0&';
    } else {
        data += 'departmentid=' + department.value + '&';
    }

    if (typeof(roleDataJson) == 'undefined') {
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
    }

    if (fastui.testInput(false, 'name', /^[\s\S]{2,24}$/) == false) {
        fastui.inputTips('name', lang.user.tips.input['name']);
        return;
    } else if (name.length > 0) {
        data += 'name=' + escape(name) + '&';
    }

    if (fastui.testInput(false, 'code', /^[\w\-]{2,16}$/) == false) {
        fastui.inputTips('code', lang.user.tips.input['code']);
        return;
    } else if (code.length > 0) {
        data += 'code=' + escape(code) + '&';
    }

    if (fastui.testInput(false, 'title', /^[\s\S]{2,32}$/) == false) {
        fastui.inputTips('title', lang.user.tips.input['title']);
        return;
    } else if (title.length > 0) {
        data += 'title=' + escape(title) + '&';
    }

    if (fastui.testInput(false, 'email', /^[\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$/) == false) {
        fastui.inputTips('email', lang.user.tips.input['email']);
        return;
    } else if (email.length > 0) {
        data += 'email=' + escape(email) + '&';
    }

    if (password.length == 0 && email.length == 0) {
        fastui.inputTips('password', lang.user.tips.input['password-and-email-empty']);
        return;
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

    if (fastui.testInput(true, 'uploadsize', /^[\d]{1,5}$/) == false) {
        fastui.inputTips('uploadsize', lang.user.tips.input['upload-size']);
        return;
    } else {
        if (uploadSize < 0 || uploadSize > 10240) {
            fastui.inputTips('uploadsize', lang.user.tips.input['upload-size-range']);
            return;
        }

        data += 'uploadsize=' + uploadSize + '&';
    }

    if (fastui.testInput(true, 'downloadsize', /^[\d]{1,5}$/) == false) {
        fastui.inputTips('downloadsize', lang.user.tips.input['download-size']);
        return;
    } else {
        if (downloadSize < 0 || downloadSize > 10240) {
            fastui.inputTips('downloadsize', lang.user.tips.input['download-size-range']);
            return;
        }

        data += 'downloadsize=' + downloadSize + '&';
    }

    data += 'admin=' + admin + '&';
    data += 'send=' + send + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/user/add', 
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


// 获取随机密码
function randomPassword() {
    var chars = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    var key = '';

    for (var i = 0; i < 6; i++) {
        key += chars.charAt(Math.floor(Math.random() * chars.length));
    }

    return key;
}