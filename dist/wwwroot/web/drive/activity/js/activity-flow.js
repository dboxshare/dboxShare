// 页面初始化事件调用函数
function init() {
    dataListPageInit();
    dataListPageView();

    fastui.list.scrollDataLoad('/api/drive/activity/flow-list-json');
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
    var menus = $id('page-menu').$class('item');
    var type = $query('type');

    if (type.length == 0) {
        menus[0].className = 'item-current';
        return;
    }

    for (var i = 0; i < menus.length; i++) {
        if (menus[i].$get('onClick').toString().indexOf(type) > -1) {
            menus[i].className = 'item-current';
            break;
        }
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

        // 用户
        row.addCell(
            {width: '150', align: 'center'}, 
            function() {
                var html = '';

                html += '<div class="avatar"><img src="/ui/images/avatar-icon.png" width="32" /></div>';
                html += '<div class="username"><a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/account/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.items[i].ds_username)) + '\', 500, 500);">' + jsonData.items[i].ds_username + '</a></div>';

                return html;
            });

        // 信息
        row.addCell(
            null, 
            function() {
                var html = '';

                html += '<div class="time">' + logTime(jsonData.items[i].ds_time) + '</div>';
                html += '<div class="box">';
                html += '<div class="filename">' + logData(jsonData.items[i].ds_fileid, jsonData.items[i].ds_filename, jsonData.items[i].ds_fileextension, jsonData.items[i].ds_fileversion, jsonData.items[i].ds_isfolder, jsonData.items[i].ds_action) + '</div>';
                html += '<div class="action">' + logAction(jsonData.items[i].ds_action) + '</div>';
                html += '</div>';

                return html;
            });
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


// 获取日志数据
function logData(id, name, extension, version, isFolder, action) {
    if (isFolder == 1) {
        if (/^(folder|file)-delete$/.test(action) == false) {
            return '<a href="javascript: fastui.windowPopup(\'folder-detail\', \'' + lang.file.context['detail'] + ' - ' + name + '\', \'/web/drive/folder/folder-detail.html?id=' + id + '\', 800, 500);" title="' + name + '">' + name + '</a>';
        } else {
            return '<label title="' + name + '">' + name + '</label>';
        }
    } else {
        if (/^(folder|file)-delete$/.test(action) == false) {
            return '<a href="javascript: fastui.windowPopup(\'file-detail\', \'' + lang.file.context['detail'] + ' - ' + name + '' + extension + '\', \'/web/drive/file/file-detail.html?id=' + id + '\', 800, 500);" title="' + name + '' + extension + '">' + name + '' + extension + '</a>&nbsp;&nbsp;<span class="version">v' + version + '</span>';
        } else {
            return '<label title="' + name + '' + extension + '">' + name + '' + extension + '</label>&nbsp;&nbsp;<span class="version">v' + version + '</span>';
        }
    }
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
        return '0 ' + lang.timespan['second'] + ' ' + time;
    } else if (Math.floor(ms / second) < 60) {
        return Math.floor(ms / second) + ' ' + lang.timespan['second'] + ' ' + time;
    } else if (Math.floor(ms / minute) < 60) {
        return Math.floor(ms / minute) + ' ' + lang.timespan['minute'] + ' ' + time;
    } else if (Math.floor(ms / hour) < 24) {
        return Math.floor(ms / hour) + ' ' + lang.timespan['hour'] + ' ' + time;
    } else if (Math.floor(ms / day) < 30) {
        return Math.floor(ms / day) + ' ' + lang.timespan['day'] + ' ' + time;
    } else if (Math.floor(ms / month) < 12) {
        return Math.floor(ms / month) + ' ' + lang.timespan['month'] + ' ' + time;
    } else if (Math.floor(ms / year) > 0) {
        return Math.floor(ms / year) + ' ' + lang.timespan['year'] + ' ' + time;
    }
}