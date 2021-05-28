var purview = '';
var share = 0;
var lock = 0;
var recycle = 0;
var selectable = true;


$include('/storage/data/file-type-json.js');


// 页面初始化事件调用函数
function init() {
    dataListPageInit();
    dataListPageView();
}


// 页面调整事件调用函数
function resize() {
    dataListPageInit();
}


// 数据列表页面初始化
function dataListPageInit() {
    var keyword = $query('keyword');

    // 列表页面布局调整
    fastui.list.layoutResize();

    if (keyword.length == 0) {
        fastui.valueTips('keyword', lang.file.tips.value['keyword']);
    } else {
        $id('keyword').value = keyword;
    }
}


// 数据列表页面视图
function dataListPageView() {
    var folderId = $query('folderid') || 0;

    if ($query('keyword').length > 0) {
        $id('button-folder-add').style.display = 'none';
        $id('button-upload').style.display = 'none';
    }

    if (folderId == 0) {
        if ($query('area') == 'share') {
            $id('button-upload').style.display = 'none';
        }

        dataListFilter();

        window.setTimeout(function() {
            dataListLocation();
            }, 0);

        fastui.list.scrollDataLoad('/api/drive/file/list-json');
    } else {
        $id('button-list').style.display = 'none';

        // 获取文件夹及用户权限
        $ajax({
            type: 'GET', 
            url: '/api/drive/file/list-attribute-json?id=' + folderId + '&folder=true', 
            async: true, 
            callback: function(data) {
                var removed = false;

                if ($jsonString(data) == false) {
                    removed = true;
                } else {
                    window.eval('var attributeDataJson = ' + data + ';');

                    purview = attributeDataJson.purview;
                    share = attributeDataJson.share;
                    lock = attributeDataJson.lock;
                    recycle = attributeDataJson.recycle;
                }

                if (purviewCheck(purview, 'manager') == false || lock == 1) {
                    $id('button-folder-add').style.display = 'none';
                    $id('button-move').style.display = 'none';
                    $id('button-remove').style.display = 'none';
                }

                if (purviewCheck(purview, 'uploader') == false || lock == 1) {
                    $id('button-upload').style.display = 'none';
                }

                if (purviewCheck(purview, 'downloader') == false) {
                    $id('button-download').style.display = 'none';
                }

                if (purview == 'viewer' || lock == 1) {
                    selectable = false;
                }

                if (recycle == 1 || removed == true) {
                    $cookie('recent-folder-position', '');

                    fastui.coverTips(lang.file.tips['current-folder-removed']);
                    return;
                }

                $id('button-list').style.display = 'block';

                dataListFilter();

                window.setTimeout(function() {
                    dataListLocation();
                    }, 0);

                fastui.list.scrollDataLoad('/api/drive/file/list-json');
                }
            });
    }
}


// 数据列表过滤
function dataListFilter() {
    var type = $query('type');
    var size = $query('size');
    var time = $query('time');
    var items;

    if (type.length == 0) {
        items = '<li class="item-current">' + lang.file['unlimited'] + '</li>';
    } else {
        items = '<li onClick="$location(\'type\', \'\');">' + lang.file['unlimited'] + '</li>';
    }

    for (var i in lang.type) {
        if (i != 'folder') {
            if (i == type) {
                items += '<li class="item-current">' + lang.type[i] + '</li>';
            } else {
                items += '<li onClick="$location(\'type\', \'' + i + '\');">' + lang.type[i] + '</li>';
            }
        }
    }

    $id('filter-type').innerHTML += '<ul>' + items + '</ul>';

    if (size.length == 0) {
        items = '<li class="item-current">' + lang.file['unlimited'] + '</li>';
    } else {
        items = '<li onClick="$location(\'size\', \'\');">' + lang.file['unlimited'] + '</li>';
    }

    for (var j in lang.size) {
        if (j == size) {
            items += '<li class="item-current">' + lang.size[j] + '</li>';
        } else {
            items += '<li onClick="$location(\'size\', \'' + j + '\');">' + lang.size[j] + '</li>';
        }
    }

    $id('filter-size').innerHTML += '<ul>' + items + '</ul>';

    if (time.length == 0) {
        items = '<li class="item-current">' + lang.file['unlimited'] + '</li>';
    } else {
        items = '<li onClick="$location(\'time\', \'\');">' + lang.file['unlimited'] + '</li>';
    }

    for (var k in lang.time) {
        if (k == time) {
            items += '<li class="item-current">' + lang.time[k] + '</li>';
        } else {
            items += '<li onClick="$location(\'time\', \'' + k + '\');">' + lang.time[k] + '</li>';
        }
    }

    $id('filter-time').innerHTML += '<ul>' + items + '</ul>';
}


