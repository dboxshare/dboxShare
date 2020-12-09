// 页面初始化事件调用函数
function init() {
    dataListPageInit();
    dataListPageView();
    dataListFilter();

    fastui.list.scrollDataLoad('/web/admin/log/log-list-json.ashx');
}


// 页面调整事件调用函数
function resize() {
    dataListPageInit();
}


// 数据列表页面初始化
function dataListPageInit() {
    var keyword = $query('keyword');
    var timestart = $query('timestart');
    var timeend = $query('timeend');

    // 列表页面布局调整
    fastui.list.layoutResize();

    if (keyword.length == 0) {
        fastui.valueTips('keyword', lang.log.tips.value['keyword']);
    } else {
        $id('keyword').value = keyword;
    }

    if (timestart.length == 0) {
        $id('timestart').value = $formatTime('yyyy-MM-dd HH:mm', new Date(new Date().getTime() - (24 * 60 * 60 * 1000)));
    } else {
        $id('timestart').value = timestart;
    }

    if (timeend.length == 0) {
        $id('timeend').value = $formatTime('yyyy-MM-dd HH:mm');
    } else {
        $id('timeend').value = timeend;
    }
}


// 数据列表页面视图
function dataListPageView() {
    var items = $id('page-menu').$class('subitem');
    var timestamp = $query('timestamp');
    var i;

    if ($query('timestart').length > 0 && $query('timeend').length > 0) {
        return;
    }

    if (timestamp.length == 0) {
        items[0].className = 'subitem-current';
        return;
    }

    for (i = 0; i < items.length; i++) {
        if (items[i].$get('onClick').toString().indexOf(timestamp) > -1) {
            items[i].className = 'subitem-current';
            break;
        }
    }
}


// 数据列表筛选
function dataListFilter() {
    var action = $query('action');
    var folders = '';
    var files = '';
    var i;

    for (i in lang.action) {
        if (/^(add|rename|remark|lock|unlock|share|unshare|sync|unsync|move|remove|restore|delete)$/.test(i) == true) {
            if ('folder-' + i == action) {
                folders += '<li class=\"item-current\">' + lang.action[i] + '</li>';
            } else {
                folders += '<li onClick=\"$location(\'action\', \'folder-' + i + '\');\">' + lang.action[i] + '</li>';
            }
        }
    }

    $id('filter-folder').innerHTML += '<ul>' + folders + '</ul>';

    for (i in lang.action) {
        if (/^(upload|download|view|rename|remark|lock|unlock|copy|move|remove|restore|delete|upversion|replace)$/.test(i) == true) {
            if ('file-' + i == action) {
                files += '<li class=\"item-current\">' + lang.action[i] + '</li>';
            } else {
                files += '<li onClick=\"$location(\'action\', \'file-' + i + '\');\">' + lang.action[i] + '</li>';
            }
        }
    }

    $id('filter-file').innerHTML += '<ul>' + files + '</ul>';
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
            {width: 32}, 
            '<img src=\"/ui/images/datalist-log-icon.png\" width=\"32\" />'
            );

        // 文件夹 / 文件
        row.addCell(
            null, 
            '' + logData(jsonData.items[i].dbs_fileid, jsonData.items[i].dbs_filename, jsonData.items[i].dbs_fileextension, jsonData.items[i].dbs_fileversion, jsonData.items[i].dbs_isfolder, jsonData.items[i].dbs_action) + ''
            );

        // 操作
        row.addCell(
            {width: 250}, 
            function() {
                if (jsonData.items[i].dbs_isfolder == 1) {
                    return '' + lang.log['folder'] + ' ' + logAction(jsonData.items[i].dbs_action) + '';
                } else {
                    return '' + lang.log['file'] + ' ' + logAction(jsonData.items[i].dbs_action) + '';
                }
            });

        // 用户
        row.addCell(
            {width: 150}, 
            function() {
                var html = '';

                html += '<a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonData.items[i].dbs_username)) + '\', 500, 500);\">' + jsonData.items[i].dbs_username + '</a>';
                html += '<br />';
                html += '' + jsonData.items[i].dbs_ip + '';

                return html;
            });

        // 时间
        row.addCell(
            {width: 150}, 
            '' + logTime(jsonData.items[i].dbs_time) + ''
            );

        row.addCell({width: 20}, '');

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


// 获取日志数据信息
function logData(id, name, extension, version, isFolder, action) {
    if (isFolder == 1) {
        if (/^(folder|file)-delete$/.test(action) == false) {
            return '<a href=\"javascript: fastui.windowPopup(\'folder-detail\', \'' + lang.file.context['detail'] + ' - ' + name + '\', \'/web/drive/file/folder-detail.html?id=' + id + '\', 800, 500);\" title=\"' + name + '\">' + name + '</a>';
        } else {
            return '<label title=\"' + name + '\">' + name + '</label>';
        }
    } else {
        if (/^(folder|file)-delete$/.test(action) == false) {
            return '<a href=\"javascript: fastui.windowPopup(\'file-detail\', \'' + lang.file.context['detail'] + ' - ' + name + '' + extension + '\', \'/web/drive/file/file-detail.html?id=' + id + '\', 800, 500);\" title=\"' + name + '' + extension + '\">' + name + '' + extension + '</a>&nbsp;&nbsp;<span class=\"version\">v' + version + '</span>';
        } else {
            return '<label title=\"' + name + '' + extension + '\">' + name + '' + extension + '</label>&nbsp;&nbsp;<span class=\"version\">v' + version + '</span>';
        }
    }
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


// 自定义时间查询
function customTime() {
    var timestart = $id('timestart').value;
    var timeend = $id('timeend').value;
    var param = '';

    if (fastui.testInput(true, 'timestart', /^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}$/) == false) {
        fastui.textTips(lang.log.tips.input['time-start']);
        return;
    } else {
        param += 'timestart=' + escape(timestart) + '&';
    }

    if (fastui.testInput(true, 'timeend', /^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}$/) == false) {
        fastui.textTips(lang.log.tips.input['time-end']);
        return;
    } else {
        param += 'timeend=' + escape(timeend) + '&';
    }

    param = param.substring(0, param.length - 1);

    $location('?' + param);
}