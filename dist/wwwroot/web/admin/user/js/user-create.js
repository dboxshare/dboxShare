$include('/storage/data/department-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    if (typeof(departmentDataJson) == 'undefined') {
        $id('department-select').parentNode.style.display = 'none';
    } else {
        fastui.listTreeSelect('department-select', departmentDataJson, 'ds_id', 'ds_departmentid', 'ds_name', 0, 0, false);
    }

    itemAdd();

    fastui.valueTips('username_0', lang.user.tips.value['username']);
    fastui.valueTips('password_0', lang.user.tips.value['password-add']);
    fastui.valueTips('name_0', lang.user.tips.value['name']);
    fastui.valueTips('code_0', lang.user.tips.value['code']);
    fastui.valueTips('title_0', lang.user.tips.value['title']);
    fastui.valueTips('email_0', lang.user.tips.value['email']);
    fastui.valueTips('phone_0', lang.user.tips.value['phone']);
    fastui.valueTips('tel_0', lang.user.tips.value['tel']);

    // 列表页面布局调整
    fastui.list.layoutResize();

    // 编辑列表初始化
    fastui.list.editListInit();
}


// 用户项目添加
function itemAdd() {
    var list = fastui.list.dataTable('data-list');
    var index = list.rows.length;
    var row = list.addRow(index);

    row.id = new Date().getTime();

    // 移除
    row.addCell(
        {width: '32'}, 
        '<span class="button-remove" onClick="itemRemove(' + row.id + ');"></span>'
        );

    // 用户账号
    row.addCell(
        {width: '200'}, 
        '<input name="username" type="text" id="username_' + index + '" maxlength="16" />'
        );

    // 登录密码
    row.addCell(
        {width: '200'}, 
        '<input name="password" type="password" class="ime-disabled" id="password_' + index + '" maxlength="16" />'
        );

    // 真实姓名
    row.addCell(
        {width: '200'}, 
        '<input name="name" type="text" id="name_' + index + '" maxlength="24" />'
        );

    // 员工编号
    row.addCell(
        {width: '200'}, 
        '<input name="code" type="text" class="ime-disabled" id="code_' + index + '" maxlength="16" />'
        );

    // 职称
    row.addCell(
        {width: '200'}, 
        '<input name="title" type="text" id="title_' + index + '" maxlength="32" />'
        );

    // 电子邮箱
    row.addCell(
        {width: '200'}, 
        '<input name="email" type="text" class="ime-disabled" id="email_' + index + '" maxlength="50" />'
        );

    // 手机号码
    row.addCell(
        {width: '200'}, 
        '<input name="phone" type="text" class="ime-disabled" id="phone_' + index + '" maxlength="17" />'
        );

    // 座机号码
    row.addCell(
        {width: '200'}, 
        '<input name="tel" type="text" class="ime-disabled" id="tel_' + index + '" maxlength="28" />'
        );

    row.addCell(null, '');
}


// 用户项目移除
function itemRemove(id) {
    var list = $id('data-list');
    var row = $id(id);

    if (row == null) {
        return;
    }

    list.deleteRow(row.rowIndex);
}


// 用户创建表单提交
function userCreate() {
    var department = $id('department');
    var username = $name('username');
    var password = $name('password');
    var name = $name('name');
    var code = $name('code');
    var title = $name('title');
    var email = $name('email');
    var phone = $name('phone');
    var tel = $name('tel');
    var data = '';

    if (typeof(departmentDataJson) == 'undefined') {
        data += 'departmentid=0&';
    } else {
        data += 'departmentid=' + department.value + '&';
    }

    if (username.length == 0) {
        itemAdd();
        return;
    }

    for (var i = 0; i < username.length; i++) {
        if (fastui.testString(username[i].value, /^[\d]+$/) == true) {
            fastui.textTips(lang.user.tips.input['username-pure-number-error']);
            username[i].focus();
            return;
        }

        if (fastui.testString(username[i].value, /^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\"\\\|\,\.\<\>\/\?]{2,16}$/) == false) {
            fastui.textTips(lang.user.tips.input['username']);
            username[i].focus();
            return;
        } else {
            data += 'username=' + escape(username[i].value) + '&';
        }

        if (password[i].value.length == 0) {
            data += 'password=' + escape(randomPassword()) + '&';
        } else {
            if (fastui.testString(password[i].value, /^[^\s\'\"\%\*\<\>\=]{6,16}$/) == false) {
                fastui.textTips(lang.user.tips.input['password']);
                password[i].focus();
                return;
            } else {
                data += 'password=' + escape(password[i].value) + '&';
            }
        }

        if (name[i].value.length == 0) {
            data += 'name=&';
        } else {
            if (fastui.testString(name[i].value, /^[\s\S]{2,24}$/) == false) {
                fastui.textTips(lang.user.tips.input['name']);
                name[i].focus();
                return;
            } else {
                data += 'name=' + escape(name[i].value) + '&';
            }
        }

        if (code[i].value.length == 0) {
            data += 'code=&';
        } else {
            if (fastui.testString(code[i].value, /^[\w\-]{2,16}$/) == false) {
                fastui.textTips(lang.user.tips.input['code']);
                code[i].focus();
                return;
            } else {
                data += 'code=' + escape(code[i].value) + '&';
            }
        }

        if (title[i].value.length == 0) {
            data += 'title=&';
        } else {
            if (fastui.testString(title[i].value, /^[\s\S]{2,32}$/) == false) {
                fastui.textTips(lang.user.tips.input['title']);
                title[i].focus();
                return;
            } else {
                data += 'title=' + escape(title[i].value) + '&';
            }
        }

        if (email[i].value.length == 0) {
            if (password[i].value.length == 0) {
                fastui.textTips(lang.user.tips.input['password-and-email-empty']);
                password[i].focus();
                return;
            }

            data += 'email=&';
        } else {
            if (fastui.testString(email[i].value, /^[\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$/) == false) {
                fastui.textTips(lang.user.tips.input['email']);
                email[i].focus();
                return;
            } else {
                data += 'email=' + escape(email[i].value) + '&';
            }
        }

        if (phone[i].value.length == 0) {
            data += 'phone=&';
        } else {
            if (fastui.testString(phone[i].value, /^\+?([\d]{2,4}\-?)?[\d]{6,11}$/) == false) {
                fastui.textTips(lang.user.tips.input['phone']);
                phone[i].focus();
                return;
            } else {
                data += 'phone=' + escape(phone[i].value) + '&';
            }
        }

        if (tel[i].value.length == 0) {
            data += 'tel=&';
        } else {
            if (fastui.testString(tel[i].value, /^\+?([\d]{2,4}\-?){0,2}[\d]{6,8}(\-?[\d]{2,8})?$/) == false) {
                fastui.textTips(lang.user.tips.input['tel']);
                tel[i].focus();
                return;
            } else {
                data += 'tel=' + escape(tel[i].value) + '&';
            }
        }
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/user/create', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.dataListActionCallback('complete');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('user-create');
                    }, 500);
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