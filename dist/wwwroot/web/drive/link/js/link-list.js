// 页面初始化事件调用函数
function init() {
    dataListPageInit();

    fastui.list.scrollDataLoad('/api/drive/link/list-json');
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


// 数据列表项目撤消(询问)
function dataListItemRevoke(index) {
    var id = jsonData.items[index].ds_id;
    var title = jsonData.items[index].ds_title;

    fastui.dialogConfirm(lang.link.tips.confirm['revoke'] + '<br />' + title, 'dataListItemRevokeOk(' + id + ')');
}


// 数据列表项目撤消(提交)
function dataListItemRevokeOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/link/revoke', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
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

    var rows = list.rows.length;

    for (var i = rows; i < jsonData.items.length; i++) {
        var row = list.addRow(i);

        // 图标
        row.addCell(
            {width: '32'}, 
            '<img src="/ui/images/datalist-link-icon.png" width="32" />'
            );

        // 标题
        row.addCell(
            null, 
            '<a href="javascript: fastui.windowPopup(\'link-detail\', \'' + lang.file.context['link'] + ' - ' + jsonData.items[i].ds_title + '\', \'/web/drive/link/link-detail.html?id=' + jsonData.items[i].ds_id + '\', 800, 500);" title="' + jsonData.items[i].ds_title + '">' + jsonData.items[i].ds_title + '</a>'
            );

        // 状态
        row.addCell(
            {width: '100'}, 
            function() {
                if (jsonData.items[i].ds_revoke == 0) {
                    if (new Date(jsonData.items[i].ds_deadline.replace(/\-/g, '/')).getTime() > new Date().getTime()) {
                        return '' + lang.link.data.status['sharing'] + '';
                    } else {
                        return '' + lang.link.data.status['expired'] + '';
                    }
                } else {
                    return '' + lang.link.data.status['revoked'] + '';
                }
            });

        // 查看次数
        row.addCell(
            {width: '100'}, 
            '' + jsonData.items[i].ds_count + ''
            );

        // 截止时间
        row.addCell(
            {width: '200'}, 
            '' + jsonData.items[i].ds_deadline + ''
            );

        // 发布时间
        row.addCell(
            {width: '200'}, 
            function() {
                var html = '';

                html += '' + postTime(jsonData.items[i].ds_time) + '';
                html += '<br />';
                html += '' + jsonData.items[i].ds_time + '';

                return html;
            });

        // 操作
        row.addCell(
            {width: '150'}, 
            function() {
                var html = '';

                html += '<div class="datalist-action">';
                html += '<span class="button" onClick="fastui.windowPopup(\'link-log\', \'' + lang.link.button['log'] + ' - ' + jsonData.items[i].ds_title + '\', \'/web/drive/link/link-log.html?id=' + jsonData.items[i].ds_id + '\', 800, 500);">' + lang.link.button['log'] + '</span>';

                if (jsonData.items[i].ds_revoke == 0) {
                    if (new Date(jsonData.items[i].ds_deadline.replace(/\-/g, '/')).getTime() > new Date().getTime()) {
                        html += '<span class="button" onClick="dataListItemRevoke(' + i + ');">' + lang.link.button['revoke'] + '</span>';
                    }
                }

                html += '</div>';

                return html;
            });
    }
}


// 获取发布时间
function postTime(time) {
    var ms = new Date().getTime() - new Date(time.replace(/\-/g, '/'));
    var second = 1000;
    var minute = second * 60;
    var hour = minute * 60;
    var day = hour * 24;
    var month = day * 30;
    var year = day * 365;

    if (Math.floor(ms / second) == 0) {
        return '0 ' + lang.timespan['second'];
    } else if (Math.floor(ms / second) < 60) {
        return Math.floor(ms / second) + ' ' + lang.timespan['second'];
    } else if (Math.floor(ms / minute) < 60) {
        return Math.floor(ms / minute) + ' ' + lang.timespan['minute'];
    } else if (Math.floor(ms / hour) < 24) {
        return Math.floor(ms / hour) + ' ' + lang.timespan['hour'];
    } else if (Math.floor(ms / day) < 30) {
        return Math.floor(ms / day) + ' ' + lang.timespan['day'];
    } else if (Math.floor(ms / month) < 12) {
        return Math.floor(ms / month) + ' ' + lang.timespan['month'];
    } else if (Math.floor(ms / year) > 0) {
        return Math.floor(ms / year) + ' ' + lang.timespan['year'];
    }
}