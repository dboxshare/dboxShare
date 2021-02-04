/*
 * fastui library for dboxshare v0.2.1.2012
 * based on fastdom.js
 */


var fastui = {};
var inited = false;
var loaded = false;


/*
 * 页面初始化事件
 */
(function() {
    var state = false;

    document.onreadystatechange = function() {
        if (document.readyState == 'interactive' || document.readyState == 'complete') {
            if (state == true) {
                return;
            } else {
                state = true;
            }

            try {
                _init();
                } catch(e) {}
        }
        };
    })();


/*
 * 页面加载事件
 */
(function() {
    window.onload = function() {
        try {
            _load();
            } catch(e) {}
        };
    })();


/*
 * 页面退出事件
 */
(function() {
window.onunload = function() {
    $clear('iframe');

    try {
        _unload();
        } catch(e) {}
    };
    })();


/*
 * 页面调整事件
 */
(function() {
    window.onresize = function() {
        if (arguments.callee.timer) {
            window.clearTimeout(arguments.callee.timer);
        }

        arguments.callee.timer = window.setTimeout(function() {
            try {
                _resize();
                } catch(e) {}
        }, 100);
    };
    })();


/*
 * 页面预初始化函数
 */
function _init() {
    // 调整页面字体大小
    var clientWidth = window.top.$page().client.width;
    var fontSize = '12px';

    if (clientWidth > 2560) {
        fontSize = '18px';
    } else if (clientWidth > 2304) {
        fontSize = '17px';
    } else if (clientWidth > 2048) {
        fontSize = '16px';
    } else if (clientWidth > 1792) {
        fontSize = '15px';
    } else if (clientWidth > 1536) {
        fontSize = '14px';
    } else if (clientWidth > 1280) {
        fontSize = '13px';
    } else {
        fontSize = '12px';
    }

    $tag('html')[0].style.fontSize = fontSize;
    $tag('body')[0].style.fontSize = fontSize;

    // 语言处理
    try {
        lang.init();
        } catch(e) {
            try {
                init();
                } catch(e) {}

            inited = true;

            _load();
            }
}


/*
 * 页面预加载函数
 */
function _load() {
    if (inited == true && loaded == false) {
        try {
            load();
            } catch(e) {}

        loaded = true;
    }
}


/*
 * 页面预退出函数
 */
function _unload() {
    try {
        unload();
        } catch(e) {}
}


/*
 * 页面预调整函数
 */
function _resize() {
    try {
        resize();
        } catch(e) {}
}


