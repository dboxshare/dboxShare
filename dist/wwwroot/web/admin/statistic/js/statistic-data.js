// 页面初始化事件调用函数
function init() {
    basisStatistic();
    timeStatistic();
}


// 基本统计数据载入
function basisStatistic() {
    $ajax({
        type: 'GET', 
        url: '/api/admin/statistic/basis-data-json', 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.admin.statistic.tips['data-exception']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('user-count').innerHTML = toThousand(jsonData.user_count);
                $id('folder-count').innerHTML = toThousand(jsonData.folder_count);
                $id('file-count').innerHTML = toThousand(jsonData.file_count);
                $id('occupy-space').innerHTML = toThousand(fileSize(jsonData.occupy_space));
            }
            }
        });
}


// 时间统计数据载入
function timeStatistic() {
    $ajax({
        type: 'GET', 
        url: '/api/admin/statistic/time-data-json', 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.admin.statistic.tips['data-exception']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('today-upload').innerHTML = jsonData.today_upload;
                $id('today-download').innerHTML = jsonData.today_download;
                $id('yesterday-upload').innerHTML = jsonData.yesterday_upload;
                $id('yesterday-download').innerHTML = jsonData.yesterday_download;
                $id('week-upload').innerHTML = jsonData.week_upload;
                $id('week-download').innerHTML = jsonData.week_download;
                $id('month-upload').innerHTML = jsonData.month_upload;
                $id('month-download').innerHTML = jsonData.month_download;
            }
            }
        });
}


// 获取文件大小
function fileSize(byte) {
    if (Math.ceil(byte / 1024) < 1024) {
        return Math.ceil(byte / 1024) + ' KB';
    } else if (Math.ceil(byte / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024).toFixed(2) + ' MB';
    } else if (Math.ceil(byte / 1024 / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024 / 1024).toFixed(2) + ' GB';
    } else if (Math.ceil(byte / 1024 / 1024 / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024 / 1024 / 1024).toFixed(2) + ' TB';
    }
}


// 获取千分数格式数值
function toThousand(number) {
    return number.replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
}