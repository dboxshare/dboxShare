// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/folder/data-json?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.file.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('name').innerHTML = jsonData.ds_name;
                $id('remark').innerHTML = jsonData.ds_remark;
                $id('share').innerHTML = jsonData.ds_share == 1 ? lang.file.data.status['yes'] : lang.file.data.status['no'];
                $id('lock').innerHTML = jsonData.ds_lock == 1 ? lang.file.data.status['yes'] : lang.file.data.status['no'];
                $id('owner').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/user/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.ds_username)) + '\', 500, 500);">' + jsonData.ds_username + '</a>';
                $id('create').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/user/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.ds_createusername)) + '\', 500, 500);">' + jsonData.ds_createusername + '</a>&nbsp;&nbsp;<font color="#757575">' + jsonData.ds_createtime + '</font>';
                $id('update').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/user/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.ds_updateusername)) + '\', 500, 500);">' + jsonData.ds_updateusername + '</a>&nbsp;&nbsp;<font color="#757575">' + jsonData.ds_updatetime + '</font>';

                folderPath(jsonData.ds_folderid);
                folderTotal(jsonData.ds_id);
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
    }

    $ajax({
        type: 'GET', 
        url: '/api/drive/folder/path-json?folderid=' + folderId, 
        async: true, 
        callback: function(data) {
            if (data.length == 0 || data == '[]') {
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


// 获取文件夹大小
function folderSize(byte) {
    if (Math.ceil(byte / 1024) < 1024) {
        return Math.ceil(byte / 1024) + ' KB';
    } else if (Math.ceil(byte / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024).toFixed(2) + ' MB';
    } else if (Math.ceil(byte / 1024 / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024 / 1024).toFixed(2) + ' GB';
    }
}


// 文件夹统计
function folderTotal(id) {
    $ajax({
        type: 'GET', 
        url: '/api/drive/folder/total-json?id=' + id, 
        async: true, 
        callback: function(data) {
            if (data.length > 0 && data != '{}') {
                window.eval('var totalDataJson = ' + data + ';');

                $id('used-space').innerHTML = folderSize(totalDataJson.occupy_space);
                $id('folder-count').innerHTML = totalDataJson.folder_count;
                $id('file-count').innerHTML = totalDataJson.file_count;
            }
            }
        });
}