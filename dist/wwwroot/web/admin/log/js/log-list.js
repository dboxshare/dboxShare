// 页面初始化事件调用函数
function init() {
    dataListPageInit();
    dataListPageView();
    dataListFilter();

    fastui.list.scrollDataLoad('/api/admin/log/list-json');
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
    var menus = $id('page-menu').$class('subitem');
    var time = $query('time');

    if ($query('timestart').length > 0 && $query('timeend').length > 0) {
        return;
    }

    if (time.length == 0) {
        menus[0].className = 'subitem-current';
        return;
    }

    for (var i = 0; i < menus.length; i++) {
        if (menus[i].$get('onClick').toString().indexOf(time) > -1) {
            menus[i].className = 'subitem-current';
            break;
        }
    }
}


// 数据列表筛选
function dataListFilter() {
    var action = $query('action');
    var folders = '';
    var files = '';

    for (var i in lang.action) {
        if (/^(add|rename|remark|link|lock|unlock|share|unshare|sync|unsync|move|remove|restore|delete)$/.test(i) == true) {
            if ('folder-' + i == action) {
                folders += '<li class="item-current">' + lang.action[i] + '</li>';
            } else {
                folders += '<li onClick="$location(\'action\', \'folder-' + i + '\');">' + lang.action[i] + '</li>';
            }
        }
    }

    $id('filter-folder').innerHTML += '<ul>' + folders + '</ul>';

    for (var j in lang.action) {
        if (/^(upload|download|view|rename|remark|link|lock|unlock|copy|move|remove|restore|delete|upversion|replace)$/.test(j) == true) {
            if ('file-' + j == action) {
                files += '<li class="item-current">' + lang.action[j] + '</li>';
            } else {
                files += '<li onClick="$location(\'action\', \'file-' + j + '\');">' + lang.action[j] + '</li>';
            }
        }
    }

    $id('filter-file').innerHTML += '<ul>' + files + '</ul>';
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

        // 文件夹 / 文件
        row.addCell(
            null, 
            '' + logData(jsonData.items[i].ds_fileid, jsonData.items[i].ds_filename, jsonData.items[i].ds_fileextension, jsonData.items[i].ds_fileversion, jsonData.items[i].ds_isfolder, jsonData.items[i].ds_action) + ''
            );

        // 操作
        row.addCell(
            {width: '250'}, 
            function() {
                if (jsonData.items[i].ds_isfolder == 1) {
                    return '' + lang.log['folder'] + ' ' + logAction(jsonData.items[i].ds_action) + '';
                } else {
                    return '' + lang.log['file'] + ' ' + logAction(jsonData.items[i].ds_action) + '';
                }
            });

        // 用户
        row.addCell(
            {width: '150'}, 
            function() {
                var html = '';

                html += '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/account/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.items[i].ds_username)) + '\', 500, 500);">' + jsonData.items[i].ds_username + '</a>';
                html += '<br />';
                html += '' + jsonData.items[i].ds_ip + '';

                return html;
            });

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


// 日志数据导出
function logDataExport() {
    var iframe = $id('log-data-export-iframe');

    if (iframe == null) {
        iframe = $create({
            tag: 'iframe', 
            attr: {
                id: 'log-data-export-iframe', 
                name: 'log-data-export-iframe', 
                src: '/api/admin/log/export' + window.location.search, 
                width: '0', 
                height: '0', 
                frameBorder: '0'
                }
            }).$add(document.body);
    } else {
        iframe.src = $url('/api/admin/log/export' + window.location.search);
    }
}