$include('/storage/data/user-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    $id('date').value = $formatTime('yyyy-MM-dd', new Date(new Date().getTime() + (24 * 60 * 60 * 1000)));
    $id('hour').value = $formatTime('HH');
    $id('minute').value = $formatTime('mm');

    fastui.radioSelect('level-select', [{value:'0',name:'' + lang.task.data.level['normal'] + ''},{value:'1',name:'' + lang.task.data.level['important'] + ''},{value:'2',name:'' + lang.task.data.level['urgent'] + ''}], 'value', 'name', '#2196F3');

    fastui.listSelect('date-select', deadlineDate(30), 'value', 'name');
    fastui.listSelect('hour-select', deadlineHour(), 'value', 'name');
    fastui.listSelect('minute-select', deadlineMinute(), 'value', 'name');

    fastui.valueTips('content', lang.task.tips.value['content']);
}


// 用户选择(弹出窗口)
function userSelect() {
    var users = $name('user');
    var data = '';

    for (var i = 0; i < users.length; i++) {
        data += users[i].value + ',';
    }

    data = data.substring(0, data.length - 1);

    window.parent.fastui.windowPopup('task-user-select', lang.object['user'], '/web/drive/task/task-user-select.html' + window.location.search + '&user=' + data + '&iframe=task-assign-iframe&callback=userSelectCallback', 800, 500);
}


// 用户选择(回调函数)
function userSelectCallback(data) {
    var users = data.split(',');
    var items = '';

    for (var i = 0; i < users.length; i++) {
        for (var j = 0; j < userDataJson.length; j++) {
            if (userDataJson[j].ds_id == users[i]) {
                items += '<div class="item">';
                items += '<span class="username">' + userDataJson[j].ds_username + '</span>';
                items += '<span class="title">' + userDataJson[j].ds_title + '</span>';
                items += '<span class="delete" onClick="userDelete(' + userDataJson[j].ds_id + ');">✕</span>';
                items += '<input name="user" type="hidden" value="' + userDataJson[j].ds_id + '" />';
                items += '</div>';
                break;
            }
        }
    }

    $id('user-list').innerHTML = items;
}


// 用户删除
function userDelete(id) {
    var users = $name('user');

    for (var i = 0; i < users.length; i++) {
        if (users[i].value == id) {
            $remove(users[i].parentNode);
            return;
        }
    }
}


// 生成期限日期数据
function deadlineDate(day) {
    var now = new Date();
    var json = '';

    for (var i = 0; i < day; i++) {
        var time = new Date(now.getFullYear(), now.getMonth(), now.getDate() + i);
        var date = $formatTime('yyyy-MM-dd', time);
        var week = '';

        switch(time.getDay()) {
            case 0:
            week = lang.task.data.deadline.week['sunday'];
            break;

            case 1:
            week = lang.task.data.deadline.week['monday'];
            break;

            case 2:
            week = lang.task.data.deadline.week['tuesday'];
            break;

            case 3:
            week = lang.task.data.deadline.week['wednesday'];
            break;

            case 4:
            week = lang.task.data.deadline.week['thursday'];
            break;

            case 5:
            week = lang.task.data.deadline.week['friday'];
            break;

            case 6:
            week = lang.task.data.deadline.week['saturday'];
            break;
        }

        json += '{\'value\':\'' + date + '\',\'name\':\'' + date + ' ' + week + '\'},';
    }

    json = json.substring(0, json.length - 1);

    json = window.eval('[' + json + ']');

    return json;
}


// 生成期限小时数据
function deadlineHour() {
    var json = '';

    for (var i = 0; i < 24; i++) {
        var hour = i < 10 ? '0' + i : i;

        json += '{\'value\':\'' + hour + '\',\'name\':\'' + hour + ' ' + lang.task.data.deadline.time['hour'] + '\'},';
    }

    json = json.substring(0, json.length - 1);

    json = window.eval('[' + json + ']');

    return json;
}


// 生成期限分钟数据
function deadlineMinute() {
    var json = '';

    for (var i = 0; i < 60; i++) {
        var minute = i < 10 ? '0' + i : i;

        json += '{\'value\':\'' + minute + '\',\'name\':\'' + minute + ' ' + lang.task.data.deadline.time['minute'] + '\'},';
    }

    json = json.substring(0, json.length - 1);

    json = window.eval('[' + json + ']');

    return json;
}


// 任务分派表单提交
function taskAssign() {
    var id = $query('id');
    var users = $name('user');
    var level = $id('level').value;
    var date = $id('date').value;
    var hour = $id('hour').value;
    var minute = $id('minute').value;
    var content = $id('content').value;
    var data = '';

    if (fastui.testString(id, /^[\d]+$/) == false) {
        fastui.textTips(lang.task.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    if (users.length == 0) {
        fastui.textTips(lang.task.tips['please-select-user']);
        return;
    }

    for (var i = 0; i < users.length; i++) {
        data += 'user=' + users[i].value + '&';
    }

    data += 'level=' + level + '&';

    var deadline = date + ' ' + hour + ':' + minute + ':00';

    if (new Date(deadline.replace(/\-/g, '/')) < new Date()) {
        fastui.textTips(lang.task.tips['deadline-time-error']);
        return;
    }

    data += 'deadline=' + escape(deadline) + '&';

    if (fastui.testInput(true, 'content', /^[\s\S]{1,500}$/) == false) {
        fastui.inputTips('content', lang.task.tips.input['content']);
        return;
    } else {
        data += 'content=' + escape(content) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/task/assign', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.iconTips('tick');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('task-assign');
                    }, 500);
            } else {
                fastui.textTips(lang.task.tips['operation-failed']);
            }
            }
        });
}