$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataReader();
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

    fastui.valueTips('password', lang.user.tips.value['password-modify']);
    fastui.valueTips('password-confirm', lang.user.tips.value['password-confirm']);
    fastui.valueTips('code', lang.user.tips.value['code']);
    fastui.valueTips('position', lang.user.tips.value['position']);
    fastui.valueTips('email', lang.user.tips.value['email']);
    fastui.valueTips('phone', lang.user.tips.value['phone']);
    fastui.valueTips('tel', lang.user.tips.value['tel']);
    fastui.sideTips('password', lang.user.tips.side['password']);
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/web/admin/user/user-data-json.ashx?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if (data.length == 0 || data == '{}') {
                fastui.coverTips(lang.user.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('department').value = jsonData.dbs_departmentid.indexOf('/') == -1 ? jsonData.dbs_departmentid : jsonData.dbs_departmentid.match(/\/([\d]+)\/$/i)[1];
                $id('role').value = jsonData.dbs_roleid;
                $id('username').value = jsonData.dbs_username;
                $id('code').value = jsonData.dbs_code;
                $id('position').value = jsonData.dbs_position;
                $id('email').value = jsonData.dbs_email;
                $id('phone').value = jsonData.dbs_phone;
                $id('tel').value = jsonData.dbs_tel;
                $id('admin').checked = (jsonData.dbs_admin == 1 ? true : false);
                $id('leave').checked = (jsonData.dbs_status == 0 ? true : false);
            }

            dataInit();

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 用户修改表单提交
function userModify() {
    var id = $query('id');
    var department = $id('department');
    var role = $id('role');
    var password = $id('password').value;
    var code = $id('code').value;
    var position = $id('position').value;
    var email = $id('email').value;
    var phone = $id('phone').value;
    var tel = $id('tel').value;
    var admin = $id('admin').checked;
    var leave = $id('leave').checked;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.user.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

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

    if (fastui.testInput(false, 'password', /^[\S]{6,16}$/) == false) {
        fastui.inputTips('password', lang.user.tips.input['password']);
        return;
    } else if (password.length > 0) {
        if (fastui.testInput(false, 'password-confirm', /^[\S]{6,16}$/) == false) {
            fastui.inputTips('password-confirm', lang.user.tips.input['password-confirm']);
            return;
        }

        if (fastui.testCompare('password', 'password-confirm') == false) {
            fastui.inputTips('password-confirm', lang.user.tips.input['password-confirm-error']);
            return;
        }

        data += 'password=' + escape(password) + '&';
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
    data += 'leave=' + leave + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/admin/user/user-action.ashx?action=modify', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.dataListActionCallback('complete');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('user-modify');
                    }, 500);
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