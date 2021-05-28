// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    var field = $query('field');

    if (field == 'uploadsize') {
        $id('uploadsize').parentNode.parentNode.style.display = 'block';

        fastui.sideTips('uploadsize', lang.user.tips.side['upload-size']);
    }

    if (field == 'downloadsize') {
        $id('downloadsize').parentNode.parentNode.style.display = 'block';

        fastui.sideTips('downloadsize', lang.user.tips.side['download-size']);
    }
}


// 用户更改表单提交
function userChange() {
    var checkboxes = window.parent.$name('id');
    var field = $query('field');
    var uploadSize = $id('uploadsize').value;
    var downloadSize = $id('downloadsize').value;
    var data = '';

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            data += 'id=' + checkboxes[i].value + '&';
        }
    }

    if (data.length == 0) {
        window.parent.fastui.textTips(lang.user.tips['please-select-item']);
        window.parent.fastui.windowClose('user-change');
        return;
    }

    if (field == 'uploadsize') {
        if (fastui.testInput(true, 'uploadsize', /^[\d]{1,5}$/) == false) {
            fastui.inputTips('uploadsize', lang.user.tips.input['upload-size']);
            return;
        } else {
            if (uploadSize < 0 || uploadSize > 10240) {
                fastui.inputTips('uploadsize', lang.user.tips.input['upload-size-range']);
                return;
            }

            data += 'uploadsize=' + uploadSize + '&';
        }
    }

    if (field == 'downloadsize') {
        if (fastui.testInput(true, 'downloadsize', /^[\d]{1,5}$/) == false) {
            fastui.inputTips('downloadsize', lang.user.tips.input['download-size']);
            return;
        } else {
            if (downloadSize < 0 || downloadSize > 10240) {
                fastui.inputTips('downloadsize', lang.user.tips.input['download-size-range']);
                return;
            }

            data += 'downloadsize=' + downloadSize + '&';
        }
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/user/change', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.dataListActionCallback('complete');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('user-change');
                    }, 500);
            } else {
                fastui.textTips(lang.user.tips['operation-failed']);
            }
            }
        });
}