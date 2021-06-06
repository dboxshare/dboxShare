// 页面初始化事件调用函数
function init() {
    pageInit();
    treeInit();
    resizeInit();
}


// 页面初始化
function pageInit() {
    // 初始化侧栏宽度(根据标签菜单边界)
    $id('sidebar').style.width = window.parent.$id('tab').$tag('span')[1].offsetLeft + 'px';
}


// 树形目录初始化
function treeInit() {
    var tree = $id('tree-iframe');

    if (tree == null) {
        return;
    }

    tree.style.height = ($page().client.height - $id('menu').offsetHeight) + 'px';

    tree.src = $url('/web/drive/folder/folder-tree.html');
}


// 文件列表初始化
function listInit() {
    var list = $id('list-iframe');
    var position = $cookie('recent-folder-position');

    if (list == null) {
        return;
    }

    if (position.length == 0) {
        list.src = $url('/web/drive/file/file-list.html');
    } else {
        var positions = position.split(',');

        list.src = $url('/web/drive/file/file-list.html?folderid=' + positions[positions.length - 1]);
    }
}


// 尺寸调整初始化
function resizeInit() {
    var divider = $id('divider');
    var sidebarMinWidth = window.parent.$id('tab').$tag('span')[1].offsetLeft;
    var sidebarMaxWidth = $id('explorer').offsetWidth / 2 < 640 ? 480 : 640;

    divider.onmousedown = function(e) {
        var event = e || window.event;
        var cover = $id('cover');
        var sidebar = $id('sidebar');
        var listbar = $id('listbar');
        var sidebarWidth = sidebar.offsetWidth;
        var listbarWidth = listbar.offsetWidth;
        var mouseX = event.clientX;
        var isResizing = true;

        cover.style.display = 'block';

        cover.onmousemove = function(e) {
            if (isResizing == true) {
                var event = e || window.event;

                if (event.clientX < mouseX) {
                    if (sidebarWidth - (mouseX - event.clientX) < sidebarMinWidth) {
                        return;
                    }

                    sidebar.style.width = (sidebarWidth - (mouseX - event.clientX)) + 'px';
                    listbar.style.width = (listbarWidth + (mouseX - event.clientX)) + 'px';
                } else {
                    if (sidebarWidth + (event.clientX - mouseX) > sidebarMaxWidth) {
                        return;
                    }

                    sidebar.style.width = (sidebarWidth + (event.clientX - mouseX)) + 'px';
                    listbar.style.width = (listbarWidth - (event.clientX - mouseX)) + 'px';
                }

                sidebarWidth = sidebar.offsetWidth;
                listbarWidth = listbar.offsetWidth;
                mouseX = event.clientX;
            }
            };

        divider.onmouseup = function() {
            cover.style.display = 'none';

            cover.onmousemove = null;

            isResizing = false;
            };

        cover.onmouseup = function() {
            cover.style.display = 'none';

            cover.onmousemove = null;

            isResizing = false;
            };

        return false;
        };
}


// 菜单项目点击转到指定路径
function menuGoto(path, e) {
    var event = e || window.event;
    var tag = event.target || event.srcElement;
    var list = $id('list-iframe');

    list.src = $url(path);

    // 切换菜单项目
    var menus = $id('menu').$class('item');

    for (var i = 0; i < menus.length; i++) {
        if (menus[i].$get('onClick').indexOf('\'' + path + '\'') > -1) {
            menus[i].style.backgroundColor = '#E0E0E0';
        } else {
            menus[i].style.backgroundColor = '';
        }
    }
}


// 上传窗口显示
function uploadWindowShow() {
    var upload = $id('upload-window');

    if (upload != null) {
        upload.style.display = 'block';
    }
}


// 上传窗口隐藏
function uploadWindowHide() {
    var upload = $id('upload-window');

    if (upload != null) {
        upload.style.display = 'none';
    }
}


// 上传目标文件夹(上传参数设置)
function uploadToFolder(folderId, folderName) {
    var upload = $id('upload-iframe');

    if (upload == null) {
        return;
    }

    upload.contentWindow.$id('folderid').value = folderId;
    upload.contentWindow.$id('foldername').value = folderName;
}


// 上传完成刷新文件列表
function uploadCompleteRefresh() {
    var list = $id('list-iframe');

    if (list == null) {
        return;
    }

    uploadWindowHide();

    list.contentWindow.dataListActionCallback('complete');
}