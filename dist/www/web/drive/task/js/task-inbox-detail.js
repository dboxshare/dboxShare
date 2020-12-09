// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/web/drive/task/task-inbox-data-json.ashx?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if (data.length == 0 || data == '{}') {
                fastui.coverTips(lang.task.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                if (jsonData.dbs_isfolder == 1) {
                    $id('file').innerHTML = '<a href=\"javascript: window.parent.fastui.windowPopup(\'folder-detail\', \'' + lang.file.context['detail'] + ' - ' + jsonData.dbs_filename + '\', \'/web/drive/file/folder-detail.html?id=' + jsonData.dbs_fileid + '\', 800, 500);\" title=\"' + jsonData.dbs_filename + '\">' + jsonData.dbs_filename + '</a>';
                } else {
                    $id('file').innerHTML = '<a href=\"javascript: window.parent.fastui.windowPopup(\'file-detail\', \'' + lang.file.context['detail'] + ' - ' + jsonData.dbs_filename + '' + jsonData.dbs_fileextension + '\', \'/web/drive/file/file-detail.html?id=' + jsonData.dbs_fileid + '\', 800, 500);\" title=\"' + jsonData.dbs_filename + '' + jsonData.dbs_fileextension + '\">' + jsonData.dbs_filename + '' + jsonData.dbs_fileextension + '</a>';
                }

                $id('user').innerHTML = '<a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonData.dbs_username)) + '\', 500, 500);\">' + jsonData.dbs_username + '</a>';
                $id('level').innerHTML = taskLevel(jsonData.dbs_level);
                $id('status').innerHTML = taskStatus(jsonData.dbs_status);
                $id('deadline').innerHTML = jsonData.dbs_deadline;
                $id('cause').innerHTML = lang.task['cause'] + ' : ' + jsonData.dbs_cause;
                $id('time').innerHTML = taskTime(jsonData.dbs_time);
                $id('content').innerHTML = jsonData.dbs_content;

                if (jsonData.dbs_revoke == 0) {
                    if (new Date(jsonData.dbs_deadline.replace(/\-/g, '/')).getTime() > new Date().getTime()) {
                        $id('left-time').innerHTML = taskDeadline(jsonData.dbs_deadline);

                        if (jsonData.dbs_status == 0) {
                            $id('accept-button').style.display = 'inline-block';
                            $id('reject-button').style.display = 'inline-block';
                        } else if (jsonData.dbs_status == 1) {
                            $id('completed-button').style.display = 'inline-block';
                        }
                    } else {
                        $id('left-time').innerHTML = lang.task.data.status['expired'];

                        $id('expired-warning').style.display = 'block';
                    }
                } else {
                    $id('revoked-warning').style.display = 'block';
                }

                taskMember(jsonData.dbs_id);
            }

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 任务参与成员列表
function taskMember(id) {
    $ajax({
        type: 'GET', 
        url: '/web/drive/task/task-member-list-json.ashx?id=' + id, 
        async: true, 
        callback: function(data) {
            var list = fastui.list.dataTable('data-list');
            var row;
            var i;

            if (data.length > 0 && data != '{}') {
                window.eval('var jsonDataMember = {items:' + data + '};');

                for (i = 0; i < jsonDataMember.items.length; i++) {
                    row = list.addRow(i);

                    // 图标
                    row.addCell(
                        {width: '50', align: 'center'}, 
                        '<div class=\"icon\"><img src=\"/ui/images/avatar-icon.png\" width=\"32\" /></div>'
                        );

                    // 信息
                    row.addCell(
                        null, 
                        function() {
                            var html = '';

                            html += '<div class=\"time\">';
                        
                            if (jsonDataMember.items[i].dbs_status == -1) {
                                html += '' + taskTime(jsonDataMember.items[i].dbs_rejectedtime) + '';
                            } else if (jsonDataMember.items[i].dbs_status == 1) {
                                html += '' + taskTime(jsonDataMember.items[i].dbs_acceptedtime) + '';
                            } else if (jsonDataMember.items[i].dbs_status == 2) {
                                html += '' + taskTime(jsonDataMember.items[i].dbs_completedtime) + '';
                            }

                            html += '</div>';

                            html += '<div class=\"box\">';
                            html += '<div class=\"username\"><a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonDataMember.items[i].dbs_username)) + '\', 500, 500);\">' + jsonDataMember.items[i].dbs_username + '</a></div>';

                            if (jsonDataMember.items[i].dbs_status == -1) {
                                if (jsonDataMember.items[i].dbs_userid == window.top.myId) {
                                    if (jsonDataMember.items[i].dbs_reason.length > 0) {
                                        html += '<div class=\"message\">' + lang.task['reason'] + ' : ' + jsonDataMember.items[i].dbs_reason + '</div>';
                                    }
                                }
                            } else if (jsonDataMember.items[i].dbs_status == 2) {
                                if (jsonDataMember.items[i].dbs_postscript.length > 0) {
                                    html += '<div class=\"message\">' + lang.task['postscript'] + ' : ' + jsonDataMember.items[i].dbs_postscript + '</div>';
                                }
                            }

                            html += '<span class=\"status\">' + taskStatus(jsonDataMember.items[i].dbs_status) + '</span>';
                            html += '</div>';

                            return html;
                        });
                }
            }
            }
        });
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