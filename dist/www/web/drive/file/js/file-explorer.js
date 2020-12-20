// 页面初始化事件调用函数
function init() {
    pageInit();
    treeInit();
    sideResize();
}


// 页面初始化
function pageInit() {
    // 初始化左侧目录树宽度(根据标签菜单边界)
    $id('tree').style.width = window.parent.$id('tab').$tag('span')[1].offsetLeft + 'px';
}


// 树形目录初始化
function treeInit() {
    var tree = $id('tree-iframe');

    if (tree == null) {
        return;
    }

    tree.src = '/web/drive/file/folder-tree.html';
}


// 文件列表初始化
function listInit() {
    var list = $id('list-iframe');
    var position = $cookie('recent-folder-position');
    var positions = new Array();

    if (list == null) {
        return;
    }

    if (position.length == 0) {
        list.src = '/web/drive/file/file-list.html';
    } else {
        positions = position.split(',');

        list.src = '/web/drive/file/file-list.html?folderid=' + positions[positions.length - 1];
    }
}


// 边界调整
function sideResize() {
    var side = $id('side');
    var list = $id('list');
    var treeMinWidth = window.parent.$id('tab').$tag('span')[1].offsetLeft;
    var treeMaxWidth = $id('explorer').offsetWidth / 2 < 640 ? 480 : 640;

    side.onmousedown = function(event) {
        var event = event || window.event;
        var cover = $id('cover');
        var tree = $id('tree');
        var list = $id('list');
        var treeW = tree.offsetWidth;
        var listW = list.offsetWidth;
        var mouseX = event.clientX;
        var isResizing = true;

        cover.style.display = 'block';

        cover.onmousemove = function(event) {
            if (isResizing == true) {
                var event = event || window.event;

                if (event.clientX < mouseX) {
                    if (treeW - (mouseX - event.clientX) < treeMinWidth) {
                        return;
                    }

                    tree.style.width = (treeW - (mouseX - event.clientX)) + 'px';
                    list.style.width = (listW + (mouseX - event.clientX)) + 'px';
                } else {
                    if (treeW + (event.clientX - mouseX) > treeMaxWidth) {
                        return;
                    }

                    tree.style.width = (treeW + (event.clientX - mouseX)) + 'px';
                    list.style.width = (listW - (event.clientX - mouseX)) + 'px';
                }

                treeW = tree.offsetWidth;
                listW = list.offsetWidth;
                mouseX = event.clientX;
            }
            };

        side.onmouseup = function() {
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

    list.contentWindow.fastui.textTips(lang.file.tips['upload-finished'], 5000, 8);

    list.contentWindow.dataListActionCallback('complete');
}