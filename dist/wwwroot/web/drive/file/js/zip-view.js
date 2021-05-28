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


// 压缩包数据载入
function dataLoad() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/file/zip-data-json?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if (data.length > 0 && data.indexOf('[]') == -1) {
                $json('jsonData', '{items:' + data + '}');

                zipTree();
            }

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 生成压缩包文件树形目录(主体)
function zipTree() {
    if (jsonData.items.length == 0) {
        fastui.coverTips(lang.file.tips['zip-file-error']);
        return;
    }

    $id('container').innerHTML = zipTreeNode('', 0);
}


// 生成压缩包文件树形目录(节点)
function zipTreeNode(path, level) {
    var items = '';

    for (var i = 0; i < jsonData.items.length; i++) {
        if (window.eval('/^' + $jsonEscape(path) + '[^\\/]+\\/$/').test(jsonData.items[i].path) == true) {
            var node = zipTreeNode(jsonData.items[i].path, level + 1);

            items += '<li>';
            items += '<div class="item" onClick="itemSelect(\'' + jsonData.items[i].path + '\', event);">';
            items += '<div class="blank" style="width: ' + (24 * level) + 'px;"></div>';
            items += '<div class="state">';

            if (node.length > 0) {
                items += '<img src="/ui/images/select-node-close-icon.png" width="16" />';
            }

            items += '</div>';
            items += '<div class="box"><input name="path" type="checkbox" id="path_' + i + '" value="' + jsonData.items[i].path + '" onClick="checkboxSelect(\'' + jsonData.items[i].path + '\', true, event);" /><label for="path_' + i + '"></label></div>';
            items += '<div class="icon"><img src="/ui/images/select-folder-icon.png" width="16" /></div>';
            items += '<div class="name">' + jsonData.items[i].path.match(/\/?([^\/]+)\/$/)[1] + '</div>';
            items += '</div>';
            items += '<div class="node">' + node + '</div>';
            items += '</li>';
        }
    }

    for (var j = 0; j < jsonData.items.length; j++) {
        if (window.eval('/^' + $jsonEscape(path) + '[^\\/]+$/').test(jsonData.items[j].path) == true) {
            items += '<li>';
            items += '<div class="item" onClick="checkboxSelect(\'' + jsonData.items[j].path + '\', false, event);">';
            items += '<div class="blank" style="width: ' + (24 * level) + 'px;"></div>';
            items += '<div class="state none"></div>';
            items += '<div class="box"><input name="path" type="checkbox" id="path_' + i + '" value="' + jsonData.items[j].path + '" /><label for="path_' + i + '"></label></div>';
            items += '<div class="icon"><img src="/ui/images/select-file-icon.png" width="16" /></div>';
            items += '<div class="name">' + jsonData.items[j].path.match(/\/?([^\/]+)$/)[1] + '&nbsp;&nbsp;<font color="#757575">' + fileSize(jsonData.items[j].size) + '</font></div>';
            items += '</div>';
            items += '<div class="node"></div>';
            items += '</li>';
        }
    }

    if (items.length == 0) {
        return '';
    } else {
        return '<ul id="' + path + '">' + items + '</ul>';
    }
}


// 项目选择
function itemSelect(path, e) {
    var event = e || window.event;
    var tag = event.target || event.srcElement;
    var checkboxes = $name('path');
    var states = $class('state');
    var nodes = $class('node');

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].value == path) {
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
        }
    }
}


// 复选框选择
function checkboxSelect(path, folder, e) {
    var event = e || window.event;
    var tag = event.target || event.srcElement;
    var checkboxes = $name('path');
    var items = $class('item');
    var nodes = $class('node');

    if (tag.tagName.toLowerCase() == 'img') {
        if (tag.src.indexOf('select-node-open-icon') > 0 || tag.src.indexOf('select-node-close-icon') > 0) {
            return;
        }
    }

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].value == path) {
            if (tag.tagName.toLowerCase() == 'input') {
                if (checkboxes[i].checked == true) {
                    items[i].style.backgroundColor = '#E0E0E0';
                } else {
                    items[i].style.backgroundColor = '';
                }
            } else {
                if (checkboxes[i].checked == false) {
                    checkboxes[i].checked = true;

                    items[i].style.backgroundColor = '#E0E0E0';
                } else {
                    checkboxes[i].checked = false;

                    items[i].style.backgroundColor = '';
                }
            }

            if (folder == true) {
                var subboxes = nodes[i].$tag('input');
                var subitems = nodes[i].$class('item');

                for (var j = 0; j < subboxes.length; j++) {
                    if (checkboxes[i].checked == true) {
                        subboxes[j].checked = true;

                        subitems[j].style.backgroundColor = '#E0E0E0';
                    } else {
                        subboxes[j].checked = false;

                        subitems[j].style.backgroundColor = '';
                    }
                }
            }
        }
    }
}


