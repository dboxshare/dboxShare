var complete = false;
var interval = null;


// 页面初始化事件调用函数
function init() {
    unpackInit();
}


// 解压初始化
function unpackInit() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/file/zip-file-unpack?state=unpacking', 
        async: true, 
        callback: function(data) {
            if (data == 'complete') {
                var iframe = $create({
                    tag: 'iframe', 
                    attr: {
                        id: 'unpack-iframe', 
                        name: 'unpack-iframe', 
                        src: '/api/drive/file/zip-file-unpack' + window.location.search, 
                        width: '0', 
                        height: '0', 
                        frameBorder: '0', 
                        onload: 'unpackComplete();', 
                        onreadystatechange: 'unpackComplete();'
                        }
                    }).$add(document.body);

                $id('tips').innerHTML = lang.file.tips['unpack-status-unpacking'];

                // 解压监听
                interval = window.setInterval(function() {
                    unpackComplete();
                    }, 1000);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 解压完成
function unpackComplete() {
    var iframe = $id('unpack-iframe');
    var iDocument = iframe.contentDocument || iframe.contentWindow.document;

    window.setTimeout(function() {
        if (iDocument.readyState != 'interactive' && iDocument.readyState != 'complete') {
            return;
        } else {
            if (iDocument.body == null) {
                return;
            }

            var content = iDocument.body.innerHTML.replace(/\<[^\>]+\>/g, '');

            if (content.length == 0) {
                if ($cookie('zip-file-unpack-state') != 'complete') {
                    return;
                }
            } else {
                window.clearInterval(interval);
            }

            if (content == 'no-permission') {
                window.parent.fastui.textTips(lang.file.tips['no-permission']);
                window.parent.fastui.windowClose('zip-file-unpack');
                return;
            }

            if (content == 'wrong-password') {
                window.parent.unzipKey('unpack');
                window.parent.fastui.windowClose('zip-file-unpack');
                return;
            }

            $id('waiting').style.display = 'none';
            $id('complete').style.display = 'block';

            $id('tips').innerHTML = lang.file.tips['unpack-status-complete'];

            window.setTimeout(function() {
                window.parent.unpackComplete();
                window.parent.fastui.windowClose('zip-file-unpack');
                }, 3000);

            complete = true;
        }
        }, 0);
}