// 数据列表路径
function dataListLocation() {
    var folderId = $query('folderid') || 0;
    var path = $id('location').$class('path')[0];
    var tree = window.parent.$id('tree-iframe').contentWindow;

    if ($query('keyword').length > 0) {
        path.innerHTML += '<a href="javascript: $location(\'keyword\', \'\');">' + lang.file['exit-query'] + '</a>'
        return;
    }

    if (folderId == 0) {
        path.innerHTML += lang.file['all'];
    } else {
        path.innerHTML += '<a href="javascript: $location(\'folderid\', 0);">' + lang.file['all'] + '</a>';
    }

    if (folderId == 0) {
        if (typeof(tree.jsonData) != 'undefined') {
            tree.folderTree();

            window.parent.uploadToFolder(0, '');
        }

        $cookie('recent-folder-position', '');
        return;
    }

    $ajax({
        type: 'GET', 
        url: '/api/drive/folder/path-json?folderid=' + folderId, 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == true) {
                var location = '';
                var position = '';

                window.eval('var pathDataJson = {items:' + data + '};');

                for (var i = 0; i < pathDataJson.items.length; i++) {
                    if (i == 0) {
                        // 上传目标文件夹设置
                        window.parent.uploadToFolder(pathDataJson.items[i].ds_id, pathDataJson.items[i].ds_name);

                        location = ' / ' + pathDataJson.items[i].ds_name + '' + location;
                    } else {
                        location = ' / <a href="javascript: $location(\'folderid\', ' + pathDataJson.items[i].ds_id + ');">' + pathDataJson.items[i].ds_name + '</a>' + location;
                    }

                    position = pathDataJson.items[i].ds_id + ',' + position;
                }

                position = position.length == 0 ? '' : position.substring(0, position.length - 1);

                // 文件夹树形目录选择
                try {
                    tree.folderTree();
                    tree.folderTreeSelected(position);
                    } catch(e) {}

                $id('position').value = position;

                path.innerHTML += location;

                // 判读目录是否有效(保存最近浏览目录)
                if ($id('cover-tips') == null) {
                    $cookie('recent-folder-position', position);
                }
            }
            }
        });
}


// 数据列表下载全部(弹出下载打包窗口)
function dataListDownloadAll() {
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

    fastui.windowPopup('file-download-package', '', '/web/drive/file/file-download-package.html?' + data, 400, 140);
}


// 数据列表分享全部(弹出链接分享窗口)
function dataListLinkAll() {
    var checkboxes = $name('id');
    var title = '';
    var data = '';

    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            if (title.length == 0) {
                title = jsonData.items[i].ds_name + jsonData.items[i].ds_extension + '(' + lang.file['and-other-file'] + ')';
            }

            data += 'id=' + checkboxes[i].value + '&';
        }
    }

    if (data.length == 0) {
        fastui.textTips(lang.file.tips['please-select-item']);
        return;
    }

    data = data.substring(0, data.length - 1);

    fastui.windowPopup('link-share', lang.file.context['link'], '/web/drive/link/link-share.html?title=' + encodeURI(encodeURIComponent(title)) + '&' + data, 800, 500);
}


// 数据列表移动全部(弹出文件夹选择窗口)
function dataListMoveAll() {
    var checkboxes = $name('id');
    var selected = false;

    // 检查是否已选择项目
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            selected = true;
            break;
        }
    }

    if (selected == false) {
        fastui.textTips(lang.file.tips['please-select-item']);
        return;
    }

    var position = $id('position');

    if (position != null) {
        position = position.value;
    }

    fastui.windowPopup('folder-select', lang.file.context['move'], '/web/drive/folder/folder-select.html?folderid=' + $query('folderid') + '&position=' + position + '&callback=dataListMoveAllCallback&source=false&lock=false', 800, 500);
}


