var uploading = false;


// 正在上传弹出提示(离开页面、刷新页面、关闭浏览器、注销登录)
window.onbeforeunload = function() {
    if (uploading == true) {
        fastui.textTips(lang.file.tips['uploading-not-close']);
        return false;
    }
    };


// 页面初始化事件调用函数
function init() {
    webUploader();

    $id('upload-tips').innerHTML = lang.file.tips['upload-note'].replace(/\[size\]/, window.top.uploadSize);

    fastui.valueTips('remark', lang.file.tips.value['remark']);
}


// 构建WebUploader
function webUploader() {
jQuery(function() {
    if (!WebUploader.Uploader.support()) {
        fastui.coverTips(lang.file.tips['upload-browser-unsupport']);
        return false;
    }

    var $ = jQuery;
    var list = $('#file-list');
    var action = $('#button-action');
    var state = 'pending';
    var size = window.top.uploadSize * 1024 * 1024;
    var uploader = WebUploader.create({
        server: '/web/drive/file/file-upversion.ashx', 
        swf: '/libs/webuploader/webuploader.swf', 
        dnd: document.body, 
        paste: document.body, 
        pick: {
            id: '#button-picker', 
            multiple: false
            }, 
        auto: false, 
        resize: false, 
        chunked: true, 
        chunkSize: 1024 * 1024, 
        threads: 1, 
        fileNumLimit: 1, 
        fileSingleSizeLimit: size == 2147483648 ? size - 1 : size, 
        duplicate: true, 
        fromData: {
            guid: '', 
            fileId: '', 
            remark: ''
            }
        });

    uploader.on('beforeFileQueued', function(file) {
        var item = $('.file-item');
        var extension = $query('extension');
        var size = window.top.uploadSize * 1024 * 1024;

        if (item.length > 0) {
            uploader.removeFile($(item).attr('id'), true);

            $(item).remove();
        }

        if (file.size > (size == 2147483648 ? size - 1 : size)) {
            fastui.textTips(file.name + ' > ' + window.top.uploadSize + ' MB');
            return false;
        }

        if (window.eval('/\\' + extension + '\\w?$/i').test(file.name) == false) {
            fastui.textTips(lang.file.tips['upload-extension-error'] + ' ' + file.ext);
            return false;
        }

        if ($('#upload-tips').length > 0) {
            $('#upload-tips').remove();
        }
        });

    uploader.on('fileQueued', function(file) {
        var html = '';

        html += '<div id=\"' + file.id + '\" class=\"file-item\">';
        html += '<div class=\"name\">' + file.name + '</div>';
        html += '<div class=\"task\">';
        html += '<span class=\"state\">' + lang.file.tips['upload-task-waiting'] + '</span>';
        html += '<span class=\"size\">' + fileSize(file.size) + '</span>';
        html += '</div>';
        html += '<div class=\"progress\"><label class=\"bar\"></label></div>';
        html += '</div>';

        list.append(html);
        });

    uploader.on('uploadStart', function(file) {
        var guid = Math.random();
        var fileId = $query('id');
        var remark = $('#remark').val();

        if (fastui.testString(fileId, /^[1-9]{1}[\d]*$/) == false) {
            uploader.cancelFile(file);
            return false;
        }

        if (fastui.testInput(false, 'remark', /^[\s\S]{1,100}$/) == false) {
            uploader.cancelFile(file);
            return false;
        }

        uploader.options.formData.guid = guid;
        uploader.options.formData.fileId = fileId;
        uploader.options.formData.remark = remark;
        });

    uploader.on('uploadProgress', function(file, percentage) {
        var item = $('#' + file.id);
        var percent = (percentage * 100) > 100 ? 100 : parseInt(percentage * 100);

        item.css('background', '#BBDEFB');
        item.find('.state').text(lang.file.tips['upload-task-uploading'] + percent + '%');
        item.find('.bar').css('width', percent + '%');
        });

    uploader.on('uploadError', function(file, reason) {
        var item = $('#' + file.id);

        item.css('background', '#FFCDD2');
        item.find('.state').text(lang.file.tips['upload-task-error'] + reason);
        });

    uploader.on('uploadSuccess', function(file) {
        var item = $('#' + file.id);

        item.css('background', '#C8E6C9');
        item.find('.state').text(lang.file.tips['upload-task-success'] + '100%');
        });

    uploader.on('uploadComplete', function(file) {
        $('#' + file.id).find('.progress').fadeOut();
        });

    uploader.on('uploadFinished', function() {
        if (uploader.getStats().uploadFailNum == 0) {
            uploading = false;

            var iframe = window.parent.$id('file-version-iframe');

            if (iframe == null) {
                window.parent.fastui.iconTips('tick');
            } else {
                iframe.contentWindow.dataListActionCallback('complete');
            }

            window.setTimeout(function() {
                window.parent.fastui.windowClose('file-upversion');
                }, 500);
        }
        });

    uploader.on('all', function(type) {
        if (type === 'startUpload') {
            state = 'uploading';
        } else if (type === 'stopUpload') {
            state = 'paused';
        } else if (type === 'uploadFinished') {
            state = 'done';
        }

        if (state === 'uploading') {
            action.text(lang.file.button['upload-pause']);
        } else if (state === 'paused') {
            action.text(lang.file.button['upload-continue']);
        } else if (state === 'done') {
            action.text(lang.file.button['upload-start']);
        }
        });

    action.on('click', function() {
        if (uploader.getFiles().length == 0) {
            fastui.textTips(lang.file.tips['please-select-file']);
            return false;
        }

        if (state === 'uploading') {
            uploader.stop(true);
        } else if (state === 'paused') {
            uploader.upload();
        } else if (state === 'pending') {
            var fileId = $query('id');
            var remark = $('#remark').val();

            if (fastui.testString(fileId, /^[1-9]{1}[\d]*$/) == false) {
                fastui.textTips(lang.input.tips.input['id']);
                return false;
            }

            if (fastui.testInput(false, 'remark', /^[\s\S]{1,100}$/) == false) {
                fastui.inputTips('remark', lang.file.tips.input['remark']);
                return false;
            }

            uploading = true;

            uploader.upload();
        }
        });
    });
}


// 获取文件大小
function fileSize(byte) {
    if (Math.ceil(byte / 1024) < 1024) {
        return Math.ceil(byte / 1024) + ' KB';
    } else if (Math.ceil(byte / 1024 / 1024) < 1024) {
        return (byte / 1024 / 1024).toFixed(2) + ' MB';
    }
}