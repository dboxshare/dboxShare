$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataListPageInit();
    dataListFilter();
    dataListLocation();

    fastui.list.scrollDataLoad('/api/admin/user/list-json');
}


// 页面调整事件调用函数
function resize() {
    dataListPageInit();
}


// 数据列表页面初始化
function dataListPageInit() {
    var keyword = $query('keyword');

    // 列表页面布局调整
    fastui.list.layoutResize();

    if (typeof(departmentDataJson) == 'undefined') {
        $id('filter-department').style.display = 'none';
        $id('button-classify-department').style.display = 'none';
        $id('location').style.display = 'none';
    }

    if (typeof(roleDataJson) == 'undefined') {
        $id('filter-role').style.display = 'none';
        $id('button-classify-role').style.display = 'none';
    }

    if (typeof(departmentDataJson) == 'undefined' && typeof(roleDataJson) == 'undefined') {
        $id('button-classify').style.display = 'none';
    }

    if (keyword.length == 0) {
        fastui.valueTips('keyword', lang.user.tips.value['keyword']);
    } else {
        $id('keyword').value = keyword;
    }
}


// 数据列表筛选
function dataListFilter() {
    var departmentId = $query('departmentid') || 0;
    var roleId = $query('roleid');
    var status = $query('status');
    var items = '';

    if (typeof(departmentDataJson) != 'undefined') {
        if (departmentId == 0) {
            items = '<li class="item-current">' + lang.user['unlimited'] + '</li>';
        } else {
            items = '<li onClick="$location(\'departmentid\', \'\');">' + lang.user['unlimited'] + '</li>';
        }

        for (var i = 0; i < departmentDataJson.length; i++) {
            if (departmentDataJson[i].ds_departmentid == departmentId) {
                if (departmentDataJson[i].ds_id == departmentId) {
                    items += '<li class="item-current">' + departmentDataJson[i].ds_name + '</li>';
                } else {
                    items += '<li onClick="$location(\'departmentid\', ' + departmentDataJson[i].ds_id + ');">' + departmentDataJson[i].ds_name + '</li>';
                }
            }
        }

        $id('filter-department').innerHTML += '<ul>' + items + '</ul>';
    }

    if (typeof(roleDataJson) != 'undefined') {
        if (roleId == 0) {
            items = '<li class="item-current">' + lang.user['unlimited'] + '</li>';
        } else {
            items = '<li onClick="$location(\'roleid\', \'\');">' + lang.user['unlimited'] + '</li>';
        }

        for (var j = 0; j < roleDataJson.length; j++) {
            if (roleDataJson[j].ds_id == roleId) {
                items += '<li class="item-current">' + roleDataJson[j].ds_name + '</li>';
            } else {
                items += '<li onClick="$location(\'roleid\', ' + roleDataJson[j].ds_id + ');">' + roleDataJson[j].ds_name + '</li>';
            }
        }

        $id('filter-role').innerHTML += '<ul>' + items + '</ul>';
    }

    if (status.length == 0) {
        items = '<li class="item-current">' + lang.user['unlimited'] + '</li>';
    } else {
        items = '<li onClick="$location(\'status\', \'\');">' + lang.user['unlimited'] + '</li>';
    }

    for (var k in lang.user.data.status) {
        if (k == status) {
            items += '<li class="item-current">' + lang.user.data.status[k] + '</li>';
        } else {
            items += '<li onClick="$location(\'status\', \'' + k + '\');">' + lang.user.data.status[k] + '</li>';
        }
    }

    $id('filter-status').innerHTML += '<ul>' + items + '</ul>';
}


