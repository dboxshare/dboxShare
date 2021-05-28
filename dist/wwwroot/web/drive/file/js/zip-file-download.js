var complete = false;
var interval = null;


// 页面初始化事件调用函数
function init() {
    downloadInit();
}


// 下载初始化
function downloadInit() {
    $ajax({
        type: 'GET', 
        url: '/api/drive/file/zip-file-download?state=packing', 
        async: true, 
        callback: function(data) {
            if (data == 'complete') {
                var iframe = $create({
                    tag: 'iframe', 
                    attr: {
                        id: 'download-iframe', 
                        name: 'download-iframe', 
                        src: '/api/drive/file/zip-file-download' + window.location.search, 
                        width: '0', 
                        height: '0', 
                        frameBorder: '0', 
                        onload: 'downloadProgress();', 
                        onreadystatechange: 'downloadProgress();'
                        }
                    }).$add(document.body);

                $id('tips').innerHTML = lang.file.tips['download-status-packing'];

                // 下载监听
                interval = window.setInterval(function() {
                    downloadProgress();
                    }, 1000);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 下载进度
function downloadProgress() {
    var iframe = $id('download-iframe');

    if (iframe == null) {
        return;
    }

    var iDocument = iframe.contentWindow.document || iframe.contentDocument;

    window.setTimeout(function() {
        if (iDocument.readyState != 'interactive' && iDocument.readyState != 'complete') {
            return;
        } else {
            if (iDocument.body == null) {
                return;
            }

            var content = iDocument.body.innerHTML.replace(/\<[^\>]+\>/g, '');

            if (content.length == 0) {
                if ($cookie('zip-file-download-state') != 'complete') {
                    return;
                }
            } else {
                window.clearInterval(interval);
            }

            if (content == 'no-permission') {
                window.parent.fastui.textTips(lang.file.tips['no-permission']);
                window.parent.fastui.windowClose('zip-file-download');
                return;
            }

            if (content == 'download-size-limit') {
                window.parent.fastui.textTips(lang.file.tips['download-size-limit'].replace(/\{size\}/, window.top.myDownloadSize));
                window.parent.fastui.windowClose('zip-file-download');
                return;
            }

            if (content == 'wrong-password') {
                window.parent.unzipKey('download');
                window.parent.fastui.windowClose('zip-file-download');
                return;
            }

            $id('waiting').style.display = 'none';
            $id('complete').style.display = 'block';

            $id('tips').innerHTML = lang.file.tips['download-status-start'];

            window.setTimeout(function() {
                window.parent.fastui.windowClose('zip-file-download');
                }, 3000);

            complete = true;
        }
        }, 0);
}