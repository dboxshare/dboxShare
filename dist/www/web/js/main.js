// 弹出提示(离开页面、刷新页面、关闭浏览器、注销登录)
window.onbeforeunload = function() {
    return '';
};


// 页面初始化事件调用函数
function init() {
    pageResize();

    tabCreate('explorer', lang.main.tab['explorer'], '/web/drive/file/file-explorer.html', true);
    tabCreate('task', lang.main.tab['task'], '/web/drive/task/task-inbox.html', false);
    tabCreate('discuss', lang.main.tab['discuss'], '/web/drive/discuss/discuss-flow.html', false);
    tabCreate('activity', lang.main.tab['activity'], '/web/drive/activity/activity-flow.html', false);
    tabCreate('recycle', lang.main.tab['recycle'], '/web/drive/file/file-recycle.html', false);

    // 管理员
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
        url: '/web/user/logout.ashx', 
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
        html: text + '<img src=\"/ui/images/tab-reload-icon.png\" class=\"reload\" title=\"' + lang.main.tab['refresh'] + '\" onClick=\"tabReload(\'' + id + '\');\" />'
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
    var tabs = $id('tab').$tag('span');
    var iframes = $id('content').$tag('iframe');
    var i;

    // 标签切换
    for (i = 0; i < tabs.length; i++) {
        if (tabs[i].id == 'tab-' + id) {
            tabs[i].className = 'tab-current';

            tabs[i].$tag('img')[0].style.display = 'block';
        } else {
            tabs[i].className = 'tab';

            tabs[i].$tag('img')[0].style.display = 'none';
        }
    }

    // 内容页框架切换
    for (i = 0; i < iframes.length; i++) {
        if (iframes[i].id == 'iframe-' + id) {
            if (iframes[i].src.length == 0) {
                iframes[i].src = $url(iframes[i].$get('source'));

                window.setTimeout(function() {
                    tabLoading(id);
                    }, 0);
            }

            iframes[i].style.display = 'block';
        } else {
            iframes[i].style.display = 'none';
        }
    }
}


// 标签页刷新
function tabReload(id) {
    var iframes = $id('content').$tag('iframe');
    var i;

    for (i = 0; i < iframes.length; i++) {
        if (iframes[i].id == 'iframe-' + id) {
            iframes[i].contentWindow.location.reload(true);
            break;
        }
    }
}


// 标签页加载等待
function tabLoading(id) {
    var content = $id('content');
    var iframe = $id('iframe-' + id);
    var cover = $id('content-loading-cover');

    cover.style.display = 'block';
    cover.style.width = content.offsetWidth + 'px';
    cover.style.height = content.offsetHeight + 'px';

    window.setTimeout(function() {
        cover.style.filter = 'alpha(opacity=75)';
        cover.style.opacity = '.75';
        }, 0);

    if (iframe.attachEvent) {
        iframe.attachEvent('onload', function() {
            cover.style.filter = "alpha(opacity=0)";
            cover.style.opacity = "0";

            window.setTimeout(function() {
                cover.style.display = 'none';
                }, 200);
            });
    } else {
        iframe.onload = function() {
            cover.style.filter = "alpha(opacity=0)";
            cover.style.opacity = "0";

            window.setTimeout(function() {
                cover.style.display = 'none';
                }, 200);
            };
    }

    // 遮蔽层超时处理
    window.setTimeout(function() {
        if (cover.style.display == 'block') {
            cover.style.filter = "alpha(opacity=0)";
            cover.style.opacity = "0";

            window.setTimeout(function() {
                cover.style.display = 'none';
                }, 200);
        }
    }, 10000);
}


// 工具菜单初始化
function toolInit() {
    var tools = $id('tool').$tag('span');
    var i;

    for (i = 0; i < tools.length; i++) {
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
    var content = $id('content');
    var cover = $id('content-cover');

    if (layer == undefined) {
        return;
    }

    if (cover != null) {
        cover.style.filter = "alpha(opacity=0)";
        cover.style.opacity = "0";

        window.setTimeout(function() {
            cover.style.display = 'none';
            }, 200);
    }

    layer.style.display = 'none';
    layer.onclick = '';
}


// 工具菜单弹出层显示
function toolLayerShow(layer) {
    var content = $id('content');
    var cover = $id('content-cover');

    if (layer == undefined) {
        return;
    }

    cover.style.display = 'block';
    cover.style.width = content.offsetWidth + 'px';
    cover.style.height = content.offsetHeight + 'px';

    window.setTimeout(function() {
        cover.style.filter = 'alpha(opacity=75)';
        cover.style.opacity = '.75';
        }, 0);

    layer.style.display = 'block';
    layer.onclick = function() {toolLayerHide(layer); };
}