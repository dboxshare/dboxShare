/*
 * fastui library for dboxshare v0.2.1.2012
 * @module list
 */


(function() {
fastui.list = {
    path: '', 
    page: 0, 
    size: 0, 
    wait: false, 
    complete: false, 
    reverse: false, 


    /*
     * 列表页面布局调整
     */
    layoutResize: function(type) {
        var header = $id('datalist-header');
        var footer = $id('datalist-footer');
        var container = $id('datalist-container');

        if (typeof(type) == 'undefined') {
            var button = $id('button-list');
            var keyword = $id('keyword');

            // 适应页面缩小顶部工具栏查询框宽度
            if (button != null && keyword != null) {
                if (container.offsetWidth - button.offsetWidth < keyword.offsetWidth) {
                    keyword.style.width = '240px';
                }
            }

            // 获取顶部浮动栏高度
            if (header == null) {
                header = 0;
            } else {
                header = header.offsetHeight;
            }

            // 获取底部浮动栏高度
            if (footer == null) {
                footer = 0;
            } else {
                footer = footer.offsetHeight;
            }

            // 适应页面调整中部容器高度
            container.style.height = ($page().client.height - (header + footer)) + 'px';
        } else if (type == 'align-cells') {
            // 数据列表头单元格对齐
            if (header != null) {
                var headerTable = header.$tag('table')[0];

                if (headerTable != null) {
                    var headerThead = headerTable.rows[0];
                    var headerTd = headerThead.cells[headerThead.cells.length - 1];
                    var headerWidth = Number(headerTd.getAttribute('width')) || 0;
                    var scrollWidth = $id('datalist-container').offsetWidth - $id('data-list').offsetWidth;

                    headerTd.setAttribute('width', headerWidth + (scrollWidth > 17 ? 0 : scrollWidth));
                }
            }
        }
    },


    /*
     * 数据列表初始化
     */
    dataInit: function() {
        var list = $id('data-list');
        var rows = list.rows == null ? 0 : list.rows.length;

        if ($id('data-count') != null) {
            $id('data-count').innerHTML = 0;
        }

        for (var i = 0; i < rows; i++) {
            list.deleteRow(i);
            rows--;
            i--;
        }

        fastui.list.page = 0;
        fastui.list.size = 0;
        fastui.list.wait = false;
        fastui.list.complete = false;
        fastui.list.reverse = false;
    },


    /*
     * 数据列表查询
     */
    dataQuery: function() {
        var keyword = $id('keyword').value;

        if (keyword.length == 0) {
            $location('keyword', '');
            return;
        }

        $location('keyword', encodeURIComponent(keyword));
    },


    /*
     * 数据列表排序
     */
    dataSort: function(field, primer) {
        var list = $id('data-list');
        var rows = list.rows == null ? 0 : list.rows.length;

        for (var i = 0; i < rows; i++) {
            list.deleteRow(i);
            rows--;
            i--;
        }

        $id('datalist-container').scrollTop = 0;

        dataListView(field, fastui.list.reverse, primer);
    },


    /*
     * 数据列表排序更改
     */
    sortChange: function(field, reverse) {
        var fields = $name('sort-field');

        for (var i = 0; i < fields.length; i++) {
            if (fields[i].id.indexOf(field) > -1) {
                if (reverse == true) {
                    fields[i].style.backgroundImage = 'url(/ui/images/datalist-sortby-down-icon.png)';
                } else {
                    fields[i].style.backgroundImage = 'url(/ui/images/datalist-sortby-up-icon.png)';
                }

                fields[i].style.backgroundPosition = 'center right';
                fields[i].style.backgroundRepeat = 'no-repeat';
                fields[i].style.backgroundSize = '20px auto';
            } else {
                fields[i].style.backgroundImage = '';
            }
        }

        fastui.list.reverse = reverse == true ? false : true;
    },


    /*
     * 数据列表加载
     */
    dataLoad: function(path, init) {
        var container = $id('datalist-container');

        fastui.list.path = path;

        if (typeof(init) == 'boolean') {
            if (init == true) {
                fastui.list.dataInit();
            }
        }

        fastui.list.dataLoading(true);

        if (path.indexOf('?') == -1) {
            path += window.location.search;
        }

        $ajax({
            type: 'GET', 
            url: path, 
            async: true, 
            callback: function(data) {
                var count = $id('data-count');

                if ($jsonString(data) == false) {
                    if (count != null) {
                        count.innerHTML = lang.list['data-count'].replace(/\[count\]/, 0);
                    }

                    fastui.list.noData(true);
                } else {
                    $json('jsonData', '{items:' + data + '};');

                    if (count != null) {
                        count.innerHTML = lang.list['data-count'].replace(/\[count\]/, jsonData.items.length);
                    }

                    fastui.list.noData(false);

                    dataListView();

                    if (container.scrollHeight > container.clientHeight) {
                        fastui.list.layoutResize('align-cells');
                    }
                }

                window.setTimeout(function() {
                    fastui.list.dataLoading(false);
                    }, 500);
                }
            });
    },


    /*
     * 数据列表滚动加载
     */
    scrollDataLoad: function(path, init) {
        var container = $id('datalist-container');

        fastui.list.path = path;

        if (typeof(init) == 'boolean') {
            if (init == true) {
                fastui.list.dataInit();
            }
        }

        if (fastui.list.page == 0) {
            container.onscroll = function() {
                fastui.list.scrollLoadEvent();
                };
        }

        if (container.scrollHeight - container.clientHeight - container.scrollTop <= 8) {
            var isLoading = true;
        } else {
            var isLoading = false;
        }

        if (fastui.list.page == 0 || isLoading == true) {
            fastui.list.page = fastui.list.page + 1;
            fastui.list.wait = true;

            fastui.list.dataLoading(true);

            if (window.location.search.length == 0) {
                path += '?page=' + fastui.list.page;
            } else {
                if (path.indexOf('?') == -1) {
                    path += window.location.search + '&page=' + fastui.list.page;
                } else {
                    path += '&page=' + fastui.list.page;
                }
            }

            $ajax({
                type: 'GET', 
                url: path, 
                async: true, 
                callback: function(data) {
                    var count = $id('data-count');

                    if ($jsonString(data) == false) {
                        if (count != null && fastui.list.page == 1) {
                            count.innerHTML = lang.list['data-count'].replace(/\[count\]/, 0);
                        }

                        if (fastui.list.page == 1) {
                            fastui.list.noData(true);
                        }

                        fastui.list.complete = true;
                    } else {
                        if (fastui.list.page == 1) {
                            $json('jsonData', '{items:' + data + '};');
                        } else {
                            fastui.list.jsonMerge(data);
                        }

                        if (count != null) {
                            count.innerHTML = lang.list['data-count'].replace(/\[count\]/, jsonData.items.length);
                        }

                        if (fastui.list.page == 1) {
                            fastui.list.noData(false);
                        }

                        if (fastui.list.size == 0) {
                            fastui.list.size = jsonData.items.length;
                        }

                        dataListView('', '', '');

                        if (fastui.list.page == 1) {
                            if (container.scrollHeight > container.clientHeight) {
                                fastui.list.layoutResize('align-cells');
                            }
                        }
                    }

                    window.setTimeout(function() {
                        fastui.list.dataLoading(false);
                        }, 500);
                    }
                });
        }
    },


    /*
     * 数据列表滚动加载事件
     */
    scrollLoadEvent: function() {
        var container = $id('datalist-container');

        if (arguments.callee.timer) {
            window.clearTimeout(arguments.callee.timer);
        }

        arguments.callee.timer = window.setTimeout(function() {
            if (fastui.list.wait == false && fastui.list.complete == false) {
                fastui.list.scrollDataLoad(fastui.list.path, false);
            }
            }, 100);

        if (fastui.list.wait == true) {
            fastui.list.wait = false;
            fastui.list.complete = container.clientHeight == container.scrollHeight ? true : fastui.list.complete;
        }

        if (fastui.list.complete == true) {
            container.onscroll = null;
        }
    },


    /*
     * 数据列表滚动分块加载
     */
    scrollLoadBlock: function(field, reverse, primer, index) {
        var container = $id('datalist-container');

        // 滚动加载分块数据
        if (index < jsonData.items.length) {
            container.onscroll = function() {
                window.setTimeout(function() {
                    if (container.clientHeight + container.scrollTop > container.scrollHeight - 192) {
                        dataListView(field, reverse, primer);
                    }
                    }, 0);
                };
        } else {
            // 分块数据加载完毕调用滚动加载事件
            fastui.list.scrollLoadEvent();
        }
    },


    /*
     * 数据列表正在加载提示
     */
    dataLoading: function(display) {
        var container = $id('datalist-container');

        if (container == null) {
            return;
        }

        var icon = $id('data-loading-icon');

        if (icon == null) {
            icon = $create({
                tag: 'div', 
                attr: {id: 'data-loading-icon'}, 
                html: '<img src="/ui/images/loading-icon.gif" width="40" />'
                }).$add(container);
        }

        if (display == true) {
            icon.style.display = 'block';
            icon.style.top = (container.offsetTop + Math.ceil((container.offsetHeight - 40) / 2)) + 'px';
            icon.style.left = (container.offsetLeft + Math.ceil((container.offsetWidth - 40) / 2)) + 'px';
        } else {
            icon.style.display = 'none';
        }
    },


    /*
     * 数据列表没有数据提示
     */
    noData: function(show) {
        var tips = $id('no-data-tips');

        if (tips != null) {
            if (show == true) {
                tips.style.display = 'block';
            } else {
                tips.style.display = 'none';
            }
        }
    },


    /*
     * 数据列表 json 数据合并
     */
    jsonMerge: function(data) {
        window.eval('var json = {items:' + data + '};');

        if (json.items.length == 0) {
            fastui.list.complete = true;
            return;
        }

        for (var i = 0; i < json.items.length; i++) {
            jsonData.items.push(json.items[i]);
        }
    },


    /*
     * 数据列表表格初始化
     */
    dataTable: function(id) {
        var table = $id(id);

        if (table == null) {
            return null;
        }

        // 绑定添加新行方法
        table.addRow = function(index) {
            var row = fastui.list.dataTableAddRow(table, index);

            // 绑定添加新列方法
            row.addCell = function(attr, html) {
                return fastui.list.dataTableAddCell(row, table.rows[index].cells.length, attr, html);
                };

            return row;
            };

        return table;
    },


    /*
     * 数据列表表格添加新行
     */
    dataTableAddRow: function(table, index) {
        return table.insertRow(index);
    },


    /*
     * 数据表格列表添加新列(单元格)
     */
    dataTableAddCell: function(row, index, attr, content) {
        var cell = row.insertCell(index);

        // 设置行列单元格属性
        // attr 参数值为 JSON 数据
        var attr = attr || {};

        for (var i in attr) {
            cell.setAttribute(i, attr[i]);
        }

        // 设置行列单元格内容
        // content 参数值为字符串或处理函数
        if (typeof(content) == 'string') {
            cell.innerHTML = content;
        } else if (typeof(content) == 'function') {
            cell.innerHTML = content();
        }

        return cell;
    },


    /*
     * 数据列表绑定事件
     */
    bindEvent: function(index) {
        var rows = $id('data-list').$tag('tr');
        var checkboxes = $name('id');

        for (var i = index; i < rows.length; i++) {
            // 单击列表行选中项目
            (function(index) {rows[i].onclick = function(e) {
                var event = e || window.event;
                var tag = event.target || event.srcElement;

                if (tag.tagName.toLowerCase() != 'a' && tag.parentNode.tagName.toLowerCase() != 'a' && tag.className.indexOf('button') == -1) {
                    fastui.list.selectRow(rows[index], checkboxes[index]);
                }
                };})(i);
        }
    },


    /*
     * 选择全部列表行
     */
    selectAll: function(select) {
        var rows = $id('data-list').$tag('tr');
        var checkboxes = $name('id');

        for (var i = 0; i < checkboxes.length; i++) {
            // 复选框 disabled 状态禁止选择
            if (checkboxes[i].disabled == false) {
                checkboxes[i].checked = select.checked;

                if (select.checked == true) {
                    rows[i].style.backgroundColor = '#E0E0E0';
                } else {
                    rows[i].style.backgroundColor = '';
                }
            }
        }
    },


    /*
     * 选择单个列表行
     */
    selectRow: function(row, checkbox) {
        // 复选框 disabled 状态禁止选择
        if (checkbox.disabled == true) {
            return;
        }

        if (checkbox.checked == true) {
            checkbox.checked = false;

            row.style.backgroundColor = '';
        } else {
            checkbox.checked = true;

            row.style.backgroundColor = '#E0E0E0';
        }
    },


    /*
     * 数据编辑列表初始化
     */
    editListInit: function() {
        var list = $id('data-list');

        list.onkeydown = function(event) {
            fastui.list.editListArrowKey(event);
            };
    },


    /*
     * 数据编辑列表方向键移动焦点
     */
    editListArrowKey: function(e) {
        var event = e || window.event;
        var list = $id('data-list');
        var inputs = list.$tag('input');
        var count = list.$tag('tr')[0].$tag('input').length;
        var focus = document.activeElement;

        for (var i = 0; i < inputs.length; i++) {
            if (inputs[i] == focus) {
                if (event.keyCode == 37) {
                    if (i - 1 >= 0) {
                        inputs[i - 1].focus();
                    }
                } else if (event.keyCode == 38) {
                    if (i - count >= 0) {
                        inputs[i - count].focus();
                    }
                } else if (event.keyCode == 39) {
                    if (i + 1 < inputs.length) {
                        inputs[i + 1].focus();
                    }
                } else if (event.keyCode == 40) {
                    if (i + count < inputs.length) {
                        inputs[i + count].focus();
                    }
                }
            }
        }
    },


    /*
     * 数据编辑列表输入框建议初始化
     */
    inputSuggest: function(input, url) {
        if (input.onkeyup == null) {
            input.onkeyup = function() {
                if (input.value.length > 0) {
                    fastui.list.inputSuggestQuery(input, url);
                }
                };
        }
    },


    /*
     * 数据编辑列表输入框建议查询
     */
    inputSuggestQuery: function(input, url) {
        url += '?' + input.name + '=' + escape(input.value);

        // 查询结果
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
                        items += '<li onClick="fastui.list.inputSuggestSelect(\'' + input.id + '\', \'' + json[i].value + '\');">' + json[i].text + '</li>';
                    }

                    input.parentNode.style.position = 'relative';
                    input.parentNode.style.zIndex = '1000';

                    var suggest = $class(input.parentNode, 'input-suggest')[0];

                    if (suggest == null) {
                        suggest = $create({
                            tag: 'div', 
                            style: {
                                width: input.parentNode.offsetWidth + 'px'
                                }, 
                            css: 'input-suggest'
                            }).$add(input.parentNode);
                    }

                    suggest.innerHTML = '<ul>' + items + '</ul>';

                    suggest.style.display = 'block';

                    var height = suggest.$tag('li')[0].offsetHeight;
                    var count = 0;

                    if (input.$top() + input.offsetHeight < $page().client.height / 2) {
                        count = parseInt(($page().client.height - (input.$top() + input.offsetHeight)) / height);

                        suggest.style.top = (input.offsetTop + input.offsetHeight) + 'px';
                    } else {
                        count = parseInt(input.$top() / height);

                        suggest.style.top = '-' + (suggest.offsetHeight - input.offsetTop) + 'px';
                    }

                    if (json.length > count) {
                        suggest.style.height = (height * count) + 'px';
                    }

                    suggest.style.left = '0px';

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
     * 数据编辑列表输入框建议选择
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
     * 数据列表自定义鼠标右击弹出菜单
     */
    contextMenu: function(j) {
        var container = $id('datalist-container');

        // 屏蔽浏览器右击菜单
        container.oncontextmenu = function() {
            var selection = window.getSelection ? window.getSelection().toString() : document.selection.createRange().text;

            if (selection.length == 0) {
                return false;
            }
            };

        var rows = $id('data-list').$tag('tr');

        for (var i = j; i < rows.length; i++) {
            // 列表行绑定鼠标右击事件
            (function(index) {rows[i].oncontextmenu = function(e) {
                var event = e || window.event;
                var clientX = event.clientX - container.$left();
                var clientY = event.clientY;
                var selection = window.getSelection ? window.getSelection().toString() : document.selection.createRange().text;

                if (selection.length > 0) {
                    return;
                }
            
                var layer = $id('context-menu');
                var id = rows[index].$tag('input')[0].value;

                layer.innerHTML = layer.innerHTML.replace(/\(\d+\)/g, "(' + id + ')");

                if (container.clientHeight - clientY > layer.offsetHeight) {
                    layer.style.top = (clientY - 16) + 'px';
                } else {
                    layer.style.top = (clientY - layer.offsetHeight + 16) + 'px';
                }

                if (layer.offsetTop < container.offsetTop) {
                    layer.style.top = container.offsetTop + 'px';
                }

                if (container.clientWidth - clientX > layer.offsetWidth) {
                    layer.style.left = (clientX - 16) + 'px';
                } else {
                    layer.style.left = (clientX - layer.offsetWidth + 16) + 'px';
                }

                if (layer.offsetLeft < 0) {
                    layer.style.left = '0px';
                }

                layer.style.display = 'block';

                rows[index].style.backgroundColor = '#E0E0E0';

                (function(idx) {layer.onmouseenter = function() {
                    layer.style.display = 'block';
                    rows[idx].style.backgroundColor = '#E0E0E0';
                    };})(index);

                (function(idx) {layer.onmouseleave = function() {
                    layer.style.display = 'none';
                    rows[idx].style.backgroundColor = '';
                    };})(index);

                (function(idx) {layer.onclick = function() {
                    layer.style.display = 'none';
                    rows[idx].style.backgroundColor = '';
                    };})(index);
                };})(i);
        }
    },


    /*
     * 工具栏按钮菜单弹出层隐藏
     */
    buttonLayerHide: function(element, tag) {
        if (typeof(element) == 'string') {
            element = $id(element);
        }

        if (element == null) {
            return;
        }

        var layer = element.getElementsByTagName(tag)[0];

        if (layer == undefined) {
            return;
        }

        layer.style.display = 'none';
        layer.onclick = '';
    },


    /*
     * 工具栏按钮菜单弹出层显示
     */
    buttonLayerShow: function(element, tag) {
        if (typeof(element) == 'string') {
            element = $id(element);
        }

        if (element == null) {
            return;
        }

        var layer = element.getElementsByTagName(tag)[0];

        if (layer == undefined) {
            return;
        }

        layer.style.display = 'block';

        layer.onclick = function() {fastui.list.buttonLayerHide(element, tag);};
    },


    /*
     * 工具栏过滤按钮弹出层隐藏
     */
    filterLayerHide: function(element, tag) {
        if (typeof(element) == 'string') {
            element = $id(element);
        }

        if (element == null) {
            return;
        }

        var layer = element.getElementsByTagName(tag)[0];

        if (layer == undefined) {
            return;
        }

        layer.style.display = 'none';
        layer.onclick = '';
    },


    /*
     * 工具栏过滤按钮弹出层显示
     */
    filterLayerShow: function(element, tag) {
        if (typeof(element) == 'string') {
            element = $id(element);
        }

        if (element == null) {
            return;
        }

        var layer = element.getElementsByTagName(tag)[0];

        if (layer == undefined) {
            return;
        }

        var header = $id('datalist-header');
        var footer = $id('datalist-footer');
        var container = $id('datalist-container');

        layer.style.display = 'block';
        layer.style.width = container.offsetWidth + 'px';
        layer.style.height = (container.offsetHeight + (header == null ? 0 : header.offsetHeight) + (footer == null ? 0 : footer.offsetHeight)) + 'px';

        layer.onclick = function() {fastui.list.filterLayerHide(element, tag);};
    }


    };
    })();