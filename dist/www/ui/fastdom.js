/*
 * fastdom js library v0.2.0.2006
 * powered by dboxshare project team <http://www.dboxshare.com/>
 *
 * released under the MIT license
 */


var fastdom = {};


fastdom.about = {
    library: 'fastdom.js', 
    version: 'v0.2.0.2006', 
    website: 'http://www.dboxshare.com/', 
    license: 'MIT'
    };


/*
 * DOM
 */


/*
 * 根据id获取元素
 * @param {String} id 元素id值(必需)
 * @return {Element} 返回元素对象
 */
function $id(id) {
    var element = document.getElementById(id);

    if (element == null) {
        return null;
    }

    $chain(element);

    return element;
}


/*
 * 根据name获取元素集合
 * @param {String} name: 元素name值(必需)
 * @return {NodeList} 返回元素对象集合
 */
function $name(name) {
    var elements = document.getElementsByName(name);
    var tags = new Array();
    var i;

    for (i = 0; i < elements.length; i++) {
        (function(index) {
            $chain(elements[index]);
            })(i);
    }

    // 兼用老式浏览器
    if (elements.length == 0) {
        tags = document.getElementsByTagName('*');

        elements = new Array();

        for (i = 0; i < tags.length; i++) {
            if (tags[i].getAttribute('name') == name) {
                elements[elements.length] = tags[i];

                (function(index) {
                    $chain(elements[index]);
                    })(elements.length - 1);
            }
        }
    }

    return elements;
}


/*
 * 根据标签名称获取元素集合
 * @param {Element} element 继承元素对象(可选)
 * @param {String} tag 元素标签名称(必需)
 * @return {NodeList} 返回元素对象集合
 */
function $tag(element, tag) {
    var elements = new Array();
    var i;

    if (typeof(element) == 'string') {
        elements = document.getElementsByTagName(element);
    } else if (typeof(element) == 'object') {
        elements = element.getElementsByTagName(tag);
    } else {
        return null;
    }

    for (i = 0; i < elements.length; i++) {
        (function(index) {
            $chain(elements[index]);
            })(i);
    }

    return elements;
}


/*
 * 根据class获取元素集合
 * @param {Element} element 继承元素对象(可选)
 * @param {String} name 元素class值(必需)
 * @return {NodeList} 返回元素对象集合
 */
function $class(element, name) {
    var elements = new Array();
    var tags = new Array();
    var classes = new Array();
    var i;
    var j;

    try {
        if (typeof(element) == 'string') {
            tags = document.getElementsByClassName(element);
        } else if (typeof(element) == 'object') {
            tags = element.getElementsByClassName(name);
        } else {
            return null;
        }

        for (i = 0; i < tags.length; i++) {
            elements[elements.length] = tags[i];

            (function(index) {
                $chain(elements[index]);
                })(elements.length - 1);
        }
    } catch(e) {
        // 兼用老式浏览器
        if (typeof(element) == 'string') {
            tags = document.getElementsByTagName('*');
            name = element;
        } else if (typeof(element) == 'object') {
            tags = element.getElementsByTagName('*');
        } else {
            return null;
        }

        for (i = 0; i < tags.length; i++) {
            classes = tags[i].className.split(' ');

            for (j = 0; j < classes.length; j++) {
                if (classes[j] == name) {
                    elements[elements.length] = tags[i];

                    (function(index) {
                        $chain(elements[index]);
                        })(elements.length - 1);
                }
            }
        }
    }

    return elements;
}


/*
 * 根据属性名称及值获取元素集合
 * @param {Element} element 继承元素对象(可选)
 * @param {String} name 元素属性名称(必需)
 * @param {String} value 元素属性值(必需)
 * @return {NodeList} 返回元素对象集合
 */
function $attr(element, name, value) {
    var elements = new Array();
    var tags = new Array();
    var i;

    if (typeof(element) == 'string') {
        tags = document.getElementsByTagName('*');
        value = name;
        name = element;
    } else if (typeof(element) == 'object') {
        tags = element.getElementsByTagName('*');
    } else {
        return null;
    }

    for (i = 0; i < tags.length; i++) {
        if (tags[i].getAttribute(name) != null) {
            if (value != undefined) {
                if (tags[i].getAttribute(name) != value) {
                    continue;
                }
            }

            elements[elements.length] = tags[i];

            (function(index) {
                $chain(elements[index]);
                })(elements.length - 1);
        }
    }

    return elements;
}


