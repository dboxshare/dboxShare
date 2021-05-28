var windowObject = window.location.pathname.indexOf('file-version') == -1 ? window.self : window.parent;
var intervalObject = null;


// 监听键盘
document.onkeydown = function(e) {
    var event = e || window.event;

    // 文件查看左方向键控制(上一个)
    if (event.keyCode == 37) {
        var previous = windowObject.$id('view-previous');

        if (previous != null) {
            previous.click();
            return;
        }
    }

    // 文件查看右方向键控制(下一个)
    if (event.keyCode == 39) {
        var next = windowObject.$id('view-next');

        if (next != null) {
            next.click();
            return;
        }
    }
};


// 权限校验
function purviewCheck(value, target) {
    switch(value) {
        case 'viewer':
        value = 1;
        break;

        case 'downloader':
        value = 2;
        break;

        case 'uploader':
        value = 3;
        break;

        case 'editor':
        value = 4;
        break;

        case 'manager':
        value = 5;
        break;

        case 'creator':
        value = 6;
        break;
    }

    switch(target) {
        case 'viewer':
        target = 1;
        break;

        case 'downloader':
        target = 2;
        break;

        case 'uploader':
        target = 3;
        break;

        case 'editor':
        target = 4;
        break;

        case 'manager':
        target = 5;
        break;

        case 'creator':
        target = 6;
        break;
    }

    if (value >= target) {
        return true;
    } else {
        return false;
    }
}


// 数据列表自定义菜单事件(鼠标右击)
function dataListContextMenuEvent(j) {
    var container = $id('datalist-container');

    // 屏蔽浏览器右击菜单
    container.oncontextmenu = function() {
        var selection = window.getSelection ? window.getSelection().toString() : document.selection.createRange().text;

        if (selection.length == 0) {
            return false;
        }
        };

    var rows = $id('data-list').$tag('tr');
    var checkboxes = $name('id');
    var folderId = $query('folderId');

    for (var i = j; i < rows.length; i++) {
        (function(index) {rows[i].oncontextmenu = function(e) {
            var event = e || window.event;
            var clientX = event.clientX;
            var clientY = event.clientY;
            var selection = window.getSelection ? window.getSelection().toString() : document.selection.createRange().text;

            // 定位列表区域
            clientX = clientX - container.$left();

            if (selection.length > 0) {
                return;
            }

            var id = jsonData.items[index].ds_id;
            var folder = jsonData.items[index].ds_folder;
            var url = '/api/drive/file/context-attribute-json?id=' + id + '&folder=' + (folder == 1 ? true : false);

            // 判断是否创建者
            if (jsonData.items[index].ds_userid == window.top.myId || jsonData.items[index].ds_createusername == window.top.myUsername) {
                url += '&creator=true';
            }

            // 获取菜单属性
            $ajax({
                type: 'GET', 
                url: url, 
                async: true, 
                callback: function(data) {
                    if ($jsonString(data) == false) {
                        fastui.windowPopup('login', '', '/web/account/login.html', 350, 350);
                        return;
                    } else {
                        window.eval('var attributeDataJson = ' + data + ';');

                        var role = attributeDataJson.role;
                        var collect = attributeDataJson.collect;
                        var follow = attributeDataJson.follow;

                        dataListContextMenuView(container, rows, checkboxes, index, role, collect, follow, clientX, clientY);
                    }
                    }
                });
            };})(i);
    }
}


