var purview = 'viewer';
var lock = 0;
var recycle = 0;
var selectable = true;


$include('/storage/data/file-type-json.js');


// 页面初始化事件调用函数
function init() {
    dataListPageInit();
    dataListPageView();
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


// 数据列表页面视图
function dataListPageView() {
    var id = $query('id') || 0;

    if ($query('myself') == 'true') {
        $id('myself').checked = true;
    }

    $id('button-list').style.display = 'none';

    // 获取文件及用户权限信息
    $ajax({
        type: 'GET', 
        url: '/web/drive/file/list-attribute-json.ashx?id=' + id + '&folder=false', 
        async: true, 
        callback: function(data) {
            var removed = false;

            if (data.length == 0) {
                removed = true;
            } else {
                window.eval('var jsonDataAttribute = ' + data + ';');

                purview = jsonDataAttribute.purview;
                lock = jsonDataAttribute.lock;
                recycle = jsonDataAttribute.recycle;
            }

            if (purviewCheck(purview, 'editor') == false || lock == 1) {
                $id('button-upversion').style.display = 'none';
            }

            if (purviewCheck(purview, 'manager') == false || lock == 1) {
                $id('button-remove').style.display = 'none';

                selectable = false;
            }

            if (recycle == 1 || removed == true) {
                fastui.coverTips(lang.file.tips['current-file-removed']);
                return;
            }

            $id('button-list').style.display = 'block';

            fastui.list.scrollDataLoad('/web/drive/file/file-version-list-json.ashx');
            }
        });
}


// 数据列表项目替换(询问)
function dataListItemReplace(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['replace'] + '<br />' + name + extension, 'dataListItemReplaceOk(' + id + ')');
}


// 数据列表项目替换(提交)
function dataListItemReplaceOk(id) {
    var data = 'id=' + $query('id') + '&versionid=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/file-action.ashx?action=replace', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.list.scrollDataLoad(window.parent.fastui.list.path, true);

                fastui.list.scrollDataLoad(fastui.list.path, true);

                fastui.iconTips('tick');
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.iconTips('cross');
            }
            }
        });
}


// 数据列表项目移除(询问)
function dataListItemRemove(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['remove'] + '<br />' + name + extension, 'dataListItemRemoveOk(' + id + ')');
}


// 数据列表项目移除(提交)
function dataListItemRemoveOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/file-action.ashx?action=remove', 
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
        fastui.textTips(lang.file.tips['please-select-item']);
        return;
    }

    switch(action) {
        case 'removeall':
        tips = lang.file.tips.confirm['remove-all'];
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
            url: '/web/drive/file/file-action.ashx?action=' + action, 
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
    } else if (data == 'no-permission') {
        fastui.textTips(lang.file.tips['no-permission']);
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

    j = list.rows.length;

    for (i = j; i < jsonData.items.length; i++) {
        row = list.addRow(i);

        (function(index) {row.ondblclick = function() {
            fileView(index, -64, -64);
            };})(i);

        // 复选框
        row.addCell(
            {width: '20', align: 'center'}, 
            function() {
                if (selectable == true) {
                    return '<input name=\"id\" type=\"checkbox\" class=\"checkbox\" value=\"' + jsonData.items[i].dbs_id + '\" />';
                } else {
                    return '<input name=\"id\" type=\"checkbox\" class=\"checkbox\" value=\"' + jsonData.items[i].dbs_id + '\" disabled=\"disabled\" />';
                }
            });

        // 图标
        row.addCell(
            {width: '32'}, 
            '<img src=\"' + fileIcon(jsonData.items[i].dbs_extension) + '\" width=\"32\" />'
            );

        // 名称
        row.addCell(
            null, 
            function() {
                var html = '';

                html += '<a href=\"javascript: fileView(' + i + ', 0, 0);\">' + lang.file['version'] + ' #' + jsonData.items[i].dbs_version + '</a>';

                if (jsonData.items[i].dbs_remark.length == 0) {
                    html += '<br /><font color=\"#757575\">' + lang.file['no-remark'] + '</font>';
                } else {
                    html += '<br /><font color=\"#757575\">' + jsonData.items[i].dbs_remark + '</font>';
                }

                return html;
            });

        // 上传时间
        row.addCell(
            {width: '250'}, 
            function() {
                var html = '';

                html += '' + jsonData.items[i].dbs_updatetime + '';
                html += '<br />';
                html += '' + fileEvent('upload', jsonData.items[i].dbs_updateusername, jsonData.items[i].dbs_updatetime) + '';

                return html;
            });

        // 操作
        row.addCell(
            {width: '150'}, 
            function() {
                var html = '';

                html += '<div class=\"datalist-action\">';

                if (jsonData.items[i].dbs_updateusername == window.top.myUsername && jsonData.items[i].dbs_lock == 0) {
                    html += '<span class=\"button\" onClick=\"dataListItemRemove(' + i + ');\">' + lang.file.button['remove'] + '</span>';
                }

                html += '<span class=\"button\" onClick=\"dataListContextMenuClickEvent(' + i + ', event);\">﹀</span>';
                html += '</div>';

                return html;
            });

        if (i - j == fastui.list.size) {
            break;
        }
    }

    // 数据列表事件绑定(右击弹出菜单)
    dataListContextMenuEvent(j);

    if (purviewCheck(purview, 'manager') == true) {
        // 数据列表事件绑定(点击选择项目)
        fastui.list.bindEvent(j);
    }

    // 数据列表分块加载(应用于数据重载)
    fastui.list.scrollLoadBlock(field, reverse, primer, i);
}