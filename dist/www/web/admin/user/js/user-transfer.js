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
    var container = $id('container');
    var items = '';
    var node;
    var i;

    if (typeof(jsonDataDepartment) == 'undefined') {
        for (i = 0; i < jsonDataUser.length; i++) {
            if (jsonDataUser[i].dbs_id == $query('id')) {
                continue;
            }

            items += '<li>';
            items += '<div class=\"item\" onClick=\"radioSelect(' + jsonDataUser[i].dbs_id + ', event);\">';
            items += '<div class=\"blank\"></div>';
            items += '<div class=\"state none\"></div>';
            items += '<div class=\"box\"><input name=\"id\" type=\"radio\" class=\"radio\" value=\"' + jsonDataUser[i].dbs_id + '\" /></div>';
            items += '<div class=\"icon\"><img src=\"/ui/images/select-user-icon.png\" width=\"16\" /></div>';
            items += '<div class=\"name\">' + jsonDataUser[i].dbs_username + '&nbsp;&nbsp;<font color=\"#757575\">' + jsonDataUser[i].dbs_position + '</font></div>';
            items += '</div>';
            items += '<div class=\"node\"></div>';
            items += '</li>';
        }
    } else {
        for (i = 0; i < jsonDataDepartment.length; i++) {
              if (jsonDataDepartment[i].dbs_departmentid == 0) {
                node = userTreeNode(jsonDataDepartment[i].dbs_id, 1);

                items += '<li>';
                items += '<div class=\"item\" onClick=\"itemSelect(' + jsonDataDepartment[i].dbs_id + ', event);\">';
                items += '<div class=\"state\">';

                if (node.length > 0) {
                    items += '<img src=\"/ui/images/select-node-close-icon.png\" width=\"16\" />';
                }

                items += '</div>';
                items += '<div class=\"box none\"><input name=\"id\" type=\"radio\" class=\"none\" value=\"' + jsonDataDepartment[i].dbs_id + '\" /></div>';
                items += '<div class=\"icon\"><img src=\"/ui/images/select-department-icon.png\" width=\"16\" /></div>';
                items += '<div class=\"name\">' + jsonDataDepartment[i].dbs_name + '</div>';
                items += '</div>';
                items += '<div class=\"node\">' + node + '</div>';
                items += '</li>';
            }
        }
    }

    container.innerHTML = '<ul>' + items + '</ul>';
}


// 生成用户树形目录(节点)
function userTreeNode(departmentId, level) {
    var items = '';
    var node;
    var i;

    for (i = 0; i < jsonDataDepartment.length; i++) {
          if (jsonDataDepartment[i].dbs_departmentid == departmentId) {
            node = userTreeNode(jsonDataDepartment[i].dbs_id, level + 1);

            items += '<li>';
            items += '<div class=\"item\" onClick=\"itemSelect(' + jsonDataDepartment[i].dbs_id + ', event);\">';
            items += '<div class=\"blank\" style=\"width: ' + (24 * level) + 'px;\"></div>';
            items += '<div class=\"state\">';

            if (node.length > 0) {
                items += '<img src=\"/ui/images/select-node-close-icon.png\" width=\"16\" />';
            }

            items += '</div>';
            items += '<div class=\"box none\"><input name=\"id\" type=\"radio\" class=\"none\" value=\"' + jsonDataDepartment[i].dbs_id + '\" /></div>';
            items += '<div class=\"icon\"><img src=\"/ui/images/select-department-icon.png\" width=\"16\" /></div>';
            items += '<div class=\"name\">' + jsonDataDepartment[i].dbs_name + '</div>';
            items += '</div>';
            items += '<div class=\"node\">' + node + '</div>';
            items += '</li>';
        }
    }

    for (i = 0; i < jsonDataUser.length; i++) {
        if (jsonDataUser[i].dbs_id == $query('id')) {
            continue;
        }

        if (window.eval('/\\/' + departmentId + '\\/$/').test(jsonDataUser[i].dbs_departmentid) == true) {
            items += '<li>';
            items += '<div class=\"item\" onClick=\"radioSelect(' + jsonDataUser[i].dbs_id + ', event);\">';
            items += '<div class=\"blank\" style=\"width: ' + (24 * level) + 'px;\"></div>';
            items += '<div class=\"state none\"></div>';
            items += '<div class=\"box\"><input name=\"id\" type=\"radio\" class=\"radio\" value=\"' + jsonDataUser[i].dbs_id + '\" /></div>';
            items += '<div class=\"icon\"><img src=\"/ui/images/select-user-icon.png\" width=\"16\" /></div>';
            items += '<div class=\"name\">' + jsonDataUser[i].dbs_username + '&nbsp;&nbsp;<font color=\"#757575\">' + jsonDataUser[i].dbs_position + '</font></div>';
            items += '</div>';
            items += '<div class=\"node\"></div>';
            items += '</li>';
        }
    }

    if (items.length == 0) {
        return '';
    } else {
        return '<ul id=\"' + departmentId + '\">' + items + '</ul>';
    }
}


// 项目选择
function itemSelect(id, event) {
    var event = event || window.event;
    var tag = event.target || event.srcElement;
    var radios = $name('id');
    var states = $class('state');
    var nodes = $class('node');
    var mark;
    var i;

    for (i = 0; i < radios.length; i++) {
        if (radios[i].$get('class') == 'none' && radios[i].value == id) {
            if (tag.tagName.toLowerCase() != 'input') {
                if (typeof(states[i]) == 'undefined') {
                    continue;
                }

                mark = states[i].$tag('img')[0];

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
}


// 单选框选择
function radioSelect(id, event) {
    var event = event || window.event;
    var tag = event.target || event.srcElement;
    var radios = $name('id');
    var items = $class('item');
    var i;

    for (i = 0; i < radios.length; i++) {
        if (radios[i].$get('class') == 'radio' && radios[i].value == id) {
            if (tag.tagName.toLowerCase() == 'input') {
                if (radios[i].checked == true) {
                    items[i].style.backgroundColor = '#E0E0E0';
                } else {
                    items[i].style.backgroundColor = '';
                }
            } else {
                if (radios[i].checked == false) {
                    radios[i].checked = true;

                    items[i].style.backgroundColor = '#E0E0E0';
                } else {
                    radios[i].checked = false;

                    items[i].style.backgroundColor = '';
                }
            }
        } else {
            items[i].style.backgroundColor = '';
        }
    }
}


// 用户移交(询问)
function userTransfer() {
    var radios = $name('id');
    var id = '';
    var i;

    for (i = 0; i < radios.length; i++) {
        if (radios[i].checked == true) {
            id = radios[i].value;
            break;
        }
    }

    if (id.length == 0) {
        fastui.textTips(lang.user.tips['please-select-user']);
        return;
    }

    fastui.dialogConfirm(lang.user.tips.confirm['transfer'], 'userTransferOk(' + id + ')');
}


// 用户移交(提交)
function userTransferOk(toUserId) {
    var fromUserId = $query('id');
    var data = '';

    data += 'fromuserid=' + fromUserId + '&';
    data += 'touserid=' + toUserId + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/admin/user/user-action.ashx?action=transfer', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                window.parent.fastui.iconTips('tick');

                window.setTimeout(function() {
                    window.parent.fastui.windowClose('user-transfer');
                    }, 500);
            } else {
                fastui.textTips(lang.user.tips['operation-failed']);
            }
            }
        });
}