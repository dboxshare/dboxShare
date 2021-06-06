$include('/storage/data/file-type-json.js');


// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/file/data-json?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.file.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('name').innerHTML = jsonData.ds_name;
                $id('extension').innerHTML = jsonData.ds_extension;
                $id('version').innerHTML = jsonData.ds_version;
                $id('type').innerHTML = fileType(jsonData.ds_extension);
                $id('size').innerHTML = fileSize(jsonData.ds_size);
                $id('remark').innerHTML = jsonData.ds_remark;
                $id('share').innerHTML = jsonData.ds_share == 1 ? lang.file.data.status['yes'] : lang.file.data.status['no'];
                $id('lock').innerHTML = jsonData.ds_lock == 1 ? lang.file.data.status['yes'] : lang.file.data.status['no'];
                $id('owner').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/account/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.ds_username)) + '\', 500, 500);">' + jsonData.ds_username + '</a>';
                $id('create').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/account/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.ds_createusername)) + '\', 500, 500);">' + jsonData.ds_createusername + '</a>&nbsp;&nbsp;' + jsonData.ds_createtime + '';
                $id('update').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/account/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.ds_updateusername)) + '\', 500, 500);">' + jsonData.ds_updateusername + '</a>&nbsp;&nbsp;' + jsonData.ds_updatetime + '';

                folderPath(jsonData.ds_folderid);
            }

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 获取文件夹路径
function folderPath(folderId) {
    if (folderId == 0) {
        $id('path').innerHTML = ' / ';
        return;
    }

    $ajax({
        type: 'GET', 
        url: '/api/drive/folder/path-json?folderid=' + folderId, 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                $id('path').innerHTML = ' / ';
            } else {
                window.eval('var pathDataJson = {items:' + data + '};');

                var path = '';

                for (var i = 0; i < pathDataJson.items.length; i++) {
                    path = ' / ' + pathDataJson.items[i].ds_name + '' + path;
                }

                $id('path').innerHTML = path;
            }
            }
        });
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