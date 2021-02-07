var uploading = false;


// 正在上传弹出提示(离开页面、刷新页面、关闭浏览器、注销登录)
window.onbeforeunload = window.parent.onbeforeunload = function() {
    if (uploading == true) {
        window.parent.fastui.textTips(lang.file.tips['uploading-not-close']);
        return false;
    }
    };


// 页面初始化事件调用函数
function init() {
    webUploader();

    $id('upload-tips').innerHTML = lang.file.tips['upload-note'].replace(/\{size\}/, window.top.uploadSize);
}


// 构建 WebUploader
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
        server: '/api/drive/file/upload', 
        swf: '/libs/webuploader/webuploader.swf', 
        dnd: document.body, 
        paste: document.body, 
        pick: {
            id: '#button-picker', 
            multiple: true
            }, 
        auto: false, 
        resize: false, 
        chunked: true, 
        chunkSize: 1024 * 1024, 
        threads: 1, 
        fileNumLimit: 1024, 
        fileSingleSizeLimit: size == 2147483648 ? size - 1 : size, 
        duplicate: true, 
        fromData: {
            guid: '', 
            folderId: ''
            }
        });

    uploader.on('beforeFileQueued', function(file) {
        var size = window.top.uploadSize * 1024 * 1024;

        if (file.size > (size == 2147483648 ? size - 1 : size)) {
            fastui.textTips(file.name + ' > ' + window.top.uploadSize + ' MB');
            return false;
        }

        if (window.top.uploadExtension.length > 0) {
            if (window.top.uploadExtension.indexOf(file.ext) == -1) {
                fastui.textTips(lang.file.tips['upload-extension-error'] + ' ' + file.ext);
                return false;
            }
        }

        if ($('#upload-tips').length > 0) {
            $('#upload-tips').remove();
        }
        });

    uploader.on('fileQueued', function(file) {
        var folderId = $('#folderid').val();
        var folderName = $('#foldername').val();
        var html = '';

        html += '<div id="' + file.id + '" class="file-item">';
        html += '<div class="name">' + file.name + '</div>';
        html += '<div class="task">';
        html += '<span class="state">' + lang.file.tips['upload-task-waiting'] + '</span>';
        html += '<span class="size">' + fileSize(file.size) + '</span>';
        html += '<span class="folder">/' + folderName + '</span>';
        html += '</div>';
        html += '<div class="progress"><label class="bar"></label></div>';
        html += '<span class="remove">✕</span>';
        html += '<input type="hidden" value="' + folderId + '" />';
        html += '</div>';

        list.append(html);
        });

    uploader.on('uploadStart', function(file) {
        var guid = newGuid();
        var folderId = $('#' + file.id).find('input').val();

        if (fastui.testString(folderId, /^[1-9]{1}[\d]*$/) == false) {
            folderId = 0;
        }

        uploader.options.formData.guid = guid;
        uploader.options.formData.folderId = folderId;
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
        item.find('.state').text(lang.file.tips['upload-task-error'] + ' ' + reason);
        });

    uploader.on('uploadSuccess', function(file) {
        var item = $('#' + file.id);

        item.css('background', '#C8E6C9');
        item.find('.state').text(lang.file.tips['upload-task-success'] + '100%');

        window.setTimeout(function() {
            $(item).fadeOut(function() {
                $(item).remove();
                });
            }, 500);
        });

    uploader.on('uploadComplete', function(file) {
        $('#' + file.id).find('.progress').fadeOut();
        });

    uploader.on('uploadFinished', function() {
        if (uploader.getStats().uploadFailNum == 0) {
            uploading = false;

            window.parent.uploadCompleteRefresh();
        }
        });

    uploader.on('all', function(type) {
        if (type === 'startUpload') {
            state = 'uploading';
        } else if (type === 'stopUpload') {
            state = 'paused';
        } else if (type === 'uploadFinished') {
            state = 'pending';
        }

        if (state === 'uploading') {
            action.text(lang.file.button['upload-pause']);
        } else if (state === 'paused') {
            action.text(lang.file.button['upload-continue']);
        } else if (state === 'pending') {
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
            uploading = true;

            uploader.upload();
        }
        });

    list.on('click', '.remove', function() {
        var item = $(this).parent();

        uploader.removeFile($(item).attr('id'), true);

        $(item).remove();
        });
    });
}


// 生成 Guid
function newGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(char) {
        var random = Math.random() * 16 | 0;
        var value = char == 'x' ? random : (random & 0x3 | 0x8);

        return value.toString(16);
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