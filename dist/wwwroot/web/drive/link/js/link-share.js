// 页面初始化事件调用函数
function init() {
    dataInit();
}


// 数据初始化
function dataInit() {
    $tag('table')[1].style.display = 'none';

    if (window.location.search.match(/&id=\d+/ig).length == 1) {
        $id('file-list').parentNode.parentNode.style.display = 'none';
    } else {
        fileList();
    }

    $id('title').value = decodeURIComponent($query('title'));

    fastui.radioSelect('day-select', expiryDateJson(), 'value', 'name', '#2196F3');

    fastui.valueTips('title', lang.link.tips.value['title']);
    fastui.valueTips('email', lang.link.tips.value['email']);
    fastui.valueTips('message', lang.link.tips.value['message']);

    linkPassword();
}


// 分享文件列表
function fileList() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/link/file-list-json' + window.location.search, 
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
                        items += '' + fileDataJson.items[i].ds_name + '';
                    } else {
                        items += '' + fileDataJson.items[i].ds_name + '' + fileDataJson.items[i].ds_extension + '';
                        items += '&nbsp;&nbsp;';
                        items += 'v' + fileDataJson.items[i].ds_version + '';
                    }

                    items += '</span>';
                    items += '<span class="delete" onClick="fileDelete(' + fileDataJson.items[i].ds_id + ');">✕</span>';
                    items += '<input name="fileid" type="hidden" value="' + fileDataJson.items[i].ds_id + '" />';
                    items += '</div>';
                }

                $id('file-list').innerHTML = items;
            }
            }
        });
}


// 分享文件列表项目删除
function fileDelete(id) {
    var files = $name('fileid');

    for (var i = 0; i < files.length; i++) {
        if (files[i].value == id) {
            $remove(files[i].parentNode);
            return;
        }
    }
}


// 生成有效期数据
function expiryDateJson() {
    var days = '1,2,3,5,7,14,21,28';
    var json = '';

    days = days.split(',');

    for (var i = 0; i < days.length; i++) {
        json += '{\'value\':\'' + days[i] + '\',\'name\':\'' + days[i] + ' ' + lang.link.data.time['day'] + '\'},';
    }

    json = json.substring(0, json.length - 1);

    json = window.eval('[' + json + ']');

    return json;
}


// 生成链接分享提取密码
function linkPassword() {
    var chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    var key = "";

    for (var i = 0; i < 6; i++) {
        key += chars.charAt(Math.floor(Math.random() * chars.length));
    }

    $id("password").value = key;
}


// 生成链接分享代码标识
function linkCodeId() {
    var chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    var code = "";

    for (var i = 0; i < 16; i++) {
        code += chars.charAt(Math.floor(Math.random() * chars.length));
    }

    return code;
}


// 链接分享表单提交
function linkShare() {
    var codeId = linkCodeId();
    var files = $name('fileid');
    var title = $id('title').value;
    var day = $id('day').value;
    var password = $id('password').value;
    var email = $id('email').value;
    var message = $id('message').value;
    var data = '';

    if (files.length == 0) {
        var id = window.location.search.match(/id=([1-9]{1}[\d]*)/g);

        if (id.length > 1) {
            fastui.textTips(lang.link.tips['operation-failed']);
            return;
        }

        var fileId = $query('id');

        if (fastui.testString(fileId, /^[1-9]{1}[\d]*$/) == false) {
            fastui.textTips(lang.link.tips['illegal-operation']);
            return;
        } else {
            data += 'fileid=' + fileId + '&';
        }
    } else {
        for (var i = 0; i < files.length; i++) {
            data += 'fileid=' + files[i].value + '&';
        }
    }

    data += 'codeid=' + escape(codeId) + '&';

    if (fastui.testInput(true, 'title', /^[\s\S]{1,100}$/) == false) {
        fastui.inputTips('title', lang.link.tips.input['title']);
        return;
    } else {
        data += 'title=' + escape(title) + '&';
    }

    data += 'day=' + day + '&';

    if (fastui.testInput(true, 'password', /^[\w]{6}$/) == false) {
        fastui.inputTips('password', lang.link.tips.input['password']);
        return;
    } else {
        data += 'password=' + escape(password) + '&';
    }

    if (fastui.testInput(false, 'email', /^([\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?\;?)+$/) == false) {
        fastui.inputTips('email', lang.link.tips.input['email']);
        return;
    } else if (email.length > 0) {
        data += 'email=' + escape(email) + '&';
    }

    if (fastui.testInput(false, 'message', /^[\s\S]{1,200}$/) == false) {
        fastui.inputTips('message', lang.link.tips.input['message']);
        return;
    } else if (message.length > 0) {
        data += 'message=' + escape(message) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/drive/link/share', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                fastui.iconTips('tick');

                var tables = $tag('table');

                tables[0].$remove();

                tables[0].style.display = 'block';

                $id('title').innerHTML = title;
                $id('link').innerHTML = window.location.protocol + '//' + window.location.host + '/web/link/?' + window.top.myId + '_' + codeId;
                $id('password').innerHTML = password;

                var qrcode = new QRCode($id('qrcode'), {
                    text: $id('link').innerHTML, 
                    width: 128, 
                    height: 128, 
                    colorDark: '#000000', 
                    colorLight: '#FFFFFF', 
                    correctLevel: QRCode.CorrectLevel.H
                    });
            } else {
                fastui.textTips(lang.link.tips['operation-failed']);
            }
            }
        });
}


// 复制链接分享信息
function linkCopy() {
    var clipboard = new Clipboard('#copy-button', {
        text: function() {
            var text = lang.link.tips['link-copy-text'];

            text = text.replace(/\{title\}/, $id('title').innerHTML);
            text = text.replace(/\{link\}/, $id('link').innerHTML);
            text = text.replace(/\{password\}/, $id('password').innerHTML);

            return text;
            }
        });

    fastui.iconTips('tick');
}