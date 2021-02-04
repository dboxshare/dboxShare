$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataListPageInit();

    fastui.list.scrollDataLoad('/api/admin/user/recycle-list-json');
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
    var id = jsonData.items[index].ds_id;
    var username = jsonData.items[index].ds_username;

    fastui.dialogConfirm(lang.user.tips.confirm['restore'] + '<br />' + username, 'dataListItemRestoreOk(' + id + ')');
}


// 数据列表项目还原(提交)
function dataListItemRestoreOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/user/restore', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 数据列表项目删除(询问)
function dataListItemDelete(index) {
    var id = jsonData.items[index].ds_id;
    var username = jsonData.items[index].ds_username;

    fastui.dialogConfirm(lang.user.tips.confirm['delete'] + '<br />' + username, 'dataListItemDeleteOk(' + id + ')');
}


// 数据列表项目删除(提交)
function dataListItemDeleteOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/user/delete', 
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

    // 检查是否已选择项目
    for (var i = 0; i < checkboxes.length; i++) {
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
        case 'restore-all':
        tips = lang.user.tips.confirm['restore-all'];
        break;

        case 'delete-all':
        tips = lang.user.tips.confirm['delete-all'];
        break;
    }

    fastui.dialogConfirm(tips, 'dataListActionOk(\'' + action + '\')');
}


// 数据列表操作(提交)
function dataListActionOk(action) {
    var checkboxes = $name('id');
    var data = '';

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            data += 'id=' + checkboxes[i].value + '&';
        }
    }

    if (data.length > 0) {
        data = data.substring(0, data.length - 1);

        fastui.coverShow('waiting-cover');

        $ajax({
            type: 'POST', 
            url: '/api/admin/user/' + action, 
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

    if (typeof(jsonData) == 'undefined') {
        return;
    }

    if (field.length > 0) {
        jsonData.items.sort($jsonSort(field, reverse, primer));

        fastui.list.sortChange(field.substring(field.indexOf('_') + 1), reverse);
    }

    var j = list.rows.length;

    for (var i = j; i < jsonData.items.length; i++) {
        var row = list.addRow(i);

        // 复选框
        row.addCell(
        {width: '20', align: 'center'}, 
        '<input name="id" type="checkbox" value="' + jsonData.items[i].ds_id + '" /><label></label>'
        );

        // 图标
        row.addCell(
            {width: '32'}, 
            '<img src="/ui/images/datalist-user-icon.png" width="32" />'
            );

        // 用户账号
        row.addCell(
            {width: '250'}, 
            function() {
                var html = '';

                html += '' + jsonData.items[i].ds_username + '';
                html += '&nbsp;&nbsp;';
                html += '<font color="#757575">' + jsonData.items[i].ds_title + '</font>';

                return html;
            });

        // 部门
        row.addCell(
            null, 
            '' + userDepartment(jsonData.items[i].ds_departmentid) + ''
            );

        // 角色
        row.addCell(
            {width: '125'}, 
            '' + userRole(jsonData.items[i].ds_roleid) + ''
            );

        // 状态
        row.addCell(
            {width: '75'}, 
            '' + userStatus(jsonData.items[i].ds_status) + ''
            );

        // 操作
        row.addCell(
            {width: '225'}, 
            function() {
                var html = '';

                html += '<div class="datalist-action">';
                html += '<span class="button" onClick="dataListItemRestore(' + i + ');">' + lang.user.button['restore'] + '</span>';
                html += '<span class="button" onClick="dataListItemDelete(' + i + ');">' + lang.user.button['delete'] + '</span>';
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
    if (typeof(departmentDataJson) == 'undefined') {
        return '';
    }

    try {
        var items = id.split('/');
        var department = '';

        for (var i = 1; i < items.length - 1; i++) {
            for (var j = 0; j < departmentDataJson.length; j++) {
                if (departmentDataJson[j].ds_id == items[i]) {
                    department += departmentDataJson[j].ds_name + ' / ';
                    break;
                }
            }
        }

        return department.substring(0, department.length - 3);
        } catch(e) {}
}


// 获取用户角色
function userRole(id) {
    if (typeof(roleDataJson) == 'undefined') {
        return '';
    }

    try {
        for (var i = 0; i < roleDataJson.length; i++) {
            if (roleDataJson[i].ds_id == id) {
                return roleDataJson[i].ds_name;
            }
        }

        return '';
        } catch(e) {}
}


// 获取用户状态
function userStatus(status) {
    try {
        if (status == 0) {
            return lang.user.data.status['job-on'];
        } else if (status == -1) {
            return lang.user.data.status['job-off'];
        }
        } catch(e) {}
}