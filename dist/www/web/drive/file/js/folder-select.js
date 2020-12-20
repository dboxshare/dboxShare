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
        url: '/web/drive/file/folder-list-json.ashx' + window.location.search, 
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
    var node;
    var i;

    container.innerHTML = '';

    items += '<li>';
    items += '<input name=\"id\" type=\"radio\" class=\"none\" value=\"0\" />';
    items += '<div class=\"item\" onClick=\"folderTreeSelect(0, event);\">';
    items += '<div class=\"state none\"></div>';
    items += '<div class=\"icon\"><img src=\"/ui/images/select-root-icon.png\" width=\"16\" /></div>';
    items += '<div class=\"name\">' + lang.file['all'] + '</div>';
    items += '</div>';
    items += '<div class=\"form\"></div>';
    items += '<div class=\"node\"></div>';
    items += '</li>';

    for (i = 0; i < jsonData.items.length; i++) {
          if (jsonData.items[i].dbs_folderid == 0) {
            if (lock == 'false') {
                if (jsonData.items[i].dbs_lock == 1) {
                    continue;
                }
            }

            node = folderTreeNode(jsonData.items[i].dbs_id, folderId, source, lock, 1);

            items += '<li>';
            items += '<input name=\"id\" type=\"radio\" class=\"none\" value=\"' + jsonData.items[i].dbs_id + '\" />';
            items += '<div class=\"item\" onClick=\"folderTreeSelect(' + jsonData.items[i].dbs_id + ', event);\">';
            items += '<div class=\"state\">';

            if (node.length > 0) {
                items += '<img src=\"/ui/images/select-node-close-icon.png\" width=\"16\" />';
            }

            items += '</div>';
            items += '<div class=\"icon\"><img src=\"/ui/images/select-folder-icon.png\" width=\"16\" /></div>';
            items += '<div class=\"name\">' + jsonData.items[i].dbs_name + '</div>';
            items += '</div>';
            items += '<div class=\"form\"></div>';
            items += '<div class=\"node\">' + node + '</div>';
            items += '</li>';
        }
    }

    container.innerHTML = '<ul>' + items + '</ul>';

    folderTreeSelected(position);
}


// 生成文件夹树形目录(节点)
function folderTreeNode(id, folderId, source, lock, level) {
    var items = '';
    var node;
    var i;

    for (i = 0; i < jsonData.items.length; i++) {
          if (jsonData.items[i].dbs_folderid == id) {
            if (lock == 'false') {
                if (jsonData.items[i].dbs_lock == 1) {
                    continue;
                }
            }

            node = folderTreeNode(jsonData.items[i].dbs_id, folderId, source, lock, level + 1);

            items += '<li>';
            items += '<input name=\"id\" type=\"radio\" class=\"none\" value=\"' + jsonData.items[i].dbs_id + '\" />';
            items += '<div class=\"item\" onClick=\"folderTreeSelect(' + jsonData.items[i].dbs_id + ', event);\">';
            items += '<div class=\"blank\" style=\"width: ' + (24 * level) + 'px;\"></div>';
            items += '<div class=\"state\">';

            if (node.length > 0) {
                items += '<img src=\"/ui/images/select-node-close-icon.png\" width=\"16\" />';
            }

            items += '</div>';
            items += '<div class=\"icon\"><img src=\"/ui/images/select-folder-icon.png\" width=\"16\" /></div>';
            items += '<div class=\"name\">' + jsonData.items[i].dbs_name + '</div>';
            items += '</div>';
            items += '<div class=\"form\" style=\"padding-left: ' + ((24 * level) + 56) + 'px;\"></div>';
            items += '<div class=\"node\">' + node + '</div>';
            items += '</li>';
        }
    }

    if (items.length == 0) {
        return '';
    } else {
        return '<ul id=\"' + folderId + '\">' + items + '</ul>';
    }
}


// 文件夹树形项目选择(手动选择)
function folderTreeSelect(folderId, event) {
    var event = event || window.event;
    var tag = event.target || event.srcElement;
    var radios = $name('id');
    var items = $class('item');
    var states = $class('state');
    var nodes = $class('node');
    var mark;
    var i;

    if (tag.tagName.toLowerCase() == 'a') {
        return;
    }

    for (i = 0; i < radios.length; i++) {
        if (radios[i].value == folderId) {
            radios[i].checked = true;

            items[i].style.backgroundColor = '#E0E0E0';

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
        } else {
            radios[i].checked = false;

            items[i].style.backgroundColor = '';
        }
    }
}


