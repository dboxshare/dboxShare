$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataReader();
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

    fastui.valueTips('password', lang.user.tips.value['password-modify']);
    fastui.valueTips('password-confirm', lang.user.tips.value['password-confirm']);
    fastui.valueTips('name', lang.user.tips.value['name']);
    fastui.valueTips('code', lang.user.tips.value['code']);
    fastui.valueTips('title', lang.user.tips.value['title']);
    fastui.valueTips('email', lang.user.tips.value['email']);
    fastui.valueTips('phone', lang.user.tips.value['phone']);
    fastui.valueTips('tel', lang.user.tips.value['tel']);
    fastui.sideTips('password', lang.user.tips.side['password']);
    fastui.sideTips('uploadsize', lang.user.tips.side['upload-size']);
    fastui.sideTips('downloadsize', lang.user.tips.side['download-size']);
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/api/admin/user/data-json?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.user.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('department').value = jsonData.ds_departmentid.indexOf('/') == -1 ? jsonData.ds_departmentid : jsonData.ds_departmentid.match(/\/([\d]+)\/$/i)[1];
                $id('role').value = jsonData.ds_roleid;
                $id('username').value = jsonData.ds_username;
                $id('name').value = jsonData.ds_name;
                $id('code').value = jsonData.ds_code;
                $id('title').value = jsonData.ds_title;
                $id('email').value = jsonData.ds_email;
                $id('phone').value = jsonData.ds_phone;
                $id('tel').value = jsonData.ds_tel;
                $id('uploadsize').value = jsonData.ds_uploadsize;
                $id('downloadsize').value = jsonData.ds_downloadsize;
                $id('admin').checked = jsonData.ds_admin == 1 ? true : false;
                $id('freeze').checked = jsonData.ds_freeze == 1 ? true : false;
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
    var name = $id('name').value;
    var code = $id('code').value;
    var title = $id('title').value;
    var email = $id('email').value;
    var phone = $id('phone').value;
    var tel = $id('tel').value;
    var uploadSize = $id('uploadsize').value;
    var downloadSize = $id('downloadsize').value;
    var admin = $id('admin').checked;
    var freeze = $id('freeze').checked;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.user.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

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
    data += 'freeze=' + freeze + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/user/modify', 
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
            } else if (data == 'phone-existed') {
                fastui.inputTips('phone', lang.user.tips['phone-existed']);
            } else {
                fastui.textTips(lang.user.tips['operation-failed']);
            }
            }
        });
}