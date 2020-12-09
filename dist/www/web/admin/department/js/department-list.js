var level = 1;


$include('/storage/data/department-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataListPageInit();
    dataListLocation();

    fastui.list.dataLoad('/web/admin/department/department-list-json.ashx');
}


// 页面调整事件调用函数
function resize() {
    dataListPageInit();
}


// 数据列表页面初始化
function dataListPageInit() {
    // 列表页面布局调整
    fastui.list.layoutResize();

    if (typeof(jsonDataDepartment) == 'undefined') {
        $id('location').style.display = 'none';
    }
}


// 数据列表路径
function dataListLocation() {
    try {
        var id = $query('departmentid') || 0;
        var path = $id('location').$class('path')[0];
        var departmentId;
        var name;
        var location;
        var i;

        if (id == 0) {
            path.innerHTML += lang.department['all'];
        } else {
            path.innerHTML += '<a href=\"/web/admin/department/department-list.html\">' + lang.department['all'] + '</a>';
        }

        if (id == 0) {
            return;
        }

        for (i = 0; i < jsonDataDepartment.length; i++) {
            if (jsonDataDepartment[i].dbs_id == id) {
                departmentId = jsonDataDepartment[i].dbs_departmentid;
                name = jsonDataDepartment[i].dbs_name;
                level++;
                break;
            }
        }

        location = ' / ' + name;

        while (departmentId > 0) {
            for (i = 0; i < jsonDataDepartment.length; i++) {
                if (jsonDataDepartment[i].dbs_id == departmentId) {
                    id = jsonDataDepartment[i].dbs_id;
                    departmentId = jsonDataDepartment[i].dbs_departmentid;
                    name = jsonDataDepartment[i].dbs_name;
                    level++;
                    break;
                }
            }

            location = ' / <a href=\"javascript: $location(\'departmentid\', ' + id + ');\">' + name + '</a>' + location;
        }

        path.innerHTML += location;
    } catch(e) {}
}


// 数据列表项目删除(询问)
function dataListItemDelete(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;

    fastui.dialogConfirm(lang.department.tips.confirm['delete'] + '<br />' + name, 'dataListItemDeleteOk(' + id + ')');
}


// 数据列表项目删除(提交)
function dataListItemDeleteOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/admin/department/department-action.ashx?action=delete', 
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
    var row;
    var i;

    for (i = 0; i < jsonData.items.length; i++) {
        row = list.addRow(i);

        // 图标
        row.addCell(
            {width: 32}, 
            '<img src=\"/ui/images/datalist-department-icon.png\" width=\"32\" />'
            );

        // 名称
        row.addCell(
            null, 
            function() {
                var html = '';

                // 分类层级限制
                if (level < 5) {
                    html += '<a href=\"javascript: $location(\'departmentid\', ' + jsonData.items[i].dbs_id + ');\">' + jsonData.items[i].dbs_name + '</a>';
                } else {
                    html += '' + jsonData.items[i].dbs_name + '';
                }

                html += '<input name=\"id\" type=\"hidden\" value=\"' + jsonData.items[i].dbs_id + '\" />';

                return html;
            });

        // 排序
        row.addCell(
            {width: 50}, 
            function() {
                var html = '';

                html += '<img src=\"/ui/images/datalist-move-up-icon.png\" width=\"12\" onClick=\"dataListSortMoveUp(this);\" title=\"' + lang.department.button['move-up'] + '\" class=\"pointer\" />';
                html += '&nbsp;&nbsp;&nbsp;&nbsp;';
                html += '<img src=\"/ui/images/datalist-move-down-icon.png\" width=\"12\" onClick=\"dataListSortMoveDown(this);\" title=\"' + lang.department.button['move-down'] + '\" class=\"pointer\" />';

                return html;
            });

        // 操作
        row.addCell(
            {width: 150}, 
            function() {
                var html = '';

                html += '<div class=\"datalist-action\">';
                html += '<span class=\"button\" onClick=\"fastui.windowPopup(\'department-modify\', \'' + lang.department.button['modify'] + ' - ' + jsonData.items[i].dbs_name + '\', \'/web/admin/department/department-modify.html?id=' + jsonData.items[i].dbs_id + '\', 800, 250);\">' + lang.department.button['modify'] + '</span>';
                html += '<span class=\"button\" onClick=\"dataListItemDelete(' + i + ');\">' + lang.department.button['delete'] + '</span>';
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
    var i;

    for (i = 0; i < list.rows.length; i++) {
        data += 'id=' + items[i].value + '&';
    }

    data = data.substring(0, data.length - 1);

    $ajax({
        type: 'POST', 
        url: '/web/admin/department/department-action.ashx?action=sort', 
        async: true, 
        data: data
        });
}