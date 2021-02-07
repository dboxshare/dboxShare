lang.object = {
    'folder' : '文件夾', 
    'file' : '文件', 
    'department' : '部門', 
    'role' : '角色', 
    'user' : '用戶', 
    'discuss' : '討論', 
    'task' : '任務', 
    'log' : '日志'
    };


lang.department = {
    'name' : '名稱', 
    'order' : '排序', 
    'all' : '全部', 

    'button' : {
        'add' : '新建部門', 
        'modify' : '修改', 
        'delete' : '刪除', 
        'move-up' : '上移', 
        'move-down' : '下移'
        }, 

    'tips' : {
        'confirm' : {
            'delete' : '確定刪除嗎？'
            }, 
        'value' : {
            'name' : '不能包含符號\<br \/\>長度 2~24 個字符\<br \/\>多個名稱使用回車換行分隔\<br \/\>每次限制 50 行'
            }, 
        'input' : {
            'id' : 'Id 錯誤或非法操作', 
            'name' : '名稱空值或格式錯誤'
            }, 
        'name-existed' : '該名稱已經存在', 
        'unexist-or-error' : '數據不存在或錯誤', 
        'operation-failed' : '操作失敗'
        }
    };


lang.role = {
    'name' : '名稱', 
    'order' : '排序', 

    'button' : {
        'add' : '新建角色', 
        'modify' : '修改', 
        'delete' : '刪除', 
        'move-up' : '上移', 
        'move-down' : '下移'
        }, 

    'tips' : {
        'confirm' : {
            'delete' : '確定刪除嗎？'
            }, 
        'value' : {
            'name' : '不能包含符號\<br \/\>長度 2~24 個字符\<br \/\>多個名稱使用回車換行分隔\<br \/\>每次限制 50 行'
            }, 
        'input' : {
            'id' : 'Id 錯誤或非法操作', 
            'name' : '名稱空值或格式錯誤'
            }, 
        'name-existed' : '該名稱已經存在', 
        'unexist-or-error' : '數據不存在或錯誤', 
        'operation-failed' : '操作失敗'
        }
    };


