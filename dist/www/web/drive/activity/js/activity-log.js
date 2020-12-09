// 页面初始化事件调用函数
function init() {
    dataListPageInit();

    fastui.list.scrollDataLoad('/web/drive/activity/activity-log-list-json.ashx');
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
    var row;
    var i;
    var j;

    if (typeof(jsonData) == 'undefined') {
        return;
    }

    j = list.rows.length;

    for (i = j; i < jsonData.items.length; i++) {
        row = list.addRow(i);

        // 图标
        row.addCell(
            {width: '32'}, 
            '<img src=\"/ui/images/datalist-log-icon.png\" width=\"32\" />'
            );

        // 操作
        row.addCell(
            null, 
            function() {
                if (jsonData.items[i].dbs_fileversion == 0) {
                    return '' + logAction(jsonData.items[i].dbs_action) + '';
                } else {
                    return '' + logAction(jsonData.items[i].dbs_action) + '&nbsp;&nbsp;<span class=\"version\">v' + jsonData.items[i].dbs_fileversion + '</span>';
                }
            });

        // 用户
        row.addCell(
            {width: '150'}, 
            '<a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonData.items[i].dbs_username)) + '\', 500, 500);\">' + jsonData.items[i].dbs_username + '</a>'
            );

        // 时间
        row.addCell(
            {width: '150'}, 
            '' + logTime(jsonData.items[i].dbs_time) + ''
            );

        row.addCell({width: '20'}, '');

        if (i - j == fastui.list.size) {
            break;
        }
    }
}


// 获取日志操作信息
function logAction(action) {
    var actions = new Array();
    var operation = '';
    var i;

    actions = action.split('-');

    if (actions[1] != undefined) {
        for (i in lang.action) {
            if (i == actions[1]) {
                operation = lang.action[i];
                break;
            }
        }
    }

    return operation;
}


// 获取日志时间信息
function logTime(time) {
    var ms = new Date().getTime() - new Date(time.replace(/\-/g, '/'));
    var second = 1000;
    var minute = second * 60;
    var hour = minute * 60;
    var day = hour * 24;
    var month = day * 30;
    var year = day * 365;

    if (Math.floor(ms / second) == 0) {
        return '0 ' + lang.timespan['second'] + '<br />' + time;
    } else if (Math.floor(ms / second) < 60) {
        return Math.floor(ms / second) + ' ' + lang.timespan['second'] + '<br />' + time;
    } else if (Math.floor(ms / minute) < 60) {
        return Math.floor(ms / minute) + ' ' + lang.timespan['minute'] + '<br />' + time;
    } else if (Math.floor(ms / hour) < 24) {
        return Math.floor(ms / hour) + ' ' + lang.timespan['hour'] + '<br />' + time;
    } else if (Math.floor(ms / day) < 30) {
        return Math.floor(ms / day) + ' ' + lang.timespan['day'] + '<br />' + time;
    } else if (Math.floor(ms / month) < 12) {
        return Math.floor(ms / month) + ' ' + lang.timespan['month'] + ' ' + time;
    } else if (Math.floor(ms / year) > 0) {
        return Math.floor(ms / year) + ' ' + lang.timespan['year'] + '<br />' + time;
    }
}