/*
 * 创建元素
 * @param {Object} json json格式数据(必需)
 *                 json[tag] 创建元素标签名称(必需)
 *                 json[attr] 元素属性json格式数据(可选)
 *                 json[style] 元素样式json格式数据(可选)
 *                 json[css] 元素绑定css类(可选)
 *                 json[html] 元素html内容(可选)
 * @return {Element} 返回元素对象
 * @example $create({tag: 'div', attr: {id: 'demo'}, style: {color: '#FFFFFF'}, css: 'demo', html: 'content'});
 */
function $create(json) {
    var tag = json.tag || '';
    var attr = json.attr || {};
    var style = json.style || {};
    var css = json.css || '';
    var html = json.html || '';
    var element;
    var i;

    if (tag.length == 0) {
        return;
    }

    element = document.createElement(tag)

    for (i in attr) {
        element.setAttribute(i, attr[i]);
    }

    for (i in style) {
        element.style[i] = style[i];
    }

    if (css.length > 0) {
        element.className = css;
    }

    if (html.length > 0) {
        element.innerHTML = html;
    }

    element.$add = function(container) {
        container.appendChild(this);

        return this;
        };

    $chain(element);

    return element;
}


/*
 * 获取元素属性值
 * @param {Object} element 元素id值或对象(必需)
 * @param {String} name 元素属性名称(必需)
 * @return {String} 返回元素属性值
 */
function $get(element, name) {
    if (typeof(element) == 'string') {
        element = $id(element);
    }

    if (element == null) {
        return undefined;
    }

    return element.getAttribute(name);
}


/*
 * 设置元素属性
 * @param {Object} element 元素id值或对象(必需)
 * @param {String} name 元素属性名称(必需)
 * @param {String} value 元素属性值(必需)
 * @return {Element} 返回元素对象
 */
function $set(element, name, value) {
    if (typeof(element) == 'string') {
        element = $id(element);
    }

    if (element == null) {
        return undefined;
    }

    element.setAttribute(name, value);

    return element;
}


/*
 * 绑定元素事件
 * @param {Object} element 元素id值或对象(必需)
 * @param {String} type 事件类型(必需)
 * @param {Object} callback 回调函数(必需)
 */
function $on(element, type, callback) {
    if (typeof(element) == 'string') {
        element = $id(element);
    }

    if (element == null) {
        return undefined;
    }

    if (element.addEventListener) {
        element.addEventListener(type, callback, false);
    } else if (element.attachEvent) {
        element.attachEvent('on' + type, callback);
    } else {
        element['on' + type] = callback;
    }
}


/*
 * 获取或设置元素样式
 * @param {Object} element 元素id值或对象(必需)
 * @param {Object} object 获取=样式名称 / 设置=样式json格式数据(必需)
 * @return {String} 返回获取元素样式值
 */
function $style(element, object) {
    if (typeof(element) == 'string') {
        element = $id(element);
    }

    if (element == null) {
        return undefined;
    }

    // 获取
    if (typeof(object) == 'string') {
        if (element.currentStyle) {
            return element.currentStyle[object];
        } else {
            return document.defaultView.getComputedStyle(element, null)[object];
        }
    }
    
    // 设置
    if (typeof(object) == 'object') {
        var i;

        for (i in object) {
            element.style[i] = object[i];
        }
    }
}


/*
 * 隐藏元素
 * @param {Object} element 元素id值或对象(必需)
 */
function $hide(element) {
    if (typeof(element) == 'string') {
        element = $id(element);
    }

    if (element == null) {
        return undefined;
    }

    element.style.display = 'none';
}


/*
 * 显示元素
 * @param {Object} element 元素id值或对象(必需)
 */
function $show(element) {
    if (typeof(element) == 'string') {
        element = $id(element);
    }

    if (element == null) {
        return undefined;
    }

    element.style.display = 'block';
}


/*
 * 移除元素
 * @param {Object} element 元素id值或对象(必需)
 */
function $remove(element) {
    if (typeof(element) == 'string') {
        element = $id(element);
    }

    if (element == null) {
        return undefined;
    }

    if (element.parentNode == null) {
        return undefined;
    }

    // 移除iframe元素
    try {
        if (element.tagName.toLowerCase() == 'iframe') {
            element.src = 'about:blank';
            element.contentWindow.document.write('');
            element.contentWindow.document.clear();
            element.contentWindow.close();
        } else {
            var iframes = element.$tag('iframe');
            var i;

            for (i = 0; i < iframes.length; i++) {
                iframes[i].$remove();
            }
        }
        } catch(e) {}

    element.parentNode.removeChild(element);
}


