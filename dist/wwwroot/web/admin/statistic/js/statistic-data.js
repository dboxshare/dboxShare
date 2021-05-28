// 页面初始化事件调用函数
function init() {
    basisStatistic();
    uploadStatistic();
    downloadStatistic();
}


// 基本统计数据加载
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

                $id('user-total').innerHTML = toThousand(jsonData.user_total);
                $id('folder-total').innerHTML = toThousand(jsonData.folder_total);
                $id('file-total').innerHTML = toThousand(jsonData.file_total);
                $id('occupy-total').innerHTML = toThousand(fileSize(jsonData.occupy_total));
            }
            }
        });
}


// 上传统计数据加载
function uploadStatistic() {
    $ajax({
        type: 'GET', 
        url: '/api/admin/statistic/upload-data-json', 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.admin.statistic.tips['data-exception']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('today-upload-total').innerHTML = jsonData.today_upload_total;
                $id('yesterday-upload-total').innerHTML = jsonData.yesterday_upload_total;
                $id('week-upload-total').innerHTML = jsonData.week_upload_total;
                $id('month-upload-total').innerHTML = jsonData.month_upload_total;
            }
            }
        });
}


// 下载统计数据加载
function downloadStatistic() {
    $ajax({
        type: 'GET', 
        url: '/api/admin/statistic/download-data-json', 
        async: true, 
        callback: function(data) {
            if ($jsonString(data) == false) {
                fastui.coverTips(lang.admin.statistic.tips['data-exception']);
            } else {
                window.eval('var jsonData = ' + data + ';');

                $id('today-download-total').innerHTML = jsonData.today_download_total;
                $id('yesterday-download-total').innerHTML = jsonData.yesterday_download_total;
                $id('week-download-total').innerHTML = jsonData.week_download_total;
                $id('month-download-total').innerHTML = jsonData.month_download_total;
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
    } else {
        return (byte / 1024 / 1024 / 1024 / 1024).toFixed(2) + ' TB';
    }
}


// 获取千分数格式数值
function toThousand(number) {
    return number.replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
}