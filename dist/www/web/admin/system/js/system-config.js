// 页面初始化事件调用函数
function init() {
    dataReader();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('uploadextension', lang.admin.config.tips.value['upload-extension']);
}


// 数据读取
function dataReader() {
    $ajax({
        type: 'GET', 
        url: '/web/admin/system/system-config-data-json.ashx', 
        async: true, 
        callback: function(data) {
            if (data.length == 0 || data == '{}') {
                fastui.coverTips(lang.admin.config.tips['config-read-failed']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('appname').value = jsonData.appname;
                $id('uploadextension').value = jsonData.uploadextension.replace(/[\,]+/g, "\r");
                $id('uploadsize').value = jsonData.uploadsize;
                $id('versioncount').value = jsonData.versioncount;
                $id('mailaddress').value = jsonData.mailaddress;
                $id('mailserver').value = jsonData.mailserver;
                $id('mailport').value = jsonData.mailport;
                $id('mailssl').checked = jsonData.mailssl == 'true' ? true : false;
                $id('mailusername').value = jsonData.mailusername;
                $id('mailpassword').value = jsonData.mailpassword;
            }

            dataInit();

            fastui.coverHide('loading-cover');
            }
        });

    fastui.coverShow('loading-cover');
}


// 系统配置表单提交
function systemConfig() {
    var appName = $id('appname').value;
    var uploadExtension = $id('uploadextension').value;
    var uploadSize = $id('uploadsize').value;
    var versionCount = $id('versioncount').value;
    var mailAddress = $id('mailaddress').value;
    var mailServer = $id('mailserver').value;
    var mailPort = $id('mailport').value;
    var mailSsl = $id('mailssl').checked;
    var mailUsername = $id('mailusername').value;
    var mailPassword = $id('mailpassword').value;
    var data = '';

    if (fastui.testInput(true, 'appname', /^[\w]{1,50}$/) == false) {
        fastui.inputTips('appname', lang.admin.config.tips.input['app-name']);
        return;
    } else {
        data += 'appname=' + escape(appName) + '&';
    }

    if (fastui.testInput(false, 'uploadextension', /^([\w]{1,8}[\r\n]?){1,500}$/) == false) {
        fastui.inputTips('uploadextension', lang.admin.config.tips.input['upload-extension']);
        return;
    } else if (uploadExtension.length > 0) {
        data += 'uploadextension=' + escape(uploadExtension.replace(/[\n]+/g, ',')) + '&';
    }

    if (fastui.testInput(true, 'uploadsize', /^[\d]{2,4}$/) == false) {
        fastui.inputTips('uploadsize', lang.admin.config.tips.input['upload-size']);
        return;
    } else {
        data += 'uploadsize=' + uploadSize + '&';
    }

    if (uploadSize < 1 || uploadSize > 2048) {
        fastui.inputTips('uploadsize', lang.admin.config.tips.input['upload-size-range']);
        return;
    }

    if (fastui.testInput(true, 'versioncount', /^[\d]{2,4}$/) == false) {
        fastui.inputTips('versioncount', lang.admin.config.tips.input['version-count']);
        return;
    } else {
        data += 'versioncount=' + versionCount + '&';
    }

    if (versionCount < 10 || versionCount > 1024) {
        fastui.inputTips('versioncount', lang.admin.config.tips.input['version-count-range']);
        return;
    }

    if (fastui.testInput(true, 'mailaddress', /^[\w\-\@\.]{1,50}$/) == false) {
        fastui.inputTips('mailaddress', lang.admin.config.tips.input['mail-address']);
        return;
    } else {
        data += 'mailaddress=' + escape(mailAddress) + '&';
    }

    if (fastui.testInput(true, 'mailserver', /^[\w\-\.\:]{1,50}$/) == false) {
        fastui.inputTips('mailserver', lang.admin.config.tips.input['mail-server']);
        return;
    } else {
        data += 'mailserver=' + escape(mailServer) + '&';
    }

    if (fastui.testInput(true, 'mailport', /^[\d]+$/) == false) {
        fastui.inputTips('mailport', lang.admin.config.tips.input['mail-port']);
        return;
    } else {
        data += 'mailport=' + mailPort + '&';
    }

    data += 'mailssl=' + mailSsl + '&';

    if (fastui.testInput(true, 'mailusername', /^[\w\-\@\.]{1,50}$/) == false) {
        fastui.inputTips('mailusername', lang.admin.config.tips.input['mail-username']);
        return;
    } else {
        data += 'mailusername=' + escape(mailUsername) + '&';
    }

    if (fastui.testInput(true, 'mailpassword', /^[\S]{1,50}$/) == false) {
        fastui.inputTips('mailpassword', lang.admin.config.tips.input['mail-password']);
        return;
    } else {
        data += 'mailpassword=' + escape(mailPassword) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/web/admin/system/system-config.ashx', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                fastui.iconTips('tick');

                window.setTimeout(function() {
                    fastui.textTips(lang.admin.config.tips['re-login']);

                    window.setTimeout(function() {
                        fastui.windowPopup('re-login', '', '/web/user/login.html', 350, 350);
                        }, 3000);
                    }, 3000);
            } else {
                fastui.textTips(lang.admin.config.tips['operation-failed']);
            }
            }
        });
}