lang.user = {
    'department' : '部門', 
    'role' : '角色', 
    'username' : '用戶賬號', 
    'password' : '登錄密碼', 
    'password-confirm' : '確認密碼', 
    'code' : '員工編號', 
    'title' : '職稱', 
    'email' : '電子郵箱', 
    'phone' : '手機號碼', 
    'tel' : '座機號碼', 
    'admin' : '系統管理員', 
    'status' : '狀態', 
    'freeze-account' : '凍結賬號 / 離職', 
    'create-account-email' : '發送賬號密碼通知郵件', 
    'vericode' : '驗證碼', 
    'ad-username' : 'AD 域用戶賬號', 
    'all' : '全部', 
    'unlimited' : '不限', 
    'user-card' : '用戶名片', 
    'not-logged-in' : '暫未登錄', 
    'last-login' : '最近登錄', 

    'button' : {
        'filter' : '篩選', 
        'add' : '新建用戶', 
        'create' : '批量創建', 
        'modify' : '修改', 
        'classify' : '歸入', 
        'classify-department' : '歸入部門', 
        'classify-role' : '歸入角色', 
        'transfer' : '移交', 
        'remove' : '移除', 
        'restore' : '還原', 
        'delete' : '徹底刪除', 
        'get-vericode' : '獲取驗證碼', 
        'reset-password' : '重設登錄密碼', 
        'bind-account' : '綁定賬號', 
        'create-account' : '創建賬號'
        }, 

    'tips' : {
        'confirm' : {
            'remove' : '確定移除嗎？', 
            'remove-all' : '確定移除選中的項目嗎？', 
            'restore' : '確定還原嗎？', 
            'restore-all' : '確定還原選中的項目嗎？', 
            'delete' : '確定徹底刪除嗎？', 
            'delete-all' : '確定徹底刪除選中的項目嗎？', 
            'transfer' : '確定移交用戶數據嗎？'
            }, 
        'value' : {
            'keyword' : '用戶賬號、郵箱、手機', 
            'username' : '不能包含符號', 
            'password' : '建議使用組合字符', 
            'password-add' : '空值表示隨機密碼', 
            'password-modify' : '空值表示不修改密碼', 
            'password-confirm' : '重復一次登錄密碼', 
            'code' : '英文數字組合', 
            'title' : '不能包含符號', 
            'email' : 'username@mail.com', 
            'phone' : '數字和“-”符號組合', 
            'tel' : '數字和“-”符號組合', 
            'vericode' : '6 位數字', 
            'bind-username' : '請輸入綁定用戶賬號', 
            'bind-password' : '請輸入綁定用戶密碼'
            }, 
        'side' : {
            'username' : '2~16 個英文、數字或中文組合', 
            'password' : '6~16 個英文、數字或符號組合', 
            'vericode' : '驗證碼已發送到您的電子郵箱'
            }, 
        'input' : {
            'id' : 'Id 錯誤或非法操作', 
            'username' : '用戶賬號空值或包含非法符號', 
            'username-pure-number-error' : '用戶賬號不能使用純數字', 
            'password' : '登錄密碼空值或包含非法符號', 
            'password-confirm' : '確認密碼空值或包含非法符號', 
            'password-confirm-error' : '登錄密碼與確認密碼不一致', 
            'password-and-email-empty' : '登錄密碼和電子郵箱必須填寫其中一項', 
            'code' : '員工編號空值或包含非法符號', 
            'title' : '職稱空值或格式錯誤', 
            'email' : '電子郵箱空值或格式錯誤', 
            'phone' : '手機號碼空值或格式錯誤', 
            'tel' : '座機號碼空值或格式錯誤'
            }, 
        'username-existed' : '該用戶賬號已經存在', 
        'email-existed' : '該電子郵箱已經存在', 
        'phone-existed' : '該手機號碼已經存在', 
        'please-select-item' : '請選擇項目', 
        'please-select-user' : '請選擇用戶', 
        'username-email-error' : '用戶賬號或電子郵箱錯誤', 
        'forgot-lock-ip' : '您已被鎖定 20 分鐘內禁止重新操作', 
        'user-unexist' : '用戶不存在', 
        'vericode-error' : '驗證碼錯誤', 
        'reset-success' : '登錄密碼重置成功', 
        'bind-login-failed' : '該用戶不存在或登錄密碼錯誤', 
        'bind-success' : '用戶綁定成功', 
        'create-success' : '用戶創建成功', 
        'unexist-or-error' : '數據不存在或錯誤', 
        'operation-failed' : '操作失敗', 
        'transfer-note' : '溫馨提示：僅移交文件數據，不包含任務、日志等數據；', 
        'create-account-note' : '我沒有本站用戶賬號？請點擊<a href="javascript: userCreate();">創建用戶賬號</a>'
        }, 

    'data' : {
        'status' : {
            'job-on' : '在職', 
            'job-off' : '離職'
            }
        }
    };


