// 页面初始化事件调用函数
function init() {
    pageResize();
    dataLoad();
}


// 页面调整
function pageResize() {
    var container = $id('container');
    var footer = $id('footer');

    container.style.height = (container.offsetHeight - footer.offsetHeight) + 'px';
}


// 文件夹数据载入
function dataLoad() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/folder/list-json' + window.location.search, 
        async: true, 
        callback: function(data) {
            if (data.length > 0 && data.indexOf('[]') == -1) {
                $json('jsonData', '{items:' + data + '}');

                folderTree();

                recentSelectPath();
            }
            }
        });
}


// 生成文件夹树形目录(主体)
function folderTree() {
    var container = $id('container');
    var folderId = $query('folderid') || 0;
    var position = $query('position') || '';
    var source = $query('source') || 'true';
    var lock = $query('lock') || 'true';
    var items = '';

    container.innerHTML = '';

    items += '<li>';
    items += '<input name="id" type="radio" class="none" value="0" />';
    items += '<div class="item" onClick="folderTreeSelect(0, event);">';
    items += '<div class="state none"></div>';
    items += '<div class="icon"><img src="/ui/images/select-root-icon.png" width="16" /></div>';
    items += '<div class="name">' + lang.file['all'] + '</div>';
    items += '</div>';
    items += '<div class="form"></div>';
    items += '<div class="node"></div>';
    items += '</li>';

    for (var i = 0; i < jsonData.items.length; i++) {
          if (jsonData.items[i].ds_folderid == 0) {
            if (lock == 'false') {
                if (jsonData.items[i].ds_lock == 1) {
                    continue;
                }
            }

            var node = folderTreeNode(jsonData.items[i].ds_id, folderId, source, lock, 1);

            items += '<li>';
            items += '<input name="id" type="radio" class="none" value="' + jsonData.items[i].ds_id + '" />';
            items += '<div class="item" onClick="folderTreeSelect(' + jsonData.items[i].ds_id + ', event);">';
            items += '<div class="state">';

            if (node.length > 0) {
                items += '<img src="/ui/images/select-node-close-icon.png" width="16" />';
            }

            items += '</div>';
            items += '<div class="icon"><img src="/ui/images/select-folder-icon.png" width="16" /></div>';
            items += '<div class="name">' + jsonData.items[i].ds_name + '</div>';
            items += '</div>';
            items += '<div class="form"></div>';
            items += '<div class="node">' + node + '</div>';
            items += '</li>';
        }
    }

    container.innerHTML = '<ul>' + items + '</ul>';

    folderTreeSelected(position);
}


// 生成文件夹树形目录(节点)
function folderTreeNode(id, folderId, source, lock, level) {
    var items = '';

    for (var i = 0; i < jsonData.items.length; i++) {
          if (jsonData.items[i].ds_folderid == id) {
            if (lock == 'false') {
                if (jsonData.items[i].ds_lock == 1) {
                    continue;
                }
            }

            var node = folderTreeNode(jsonData.items[i].ds_id, folderId, source, lock, level + 1);

            items += '<li>';
            items += '<input name="id" type="radio" class="none" value="' + jsonData.items[i].ds_id + '" />';
            items += '<div class="item" onClick="folderTreeSelect(' + jsonData.items[i].ds_id + ', event);">';
            items += '<div class="blank" style="width: ' + (24 * level) + 'px;"></div>';
            items += '<div class="state">';

            if (node.length > 0) {
                items += '<img src="/ui/images/select-node-close-icon.png" width="16" />';
            }

            items += '</div>';
            items += '<div class="icon"><img src="/ui/images/select-folder-icon.png" width="16" /></div>';
            items += '<div class="name">' + jsonData.items[i].ds_name + '</div>';
            items += '</div>';
            items += '<div class="form" style="padding-left: ' + ((24 * level) + 56) + 'px;"></div>';
            items += '<div class="node">' + node + '</div>';
            items += '</li>';
        }
    }

    if (items.length == 0) {
        return '';
    } else {
        return '<ul id="' + folderId + '">' + items + '</ul>';
    }
}


// 文件夹树形项目选择(手动选择)
function folderTreeSelect(folderId, _event) {
    var event = _event || window.event;
    var tag = event.target || event.srcElement;
    var radios = $name('id');
    var items = $class('item');
    var states = $class('state');
    var nodes = $class('node');

    if (tag.tagName.toLowerCase() == 'a') {
        return;
    }

    for (var i = 0; i < radios.length; i++) {
        if (radios[i].value == folderId) {
            radios[i].checked = true;

            items[i].style.backgroundColor = '#E0E0E0';

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
        } else {
            radios[i].checked = false;

            items[i].style.backgroundColor = '';
        }
    }
}


