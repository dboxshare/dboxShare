// 页面初始化事件调用函数
function init() {
    dataListPageInit();

    fastui.list.scrollDataLoad('/web/drive/discuss/discuss-list-json.ashx', false, false, 0);
}


// 页面调整事件调用函数
function resize() {
    dataListPageInit();
}


// 数据列表页面初始化
function dataListPageInit() {
    // 列表页面布局调整
    fastui.list.layoutResize();

    fastui.valueTips('content', lang.discuss.tips.value['content']);
}


// 数据列表项目撤回(询问)
function dataListItemRevoke(index) {
    var id = jsonData.items[index].dbs_id;

    fastui.dialogConfirm(lang.discuss.tips.confirm['revoke'], 'dataListItemRevokeOk(' + id + ')');
}


// 数据列表项目撤回(提交)
function dataListItemRevokeOk(id) {
    var data = 'id=' + id;

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/discuss/discuss-action.ashx?action=revoke', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                fastui.list.scrollDataLoad(fastui.list.path, true);

                fastui.iconTips('tick');
            } else {
                fastui.iconTips('cross');
            }
            }
        });
}


// 数据列表视图
function dataListView(field, reverse, primer) {
    var container = $id('datalist-container');
    var position = container.scrollHeight;
    var list = fastui.list.dataTable('data-list');
    var row;
    var self;
    var i;
    var j;

    if (typeof(jsonData) == 'undefined') {
        return;
    }

    j = list.rows.length;

    for (i = j; i < jsonData.items.length; i++) {
        row = list.addRow(0);

        // 判断信息是否本人发送
        if (jsonData.items[i].dbs_userid == window.top.myId) {
            self = true;
        } else {
            self = false;
        }

        // 其他人
        row.addCell(
            {width: '100', align: 'center'}, 
            function() {
                var html = '';

                if (self == false) {
                    html += '<div class=\"avatar\"><img src=\"/ui/images/avatar-icon.png\" width=\"32\" /></div>';
                    html += '<div class=\"username\"><a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonData.items[i].dbs_username)) + '\', 500, 500);\">' + jsonData.items[i].dbs_username + '</a></div>';
                } else {
                    return '';
                }

                return html;
            });

        // 信息
        row.addCell(
            {align: self == true ? 'right' : 'left'}, 
            function() {
                var html = '';

                html += '<div class=\"time\">' + messageTime(jsonData.items[i].dbs_time) + '</div>';

                if (self == true) {
                    if (jsonData.items[i].dbs_revoke == 0) {
                        html += '<div class=\"content self\">';
                        html += '' + jsonData.items[i].dbs_content + '';

                        if (Math.floor((new Date().getTime() - new Date(jsonData.items[i].dbs_time.replace(/\-/g, '/')).getTime()) / (1000 * 60)) <= 20) {
                            html += '<span class=\"revoke\" title=\"' + lang.discuss.button['revoke'] + '\" onClick=\"dataListItemRevoke(' + i + ');\">✕</span>';
                        }

                        html += '</div>';
                    } else {
                        html += '<font color=\"#757575\">' + lang.discuss.tips['message-revoked'] + '</font>';
                    }
                } else {
                    if (jsonData.items[i].dbs_revoke == 0) {
                        html += '<div class=\"content other\">' + jsonData.items[i].dbs_content + '</div>';
                    } else {
                        html += '<font color=\"#757575\">' + lang.discuss.tips['message-revoked'] + '</font>';
                    }
                }

                return html;
            });

        // 本人
        row.addCell(
            {width: '100', align: 'center'}, 
            function() {
                if (self == true) {
                    return '<div class=\"avatar\"><img src=\"/ui/images/avatar-icon.png\" width=\"32\" /></div><div class=\"username\"><a href=\"javascript: window.parent.fastui.windowPopup(\'user-card\', \'' + lang.user['card'] + '\', \'/web/user/card.html?username=' + encodeURI(encodeURIComponent(jsonData.items[i].dbs_username)) + '\', 500, 500);\">' + jsonData.items[i].dbs_username + '</a></div>';
                } else {
                    return '';
                }
            });
    }

    if (j == 0) {
        container.scrollTop = container.scrollHeight;
    } else {
        container.scrollTop = container.scrollHeight - position;
    }
}


// 获取消息时间信息
function messageTime(time) {
    var ms = new Date().getTime() - new Date(time.replace(/\-/g, '/'));
    var second = 1000;
    var minute = second * 60;
    var hour = minute * 60;
    var day = hour * 24;
    var month = day * 30;
    var year = day * 365;

    if (Math.floor(ms / second) == 0) {
        return '0 ' + lang.timespan['second'] + ' ' + time;
    } else if (Math.floor(ms / second) < 60) {
        return Math.floor(ms / second) + ' ' + lang.timespan['second'] + ' ' + time;
    } else if (Math.floor(ms / minute) < 60) {
        return Math.floor(ms / minute) + ' ' + lang.timespan['minute'] + ' ' + time;
    } else if (Math.floor(ms / hour) < 24) {
        return Math.floor(ms / hour) + ' ' + lang.timespan['hour'] + ' ' + time;
    } else if (Math.floor(ms / day) < 30) {
        return Math.floor(ms / day) + ' ' + lang.timespan['day'] + ' ' + time;
    } else if (Math.floor(ms / month) < 12) {
        return Math.floor(ms / month) + ' ' + lang.timespan['month'] + ' ' + time;
    } else if (Math.floor(ms / year) > 0) {
        return Math.floor(ms / year) + ' ' + lang.timespan['year'] + ' ' + time;
    }
}


// 消息发表
function messagePost() {
    var id = $query('id');
    var content = $id('content').value;
    var data = '';

    data += 'id=' + id + '&';

    if (fastui.testInput(true, 'content', /^[\s\S]{1,200}$/) == false) {
        fastui.textTips(lang.discuss.tips.input['content']);
        return;
    } else {
        data += 'content=' + escape(content) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/drive/discuss/discuss-action.ashx?action=post', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                $id('content').value = '';

                fastui.list.scrollDataLoad(fastui.list.path, true, false, 0);
            } else {
                fastui.textTips(lang.discuss.tips['operation-failed']);
            }
            }
        });
}