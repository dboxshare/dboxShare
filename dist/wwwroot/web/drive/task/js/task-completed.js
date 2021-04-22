// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('remark', lang.task.tips.value['remark']);
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
    var remark = $id('remark').value;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.task.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    if (fastui.testInput(false, 'remark', /^[\s\S]{1,200}$/) == false) {
        fastui.inputTips('remark', lang.task.tips.input['remark']);
        return;
    } else if (remark.length > 0) {
        data += 'remark=' + escape(remark) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/task/completed', 
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