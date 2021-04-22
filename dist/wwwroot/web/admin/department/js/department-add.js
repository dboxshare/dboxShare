// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('name', lang.department.tips.value['name']);
}


// 部门添加表单提交
function departmentAdd() {
    var departmentId = $query('departmentid') || 0;
    var name = $id('name').value;
    var data = '';

    data += 'departmentid=' + departmentId + '&';

    if (fastui.testInput(true, 'name', /^([^\\\/\:\*\?\"\<\>\|]{2,24}[\r\n]?){1,50}$/) == false) {
        fastui.inputTips('name', lang.department.tips.input['name']);
        return;
    } else {
        data += 'name=' + escape(name) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/department/add', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.dataListActionCallback('complete');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('department-add');
                    }, 500);
            } else if (data == 'existed') {
                fastui.inputTips('name', lang.department.tips['name-existed']);
            } else {
                fastui.textTips(lang.department.tips['operation-failed']);
            }
            }
        });
}