lang.file = {
    'name' : '名稱', 
    'version' : '版本', 
    'type' : '類型', 
    'size' : '大小', 
    'space-usage' : '占用', 
    'contain' : '包含', 
    'contain-folder' : '個文件夾', 
    'contain-file' : '個文件', 
    'path' : '路徑', 
    'remark' : '備注', 
    'share' : '共享', 
    'lock' : '鎖定', 
    'sync' : '與父級目錄同步', 
    'owner' : '所有者', 
    'create' : '創建', 
    'update' : '更新', 
    'time' : '時間', 
    'folder-inherit' : '繼承父級目錄權限', 
    'folder-share' : '共享文件夾', 
    'create-time' : '創建時間', 
    'update-time' : '更新時間', 
    'remove-time' : '移除時間', 
    'upload-time' : '上傳時間', 
    'all' : '全部', 
    'unlimited' : '不限', 
    'exit-query' : '退出搜索', 
    'only-look-me' : '只看我的', 
    'no-remark' : '暫無備注', 
    'event-create' : '創建于', 
    'event-update' : '更新于', 
    'event-remove' : '移除于', 
    'event-upload' : '上傳于', 

    'button' : {
        'filter' : '篩選', 
        'new' : '新建', 
        'add' : '新建文件夾', 
        'modify' : '修改', 
        'upload' : '上傳', 
        'upversion' : '上傳新版本', 
        'download' : '下載', 
        'move' : '移動', 
        'remove' : '移除', 
        'restore' : '還原', 
        'delete' : '徹底刪除', 
        'confirm' : '確定', 
        'cancel' : '取消', 
        'select' : '選擇', 
        'unpack' : '解壓', 
        'open-share' : '開啟共享', 
        'department-add' : '+ 部門', 
        'role-add' : '+ 角色', 
        'user-add' : '+ 用戶', 
        'purview-change' : '更改子級目錄', 
        'upload-picker' : '選擇文件', 
        'upload-start' : '開始上傳', 
        'upload-pause' : '暫停上傳', 
        'upload-continue' : '繼續上傳', 
        'upload-renew' : '上傳新版本', 
        'view-previous' : '上一個', 
        'view-next' : '下一個'
        }, 

    'tips' : {
        'confirm' : {
            'lock' : '確定鎖定嗎？', 
            'unlock' : '確定解除鎖定嗎？', 
            'replace' : '確定替換嗎？', 
            'move' : '確定移動嗎？', 
            'move-all' : '確定移動選中的項目嗎？', 
            'remove' : '確定移除嗎？', 
            'remove-all' : '確定移除選中的項目嗎？', 
            'restore' : '確定還原嗎？', 
            'restore-all' : '確定還原選中的項目嗎？', 
            'delete' : '確定徹底刪除嗎？', 
            'delete-all' : '確定徹底刪除選中的項目嗎？'
            }, 
        'value' : {
            'keyword' : '請輸入文件關鍵詞', 
            'name' : '50 個字符以內', 
            'remark' : '100 個字符以內', 
            'new-folder' : '新建文件夾', 
            'unzip-key' : '請輸入解壓密碼'
            }, 
        'input' : {
            'id' : 'Id 錯誤或非法操作', 
            'name' : '名稱空值或包含非法符號', 
            'remark' : '備注空值或長度超出限制', 
            'unzip-key' : '請輸入解壓密碼'
            }, 
        'query-folder' : '在當前文件夾查找', 
        'name-existed' : '該名稱已經存在', 
        'current-folder-removed' : '當前文件夾已經移除', 
        'current-file-removed' : '當前文件已經移除', 
        'please-select-item' : '請選擇項目', 
        'please-select-folder' : '請選擇文件夾', 
        'please-select-file' : '請選擇文件', 
        'no-permission' : '您沒有權限執行該操作', 
        'unallow-select-item' : '不允許選擇此項目', 
        'not-allow-delete' : '不允許徹底刪除', 
        'upload-task-waiting' : '等待上傳...', 
        'upload-task-uploading' : '正在上傳...', 
        'upload-task-error' : '上傳錯誤', 
        'upload-task-success' : '上傳成功', 
        'upload-extension-error' : '不允許該擴展名', 
        'upload-note' : '溫馨提示：<br />支持拖放文件夾、文件到此窗口添加列表；<br />文件大小不能大于 \{size\} MB；', 
        'upload-browser-unsupport' : '您的瀏覽器不支持 HTML5 和 Flash', 
        'uploading-not-close' : '正在上傳請勿刷新或關閉頁面', 
        'download-waiting' : '正在等待下載...', 
        'download-status-packing' : '正在打包...', 
        'download-status-start' : '開始下載...3 秒后關閉窗口', 
        'unpack-status-unpacking' : '正在解壓...', 
        'unpack-status-complete' : '解壓完成...3 秒后關閉窗口', 
        'unpack-complete' : '解壓已完成！<a href="javascript: unpackComplete_GotoView();">點擊查看</a>', 
        'zip-file-error' : '壓縮文件錯誤或異常', 
        'copy-complete' : '復制已完成！<a href="javascript: listGoto(\{folderid\});">點擊查看</a>', 
        'move-complete' : '移動已完成！<a href="javascript: listGoto(\{folderid\});">點擊查看</a>', 
        'purview-add-ok' : '添加成功', 
        'purview-modify-ok' : '修改成功', 
        'purview-share-ok' : '設置共享成功', 
        'purview-unshare-ok' : '取消共享成功', 
        'purview-sync-ok' : '設置同步成功', 
        'purview-unsync-ok' : '取消同步成功', 
        'purview-change-ok' : '更改成功', 
        'view-waiting-convert' : '文件正在等待轉換暫時無法預覽', 
        'view-reach-first' : '到達最前一個', 
        'view-reach-last' : '到達最后一個', 
        'view-browser-unsupport' : '在線預覽必須使用支持 HTML5 的新式瀏覽器', 
        'unexist-or-error' : '數據不存在或錯誤', 
        'operation-failed' : '操作失敗', 
        'recycle-note' : '溫馨提示：移除少于 30 天的共享項目不允許徹底刪除；'
        }, 

    'context' : {
        'download' : '下載', 
        'upversion' : '上傳新版本', 
        'replace' : '替換該版本', 
        'lock' : '鎖定', 
        'unlock' : '解除鎖定', 
        'rename' : '重命名', 
        'remark' : '備注', 
        'modify' : '修改', 
        'purview' : '共享權限', 
        'copy' : '復制到...', 
        'move' : '移動到...', 
        'remove' : '移除到回收站', 
        'restore' : '還原', 
        'delete' : '徹底刪除', 
        'task' : '任務分派', 
        'discuss' : '討論', 
        'version' : '歷史版本', 
        'log' : '操作日志', 
        'detail' : '詳細屬性'
        }, 

    'data' : {
        'role' : {
            'viewer' : '查看者', 
            'downloader' : '下載者', 
            'uploader' : '上傳者', 
            'editor' : '編輯者', 
            'manager' : '管理者'
            }, 

        'status' : {
            'yes' : '是', 
            'no' : '否'
            }
        }
    };