/*
 * 清理元素
 * @param {Object} tag 元素标签名称(必需)
 */
function $clear(tag) {
    var elements = $tag(tag);
    var i;

    for (i = 0; i < elements.length; i++) {
        elements[i].$remove();
    }
}


/*
 * 包含元素验证
 * @param {Element} source 来源元素(必需)
 * @param {Element} node 节点元素(必需)
 * @return {Boolean} 返回是否包含
 */
function $contains(source, node) {
    while (node != null && typeof(node.tagName) != 'undefined') {
        node = node.parentNode;

        if (node == source) {
            return true;
        }
    }

    return false;
}


/*
 * 绑定元素链
 * @param {Object} element 元素对象(必需)
 * @return {Element} 返回元素对象
 */
function $chain(element) {
    element.$tag = function(tag) {
        return $tag(this, tag);
        };

    element.$class = function(name) {
        return $class(this, name);
        };

    element.$attr = function(name, value) {
        return $attr(this, name, value);
        };

    element.$get = function(name) {
        return $get(this, name);
        };

    element.$set = function(name, value) {
        $set(this, name, value);
        };

    element.$on = function(type, callback) {
        $on(this, type, callback);
        };

    element.$css = function(json) {
        $style(this, json);
        };

    element.$style = function(name) {
        return $style(this, name);
        };

    element.$hide = function() {
        $hide(this);
        };

    element.$show = function() {
        $show(this);
        };

    element.$remove = function() {
        $remove(this);
        };

    return element;
}


/*
 * Method
 */


/*
 * 获取当前网址参数值
 * @param {String} name 参数名称(必需)
 * @return {String} 返回参数值
 */
function $query(name) {
    var reg = new RegExp('(?:^|&)' + name + '=([^&]*?)(?:&|$)', 'i');
    var param = window.location.search.substr(1).match(reg);

    if (param != null) {
        if (/\%u[\w]+/.test(param[1]) == true) {
            return unescape(param[1]);
        } else {
            return decodeURIComponent(param[1]);
        }
    } else {
        return '';
    }
}


/*
 * 获取或设置cookie
 * @param {String} name 获取或设置cookie名称(必需)
 * @param {String} value cookie值(设置必需)
 * @param {Number} day cookie保存天数(设置必需)
 * @return {String} 返回获取cookie值
 */
function $cookie(name, value, day) {
    // 获取
    if (typeof(name) == 'string' && value == undefined) {
        var reg = new RegExp('(?:^| )' + name + '=([^;]*?)(?:;|$)', 'i');
        var cookies = document.cookie.match(reg);

        if (cookies == null) {
            return '';
        } else {
            if (/\%u[\w]+/.test(cookies[1]) == true) {
                return unescape(cookies[1]);
            } else {
                return decodeURIComponent(cookies[1]);
            }
        }
    }

    // 设置
    if (typeof(name) == 'string' && value != undefined && day != undefined) {
        if (value.length == 0 && day == 0) {
            document.cookie = name + '=;SameSite=Lax;';
        } else if (value.length > 0 && day == 0) {
            document.cookie = name + '=' + value + ';SameSite=Lax;';
        } else {
            var expires = new Date();

            expires.setTime(expires.getTime() + (1000 * 60 * 60 * 24 * day));

            document.cookie = name + '=' + value + ';expires=' + expires.toGMTString() + ';SameSite=Lax;';
        }
    }
}


/*
 * 正则表达式匹配字符串
 * @param {String} data 匹配字符串数据(必需)
 * @param {String} pattern 正则表达式(必需)
 * @return {String} 返回匹配结果
 */
function $match(data, pattern) {
    var match = data.match(pattern);

    if (match != null) {
        return match[1];
    } else {
        return '';
    }
}


/*
 * 时间戳动态网址
 * @param {String} url 原始网址(必需)
 * @return {String} 返回时间戳网址
 */
function $url(url) {
    var time = new Date();
    var path = '';
    var params = '';

    if (url.indexOf('?') == -1) {
        return url + '?' + time.getTime();
    } else {
        if (/\?\d+/.test(url) == true) {
            return url.replace(/\?([\d]+)/, '?' + time.getTime());
        } else {
            path = url.substring(0, url.indexOf('?'));

            params = url.substring(url.indexOf('?') + 1);

            return path + '?' + time.getTime() + '&' + params;
        }
    }
}


