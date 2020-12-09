$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    var type = $query('type');

    if (type == 'department') {
        fastui.listTreeSelect('department-select', jsonDataDepartment, 'dbs_id', 'dbs_departmentid', 'dbs_name', 0, 0);
    } else {
        $id('department-select').style.display = 'none';
    }

    if (type == 'role') {
        fastui.listSelect('role-select', jsonDataRole, 'dbs_id', 'dbs_name');
    } else {
        $id('role-select').style.display = 'none';
    }
}


// 用户归类表单提交
function userClassify() {
    var checkboxes = window.parent.$name('id');
    var type = $query('type');
    var classifyId = $id(type).value;
    var classifyType = type;
    var data = '';
    var i;

    for (i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            data += 'id=' + checkboxes[i].value + '&';
        }
    }

    if (data.length == 0) {
        window.parent.fastui.textTips(lang.user.tips['please-select-item']);
        window.parent.fastui.windowClose('user-classify');
        return;
    }

    data += 'classifyid=' + classifyId + '&';
    data += 'classifytype=' + classifyType + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/admin/user/user-action.ashx?action=classify', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.dataListActionCallback('complete');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('user-classify');
                    }, 500);
            } else {
                fastui.textTips(lang.user.tips['operation-failed']);
            }
            }
        });
}