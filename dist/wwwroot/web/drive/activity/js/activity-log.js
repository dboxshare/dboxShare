// 页面初始化事件调用函数
function init() {
    dataListPageInit();

    fastui.list.scrollDataLoad('/api/drive/activity/log-list-json');
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
            '<img src="/ui/images/datalist-log-icon.png" width="32" />'
            );

        // 操作
        row.addCell(
            null, 
            function() {
                if (jsonData.items[i].ds_fileversion == 0) {
                    return '' + logAction(jsonData.items[i].ds_action) + '';
                } else {
                    return '' + logAction(jsonData.items[i].ds_action) + '&nbsp;&nbsp;<span class="version">v' + jsonData.items[i].ds_fileversion + '</span>';
                }
            });

        // 用户
        row.addCell(
            {width: '150'}, 
            '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/account/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.items[i].ds_username)) + '\', 500, 500);">' + jsonData.items[i].ds_username + '</a>'
            );

        // 时间
        row.addCell(
            {width: '150'}, 
            function() {
                var html = '';

                html += '' + logTime(jsonData.items[i].ds_time) + '';
                html += '<br />';
                html += '' + jsonData.items[i].ds_time + '';

                return html;
            });

        row.addCell({width: '20'}, '');
    }
}


// 获取日志操作
function logAction(action) {
    var actions = action.split('-');

    if (actions[1] != undefined) {
        for (var i in lang.action) {
            if (i == actions[1]) {
                return lang.action[i];
            }
        }
    }

    return '';
}


// 获取日志时间
function logTime(time) {
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