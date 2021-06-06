// 回车键提交登录表单
document.onkeydown = function(e) {
    var event = e || window.event;

    if (event.keyCode == 13) {
        var cover = $class('page-cover')[0];

        if (cover == undefined || cover.style.display == 'none') {
            shareExtract();
            return;
        }
    }
};


// 页面初始化事件调用函数
function init() {
    dataInit();
    dataReader();
}


// 数据初始化
function dataInit() {
    fastui.valueTips('password', lang.link.tips.value['password']);
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


// 链接分享提取表单提交
function shareExtract() {
    var match = window.location.search.match(/(\d+)\_([\w]{16})/);
    var password = $id('password').value;
    var data = '';

    if (match == null) {
        fastui.coverTips(lang.link.tips['illegal-operation']);
        return;
    }

    data += 'userid=' + escape(match[1]) + '&';
    data += 'codeid=' + escape(match[2]) + '&';

    if (fastui.testInput(true, 'password', /^[\w]{6}$/) == false) {
        fastui.inputTips('password', lang.link.tips.input['password']);
        return;
    } else {
        data += 'password=' + escape(password) + '&';
    }

    data = data.substring(0, data.length - 1);

    fastui.coverShow('waiting-cover');

    $ajax({
        type: 'POST', 
        url: '/api/link/extract-login', 
        async: true, 
        data: data, 
        callback: function(data) {
            fastui.coverHide('waiting-cover');

            if (data == 'complete') {
                $location('/web/link/file-list.html?linkid=' + match[1] + '_' + match[2]);
            } else if (data == 'link-login-failed') {
                fastui.textTips(lang.link.tips['link-login-failed']);
            } else if (data == 'link-lock-ip') {
                fastui.textTips(lang.link.tips['link-lock-ip']);
            } else {
                fastui.textTips(lang.link.tips['operation-failed']);
            }
            }
        });
}