// 数据列表自定义菜单视图(鼠标右击)
function dataListContextMenuView(container, rows, checkboxes, index, role, collect, follow, clientX, clientY) {
    var folder = jsonData.items[index].ds_folder == 1 ? true : false;
    var share = jsonData.items[index].ds_share;
    var lock = jsonData.items[index].ds_lock;

    // 根据用户角色显示自定义菜单项目
    dataListContextMenuShow(folder, share, lock, role, collect, follow);

    // 判断列表项目是否集合操作
    if (checkboxes[index].checked == true) {
        var count = 0;

        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked == true) {
                count++;
            }

            if (count > 1) {
                break;
            }
        }

        if (count > 1) {
            if (purview.length == 0) {
                dataListContextMenuCollection(container, clientX, clientY);
                return;
            } else {
                if (share == 0 || purviewCheck(purview, 'manager') == true) {
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
        var layer = $id('folder-context-menu');
    } else {
        var layer = $id('file-context-menu');
    }

    if (layer == null) {
        return;
    }

    layer.innerHTML = layer.innerHTML.replace(/\(\d+\)/g, '(' + index + ')');

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
function dataListContextMenuClickEvent(index, e) {
    var event = e || window.event;
    var clientX = event.clientX;
    var clientY = event.clientY;
    var container = $id('datalist-container');
    var rows = $id('data-list').$tag('tr');
    var checkboxes = $name('id');
    var folderId = $query('folderId');

    // 定位列表区域
    clientX = clientX - container.$left();

    var id = jsonData.items[index].ds_id;
    var folder = jsonData.items[index].ds_folder;
    var url = '/api/drive/file/context-attribute-json?id=' + id + '&folder=' + (folder == 1 ? true : false);

    // 判断是否创建者
    if (jsonData.items[index].ds_userid == window.top.myId || jsonData.items[index].ds_createusername == window.top.myUsername) {
        url += '&creator=true';
    }

    // 获取菜单属性
    $ajax({
        type: 'GET', 
        url: url, 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.windowPopup('login', '', '/web/account/login.html', 350, 350);
                return;
            } else {
                window.eval('var attributeDataJson = ' + data + ';');

                var role = attributeDataJson.role;
                var collect = attributeDataJson.collect;
                var follow = attributeDataJson.follow;

                dataListContextMenuClickView(container, rows, checkboxes, index, role, collect, follow, clientX, clientY);
            }
            }
        });
}


