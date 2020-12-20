$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataListPageInit();

    fastui.list.scrollDataLoad('/web/admin/user/user-recycle-list-json.ashx');
}


// 页面调整事件调用函数
function resize() {
    dataListPageInit();
}


// 数据列表页面初始化
function dataListPageInit() {
    // 列表页面布局调整
    fastui.list.layoutResize();
}


// 数据列表项目还原(询问)
function dataListItemRestore(index) {
    var id = jsonData.items[index].dbs_id;
    var username = jsonData.items[index].dbs_username;

    fastui.dialogConfirm(lang.user.tips.confirm['restore'] + '<br />' + username, 'dataListItemRestoreOk(' + id + ')');
}


// 数据列表项目还原(提交)
function dataListItemRestoreOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/admin/user/user-action.ashx?action=restore', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 数据列表项目删除(询问)
function dataListItemDelete(index) {
    var id = jsonData.items[index].dbs_id;
    var username = jsonData.items[index].dbs_username;

    fastui.dialogConfirm(lang.user.tips.confirm['delete'] + '<br />' + username, 'dataListItemDeleteOk(' + id + ')');
}


// 数据列表项目删除(提交)
function dataListItemDeleteOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/admin/user/user-action.ashx?action=delete', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 数据列表操作(询问)
function dataListAction(action) {
    var checkboxes = $name('id');
    var selected = false;
    var tips;
    var i;

    // 检查是否已选择项目
    for (i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            selected = true;
            break;
        }
    }

    if (selected == false) {
        fastui.textTips(lang.user.tips['please-select-item']);
        return;
    }

    switch(action) {
        case 'restoreall':
        tips = lang.user.tips.confirm['restore-all'];
        break;

        case 'deleteall':
        tips = lang.user.tips.confirm['delete-all'];
        break;
    }

    fastui.dialogConfirm(tips, 'dataListActionOk(\'' + action + '\')');
}


// 数据列表操作(提交)
function dataListActionOk(action) {
    var checkboxes = $name('id');
    var data = '';
    var i;

    for (i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            data += 'id=' + checkboxes[i].value + '&';
        }
    }

    if (data.length > 0) {
        data = data.substring(0, data.length - 1);

        fastui.coverShow('waiting-cover');

        $ajax({
            type: 'POST', 
            url: '/web/admin/user/user-action.ashx?action=' + action, 
            async: true, 
            data: data, 
            callback: 'dataListActionCallback'
            });
    }
}


// 数据列表操作(回调函数)
function dataListActionCallback(data) {
    fastui.coverHide('waiting-cover');

    if (data == 'complete') {
        fastui.list.scrollDataLoad(fastui.list.path, true);

        fastui.iconTips('tick');
    } else {
        fastui.iconTips('cross');
    }
}


// 数据列表视图
function dataListView(field, reverse, primer) {
    var list = fastui.list.dataTable('data-list');
    var row;
    var i;
    var j;

    if (typeof(jsonData) == 'undefined') {
        return;
    }

    if (field.length > 0) {
        jsonData.items.sort($jsonSort(field, reverse, primer));

        fastui.list.sortChange(field.substring(field.indexOf('_') + 1), reverse);
    }

    j = list.rows.length;

    for (i = j; i < jsonData.items.length; i++) {
        row = list.addRow(i);

        // 复选框
        row.addCell(
        {width: '20', align: 'center'}, 
        '<input name=\"id\" type=\"checkbox\" class=\"checkbox\" value=\"' + jsonData.items[i].dbs_id + '\" />'
        );

        // 图标
        row.addCell(
            {width: '32'}, 
            '<img src=\"/ui/images/datalist-user-icon.png\" width=\"32\" />'
            );

        // 用户账号
        row.addCell(
            {width: '200'}, 
            function() {
                var html = '';

                html += '' + jsonData.items[i].dbs_username + '';
                html += '&nbsp;&nbsp;';
                html += '<font color=\"#757575\">' + jsonData.items[i].dbs_position + '</font>';

                return html;
            });

        // 部门
        row.addCell(
            null, 
            '' + userDepartment(jsonData.items[i].dbs_departmentid) + ''
            );

        // 角色
        row.addCell(
            {width: '100'}, 
            '' + userRole(jsonData.items[i].dbs_roleid) + ''
            );

        // 状态
        row.addCell(
            {width: '50'}, 
            '' + userStatus(jsonData.items[i].dbs_status) + ''
            );

        // 操作
        row.addCell(
            {width: '225'}, 
            function() {
                var html = '';

                html += '<div class=\"datalist-action\">';
                html += '<span class=\"button\" onClick=\"dataListItemRestore(' + i + ');\">' + lang.user.button['restore'] + '</span>';
                html += '<span class=\"button\" onClick=\"dataListItemDelete(' + i + ');\">' + lang.user.button['delete'] + '</span>';
                html += '</div>';

                return html;
            });

        if (i - j == fastui.list.size) {
            break;
        }
    }

    // 数据列表事件绑定(点击选择项目)
    fastui.list.bindEvent(j);

    // 数据列表分块加载(应用于数据重载)
    fastui.list.scrollLoadBlock(field, reverse, primer, i);
}


// 获取用户部门
function userDepartment(id) {
    if (typeof(jsonDataDepartment) == 'undefined') {
        return '';
    }

    try {
        var items = new Array();
        var department = '';
        var i;
        var j;

        items = id.split('/');

        for (i = 1; i < items.length - 1; i++) {
            for (j = 0; j < jsonDataDepartment.length; j++) {
                if (jsonDataDepartment[j].dbs_id == items[i]) {
                    department += jsonDataDepartment[j].dbs_name + ' / ';
                    break;
                }
            }
        }

        return department.substring(0, department.length - 3);
        } catch(e) {}
}


// 获取用户角色
function userRole(id) {
    if (typeof(jsonDataRole) == 'undefined') {
        return '';
    }

    try {
        var i;

        for (i = 0; i < jsonDataRole.length; i++) {
            if (jsonDataRole[i].dbs_id == id) {
                return jsonDataRole[i].dbs_name;
            }
        }

        return '';
        } catch(e) {}
}


// 获取用户状态
function userStatus(status) {
    try {

        if (status == 1) {
            return lang.user.data.status['job-on'];
        } else if (status == 0) {
            return lang.user.data.status['job-off'];
        }
        } catch(e) {}
}