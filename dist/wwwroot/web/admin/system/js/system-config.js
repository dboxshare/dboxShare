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
        url: '/api/admin/system/config-data-json', 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.admin.config.tips['config-read-failed']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('appname').value = jsonData.appname;
                $id('uploadextension').value = jsonData.uploadextension.replace(/[\,]+/g, "\r");
                $id('uploadsize').value = jsonData.uploadsize;
                $id('downloadsize').value = jsonData.downloadsize;
                $id('versioncount').value = jsonData.versioncount;
                $id('mailaddress').value = jsonData.mailaddress;
                $id('mailusername').value = jsonData.mailusername;
                $id('mailpassword').value = jsonData.mailpassword;
                $id('mailserver').value = jsonData.mailserver;
                $id('mailserverport').value = jsonData.mailserverport;
                $id('mailserverssl').checked = jsonData.mailserverssl;
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
    var downloadSize = $id('downloadsize').value;
    var versionCount = $id('versioncount').value;
    var mailAddress = $id('mailaddress').value;
    var mailUsername = $id('mailusername').value;
    var mailPassword = $id('mailpassword').value;
    var mailServer = $id('mailserver').value;
    var mailServerPort = $id('mailserverport').value;
    var mailServerSSL = $id('mailserverssl').checked;
    var data = '';

    if (fastui.testInput(true, 'appname', /^[\w]{1,50}$/) == false) {
        fastui.inputTips('appname', lang.admin.config.tips.input['app-name']);
        return;
    } else {
        data += 'appname=' + escape(appName) + '&';
    }

    if (fastui.testInput(false, 'uploadextension', /^([\w]{1,8}[\r\n]?){1,200}$/) == false) {
        fastui.inputTips('uploadextension', lang.admin.config.tips.input['upload-extension']);
        return;
    } else if (uploadExtension.length > 0) {
        data += 'uploadextension=' + escape(uploadExtension.replace(/[\n]+/g, ',')) + '&';
    }

    if (fastui.testInput(true, 'uploadsize', /^[\d]{1,5}$/) == false) {
        fastui.inputTips('uploadsize', lang.admin.config.tips.input['upload-size']);
        return;
    } else {
        if (uploadSize < 1 || uploadSize > 10240) {
            fastui.inputTips('uploadsize', lang.admin.config.tips.input['upload-size-range']);
            return;
        }

        data += 'uploadsize=' + uploadSize + '&';
    }

    if (fastui.testInput(true, 'downloadsize', /^[\d]{1,5}$/) == false) {
        fastui.inputTips('downloadsize', lang.admin.config.tips.input['download-size']);
        return;
    } else {
        if (downloadSize < 1 || downloadSize > 10240) {
            fastui.inputTips('downloadsize', lang.admin.config.tips.input['download-size-range']);
            return;
        }

        data += 'downloadsize=' + downloadSize + '&';
    }

    if (fastui.testInput(true, 'versioncount', /^[\d]{2,3}$/) == false) {
        fastui.inputTips('versioncount', lang.admin.config.tips.input['version-count']);
        return;
    } else {
        if (versionCount < 50 || versionCount > 500) {
            fastui.inputTips('versioncount', lang.admin.config.tips.input['version-count-range']);
            return;
        }

        data += 'versioncount=' + versionCount + '&';
    }

    if (fastui.testInput(true, 'mailaddress', /^[\w\-\@\.]{1,50}$/) == false) {
        fastui.inputTips('mailaddress', lang.admin.config.tips.input['mail-address']);
        return;
    } else {
        data += 'mailaddress=' + escape(mailAddress) + '&';
    }

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

    if (fastui.testInput(true, 'mailserver', /^[\w\-\.\:]{1,50}$/) == false) {
        fastui.inputTips('mailserver', lang.admin.config.tips.input['mail-server']);
        return;
    } else {
        data += 'mailserver=' + escape(mailServer) + '&';
    }

    if (fastui.testInput(true, 'mailserverport', /^[\d]{1,5}$/) == false) {
        fastui.inputTips('mailserverport', lang.admin.config.tips.input['mail-server-port']);
        return;
    } else {
        data += 'mailserverport=' + mailServerPort + '&';
    }

    data += 'mailserverssl=' + mailServerSSL + '&';

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/admin/system/config', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                fastui.iconTips('tick');

                window.setTimeout(function() {
                    fastui.textTips(lang.admin.config.tips['re-login'], 3000, 8);

                    window.setTimeout(function() {
                        fastui.windowPopup('login-box', '', '/web/account/login.html', 350, 350);
                        }, 3000);
                    }, 500);
            } else {
                fastui.textTips(lang.admin.config.tips['operation-failed']);
            }
            }
        });
}