// 文件夹树形项目选择(自动选择上次已选中项目)
function folderTreeSelected(position) {
    var folders = new Array();
    var radios = $name('id');
    var items = $class('item');
    var i;
    var j;

    if (position.length == 0) {
        return;
    }

    folders = position.split(',');

    for (i = 0; i < folders.length; i++) {
        for (j = 0; j < radios.length; j++) {
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
    var forms = $class('form');
    var create = $id('create');
    var html = '';
    var i;
    var j;

    for (i = 0; i < radios.length; i++) {
        if (radios[i].checked == true) {
            j = i;
            break;
        }
    }

    if (create != null) {
        create.$remove();
    }

    if (typeof(j) == 'undefined') {
        html += '<div id=\"create\" style=\"padding-left: 56px;\">';
        html += '<input name=\"name\" type=\"text\" class=\"input\" id=\"name\" value=\"' + lang.file.tips.value['new-folder'] + '\" maxlength=\"50\" />';
        html += '<span class=\"button\" onClick=\"folderCreateSubmit();\">' + lang.file.button['new'] + '</span>';
        html += '</div>';

        $id('container').innerHTML += html;
    } else {
        html += '<div id=\"create\">';
        html += '<input name=\"name\" type=\"text\" class=\"input\" id=\"name\" value=\"' + lang.file.tips.value['new-folder'] + '\" maxlength=\"50\" />';
        html += '<span class=\"button\" onClick=\"folderCreateSubmit();\">' + lang.file.button['new'] + '</span>';
        html += '</div>';

        forms[j].innerHTML += html;
    }

    $id('name').focus();
    $id('name').select();
}


// 新建文件夹表单(提交)
function folderCreateSubmit() {
    var radios = $name('id');
    var name = $id('name').value;
    var data = '';
    var i;
    var j;

    for (i = 0; i < radios.length; i++) {
        if (radios[i].checked == true) {
            j = i;
            break;
        }
    }

    if (typeof(j) == 'undefined') {
        data += 'folderid=0&';
    } else {
        data += 'folderid=' + radios[i].value + '&';
    }

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
        url: '/web/drive/file/folder-action.ashx?action=add', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                dataLoad();

                window.parent.folderTreeReload();
            } else if (data == 'existed') {
                fastui.inputTips('name', lang.file.tips['name-existed']);
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
    var i;

    if (selectId.length == 0) {
        return;
    }

    for (i = 0; i < jsonData.items.length; i++) {
          if (jsonData.items[i].dbs_id == selectId) {
            folderId = jsonData.items[i].dbs_folderid;
            folderPath = ' / ' + jsonData.items[i].dbs_name;
            break;
        }
    }

    while (folderId > 0) {
        for (i = 0; i < jsonData.items.length; i++) {
            if (jsonData.items[i].dbs_id == folderId) {
                folderId = jsonData.items[i].dbs_folderid;
                folderPath = ' / ' + jsonData.items[i].dbs_name + folderPath;
                break;
            }
        }
    }

    if (folderPath.length > 0) {
        $id('recent-select').innerHTML = '<input name="recent-select-id" type="checkbox" class="checkbox" id="recent-select-id" value="' + selectId + '" />&nbsp;&nbsp;<label for="recent-select-id"><font color=\"#757575\">' + folderPath.substring(3, folderPath.length) + '</font></label>';
    }
}


// 提交文件夹选择
function folderSelect() {
    var folderId = $query('folderid') || 0;
    var iframe = $query('iframe');
    var callback = $query('callback');
    var source = $query('source') || 'true';
    var form = iframe.length == 0 ? 'window.parent' : 'window.parent.$id(\'' + iframe + '\').contentWindow';
    var radios = $name('id');
    var recent = $id('recent-select-id');
    var id = '';
    var name = '';
    var i;

    if (recent == null ? false : recent.checked == true) {
        for (i = 0; i < jsonData.items.length; i++) {
              if (jsonData.items[i].dbs_id == recent.value) {
                id = jsonData.items[i].dbs_id;
                name = jsonData.items[i].dbs_name;
                break;
            }
        }
    } else {
        for (i = 0; i < radios.length; i++) {
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
        if (folderId == id) {
            fastui.textTips(lang.file.tips['unallow-select-item']);
            return;
        }
    }

    window.eval('var _function = ' + form + '.' + callback);
    _function(id, name);

    $cookie('recent-select-folder-id', id, 7);

    window.parent.fastui.windowClose('folder-select');
}