// 数据列表自定义菜单视图(鼠标点击)
function dataListContextMenuClickView(container, rows, checkboxes, index, role, collect, follow, clientX, clientY) {
    var id = jsonData.items[index].ds_id;
    var folder = jsonData.items[index].ds_folder == 1 ? true : false;
    var share = jsonData.items[index].ds_share;
    var lock = jsonData.items[index].ds_lock;

    // 根据用户角色显示自定义菜单项目
    dataListContextMenuShow(folder, share, lock, role, collect, follow);

    // 判断自定义菜单类型(文件夹或文件)
    if (folder == true) {
        var layer = $id('folder-context-menu');
    } else {
        var layer = $id('file-context-menu');
    }

    if (layer == null) {
        return;
    }

    layer.innerHTML = layer.innerHTML.replace(/\(\d+\)/g, '(' + index + ')');

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
function dataListContextMenuShow(folder, share, lock, role, collect, follow) {
    var viewer = purviewCheck(role, 'viewer') == true ? 'block' : 'none';
    var downloader = purviewCheck(role, 'downloader') == true ? 'block' : 'none';
    var uploader = purviewCheck(role, 'uploader') == true ? 'block' : 'none';
    var editor = purviewCheck(role, 'editor') == true ? 'block' : 'none';
    var manager = purviewCheck(role, 'manager') == true ? 'block' : 'none';
    var creator = purviewCheck(role, 'creator') == true ? 'block' : 'none';

    if (folder == true) {
        if ($id('folder-context-menu-rename') != null) {
            $id('folder-context-menu-rename').style.display = lock == 1 ? 'none' : creator;
        }

        if ($id('folder-context-menu-remark') != null) {
            $id('folder-context-menu-remark').style.display = lock == 1 ? 'none' : manager;
        }

        if ($id('folder-context-menu-purview') != null) {
            $id('folder-context-menu-purview').style.display = share == 1 ? 'block' : creator;
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

        if ($id('folder-context-menu-lock') != null) {
            $id('folder-context-menu-lock').style.display = lock == 1 ? 'none' : creator;
        }

        if ($id('folder-context-menu-unlock') != null) {
            $id('folder-context-menu-unlock').style.display = lock == 0 ? 'none' : creator;
        }
    } else {
        if ($id('file-context-menu-download') != null) {
            $id('file-context-menu-download').style.display = downloader;
        }

        if ($id('file-context-menu-collect') != null) {
            $id('file-context-menu-collect').style.display = collect == 0 ? 'block' : 'none';
        }

        if ($id('file-context-menu-uncollect') != null) {
            $id('file-context-menu-uncollect').style.display = collect == 1 ? 'block' : 'none';
        }

        if ($id('file-context-menu-follow') != null) {
            $id('file-context-menu-follow').style.display = follow == 0 ? 'block' : 'none';
        }

        if ($id('file-context-menu-unfollow') != null) {
            $id('file-context-menu-unfollow').style.display = follow == 1 ? 'block' : 'none';
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

        if ($id('file-context-menu-lock') != null) {
            $id('file-context-menu-lock').style.display = lock == 1 ? 'none' : creator;
        }

        if ($id('file-context-menu-unlock') != null) {
            $id('file-context-menu-unlock').style.display = lock == 0 ? 'none' : creator;
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
    if (extension.length > 0) {
        extension = extension.substring(1);

        for (var type in fileTypeJson) {
            var extensions = fileTypeJson[type].split(',');

            for (var i = 0; i < extensions.length; i++) {
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
    } else if (Math.ceil(byte / 1024 / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024 / 1024).toFixed(2) + ' GB';
    } else {
        return (byte / 1024 / 1024 / 1024).toFixed(2) + ' GB';
    }
}


// 获取文件类型
function fileType(extension) {
    if (extension.length > 0) {
        extension = extension.substring(1);

        for (var type in fileTypeJson) {
            var extensions = fileTypeJson[type].split(',');

            for (var i = 0; i < extensions.length; i++) {
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
    }

    if ($page().client.width > 500 && $page().client.height > 500) {
        username = '<a href="javascript: window.self.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/account/user-card.html?username=' + encodeURI(encodeURIComponent(username)) + '\', 500, 500);">' + username + '</a>';
    } else {
        username = '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/account/user-card.html?username=' + encodeURI(encodeURIComponent(username)) + '\', 500, 500);">' + username + '</a>';
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


// 文件下载
function fileDownload(id, codeId) {
    var iframe = $id('file-download-iframe');

    if (iframe != null) {
        fastui.textTips(lang.file.tips['download-waiting'], 0, 8);
        return;
    }

    $ajax({
        type: 'GET', 
        url: '/api/drive/file/download?state=waiting', 
        async: true, 
        callback: function(data) {
            if (data == 'complete') {
                iframe = $create({
                    tag: 'iframe', 
                    attr: {
                        id: 'file-download-iframe', 
                        name: 'file-download-iframe', 
                        src: '/api/drive/file/download?id=' + id + '&codeid=' + codeId, 
                        width: '0', 
                        height: '0', 
                        frameBorder: '0', 
                        onload: 'fileDownloadProgress();', 
                        onreadystatechange: 'fileDownloadProgress();'
                        }
                    }).$add(document.body);

                fastui.textTips(lang.file.tips['download-waiting'], 0, 8);

                // 文件下载监听
                intervalObject = window.setInterval(function() {
                    fileDownloadProgress();
                    }, 1000);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 文件下载准备完成(开始下载)
function fileDownloadProgress() {
    var iframe = $id('file-download-iframe');

    if (iframe == null) {
        return;
    }

    var iDocument = iframe.contentWindow.document || iframe.contentDocument;

    window.setTimeout(function() {
        if (iDocument.readyState != 'interactive' && iDocument.readyState != 'complete') {
            return;
        } else {
            if (iDocument.body == null) {
                return;
            }

            var content = iDocument.body.innerHTML.replace(/\<[^\>]+\>/g, '');

            if (content.length == 0) {
                if ($cookie('file-download-state') != 'complete') {
                    return;
                }
            } else {
                window.clearInterval(intervalObject);
            }

            if (content == 'download-size-limit') {
                fastui.textTips(lang.file.tips['download-size-limit'].replace(/\{size\}/, window.top.myDownloadSize));
                return;
            }

            window.setTimeout(function() {
                iframe.$remove();
                }, 500);

            var tips = $id('text-tips');

            if (tips == null) {
                return;
            }

            window.setTimeout(function() {
                tips.$remove();
                }, 500);
        }
        }, 0);
}


// 文件预览(弹出窗口)
function fileView(index, width, height) {
    var id = jsonData.items[index].ds_id;
    var codeId = jsonData.items[index].ds_codeid;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;
    var version = jsonData.items[index].ds_version;

    if (width == 0 || width > windowObject.$page().client.width) {
        width = windowObject.$page().client.width;
    }

    if (height == 0 || height > windowObject.$page().client.height) {
        height = windowObject.$page().client.height;
    }

    if (width < 0) {
        width = windowObject.$page().client.width - -(width);
    }

    if (height < 0) {
        height = windowObject.$page().client.height - -(height);
    }

    var attach = '';

    if (fileViewControl() == true) {
        // 根据窗体类型处理切换
        if (windowObject == window.self) {
            attach += '<span id="view-previous" class="button" title="' + lang.file.tips['view-previous'] + '" onClick="fileViewChange(-1, ' + index + ', ' + width + ', ' + height + ');">' + lang.file.button['view-previous'] + '</span>';
            attach += '<span id="view-next" class="button" title="' + lang.file.tips['view-next'] + '" onClick="fileViewChange(1, ' + index + ', ' + width + ', ' + height + ');">' + lang.file.button['view-next'] + '</span>';
        } else {
            attach += '<span id="view-previous" class="button" title="' + lang.file.tips['view-previous'] + '" onClick="$id(\'file-version-iframe\').contentWindow.fileViewChange(-1, ' + index + ', ' + width + ', ' + height + ');">' + lang.file.button['view-previous'] + '</span>';
            attach += '<span id="view-next" class="button" title="' + lang.file.tips['view-next'] + '" onClick="$id(\'file-version-iframe\').contentWindow.fileViewChange(1, ' + index + ', ' + width + ', ' + height + ');">' + lang.file.button['view-next'] + '</span>';
        }
    }

    var url = '/web/drive/file/file-view.html?id=' + id + '&codeid=' + codeId + '&extension=' + extension;
    var html = '';

    html += '<div class="header">';
    html += '<span class="title">' + name + '' + extension + '&nbsp;&nbsp;v' + version + '</span>';
    html += '<span class="close"><img src="/ui/images/window-close-icon.png" width="16" onClick="fastui.windowClose(\'file-view\');" /></span>';
    html += '<span class="attach">' + attach + '</span>';
    html += '</div>';
    html += '<div class="content">';
    html += '<iframe id="file-view-iframe" src="' + $url(url) + '" width="' + width + '" height="' + (height - 40) + '" frameborder="0" scrolling="yes"></iframe>';
    html += '</div>';

    var popup = windowObject.$id('file-view-popup-window');

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

        popup.style.top = Math.ceil((windowObject.$page().client.height - popup.offsetHeight) / 2) + 'px';
        popup.style.left = Math.ceil((windowObject.$page().client.width - popup.offsetWidth) / 2) + 'px';
    } else {
        popup.innerHTML = html;
    }
}


// 文件预览(控制)
function fileViewControl() {
    var count = 0;

    for (var i = 0; i < jsonData.items.length; i++) {
        if (jsonData.items[i].ds_folder == 0) {
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

    // 计算第一个文件索引位置
    for (var i = 0; i < count; i++) {
        if (jsonData.items[i].ds_folder == 0) {
            first = i;
            break;
        }
    }

    // 上一个
    if (action == -1) {
        for (var j = 0; j < count; j++) {
            index = index - 1 < 0 ? count - 1 : index - 1;

            if (jsonData.items[index].ds_folder == 0) {
                break;
            }
        }
    }

    // 下一个
    if (action == 1) {
        for (var j = 0; j < count; j++) {
            index = index + 1 > count - 1 ? 0 : index + 1;

            if (jsonData.items[index].ds_folder == 0) {
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


// 列表转到(文件夹跳转)
function listGoto(folderId) {
    var path = '/web/drive/file/file-list.html?folderid=' + folderId;

    $location(path);
}


// 文件夹自定义菜单项目(链接分享)
function folderContextMenuLink(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    fastui.windowPopup('folder-link', lang.file.context['link'] + ' - ' + name, '/web/drive/link/link-share.html?id=' + id + '&title=' + encodeURI(encodeURIComponent(name)), 800, 500);
}


// 文件夹自定义菜单项目(重命名)
function folderContextMenuRename(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var layer = $id('edit-box');
    var html = '';

    if (layer != null) {
        return;
    }

    fastui.coverShow('page-cover');

    html += '<div class="form">';
    html += '<input name="name" type="text" id="name" value="' + name + '" maxlength="50" />';
    html += '<span class="button-ok" onClick="folderContextMenuRenameOk(' + id + ');">' + lang.file.button['confirm'] + '</span>';
    html += '<span class="button-cancel" onClick="folderContextMenuRenameCancel();">' + lang.file.button['cancel'] + '</span>';
    html += '</div>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'edit-box'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil(($page().client.height - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil(($page().client.width - layer.offsetWidth) / 2) + 'px';

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
        url: '/api/drive/folder/rename', 
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
    var id = jsonData.items[index].ds_id;
    var remark = jsonData.items[index].ds_remark;
    var layer = $id('edit-box');
    var html = '';

    if (layer != null) {
        return;
    }

    fastui.coverShow('page-cover');

    html += '<div class="form">';
    html += '<input name="remark" type="text" id="remark" value="' + remark + '" maxlength="100" />';
    html += '<span class="button-ok" onClick="folderContextMenuRemarkOk(' + id + ');">' + lang.file.button['confirm'] + '</span>';
    html += '<span class="button-cancel" onClick="folderContextMenuRemarkCancel();">' + lang.file.button['cancel'] + '</span>';
    html += '</div>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'edit-box'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil(($page().client.height - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil(($page().client.width - layer.offsetWidth) / 2) + 'px';

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
        url: '/api/drive/folder/remark', 
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
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    if (jsonData.items[index].ds_userid == window.top.myId || jsonData.items[index].ds_createusername == window.top.myUsername) {
        fastui.windowPopup('purview-manage', lang.file.context['purview'] + ' - ' + name, '/web/drive/purview/purview-manage.html?id=' + id, 800, 500);
    } else {
        fastui.windowPopup('purview-detail', lang.file.context['purview'] + ' - ' + name, '/web/drive/purview/purview-detail.html?id=' + id, 800, 500);
    }
}


// 文件夹自定义菜单项目(移动)
function folderContextMenuMove(index) {
    var id = jsonData.items[index].ds_id;
    var position = $id('position');

    if (position != null) {
        position = position.value;
    }

    $id('move-data-id').value = id;

    fastui.windowPopup('folder-select', lang.file.context['move'], '/web/drive/folder/folder-select.html?folderid=' + $query('folderid') + '&position=' + position + '&callback=folderContextMenuMoveOk&source=false&lock=false', 800, 500);
}


// 文件夹自定义菜单项目(移动提交)
function folderContextMenuMoveOk(folderId, folderName) {
    var id = $id('move-data-id').value;
    var data = 'folderid=' + folderId + '&id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/folder/move', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                fastui.textTips(lang.file.tips['move-complete'].replace(/\{folderid\}/, folderId), 0, 8);
            }

            dataListActionCallback(data);
            }
        });
}


// 文件夹自定义菜单项目(移除)
function folderContextMenuRemove(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    fastui.dialogConfirm(lang.file.tips.confirm['remove'] + '<br />' + name, 'folderContextMenuRemoveOk(' + id + ')');
}


// 文件夹自定义菜单项目(移除提交)
function folderContextMenuRemoveOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/folder/remove', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件夹自定义菜单项目(还原)
function folderContextMenuRestore(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    fastui.dialogConfirm(lang.file.tips.confirm['restore'] + '<br />' + name, 'folderContextMenuRestoreOk(' + id + ')');
}


// 文件夹自定义菜单项目(还原提交)
function folderContextMenuRestoreOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/folder/restore', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件夹自定义菜单项目(删除)
function folderContextMenuDelete(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    if ($name('deletable')[index].value == 'false') {
        fastui.textTips(lang.file.tips['not-allow-delete']);
        return;
    }

    fastui.dialogConfirm(lang.file.tips.confirm['delete'] + '<br />' + name, 'folderContextMenuDeleteOk(' + id + ')');
}


// 文件夹自定义菜单项目(删除提交)
function folderContextMenuDeleteOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/folder/delete', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件夹自定义菜单项目(锁定)
function folderContextMenuLock(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    fastui.dialogConfirm(lang.file.tips.confirm['lock'] + '<br />' + name, 'folderContextMenuLockOk(' + id + ')');
}


// 文件夹自定义菜单项目(锁定提交)
function folderContextMenuLockOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/folder/lock', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件夹自定义菜单项目(取消锁定)
function folderContextMenuUnlock(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    fastui.dialogConfirm(lang.file.tips.confirm['unlock'] + '<br />' + name, 'folderContextMenuUnlockOk(' + id + ')');
}


// 文件夹自定义菜单项目(取消锁定提交)
function folderContextMenuUnlockOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/folder/unlock', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件夹自定义菜单项目(操作日志)
function folderContextMenuLog(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    fastui.windowPopup('activity-log', lang.file.context['log'] + ' - ' + name, '/web/drive/activity/activity-log.html?id=' + id, 800, 500);
}


// 文件夹自定义菜单项目(详细属性)
function folderContextMenuDetail(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;

    fastui.windowPopup('folder-detail', lang.file.context['detail'] + ' - ' + name, '/web/drive/folder/folder-detail.html?id=' + id, 800, 500);
}


// 文件自定义菜单项目(下载)
function fileContextMenuDownload(index) {
    var id = jsonData.items[index].ds_id;
    var codeId = jsonData.items[index].ds_codeid;

    fileDownload(id, codeId);
}


// 文件自定义菜单项目(收藏)
function fileContextMenuCollect(index) {
    var id = jsonData.items[index].ds_id;
    var data = 'id=' + id;

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/collect', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                fastui.iconTips('tick');
            } else {
                fastui.iconTips('cross');
            }
            }
        });
}


// 文件自定义菜单项目(取消收藏)
function fileContextMenuUncollect(index) {
    var id = jsonData.items[index].ds_id;
    var data = 'id=' + id;

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/uncollect', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                fastui.iconTips('tick');
            } else {
                fastui.iconTips('cross');
            }
            }
        });
}


// 文件自定义菜单项目(关注)
function fileContextMenuFollow(index) {
    var id = jsonData.items[index].ds_id;
    var data = 'id=' + id;

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/follow', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                fastui.iconTips('tick');
            } else {
                fastui.iconTips('cross');
            }
            }
        });
}


// 文件自定义菜单项目(取消关注)
function fileContextMenuUnfollow(index) {
    var id = jsonData.items[index].ds_id;
    var data = 'id=' + id;

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/unfollow', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                fastui.iconTips('tick');
            } else {
                fastui.iconTips('cross');
            }
            }
        });
}


// 文件自定义菜单项目(链接分享)
function fileContextMenuLink(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    fastui.windowPopup('file-link', lang.file.context['link'] + ' - ' + name + extension, '/web/drive/link/link-share.html?id=' + id + '&title=' + encodeURI(encodeURIComponent(name + extension)), 800, 500);
}


// 文件自定义菜单项目(重命名)
function fileContextMenuRename(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var layer = $id('edit-box');
    var html = '';

    if (layer != null) {
        return;
    }

    fastui.coverShow('page-cover');

    html += '<div class="form">';
    html += '<input name="name" type="text" id="name" value="' + name + '" maxlength="75" />';
    html += '<span class="button-ok" onClick="fileContextMenuRenameOk(' + id + ');">' + lang.file.button['confirm'] + '</span>';
    html += '<span class="button-cancel" onClick="fileContextMenuRenameCancel();">' + lang.file.button['cancel'] + '</span>';
    html += '</div>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'edit-box'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil(($page().client.height - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil(($page().client.width - layer.offsetWidth) / 2) + 'px';

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
        url: '/api/drive/file/rename', 
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
    var id = jsonData.items[index].ds_id;
    var remark = jsonData.items[index].ds_remark;
    var layer = $id('edit-box');
    var html = '';

    if (layer != null) {
        return;
    }

    fastui.coverShow('page-cover');

    html += '<div class="form">';
    html += '<input name="remark" type="text" id="remark" value="' + remark + '" maxlength="100" />';
    html += '<span class="button-ok" onClick="fileContextMenuRemarkOk(' + id + ');">' + lang.file.button['confirm'] + '</span>';
    html += '<span class="button-cancel" onClick="fileContextMenuRemarkCancel();">' + lang.file.button['cancel'] + '</span>';
    html += '</div>';

    layer = $create({
        tag: 'div', 
        attr: {id: 'edit-box'}, 
        html: html
        }).$add(document.body);

    layer.style.top = Math.ceil(($page().client.height - layer.offsetHeight) / 2) + 'px';
    layer.style.left = Math.ceil(($page().client.width - layer.offsetWidth) / 2) + 'px';

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
        url: '/api/drive/file/remark', 
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
    var id = jsonData.items[index].ds_id;
    var position = $id('position');

    if (position != null) {
        position = position.value;
    }

    $id('copy-data-id').value = id;

    fastui.windowPopup('folder-select', lang.file.context['copy'], '/web/drive/folder/folder-select.html?folderid=' + $query('folderid') + '&position=' + position + '&callback=fileContextMenuCopyOk&source=true&lock=false', 800, 500);
}


// 文件自定义菜单项目(复制提交)
function fileContextMenuCopyOk(folderId, folderName) {
    var id = $id('copy-data-id').value;
    var data = 'folderid=' + folderId + '&id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/copy', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                fastui.textTips(lang.file.tips['copy-complete'].replace(/\{folderid\}/, folderId), 0, 8);
            }

            dataListActionCallback(data);
            }
        });
}


