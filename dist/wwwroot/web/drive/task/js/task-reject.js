// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('reason', lang.task.tips.value['reason']);
}


// 确定选项更改
function confirmChange(button) {
    if ($id('confirm').checked == true) {
        $id(button).disabled = false;
    } else {
        $id(button).disabled = true;
    }
}


// 任务拒绝表单提交
function taskReject() {
    var id = $query('id');
    var reason = $id('reason').value;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.task.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    if (fastui.testInput(true, 'reason', /^[\s\S]{1,200}$/) == false) {
        fastui.inputTips('reason', lang.task.tips.input['reason']);
        return;
    } else {
        data += 'reason=' + escape(reason) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/task/reject', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.iconTips('tick');

                window.setTimeout(function() {
                    window.parent.location.reload(true);

                    window.parent.fastui.windowClose('task-reject');
                    }, 500);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}