// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/task/outbox-data-json?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.task.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                if (jsonData.ds_isfolder == 1) {
                    $id('file').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'folder-detail\', \'' + lang.file.context['detail'] + ' - ' + jsonData.ds_filename + '\', \'/web/drive/folder/folder-detail.html?id=' + jsonData.ds_fileid + '\', 800, 500);" title="' + jsonData.ds_filename + '">' + jsonData.ds_filename + '</a>';
                } else {
                    $id('file').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'file-detail\', \'' + lang.file.context['detail'] + ' - ' + jsonData.ds_filename + '' + jsonData.ds_fileextension + '\', \'/web/drive/file/file-detail.html?id=' + jsonData.ds_fileid + '\', 800, 500);" title="' + jsonData.ds_filename + '' + jsonData.ds_fileextension + '">' + jsonData.ds_filename + '' + jsonData.ds_fileextension + '</a>';
                }

                $id('user').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/user/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.ds_username)) + '\', 500, 500);">' + jsonData.ds_username + '</a>';
                $id('level').innerHTML = taskLevel(jsonData.ds_level);
                $id('deadline').innerHTML = jsonData.ds_deadline;
                $id('cause').innerHTML = lang.task['cause'] + ' : ' + jsonData.ds_cause;
                $id('time').innerHTML = taskTime(jsonData.ds_time);
                $id('content').innerHTML = jsonData.ds_content;

                if (jsonData.ds_revoke == 0) {
                    if (new Date(jsonData.ds_deadline.replace(/\-/g, '/')).getTime() > new Date().getTime()) {
                        $id('left-time').innerHTML = taskDeadline(jsonData.ds_deadline);
                    } else {
                        $id('left-time').innerHTML = lang.task.data.status['expired'];

                        $id('expired-warning').style.display = 'block';
                    }

                    if (Math.floor((new Date().getTime() - new Date(jsonData.ds_time.replace(/\-/g, '/')).getTime()) / (1000 * 60 * 60)) <= 24) {
                        $id('revoke-button').style.display = 'inline-block';
                    }
                } else {
                    $id('revoked-warning').style.display = 'block';
                }

                taskMember(jsonData.ds_id);
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
        url: '/api/drive/task/member-list-json?id=' + id, 
        async: true, 
        callback: function(data) {
            var list = fastui.list.dataTable('data-list');

            if (data.length > 0 && data != '{}') {
                window.eval('var memberDataJson = {items:' + data + '};');

                for (var i = 0; i < memberDataJson.items.length; i++) {
                    var row = list.addRow(i);

                    // 图标
                    row.addCell(
                        {width: '50', align: 'center'}, 
                        '<div class="icon"><img src="/ui/images/avatar-icon.png" width="32" /></div>'
                        );

                    // 信息
                    row.addCell(
                        null, 
                        function() {
                            var html = '';

                            html += '<div class="time">';
                        
                            if (memberDataJson.items[i].ds_status == -1) {
                                html += '' + taskTime(memberDataJson.items[i].ds_rejectedtime) + '';
                            } else if (memberDataJson.items[i].ds_status == 1) {
                                html += '' + taskTime(memberDataJson.items[i].ds_acceptedtime) + '';
                            } else if (memberDataJson.items[i].ds_status == 2) {
                                html += '' + taskTime(memberDataJson.items[i].ds_completedtime) + '';
                            }

                            html += '</div>';

                            html += '<div class="box">';
                            html += '<div class="username"><a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/user/user-card.html?username=' + encodeURI(encodeURIComponent(memberDataJson.items[i].ds_username)) + '\', 500, 500);">' + memberDataJson.items[i].ds_username + '</a></div>';

                            if (memberDataJson.items[i].ds_status == -1) {
                                if (memberDataJson.items[i].ds_reason.length > 0) {
                                    html += '<div class="message">' + lang.task['reason'] + ' : ' + memberDataJson.items[i].ds_reason + '</div>';
                                }
                            } else if (memberDataJson.items[i].ds_status == 2) {
                                if (memberDataJson.items[i].ds_remark.length > 0) {
                                    html += '<div class="message">' + lang.task['remark'] + ' : ' + memberDataJson.items[i].ds_remark + '</div>';
                                }
                            }

                            html += '<span class="status">' + taskStatus(memberDataJson.items[i].ds_status) + '</span>';
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