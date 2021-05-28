// 页面初始化事件调用函数
function init() {
    dataInit();
    dataReader();
}


// 数据初始化
function dataInit() {
    
}


// 数据读取
function dataReader() {
    var match = window.location.search.match(/(\d+)\_([\w]{16})/);

    if (match == null) {
        fastui.coverTips(lang.link.tips['illegal-operation']);
        return;
    }

    $ajax({
        type: 'GET', 
        url: '/api/link/link-data-json?userid=' + match[1] + '&codeid=' + match[2], 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.link.tips['unexist-or-error']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                if (jsonData.ds_revoke == 0) {
                    if (new Date(jsonData.ds_deadline.replace(/\-/g, '/')).getTime() > new Date().getTime()) {
                        $id('left-time').innerHTML = shareLeftTime(jsonData.ds_deadline);

                        fileList();
                    } else {
                        fastui.coverTips(lang.link.tips['link-expired']);
                    }
                } else {
                    fastui.coverTips(lang.link.tips['link-revoked']);
                }
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
        url: '/api/link/file-list-json' + window.location.search, 
        async: true, 
        callback: function(data) {
            if (data == 'link-login-failed') {
                $location('/web/link/' + window.location.search);
            } else if ($jsonString(data) == true) {
                var items = '';

                window.eval('var fileDataJson = {items:' + data + '};');

                for (var i = 0; i < fileDataJson.items.length; i++) {
                    items += '<div class="item">';
                    items += '<span class="box"><input name="id" type="checkbox" id="file_' + fileDataJson.items[i].ds_id + '" value="' + fileDataJson.items[i].ds_id + '" /><label for="file_' + fileDataJson.items[i].ds_id + '"></label></span>';
                    items += '<span class="icon">';

                    if (fileDataJson.items[i].ds_folder == 1) {
                        items += '<img src="/ui/images/datalist-folder-icon.png" width="24" />';
                    } else {
                        items += '<img src="/ui/images/datalist-file-icon.png" width="24" />';
                    }

                    items += '</span>';
                    items += '<span class="name">';

                    if (fileDataJson.items[i].ds_folder == 1) {
                        items += '<a href="javascript: $location(\'folderid\', ' + fileDataJson.items[i].ds_id + ');" title="' + fileDataJson.items[i].ds_name + '">' + fileDataJson.items[i].ds_name + '</a>';
                    } else {
                        items += '<a href="javascript: fileDownload(' + fileDataJson.items[i].ds_id + ', \'' + fileDataJson.items[i].ds_codeid + '\');" title="' + fileDataJson.items[i].ds_name + '' + fileDataJson.items[i].ds_extension + '">' + fileDataJson.items[i].ds_name + '' + fileDataJson.items[i].ds_extension + '</a>';
                    }

                    items += '</span>';

                    if (fileDataJson.items[i].ds_folder == 0) {
                        items += '<span class="version"><label class="number">v' + fileDataJson.items[i].ds_version + '</label></span>';
                        items += '<span class="size">' + fileSize(fileDataJson.items[i].ds_size) + '</span>';
                        items += '<span class="time">' + fileTime(fileDataJson.items[i].ds_createtime, fileDataJson.items[i].ds_updatetime) + '</span>';
                    }

                    items += '</div>';
                }

                $id('file-list').innerHTML = items;
            }
            }
        });
}


// 获取分享文件大小
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


// 获取分享文件更新时间
function fileTime(createTime, updateTime) {
    if (updateTime.length == 0) {
        var time = createTime;
    } else {
        var time = updateTime;
    }

    var ms = new Date().getTime() - new Date(time.replace(/\-/g, '/'));
    var second = 1000;
    var minute = second * 60;
    var hour = minute * 60;
    var day = hour * 24;
    var month = day * 30;
    var year = day * 365;

    if (Math.floor(ms / second) == 0) {
        return '0 ' + lang.timespan['second'];
    } else if (Math.floor(ms / second) < 60) {
        return Math.floor(ms / second) + ' ' + lang.timespan['second'];
    } else if (Math.floor(ms / minute) < 60) {
        return Math.floor(ms / minute) + ' ' + lang.timespan['minute'];
    } else if (Math.floor(ms / hour) < 24) {
        return Math.floor(ms / hour) + ' ' + lang.timespan['hour'];
    } else if (Math.floor(ms / day) < 30) {
        return Math.floor(ms / day) + ' ' + lang.timespan['day'];
    } else if (Math.floor(ms / month) < 12) {
        return Math.floor(ms / month) + ' ' + lang.timespan['month'];
    } else if (Math.floor(ms / year) > 0) {
        return Math.floor(ms / year) + ' ' + lang.timespan['year'];
    }
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


// 文件下载
function fileDownload(id, codeId) {
    var iframe = $id('file-download-iframe');

    if (iframe != null) {
        fastui.textTips(lang.file.tips['download-waiting'], 0);
        return;
    }

    $ajax({
        type: 'GET', 
        url: '/api/link/file-download?state=waiting', 
        async: true, 
        callback: function(data) {
            if (data == 'complete') {
                iframe = $create({
                    tag: 'iframe', 
                    attr: {
                        id: 'file-download-iframe', 
                        name: 'file-download-iframe', 
                        src: '/api/link/file-download?id=' + id + '&codeid=' + codeId, 
                        width: '0', 
                        height: '0', 
                        frameBorder: '0', 
                        onload: 'fileDownloadProgress();', 
                        onreadystatechange: 'fileDownloadProgress();'
                        }
                    }).$add(document.body);

                fastui.textTips(lang.file.tips['download-waiting'], 0);

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


// 文件进度
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


// 文件打包下载(弹出下载打包窗口)
function fileDownloadPackage() {
    var checkboxes = $name('id');
    var data = '';

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            data += 'id=' + checkboxes[i].value + '&';
        }
    }

    if (data.length == 0) {
        fastui.textTips(lang.file.tips['please-select-item']);
        return;
    }

    data = data.substring(0, data.length - 1);

    fastui.windowPopup('file-download-package', '', '/web/link/file-download-package.html?' + data, 400, 140);
}


// 选择全部
function selectAll(select) {
    var checkboxes = $name('id');

    for (var i = 0; i < checkboxes.length; i++) {
        checkboxes[i].checked = select.checked;
    }
}