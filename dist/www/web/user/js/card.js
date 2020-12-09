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
        url: '/web/user/card-data-json.ashx?id=' + $query('id') + '&username=' + encodeURIComponent($query('username')), 
        async: true, 
        callback: function(data) {
            if (data.length == 0 || data == '{}') {
                fastui.coverTips(lang.user.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                if (typeof(jsonDataDepartment) == 'undefined') {
                    $id('department').parentNode.parentNode.style.display = 'none';
                } else {
                    $id('department').innerHTML = userDepartment(jsonData.dbs_departmentid);
                }

                if (typeof(jsonDataRole) == 'undefined') {
                    $id('role').parentNode.parentNode.style.display = 'none';
                } else {
                    $id('role').innerHTML = userRole(jsonData.dbs_roleid);
                }
        
                $id('username').innerHTML = jsonData.dbs_username;
                $id('code').innerHTML = jsonData.dbs_code;
                $id('position').innerHTML = jsonData.dbs_position;
                $id('email').innerHTML = jsonData.dbs_email;
                $id('phone').innerHTML = jsonData.dbs_phone;
                $id('tel').innerHTML = jsonData.dbs_tel;
                $id('admin').innerHTML = jsonData.dbs_admin == 1 ? lang.user['admin'] : '';
                $id('status').innerHTML = jsonData.dbs_status == 1 ? lang.user.data.status['job-on'] : lang.user.data.status['job-off'];

                if (jsonData.dbs_loginip == '0.0.0.0') {
                    $id('logintime').innerHTML = lang.user['not-logged-in'];
                } else {
                    $id('logintime').innerHTML = toTimespan(jsonData.dbs_logintime);
                }
            }

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 获取用户部门
function userDepartment(id) {
    if (typeof(jsonDataDepartment) == 'undefined') {
        return '';
    }

    try {
        var items = new Array();
        var department = '';
        var i;
        var j;

        items = id.split('/');

        for (i = 1; i < items.length - 1; i++) {
            for (j = 0; j < jsonDataDepartment.length; j++) {
                if (jsonDataDepartment[j].dbs_id == items[i]) {
                    department += jsonDataDepartment[j].dbs_name + ' / ';
                    break;
                }
            }
        }

        return department.substring(0, department.length - 3);
        } catch(e) {}
}


// 获取用户角色
function userRole(id) {
    if (typeof(jsonDataRole) == 'undefined') {
        return '';
    }

    try {
        var i;

        for (i = 0; i < jsonDataRole.length; i++) {
            if (jsonDataRole[i].dbs_id == id) {
                return jsonDataRole[i].dbs_name;
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