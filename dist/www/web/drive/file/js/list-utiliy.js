var windowObject = window.location.pathname.indexOf('file-version') == -1 ? window.self : window.parent;


// 权限校验
function purviewCheck(purview, role) {
    switch(purview) {
        case 'viewer':
        purview = 1;
        break;

        case 'downloader':
        purview = 2;
        break;

        case 'uploader':
        purview = 3;
        break;

        case 'editor':
        purview = 4;
        break;

        case 'manager':
        purview = 5;
        break;

        case 'creator':
        purview = 6;
        break;
    }

    switch(role) {
        case 'viewer':
        role = 1;
        break;

        case 'downloader':
        role = 2;
        break;

        case 'uploader':
        role = 3;
        break;

        case 'editor':
        role = 4;
        break;

        case 'manager':
        role = 5;
        break;

        case 'creator':
        role = 6;
        break;
    }

    if (purview >= role) {
        return true;
    } else {
        return false;
    }
}


// 数据列表自定义菜单事件(鼠标右击)
function dataListContextMenuEvent(j) {
    var container = $id('datalist-container');
    var rows = $id('data-list').$tag('tr');
    var checkboxes = $name('id');
    var i;

    // 屏蔽浏览器右击菜单
    container.oncontextmenu = function() {
        var selection = window.getSelection ? window.getSelection().toString() : document.selection.createRange().text;

        if (selection.length == 0) {
            return false;
        }
        };

    for (i = j; i < rows.length; i++) {
        (function(index) {rows[i].oncontextmenu = function(event) {
            var event = event || window.event;
            var clientX = event.clientX;
            var clientY = event.clientY;
            var selection = window.getSelection ? window.getSelection().toString() : document.selection.createRange().text;
            var folderId = $query('folderId');
            var role = 'viewer';
            var url = '/web/drive/purview/purview-role.ashx?id=' + jsonData.items[index].dbs_id + '&folder=' + (jsonData.items[index].dbs_folder == 1 ? true : false);

            clientX = clientX - $left(container);

            if (selection.length > 0) {
                return;
            }

            // 判断是否创建者
            if (jsonData.items[index].dbs_userid == window.top.myId || jsonData.items[index].dbs_createusername == window.top.myUsername) {
                dataListContextMenuView(container, rows, checkboxes, index, 'creator', clientX, clientY);
                return;
            }

            // 判断是否需要获取用户角色
            if (folderId.length == 0 || folderId == 0) {
                // 获取用户角色
                $ajax({
                    type: 'GET', 
                    url: url, 
                    async: true, 
                    callback: function(data) {
                        if (data.length == 0) {
                            fastui.windowPopup('login', '', '/web/user/login.html', 350, 350);
                            return;
                        } else {
                            role = data;
                        }

                        dataListContextMenuView(container, rows, checkboxes, index, role, clientX, clientY);
                        }
                    });
            } else {
                dataListContextMenuView(container, rows, checkboxes, index, purview, clientX, clientY);
            }
            };})(i);
    }
}


// 数据列表自定义菜单视图(鼠标右击)
function dataListContextMenuView(container, rows, checkboxes, index, role, clientX, clientY) {
    var folder = jsonData.items[index].dbs_folder == 1 ? true : false;
    var share = jsonData.items[index].dbs_share;
    var lock = jsonData.items[index].dbs_lock;
    var layer = null;
    var count = 0;

    // 根据用户角色显示自定义菜单项目
    dataListContextMenuShow(folder, share, lock, role);

    // 判断列表项目是否集合操作
    if (checkboxes[index].checked == true) {
        for (i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked == true) {
                count++;
            }

            if (count > 1) {
                break;
            }
        }

        if (count > 1) {
            if (typeof(purview) == 'undefined') {
                dataListContextMenuCollection(container, clientX, clientY);
                return;
            } else {
                if (purviewCheck(purview, 'manager') == true) {
                    dataListContextMenuCollection(container, clientX, clientY);
                    return;
                } else {
                    return;
                }
            }
        }
    }

    // 判断自定义菜单类型(文件夹或文件)
    if (folder == true) {
        layer = $id('folder-context-menu');
    } else {
        layer = $id('file-context-menu');
    }

    layer.innerHTML = layer.innerHTML.replace(/\(\d*\)/g, '(' + index + ')');

    layer.style.display = 'block';
    layer.style.visibility = 'hidden';

    if (container.clientHeight - clientY > layer.offsetHeight) {
        layer.style.top = (clientY - 16) + 'px';
    } else {
        layer.style.top = (clientY - layer.offsetHeight + 16) + 'px';
    }

    if (layer.offsetTop < container.offsetTop) {
        layer.style.top = container.offsetTop + 'px';
    }

    if (container.clientWidth - clientX > layer.offsetWidth) {
        layer.style.left = (clientX - 16) + 'px';
    } else {
        layer.style.left = (clientX - layer.offsetWidth + 16) + 'px';
    }

    if (layer.offsetLeft < 0) {
        layer.style.left = '0px';
    }

    layer.style.visibility = 'visible';

    rows[index].style.backgroundColor = '#E0E0E0';

    (function(idx) {layer.onmouseenter = function() {
        layer.style.display = 'block';

        rows[idx].style.backgroundColor = '#E0E0E0';
        };})(index);

    (function(idx) {layer.onmouseleave = function() {
        layer.style.display = 'none';

        if (checkboxes[idx].checked == false) {
            rows[idx].style.backgroundColor = '';
        }
        };})(index);

    (function(idx) {layer.onclick = function() {
        layer.style.display = 'none';

        if (checkboxes[idx].checked == false) {
            rows[idx].style.backgroundColor = '';
        }
        };})(index);
}


