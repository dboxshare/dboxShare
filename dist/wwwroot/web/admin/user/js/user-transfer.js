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
    var items = '';

    if (typeof(departmentDataJson) == 'undefined') {
        for (var i = 0; i < userDataJson.length; i++) {
            if (userDataJson[i].ds_id == $query('id')) {
                continue;
            }

            items += '<li>';
            items += '<div class="item" onClick="radioSelect(' + userDataJson[i].ds_id + ', event);">';
            items += '<div class="blank"></div>';
            items += '<div class="state none"></div>';
            items += '<div class="box"><input name="id" type="radio" value="' + userDataJson[i].ds_id + '" optional="true" /><label name="radio"></label></div>';
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
                items += '<div class="box none"><input name="id" type="radio" value="' + departmentDataJson[i].ds_id + '" optional="false" /></div>';
                items += '<div class="icon"><img src="/ui/images/select-department-icon.png" width="16" /></div>';
                items += '<div class="name">' + departmentDataJson[i].ds_name + '</div>';
                items += '</div>';
                items += '<div class="node">' + node + '</div>';
                items += '</li>';
            }
        }
    }

    $id('container').innerHTML = '<ul>' + items + '</ul>';
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
            items += '<div class="box none"><input name="id" type="radio" value="' + departmentDataJson[i].ds_id + '" optional="false" /></div>';
            items += '<div class="icon"><img src="/ui/images/select-department-icon.png" width="16" /></div>';
            items += '<div class="name">' + departmentDataJson[i].ds_name + '</div>';
            items += '</div>';
            items += '<div class="node">' + node + '</div>';
            items += '</li>';
        }
    }

    var userId = $query('id');

    for (var j = 0; j < userDataJson.length; j++) {
        if (userDataJson[j].ds_id == userId) {
            continue;
        }

        if (window.eval('/\\/' + departmentId + '\\/$/').test(userDataJson[j].ds_departmentid) == true) {
            items += '<li>';
            items += '<div class="item" onClick="radioSelect(' + userDataJson[j].ds_id + ', event);">';
            items += '<div class="blank" style="width: ' + (24 * level) + 'px;"></div>';
            items += '<div class="state none"></div>';
            items += '<div class="box"><input name="id" type="radio" value="' + userDataJson[j].ds_id + '" optional="true" /><label name="radio"></label></div>';
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


// 项目选择
function itemSelect(id, _event) {
    var event = _event || window.event;
    var tag = event.target || event.srcElement;
    var radios = $name('id');
    var states = $class('state');
    var nodes = $class('node');

    if ($get(tag, 'name') == 'radio') {
        return;
    }

    for (var i = 0; i < radios.length; i++) {
        if (radios[i].$get('optional') == 'false' && radios[i].value == id) {
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


// 单选框选择
function radioSelect(id, _event) {
    var event = _event || window.event;
    var tag = event.target || event.srcElement;
    var radios = $name('id');
    var items = $class('item');

    for (var i = 0; i < radios.length; i++) {
        if (radios[i].$get('optional') == 'true' && radios[i].value == id) {
            if (radios[i].checked == false) {
                radios[i].checked = true;

                items[i].style.backgroundColor = '#E0E0E0';
            } else {
                radios[i].checked = false;

                items[i].style.backgroundColor = '';
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

    for (var i = 0; i < radios.length; i++) {
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
        url: '/api/admin/user/transfer', 
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