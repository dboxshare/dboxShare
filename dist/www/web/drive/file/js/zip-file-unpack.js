var interval;
var complete;


// 页面初始化事件调用函数
function init() {
    unpackInit();
}


// 页面载入事件调用函数
function load() {
    unpackMonitor();
}


// 解压初始化
function unpackInit() {
    $id('unpack-iframe').src = '/web/drive/file/zip-file-unpack.ashx' + window.location.search;

    $id('tips').innerHTML = lang.file.tips['unpack-status-unpacking'];

    complete = false;

    unpackProgress();
}


// 解压状态监听
function unpackMonitor() {
    interval = window.setInterval(function() {
        unpackEvent();
        }, 100);
}


// 解压事件
function unpackEvent() {
    var iframe = $id('unpack-iframe');
    var document = iframe.contentDocument || iframe.contentWindow.document;

    window.setTimeout(function() {
        if (document.readyState !== 'complete' && document.readyState != 'loaded') {
            return;
        }
        }, 0);

    if (document.body == null) {
        return;
    }

    if (document.body.innerHTML == 'no-permission') {
        window.parent.fastui.textTips(lang.file.tips['no-permission']);
        window.parent.fastui.windowClose('zip-file-unpack');
        return;
    }

    if (document.body.innerHTML == 'wrong-password') {
        window.parent.unzipKey('unpack');
        window.parent.fastui.windowClose('zip-file-unpack');
        return;
    }

    $id('tips').innerHTML = lang.file.tips['unpack-status-complete'];

    $id('progress').$class('bar')[0].style.width = '100%';

    window.setTimeout(function() {
        window.parent.unpackComplete();
        window.parent.fastui.windowClose('zip-file-unpack');
        }, 3000);

    complete = true;

    window.clearInterval(interval);
}


// 解压进度
function unpackProgress() {
    var progress = $id('progress');
    var bar = progress.$class('bar')[0];
    var i;

    for (i = 0; i < 100; i++) {
        (function(index) {
            window.setTimeout(function() {
                if (complete == true) {
                    return;
                }

                bar.style.width = index + '%';
                }, index * 1000);
            })(i);
    }
}