// 提交下载选择(打包)
function downloadSelect() {
    var checkboxes = $name('path');
    var id = $query('id');
    var password = $id('password').value;
    var data = '';

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            var index = zipIndex(checkboxes[i].value);

            if (index == -1) {
                continue;
            }

            data += 'item=' + index + '&';
        }
    }

    if (data.length == 0) {
        fastui.textTips(lang.file.tips['please-select-item']);
        return;
    } else {
        data = data.substring(0, data.length - 1);
    }

    fastui.windowPopup('zip-file-download', '', '/web/drive/file/zip-file-download.html?id=' + id + '&password=' + password + '&' + data, 400, 140);
}


// 提交解压选择(入库)
function unpackSelect() {
    var checkboxes = $name('path');
    var id = $query('id');
    var password = $id('password').value;
    var data = '';

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            var index = zipIndex(checkboxes[i].value);

            if (index == -1) {
                continue;
            }

            data += 'item=' + index + '&';
        }
    }

    if (data.length == 0) {
        fastui.textTips(lang.file.tips['please-select-item']);
        return;
    } else {
        data = data.substring(0, data.length - 1);
    }

    fastui.windowPopup('zip-file-unpack', '', '/web/drive/file/zip-file-unpack.html?id=' + id + '&password=' + password + '&' + data, 400, 140);
}


// 解压完成(提示)
function unpackComplete() {
    fastui.textTips(lang.file.tips['unpack-complete'], 0);
}


// 解压完成(转到文件列表根目录)
function unpackComplete_GotoView() {
    window.parent.$location('/web/drive/file/file-list.html');
}


// 获取压缩包文件索引
function zipIndex(path) {
    for (var i = 0; i < jsonData.items.length; i++) {
        if (jsonData.items[i].path == path) {
            return i;
        }
    }

    return -1;
}


// 获取压缩包文件大小
function fileSize(byte) {
    if (Math.ceil(byte / 1024) < 1024) {
        return Math.ceil(byte / 1024) + ' KB';
    } else if (Math.ceil(byte / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024).toFixed(2) + ' MB';
    } else if (Math.ceil(byte / 1024 / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024 / 1024).toFixed(2) + ' GB';
    } else {
        return (byte / 1024 / 1024 / 1024).toFixed(2) + ' GB';
    }
}


// 压缩包解压密码(生成表单)
function unzipKey(action) {
    var layer = $id('unzip-key-box');
    var html = '';

    if (layer != null) {
        return;
    }

    fastui.coverShow('page-cover');

    html += '<div class="form">';
    html += '<input name="key" type="text" id="key" value="' + lang.file.tips.value['unzip-key'] + '" maxlength="32" />';
    html += '<span class="button-ok" onClick="unzipKeyOk(\'' + action + '\');">' + lang.file.button['confirm'] + '</span>';
    html += '<span class="button-cancel" onClick="unzipKeyCancel();">' + lang.file.button['cancel'] + '</span>';
    html += '</div>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'unzip-key-box'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil(($page().client.height - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil(($page().client.width - layer.offsetWidth) / 2) + 'px';

    $id('key').focus();
    $id('key').select();
}


// 压缩包解压密码(表单提交)
function unzipKeyOk(action) {
    var key = $id('key').value;

    if (key.length == 0) {
        fastui.inputTips('key', lang.file.tips.input['unzip-key']);
        return;
    }

    $id('password').value = key;

    $id('unzip-key-box').$remove();

    fastui.coverHide('page-cover');

    if (action == 'download') {
        $id('button-download').click();
    } else if (action == 'unpack') {
        $id('button-unpack').click();
    }
}


// 压缩包解压密码(取消)
function unzipKeyCancel() {
    $id('unzip-key-box').$remove();

    fastui.coverHide('page-cover');
}