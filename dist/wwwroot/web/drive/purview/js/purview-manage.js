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
    if (typeof(departmentDataJson) == 'undefined') {
        $id('button-department-add').style.display = 'none';
    }

    if (typeof(roleDataJson) == 'undefined') {
        $id('button-role-add').style.display = 'none';
    }

    $id('button-change').title = lang.file.tips['purview-sync-change'];
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
        url: '/api/drive/purview/data-json?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.file.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('share').checked = jsonData.ds_share == 1 ? true : false;
                $id('sync').checked = jsonData.ds_sync == 1 ? true : false;

                if (jsonData.ds_share == 0) {
                    openShare();
                }
            }

            fastui.list.dataLoad('/api/drive/purview/list-json');

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 开启共享(生成按钮)
function openShare() {
    fastui.coverShow('page-cover');

    var html = '<span class="button" onClick="openShareOk();">' + lang.file.button['open-share'] + '</span>';
    var layer = $create({
        tag: 'div', 
        attr: {id: 'open-share'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil(($page().client.height - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil(($page().client.width - layer.offsetWidth) / 2) + 'px';
}


// 开启共享(提交)
function openShareOk() {
    var id = $query('id');
    var data = 'id=' + id + '&share=true';

    $ajax({
        type: 'POST', 
        url: '/api/drive/purview/share', 
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
    var id = jsonData.items[index].ds_fileid;

    if (jsonData.items[index].ds_departmentid.length > 0) {
        var type = 'department';
        var typeId = jsonData.items[index].ds_departmentid;
        var typeName = purviewDepartment(typeId);
    } else if (jsonData.items[index].ds_roleid > 0) {
        var type = 'role';
        var typeId = jsonData.items[index].ds_roleid;
        var typeName = purviewRole(typeId);
    } else if (jsonData.items[index].ds_userid > 0) {
        var type = 'user';
        var typeId = jsonData.items[index].ds_userid;
        var typeName = purviewUser(typeId);
    }

    fastui.dialogConfirm(lang.user.tips.confirm['delete'] + '<br />' + typeName, 'dataListItemDeleteOk(' + id + ', \'' + type + '\', \'' + typeId + '\');');
}


// 数据列表项目删除(提交)
function dataListItemDeleteOk(id, type, typeId) {
    var data = 'id=' + id + '&type=' + type + '&typeid=' + typeId;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/purview/delete', 
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
                if (jsonData.items[i].ds_departmentid.length > 0) {
                    var type = 'department';
                    var typeId = jsonData.items[i].ds_departmentid;
                } else if (jsonData.items[i].ds_roleid > 0) {
                    var type = 'role';
                    var typeId = jsonData.items[i].ds_roleid;
                } else if (jsonData.items[i].ds_userid > 0) {
                    var type = 'user';
                    var typeId = jsonData.items[i].ds_userid;
                }

                return '<div id="purview-select_' + i + '" onClick="purviewModify(\'' + type + '\', \'' + typeId + '\', ' + i + ');"><input name="purview_' + i + '" type="hidden" id="purview_' + i + '" value="' + jsonData.items[i].ds_purview + '" /></div>';
            });

        fastui.radioSelect('purview-select_' + i, [{value:'viewer',name:'' + lang.file.data.purview['viewer'] + ''},{value:'downloader',name:'' + lang.file.data.purview['downloader'] + ''},{value:'uploader',name:'' + lang.file.data.purview['uploader'] + ''},{value:'editor',name:'' + lang.file.data.purview['editor'] + ''},{value:'manager',name:'' + lang.file.data.purview['manager'] + ''}], 'value', 'name', '#2196F3');

        // 操作
        row.addCell(
            {width: '24'}, 
            function() {
                if (jsonData.items[i].ds_departmentid.length > 0) {
                    return '<span class="delete" onClick="dataListItemDelete(' + i + ', \'department\');">✕</span>';
                } else if (jsonData.items[i].ds_roleid > 0) {
                    return '<span class="delete" onClick="dataListItemDelete(' + i + ', \'role\');">✕</span>';
                } else if (jsonData.items[i].ds_userid > 0) {
                    return '<span class="delete" onClick="dataListItemDelete(' + i + ', \'user\');">✕</span>';
                } else {
                    return '';
                }
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


// 权限选择(弹出选择窗口)
function purviewSelect(type, text) {
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

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.file.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    data += 'type=' + type + '&';

    for (var i = 0; i < items.length; i++) {
        data += 'typeid=' + items[i] + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/purview/add', 
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
    var purview = $id('purview_' + index).value;
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
        url: '/api/drive/purview/modify', 
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
        url: '/api/drive/purview/share', 
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
        url: '/api/drive/purview/sync', 
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
        url: '/api/drive/purview/change', 
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