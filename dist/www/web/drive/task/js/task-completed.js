// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('postscript', lang.task.tips.value['postscript']);
}


// 确定选项更改
function confirmChange(button) {
    if ($id('confirm').checked == true) {
        $id(button).disabled = false;
    } else {
        $id(button).disabled = true;
    }
}


// 任务完成表单提交
function taskCompleted() {
    var id = $query('id');
    var postscript = $id('postscript').value;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.task.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    if (fastui.testInput(false, 'postscript', /^[\s\S]{1,200}$/) == false) {
        fastui.inputTips('postscript', lang.task.tips.input['postscript']);
        return;
    } else if (postscript.length > 0) {
        data += 'postscript=' + escape(postscript) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/task/task-action.ashx?action=completed', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.iconTips('tick');

                window.setTimeout(function() {
                    window.parent.location.reload(true);

                    window.parent.fastui.windowClose('task-completed');
                    }, 500);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}