lang.discuss = {
    'menu' : {
        'all' : '全部', 
        'created' : '我創建的', 
        'shared' : '與我共享的'
    }, 

    'button' : {
        'post' : '發表', 
        'revoke' : '撤回', 
        'enter' : '進入討論'
        }, 

    'tips' : {
        'confirm' : {
            'revoke' : '確定撤回嗎？'
            }, 
        'value' : {
            'content' : '200 個字符以內'
        }, 
        'input' : {
            'content' : '內容空值或長度超出限制'
            }, 
        'message-revoked' : '該消息已被撤回', 
        'operation-failed' : '操作失敗'
        }
    };


lang.task = {
    'file' : '文件夾 / 文件', 
    'user' : '分派人', 
    'member' : '參與成員', 
    'content' : '內容', 
    'level' : '級別', 
    'deadline' : '期限', 
    'left-time' : '剩余時間', 
    'time' : '發布時間', 
    'status' : '狀態', 
    'cause' : '撤消原因', 
    'reason' : '拒絕原因', 
    'remark' : '完成備注', 

    'menu' : {
        'inbox' : '收件箱', 
        'outbox' : '發件箱', 
        'all' : '全部', 
        'unprocessed' : '未處理', 
        'accepted' : '已接受', 
        'rejected' : '已拒絕', 
        'completed' : '已完成', 
        'expired' : '已過期', 
        'revoked' : '已撤消', 
        'processing' : '進行中'
    }, 

    'button' : {
        'assign' : '任務分派', 
        'select' : '選擇', 
        'revoke' : '撤消任務', 
        'pending' : '等待處理', 
        'accept' : '接受任務', 
        'reject' : '拒絕任務', 
        'completed' : '完成任務', 
        'processing' : '正在進行', 
        'detail' : '任務詳情'
        }, 

    'tips' : {
        'confirm' : {
            'revoke' : '我確定撤消該任務', 
            'accept' : '我已閱讀詳細任務內容并確定接受', 
            'reject' : '我已閱讀詳細任務內容并確定拒絕', 
            'completed' : '我已審查任務并確定完成'
            }, 
        'value' : {
            'content' : '500 個字符以內', 
            'cause' : '請輸入撤消原因(必填)<br />200 個字符以內', 
            'reason' : '請輸入拒絕原因(必填)<br />200 個字符以內', 
            'remark' : '請輸入完成備注(可選)<br />200 個字符以內'
        }, 
        'input' : {
            'id' : 'Id 錯誤或非法操作', 
            'content' : '內容空值或長度超出限制', 
            'cause' : '撤消原因空值或長度超出限制', 
            'reason' : '拒絕原因空值或長度超出限制', 
            'remark' : '完成備注空值或長度超出限制'
            }, 
        'please-select-user' : '請選擇用戶', 
        'deadline-time-error' : '期限不能早于當前時間', 
        'unexist-or-error' : '數據不存在或錯誤', 
        'operation-failed' : '操作失敗'
        }, 

    'data' : {
        'level' : {
            'normal' : '常規', 
            'important' : '重要', 
            'urgent' : '緊急'
            }, 

        'status' : {
            'unprocessed' : '未處理', 
            'accepted' : '已接受', 
            'rejected' : '已拒絕', 
            'completed' : '已完成', 
            'expired' : '已過期', 
            'revoked' : '已撤消'
            }, 

        'deadline' : {
            'week' : {
                'monday' : '星期一', 
                'tuesday' : '星期二', 
                'wednesday' : '星期三', 
                'thursday' : '星期四', 
                'friday' : '星期五', 
                'saturday' : '星期六', 
                'sunday' : '星期日'
                }, 

            'time' : {
                'day' : '天', 
                'hour' : '時', 
                'minute' : '分', 
                'second' : '秒'
                }
            }
        }
    };