/*
 * 路径跳转或当前网址参数替换
 * @param {String} path 跳转路径或替换参数名称(必需)
 * @param {String} value 替换参数值(可选)
 */
function $location(param, value) {
    if (typeof(param) == 'string' && value == undefined) {
        // location兼用跳转(解决session丢失)
        var path = param;

        if ($agent(/MSIE [6-8]\.0/) == true) {
            // 兼用老式浏览器
            var link = $id('a-link');

            if (link == null) {
                link = $create({
                    tag: 'a', 
                    attr: {id: 'a-link'}, 
                    style: 'none'
                    }).$add(document.body);
            }

            link.href = $url(path);
            link.click();
        } else {
            try {
                window.location.href = $url(path);
                } catch(e) {}
        }
    } else {
        // 网址参数替换
        var name = param;
        var search = window.location.search;
        var field = $query(name);

        if (value.length == 0) {
            search = search.replace(window.eval('/&?' + name + '=[^&]*/i'), '');
        } else {
            if (window.eval('/&?' + name + '=/i').test(search) == false) {
                search = (search.length == 0 ? '?' : search + '&') + '' + name + '=' + value;
            } else {
                search = search.replace(window.eval('/' + name + '=[^&]*/i'), '' + name + '=' + value);
            }
        }

        $location(search);
    }
}


/*
 * json字符串转换json对象
 * @param {String} name json对象名称(必需)
 * @param {String} data json格式数据(必需)
 */
function $json(name, data) {
    if (window.execScript) {
        window.execScript('var ' + name + ' = ' + data + ';');
    } else {
        window.eval('var ' + name + ' = ' + data + ';');
    }
}


/*
 * 插入脚本文件(页面包含)
 * @param {String} url 脚本url(必需)
 */
function $include(url) {
    if (/\.js$/i.test(url) == true) {
        document.write('<script language=\"javascript\" type=\"text/javascript\" src=\"' + url + '?v=' + new Date().getTime() + '\"><\/script>');
    }

    if (/\.css$/i.test(url) == true) {
        document.write('<link rel=\"stylesheet\" type=\"text/css\" href=\"' + url + '?v=' + new Date().getTime() + '\">');
    }
}


/*
 * 增加脚本文件(加载完成回调)
 * @param {String} url 脚本url(必需)
 * @param {Object} callback 回调函数(可选)
 */
function $script(url, callback) {
    if (/\.js$/i.test(url) == true) {
        var script = $create({
            tag: 'script', 
            attr: {
                language: 'javascript', 
                type: 'text/javascript', 
                src: url + '?v=' + new Date().getTime()
                }
            }).$add($tag('head')[0]);
    }

    if (/\.css$/i.test(url) == true) {
        var script = $create({
            tag: 'link', 
            attr: {
                type: 'text/css', 
                rel: 'stylesheet', 
                href: url + '?v=' + new Date().getTime()
                }
            }).$add($tag('head')[0]);
    }

    if (script.readyState) {
        script.onreadystatechange = function() {
            if (script.readyState == 'loaded' || script.readyState == 'complete') {
                script.onreadystatechange = null;

                if (callback != undefined) {
                    if (typeof(callback) == 'function') {
                        callback();
                    } else if (typeof(callback) == 'string') {
                        window.eval('var _function = ' + callback);
                        _function();
                    }
                }
            }
            };
    } else {
        script.onload = function() {
            if (callback != undefined) {
                if (typeof(callback) == 'function') {
                    callback();
                } else if (typeof(callback) == 'string') {
                    window.eval('var _function = ' + callback);
                    _function();
                }
            }
            };
    }
}


/*
 * 验证浏览器用户代理头
 * @param {String} pattern 正则表达式(必需)
 * @return {Boolean} 返回是否匹配
 */
function $agent(pattern) {
    return pattern.test(navigator.userAgent);
}


/*
 * 浏览器HTML5支持验证
 * @return {Boolean} 返回是否支持
 */
function $html5() {
    if (typeof(Worker) == 'undefined') {
        return false;
    } else {
        return true;
    }
}


/*
 * 获取元素offsetTop
 * @param {Object} element 元素id值或对象(必需)
 * @return {Number} 返回像素数值
 */
function $top(element) {
    if (typeof(element) == 'string') {
        element = $id(element);
    }

    if (element == null) {
        return 0;
    }

    var top = 0;

    top = element.offsetTop;

    while (element = element.offsetParent) {
        top += element.offsetTop;
    }

    return top;
}


