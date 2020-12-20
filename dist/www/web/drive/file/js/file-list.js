var purview = 'viewer';
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
    var keyword = $query('keyword');

    if (keyword.length > 0) {
        $id('button-folder-add').style.display = 'none';
        $id('button-upload').style.display = 'none';
    }

    if (folderId == 0) {
        $class('button-query-folder')[0].style.display = 'none';

        dataListFilter();

        window.setTimeout(function() {
            dataListLocation();
            }, 0);

        fastui.list.scrollDataLoad('/web/drive/file/file-list-json.ashx');
    } else {
        $id('button-list').style.display = 'none';

        // 获取文件夹及用户权限信息
        $ajax({
            type: 'GET', 
            url: '/web/drive/file/list-attribute-json.ashx?id=' + folderId + '&folder=true', 
            async: true, 
            callback: function(data) {
                var removed = false;

                if (data.length == 0) {
                    removed = true;
                } else {
                    window.eval('var jsonDataAttribute = ' + data + ';');

                    purview = jsonDataAttribute.purview;
                    lock = jsonDataAttribute.lock;
                    recycle = jsonDataAttribute.recycle;
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
                    $cookie('recent-folder-position', '', 0);

                    fastui.coverTips(lang.file.tips['current-folder-removed']);
                    return;
                }

                $id('button-list').style.display = 'block';

                dataListFilter();

                window.setTimeout(function() {
                    dataListLocation();
                    }, 0);

                fastui.list.scrollDataLoad('/web/drive/file/file-list-json.ashx');
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
    var i;

    if (type.length == 0) {
        items = '<li class=\"item-current\">' + lang.file['unlimited'] + '</li>';
    } else {
        items = '<li onClick=\"$location(\'type\', \'\');\">' + lang.file['unlimited'] + '</li>';
    }

    for (i in lang.type) {
        if (i != 'folder') {
            if (i == type) {
                items += '<li class=\"item-current\">' + lang.type[i] + '</li>';
            } else {
                items += '<li onClick=\"$location(\'type\', \'' + i + '\');\">' + lang.type[i] + '</li>';
            }
        }
    }

    $id('filter-type').innerHTML += '<ul>' + items + '</ul>';

    if (size.length == 0) {
        items = '<li class=\"item-current\">' + lang.file['unlimited'] + '</li>';
    } else {
        items = '<li onClick=\"$location(\'size\', \'\');\">' + lang.file['unlimited'] + '</li>';
    }

    for (i in lang.size) {
        if (i == size) {
            items += '<li class=\"item-current\">' + lang.size[i] + '</li>';
        } else {
            items += '<li onClick=\"$location(\'size\', \'' + i + '\');\">' + lang.size[i] + '</li>';
        }
    }

    $id('filter-size').innerHTML += '<ul>' + items + '</ul>';

    if (time.length == 0) {
        items = '<li class=\"item-current\">' + lang.file['unlimited'] + '</li>';
    } else {
        items = '<li onClick=\"$location(\'time\', \'\');\">' + lang.file['unlimited'] + '</li>';
    }

    for (i in lang.timestamp) {
        if (i == time) {
            items += '<li class=\"item-current\">' + lang.timestamp[i] + '</li>';
        } else {
            items += '<li onClick=\"$location(\'time\', \'' + i + '\');\">' + lang.timestamp[i] + '</li>';
        }
    }

    $id('filter-time').innerHTML += '<ul>' + items + '</ul>';
}


// 数据列表路径
function dataListLocation() {
    var folderId = $query('folderid') || 0;
    var keyword = $query('keyword');
    var path = $id('location').$class('path')[0];
    var tree = window.parent.$id('tree-iframe').contentWindow;
    var i;

    if (keyword.length > 0) {
        path.innerHTML += '<a href=\"/web/drive/file/file-list.html?folderid=' + folderId + '\">' + lang.file['exit-query'] + '</a>'
        return;
    }

    if (folderId == 0) {
        path.innerHTML += lang.file['all'];
    } else {
        path.innerHTML += '<a href=\"/web/drive/file/file-list.html\">' + lang.file['all'] + '</a>';
    }

    if (folderId == 0) {
        if (typeof(tree.jsonData) != 'undefined') {
            tree.folderTree();

            window.parent.uploadToFolder(0, '');
        }

        $cookie('recent-folder-position', '', 0);

        return;
    }

    $ajax({
        type: 'GET', 
        url: '/web/drive/file/folder-path-json.ashx?folderid=' + folderId, 
        async: true, 
        callback: function(data) {
            var location = '';
            var position = '';
            var i;

            if (data.length > 0) {
                window.eval('var jsonDataPath = {items:' + data + '};');

                for (i = 0; i < jsonDataPath.items.length; i++) {
                    if (i == 0) {
                        // 上传目标文件夹设置
                        window.parent.uploadToFolder(jsonDataPath.items[i].dbs_id, jsonDataPath.items[i].dbs_name);

                        location = ' / ' + jsonDataPath.items[i].dbs_name + '' + location;
                    } else {
                        location = ' / <a href=\"javascript: $location(\'folderid\', ' + jsonDataPath.items[i].dbs_id + ');\">' + jsonDataPath.items[i].dbs_name + '</a>' + location;
                    }

                    position = jsonDataPath.items[i].dbs_id + ',' + position;
                }

                position = position.length == 0 ? '' : position.substring(0, position.length - 1);

                // 文件夹树形目录选择
                try {
                    tree.folderTree();
                    tree.folderTreeSelected(position);
                    } catch(e) {}

                $id('position').value = position;

                path.innerHTML += location;

                if ($id('cover-tips') == null) {
                    $cookie('recent-folder-position', position, 0);
                }
            }
            }
        });
}


// 数据列表下载全部(弹出下载打包窗口)
function dataListDownloadAll() {
    var folderId = $query('folderid') || 0;
    var checkboxes = $name('id');
    var data = '';
    var i;

    for (i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            if (folderId == 0) {
                if (jsonData.items[i].dbs_folder == 1) {
                    fastui.textTips(lang.file.tips['download-folder-limit']);
                    return;
                }
            }

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


// 数据列表移动全部(弹出文件夹选择窗口)
function dataListMoveAll() {
    var checkboxes = $name('id');
    var selected = false;
    var i;

    // 检查是否已选择项目
    for (i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            selected = true;
            break;
        }
    }

    if (selected == false) {
        fastui.textTips(lang.file.tips['please-select-item']);
        return;
    }

    fastui.windowPopup('folder-select', lang.file.context['move'], '/web/drive/file/folder-select.html?folderid=' + $query('folderid') + '&position=' + $id('position').value + '&callback=dataListMoveAllCallback&source=false&lock=false', 800, 500);
}


// 数据列表移动全部(回调函数)
function dataListMoveAllCallback(folderId, folderName) {
    $id('move-to-id').value = folderId;
    dataListAction('moveall');
}


// 数据列表项目锁定(询问)
function dataListItemLock(index, folder) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['lock'] + '<br />' + name + extension, 'dataListItemLockOk(' + id + ', ' + folder + ')');
}


// 数据列表项目锁定(提交)
function dataListItemLockOk(id, folder) {
    var url = folder == true ? '/web/drive/file/folder-action.ashx?action=lock' : '/web/drive/file/file-action.ashx?action=lock';
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: url, 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 数据列表项目解除锁定(询问)
function dataListItemUnlock(index, folder) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['unlock'] + '<br />' + name + extension, 'dataListItemUnlockOk(' + id + ', ' + folder + ')');
}


// 数据列表项目解除锁定(提交)
function dataListItemUnlockOk(id, folder) {
    var url = folder == true ? '/web/drive/file/folder-action.ashx?action=unlock' : '/web/drive/file/file-action.ashx?action=unlock';
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: url, 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 数据列表项目移除(询问)
function dataListItemRemove(index, folder) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['remove'] + '<br />' + name + extension, 'dataListItemRemoveOk(' + id + ', ' + folder + ')');
}


