// 确定选项更改
function confirmChange(button) {
    if ($id('confirm').checked == true) {
        $id(button).disabled = false;
    } else {
        $id(button).disabled = true;
    }
}


// 任务接受表单提交
function taskAccept() {
    var id = $query('id');
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.task.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/task/accept', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.iconTips('tick');

                window.setTimeout(function() {
                    window.parent.location.reload(true);

                    window.parent.fastui.windowClose('task-accept');
                    }, 500);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}