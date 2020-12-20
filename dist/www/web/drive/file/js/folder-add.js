// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    $id('name').value = lang.file.tips.value['new-folder'];

    if ($query('folderid') == 0) {
        $id('inherit').parentNode.parentNode.style.display = 'none';
    } else {
        $id('inherit').checked = true;
    }

    fastui.valueTips('name', lang.file.tips.value['name']);
    fastui.valueTips('remark', lang.file.tips.value['remark']);
}


// 文件夹添加表单提交
function folderAdd() {
    var folderId = $query('folderid');
    var name = $id('name').value;
    var remark = $id('remark').value;
    var inherit = $id('inherit').checked;
    var share = $id('share').checked;
    var data = '';

    if (fastui.testString(folderId, /^[1-9]{1}[\d]*$/) == false) {
        data += 'folderid=0&';
    } else {
        data += 'folderid=' + folderId + '&';
    }

    if (fastui.testInput(true, 'name', /^[^\\\/\:\*\?\"\<\>\|]{1,50}$/) == false) {
        fastui.inputTips('name', lang.file.tips.input['name']);
        return;
    } else {
        data += 'name=' + escape(name) + '&';
    }

    if (fastui.testInput(false, 'remark', /^[\s\S]{1,100}$/) == false) {
        fastui.inputTips('remark', lang.file.tips.input['remark']);
        return;
    } else if (remark.length > 0) {
        data += 'remark=' + escape(remark) + '&';
    }

    data += 'inherit=' + inherit + '&';
    data += 'share=' + share + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/folder-action.ashx?action=add', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (/^\d+$/.test(data) == true || data == 'complete') {
                window.parent.dataListActionCallback('complete');

                window.parent.folderTreeReload();

                window.setTimeout(function() {
                    // 共享文件夹(弹出共享权限管理窗口)
                    if (share == true) {
                        window.parent.fastui.windowPopup('purview-manage', lang.file.context['purview'] + ' - ' + name, '/web/drive/purview/purview-manage.html?id=' + data, 800, 500);
                    }

                    window.parent.fastui.windowClose('folder-add');
                    }, 500);
            } else if (data == 'existed') {
                fastui.inputTips('name', lang.file.tips['name-existed']);

                $id('name').focus();
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}