(function() {
fastui = {

/*
 * 模拟表单组件
 */

    /*
     * 生成单选框视图
     */
    radioSelect: function(id, json, value, text, color, callback) {
        var box = $id(id);

        if (box == null) {
            return;
        }

        box.className = 'radio-select';

        var input = box.$tag('input')[0];

        if (input == null) {
            return;
        }

        var valued = input.value;

        for (var i = 0; i < json.length; i++) {
            var label = $create({
                tag: 'label', 
                html: json[i][text]
                }).$add(box);

            (function(index) {
                label.onclick = function() {
                    var labels = box.$tag('label');

                    input.value = json[index][value];

                    if (typeof(callback) == 'function') {
                        callback(json[index][value], json[index][text]);
                    } else if (typeof(callback) == 'string') {
                            window.eval('var callbackFunction = ' + callback);
                            callbackFunction(json[index][value], json[index][text]);
                    }

                    for (var j = 0; j < json.length; j++) {
                        if (json[j][value] == json[index][value]) {
                            labels[j].style.color = '#FFFFFF';
                            labels[j].style.backgroundColor = color;
                        } else {
                            labels[j].style.color = '';
                            labels[j].style.backgroundColor = '';
                        }
                    }
                };
                })(i);

            if ((i == 0 && valued.length == 0) || json[i][value] == valued) {
                input.value = json[i][value];

                label.style.color = '#FFFFFF';
                label.style.backgroundColor = color;
            } else {
                label.style.color = '';
                label.style.backgroundColor = '';
            }
        }
    },


    /*
     * 生成复选框视图
     */
    checkboxSelect: function(id, json, value, text, color) {
        var box = $id(id);

        if (box == null) {
            return;
        }

        box.className = 'checkbox-select';

        var input = box.$tag('input')[0];

        if (input == null) {
            return;
        }

        var valued = input.value;

        for (var i = 0; i < json.length; i++) {
            var label = $create({
                tag: 'label', 
                attr: {'for': id + '_' + i}, 
                html: json[i][text]
                }).$add(box);

            var checkbox = $create({
                tag: 'input', 
                attr: {
                    type: 'checkbox', 
                    id: id + '_' + i, 
                    name: id, 
                    value: json[i][value]
                    }
                }).$add(label);

            (function(label, checkbox) {
                label.onclick = function() {
                    if (checkbox.checked == true) {
                        checkbox.checked = false;
                        label.style.color = '';
                        label.style.backgroundColor = '';
                    } else {
                        checkbox.checked = true;
                        label.style.color = '#FFFFFF';
                        label.style.backgroundColor = color;
                    }
                    };
                })(label, checkbox);

            if (valued.length > 0) {
                if (checkboxSelectChecked(json[i][value], valued) == true) {
                    checkbox.checked = true;
                    label.style.color = '#FFFFFF';
                    label.style.backgroundColor = color;
                } else {
                    checkbox.checked = false;
                    label.style.color = '';
                    label.style.backgroundColor = '';
                }
            }
        }
    },


    /*
     * 判断复选框是否选中
     */
    checkboxSelectChecked: function(value, valued) {
        var values = valued.split(',');

        for (var i = 0; i < values.length; i++) {
            if (values[i] == value) {
                return true;
            }
        }

        return false;
    },


    /*
     * 获取复选框选中项目
     */
    checkboxSelectValue: function(id) {
        var checkboxes = $id(id).$tag('input');
        var data = '';

        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked == true) {
                data += checkboxes[i].value + ',';
            }
        }

        if (data.length > 0) {
            data = data.substring(0, data.length - 1);
        }

        return data;
    },


    /*
     * 生成列表选择框视图
     */
    listSelect: function(id, json, value, text, init, callback) {
        var box = $id(id);

        if (box == null) {
            return;
        }

        box.className = 'list-select';

        if (init == undefined) {
            init = true;
        }

        var input = box.$tag('input')[0];

        if (input == null) {
            return;
        }

        var valued = input.value;

        var label = $create({tag: 'label'}).$add(box);

        var ul = $create({tag: 'ul'}).$add(box);

        for (var i = 0; i < json.length; i++) {
            var li = $create({
                tag: 'li', 
                html: '<div class="item"><div class="text">' + json[i][text] + '</div></div>'
                }).$add(ul);

            (function(index) {
                li.onclick = function() {
                    label.innerHTML = json[index][text];

                    input.value = json[index][value];

                    ul.style.display = 'none';

                    if (typeof(callback) == 'function') {
                        callback(json[index][value], json[index][text]);
                    } else if (typeof(callback) == 'string') {
                            window.eval('var callbackFunction = ' + callback);
                            callbackFunction(json[index][value], json[index][text]);
                    }
                };
                })(i);

            if (init == true) {
                if (input.value.length == 0 || json[i][value] == valued) {
                    input.value = json[i][value];
                    label.innerHTML = json[i][text];
                }
            }
        }

        if (label.innerHTML.length == 0) {
            input.value = 0;
            label.innerHTML = '--';
        }

        ul.style.display = 'block';

        box.style.width = fastui.listSelectWidth(box) + 'px';

        ul.style.width = box.offsetWidth + 'px';

        ul.style.display = 'none';

        box.onclick = function(_event) {
            var event = _event || window.event;
            var tag = event.target || event.srcElement;

            if ($contains(ul, tag) == true) {
                return;
            }

            box.style.zIndex = '1';

            ul.style.display = 'block';

            var height = ul.$tag('li')[0].offsetHeight;
            var count = 0;

            if (box.offsetTop + box.offsetHeight < $page().client.height / 2) {
                count = parseInt(($page().client.height - (box.offsetTop + box.offsetHeight)) / height);

                ul.style.top = box.clientHeight + 'px';
            } else {
                count = parseInt(box.offsetTop / height);

                ul.style.top = '-' + ul.offsetHeight + 'px';
            }

            if (json.length > count) {
                ul.style.height = (height * (count - 1)) + 'px';
            }

            ul.onmouseenter = function() {
                box.style.zIndex = '1';
                ul.style.display = 'block';
                };

            ul.onmouseleave = function() {
                box.style.zIndex = '0';
                ul.style.display = 'none';
                };

            box.onmouseleave = function() {
                box.style.zIndex = '0';
                ul.style.display = 'none';
                };
            };
    },


    /*
     * 获取树形列表宽度
     */
    listSelectWidth: function(box) {
        var items = box.$class('item');
        var width = 0;

        for (var i = 0; i < items.length; i++) {
            var elements = items[i].$tag('div');
            var count = 0;

            for (var j = 0; j < elements.length; j++) {
                count += elements[j].offsetWidth;
            }

            if (count > width) {
                width = count;
            }
        }

        return width + 24;
    },


    /*
     * 生成树形列表选择框视图
     */
    listTreeSelect: function(id, json, value, parent, text, start, limit, init) {
        var box = $id(id);

        if (box == null) {
            return;
        }

        box.className = 'list-select';

        if (init == undefined) {
            init = true;
        }

        var input = box.$tag('input')[0];

        if (input == null) {
            return;
        }

        var valued = input.value;

        var label = $create({tag: 'label'}).$add(box);

        var ul = $create({tag: 'ul'}).$add(box);

        for (var i = 0; i < json.length; i++) {
            if (json[i][parent] == start) {
                var li = $create({
                    tag: 'li', 
                    html: '<div class="item"><div class="text">' + json[i][text] + '</div></div>'
                    }).$add(ul);

                (function(index) {
                    li.onclick = function() {
                        label.innerHTML = json[index][text];

                        input.value = json[index][value];

                        ul.style.display = 'none';
                    };
                    })(i);

                if (init == true) {
                    if (input.value.length == 0 || json[i][value] == valued) {
                        input.value = json[i][value];
                        label.innerHTML = json[i][text];
                    }
                }

                fastui.listTreeSelectNode(id, valued, json, value, parent, text, json[i][value], init, 1);

                if (json[i][value] == limit) {
                    break;
                }
            }
        }

        if (label.innerHTML.length == 0) {
            input.value = 0;
            label.innerHTML = '--';
        }

        ul.style.display = 'block';

        box.style.width = fastui.listSelectWidth(box) + 'px';

        ul.style.width = box.offsetWidth + 'px';

        ul.style.display = 'none';

        box.onclick = function(_event) {
            var event = _event || window.event;
            var tag = event.target || event.srcElement;

            if ($contains(ul, tag) == true) {
                return;
            }

            box.style.zIndex = '1';

            ul.style.display = 'block';

            var height = ul.$tag('li')[0].offsetHeight;
            var count = 0;

            if (box.offsetTop + box.offsetHeight < $page().client.height / 2) {
                count = parseInt(($page().client.height - (box.offsetTop + box.offsetHeight)) / height);

                ul.style.top = box.clientHeight + 'px';
            } else {
                count = parseInt(box.offsetTop / height);

                ul.style.top = '-' + ul.offsetHeight + 'px';
            }

            if (json.length > count) {
                ul.style.height = (height * (count - 1)) + 'px';
            }

            ul.onmouseenter = function() {
                box.style.zIndex = '1';
                ul.style.display = 'block';
                };

            ul.onmouseleave = function() {
                box.style.zIndex = '0';
                ul.style.display = 'none';
                };

            box.onmouseleave = function() {
                box.style.zIndex = '0';
                ul.style.display = 'none';
                };
            };
    },


    /*
     * 生成树形列表选择框节点视图
     */
    listTreeSelectNode: function(id, valued, json, value, parent, text, start, init, level) {
        var box = $id(id);
        var input = box.$tag('input')[0];
        var label = box.$tag('label')[0];
        var ul = box.$tag('ul')[0];

        for (var i = 0; i < json.length; i++) {
            if (json[i][parent] == start) {
                var li = $create({
                    tag: 'li', 
                    html: '<div class="item"><div class="line" style="width: ' + (24 * level) + 'px;"></div><div class="text">' + json[i][text] + '</div></div>'
                    }).$add(ul);

                (function(index) {
                    li.onclick = function() {
                        label.innerHTML = json[index][text];

                        input.value = json[index][value];

                        ul.style.display = 'none';
                    };
                    })(i);

                if (init == true) {
                    if (json[i][value] == valued) {
                        input.value = json[i][value];
                        label.innerHTML = json[i][text];
                    }
                }

                fastui.listTreeSelectNode(id, valued, json, value, parent, text, json[i][value], init, level + 1);
            }
        }
    },


/*
 * 弹出窗口组件
 */

    /*
     * 生成弹出窗口视图
     */
    windowPopup: function(id, title, url, width, height) {
        if (width == 0 || width > $page().client.width) {
            width = $page().client.width;
        }

        if (height == 0 || height > $page().client.height) {
            height = $page().client.height;
        }

        if (width < 0) {
            width = $page().client.width - -(width);
        }

        if (height < 0) {
            height = $page().client.height - -(height);
        }

        var layer = $id(id + '-popup-window');
        var html = '';

        if (layer != null) {
            return;
        }

        html += '<div class="header">';
        html += '<span class="title">' + title + '</span>';
        html += '<span class="close"><img src="/ui/images/window-close-icon.png" width="16" onClick="fastui.windowClose(\'' + id + '\');" /></span>';
        html += '</div>';
        html += '<div class="content">';
        html += '<iframe id="' + id + '-iframe" src="' + $url(url) + '" width="' + width + '" height="' + (height - 40) + '" frameborder="0" scrolling="yes"></iframe>';
        html += '</div>';

        fastui.coverShow(id + '-page-cover');

        layer = $create({
            tag: 'div', 
            attr: {id: id + '-popup-window'}, 
            style: {
                width: width + 'px', 
                height: height + 'px'
                }, 
            css: 'popup-window', 
            html: html
            }).$add(document.body);

        layer.style.top = Math.ceil(($page().client.height - layer.offsetHeight) / 2) + 'px';
        layer.style.left = Math.ceil(($page().client.width - layer.offsetWidth) / 2) + 'px';
    },


    /*
     * 关闭弹出窗口视图
     */
    windowClose: function(id) {
        var layer = $id(id + '-popup-window');

        layer.$remove();

        fastui.coverHide(id + '-page-cover');
    },


/*
 * 对话框组件
 */

    /*
     * 生成对话框视图(alert)
     */
    dialogAlert: function(message, ok) {
        if (ok == undefined) {
            ok = '';
        }

        var layer = $id('dialog-box');
        var html = '';

        if (layer != null) {
            return;
        }

        html += '<div class="header"></div>';
        html += '<div class="content">';
        html += '<div class="icon"><img src="/ui/images/dialog-alert-icon.png" width="64" /></div>';
        html += '<div class="message">' + message + '</div>';
        html += '</div>';
        html += '<div class="footer">';
        html += '<span class="button-ok" onClick="fastui.dialogClose();' + ok + '">' + lang.ui.button['confirm'] + '</span>';
        html += '</div>';

        fastui.coverShow('page-cover');

        layer = $create({
            tag: 'div', 
            attr: {id: 'dialog-box'}, 
            css: 'dialog-box', 
            html: html
            }).$add(document.body);

        layer.style.top = Math.ceil(($page().client.height - layer.offsetHeight) / 2) + 'px';
        layer.style.left = Math.ceil(($page().client.width - layer.offsetWidth) / 2) + 'px';
    },


    /*
     * 生成对话框视图(confirm)
     */
    dialogConfirm: function(message, ok, cancel) {
        if (ok == undefined) {
            ok = '';
        }

        if (cancel == undefined) {
            cancel = '';
        }

        var layer = $id('dialog-box');
        var html = '';

        if (layer != null) {
            return;
        }

        html += '<div class="header"></div>';
        html += '<div class="content">';
        html += '<div class="icon"><img src="/ui/images/dialog-confirm-icon.png" width="64" /></div>';
        html += '<div class="message">' + message + '</div>';
        html += '</div>';
        html += '<div class="footer">';
        html += '<span class="button-ok" onClick="fastui.dialogClose();' + ok + '">' + lang.ui.button['confirm'] + '</span>';
        html += '<span class="button-cancel" onClick="fastui.dialogClose();' + cancel + '">' + lang.ui.button['cancel'] + '</span>';
        html += '</div>';

        fastui.coverShow('page-cover');

        layer = $create({
            tag: 'div', 
            attr: {id: 'dialog-box'}, 
            css: 'dialog-box', 
            html: html
            }).$add(document.body);

        layer.style.top = Math.ceil(($page().client.height - layer.offsetHeight) / 2) + 'px';
        layer.style.left = Math.ceil(($page().client.width - layer.offsetWidth) / 2) + 'px';
    },


    /*
     * 关闭对话框视图
     */
    dialogClose: function() {
        var layer = $id('dialog-box');

        layer.$remove();

        fastui.coverHide('page-cover');
    },


/*
 * 提示组件
 */

    /*
     * 文本提示
     */
    textTips: function(message, time, position) {
        var tips = $id('text-tips');

        if (tips == null) {
            tips = $create({
                tag: 'div', 
                attr: {id: 'text-tips'}, 
                css: 'text-tips', 
                html: message
                }).$add(document.body);
        } else {
            tips.innerHTML = message;
        }

        if (position == undefined) {
            tips.style.top = Math.ceil(($page().client.height - tips.offsetHeight) / 2) + 'px';
        } else {
            if (position > 0) {
                tips.style.top = position + 'px';
            } else if (position < 0) {
                tips.style.bottom = -(position) + 'px';
            } else {
                tips.style.top = Math.ceil(($page().client.height - tips.offsetHeight) / 2) + 'px';
            }
        }

        tips.style.left = Math.ceil(($page().client.width - tips.offsetWidth) / 2) + 'px';

        if (time == undefined) {
            time = 5000;
        }

        if (time == 0) {
            tips.onclick = function() {
                tips.$remove();
                };
        } else {
            window.setTimeout(function() {
                tips.$remove();
                }, time);
        }
    },


    /*
     * 图标提示
     */
    iconTips: function(type, time) {
        var tips = $id('icon-tips');

        var html = '<img src="/ui/images/tips-' + type + '-icon.png" width="80" />';

        if (tips == null) {
            tips = $create({
                tag: 'div', 
                attr: {id: 'icon-tips'}, 
                css: 'icon-tips', 
                html: html
                }).$add(document.body);
        } else {
            tips.innerHTML = html;
        }

        tips.style.top = Math.ceil(($page().client.height - tips.offsetHeight) / 2) + 'px';
        tips.style.left = Math.ceil(($page().client.width - tips.offsetWidth) / 2) + 'px';

        if (time == undefined) {
            time = 3000;
        }

        window.setTimeout(function() {
            tips.$remove();
            }, time);
    },


    /*
     * 输入框提示(默认值)
     */
    valueTips: function(input, message) {
        if (typeof(input) == 'string') {
            input = $id(input);
        }

        if (input == null) {
            return;
        }

        if (input.tagName.toLowerCase() != 'input' && input.tagName.toLowerCase() != 'textarea') {
            return;
        }

        input.parentNode.style.position = 'relative';

        var tips = $id(input.id + '-value-tips');

        if (tips == null) {
            tips = $create({
                tag: 'span', 
                attr: {id: input.id + '-value-tips'}, 
                css: 'value-tips', 
                html: message
                }).$add(input.parentNode);
        } else {
            tips.innerHTML = message;
        }

        if (input.value.length > 0) {
            tips.style.display = 'none';
        }

        tips.style.top = input.offsetTop + 'px';
        tips.style.left = input.offsetLeft + 'px';
        tips.style.width = input.$style('width');
        tips.style.height = input.$style('height');

        input.onblur = function() {
            if (input.value.length == 0) {
                tips.style.display = 'block';
            }
            };

        input.onfocus = function() {
            tips.style.display = 'none';
            };

        tips.onclick = function() {
            tips.style.display = 'none';
            input.focus();
            };
    },


    /*
     * 输入框提示(对话框)
     */
    inputTips: function(input, message, time) {
        if (typeof(input) == 'string') {
            input = $id(input);
        }

        if (input == null) {
            return;
        }

        if (input.tagName.toLowerCase() != 'input' && input.tagName.toLowerCase() != 'textarea') {
            return;
        }

        input.focus();

        input.parentNode.style.position = 'relative';

        if (time == undefined) {
            time = 5000;
        }

        var tips = $id(input.id + '-input-tips');

        if (tips == null) {
            tips = $create({
                tag: 'div', 
                attr: {id: input.id + '-input-tips'}, 
                css: 'input-tips', 
                html: message
                }).$add(input.parentNode);
        } else {
            tips.innerHTML = message;
        }

        tips.style.top = (input.offsetTop + input.offsetHeight) + 'px';
        tips.style.left = input.offsetLeft + 'px';

        window.setTimeout(function() {
            tips.$remove();
            }, time);
    },


    /*
     * 输入框提示(侧边)
     */
    sideTips: function(input, message) {
        if (typeof(input) == 'string') {
            input = $id(input);
        }

        if (input == null) {
            return;
        }

        var tips = $id(input.id + '-side-tips');

        if (tips == null) {
            tips = $create({
                tag: 'span', 
                attr: {id: input.id + '-side-tips'}, 
                css: 'side-tips', 
                html: message
                }).$add(input.parentNode);
        } else {
            tips.innerHTML = message;
        }

        tips.style.top = input.offsetTop + 'px';
        tips.style.left = (input.offsetWidth + 8) + 'px';
    },


    /*
     * 遮盖层提示
     */
    coverTips: function(message) {
        var tips = $id('cover-tips');

        fastui.coverShow('page-cover');

        if (tips == null) {
            tips = $create({
                tag: 'div', 
                attr: {id: 'cover-tips'}, 
                css: 'cover-tips', 
                html: message
                }).$add(document.body);
        } else {
            tips.innerHTML = message;
        }

        tips.style.top = Math.ceil(($page().client.height - tips.offsetHeight) / 2) + 'px';
        tips.style.left = Math.ceil(($page().client.width - tips.offsetWidth) / 2) + 'px';
    },


/*
 * 输入框建议组件
 */

    /*
     * 输入框建议初始化
     */
    inputSuggest: function(input, url) {
        if (input.onkeyup == null) {
            input.onkeyup = function() {
                if (input.value.length > 0) {
                    fastui.inputSuggestQuery(input, url);
                }
                };
        }
    },


    /*
     * 输入框建议查询
     */
    inputSuggestQuery: function(input, url) {
        url += '?' + input.id + '=' + escape(input.value);

        $ajax({
            type: 'GET', 
            url: url, 
            async: true, 
            callback: function(data) {
                if ($jsonString(data) == false) {
                    return;
                } else {
                    window.eval('var json = ' + data + ';');

                    var items = '';

                    for (var i = 0; i < json.length; i++) {
                        items += '<li onClick="fastui.inputSuggestSelect(\'' + input.id + '\', \'' + json[i].value + '\');">' + json[i].text + '</li>';
                    }

                    input.parentNode.style.position = 'relative';
                    input.parentNode.style.zIndex = '1000';

                    var suggest = $class(input.parentNode, 'input-suggest')[0];

                    if (suggest == null) {
                        suggest = $create({
                            tag: 'div', 
                            style: {
                                width: input.offsetWidth + 'px'
                                }, 
                            css: 'input-suggest'
                            }).$add(input.parentNode);
                    }

                    suggest.innerHTML = '<ul>' + items + '</ul>';

                    suggest.style.display = 'block';

                    var height = suggest.$tag('li')[0].offsetHeight;
                    var count = 0;

                    if ($top(input) + input.offsetHeight < $page().client.height / 2) {
                        count = parseInt(($page().client.height - ($top(input) + input.offsetHeight)) / height);

                        suggest.style.top = (input.offsetTop + input.offsetHeight) + 'px';
                    } else {
                        count = parseInt($top(input) / height);

                        suggest.style.top = '-' + (suggest.offsetHeight - input.offsetTop) + 'px';
                    }

                    if (json.length > count) {
                        suggest.style.height = (height * count) + 'px';
                    }

                    suggest.style.left = input.offsetLeft + 'px';

                    suggest.onmouseenter = function() {
                        input.parentNode.style.zIndex = '1000';

                        suggest.style.display = 'block';
                        };

                    suggest.onmouseleave = function() {
                        input.parentNode.style.zIndex = '';

                        suggest.style.display = 'none';
                        };
                }
                }
            });
    },


    /*
     * 输入框建议选择
     */
    inputSuggestSelect: function(id, value) {
        var input = $id(id);

        input.value = value;
        input.focus();

        var suggest = $class(input.parentNode, 'input-suggest')[0];

        suggest.style.display = 'none';
        suggest.innerHTML = '';
    },


/*
 * 验证组件
 */

    /*
     * 输入框匹配验证
     */
    testInput: function(required, input, pattern) {
        if (typeof(input) == 'string') {
            input = $id(input);
        }

        if (required == true) {
            if (input.value == '') {
                try {
                    input.focus();
                    } catch (e) {}

                return false;
            }
        }

        if (input.value != '') {
            if (!pattern.exec(input.value)) {
                try {
                    input.focus();
                    } catch (e) {}

                return false;
            }
        }

        return true;
    },


    /*
     * 字符串匹配验证
     */
    testString: function(data, pattern) {
        if (!pattern.exec(data)) {
            return false;
        } else {
            return true;
        }
    },


    /*
     * 比较验证
     */
    testCompare: function(a, b) {
        if ($id(a) != null) {
            a = $id(a);
        }

        if ($id(b) != null) {
            b = $id(b);
        }

        if (a.value == b.value) {
            return true;
        } else {
            try {
                b.focus();
                } catch (e) {}

            return false;
        }
    },


/*
 * 遮盖组件
 */

    /*
     * 页面遮盖层隐藏
     */
    coverHide: function(id) {
        var cover = $id(id);

        if (cover != null) {
            cover.style.display = 'none';
        }
    },


    /*
     * 页面遮盖层显示
     */
    coverShow: function(id) {
        if (window.self.location.pathname != window.parent.location.pathname) {
            var covers = window.parent.$class('page-cover');
            var light = false;

            for (var i = 0; i < covers.length; i++) {
                if (covers[i].style.display == 'block') {
                    light = true;
                    break;
                }
            }
        }

        var cover = $id(id);
        var css = '';

        if (/^([\w\-]+)?page-cover$/.test(id) == true) {
            css = 'page-cover';
        } else if (/^([\w\-]+)?loading-cover$/.test(id) == true) {
            css = 'loading-cover';
        } else if (/^([\w\-]+)?waiting-cover$/.test(id) == true) {
            css = 'waiting-cover';
        }

        if (cover == null) {
            cover = $create({
                tag: 'div', 
                attr: {id: id}, 
                css: css
                }).$add(document.body);
        }

        if (light == true) {
            cover.style.filter = 'alpha(opacity=50)';
            cover.style.opacity = '.5';
        }

        cover.style.display = 'block';
    }


    };
    })();