// 文件自定义菜单项目(移动)
function fileContextMenuMove(index) {
    var id = jsonData.items[index].ds_id;
    var position = $id('position');

    if (position != null) {
        position = position.value;
    }

    $id('move-data-id').value = id;

    fastui.windowPopup('folder-select', lang.file.context['move'], '/web/drive/folder/folder-select.html?folderid=' + $query('folderid') + '&position=' + position + '&callback=fileContextMenuMoveOk&source=false&lock=false', 800, 500);
}


// 文件自定义菜单项目(移动提交)
function fileContextMenuMoveOk(folderId, folderName) {
    var id = $id('move-data-id').value;
    var data = 'folderid=' + folderId + '&id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/move', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                fastui.textTips(lang.file.tips['move-complete'].replace(/\{folderid\}/, folderId), 0, 8);
            }

            dataListActionCallback(data);
            }
        });
}


// 文件自定义菜单项目(移除)
function fileContextMenuRemove(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['remove'] + '<br />' + name + extension, 'fileContextMenuRemoveOk(' + id + ')');
}


// 文件自定义菜单项目(移除提交)
function fileContextMenuRemoveOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/remove', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件自定义菜单项目(还原)
function fileContextMenuRestore(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['restore'] + '<br />' + name + extension, 'fileContextMenuRestoreOk(' + id + ')');
}


