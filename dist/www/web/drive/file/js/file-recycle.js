$include('/storage/data/file-type-json.js');


// 页面初始化事件调用函数
function init() {
    dataListPageInit();
    dataListPageView();

    fastui.list.scrollDataLoad('/web/drive/file/file-recycle-list-json.ashx');
}


// 页面调整事件调用函数
function resize() {
    dataListPageInit();
}


// 数据列表页面初始化
function dataListPageInit() {
    // 列表页面布局调整
    fastui.list.layoutResize();
}


// 数据列表页面视图
function dataListPageView() {
    var items = $id('page-menu').$class('item');
    var timestamp = $query('timestamp');
    var i;

    if (timestamp.length == 0) {
        items[0].className = 'item-current';
        return;
    }

    for (i = 0; i < items.length; i++) {
        if (items[i].$get('onClick').toString().indexOf(timestamp) > -1) {
            items[i].className = 'item-current';
            break;
        }
    }
}


// 数据列表项目还原(询问)
function dataListItemRestore(index, folder) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    fastui.dialogConfirm(lang.file.tips.confirm['restore'] + '<br />' + name + extension, 'dataListItemRestoreOk(' + id + ', ' + folder + ')');
}


// 数据列表项目还原(提交)
function dataListItemRestoreOk(id, folder) {
    var url = folder == true ? '/web/drive/file/folder-action.ashx?action=restore' : '/web/drive/file/file-action.ashx?action=restore';
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


// 数据列表项目删除(询问)
function dataListItemDelete(index, folder) {
    var id = jsonData.items[index].dbs_id;
    var name = jsonData.items[index].dbs_name;
    var extension = jsonData.items[index].dbs_extension;

    if ($name('deletable')[index].value == 'false') {
        fastui.textTips(lang.file.tips['not-allow-delete']);
        return;
    }

    fastui.dialogConfirm(lang.file.tips.confirm['delete'] + '<br />' + name + extension, 'dataListItemDeleteOk(' + id + ', ' + folder + ')');
}


// 数据列表项目删除(提交)
function dataListItemDeleteOk(id, folder) {
    var url = folder == true ? '/web/drive/file/folder-action.ashx?action=delete' : '/web/drive/file/file-action.ashx?action=delete';
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
    var deletables = $name('deletable');
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
            if (action == 'deleteall' && deletables[i].value == 'false') {
                continue;
            }

            if (start == -1) {
                start = i;
            }

            end = i;
        }
    }

    if (start == -1 && end == -1) {
        fastui.textTips(lang.file.tips['not-allow-delete']);
        return;
    }

    switch(action) {
        case 'restoreall':
        tips = lang.file.tips.confirm['restore-all'];
        break;

        case 'deleteall':
        tips = lang.file.tips.confirm['delete-all'];
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
    var deletable;
    var i;
    var j;

    if (typeof(jsonData) == 'undefined') {
        return;
    }

    if (field.length == 0) {
        fastui.list.sortChange('removetime', true);
    } else {
        jsonData.items.sort($jsonSort(field, reverse, primer));

        fastui.list.sortChange(field.substring(field.indexOf('_') + 1), reverse);
    }

    j = list.rows.length;

    for (i = j; i < jsonData.items.length; i++) {
        row = list.addRow(i);

        // 判断项目是否可以彻底删除
        if (jsonData.items[i].dbs_share == 0) {
            deletable = true;
        } else {
            if (Math.floor((window.top.systemDate.getTime() - new Date(jsonData.items[i].dbs_removetime.replace(/\-/g, '/')).getTime()) / (1000 * 60 * 60 * 24)) > 30) {
                deletable = true;
            } else {
                deletable = false;
            }
        }

        (function(index) {row.ondblclick = function() {
            if (jsonData.items[i].dbs_folder == 1) {
                fastui.windowPopup('folder-detail', lang.file.context['detail'] + ' - ' + jsonData.items[index].dbs_name, '/web/drive/file/folder-detail.html?id=' + jsonData.items[index].dbs_id, 800, 500);
            } else {
                fastui.windowPopup('file-detail', lang.file.context['detail'] + ' - ' + jsonData.items[index].dbs_name + jsonData.items[index].dbs_extension, '/web/drive/file/file-detail.html?id=' + jsonData.items[index].dbs_id, 800, 500);
            }
            };})(i);

        // 复选框
        row.addCell(
            {width: '20', align: 'center'}, 
            '<input name=\"id\" type=\"checkbox\" class=\"checkbox\" value=\"' + jsonData.items[i].dbs_id + '\" />'
            );

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
                    html += '<a href=\"javascript: fastui.windowPopup(\'folder-detail\', \'' + lang.file.context['detail'] + ' - ' + jsonData.items[i].dbs_name + '\', \'/web/drive/file/folder-detail.html?id=' + jsonData.items[i].dbs_id + '\', 800, 500);\" title=\"' + jsonData.items[i].dbs_name + '\">';

                    if (deletable == true) {
                        html += '' + jsonData.items[i].dbs_name + '';
                    } else {
                        html += '<font color=\"#757575\">' + jsonData.items[i].dbs_name + '</font>';
                    }

                    html += '</a>';
                } else {
                    html += '<a href=\"javascript: fastui.windowPopup(\'file-detail\', \'' + lang.file.context['detail'] + ' - ' + jsonData.items[i].dbs_name + '' + jsonData.items[i].dbs_extension + '\', \'/web/drive/file/file-detail.html?id=' + jsonData.items[i].dbs_id + '\', 800, 500);\" title=\"' + jsonData.items[i].dbs_name + '' + jsonData.items[i].dbs_extension + '\">';

                    if (deletable == true) {
                        html += '' + jsonData.items[i].dbs_name + '' + jsonData.items[i].dbs_extension + '';
                    } else {
                        html += '<font color=\"#757575\">' + jsonData.items[i].dbs_name + '' + jsonData.items[i].dbs_extension + '</font>';
                    }

                    html += '</a>';
                }

                html += '&nbsp;&nbsp;';
                html += '<span class=\"version\">v' + jsonData.items[i].dbs_version + '</span>';
                html +='<input name=\"deletable\" type=\"hidden\" value=\"' + deletable + '\" />';

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

        // 移除时间
        row.addCell(
            {width: '250'}, 
            function() {
                var html = '';

                html += '' + jsonData.items[i].dbs_removetime + '';
                html += '<br />';
                html += '' + fileEvent('remove', jsonData.items[i].dbs_removeusername, jsonData.items[i].dbs_removetime) + '';

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