// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('cause', lang.task.tips.value['cause']);
}


// 确定选项更改
function confirmChange(button) {
    if ($id('confirm').checked == true) {
        $id(button).disabled = false;
    } else {
        $id(button).disabled = true;
    }
}


// 任务撤消表单提交
function taskRevoke() {
    var id = $query('id');
    var cause = $id('cause').value;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.task.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    if (fastui.testInput(true, 'cause', /^[\s\S]{1,200}$/) == false) {
        fastui.inputTips('cause', lang.task.tips.input['cause']);
        return;
    } else {
        data += 'cause=' + escape(cause) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/task/task-action.ashx?action=revoke', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.iconTips('tick');

                window.setTimeout(function() {
                    window.parent.location.reload(true);

                    window.parent.fastui.windowClose('task-revoke');
                    }, 500);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}