// 数据列表自定义菜单集合(鼠标右击)
function dataListContextMenuCollection(container, clientX, clientY) {
    var layer = $id('context-menu-collection');

    if (layer == null) {
        return;
    }

    layer.style.display = 'block';
    layer.style.visibility = 'hidden';

    if (container.clientHeight - clientY > layer.offsetHeight) {
        layer.style.top = (clientY - 16) + 'px';
    } else {
        layer.style.top = (clientY - layer.offsetHeight + 16) + 'px';
    }

    if (layer.offsetTop < container.offsetTop) {
        layer.style.top = container.offsetTop + 'px';
    }

    if (container.clientWidth - clientX > layer.offsetWidth) {
        layer.style.left = (clientX - 16) + 'px';
    } else {
        layer.style.left = (clientX - layer.offsetWidth + 16) + 'px';
    }

    if (layer.offsetLeft < 0) {
        layer.style.left = '0px';
    }

    layer.style.visibility = 'visible';

    layer.onmouseenter = function() {
        layer.style.display = 'block';
        };

    layer.onmouseleave = function() {
        layer.style.display = 'none';
        };

    layer.onclick = function() {
        layer.style.display = 'none';
        };
}


// 数据列表自定义菜单事件(鼠标点击)
function dataListContextMenuClickEvent(index, event) {
    var event = event || window.event;
    var clientX = event.clientX;
    var clientY = event.clientY;
    var container = $id('datalist-container');
    var rows = $id('data-list').$tag('tr');
    var checkboxes = $name('id');
    var folderId = $query('folderId');
    var role = 'viewer';
    var url = '/web/drive/purview/purview-role.ashx?id=' + jsonData.items[index].dbs_id + '&folder=' + (jsonData.items[index].dbs_folder == 1 ? true : false);

    clientX = clientX - $left(container);

    // 判断是否创建者
    if (jsonData.items[index].dbs_userid == window.top.myId || jsonData.items[index].dbs_createusername == window.top.myUsername) {
        dataListContextMenuView(container, rows, checkboxes, index, 'creator', clientX, clientY);
        return;
    }

    // 判断是否需要获取用户角色
    if (folderId.length == 0 || folderId == 0) {
        // 获取用户角色
        $ajax({
            type: 'GET', 
            url: url, 
            async: true, 
            callback: function(data) {
                if (data.length == 0) {
                    fastui.windowPopup('login', '', '/web/user/login.html', 350, 350);
                    return;
                } else {
                    role = data;
                }

                dataListContextMenuClickView(container, rows, checkboxes, index, role, clientX, clientY);
                }
            });
    } else {
        dataListContextMenuClickView(container, rows, checkboxes, index, purview, clientX, clientY);
    }
}


// 数据列表自定义菜单视图(鼠标点击)
function dataListContextMenuClickView(container, rows, checkboxes, index, role, clientX, clientY) {
    var id = jsonData.items[index].dbs_id;
    var folder = jsonData.items[index].dbs_folder == 1 ? true : false;
    var share = jsonData.items[index].dbs_share;
    var lock = jsonData.items[index].dbs_lock;
    var layer = null;

    // 根据用户角色显示自定义菜单项目
    dataListContextMenuShow(folder, share, lock, role);

    if (folder == true) {
        layer = $id('folder-context-menu');
    } else {
        layer = $id('file-context-menu');
    }

    layer.innerHTML = layer.innerHTML.replace(/\(\d*\)/g, '(' + index + ')');

    layer.style.display = 'block';
    layer.style.visibility = 'hidden';

    if (container.clientHeight - clientY > layer.offsetHeight) {
        layer.style.top = (clientY - 16) + 'px';
    } else {
        layer.style.top = (clientY - layer.offsetHeight + 16) + 'px';
    }

    if (layer.offsetTop < container.offsetTop) {
        layer.style.top = container.offsetTop + 'px';
    }

    if (container.clientWidth - clientX > layer.offsetWidth) {
        layer.style.left = (clientX - 16) + 'px';
    } else {
        layer.style.left = (clientX - layer.offsetWidth + 16) + 'px';
    }

    if (layer.offsetLeft < 0) {
        layer.style.left = '0px';
    }

    layer.style.visibility = 'visible';

    layer.onmouseenter = function() {
        layer.style.display = 'block';

        rows[index].style.backgroundColor = '#E0E0E0';
        };

    layer.onmouseleave = function() {
        layer.style.display = 'none';

        if (checkboxes[index].checked == false) {
            rows[index].style.backgroundColor = '';
        }
        };

    layer.onclick = function() {
        layer.style.display = 'none';

        if (checkboxes[index].checked == false) {
            rows[index].style.backgroundColor = '';
        }
        };
}


