// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/web/drive/file/folder-data-json.ashx?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if (data.length == 0 || data == '{}') {
                fastui.coverTips(lang.file.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('name').innerHTML = jsonData.dbs_name;
                $id('remark').innerHTML = jsonData.dbs_remark;
                $id('share').innerHTML = jsonData.dbs_share == 1 ? lang.file.data.status['yes'] : lang.file.data.status['no'];
                $id('lock').innerHTML = jsonData.dbs_lock == 1 ? lang.file.data.status['yes'] : lang.file.data.status['no'];
                $id('owner').innerHTML = '<a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonData.dbs_username)) + '\', 500, 500);\">' + jsonData.dbs_username + '</a>';
                $id('create').innerHTML = '<a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonData.dbs_createusername)) + '\', 500, 500);\">' + jsonData.dbs_createusername + '</a>&nbsp;&nbsp;<font color=\"#757575\">' + jsonData.dbs_createtime + '</font>';
                $id('update').innerHTML = '<a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonData.dbs_updateusername)) + '\', 500, 500);\">' + jsonData.dbs_updateusername + '</a>&nbsp;&nbsp;<font color=\"#757575\">' + jsonData.dbs_updatetime + '</font>';

                folderPath(jsonData.dbs_folderid);
                folderTotal(jsonData.dbs_id);
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
        url: '/web/drive/file/folder-path-json.ashx?folderid=' + folderId, 
        async: true, 
        callback: function(data) {
            var path = '';
            var i;

            if (data.length == 0 || data == '[]') {
                $id('path').innerHTML = ' / ';
            } else {
                window.eval('var jsonDataPath = {items:' + data + '};');

                for (i = 0; i < jsonDataPath.items.length; i++) {
                    path = ' / ' + jsonDataPath.items[i].dbs_name + '' + path;
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
        url: '/web/drive/file/folder-total-json.ashx?id=' + id, 
        async: true, 
        callback: function(data) {
            if (data.length > 0 && data != '{}') {
                window.eval('var jsonDataTotal = ' + data + ';');

                $id('used-space').innerHTML = folderSize(jsonDataTotal.occupy_space);
                $id('folder-count').innerHTML = jsonDataTotal.folder_count;
                $id('file-count').innerHTML = jsonDataTotal.file_count;
            }
            }
        });
}