var complete;


// 页面初始化事件调用函数
function init() {
    downloadInit();
}


// 下载初始化
function downloadInit() {
    var iframe = $create({
        tag: 'iframe', 
        attr: {
            id: 'download-iframe', 
            name: 'download-iframe', 
            src: '/web/drive/file/zip-file-download.ashx' + window.location.search, 
            width: '0', 
            height: '0', 
            frameBorder: '0', 
            onload: 'downloadComplete();', 
            onreadystatechange: 'downloadComplete();'
            }
        }).$add(document.body);

    $id('tips').innerHTML = lang.file.tips['download-status-packing'];

    complete = false;

    downloadProgress();
}


// 下载完成
function downloadComplete() {
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

    if (document.body.innerHTML == 'no-permission') {
        window.parent.fastui.textTips(lang.file.tips['no-permission']);
        window.parent.fastui.windowClose('zip-file-download');
        return;
    }

    if (document.body.innerHTML == 'wrong-password') {
        window.parent.unzipKey('download');
        window.parent.fastui.windowClose('zip-file-download');
        return;
    }

    $id('tips').innerHTML = lang.file.tips['download-status-start'];

    $id('progress').$class('bar')[0].style.width = '100%';

    window.setTimeout(function() {
        window.parent.fastui.windowClose('zip-file-download');
        }, 3000);

    complete = true;
}


// 下载进度
function downloadProgress() {
    var progress = $id('progress');
    var bar = progress.$class('bar')[0];
    var i;

    for (i = 1; i < 100; i++) {
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