lang.log = {
    'folder' : '文件夾', 
    'file' : '文件', 
    'version' : '版本', 
    'user' : '用戶', 
    'action' : '操作', 
    'time' : '時間', 
    'unlimited' : '不限', 
    'time-start' : '開始', 
    'time-end' : '結束', 
    
    'menu' : {
        'all' : '全部', 
        'created' : '我創建的', 
        'shared' : '與我共享的'
    }, 

    'button' : {
        'filter' : '篩選', 
        'query' : '查詢'
        }, 

    'tips' : {
        'value' : {
            'keyword' : '用戶賬號、操作數據'
        }, 
        'input' : {
            'time-start' : '開始時間空值或格式錯誤', 
            'time-end' : '結束時間空值或格式錯誤'
            }
        }
    };


lang.login = {
    'login-id' : '用戶賬號 / 郵箱 / 手機', 
    'password' : '登錄密碼', 
    'forgot-password' : '忘記密碼', 
    'ad-user-bind' : 'AD 域用戶賬號綁定', 

    'button' : {
        'login' : '登錄'
        }, 

    'tips' : {
        'input' : {
            'login-id' : '用戶賬號 / 郵箱 / 手機空值或格式錯誤', 
            'password' : '登錄密碼空值或格式錯誤'
            }, 
        'login-failure' : '該用戶不存在或登錄密碼錯誤', 
        'login-lock-ip' : '您已被鎖定 20 分鐘內禁止登錄操作', 
        'logged-in' : '該用戶已登錄！請關閉瀏覽器重試', 
        'operation-failed' : '操作失敗', 
        'html5-browser-note' : '當前瀏覽器太老舊了，請您使用支持 HTML5 的現代瀏覽器。'
        }
    };


lang.main = {
    'tab' : {
        'explorer' : '文件管理', 
        'task' : '任務', 
        'discuss' : '討論', 
        'activity' : '動態', 
        'recycle' : '回收站', 
        'admin' : '管理面板', 
        'refresh' : '刷新'
        }, 

    'tool' : {
        'sync' : '同步工具', 
        'help' : '使用幫助', 
        'about' : '關于', 
        'profile' : '賬號設置', 
        'logout' : '退出'
        }, 

    'about' : {
        'version' : '當前版本', 
        'license' : '授權協議', 
        'website' : '官方網站'
        }
    };