// 数据列表移动全部(回调函数)
function dataListMoveAllCallback(folderId, folderName) {
    $id('move-to-id').value = folderId;
    dataListAction('move-all');
}


// 数据列表操作(询问)
function dataListAction(action) {
    var checkboxes = $name('id');
    var selected = false;
    var start = -1;
    var end = -1;
    var tips;

    // 检查是否已选择项目
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            selected = true;
            break;
        }
    }

    if (selected == false) {
        fastui.textTips(lang.file.tips['please-select-item']);
        return;
    }

    // 计算操作项目开始及结束索引
    for (var j = 0; j < checkboxes.length; j++) {
        if (checkboxes[j].checked == true) {
            if (start == -1) {
                start = j;
            }

            end = j;
        }
    }

    switch(action) {
        case 'move-all':
        tips = lang.file.tips.confirm['move-all'];
        break;

        case 'remove-all':
        tips = lang.file.tips.confirm['remove-all'];
        break;
    }

    fastui.dialogConfirm(tips, 'dataListActionOk(\'' + action + '\', ' + start + ', ' + end + ', true)');
}


// 数据列表操作(提交)
function dataListActionOk(action, start, end, cover) {
    var checkboxes = $name('id');
    var folderId = 0;
    var url = '';
    var data = '';

    if (cover == true) {
        fastui.coverShow('waiting-cover');
    }

    for (var i = start; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            if (jsonData.items[i].ds_folder == 1) {
                url = '/api/drive/folder/' + action;
            } else {
                url = '/api/drive/file/' + action;
            }

            // 计算操作数据
            if (jsonData.items[i].ds_folder == 1) {
                data = 'id=' + checkboxes[i].value;
            } else {
                // 批量文件操作
                for (var j = i, n = 1; j < checkboxes.length; j++) {
                    if (checkboxes[j].checked == true) {
                        if (jsonData.items[j].ds_folder == 1) {
                            i--;
                            break;
                        } else {
                            data += 'id=' + checkboxes[j].value + '&';

                            // 批量操作文件数量限制
                            if (n == 50) {
                                break;
                            }

                            i++;
                            n++;
                        }
                    }
                }

                data = data.substring(0, data.length - 1);
            }

            if (action == 'move-all') {
                folderId = $id('move-to-id').value;

                data += '&folderid=' + folderId;
            }

            // 提交处理
            $ajax({
                type: 'POST', 
                url: url, 
                async: true, 
                data: data, 
                callback: function(data) {
                    if (i < end) {
                        // 继续处理未完成项目
                        dataListActionOk(action, i + 1, end, false);
                    } else {
                        if (action == 'move-all') {
                            if (data == 'complete') {
                                fastui.textTips(lang.file.tips['move-complete'].replace(/\{folderid\}/, folderId), 0, 8);
                            }
                        }

                        // 操作完成
                        dataListActionCallback('complete');
                    }
                    }
                });

            break;
        }
    }
}


// 数据列表操作(回调函数)
function dataListActionCallback(data) {
    fastui.coverHide('waiting-cover');

    if (data == 'complete') {
        fastui.list.scrollDataLoad(fastui.list.path, true);

        fastui.iconTips('tick');
    } else if (data == 'no-permission') {
        fastui.textTips(lang.file.tips['no-permission']);
    } else {
        fastui.iconTips('cross');
    }
}


