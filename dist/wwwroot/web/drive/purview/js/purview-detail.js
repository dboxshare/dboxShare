$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');
$include('/storage/data/user-data-json.js');


// 页面初始化事件调用函数
function init() {
    fastui.list.dataLoad('/api/drive/purview/list-json');
}


// 数据列表视图
function dataListView() {
    var list = fastui.list.dataTable('data-list');

    for (var i = 0; i < jsonData.items.length; i++) {
        var row = list.addRow(i);

        // 图标
        row.addCell(
            {width: '32'}, 
            function() {
                if (jsonData.items[i].ds_departmentid.length > 0) {
                    return '<img src="/ui/images/datalist-department-icon.png" width="32" />';
                } else if (jsonData.items[i].ds_roleid > 0) {
                    return '<img src="/ui/images/datalist-role-icon.png" width="32" />';
                } else if (jsonData.items[i].ds_userid > 0) {
                    return '<img src="/ui/images/datalist-user-icon.png" width="32" />';
                } else {
                    return '';
                }
            });

        // 名称
        row.addCell(
            {width: '250'}, 
            function() {
                if (jsonData.items[i].ds_departmentid.length > 0) {
                    return '' + purviewDepartment(jsonData.items[i].ds_departmentid) + '';
                } else if (jsonData.items[i].ds_roleid > 0) {
                    return '' + purviewRole(jsonData.items[i].ds_roleid) + '';
                } else if (jsonData.items[i].ds_userid > 0) {
                    return '' + purviewUser(jsonData.items[i].ds_userid) + '';
                } else {
                    return '';
                }
            });

        // 权限
        row.addCell(
            {align: 'right'}, 
            function() {
                var html = '';

                for (var j in lang.file.data.purview) {
                    if (j == jsonData.items[i].ds_purview) {
                        html += '<span class="purview-selected">' + lang.file.data.purview[j] + '</span>';
                    } else {
                        html += '<span class="purview-select">' + lang.file.data.purview[j] + '</span>';
                    }
                }

                return html;
            });
    }
}


// 获取部门
function purviewDepartment(id) {
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


// 获取角色
function purviewRole(id) {
    try {
        for (var i = 0; i < roleDataJson.length; i++) {
            if (roleDataJson[i].ds_id == id) {
                return roleDataJson[i].ds_name;
            }
        }

        return '';
        } catch(e) {}
}


// 获取用户
function purviewUser(id) {
    try {
        for (var i = 0; i < userDataJson.length; i++) {
            if (userDataJson[i].ds_id == id) {
                return '' + userDataJson[i].ds_username + '&nbsp;&nbsp;<font color="#757575">' + purviewDepartment(userDataJson[i].ds_departmentid) + '</font>';
            }
        }

        return '';
        } catch(e) {}
}