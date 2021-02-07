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
        url: '/api/drive/file/download-package?state=packing', 
        async: true, 
        callback: function(data) {
            if (data == 'complete') {
                var iframe = $create({
                    tag: 'iframe', 
                    attr: {
                        id: 'download-iframe', 
                        name: 'download-iframe', 
                        src: '/api/drive/file/download-package' + window.location.search, 
                        width: '0', 
                        height: '0', 
                        frameBorder: '0', 
                        onload: 'downloadComplete();', 
                        onreadystatechange: 'downloadComplete();'
                        }
                    }).$add(document.body);

                $id('tips').innerHTML = lang.file.tips['download-status-packing'];

                // 下载监听
                interval = window.setInterval(function() {
                    downloadComplete();
                    }, 1000);
            } else {
                fastui.textTips(lang.file.tips['operation-failed']);
            }
            }
        });
}


// 下载完成
function downloadComplete() {
    var iframe = $id('download-iframe');
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
                if ($cookie('file-download-package-state') != 'complete') {
                    return;
                }
            } else {
                window.clearInterval(interval);
            }

            $id('waiting').style.display = 'none';
            $id('complete').style.display = 'block';

            $id('tips').innerHTML = lang.file.tips['download-status-start'];

            window.setTimeout(function() {
                window.parent.fastui.windowClose('file-download-package');
                }, 3000);

            complete = true;
        }
        }, 0);
}