// 文件自定义菜单项目(还原提交)
function fileContextMenuRestoreOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/restore', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件自定义菜单项目(删除)
function fileContextMenuDelete(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    if ($name('deletable')[index].value == 'false') {
        fastui.textTips(lang.file.tips['not-allow-delete']);
        return;
    }

    fastui.dialogConfirm(lang.file.tips.confirm['delete'] + '<br />' + name + extension, 'fileContextMenuDeleteOk(' + id + ')');
}


// 文件自定义菜单项目(删除提交)
function fileContextMenuDeleteOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/delete', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件自定义菜单项目(锁定)
function fileContextMenuLock(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['lock'] + '<br />' + name + extension, 'fileContextMenuLockOk(' + id + ')');
}


// 文件自定义菜单项目(锁定提交)
function fileContextMenuLockOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/lock', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件自定义菜单项目(取消锁定)
function fileContextMenuUnlock(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['unlock'] + '<br />' + name + extension, 'fileContextMenuUnlockOk(' + id + ')');
}


// 文件自定义菜单项目(取消锁定提交)
function fileContextMenuUnlockOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/unlock', 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 文件自定义菜单项目(版本管理)
function fileContextMenuVersion(index) {
    var id = jsonData.items[index].ds_id;
    var codeid = jsonData.items[index].ds_codeid;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    fastui.windowPopup('file-version', lang.file.context['version'] + ' - ' + name + extension, '/web/drive/file/file-version.html?id=' + id + '&codeid=' + codeid + '&extension=' + extension, 800, 500);
}


