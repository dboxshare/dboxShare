// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/link/data-json?id=' + $query('id'), 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.link.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('title').innerHTML = jsonData.ds_title;
                $id('deadline').innerHTML = jsonData.ds_deadline;
                $id('password').innerHTML = jsonData.ds_password;
                $id('count').innerHTML = jsonData.ds_count;
                $id('user').innerHTML = '<a href="javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['user-card'] + '\', \'/web/account/user-card.html?username=' + encodeURI(encodeURIComponent(jsonData.ds_username)) + '\', 500, 500);">' + jsonData.ds_username + '</a>';
                $id('time').innerHTML = jsonData.ds_time;

                $id('link').innerHTML = window.location.protocol + '//' + window.location.host + '/web/link/?' + jsonData.ds_userid + '_' + jsonData.ds_codeid;

                if (jsonData.ds_revoke == 0) {
                    if (new Date(jsonData.ds_deadline.replace(/\-/g, '/')).getTime() > new Date().getTime()) {
                        $id('left-time').innerHTML = shareLeftTime(jsonData.ds_deadline);
                        $id('status').innerHTML = lang.link.data.status['sharing'];
                    } else {
                        $id('status').innerHTML = lang.link.data.status['expired'];
                    }
                } else {
                    $id('status').innerHTML = lang.link.data.status['revoked'];
                }

                fileList(jsonData.ds_id);
            }

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 分享文件列表
function fileList() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/link/share-file-list-json' + window.location.search, 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == true) {
                var items = '';

                window.eval('var fileDataJson = {items:' + data + '};');

                for (var i = 0; i < fileDataJson.items.length; i++) {
                    items += '<div class="item">';
                    items += '<span class="icon">';

                    if (fileDataJson.items[i].ds_folder == 1) {
                        items += '<img src="/ui/images/datalist-folder-icon.png" width="16" />';
                    } else {
                        items += '<img src="/ui/images/datalist-file-icon.png" width="16" />';
                    }

                    items += '</span>';
                    items += '<span class="name">';

                    if (fileDataJson.items[i].ds_folder == 1) {
                        items += '<a href="javascript: window.parent.fastui.windowPopup(\'folder-detail\', \'' + lang.file.context['detail'] + ' - ' + fileDataJson.items[i].ds_name + '\', \'/web/drive/folder/folder-detail.html?id=' + fileDataJson.items[i].ds_id + '\', 800, 500);" title="' + fileDataJson.items[i].ds_name + '">' + fileDataJson.items[i].ds_name + '</a>';
                    } else {
                        items += '<a href="javascript: window.parent.fastui.windowPopup(\'file-detail\', \'' + lang.file.context['detail'] + ' - ' + fileDataJson.items[i].ds_name + '' + fileDataJson.items[i].ds_extension + '\', \'/web/drive/file/file-detail.html?id=' + fileDataJson.items[i].ds_id + '\', 800, 500);" title="' + fileDataJson.items[i].ds_name + '' + fileDataJson.items[i].ds_extension + '">' + fileDataJson.items[i].ds_name + '' + fileDataJson.items[i].ds_extension + '</a>';
                        items += '&nbsp;&nbsp;';
                        items += 'v' + fileDataJson.items[i].ds_version + '';
                    }

                    items += '</span>';
                    items += '</div>';
                }

                $id('file-list').innerHTML = items;
            }
            }
        });
}


// 获取链接分享剩余时间
function shareLeftTime(deadline) {
    var ms = new Date(deadline.replace(/\-/g, '/')) - new Date().getTime();
    var second = 1000;
    var minute = second * 60;
    var hour = minute * 60;
    var day = hour * 24;
    var days = Math.floor(ms / day);
    var hours = Math.floor((ms - (day * days)) / hour);
    var minutes = Math.floor((ms - (day * days) - (hour * hours)) / minute);
    var seconds = Math.floor((ms - (day * days) - (hour * hours) - (minute * minutes)) / second);

    return days + ' ' + lang.link.data.time['day'] + ' ' + hours + ' ' + lang.link.data.time['hour'] + ' ' + minutes + ' ' + lang.link.data.time['minute'] + ' ' + seconds + ' ' + lang.link.data.time['second'];
}