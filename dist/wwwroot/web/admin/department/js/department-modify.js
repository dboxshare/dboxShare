// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('name', lang.department.tips.value['name']);
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/api/admin/department/data-json?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.department.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('departmentid').value = jsonData.ds_departmentid;
                $id('name').value = jsonData.ds_name;
            }

            dataInit();

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 部门修改表单提交
function departmentModify() {
    var id = $query('id');
    var departmentId = $id('departmentid').value;
    var name = $id('name').value;
    var data = '';

    if (fastui.testString(id, /^[1-9]{1}[\d]*$/) == false) {
        fastui.textTips(lang.input.tips.input['id']);
        return;
    } else {
        data += 'id=' + id + '&';
    }

    data += 'departmentid=' + departmentId + '&';

    if (fastui.testInput(true, 'name', /^[^\\\/\:\*\?\"\<\>\|]{2,24}$/) == false) {
        fastui.inputTips('name', lang.department.tips.input['name']);
        return;
    } else {
        data += 'name=' + escape(name) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/department/modify', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.dataListActionCallback('complete');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('department-modify');
                    }, 500);
            } else if (data == 'existed') {
                fastui.inputTips('name', lang.department.tips['name-existed']);
            } else {
                fastui.textTips(lang.department.tips['operation-failed']);
            }
            }
        });
}