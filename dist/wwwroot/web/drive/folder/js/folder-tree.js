// 页面初始化事件调用函数
function init() {
    dataLoad(false);
}


// 文件夹数据载入
function dataLoad(update) {
    $ajax({
        type: 'GET', 
        url: '/api/drive/folder/list-json' + window.location.search, 
        async: true, 
        callback: function(data) {
            // 文件列表初始化
            if (update == false) {
                window.parent.listInit();
            }

            if (data.length > 0 && data.indexOf('[]') == -1) {
                $json('jsonData', '{items:' + data + '}');

                folderTree();

                if (update == true) {
                    var position = $cookie('recent-folder-position');

                    folderTreeSelected(position);
                }
            }
            }
        });
}


// 生成文件夹树形目录(主体)
function folderTree() {
    var container = $id('container');
    var items = '';

    if (typeof(jsonData) == 'undefined') {
        return;
    }

    container.innerHTML = '';

    for (var i = 0; i < jsonData.items.length; i++) {
          if (jsonData.items[i].ds_folderid == 0) {
            var node = folderTreeNode(jsonData.items[i].ds_id, 1);

            items += '<li>';
            items += '<input name="id" type="radio" class="none" value="' + jsonData.items[i].ds_id + '" />';
            items += '<div class="item" onClick="folderTreeSelect(' + jsonData.items[i].ds_id + ');">';
            items += '<div class="state">';

            if (node.length > 0) {
                items += '<img src="/ui/images/select-node-close-icon.png" width="16" />';
            }

            items += '</div>';
            items += '<div class="icon">';

            if (jsonData.items[i].ds_share == 0) {
                items += '<img src="/ui/images/select-folder-icon.png" width="16" />';
            } else {
                items += '<img src="/ui/images/select-folder-share-icon.png" width="16" />';
            }

            items += '</div>';
            items += '<div class="name"><a href="javascript: folderTreeGoto(' + jsonData.items[i].ds_id + ');">' + jsonData.items[i].ds_name + '</a></div>';
            items += '</div>';
            items += '<div class="node">' + node + '</div>';
            items += '</li>';
        }
    }

    container.innerHTML = '<ul>' + items + '</ul>';
}


// 生成文件夹树形目录(节点)
function folderTreeNode(folderId, level) {
    var items = '';

    for (var i = 0; i < jsonData.items.length; i++) {
          if (jsonData.items[i].ds_folderid == folderId) {
            var node = folderTreeNode(jsonData.items[i].ds_id, level + 1);

            items += '<li>';
            items += '<input name="id" type="radio" class="none" value="' + jsonData.items[i].ds_id + '" />';
            items += '<div class="item" onClick="folderTreeSelect(' + jsonData.items[i].ds_id + ');">';
            items += '<div class="blank" style="width: ' + (24 * level) + 'px;"></div>';
            items += '<div class="state">';

            if (node.length > 0) {
                items += '<img src="/ui/images/select-node-close-icon.png" width="16" />';
            }

            items += '</div>';
            items += '<div class="icon">';

            if (jsonData.items[i].ds_share == 0) {
                items += '<img src="/ui/images/select-folder-icon.png" width="16" />';
            } else {
                items += '<img src="/ui/images/select-folder-share-icon.png" width="16" />';
            }

            items += '</div>';
            items += '<div class="name"><a href="javascript: folderTreeGoto(' + jsonData.items[i].ds_id + ');">' + jsonData.items[i].ds_name + '</a></div>';
            items += '</div>';
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
function folderTreeSelect(folderId) {
    var radios = $name('id');
    var items = $class('item');
    var states = $class('state');
    var nodes = $class('node');

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


// 文件夹树形项目点击转到指定路径
function folderTreeGoto(folderId) {
    var list = window.parent.$id('list-iframe');

    list.src = $url('/web/drive/file/file-list.html?folderid=' + folderId);
}