// 数据列表自定义菜单项目显示
function dataListContextMenuShow(folder, share, lock, role) {
    switch(role) {
        case 'viewer':
        role = 1;
        break;

        case 'downloader':
        role = 2;
        break;

        case 'uploader':
        role = 3;
        break;

        case 'editor':
        role = 4;
        break;

        case 'manager':
        role = 5;
        break;

        case 'creator':
        role = 6;
        break;
    }

    var viewer = role >= 1 ? 'block' : 'none';
    var downloader = role >= 2 ? 'block' : 'none';
    var uploader = role >= 3 ? 'block' : 'none';
    var editor = role >= 4 ? 'block' : 'none';
    var manager = role >= 5 ? 'block' : 'none';
    var creator = role >= 6 ? 'block' : 'none';

    if (folder == true) {
        if ($id('folder-context-menu-lock') != null) {
            $id('folder-context-menu-lock').style.display = lock == 1 ? 'none' : creator;
        }

        if ($id('folder-context-menu-unlock') != null) {
            $id('folder-context-menu-unlock').style.display = lock == 0 ? 'none' : creator;
        }

        if ($id('folder-context-menu-rename') != null) {
            $id('folder-context-menu-rename').style.display = lock == 1 ? 'none' : creator;
        }

        if ($id('folder-context-menu-remark') != null) {
            $id('folder-context-menu-remark').style.display = lock == 1 ? 'none' : manager;
        }

        if ($id('folder-context-menu-purview') != null) {
            $id('folder-context-menu-purview').style.display = creator;
        }

        if ($id('folder-context-menu-move') != null) {
            $id('folder-context-menu-move').style.display = lock == 1 ? 'none' : creator;
        }

        if ($id('folder-context-menu-remove') != null) {
            $id('folder-context-menu-remove').style.display = lock == 1 ? 'none' : creator;
        }

        if ($id('folder-context-menu-restore') != null) {
            $id('folder-context-menu-restore').style.display = creator;
        }

        if ($id('folder-context-menu-delete') != null) {
            $id('folder-context-menu-delete').style.display = creator;
        }

        if ($id('folder-context-menu-task') != null) {
            $id('folder-context-menu-task').style.display = share == 1 && lock == 0 ? manager : 'none';
        }

        if ($id('folder-context-menu-discuss') != null) {
            $id('folder-context-menu-discuss').style.display = share == 1 && lock == 0 ? 'block' : 'none';
        }
    } else {
        if ($id('file-context-menu-download') != null) {
            $id('file-context-menu-download').style.display = downloader;
        }

        if ($id('file-context-menu-lock') != null) {
            $id('file-context-menu-lock').style.display = lock == 1 ? 'none' : creator;
        }

        if ($id('file-context-menu-unlock') != null) {
            $id('file-context-menu-unlock').style.display = lock == 0 ? 'none' : creator;
        }

        if ($id('file-context-menu-rename') != null) {
            $id('file-context-menu-rename').style.display = lock == 1 ? 'none' : manager;
        }

        if ($id('file-context-menu-remark') != null) {
            $id('file-context-menu-remark').style.display = lock == 1 ? 'none' : manager;
        }

        if ($id('file-context-menu-copy') != null) {
            $id('file-context-menu-copy').style.display = lock == 1 ? 'none' : editor;
        }

        if ($id('file-context-menu-move') != null) {
            $id('file-context-menu-move').style.display = lock == 1 ? 'none' : manager;
        }

        if ($id('file-context-menu-remove') != null) {
            $id('file-context-menu-remove').style.display = lock == 1 ? 'none' : manager;
        }

        if ($id('file-context-menu-restore') != null) {
            $id('file-context-menu-restore').style.display = manager;
        }

        if ($id('file-context-menu-delete') != null) {
            $id('file-context-menu-delete').style.display = creator;
        }

        if ($id('file-context-menu-task') != null) {
            $id('file-context-menu-task').style.display = share == 1 && lock == 0 ? 'block' : 'none';
        }

        if ($id('file-context-menu-discuss') != null) {
            $id('file-context-menu-discuss').style.display = share == 1 && lock == 0 ? 'block' : 'none';
        }

        if ($id('file-context-menu-version') != null) {
            $id('file-context-menu-version').style.display = lock == 1 ? 'none' : editor;
        }

        if ($id('file-context-menu-upversion') != null) {
            $id('file-context-menu-upversion').style.display = lock == 1 ? 'none' : editor;
        }

        if ($id('file-context-menu-replace') != null) {
            $id('file-context-menu-replace').style.display = lock == 1 ? 'none' : creator;
        }
    }
}


