$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');
$include('/storage/data/user-data-json.js');


// 页面初始化事件调用函数
function init() {
    pageInit();
    pageResize();
    dataReader();
}


// 页面初始化
function pageInit() {
    if (typeof(jsonDataDepartment) == 'undefined') {
        $id('button-department-add').style.display = 'none';
    }

    if (typeof(jsonDataRole) == 'undefined') {
        $id('button-role-add').style.display = 'none';
    }
}


// 页面调整
function pageResize() {
    var container = $id('container');
    var footer = $id('footer');

    container.style.height = (container.offsetHeight - footer.offsetHeight) + 'px';
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/web/drive/purview/purview-data-json.ashx?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if (data.length == 0 || data == '{}') {
                fastui.coverTips(lang.file.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('share').checked = jsonData.dbs_share == 1 ? true : false;
                $id('sync').checked = jsonData.dbs_sync == 1 ? true : false;

                if (jsonData.dbs_share == 0) {
                    openShare();
                }
            }

            fastui.list.dataLoad('/web/drive/purview/purview-list-json.ashx');

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 开启共享(生成按钮)
function openShare() {
    var clientWidth = $page().client.width;
    var clientHeight = $page().client.height;
    var layer;
    var html = '';

    fastui.coverShow('page-cover');

    html = '<span class=\"button\" onClick=\"openShareOk();\">' + lang.file.button['open-share'] + '</span>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'open-share'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil((clientHeight - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil((clientWidth - layer.offsetWidth) / 2) + 'px';
}


// 开启共享(提交)
function openShareOk() {
    var id = $query('id');
    var data = 'id=' + id + '&share=true';

    $ajax({
        type: 'POST', 
        url: '/web/drive/purview/purview-action.ashx?action=share', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                $id('share').checked = true;

                $id('open-share').$remove();

                fastui.coverHide('page-cover');

                fastui.iconTips('tick');
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 数据列表项目删除(询问)
function dataListItemDelete(index) {
    var id = jsonData.items[index].dbs_fileid;
    var type;
    var typeId;
    var typeName;

    if (jsonData.items[index].dbs_departmentid.length > 0) {
        type = 'department';
        typeId = jsonData.items[index].dbs_departmentid;
        typeName = purviewDepartment(typeId);
    } else if (jsonData.items[index].dbs_roleid > 0) {
        type = 'role';
        typeId = jsonData.items[index].dbs_roleid;
        typeName = purviewRole(typeId);
    } else if (jsonData.items[index].dbs_userid > 0) {
        type = 'user';
        typeId = jsonData.items[index].dbs_userid;
        typeName = purviewUser(typeId);
    }

    fastui.dialogConfirm(lang.user.tips.confirm['delete'] + '<br />' + typeName, 'dataListItemDeleteOk(' + id + ', \'' + type + '\', \'' + typeId + '\');');
}


// 数据列表项目删除(提交)
function dataListItemDeleteOk(id, type, typeId) {
    var data = 'id=' + id + '&type=' + type + '&typeid=' + typeId;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/purview/purview-action.ashx?action=delete', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 数据列表操作(回调函数)
function dataListActionCallback(data) {
    fastui.coverHide('waiting-cover');

    if (data == 'complete') {
        fastui.list.dataLoad(fastui.list.path, true);

        fastui.iconTips('tick');
    } else if (data == 'no-permission') {
        fastui.textTips(lang.file.tips['no-permission']);
    } else {
        fastui.iconTips('cross');
    }
}


// 数据列表视图
function dataListView() {
    var list = fastui.list.dataTable('data-list');
    var row;
    var type;
    var typeId;
    var i;

    for (i = 0; i < jsonData.items.length; i++) {
        row = list.addRow(i);

        if (jsonData.items[i].dbs_departmentid.length > 0) {
            type = 'department';
            typeId = jsonData.items[i].dbs_departmentid;
        } else if (jsonData.items[i].dbs_roleid > 0) {
            type = 'role';
            typeId = jsonData.items[i].dbs_roleid;
        } else if (jsonData.items[i].dbs_userid > 0) {
            type = 'user';
            typeId = jsonData.items[i].dbs_userid;
        }

        // 图标
        row.addCell(
            {width: '32'}, 
            function() {
                if (type == 'department') {
                    return '<img src=\"/ui/images/datalist-department-icon.png\" width=\"32\" />';
                } else if (type == 'role') {
                    return '<img src=\"/ui/images/datalist-role-icon.png\" width=\"32\" />';
                } else if (type == 'user') {
                    return '<img src=\"/ui/images/datalist-user-icon.png\" width=\"32\" />';
                } else {
                    return '';
                }
            });

        // 名称
        row.addCell(
            {width: '250'}, 
            function() {
                if (type == 'department') {
                    return '' + purviewDepartment(jsonData.items[i].dbs_departmentid) + '';
                } else if (type == 'role') {
                    return '' + purviewRole(jsonData.items[i].dbs_roleid) + '';
                } else if (type == 'user') {
                    return '' + purviewUser(jsonData.items[i].dbs_userid) + '';
                } else {
                    return '';
                }
            });

        // 权限
        row.addCell(
            {align: 'right'}, 
            '<div id=\"purview-select-' + i + '\" onClick=\"purviewModify(\'' + type + '\', \'' + typeId + '\', ' + i + ');\"><input name=\"purview-' + i + '\" type="hidden" id=\"purview-' + i + '\" value=\"' + jsonData.items[i].dbs_purview + '\" /></div>'
            );

        fastui.radioSelect('purview-select-' + i, [{value:'viewer',name:'' + lang.file.data.role['viewer'] + ''},{value:'downloader',name:'' + lang.file.data.role['downloader'] + ''},{value:'uploader',name:'' + lang.file.data.role['uploader'] + ''},{value:'editor',name:'' + lang.file.data.role['editor'] + ''},{value:'manager',name:'' + lang.file.data.role['manager'] + ''}], 'value', 'name', '#2196F3');

        // 操作
        row.addCell(
            {width: '24'}, 
            function() {
                if (type == 'department') {
                    return '<span class=\"delete\" onClick=\"dataListItemDelete(' + i + ', \'department\');\">✕</span>';
                } else if (type == 'role') {
                    return '<span class=\"delete\" onClick=\"dataListItemDelete(' + i + ', \'role\');\">✕</span>';
                } else if (type == 'user') {
                    return '<span class=\"delete\" onClick=\"dataListItemDelete(' + i + ', \'user\');\">✕</span>';
                } else {
                    return '';
                }
            });
    }
}


// 获取部门
function purviewDepartment(id) {
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


// 获取角色
function purviewRole(id) {
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


// 获取用户
function purviewUser(id) {
    try {
        var i;

        for (i = 0; i < jsonDataUser.length; i++) {
            if (jsonDataUser[i].dbs_id == id) {
                return '<a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonDataUser[i].dbs_username)) + '\', 500, 500);\">' + jsonDataUser[i].dbs_username + '</a>&nbsp;&nbsp;<font color=\"#757575\">' + jsonDataUser[i].dbs_position + '</font>';
            }
        }

        return '';
        } catch(e) {}
}


// 权限选择(弹出选择窗口)
function purviewSelect(type) {
    var text;

    if (type == 'department') {
        text = lang.object['department'];
    } else if (type == 'role') {
        text = lang.object['role'];
    } else if (type == 'user') {
        text = lang.object['user'];
    }

    window.parent.fastui.windowPopup('purview-select', text, '/web/drive/purview/purview-select.html?type=' + type + '&iframe=purview-manage-iframe&callback=purviewSelectCallback', 800, 500);
}


// 权限选择(回调函数)
function purviewSelectCallback(type, data) {
    purviewAdd(type, data.split(','));
}


// 权限添加操作提交
function purviewAdd(type, items) {
    var id = $query('id');
    var data = '';
    var i;

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.file.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    data += 'type=' + type + '&';

    for (i = 0; i < items.length; i++) {
        data += 'typeid=' + items[i] + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/purview/purview-action.ashx?action=add', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                fastui.list.dataLoad(fastui.list.path, true);

                fastui.textTips(lang.file.tips['purview-add-ok']);
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 权限修改操作提交
function purviewModify(type, typeId, index) {
    var id = $query('id');
    var purview = $id('purview-' + index).value;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.file.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    data += 'type=' + type + '&';
    data += 'typeid=' + typeId + '&';
    data += 'purview=' + purview + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/purview/purview-action.ashx?action=modify', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                fastui.textTips(lang.file.tips['purview-change-ok']);
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 权限共享操作提交
function purviewShare() {
    var id = $query('id');
    var share = $id('share').checked;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.file.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    data += 'share=' + share + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/purview/purview-action.ashx?action=share', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                fastui.textTips(share == true ? lang.file.tips['purview-share-ok'] : lang.file.tips['purview-unshare-ok']);
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 权限同步操作提交
function purviewSync() {
    var id = $query('id');
    var sync = $id('sync').checked;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.file.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    data += 'sync=' + sync + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/purview/purview-action.ashx?action=sync', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                fastui.textTips(sync == true ? lang.file.tips['purview-sync-ok'] : lang.file.tips['purview-unsync-ok']);
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 提交权限更改
function purviewChange() {
    var id = $query('id');
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.file.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/purview/purview-action.ashx?action=change', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                fastui.textTips(lang.file.tips['purview-change-ok']);
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}