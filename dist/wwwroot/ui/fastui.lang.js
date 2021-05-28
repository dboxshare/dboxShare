/*
 * fastui library for dboxshare v0.2.1.2012
 * @module lang
 */


var lang = {};


lang.json = {
    'zh-CHS' : '中文-简体', 
    'zh-CHT' : '中文-繁體', 
    'en' : 'English'
    };


/*
 * 获取默认语言函数
 */
lang.default = function() {
    // 第一个项目表示默认语言
    for (var i in lang.json) {
        return i;
    }
    };


/*
 * 语言初始化函数
 */
lang.init = function() {
    var language = $cookie('app-language');

    if (language.length == 0) {
        language = lang.default();

        $cookie('app-language', language, 365);
    }

    // 获取语言包最后修改时间
    $ajax({
        type: 'HEAD', 
        url: '/ui/languages/' + language + '.js?' + Math.random(), 
        async: true, 
        callback: function(http) {
            if (http.status == 200) {
                var lastTime = http.getResponseHeader('Last-Modified');
                var version;

                if (lastTime.length == 0) {
                    version = Math.random();
                } else {
                    version = new Date(lastTime).getTime();
                }

                lang.load(version);
            } else {
                // 语言包不存在执行页面初始化
                try {
                    init();
                    } catch(e) {}

                inited = true;

                _load();

                return;
            }
            }
        });
    };


/*
 * 同步加载语言包
 */
lang.load = function(version) {
    var script = $create({
        tag: 'script', 
        attr: {
            language: 'javascript', 
            type: 'text/javascript', 
            src: '/ui/languages/' + $cookie('app-language') + '.js?v=' + version
            }
        }).$add($tag('head')[0]);

    if (script.readyState) {
        script.onreadystatechange = function() {
            if (script.readyState == 'loaded' || script.readyState == 'complete') {
                script.onreadystatechange = null;

                try {
                    lang.show();
                    } catch(e) {}

                try {
                    init();
                    } catch(e) {}

                inited = true;

                _load();
            }
            };
    } else {
        script.onload = function() {
            try {
                lang.show();
                } catch(e) {}

            try {
                init();
                } catch(e) {}

            inited = true;

            _load();
            };
    }
    };


/*
 * 语言显示函数
 */
lang.show = function() {
    var elements = $tag('*');

    for (var i = 0; i < elements.length; i++) {
        var rule = elements[i].$get('lang');

        if (rule != null && rule.length > 0) {
            var params = rule.split(':');
            var keys = params[1].split('.');
            var text;

            switch(keys.length - 1) {
                case 1:
                    text = lang[keys[0]][keys[1]];
                    break;

                case 2:
                    text = lang[keys[0]][keys[1]][keys[2]];
                    break;

                case 3:
                    text = lang[keys[0]][keys[1]][keys[2]][keys[3]];
                    break;

                case 4:
                    text = lang[keys[0]][keys[1]][keys[2]][keys[3]][keys[4]];
                    break;
            }

            switch(params[0]) {
                case 'html':
                    elements[i].innerHTML = text;
                    break;

                case 'value':
                    elements[i].value = text;
                    break;

                case 'title':
                    elements[i].title = text;
                    break;

                case 'placeholder':
                    elements[i].placeholder = text;
                    break;

                default:
                    elements[i].$set(params[0], text);
            }
        }
    }
    };