/*
 * 获取元素offsetLeft
 * @param {Object} element 元素id值或对象(必需)
 * @return {Number} 返回像素数值
 */
function $left(element) {
    if (typeof(element) == 'string') {
        element = $id(element);
    }

    if (element == null) {
        return 0;
    }

    var left = 0;

    left = element.offsetLeft;

    while (element = element.offsetParent) {
        left += element.offsetLeft;
    }

    return left;
}


/*
 * 获取页面尺寸位置
 * @return {Number} 返回像素数值
 */
function $page() {
    return {
        client: {
            width: document.documentElement.clientWidth || document.body.clientWidth, 
            height: document.documentElement.clientHeight || document.body.clientHeight
        }, 
        scroll: {
            top: document.documentElement.scrollTop || document.body.scrollTop, 
            left: document.documentElement.scrollLeft || document.body.scrollLeft, 
            width: document.body.scrollWidth > document.documentElement.scrollWidth ? document.body.scrollWidth : scrollWidth = document.documentElement.scrollWidth, 
            height: document.body.scrollHeight > document.documentElement.scrollHeight ? document.body.scrollHeight : document.documentElement.scrollHeight
        }
    };
}


/*
 * Ajax
 */
 
 
/*
 * 构建request对象
 * @return {Object} 返回request对象
 */
function $request() {
    var request = false;

    if (window.XMLHttpRequest) {
        request = new XMLHttpRequest();

        if (request.overrideMimeType) {
            request.overrideMimeType('text/plain');
        }
    } else if (window.ActiveXObject) {
        try {
            request = new ActiveXObject('Msxml2.XMLHTTP');
            } catch(e) {
                try {
                    request = new ActiveXObject('Microsoft.XMLHTTP');
                    } catch(e) {}
                }
    }

    return request;
}


/*
 * ajax方法
 * @param {Object} json json格式数据(必需)
 *                 json[type] [GET|POST|HEAD]请求类型(可选)
 *                 json[url] 请求网址(必需)
 *                 json[async] [true|false] 是否异步请求(可选)
 *                 json[content] 请求Content-Type(可选)
 *                 json[data] 发送数据(可选)
 *                 json[timeout] 超时时间/毫秒(可选)
 *                 json[callback] 回调函数(可选)
 * @example $ajax({type: 'GET', url: 'test.json', async: true, callback: function(data) {}});
 */
function $ajax(json) {
    var http = $request();
    var type = json.type || 'GET';
    var url = json.url || null;
    var async = json.async || true;
    var content = json.content || 'application/x-www-form-urlencoded';
    var data = json.data || null;
    var timeout = json.timeout || null;
    var callback = json.callback || null;

    if (url == null) {
        return;
    } else {
        url = $url(url);
    }

    http.open(type, url, async);
    http.setRequestHeader('Content-Type', content);
    http.send(data);

    if (timeout != null) {
        window.setTimeout(function() {
            http.abort();
            return;
            }, timeout);
    }

    if (callback != null) {
        if (async == true) {
            http.onreadystatechange = function() {
                if (http.readyState == 4) {
                    if (type == 'GET' || type == 'POST') {
                        if (http.status == 200) {
                            if (typeof(callback) == 'function') {
                                callback(http.responseText);
                            } else if (typeof(callback) == 'string') {
                                window.eval('var _function = ' + callback);
                                _function(http.responseText);
                            }
                        }
                    } else {
                        callback(http);
                    }
                }
                };
        } else {
            if (http.readyState == 4) {
                if (type == 'GET' || type == 'POST') {
                    if (http.status == 200) {
                        if (typeof(callback) == 'function') {
                            callback(http.responseText);
                        } else if (typeof(callback) == 'string') {
                            window.eval('var _function = ' + callback);
                            _function(http.responseText);
                        }
                    }
                } else {
                    callback(http);
                }
            }
        }
    }
}


/*
 * Common
 */


/*
 * json数据排序
 * @param {String} field 排序字段(必需)
 * @param {Boolean} reverse [true|false] 是否采用倒序(必需)
 * @param {Object} primer [String|parseInt|Number|Date] 字段数据类型(必需)
 * @return {Number} 返回数值
 */