// 获取文件图标
function fileIcon(extension) {
    var extensions = new Array();
    var type;
    var i;

    if (extension.length > 0) {
        extension = extension.substring(1);

        for (type in fileTypeJson) {
            extensions = fileTypeJson[type].split(',');

            for (i = 0; i < extensions.length; i++) {
                if (extensions[i] == extension) {
                    return '/ui/images/datalist-file-' + type + '-icon.png';
                }
            }
        }
    }

    return '/ui/images/datalist-file-other-icon.png';
}


// 获取文件大小
function fileSize(byte) {
    if (Math.ceil(byte / 1024) < 1024) {
        return Math.ceil(byte / 1024) + ' KB';
    } else if (Math.ceil(byte / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024).toFixed(2) + ' MB';
    }
}


// 获取文件类型
function fileType(extension) {
    var extensions = new Array();
    var type;
    var i;

    if (extension.length > 0) {
        extension = extension.substring(1);

        for (type in fileTypeJson) {
            extensions = fileTypeJson[type].split(',');

            for (i = 0; i < extensions.length; i++) {
                if (extensions[i] == extension) {
                    return lang.type[type];
                }
            }
        }
    }

    return lang.type['other'];
}


// 获取文件操作事件
function fileEvent(event, username, time) {
    var ms = new Date().getTime() - new Date(time.replace(/\-/g, '/'));
    var second = 1000;
    var minute = second * 60;
    var hour = minute * 60;
    var day = hour * 24;
    var month = day * 30;
    var year = day * 365;

    switch(event) {
        case 'create':
        event = lang.file['event-create'];
        break;

        case 'update':
        event = lang.file['event-update'];
        break;

        case 'remove':
        event = lang.file['event-remove'];
        break;

        case 'upload':
        event = lang.file['event-upload'];
        break;
    }

    if ($page().client.width > 500 && $page().client.height > 500) {
        username = '<a href=\"javascript: window.self.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(username)) + '\', 500, 500);\">' + username + '</a>';
    } else {
        username = '<a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(username)) + '\', 500, 500);\">' + username + '</a>';
    }

    if (Math.floor(ms / second) == 0) {
        return username + ' ' + event + ' 0 ' + lang.timespan['second'];
    } else if (Math.floor(ms / second) < 60) {
        return username + ' ' + event + ' ' + Math.floor(ms / second) + ' ' + lang.timespan['second'];
    } else if (Math.floor(ms / minute) < 60) {
        return username + ' ' + event + ' ' + Math.floor(ms / minute) + ' ' + lang.timespan['minute'];
    } else if (Math.floor(ms / hour) < 24) {
        return username + ' ' + event + ' ' + Math.floor(ms / hour) + ' ' + lang.timespan['hour'];
    } else if (Math.floor(ms / day) < 30) {
        return username + ' ' + event + ' ' + Math.floor(ms / day) + ' ' + lang.timespan['day'];
    } else if (Math.floor(ms / month) < 12) {
        return username + ' ' + event + ' ' + Math.floor(ms / month) + ' ' + lang.timespan['month'];
    } else if (Math.floor(ms / year) > 0) {
        return username + ' ' + event + ' ' + Math.floor(ms / year) + ' ' + lang.timespan['year'];
    }
}


// 文件查询
function fileQuery(directory) {
    var keyword = $id('keyword').value;

    if (keyword.length == 0) {
        $location('keyword', '');
        return;
    }

    // 判断是否查找当前文件夹
    if (directory == true) {
        $location('keyword', encodeURIComponent(keyword));
    } else {
        $location('?keyword=' + encodeURIComponent(keyword));
    }
}


// 文件下载
function fileDownload(id, codeId) {
    var iframe = $id('file-download-iframe');

    if (iframe == null) {
        iframe = $create({
            tag: 'iframe', 
            attr: {
                id: 'file-download-iframe', 
                name: 'file-download-iframe', 
                width: '0', 
                height: '0', 
                frameBorder: '0'
                }, 
            css: 'none'
            }).$add(document.body);
    }

    iframe.src = '/web/drive/file/file-download.ashx?id=' + id + '&codeid=' + codeId;
}