// 数据列表视图
function dataListView(field, reverse, primer) {
    var list = fastui.list.dataTable('data-list');

    if (typeof(jsonData) == 'undefined') {
        return;
    }

    if (field.length == 0) {
        fastui.list.sortChange('name', false);
    } else {
        jsonData.items.sort($jsonSort(field, reverse, primer));

        fastui.list.sortChange(field.substring(field.indexOf('_') + 1), reverse);
    }

    var rows = list.rows.length;

    for (var i = rows; i < jsonData.items.length; i++) {
        var row = list.addRow(i);

        (function(index) {row.ondblclick = function() {
            if (jsonData.items[index].ds_folder == 1) {
                $location('folderid', jsonData.items[index].ds_id);
            } else {
                fileView(index, 0, 0);
            }
            };})(i);

        // 复选框
        row.addCell(
            {width: '20', align: 'center'}, 
            function() {
                if (selectable == true) {
                    return '<input name="id" type="checkbox" id="file_' + jsonData.items[i].ds_id + '" value="' + jsonData.items[i].ds_id + '" /><label for="file_' + jsonData.items[i].ds_id + '"></label>';
                } else {
                    return '<input name="id" type="checkbox" id="file_' + jsonData.items[i].ds_id + '" value="' + jsonData.items[i].ds_id + '" disabled="disabled" /><label for="file_' + jsonData.items[i].ds_id + '"></label>';
                }
            });

        // 图标
        row.addCell(
            {width: '32'}, 
            function() {
                var html = '';

                html += '<div class="list-icon">';

                if (jsonData.items[i].ds_folder == 1) {
                    html += '<span class="image"><img src="/ui/images/datalist-folder-icon.png" width="32" /></span>';
                } else {
                    html += '<span class="image"><img src="' + fileIcon(jsonData.items[i].ds_extension) + '" width="32" /></span>';
                }

                if (jsonData.items[i].ds_share == 1) {
                    html += '<span class="share"><img src="/ui/images/datalist-drive-share-icon.png" width="14" /></span>';
                }

                if (jsonData.items[i].ds_lock == 1) {
                    html += '<span class="lock"><img src="/ui/images/datalist-drive-lock-icon.png" width="10" /></span>';
                }

                html += '</div>';

                return html;
            });

        // 名称
        row.addCell(
            null, 
            function() {
                var html = '';

                if (jsonData.items[i].ds_folder == 1) {
                    html += '<a href="javascript: $location(\'folderid\', ' + jsonData.items[i].ds_id + ');" title="' + jsonData.items[i].ds_name + '">' + jsonData.items[i].ds_name + '</a>';
                } else {
                    html += '<a href="javascript: fileView(' + i + ', 0, 0);" title="' + jsonData.items[i].ds_name + '' + jsonData.items[i].ds_extension + '">' + jsonData.items[i].ds_name + '' + jsonData.items[i].ds_extension + '</a>';
                    html += '&nbsp;&nbsp;';
                    html += '<span class="version">v' + jsonData.items[i].ds_version + '</span>';
                }

                return html;
            });

        // 大小
        row.addCell(
            {width: '100'}, 
            function() {
                if (jsonData.items[i].ds_folder == 1) {
                    return '';
                } else {
                    return '' + fileSize(jsonData.items[i].ds_size) + '';
                }
                });

        // 类型
        row.addCell(
            {width: '100'}, 
            function() {
                if (jsonData.items[i].ds_folder == 1) {
                    return '' + lang.type['folder'] + '';
                } else {
                    return '' + fileType(jsonData.items[i].ds_extension) + '';
                }
            });

        // 更新时间
        row.addCell(
            {width: '250'}, 
            function() {
                var html = '';

                html += '' + jsonData.items[i].ds_updatetime + '';
                html += '<br />';
                html += '' + fileEvent('update', jsonData.items[i].ds_updateusername, jsonData.items[i].ds_updatetime) + '';

                return html;
            });

        // 操作
        row.addCell(
            {width: '50'}, 
            function() {
                var html = '';

                html += '<div class="datalist-action">';
                html += '<span class="button" onClick="dataListContextMenuClickEvent(' + i + ', event);">﹀</span>';
                html += '</div>';

                return html;
            });

        if (i - rows == fastui.list.size) {
            break;
        }
    }

    // 数据列表事件绑定(右击弹出菜单)
    dataListContextMenuEvent(rows);

    // 数据列表事件绑定(点击选择项目)
    fastui.list.bindEvent(rows);

    // 数据列表分块加载(应用于数据重载)
    fastui.list.scrollLoadBlock(field, reverse, primer, i);
}


// 文件夹树形目录重新加载
function folderTreeReload() {
    try {
        window.parent.$id('tree-iframe').contentWindow.dataLoad(true);
        } catch(e) {}
}