lang.admin = {
    'menu' : {
        'statistic' : '統計信息', 
        'department' : '部門結構', 
        'role' : '角色分組', 
        'user' : '用戶管理', 
        'user-list' : '用戶列表', 
        'user-recycle' : '用戶回收站', 
        'log' : '日志記錄', 
        'config' : '系統配置'
        }, 

    'statistic' : {
        'user' : '用戶', 
        'folder' : '文件夾', 
        'file' : '文件', 
        'occupy' : '占用空間', 
        'today' : '今天', 
        'yesterday' : '昨天', 
        'week' : '本周', 
        'month' : '本月', 
        'upload' : '上傳', 
        'download' : '下載', 

        tips : {
            'data-exception' : '數據異常'
            }
        }, 

    'config' : {
        'app-name' : '應用名稱', 
        'upload-extension' : '上傳擴展限制', 
        'upload-size' : '上傳大小限制', 
        'version-count' : '版本數量限制', 
        'mail-address' : '服務郵箱地址', 
        'mail-username' : '郵箱登錄賬號', 
        'mail-password' : '郵箱登錄密碼', 
        'mail-server' : '郵件服務器地址', 
        'mail-server-port' : '郵件服務器端口', 
        'mail-server-security' : '郵件服務器安全', 
        'mail-server-ssl-connection' : '啟用 SSL 連接', 

        'button' : {
            'confirm' : '確定'
            }, 

        'tips' : {
            'value' : {
                'upload-extension' : '允許上傳的文件格式\<br \/\>擴展名不帶“\.”符號\<br \/\>多個擴展名使用回車換行分隔'
                }, 
            'input' : {
                'app-name' : '應用名稱空值或格式錯誤', 
                'upload-extension' : '上傳擴展限制空值或格式錯誤', 
                'upload-size' : '上傳大小限制空值或格式錯誤', 
                'upload-size-range' : '上傳大小限制數值范圍 1~2048', 
                'version-count' : '版本數量限制空值或格式錯誤', 
                'version-count-range' : '版本數量限制數值范圍 50~500', 
                'mail-address' : '服務郵箱地址空值或格式錯誤', 
                'mail-username' : '郵箱登錄賬號空值或格式錯誤', 
                'mail-password' : '郵箱登錄密碼空值或格式錯誤', 
                'mail-server' : '郵件服務器地址空值或格式錯誤', 
                'mail-server-port' : '郵件服務器端口空值或格式錯誤'
                }, 
            're-login' : '3 秒后重新登錄', 
            'config-read-failed' : '配置讀取失敗', 
            'operation-failed' : '操作失敗'
            }
        }
    };


lang.ui = {
    'button' : {
        'confirm' : '確定', 
        'cancel' : '取消'
        }
    };


lang.list = {
    'no-data' : '暫無數據', 
    'data-count' : '已加載 [count] 個項目'
    };


lang.type = {
    'folder' : '文件夾', 
    'word' : 'Word', 
    'excel' : 'Excel', 
    'powerpoint' : 'PowerPoint', 
    'project' : 'Project', 
    'publisher' : 'Publisher', 
    'visio' : 'Visio', 
    'openoffice' : 'OpenOffice', 
    'wps' : 'WPS', 
    'pdf' : 'PDF', 
    'text' : '文本', 
    'code' : '代碼', 
    'image' : '圖片', 
    'drawing' : '圖紙', 
    'audio' : '音頻', 
    'video' : '視頻', 
    'zip' : '壓縮', 
    'other' : '其它'
    };


lang.action = {
    'upload' : '上傳', 
    'download' : '下載', 
    'add' : '添加', 
    'modify' : '修改', 
    'edit' : '編輯', 
    'view' : '查看', 
    'rename' : '重命名', 
    'remark' : '備注', 
    'copy' : '復制', 
    'move' : '移動', 
    'remove' : '移除', 
    'restore' : '還原', 
    'delete' : '刪除', 
    'upversion' : '上傳新版本', 
    'replace' : '替換版本', 
    'lock' : '鎖定', 
    'unlock' : '解除鎖定', 
    'share' : '設置共享', 
    'unshare' : '取消共享', 
    'sync' : '設置同步', 
    'unsync' : '取消同步'
    };


lang.size = {
    '0kb-100kb': '0KB~100KB', 
    '100kb-500kb': '100~500KB', 
    '500kb-1mb': '500KB~1MB', 
    '1mb-5mb': '1MB~5MB', 
    '5mb-10mb': '5MB~10MB', 
    '10mb-50mb': '10MB~50MB', 
    '50mb-100mb': '50MB~100MB', 
    '100mb-more': '100MB 或以上'
    };


lang.timestamp = {
    'this-day' : '今天', 
    '1-day-ago' : '昨天', 
    '2-day-ago' : '前天', 
    'this-week' : '本周', 
    '1-week-ago' : '一周前', 
    '2-week-ago' : '兩周前', 
    'this-month' : '本月', 
    '1-month-ago' : '一月前', 
    '2-month-ago' : '兩月前', 
    'this-year' : '本年', 
    '1-year-ago' : '一年前', 
    '2-year-ago' : '兩年前'
    };


lang.timespan = {
    'second' : '秒前', 
    'minute' : '分鐘前', 
    'hour' : '小時前', 
    'day' : '天前', 
    'month' : '月前', 
    'year' : '年前'
    };