// 数据列表项目移除(提交)
function dataListItemRemoveOk(id, folder) {
    var url = folder == true ? '/web/drive/file/folder-action.ashx?action=remove' : '/web/drive/file/file-action.ashx?action=remove';
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: url, 
        async: true, 
        data: data, 
        callback: 'dataListActionCallback'
        });
}


// 数据列表操作(询问)
function dataListAction(action) {
    var checkboxes = $name('id');
    var selected = false;
    var start = -1;
    var end = -1;
    var tips;
    var i;

    // 检查是否已选择项目
    for (i = 0; i < checkboxes.length; i++) {
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
    for (i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            if (start == -1) {
                start = i;
            }

            end = i;
        }
    }

    switch(action) {
        case 'moveall':
        tips = lang.file.tips.confirm['move-all'];
        break;

        case 'removeall':
        tips = lang.file.tips.confirm['remove-all'];
        break;
    }

    fastui.dialogConfirm(tips, 'dataListActionOk(\'' + action + '\', ' + start + ', ' + end + ', true)');
}


// 数据列表操作(提交)
function dataListActionOk(action, start, end, cover) {
    var checkboxes = $name('id');
    var url = '';
    var data = '';
    var i;
    var j;
    var n;

    if (cover == true) {
        fastui.coverShow('waiting-cover');
    }

    for (i = start; i < checkboxes.length; i++) {
        if (checkboxes[i].checked == true) {
            if (jsonData.items[i].dbs_folder == 1) {
                url = '/web/drive/file/folder-action.ashx?action=' + $match(action, /^(\w+)all$/);
            } else {
                url = '/web/drive/file/file-action.ashx?action=' + action;
            }

            // 计算操作数据
            if (jsonData.items[i].dbs_folder == 1) {
                data = 'id=' + checkboxes[i].value;
            } else {
                // 批量文件操作
                for (j = i, n = 1; j < checkboxes.length; j++) {
                    if (checkboxes[j].checked == true) {
                        if (jsonData.items[j].dbs_folder == 1) {
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

            if (action == 'moveall') {
                data += '&folderid=' + $id('move-to-id').value;
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
    var row;
    var i;
    var j;

    if (typeof(jsonData) == 'undefined') {
        return;
    }

    if (field.length == 0) {
        fastui.list.sortChange('name', false);
    } else {
        jsonData.items.sort($jsonSort(field, reverse, primer));

        fastui.list.sortChange(field.substring(field.indexOf('_') + 1), reverse);
    }

    j = list.rows.length;

    for (i = j; i < jsonData.items.length; i++) {
        row = list.addRow(i);

        (function(index) {row.ondblclick = function() {
            if (jsonData.items[index].dbs_folder == 1) {
                $location('folderid', jsonData.items[index].dbs_id);
            } else {
                fileView(index, 0, 0);
            }
            };})(i);

        // 复选框
        row.addCell(
            {width: '20', align: 'center'}, 
            function() {
                if (selectable == true) {
                    return '<input name=\"id\" type=\"checkbox\" class=\"checkbox\" value=\"' + jsonData.items[i].dbs_id + '\" />';
                } else {
                    return '<input name=\"id\" type=\"checkbox\" class=\"checkbox\" value=\"' + jsonData.items[i].dbs_id + '\" disabled=\"disabled\" />';
                }
            });

        // 图标
        row.addCell(
            {width: '32'}, 
            function() {
                var html = '';

                html += '<div class=\"list-icon\">';

                if (jsonData.items[i].dbs_folder == 1) {
                    html += '<span class=\"image\"><img src=\"/ui/images/datalist-folder-icon.png\" width=\"32\" /></span>';
                } else {
                    html += '<span class=\"image\"><img src=\"' + fileIcon(jsonData.items[i].dbs_extension) + '\" width=\"32\" /></span>';
                }

                if (jsonData.items[i].dbs_share == 1) {
                    html += '<span class=\"share\"><img src=\"/ui/images/datalist-drive-share-icon.png\" width=\"14\" /></span>';
                }

                if (jsonData.items[i].dbs_lock == 1) {
                    html += '<span class=\"lock\"><img src=\"/ui/images/datalist-drive-lock-icon.png\" width=\"10\" /></span>';
                }

                html += '</div>';

                return html;
            });

        // 名称
        row.addCell(
            null, 
            function() {
                var html = '';

                if (jsonData.items[i].dbs_folder == 1) {
                    html += '<a href=\"javascript: $location(\'folderid\', ' + jsonData.items[i].dbs_id + ');\" title=\"' + jsonData.items[i].dbs_name + '\">' + jsonData.items[i].dbs_name + '</a>';
                } else {
                    html += '<a href=\"javascript: fileView(' + i + ', 0, 0);\" title=\"' + jsonData.items[i].dbs_name + '' + jsonData.items[i].dbs_extension + '\">' + jsonData.items[i].dbs_name + '' + jsonData.items[i].dbs_extension + '</a>';
                    html += '&nbsp;&nbsp;';
                    html += '<span class=\"version\">v' + jsonData.items[i].dbs_version + '</span>';
                }

                return html;
            });

        // 大小
        row.addCell(
            {width: '75'}, 
            function() {
                if (jsonData.items[i].dbs_folder == 1) {
                    return '';
                } else {
                    return '' + fileSize(jsonData.items[i].dbs_size) + '';
                }
                });

        // 类型
        row.addCell(
            {width: '100'}, 
            function() {
                if (jsonData.items[i].dbs_folder == 1) {
                    return '' + lang.type['folder'] + '';
                } else {
                    return '' + fileType(jsonData.items[i].dbs_extension) + '';
                }
            });

        // 更新时间
        row.addCell(
            {width: '250'}, 
            function() {
                var html = '';

                html += '' + jsonData.items[i].dbs_updatetime + '';
                html += '<br />';
                html += '' + fileEvent('update', jsonData.items[i].dbs_updateusername, jsonData.items[i].dbs_updatetime) + '';

                return html;
            });

        // 操作
        row.addCell(
            {width: '50'}, 
            function() {
                var html = '';

                html += '<div class=\"datalist-action\">';
                html += '<span class=\"button\" onClick=\"dataListContextMenuClickEvent(' + i + ', event);\">﹀</span>';
                html += '</div>';

                return html;
            });

        if (i - j == fastui.list.size) {
            break;
        }
    }

    // 数据列表事件绑定(右击弹出菜单)
    dataListContextMenuEvent(j);

    // 数据列表事件绑定(点击选择项目)
    fastui.list.bindEvent(j);

    // 数据列表分块加载(应用于数据重载)
    fastui.list.scrollLoadBlock(field, reverse, primer, i);
}


// 文件夹树形目录重新加载
function folderTreeReload() {
    try {
        window.parent.$id('tree-iframe').contentWindow.dataLoad(true);
        } catch(e) {}
}