// 文件自定义菜单项目(上传新版本)
function fileContextMenuUpversion(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    fastui.windowPopup('file-upversion', lang.file.context['upversion'] + ' - ' + name + extension, '/web/drive/file/file-upversion.html?id=' + id + '&extension=' + extension + '&index=' + index, 800, 500);
}


// 文件自定义菜单项目(替换为当前版本)
function fileContextMenuReplace(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['replace'] + '<br />' + name + extension, 'fileContextMenuReplaceOk(' + id + ')');
}


// 文件自定义菜单项目(替换为当前版本提交)
function fileContextMenuReplaceOk(versionId) {
    var id = $query('id');
    var data = 'id=' + id + '&versionid=' + versionId;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/file/replace', 
        async: true, 
        data: data, 
        callback: function(data) {
            if (data == 'complete') {
                window.parent.fastui.list.scrollDataLoad(window.parent.fastui.list.path, true);
            }

            dataListActionCallback(data);
            }
        });
}


// 文件自定义菜单项目(操作日志)
function fileContextMenuLog(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    fastui.windowPopup('activity-log', lang.file.context['log'] + ' - ' + name + extension, '/web/drive/activity/activity-log.html?id=' + id, 800, 500);
}


// 文件自定义菜单项目(详细属性)
function fileContextMenuDetail(index) {
    var id = jsonData.items[index].ds_id;
    var name = jsonData.items[index].ds_name;
    var extension = jsonData.items[index].ds_extension;

    windowObject.fastui.windowPopup('file-detail', lang.file.context['detail'] + ' - ' + name + extension, '/web/drive/file/file-detail.html?id=' + id, 800, 500);
}