function $jsonSort(field, reverse, primer) {
    if (primer == Date) {
        return function(a, b) {
            a = a[field];
            b = b[field];

            if (reverse == false) {
                return new Date(a).getTime() - new Date(b).getTime();
            }

            if (reverse == true) {
                return new Date(b).getTime() - new Date(a).getTime();
            }
        }
    } else {
        reverse = (reverse) ? -1 : 1;

        return function(a, b) {
            a = a[field];
            b = b[field];

            if (typeof(primer) != 'undefined') {
                a = primer(a);
                b = primer(b);
            }

            if (a < b) {
                return reverse * -1;
            }

            if (a > b) {
                return reverse * 1;
            }

            return 0;
        }
    }
}


/*
 * json字符转义
 * @param {String} json json格式数据(必需)
 * @return {String} 返回json转义字符串
 */
function $jsonEscape(json) {
    if (json.length == 0) {
        return '';
    }

    json = json.replace(/\'/g, '\\\'');
    json = json.replace(/\"/g, '\\"');
    json = json.replace(/\\/g, '\\\\');
    json = json.replace(/\//g, '\\\/');
    json = json.replace(/\./g, '\\.');
    json = json.replace(/\+/g, '\\+');
    json = json.replace(/\-/g, '\\-');
    json = json.replace(/\*/g, '\\*');
    json = json.replace(/\?/g, '\\?');
    json = json.replace(/\=/g, '\\=');
    json = json.replace(/\|/g, '\\|');
    json = json.replace(/\(/g, '\\(');
    json = json.replace(/\)/g, '\\)');
    json = json.replace(/\[/g, '\\[');
    json = json.replace(/\]/g, '\\]');
    json = json.replace(/\{/g, '\\{');
    json = json.replace(/\}/g, '\\}');

    return json;
}


/*
 * js字符转义
 * @param {String} js js格式数据(必需)
 * @return {String} 返回js转义字符串
 */
function $jsEscape(js) {
    if (js.length == 0) {
        return '';
    }

    js = js.replace(/\'/g, '\'');
    js = js.replace(/\'/g, '\'');
    js = js.replace(/\&/g, '\&');
    js = js.replace(/\\/g, '\\');

    return js;
}


/*
 * html编码
 * @param {String} html html代码(必需)
 * @return {String} 返回html编码
 */
function $htmlEncode(html, wrap) {
    if (html.length == 0) {
        return '';
    }

    if (wrap == undefined) {
        wrap = true;
    }

    html = html.replace(/'/g, '&apos;');
    html = html.replace(/"/g, '&quot;');
    html = html.replace(/</g, '&lt;');
    html = html.replace(/>/g, '&gt;');
    html = html.replace(/ /g, '&nbsp;');

    if (wrap == true) {
        html = html.replace(/\n/g, '<br \/>');
    }

    return html;
}


/*
 * html解码
 * @param {String} text html编码(必需)
 * @return {String} 返回html代码
 */
function $htmlDecode(html, wrap) {
    if (html.length == 0) {
        return '';
    }

    if (wrap == undefined) {
        wrap = true;
    }

    html = html.replace(/&apos;/g, '\'');
    html = html.replace(/&quot;/g, '"');
    html = html.replace(/&lt;/g, '<');
    html = html.replace(/&gt;/g, '>');
    html = html.replace(/&nbsp;/g, ' ');

    if (wrap == true) {
        html = html.replace(/<br \/>/g, '\n');
    }

    return html;
}


/*
 * 时间格式化
 * @param {String} code 时间代码(必需)
 * @param {String} time 自定义时间(可选)
 * @return {String} 返回时间
 */
function $formatTime(code, time) {
    if (time == undefined) {
        time = new Date();
    }

    code = code.replace(/yyyy/, time.getFullYear());
    code = code.replace(/yy/, time.getYear());
    code = code.replace(/MM/, (time.getMonth() + 1).toString().replace(/^(\d)$/g, '0$1'));
    code = code.replace(/M/, time.getMonth() + 1);
    code = code.replace(/dd/, time.getDate().toString().replace(/^(\d)$/g, '0$1'));
    code = code.replace(/d/, time.getDate());
    code = code.replace(/HH/, time.getHours().toString().replace(/^(\d)$/g, '0$1'));
    code = code.replace(/H/, time.getHours());
    code = code.replace(/mm/, time.getMinutes().toString().replace(/^(\d)$/g, '0$1'));
    code = code.replace(/m/, time.getMinutes());
    code = code.replace(/ss/, time.getSeconds().toString().replace(/^(\d)$/g, '0$1'));
    code = code.replace(/s/, time.getSeconds());
    code = code.replace(/fff/, time.getMilliseconds());

    return code;
}