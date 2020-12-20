var complete;


// 页面初始化事件调用函数
function init() {
    unpackInit();
}


// 解压初始化
function unpackInit() {
    var iframe = $create({
        tag: 'iframe', 
        attr: {
            id: 'download-iframe', 
            name: 'download-iframe', 
            src: '/web/drive/file/zip-file-unpack.ashx' + window.location.search, 
            width: '0', 
            height: '0', 
            frameBorder: '0', 
            onload: 'unpackComplete();', 
            onreadystatechange: 'unpackComplete();'
            }
        }).$add(document.body);

    $id('tips').innerHTML = lang.file.tips['unpack-status-unpacking'];

    complete = false;

    unpackProgress();
}


// 解压完成
function unpackComplete() {
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
}


// 解压进度
function unpackProgress() {
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