// 文件夹树形项目选择(自动选择上次已选中项目)
function folderTreeSelected(position) {
    if (position.length == 0) {
        return;
    }

    var folders = position.split(',');
    var radios = $name('id');
    var items = $class('item');

    for (var i = 0; i < folders.length; i++) {
        for (var j = 0; j < radios.length; j++) {
            if (folders[i] == radios[j].value) {
                items[j].click();
                break;
            }
        }
    }
}


// 新建文件夹表单(生成)
function folderCreate() {
    var radios = $name('id');
    var index = 0;
    var html = '';

    for (var i = 0; i < radios.length; i++) {
        if (radios[i].checked == true) {
            index = i;
            break;
        }
    }

    if ($id('create') != null) {
        create.$remove();
    }

    if (index == 0) {
        html += '<div id="create" style="padding-left: 56px;">';
        html += '<input name="name" type="text" id="name" value="' + lang.file.tips.value['new-folder'] + '" maxlength="50" />';
        html += '<span class="button" onClick="folderCreateOk(0);">' + lang.file.button['new'] + '</span>';
        html += '</div>';

        $id('container').innerHTML += html;
    } else {
        html += '<div id="create">';
        html += '<input name="name" type="text" id="name" value="' + lang.file.tips.value['new-folder'] + '" maxlength="50" />';
        html += '<span class="button" onClick="folderCreateOk(' + radios[index].value + ');">' + lang.file.button['new'] + '</span>';
        html += '</div>';

        $class('form')[index].innerHTML += html;
    }

    $id('name').focus();
    $id('name').select();
}


// 新建文件夹表单(提交)
function folderCreateOk(folderId) {
    var name = $id('name').value;
    var data = '';

    data += 'folderid=' + folderId + '&';

    if (fastui.testInput(true, 'name', /^[^\\\/\:\*\?\"\<\>\|]{1,50}$/) == false) {
        fastui.inputTips('name', lang.file.tips.input['name']);
        return;
    } else {
        data += 'name=' + escape(name) + '&';
    }

    data += 'inherit=true&';

    data = data.substring(0, data.length - 1);

    $ajax({
        type: 'POST', 
        url: '/api/drive/folder/add', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                dataLoad();

                window.parent.folderTreeReload();
            } else if (data == 'existed') {
                fastui.inputTips('name', lang.file.tips['name-existed']);
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 最近选择目录
function recentSelectPath() {
    var selectId = $cookie('recent-select-folder-id');
    var folderId = 0;
    var folderPath = '';

    if (selectId.length == 0) {
        return;
    }

    for (var i = 0; i < jsonData.items.length; i++) {
          if (jsonData.items[i].ds_id == selectId) {
            folderId = jsonData.items[i].ds_folderid;
            folderPath = ' / ' + jsonData.items[i].ds_name;
            break;
        }
    }

    while (folderId > 0) {
        for (var j = 0; j < jsonData.items.length; j++) {
            if (jsonData.items[j].ds_id == folderId) {
                folderId = jsonData.items[j].ds_folderid;
                folderPath = ' / ' + jsonData.items[j].ds_name + folderPath;
                break;
            }
        }
    }

    if (folderPath.length > 0) {
        $id('recent-select').innerHTML = '<input name="recent-select-id" type="checkbox" id="recent-select-id" value="' + selectId + '" /><label for="recent-select-id"></label>&nbsp;&nbsp;<label for="recent-select-id"><font color="#757575">' + folderPath.substring(3, folderPath.length) + '</font></label>';
    }
}


// 提交文件夹选择
function folderSelect() {
    var iframe = $query('iframe');
    var callback = $query('callback');
    var source = $query('source') || 'true';
    var form = iframe.length == 0 ? 'window.parent' : 'window.parent.$id(\'' + iframe + '\').contentWindow';
    var recent = $id('recent-select-id');
    var id = '';
    var name = '';

    if (recent == null ? false : recent.checked == true) {
        for (var i = 0; i < jsonData.items.length; i++) {
              if (jsonData.items[i].ds_id == recent.value) {
                id = jsonData.items[i].ds_id;
                name = jsonData.items[i].ds_name;
                break;
            }
        }
    } else {
        var radios = $name('id');

        for (var i = 0; i < radios.length; i++) {
            if (radios[i].checked == true) {
                id = radios[i].value;
                name = $class('name')[i].innerHTML;
                break;
            }
        }

        if (id.length == 0 || name.length == 0) {
            fastui.textTips(lang.file.tips['please-select-folder']);
            return;
        }
    }

    if (source == 'false') {
        var folderId = $query('folderid') || 0;

        if (folderId == id) {
            fastui.textTips(lang.file.tips['unallow-select-item']);
            return;
        }
    }

    window.eval('var callbackFunction = ' + form + '.' + callback);
    callbackFunction(id, name);

    $cookie('recent-select-folder-id', id, 7);

    window.parent.fastui.windowClose('folder-select');
}