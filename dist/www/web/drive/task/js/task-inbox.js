// 页面初始化事件调用函数
function init() {
    dataListPageInit();
    dataListPageView();

    fastui.list.scrollDataLoad('/web/drive/task/task-inbox-list-json.ashx');
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
    var items = $id('page-menu').$class('subitem');
    var status = $query('status');
    var i;

    if (status.length == 0) {
        items[0].className = 'subitem-current';
        return;
    }

    for (i = 0; i < items.length; i++) {
        if (items[i].$get('onClick').toString().indexOf(status) > -1) {
            items[i].className = 'subitem-current';
            break;
        }
    }
}


// 数据列表视图
function dataListView(field, reverse, primer) {
    var container = $id('datalist-container');
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

        // 分派人
        row.addCell(
            {width: '150', align: 'center'}, 
            function() {
                var html = '';

                html += '<div class=\"avatar\"><img src=\"/ui/images/avatar-icon.png\" width=\"32\" /></div>';
                html += '<div class=\"username\"><a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonData.items[i].dbs_username)) + '\', 500, 500);\">' + jsonData.items[i].dbs_username + '</a></div>';

                return html;
            });

        // 信息
        row.addCell(
            null, 
            function() {
                var html = '';

                html += '<div class=\"time\">' + taskTime(jsonData.items[i].dbs_time) + '</div>';
                html += '<div class=\"box\">';
                html += '<div class=\"filename\">';

                if (jsonData.items[i].dbs_isfolder == 1) {
                    html += '<a href=\"javascript: fastui.windowPopup(\'folder-detail\', \'' + lang.file.context['detail'] + ' - ' + jsonData.items[i].dbs_filename + '\', \'/web/drive/file/folder-detail.html?id=' + jsonData.items[i].dbs_fileid + '\', 800, 500);\" title=\"' + jsonData.items[i].dbs_filename + '\">' + jsonData.items[i].dbs_filename + '</a>';
                } else {
                    html += '<a href=\"javascript: fastui.windowPopup(\'file-detail\', \'' + lang.file.context['detail'] + ' - ' + jsonData.items[i].dbs_filename + '' + jsonData.items[i].dbs_fileextension + '\', \'/web/drive/file/file-detail.html?id=' + jsonData.items[i].dbs_fileid + '\', 800, 500);\" title=\"' + jsonData.items[i].dbs_filename + '' + jsonData.items[i].dbs_fileextension + '\">' + jsonData.items[i].dbs_filename + '' + jsonData.items[i].dbs_fileextension + '</a>';
                }

                html += '</div>';
                html += '<div class=\"attribute\">';
                html += '<span class=\"level\">' + taskLevel(jsonData.items[i].dbs_level) + '</span>';
                html += '&nbsp;&nbsp;';
                html += '<span class=\"status\">' + taskStatus(jsonData.items[i].dbs_status) + '</span>';
                html += '</div>';

                if (jsonData.items[i].dbs_revoke == 0) {
                    if (new Date(jsonData.items[i].dbs_deadline.replace(/\-/g, '/')).getTime() > new Date().getTime()) {
                        html += '<span class=\"deadline\">' + lang.task['left-time'] + ' ' + taskDeadline(jsonData.items[i].dbs_deadline) + '</span>';

                        if (jsonData.items[i].dbs_status == 0) {
                            html += '<span class=\"button-dark\" onClick=\"fastui.windowPopup(\'task-inbox-detail\', \'' + lang.task.button['detail'] + ' - ' + jsonData.items[i].dbs_filename + '' + jsonData.items[i].dbs_fileextension + '\', \'/web/drive/task/task-inbox-detail.html?id=' + jsonData.items[i].dbs_id + '\', 800, 500);\">' + lang.task.button['pending'] + '</span>';
                        } else if (jsonData.items[i].dbs_status == 1) {
                            html += '<span class=\"button-dark\" onClick=\"fastui.windowPopup(\'task-inbox-detail\', \'' + lang.task.button['detail'] + ' - ' + jsonData.items[i].dbs_filename + '' + jsonData.items[i].dbs_fileextension + '\', \'/web/drive/task/task-inbox-detail.html?id=' + jsonData.items[i].dbs_id + '\', 800, 500);\">' + lang.task.button['processing'] + '</span>';
                        } else {
                            html += '<span class=\"button-light\" onClick=\"fastui.windowPopup(\'task-inbox-detail\', \'' + lang.task.button['detail'] + ' - ' + jsonData.items[i].dbs_filename + '' + jsonData.items[i].dbs_fileextension + '\', \'/web/drive/task/task-inbox-detail.html?id=' + jsonData.items[i].dbs_id + '\', 800, 500);\">' + lang.task.button['detail'] + '</span>';
                        }
                    } else {
                        html += '<span class=\"warning\">' + lang.task.data.status['expired'] + '</span>';
                        html += '<span class=\"button-light\" onClick=\"fastui.windowPopup(\'task-inbox-detail\', \'' + lang.task.button['detail'] + ' - ' + jsonData.items[i].dbs_filename + '' + jsonData.items[i].dbs_fileextension + '\', \'/web/drive/task/task-inbox-detail.html?id=' + jsonData.items[i].dbs_id + '\', 800, 500);\">' + lang.task.button['detail'] + '</span>';
                    }
                } else {
                    html += '<span class=\"warning\">' + lang.task.data.status['revoked'] + '</span>';
                    html += '<span class=\"button-light\" onClick=\"fastui.windowPopup(\'task-inbox-detail\', \'' + lang.task.button['detail'] + ' - ' + jsonData.items[i].dbs_filename + '' + jsonData.items[i].dbs_fileextension + '\', \'/web/drive/task/task-inbox-detail.html?id=' + jsonData.items[i].dbs_id + '\', 800, 500);\">' + lang.task.button['detail'] + '</span>';
                }

                html += '</div>';

                return html;
            });
    }
}


// 获取任务时间信息
function taskTime(time) {
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


// 获取任务级别信息
function taskLevel(level) {
    switch(level) {
        case '0':
        return lang.task.data.level['normal'];
        break;

        case '1':
        return lang.task.data.level['important'];
        break;

        case '2':
        return lang.task.data.level['urgent'];
        break;
    }
}


// 获取任务期限信息
function taskDeadline(deadline) {
    var ms = new Date(deadline.replace(/\-/g, '/')) - new Date().getTime();
    var second = 1000;
    var minute = second * 60;
    var hour = minute * 60;
    var day = hour * 24;
    var days = Math.floor(ms / day);
    var hours = Math.floor((ms - (day * days)) / hour);
    var minutes = Math.floor((ms - (day * days) - (hour * hours)) / minute);
    var seconds = Math.floor((ms - (day * days) - (hour * hours) - (minute * minutes)) / second);

    return days + ' ' + lang.task.data.deadline.time['day'] + ' ' + hours + ' ' + lang.task.data.deadline.time['hour'] + ' ' + minutes + ' ' + lang.task.data.deadline.time['minute'] + ' ' + seconds + ' ' + lang.task.data.deadline.time['second'];
}


// 获取任务状态信息
function taskStatus(status) {
    switch(status) {
        case '-1':
        return lang.task.data.status['rejected'];
        break;

        case '0':
        return lang.task.data.status['unprocessed'];
        break;

        case '1':
        return lang.task.data.status['accepted'];
        break;

        case '2':
        return lang.task.data.status['completed'];
        break;
    }
}