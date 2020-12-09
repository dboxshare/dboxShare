var interval;
var complete;


// 页面初始化事件调用函数
function init() {
    downloadInit();
}


// 页面载入事件调用函数
function load() {
    downloadMonitor();
}


// 下载初始化
function downloadInit() {
    $id('download-iframe').src = '/web/drive/file/file-download-package.ashx' + window.location.search;

    $id('tips').innerHTML = lang.file.tips['download-status-packing'];

    complete = false;

    downloadProgress();
}


// 下载状态监听
function downloadMonitor() {
    interval = window.setInterval(function() {
        downloadEvent();
        }, 100);
}


// 下载事件
function downloadEvent() {
    var iframe = $id('download-iframe');
    var document = iframe.contentDocument || iframe.contentWindow.document;

    window.setTimeout(function() {
        if (document.readyState !== 'complete' && document.readyState != 'loaded') {
            return;
        }
        }, 0);

    if (document.body == null) {
        return;
    }

    if (document.body.innerHTML == 'wrong-password') {
        window.parent.unzipKey();
        window.parent.fastui.windowClose('zip-file-download');
        return;
    }

    $id('tips').innerHTML = lang.file.tips['download-status-start'];

    $id('progress').$class('bar')[0].style.width = '100%';

    window.setTimeout(function() {
        window.parent.fastui.windowClose('file-download-package');
        }, 3000);

    complete = true;

    window.clearInterval(interval);
}


// 下载进度
function downloadProgress() {
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