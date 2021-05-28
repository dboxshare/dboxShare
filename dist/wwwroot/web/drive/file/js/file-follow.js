$include('/storage/data/file-type-json.js');


// 页面初始化事件调用函数
function init() {
    dataListPageInit();

    fastui.list.scrollDataLoad('/api/drive/file/follow-list-json');
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

    if (typeof(jsonData) == 'undefined') {
        return;
    }

    var rows = list.rows.length;

    for (var i = rows; i < jsonData.items.length; i++) {
        var row = list.addRow(i);

        (function(index) {row.ondblclick = function() {
            fileView(index, 0, 0);
            };})(i);

        // 图标
        row.addCell(
            {width: '32'}, 
            function() {
                var html = '';

                html += '<div class="list-icon">';
                html += '<span class="image"><img src="' + fileIcon(jsonData.items[i].ds_extension) + '" width="32" /></span>';

                if (jsonData.items[i].ds_share == 1) {
                    html += '<span class="share"><img src="/ui/images/datalist-drive-share-icon.png" width="14" /></span>';
                }

                if (jsonData.items[i].ds_lock == 1) {
                    html += '<span class="lock"><img src="/ui/images/datalist-drive-lock-icon.png" width="10" /></span>';
                }

                html += '</div>';

                return html;
            });

        // 名称
        row.addCell(
            null, 
            function() {
                var html = '';

                html += '<a href="javascript: fileView(' + i + ', 0, 0);" title="' + jsonData.items[i].ds_name + '' + jsonData.items[i].ds_extension + '">' + jsonData.items[i].ds_name + '' + jsonData.items[i].ds_extension + '</a>';
                html += '&nbsp;&nbsp;';
                html += '<span class="version">v' + jsonData.items[i].ds_version + '</span>';
                html += '<input name="id" type="hidden" value="' + jsonData.items[i].ds_id + '" />';

                return html;
            });

        // 大小
        row.addCell(
            {width: '100'}, 
            '' + fileSize(jsonData.items[i].ds_size) + ''
            );

        // 类型
        row.addCell(
            {width: '100'}, 
            '' + fileType(jsonData.items[i].ds_extension) + ''
            );

        // 更新时间
        row.addCell(
            {width: '250'}, 
            function() {
                var html = '';

                html += '' + jsonData.items[i].ds_updatetime + '';
                html += '<br />';
                html += '' + fileEvent('update', jsonData.items[i].ds_updateusername, jsonData.items[i].ds_updatetime) + '';

                return html;
            });

        // 操作
        row.addCell(
            {width: '50'}, 
            function() {
                var html = '';

                html += '<div class="datalist-action">';
                html += '<span class="button" onClick="dataListContextMenuClickEvent(' + i + ', event);">﹀</span>';
                html += '</div>';

                return html;
            });
    }

    // 数据列表事件绑定(右击弹出菜单)
    dataListContextMenuEvent(rows);
}