// 页面初始化事件调用函数
function init() {
    dataListPageInit();

    fastui.list.dataLoad('/api/admin/role/list-json');
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


// 数据列表项目删除(询问)
function dataListItemDelete(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    fastui.dialogConfirm(lang.role.tips.confirm['delete'] + '<br />' + name, 'dataListItemDeleteOk(' + id + ')');
}


// 数据列表项目删除(提交)
function dataListItemDeleteOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/role/delete', 
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
            '<img src="/ui/images/datalist-role-icon.png" width="32" />'
            );

        // 名称
        row.addCell(
            null, 
            function() {
                var html = '';

                html += '' + jsonData.items[i].ds_name + '';
                html += '<input name="id" type="hidden" value="' + jsonData.items[i].ds_id + '" />';

                return html;
            });

        // 排序
        row.addCell(
            {width: '50'}, 
            function() {
                var html = '';

                html += '<img src="/ui/images/datalist-move-up-icon.png" width="12" onClick="dataListSortMoveUp(this);" title="' + lang.role.button['move-up'] + '" class="pointer" />';
                html += '&nbsp;&nbsp;&nbsp;&nbsp;';
                html += '<img src="/ui/images/datalist-move-down-icon.png" width="12" onClick="dataListSortMoveDown(this);" title="' + lang.role.button['move-down'] + '" class="pointer" />';

                return html;
            });

        // 操作
        row.addCell(
            {width: '150'}, 
            function() {
                var html = '';

                html += '<div class="datalist-action">';
                html += '<span class="button" onClick="fastui.windowPopup(\'role-modify\', \'' + lang.role.button['modify'] + ' - ' + jsonData.items[i].ds_name + '\', \'/web/admin/role/role-modify.html?id=' + jsonData.items[i].ds_id + '\', 800, 250);">' + lang.role.button['modify'] + '</span>';
                html += '<span class="button" onClick="dataListItemDelete(' + i + ');">' + lang.role.button['delete'] + '</span>';
                html += '</div>';

                return html;
            });
    }
}


// 数据列表排序(上移)
function dataListSortMoveUp(element) {
    var list = $id('data-list');
    var index = element.parentNode.parentNode.rowIndex;

    if (index > 0) {
        list.rows[index - 1].parentNode.insertBefore(list.rows[index], list.rows[index - 1]);

        dataListSortSave();
    }
}


// 数据列表排序(下移)
function dataListSortMoveDown(element) {
    var list = $id('data-list');
    var index = element.parentNode.parentNode.rowIndex;

    if (index < (list.rows.length - 1)) {
        list.rows[index].parentNode.insertBefore(list.rows[index + 1], list.rows[index]);

        dataListSortSave();
    }
}


// 数据列表排序(保存)
function dataListSortSave() {
    var list = $id('data-list');
    var items = $name('id');
    var data = '';

    for (var i = 0; i < list.rows.length; i++) {
        data += 'id=' + items[i].value + '&';
    }

    data = data.substring(0, data.length - 1);

    $ajax({
        type: 'POST', 
        url: '/api/admin/role/sort', 
        async: true, 
        data: data
        });
}