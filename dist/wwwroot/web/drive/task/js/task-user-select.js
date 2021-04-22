$include('/storage/data/department-data-json.js');
$include('/storage/data/user-data-json.js');


// 页面初始化事件调用函数
function init() {
    pageResize();
    userTree();
}


// 页面调整
function pageResize() {
    var container = $id('container');
    var footer = $id('footer');

    container.style.height = (container.offsetHeight - footer.offsetHeight) + 'px';
}


// 生成用户树形目录(主体)
function userTree() {
    var user = $query('user');
    var items = '';

    if (typeof(departmentDataJson) == 'undefined') {
        for (var i = 0; i < userDataJson.length; i++) {
            if (userDataJson[i].ds_id == window.top.myId) {
                continue;
            }

            items += '<li>';
            items += '<div class="item" onClick="checkboxSelect(' + userDataJson[i].ds_id + ', event);">';
            items += '<div class="blank"></div>';
            items += '<div class="state none"></div>';
            items += '<div class="box"><input name="id" type="checkbox" value="' + userDataJson[i].ds_id + '" /><label name="checkbox"></label></div>';
            items += '<div class="icon"><img src="/ui/images/select-user-icon.png" width="16" /></div>';
            items += '<div class="name">' + userDataJson[i].ds_username + '&nbsp;&nbsp;<font color="#757575">' + userDataJson[i].ds_title + '</font></div>';
            items += '</div>';
            items += '<div class="node"></div>';
            items += '</li>';
        }
    } else {
        for (var i = 0; i < departmentDataJson.length; i++) {
              if (departmentDataJson[i].ds_departmentid == 0) {
                var node = userTreeNode(departmentDataJson[i].ds_id, 1);

                items += '<li>';
                items += '<div class="item" onClick="itemSelect(' + departmentDataJson[i].ds_id + ', event);">';
                items += '<div class="state">';

                if (node.length > 0) {
                    items += '<img src="/ui/images/select-node-close-icon.png" width="16" />';
                }

                items += '</div>';
                items += '<div class="box none"><input name="id" type="checkbox" value="' + departmentDataJson[i].ds_id + '" optional="false" /></div>';
                items += '<div class="icon"><img src="/ui/images/select-department-icon.png" width="16" /></div>';
                items += '<div class="name">' + departmentDataJson[i].ds_name + '<input name="name" type="hidden" value="' + departmentDataJson[i].ds_name + '" /></div>';
                items += '</div>';
                items += '<div class="node">' + node + '</div>';
                items += '</li>';
            }
        }
    }

    $id('container').innerHTML = '<ul>' + items + '</ul>';

    itemSelected(user);
}


// 生成用户树形目录(节点)
function userTreeNode(departmentId, level) {
    var items = '';

    for (var i = 0; i < departmentDataJson.length; i++) {
          if (departmentDataJson[i].ds_departmentid == departmentId) {
            var node = userTreeNode(departmentDataJson[i].ds_id, level + 1);

            items += '<li>';
            items += '<div class="item" onClick="itemSelect(' + departmentDataJson[i].ds_id + ', event);">';
            items += '<div class="blank" style="width: ' + (24 * level) + 'px;"></div>';
            items += '<div class="state">';

            if (node.length > 0) {
                items += '<img src="/ui/images/select-node-close-icon.png" width="16" />';
            }

            items += '</div>';
            items += '<div class="box none"><input name="id" type="checkbox" value="' + departmentDataJson[i].ds_id + '" optional="false" /></div>';
            items += '<div class="icon"><img src="/ui/images/select-department-icon.png" width="16" /></div>';
            items += '<div class="name">' + departmentDataJson[i].ds_name + '<input name="name" type="hidden" value="' + departmentDataJson[i].ds_name + '" /></div>';
            items += '</div>';
            items += '<div class="node">' + node + '</div>';
            items += '</li>';
        }
    }

    for (var j = 0; j < userDataJson.length; j++) {
        if (userDataJson[j].ds_id == window.top.myId) {
            continue;
        }

        if (window.eval('/\\/' + departmentId + '\\/$/').test(userDataJson[j].ds_departmentid) == true) {
            items += '<li>';
            items += '<div class="item" onClick="checkboxSelect(' + userDataJson[j].ds_id + ', event);">';
            items += '<div class="blank" style="width: ' + (24 * level) + 'px;"></div>';
            items += '<div class="state none"></div>';
            items += '<div class="box"><input name="id" type="checkbox" value="' + userDataJson[j].ds_id + '" optional="true" /><label name="checkbox"></label></div>';
            items += '<div class="icon"><img src="/ui/images/select-user-icon.png" width="16" /></div>';
            items += '<div class="name">' + userDataJson[j].ds_username + '&nbsp;&nbsp;<font color="#757575">' + userDataJson[j].ds_title + '</font></div>';
            items += '</div>';
            items += '<div class="node"></div>';
            items += '</li>';
        }
    }

    if (items.length == 0) {
        return '';
    } else {
        return '<ul id="' + departmentId + '">' + items + '</ul>';
    }
}


// 项目选择(手动选择)
function itemSelect(id, _event) {
    var event = _event || window.event;
    var tag = event.target || event.srcElement;
    var checkboxes = $name('id');
    var states = $class('state');
    var nodes = $class('node');

    if ($get(tag, 'name') == 'checkbox') {
        return;
    }

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].$get('optional') == 'false' && checkboxes[i].value == id) {
            if (typeof(states[i]) == 'undefined') {
                continue;
            }

            var mark = states[i].$tag('img')[0];

            if (typeof(mark) != 'undefined') {
                if (mark.src.indexOf('select-node-open-icon') == -1) {
                    mark.src = '/ui/images/select-node-open-icon.png';

                    nodes[i].style.display = 'block';
                } else {
                    mark.src = '/ui/images/select-node-close-icon.png';

                    nodes[i].style.display = 'none';
                }
            }

            break;
        }
    }
}


// 项目选择(自动选择上次已选中项目)
function itemSelected(user) {
    if (user.length == 0) {
        return;
    }

    var users = user.split(',');
    var radios = $name('id');
    var items = $class('item');

    for (var i = 0; i < users.length; i++) {
        for (var j = 0; j < radios.length; j++) {
            if (users[i] == radios[j].value) {
                items[j].click();
                break;
            }
        }
    }
}


// 复选框选择
function checkboxSelect(id, _event) {
    var event = _event || window.event;
    var tag = event.target || event.srcElement;
    var checkboxes = $name('id');
    var items = $class('item');

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].$get('optional') == 'true' && checkboxes[i].value == id) {
            if (checkboxes[i].checked == false) {
                checkboxes[i].checked = true;

                items[i].style.backgroundColor = '#E0E0E0';
            } else {
                checkboxes[i].checked = false;

                items[i].style.backgroundColor = '';
            }

            break;
        }
    }
}


// 提交用户选择
function userSelect() {
    var iframe = $query('iframe');
    var callback = $query('callback');
    var form = iframe.length == 0 ? 'window.parent' : 'window.parent.$id(\'' + iframe + '\').contentWindow';
    var checkboxes = $name('id');
    var data = '';

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            data += checkboxes[i].value + ',';
        }
    }

    if (data.length == 0) {
        fastui.textTips(lang.task.tips['please-select-user']);
        return;
    } else {
        data = data.substring(0, data.length - 1);
    }

    window.eval('var callbackFunction = ' + form + '.' + callback);
    callbackFunction(data);

    window.parent.fastui.windowClose('task-user-select');
}