// 数据列表路径
function dataListLocation() {
    try {
        var id = $query('departmentid') || 0;
        var path = $id('location').$class('path')[0];
        var departmentId;
        var name;

        if (id == 0) {
            path.innerHTML = lang.user['all'];
            return;
        } else {
            path.innerHTML = '<a href="/web/admin/user/user-list.html">' + lang.user['all'] + '</a>';
        }

        for (var i = 0; i < departmentDataJson.length; i++) {
            if (departmentDataJson[i].ds_id == id) {
                departmentId = departmentDataJson[i].ds_departmentid;
                name = departmentDataJson[i].ds_name;
                break;
            }
        }

        var location = ' / ' + name;

        while (departmentId > 0) {
            for (var j = 0; j < departmentDataJson.length; j++) {
                if (departmentDataJson[j].ds_id == departmentId) {
                    id = departmentDataJson[j].ds_id;
                    departmentId = departmentDataJson[j].ds_departmentid;
                    name = departmentDataJson[j].ds_name;
                    break;
                }
            }

            location = ' / <a href="javascript: $location(\'departmentid\', ' + id + ');">' + name + '</a>' + location;
        }

        path.innerHTML += location;
    } catch(e) {}
}


// 数据列表项目移除(询问)
function dataListItemRemove(index) {
    var id = jsonData.items[index].ds_id;
    var username = jsonData.items[index].ds_username;

    fastui.dialogConfirm(lang.user.tips.confirm['remove'] + '<br />' + username, 'dataListItemRemoveOk(' + id + ')');
}


// 数据列表项目移除(提交)
function dataListItemRemoveOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/user/remove', 
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
        case 'remove-all':
        tips = lang.user.tips.confirm['remove-all'];
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

    if (field.length == 0) {
        fastui.list.sortChange('username', false);
    } else {
        jsonData.items.sort($jsonSort(field, reverse, primer));

        fastui.list.sortChange(field.substring(field.indexOf('_') + 1), reverse);
    }

    var rows = list.rows.length;

    for (var i = rows; i < jsonData.items.length; i++) {
        var row = list.addRow(i);

        // 复选框
        row.addCell(
            {width: '20', align: 'center'}, 
            '<input name="id" type="checkbox" id="user_' + jsonData.items[i].ds_id + '" value="' + jsonData.items[i].ds_id + '" /><label for="user_' + jsonData.items[i].ds_id + '"></label>'
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
            {width: '150'}, 
            '' + userRole(jsonData.items[i].ds_roleid) + ''
            );

        // 限制
        row.addCell(
            {width: '150'}, 
            function() {
                var html = '';

                html += '↑ ' + jsonData.items[i].ds_uploadsize + ' MB';
                html += '<br />';
                html += '↓ ' + jsonData.items[i].ds_downloadsize + ' MB';

                return html;
            });

        // 状态
        row.addCell(
            {width: '100'}, 
            '' + userStatus(jsonData.items[i].ds_freeze) + ''
            );

        // 操作
        row.addCell(
            {width: '250'}, 
            function() {
                var html = '';

                html += '<div class="datalist-action">';
                html += '<span class="button" onClick="fastui.windowPopup(\'user-modify\', \'' + lang.user.button['modify'] + ' - ' + jsonData.items[i].ds_username + '\', \'/web/admin/user/user-modify.html?id=' + jsonData.items[i].ds_id + '\', 800, 500);">' + lang.user.button['modify'] + '</span>';
                html += '<span class="button" onClick="fastui.windowPopup(\'user-transfer\', \'' + lang.user.button['transfer'] + ' - ' + jsonData.items[i].ds_username + '\', \'/web/admin/user/user-transfer.html?id=' + jsonData.items[i].ds_id + '\', 800, 500);">' + lang.user.button['transfer'] + '</span>';
                html += '<span class="button" onClick="dataListItemRemove(' + i + ');">' + lang.user.button['remove'] + '</span>';
                html += '</div>';

                return html;
            });

        if (i - rows == fastui.list.size) {
            break;
        }
    }

    // 数据列表事件绑定(点击选择项目)
    fastui.list.bindEvent(rows);

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
function userStatus(freeze) {
    if (freeze == 0) {
        return lang.user.data.status['on-job'];
    } else if (freeze == 1) {
        return lang.user.data.status['off-job'];
    }
}