// 文件预览(弹出窗口)
function fileView(index, width, height) {
    var clientWidth = windowObject.$page().client.width;
    var clientHeight = windowObject.$page().client.height;
    var id = jsonData.items[index].dbs_id;
    var codeId = jsonData.items[index].dbs_codeid;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;
    var version = jsonData.items[index].dbs_version;
    var popup = windowObject.$id('file-view-popup-window');
    var url = '/web/drive/file/file-view.html?id=' + id + '&codeid=' + codeId + '&extension=' + extension;
    var attach = '';
    var html = '';

    if (width == 0 || width > clientWidth) {
        width = clientWidth;
    }

    if (height == 0 || height > clientHeight) {
        height = clientHeight;
    }

    if (width < 0) {
        width = clientWidth - -(width);
    }

    if (height < 0) {
        height = clientHeight - -(height);
    }

    if (fileViewControl() == true) {
        // 根据窗体类型处理切换
        if (windowObject == window.self) {
            attach = '<span class=\"button\" onClick=\"fileViewChange(-1, ' + index + ', ' + width + ', ' + height + ');\">' + lang.file.button['view-previous'] + '</span>';
            attach += '<span class=\"button\" onClick=\"fileViewChange(1, ' + index + ', ' + width + ', ' + height + ');\">' + lang.file.button['view-next'] + '</span>';
        } else {
            attach = '<span class=\"button\" onClick=\"$id(\'file-version-iframe\').contentWindow.fileViewChange(-1, ' + index + ', ' + width + ', ' + height + ');\">' + lang.file.button['view-previous'] + '</span>';
            attach += '<span class=\"button\" onClick=\"$id(\'file-version-iframe\').contentWindow.fileViewChange(1, ' + index + ', ' + width + ', ' + height + ');\">' + lang.file.button['view-next'] + '</span>';
        }
    }

    html += '<div class=\"header\">';
    html += '<span class=\"title\">' + name + '' + extension + '&nbsp;&nbsp;v' + version + '</span>';
    html += '<span class=\"close\"><img src=\"/ui/images/window-close-icon.png\" width=\"16\" onClick=\"fastui.windowClose(\'file-view\');\" /></span>';
    html += '<span class=\"attach\">' + attach + '</span>';
    html += '</div>';
    html += '<div class=\"content\">';
    html += '<iframe id=\"file-view-iframe\" src=\"' + $url(url) + '\" width=\"' + width + '\" height=\"' + (height - 40) + '\" frameborder=\"0\" scrolling=\"yes\"></iframe>';
    html += '</div>';

    if (popup == null) {
        windowObject.fastui.coverShow('file-view-page-cover');

        popup = $create({
            tag: 'div', 
            attr: {id: 'file-view-popup-window'}, 
            style: {
                    width: width + 'px', 
                    height: height + 'px'
                    }, 
            css: 'popup-window', 
            html: html
            }).$add(windowObject.document.body);

        popup.style.top = Math.ceil((clientHeight - popup.offsetHeight) / 2) + 'px';
        popup.style.left = Math.ceil((clientWidth - popup.offsetWidth) / 2) + 'px';
    } else {
        popup.innerHTML = html;
    }
}


// 文件预览(控制)
function fileViewControl() {
    var count = 0;
    var i;

    for (i = 0; i < jsonData.items.length; i++) {
        if (jsonData.items[i].dbs_folder == 0) {
            count++;
        }
        
        if (count > 1) {
            return true;
        }
    }

    return false;
}


// 文件预览(切换)
function fileViewChange(action, index, width, height) {
    var count = jsonData.items.length;
    var first = 0;
    var i;

    // 计算第一个文件索引位置
    for (i = 0; i < count; i++) {
        if (jsonData.items[i].dbs_folder == 0) {
            first = i;
            break;
        }
    }

    // 上一个
    if (action == -1) {
        for (i = 0; i < count; i++) {
            index = index - 1 < 0 ? count - 1 : index - 1;

            if (jsonData.items[index].dbs_folder == 0) {
                break;
            }
        }
    }

    // 下一个
    if (action == 1) {
        for (i = 0; i < count; i++) {
            index = index + 1 > count - 1 ? 0 : index + 1;

            if (jsonData.items[index].dbs_folder == 0) {
                break;
            }
        }
    }

    if (index == first) {
        windowObject.fastui.textTips(lang.file.tips['view-reach-first']);
    }

    if (index == count - 1) {
        windowObject.fastui.textTips(lang.file.tips['view-reach-last']);
    }

    fileView(index, width, height);
}


// 文件夹自定义菜单项目(锁定)
function folderContextMenuLock(index) {
    dataListItemLock(index, true);
}


