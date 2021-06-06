// 弹出提示(离开页面、刷新页面、关闭浏览器、注销登录)
window.onbeforeunload = function() {
    return '';
};


// 页面初始化事件调用函数
function init() {
    pageResize();

    tabCreate('file', lang.main.tab['file'], '/web/drive/file/file-explorer.html', true);
    tabCreate('activity', lang.main.tab['activity'], '/web/drive/activity/activity-flow.html', false);

    // 系统管理
    if (window.top.myAdmin == 1) {
        tabCreate('admin', lang.main.tab['admin'], '/web/admin/statistic/statistic-data.html', false);
    }

    toolInit();

    // 初次登录弹出使用帮助窗口
    if ($cookie('help').length == 0) {
        fastui.windowPopup('help', lang.main.tool['help'], '/web/tool/help.html', 800, 500);

        $cookie('help', true, 365);
    }
}


// 页面退出事件调用函数
function unload() {
    $ajax({
        type: 'GET', 
        url: '/api/account/user-logout', 
        async: true
        });
}


// 页面调整
function pageResize() {
    var tab = $id('tab');
    var content = $id('content');

    content.style.width = content.offsetWidth + 'px';
    content.style.height = (content.offsetHeight - tab.offsetHeight) + 'px';
}


// 标签页创建
function tabCreate(id, text, src, show) {
    if ($id('tab-' + id) != null) {
        tabShow(id);
        return;
    }

    // 生成标签
    var tab = $create({
        tag: 'span',
        attr: {id: 'tab-' + id},
        css: 'tab', 
        html: text + '<img src="/ui/images/tab-reload-icon.png" class="reload" title="' + lang.main.tab['refresh'] + '" onClick="tabReload(\'' + id + '\');" />'
    }).$add($id('tab'));

    tab.onclick = function() {
        tabShow(id);
    };

    // 生成内容页框架
    var iframe = $create({
        tag: 'iframe',
        attr: {
            id: 'iframe-' + id,
            source: src,
            width: '100%',
            height: '100%',
            frameBorder: '0',
            scrolling: 'yes'
        }, 
        css: 'iframe'
    }).$add($id('content'));

    if (show == true) {
        tabShow(id);
    }
}


// 标签页显示
function tabShow(id) {
    // 标签切换
    var tabs = $id('tab').$tag('span');

    for (var i = 0; i < tabs.length; i++) {
        if (tabs[i].id == 'tab-' + id) {
            tabs[i].className = 'tab-current';

            tabs[i].$tag('img')[0].style.display = 'block';
        } else {
            tabs[i].className = 'tab';

            tabs[i].$tag('img')[0].style.display = 'none';
        }
    }

    // 内容页框架切换
    var iframes = $id('content').$tag('iframe');

    for (var j = 0; j < iframes.length; j++) {
        if (iframes[j].id == 'iframe-' + id) {
            if (iframes[j].src.length == 0) {
                iframes[j].src = $url(iframes[j].$get('source'));

                window.setTimeout(function() {
                    tabLoading(id);
                    }, 0);
            }

            iframes[j].style.display = 'block';
        } else {
            iframes[j].style.display = 'none';
        }
    }
}


// 标签页刷新
function tabReload(id) {
    var iframes = $id('content').$tag('iframe');

    for (var i = 0; i < iframes.length; i++) {
        if (iframes[i].id == 'iframe-' + id) {
            iframes[i].contentWindow.location.reload(true);
            break;
        }
    }
}


// 标签页加载等待
function tabLoading(id) {
    var cover = $id('content-loading-cover');
    var content = $id('content');
    var iframe = $id('iframe-' + id);

    cover.style.display = 'block';
    cover.style.width = content.offsetWidth + 'px';
    cover.style.height = content.offsetHeight + 'px';

    if (iframe.attachEvent) {
        iframe.attachEvent('onload', function() {
            cover.style.display = 'none';
            });
    } else {
        iframe.onload = function() {
            cover.style.display = 'none';
            };
    }

    // 遮蔽层超时处理
    window.setTimeout(function() {
        if (cover.style.display == 'block') {
            cover.style.display = 'none';
        }
    }, 10000);
}


// 工具菜单初始化
function toolInit() {
    var tools = $id('tool').$tag('span');

    for (var i = 0; i < tools.length; i++) {
        if (tools[i].$tag('ul')[0] != null) {
            tools[i].onmouseenter = function() {
                toolLayerShow(this.$tag('ul')[0]);
                };

            tools[i].onmouseleave = function() {
                toolLayerHide(this.$tag('ul')[0]);
                };
        }
    }
}


// 工具菜单弹出层隐藏
function toolLayerHide(layer) {
    if (layer == null) {
        return;
    }

    var cover = $id('content-cover');
    var content = $id('content');

    if (cover != null) {
        cover.style.display = 'none';
    }

    layer.style.display = 'none';
    layer.onclick = '';
}


// 工具菜单弹出层显示
function toolLayerShow(layer) {
    if (layer == null) {
        return;
    }

    var cover = $id('content-cover');
    var content = $id('content');

    cover.style.display = 'block';
    cover.style.width = content.offsetWidth + 'px';
    cover.style.height = content.offsetHeight + 'px';

    layer.style.display = 'block';
    layer.onclick = function() {toolLayerHide(layer); };
}