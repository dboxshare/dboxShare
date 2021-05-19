$include('/storage/data/department-data-json.js');
$include('/storage/data/role-data-json.js');


// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/api/user/card-data-json?id=' + $query('id') + '&username=' + encodeURIComponent($query('username')), 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.user.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                if (typeof(departmentDataJson) == 'undefined') {
                    $id('department').parentNode.parentNode.style.display = 'none';
                } else {
                    $id('department').innerHTML = userDepartment(jsonData.ds_departmentid);
                }

                if (typeof(roleDataJson) == 'undefined') {
                    $id('role').parentNode.parentNode.style.display = 'none';
                } else {
                    $id('role').innerHTML = userRole(jsonData.ds_roleid);
                }
        
                $id('username').innerHTML = jsonData.ds_username;
                $id('code').innerHTML = jsonData.ds_code;
                $id('title').innerHTML = jsonData.ds_title;
                $id('email').innerHTML = jsonData.ds_email;
                $id('phone').innerHTML = jsonData.ds_phone;
                $id('tel').innerHTML = jsonData.ds_tel;
                $id('admin').innerHTML = jsonData.ds_admin == 1 ? lang.user['admin'] : '';
                $id('status').innerHTML = jsonData.ds_status == 0 ? lang.user.data.status['job-on'] : lang.user.data.status['job-off'];

                if (jsonData.ds_loginip == '0.0.0.0') {
                    $id('logintime').innerHTML = lang.user['not-logged-in'];
                } else {
                    $id('logintime').innerHTML = toTimespan(jsonData.ds_logintime);
                }
            }

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 获取用户部门
function userDepartment(id) {
    if (typeof(departmentDataJson) == 'undefined') {
        return '';
    }

    try {
        var items = id.split('/');
        var department = '';

        for (var i = 1; i < items.length - 1; i++) {
            for (var j = 0; j < departmentDataJson.length; j++) {
                if (departmentDataJson[j].ds_id == items[i]) {
                    department += departmentDataJson[j].ds_name + ' / ';
                    break;
                }
            }
        }

        return department.substring(0, department.length - 3);
        } catch(e) {}
}


// 获取用户角色
function userRole(id) {
    if (typeof(roleDataJson) == 'undefined') {
        return '';
    }

    try {
        for (var i = 0; i < roleDataJson.length; i++) {
            if (roleDataJson[i].ds_id == id) {
                return roleDataJson[i].ds_name;
            }
        }

        return '';
        } catch(e) {}
}


// 获取时间间隔
function toTimespan(time) {
    var ms = new Date().getTime() - new Date(time.replace(/\-/g, '/'));
    var second = 1000;
    var minute = second * 60;
    var hour = minute * 60;
    var day = hour * 24;
    var month = day * 30;
    var year = day * 365;

    if (Math.floor(ms / second) == 0) {
        return '0 ' + lang.timespan['second'] + ' ' + time;
    } else if (Math.floor(ms / second) < 60) {
        return Math.floor(ms / second) + ' ' + lang.timespan['second'] + ' ' + time;
    } else if (Math.floor(ms / minute) < 60) {
        return Math.floor(ms / minute) + ' ' + lang.timespan['minute'] + ' ' + time;
    } else if (Math.floor(ms / hour) < 24) {
        return Math.floor(ms / hour) + ' ' + lang.timespan['hour'] + ' ' + time;
    } else if (Math.floor(ms / day) < 30) {
        return Math.floor(ms / day) + ' ' + lang.timespan['day'] + ' ' + time;
    } else if (Math.floor(ms / month) < 12) {
        return Math.floor(ms / month) + ' ' + lang.timespan['month'] + ' ' + time;
    } else if (Math.floor(ms / year) > 0) {
        return Math.floor(ms / year) + ' ' + lang.timespan['year'] + ' ' + time;
    }
}