// 文件夹自定义菜单项目(解除锁定)
function folderContextMenuUnlock(index) {
    dataListItemUnlock(index, true);
}


// 文件夹自定义菜单项目(重命名)
function folderContextMenuRename(index) {
    var clientWidth = $page().client.width;
    var clientHeight = $page().client.height;
    var layer = $id('edit-box');
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var html = '';

    if (layer != null) {
        return;
    }

    fastui.coverShow('page-cover');

    html += '<div class=\"form\">';
    html += '<input name=\"name\" type=\"text\" class=\"input\" id=\"name\" value=\"' + name + '\" maxlength=\"50\" />';
    html += '<span class=\"button-ok\" onClick=\"folderContextMenuRenameOk(' + id + ');\">' + lang.file.button['confirm'] + '</span>';
    html += '<span class=\"button-cancel\" onClick=\"folderContextMenuRenameCancel();\">' + lang.file.button['cancel'] + '</span>';
    html += '</div>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'edit-box'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil((clientHeight - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil((clientWidth - layer.offsetWidth) / 2) + 'px';

    $id('name').focus();
    $id('name').select();
}


// 文件夹自定义菜单项目(重命名提交)
function folderContextMenuRenameOk(id) {
    var name = $id('name').value;
    var data = 'id=' + id + '&name=' + escape(name);

    if (fastui.testString(name, /^[^\\\/\:\*\?\"\<\>\|]{1,50}$/) == false) {
        fastui.inputTips('name', lang.file.tips.input['name']);
        return;
    }

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/folder-action.ashx?action=rename', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                $id('edit-box').$remove();

                fastui.list.scrollDataLoad(fastui.list.path, true);

                fastui.coverHide('page-cover');

                fastui.iconTips('tick');
            } else if (data == 'existed') {
                fastui.inputTips('name', lang.file.tips['name-existed']);

                $id('name').focus();
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 文件夹自定义菜单项目(重命名取消)
function folderContextMenuRenameCancel() {
    $id('edit-box').$remove();

    fastui.coverHide('page-cover');
}


// 文件夹自定义菜单项目(备注)
function folderContextMenuRemark(index) {
    var clientWidth = $page().client.width;
    var clientHeight = $page().client.height;
    var layer = $id('edit-box');
    var id = jsonData.items[index].dbs_id;
    var remark = jsonData.items[index].dbs_remark;
    var html = '';

    if (layer != null) {
        return;
    }

    fastui.coverShow('page-cover');

    html += '<div class=\"form\">';
    html += '<input name=\"remark\" type=\"text\" class=\"input\" id=\"remark\" value=\"' + remark + '\" maxlength=\"100\" />';
    html += '<span class=\"button-ok\" onClick=\"folderContextMenuRemarkOk(' + id + ');\">' + lang.file.button['confirm'] + '</span>';
    html += '<span class=\"button-cancel\" onClick=\"folderContextMenuRemarkCancel();\">' + lang.file.button['cancel'] + '</span>';
    html += '</div>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'edit-box'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil((clientHeight - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil((clientWidth - layer.offsetWidth) / 2) + 'px';

    $id('remark').focus();
    $id('remark').select();
}


// 文件夹自定义菜单项目(备注提交)
function folderContextMenuRemarkOk(id) {
    var remark = $id('remark').value;
    var data = 'id=' + id + '&remark=' + escape(remark);

    if (fastui.testString(remark, /^[\s\S]{1,100}$/) == false) {
        fastui.inputTips('remark', lang.file.tips.input['remark']);
        return;
    }

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/folder-action.ashx?action=remark', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                $id('edit-box').$remove();

                fastui.coverHide('page-cover');

                fastui.iconTips('tick');
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 文件夹自定义菜单项目(备注取消)
function folderContextMenuRemarkCancel() {
    $id('edit-box').$remove();

    fastui.coverHide('page-cover');
}


// 文件夹自定义菜单项目(共享权限)
function folderContextMenuPurview(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;

    fastui.windowPopup('purview-manage', lang.file.context['purview'] + ' - ' + name, '/web/drive/purview/purview-manage.html?id=' + id, 800, 500);
}


// 文件夹自定义菜单项目(移动)
function folderContextMenuMove(index) {
    var id = jsonData.items[index].dbs_id;

    $id('move-data-id').value = id;

    fastui.windowPopup('folder-select', lang.file.context['move'], '/web/drive/file/folder-select.html?folderid=' + $query('folderid') + '&position=' + $id('position').value + '&callback=folderContextMenuMoveOk&source=false&lock=false', 800, 500);
}


// 文件夹自定义菜单项目(移动提交)
function folderContextMenuMoveOk(folderId, folderName) {
    var id = $id('move-data-id').value;
    var data = 'folderid=' + folderId + '&id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/folder-action.ashx?action=move', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件夹自定义菜单项目(移除)
function folderContextMenuRemove(index) {
    dataListItemRemove(index, true);
}


// 文件夹自定义菜单项目(还原)
function folderContextMenuRestore(index) {
    dataListItemRestore(index, true);
}


// 文件夹自定义菜单项目(删除)
function folderContextMenuDelete(index) {
    dataListItemDelete(index, true);
}


// 文件夹自定义菜单项目(任务分派)
function folderContextMenuTask(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;

    fastui.windowPopup('task-assign', lang.file.context['task'] + ' - ' + name, '/web/drive/task/task-assign.html?id=' + id, 800, 500);
}


// 文件夹自定义菜单项目(讨论)
function folderContextMenuDiscuss(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;

    fastui.windowPopup('discuss-list', lang.file.context['discuss'] + ' - ' + name, '/web/drive/discuss/discuss-list.html?id=' + id, 800, 500);
}


// 文件夹自定义菜单项目(操作日志)
function folderContextMenuLog(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;

    fastui.windowPopup('activity-log', lang.file.context['log'] + ' - ' + name, '/web/drive/activity/activity-log.html?id=' + id, 800, 500);
}


// 文件夹自定义菜单项目(详细属性)
function folderContextMenuDetail(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;

    fastui.windowPopup('folder-detail', lang.file.context['detail'] + ' - ' + name, '/web/drive/file/folder-detail.html?id=' + id, 800, 500);
}


// 文件自定义菜单项目(下载)
function fileContextMenuDownload(index) {
    var id = jsonData.items[index].dbs_id;
    var codeId = jsonData.items[index].dbs_codeid;

    fileDownload(id, codeId);
}


// 文件自定义菜单项目(上传新版本)
function fileContextMenuUpversion(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.windowPopup('file-upversion', lang.file.context['upversion'] + ' - ' + name + extension, '/web/drive/file/file-upversion.html?id=' + id + '&extension=' + extension, 800, 500);
}


// 文件自定义菜单项目(锁定)
function fileContextMenuLock(index) {
    dataListItemLock(index, false);
}


// 文件自定义菜单项目(解除锁定)
function fileContextMenuUnlock(index) {
    dataListItemUnlock(index, false);
}


// 文件自定义菜单项目(更改为当前版本)
function fileContextMenuReplace(index) {
    dataListItemReplace(index);
}


// 文件自定义菜单项目(重命名)
function fileContextMenuRename(index) {
    var clientWidth = $page().client.width;
    var clientHeight = $page().client.height;
    var layer = $id('edit-box');
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var html = '';

    if (layer != null) {
        return;
    }

    fastui.coverShow('page-cover');

    html += '<div class=\"form\">';
    html += '<input name=\"name\" type=\"text\" class=\"input\" id=\"name\" value=\"' + name + '\" maxlength=\"75\" />';
    html += '<span class=\"button-ok\" onClick=\"fileContextMenuRenameOk(' + id + ');\">' + lang.file.button['confirm'] + '</span>';
    html += '<span class=\"button-cancel\" onClick=\"fileContextMenuRenameCancel();\">' + lang.file.button['cancel'] + '</span>';
    html += '</div>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'edit-box'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil((clientHeight - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil((clientWidth - layer.offsetWidth) / 2) + 'px';

    $id('name').focus();
    $id('name').select();
}


// 文件自定义菜单项目(重命名提交)
function fileContextMenuRenameOk(id) {
    var name = $id('name').value;
    var data = 'id=' + id + '&name=' + escape(name);

    if (fastui.testString(name, /^[^\\\/\:\*\?\"\<\>\|]{1,75}$/) == false) {
        fastui.inputTips('name', lang.file.tips.input['name']);
        return;
    }

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/file-action.ashx?action=rename', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                $id('edit-box').$remove();

                fastui.list.scrollDataLoad(fastui.list.path, true);

                fastui.coverHide('page-cover');

                fastui.iconTips('tick');
            } else if (data == 'existed') {
                fastui.inputTips('name', lang.file.tips['name-existed']);

                $id('name').focus();
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 文件自定义菜单项目(重命名取消)
function fileContextMenuRenameCancel() {
    $id('edit-box').$remove();

    fastui.coverHide('page-cover');
}


// 文件自定义菜单项目(备注)
function fileContextMenuRemark(index) {
    var clientWidth = $page().client.width;
    var clientHeight = $page().client.height;
    var layer = $id('edit-box');
    var id = jsonData.items[index].dbs_id;
    var remark = jsonData.items[index].dbs_remark;
    var html = '';

    if (layer != null) {
        return;
    }

    fastui.coverShow('page-cover');

    html += '<div class=\"form\">';
    html += '<input name=\"remark\" type=\"text\" class=\"input\" id=\"remark\" value=\"' + remark + '\" maxlength=\"100\" />';
    html += '<span class=\"button-ok\" onClick=\"fileContextMenuRemarkOk(' + id + ');\">' + lang.file.button['confirm'] + '</span>';
    html += '<span class=\"button-cancel\" onClick=\"fileContextMenuRemarkCancel();\">' + lang.file.button['cancel'] + '</span>';
    html += '</div>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'edit-box'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil((clientHeight - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil((clientWidth - layer.offsetWidth) / 2) + 'px';

    $id('remark').focus();
    $id('remark').select();
}


// 文件自定义菜单项目(备注提交)
function fileContextMenuRemarkOk(id) {
    var remark = $id('remark').value;
    var data = 'id=' + id + '&remark=' + escape(remark);

    if (fastui.testString(remark, /^[\s\S]{1,100}$/) == false) {
        fastui.inputTips('remark', lang.file.tips.input['remark']);
        return;
    }

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/file-action.ashx?action=remark', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                $id('edit-box').$remove();

                if (window.location.pathname.indexOf('file-version') > -1) {
                    fastui.list.scrollDataLoad(fastui.list.path, true);
                }

                fastui.coverHide('page-cover');

                fastui.iconTips('tick');
            } else if (data == 'no-permission') {
                fastui.textTips(lang.file.tips['no-permission']);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 文件自定义菜单项目(备注取消)
function fileContextMenuRemarkCancel() {
    $id('edit-box').$remove();

    fastui.coverHide('page-cover');
}


// 文件自定义菜单项目(复制)
function fileContextMenuCopy(index) {
    var id = jsonData.items[index].dbs_id;

    $id('copy-data-id').value = id;

    fastui.windowPopup('folder-select', lang.file.context['copy'], '/web/drive/file/folder-select.html?folderid=' + $query('folderid') + '&position=' + $id('position').value + '&callback=fileContextMenuCopyOk&source=true&lock=false', 800, 500);
}


// 文件自定义菜单项目(复制提交)
function fileContextMenuCopyOk(folderId, folderName) {
    var id = $id('copy-data-id').value;
    var data = 'folderid=' + folderId + '&id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/file-action.ashx?action=copy', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件自定义菜单项目(移动)
function fileContextMenuMove(index) {
    var id = jsonData.items[index].dbs_id;

    $id('move-data-id').value = id;

    fastui.windowPopup('folder-select', lang.file.context['move'], '/web/drive/file/folder-select.html?folderid=' + $query('folderid') + '&position=' + $id('position').value + '&callback=fileContextMenuMoveOk&source=false&lock=false', 800, 500);
}


// 文件自定义菜单项目(移动提交)
function fileContextMenuMoveOk(folderId, folderName) {
    var id = $id('move-data-id').value;
    var data = 'folderid=' + folderId + '&id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/file/file-action.ashx?action=move', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件自定义菜单项目(移除)
function fileContextMenuRemove(index) {
    dataListItemRemove(index, false);
}


// 文件自定义菜单项目(还原)
function fileContextMenuRestore(index) {
    dataListItemRestore(index, false);
}


// 文件自定义菜单项目(删除)
function fileContextMenuDelete(index) {
    dataListItemDelete(index, false);
}


// 文件自定义菜单项目(任务分派)
function fileContextMenuTask(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.windowPopup('task-assign', lang.file.context['task'] + ' - ' + name + extension, '/web/drive/task/task-assign.html?id=' + id, 800, 500);
}


// 文件自定义菜单项目(讨论)
function fileContextMenuDiscuss(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.windowPopup('discuss-list', lang.file.context['discuss'] + ' - ' + name + extension, '/web/drive/discuss/discuss-list.html?id=' + id, 800, 500);
}


// 文件自定义菜单项目(历史版本)
function fileContextMenuVersion(index) {
    var id = jsonData.items[index].dbs_id;
    var codeid = jsonData.items[index].dbs_codeid;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.windowPopup('file-version', lang.file.context['version'] + ' - ' + name + extension, '/web/drive/file/file-version.html?id=' + id + '&codeid=' + codeid + '&extension=' + extension, 800, 500);
}


// 文件自定义菜单项目(操作日志)
function fileContextMenuLog(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.windowPopup('activity-log', lang.file.context['log'] + ' - ' + name + extension, '/web/drive/activity/activity-log.html?id=' + id, 800, 500);
}


// 文件自定义菜单项目(详细属性)
function fileContextMenuDetail(index) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    windowObject.fastui.windowPopup('file-detail', lang.file.context['detail'] + ' - ' + name + extension, '/web/drive/file/file-detail.html?id=' + id, 800, 500);
}