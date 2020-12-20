lang.object = {
    'folder' : '文件夹', 
    'file' : '文件', 
    'department' : '部门', 
    'role' : '角色', 
    'user' : '用户', 
    'discuss' : '讨论', 
    'task' : '任务', 
    'log' : '日志'
    };


lang.department = {
    'name' : '名称', 
    'order' : '排序', 
    'all' : '全部', 

    'button' : {
        'add' : '新建部门', 
        'modify' : '修改', 
        'delete' : '删除', 
        'move-up' : '上移', 
        'move-down' : '下移'
        }, 

    'tips' : {
        'confirm' : {
            'delete' : '确定删除吗？'
            }, 
        'value' : {
            'name' : '不能包含符号'
            }, 
        'input' : {
            'id' : 'Id错误或非法操作', 
            'name' : '名称空值或格式错误'
            }, 
        'name-existed' : '该名称已经存在', 
        'unexist-or-error' : '数据不存在或错误', 
        'operation-failed' : '操作失败'
        }
    };


lang.role = {
    'name' : '名称', 
    'order' : '排序', 

    'button' : {
        'add' : '新建角色', 
        'modify' : '修改', 
        'delete' : '删除', 
        'move-up' : '上移', 
        'move-down' : '下移'
        }, 

    'tips' : {
        'confirm' : {
            'delete' : '确定删除吗？'
            }, 
        'value' : {
            'name' : '不能包含符号'
            }, 
        'input' : {
            'id' : 'Id错误或非法操作', 
            'name' : '名称空值或格式错误'
            }, 
        'name-existed' : '该名称已经存在', 
        'unexist-or-error' : '数据不存在或错误', 
        'operation-failed' : '操作失败'
        }
    };


lang.user = {
    'department' : '部门', 
    'role' : '角色', 
    'username' : '用户账号', 
    'password' : '登录密码', 
    'password-confirm' : '确认密码', 
    'code' : '员工编号', 
    'position' : '职称', 
    'email' : '电子邮箱', 
    'phone' : '手机号码', 
    'tel' : '座机号码', 
    'manager' : '业务管理员', 
    'admin' : '系统管理员', 
    'status' : '状态', 
    'leave-job' : '离职 / 冻结账号', 
    'create-account-email' : '发送账号密码通知邮件', 
    'all' : '全部', 
    'unlimited' : '不限', 
    'card' : '用户名片', 
    'not-logged-in' : '暂未登录', 
    'last-login' : '最近登录', 

    'button' : {
        'filter' : '筛选', 
        'add' : '新建用户', 
        'modify' : '修改', 
        'classify' : '归入', 
        'classify-department' : '归入部门', 
        'classify-role' : '归入角色', 
        'transfer' : '移交', 
        'remove' : '移除', 
        'restore' : '还原', 
        'delete' : '彻底删除'
        }, 

    'tips' : {
        'confirm' : {
            'remove' : '确定移除吗？', 
            'remove-all' : '确定移除选中的项目吗？', 
            'restore' : '确定还原吗？', 
            'restore-all' : '确定还原选中的项目吗？', 
            'delete' : '确定彻底删除吗？', 
            'delete-all' : '确定彻底删除选中的项目吗？', 
            'transfer' : '确定移交用户数据吗？'
            }, 
        'value' : {
            'keyword' : '用户账号、邮箱、手机', 
            'username' : '不能包含符号', 
            'password-add' : '空值表示随机密码', 
            'password-modify' : '空值表示不修改密码', 
            'password-confirm' : '重复一次登录密码', 
            'code' : '英文数字组合', 
            'position' : '不能包含符号', 
            'email' : 'username@mail.com', 
            'phone' : '数字和“-”符号组合', 
            'tel' : '数字和“-”符号组合'
            }, 
        'side' : {
            'username' : '2~16个英文、数字或中文组合', 
            'password' : '6~16个英文、数字或符号组合'
            }, 
        'input' : {
            'id' : 'Id错误或非法操作', 
            'username' : '用户账号空值或格式错误', 
            'username-pure-number-error' : '用户账号不能使用纯数字', 
            'password' : '登录密码空值或格式错误', 
            'password-confirm' : '确认密码空值或格式错误', 
            'password-confirm-error' : '登录密码与确认密码不一致', 
            'code' : '员工编号空值或格式错误', 
            'position' : '职称空值或格式错误', 
            'email' : '电子邮箱空值或格式错误', 
            'phone' : '手机号码空值或格式错误', 
            'tel' : '座机号码空值或格式错误'
            }, 
        'username-existed' : '该用户账号已经存在', 
        'email-existed' : '该电子邮箱已经存在', 
        'phone-existed' : '该手机号码已经存在', 
        'please-select-item' : '请选择项目', 
        'please-select-user' : '请选择用户', 
        'unexist-or-error' : '数据不存在或错误', 
        'operation-failed' : '操作失败', 
        'transfer-note' : '温馨提示：仅移交文件数据，不包含任务、日志等数据；'
        }, 

    'data' : {
        'status' : {
            'job-on' : '在职', 
            'job-off' : '离职'
            }
        }
    };


lang.file = {
    'name' : '名称', 
    'version' : '版本', 
    'type' : '类型', 
    'size' : '大小', 
    'space-usage' : '占用', 
    'contain' : '包含', 
    'contain-folder' : '个文件夹', 
    'contain-file' : '个文件', 
    'path' : '路径', 
    'remark' : '备注', 
    'share' : '共享', 
    'lock' : '锁定', 
    'sync' : '与父级目录同步', 
    'owner' : '所有者', 
    'create' : '创建', 
    'update' : '更新', 
    'time' : '时间', 
    'folder-inherit' : '继承父级目录权限', 
    'folder-share' : '共享文件夹', 
    'create-time' : '创建时间', 
    'update-time' : '更新时间', 
    'remove-time' : '移除时间', 
    'upload-time' : '上传时间', 
    'all' : '全部', 
    'unlimited' : '不限', 
    'exit-query' : '退出搜索', 
    'only-look-me' : '只看我的', 
    'no-remark' : '暂无备注', 
    'event-create' : '创建于', 
    'event-update' : '更新于', 
    'event-remove' : '移除于', 
    'event-upload' : '上传于', 

    'button' : {
        'filter' : '筛选', 
        'new' : '新建', 
        'add' : '新建文件夹', 
        'modify' : '修改', 
        'upload' : '上传', 
        'upversion' : '上传新版本', 
        'download' : '下载', 
        'move' : '移动', 
        'remove' : '移除', 
        'restore' : '还原', 
        'delete' : '彻底删除', 
        'confirm' : '确定', 
        'cancel' : '取消', 
        'select' : '选择', 
        'unpack' : '解压', 
        'open-share' : '开启共享', 
        'department-add' : '+ 部门', 
        'role-add' : '+ 角色', 
        'user-add' : '+ 用户', 
        'purview-change' : '保存更改', 
        'upload-picker' : '选择文件', 
        'upload-start' : '开始上传', 
        'upload-pause' : '暂停上传', 
        'upload-continue' : '继续上传', 
        'upload-renew' : '上传新版本', 
        'view-previous' : '上一个', 
        'view-next' : '下一个'
        }, 

    'tips' : {
        'confirm' : {
            'lock' : '确定锁定吗？', 
            'unlock' : '确定解除锁定吗？', 
            'replace' : '确定替换吗？', 
            'move' : '确定移动吗？', 
            'move-all' : '确定移动选中的项目吗？', 
            'remove' : '确定移除吗？', 
            'remove-all' : '确定移除选中的项目吗？', 
            'restore' : '确定还原吗？', 
            'restore-all' : '确定还原选中的项目吗？', 
            'delete' : '确定彻底删除吗？', 
            'delete-all' : '确定彻底删除选中的项目吗？', 
            'unpack-complete' : '解压完成！<br />确定转到解压目录吗？'
            }, 
        'value' : {
            'keyword' : '请输入文件关键词', 
            'name' : '50个字符以内', 
            'remark' : '100个字符以内', 
            'new-folder' : '新建文件夹', 
            'unzip-key' : '请输入解压密码'
            }, 
        'input' : {
            'id' : 'Id错误或非法操作', 
            'name' : '名称空值或包含以下符号 \\ \/ \: \* \? \" \< \> \|', 
            'remark' : '备注空值或长度超出限制', 
            'unzip-key' : '请输入解压密码'
            }, 
        'query-folder' : '在当前文件夹查找', 
        'name-existed' : '该名称已经存在', 
        'current-folder-removed' : '当前文件夹已经移除', 
        'current-file-removed' : '当前文件已经移除', 
        'please-select-item' : '请选择项目', 
        'please-select-folder' : '请选择文件夹', 
        'please-select-file' : '请选择文件', 
        'no-permission' : '您没有权限执行该操作', 
        'unallow-select-item' : '不允许选择此项目', 
        'not-allow-delete' : '不允许彻底删除', 
        'unexist-or-error' : '数据不存在或错误', 
        'operation-failed' : '操作失败', 
        'upload-task-waiting' : '等待上传...', 
        'upload-task-uploading' : '正在上传...', 
        'upload-task-error' : '上传错误', 
        'upload-task-success' : '上传成功', 
        'upload-extension-error' : '不允许该扩展名', 
        'upload-note' : '温馨提示：<br />支持拖放文件夹、文件到此窗口添加列表；<br />文件大小不能大于\[size\]MB；', 
        'upload-browser-unsupport' : '您的浏览器不支持HTML5和Flash', 
        'uploading-not-close' : '正在上传请勿刷新或关闭页面', 
        'upload-finished' : '上传任务已完成', 
        'download-folder-limit' : '不允许下载根目录文件夹', 
        'download-status-packing' : '正在打包...', 
        'download-status-start' : '开始下载...3秒后关闭窗口', 
        'unpack-status-unpacking' : '正在解压...', 
        'unpack-status-complete' : '解压完成...3秒后关闭窗口', 
        'zip-file-error' : '压缩文件错误或异常', 
        'purview-add-ok' : '添加成功', 
        'purview-modify-ok' : '修改成功', 
        'purview-share-ok' : '设置共享成功', 
        'purview-unshare-ok' : '取消共享成功', 
        'purview-sync-ok' : '设置同步成功', 
        'purview-unsync-ok' : '取消同步成功', 
        'purview-change-ok' : '更改成功', 
        'view-waiting-convert' : '文件正在等待转换暂时无法预览', 
        'view-reach-first' : '到达最前一个', 
        'view-reach-last' : '到达最后一个', 
        'view-image-control' : '支持鼠标拖动和缩放', 
        'view-browser-unsupport' : '在线预览必须使用支持HTML5的新式浏览器', 
        'recycle-note' : '温馨提示：移除少于30天的共享项目不允许彻底删除；'
        }, 

    'context' : {
        'download' : '下载', 
        'upversion' : '上传新版本', 
        'replace' : '替换该版本', 
        'lock' : '锁定', 
        'unlock' : '解除锁定', 
        'rename' : '重命名', 
        'remark' : '备注', 
        'modify' : '修改', 
        'purview' : '共享权限', 
        'copy' : '复制到...', 
        'move' : '移动到...', 
        'remove' : '移除到回收站', 
        'restore' : '还原', 
        'delete' : '彻底删除', 
        'task' : '任务分派', 
        'discuss' : '讨论', 
        'version' : '历史版本', 
        'log' : '操作日志', 
        'detail' : '详细属性'
        }, 

    'data' : {
        'role' : {
            'viewer' : '查看者', 
            'downloader' : '下载者', 
            'uploader' : '上传者', 
            'editor' : '编辑者', 
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
        'created' : '我创建的', 
        'shared' : '与我共享的'
    }, 

    'button' : {
        'post' : '发表', 
        'revoke' : '撤回', 
        'enter' : '进入讨论'
        }, 

    'tips' : {
        'confirm' : {
            'revoke' : '确定撤回吗？'
            }, 
        'value' : {
            'content' : '200个字符以内'
        }, 
        'input' : {
            'content' : '内容空值或长度超出限制'
            }, 
        'message-revoked' : '该消息已被撤回', 
        'operation-failed' : '操作失败'
        }
    };


lang.task = {
    'file' : '文件夹 / 文件', 
    'user' : '分派人', 
    'member' : '参与成员', 
    'content' : '内容', 
    'level' : '级别', 
    'deadline' : '期限', 
    'left-time' : '剩余时间', 
    'time' : '发布时间', 
    'status' : '状态', 
    'cause' : '撤消原因', 
    'reason' : '拒绝原因', 
    'postscript' : '附言', 

    'menu' : {
        'inbox' : '收件箱', 
        'outbox' : '发件箱', 
        'all' : '全部', 
        'unprocessed' : '未处理', 
        'accepted' : '已接受', 
        'rejected' : '已拒绝', 
        'completed' : '已完成', 
        'expired' : '已过期', 
        'revoked' : '已撤消', 
        'processing' : '进行中'
    }, 

    'button' : {
        'assign' : '任务分派', 
        'select' : '选择', 
        'revoke' : '撤消任务', 
        'pending' : '等待处理', 
        'accept' : '接受任务', 
        'reject' : '拒绝任务', 
        'completed' : '完成任务', 
        'processing' : '正在进行', 
        'detail' : '任务详情'
        }, 

    'tips' : {
        'confirm' : {
            'revoke' : '我确定撤消该任务', 
            'accept' : '我已阅读详细任务内容并确定接受', 
            'reject' : '我已阅读详细任务内容并确定拒绝', 
            'completed' : '我已审查任务并确定完成'
            }, 
        'value' : {
            'content' : '500个字符以内', 
            'cause' : '请输入撤消原因(必填)<br />200个字符以内', 
            'reason' : '请输入拒绝原因(必填)<br />200个字符以内', 
            'postscript' : '请输入完成附言(可选)<br />200个字符以内'
        }, 
        'input' : {
            'id' : 'Id错误或非法操作', 
            'content' : '内容空值或长度超出限制', 
            'cause' : '原因空值或长度超出限制', 
            'reason' : '原因空值或长度超出限制', 
            'postscript' : '附言空值或长度超出限制'
            }, 
        'please-select-user' : '请选择用户', 
        'deadline-time-error' : '期限不能早于当前时间', 
        'unexist-or-error' : '数据不存在或错误', 
        'operation-failed' : '操作失败'
        }, 

    'data' : {
        'level' : {
            'normal' : '常规', 
            'important' : '重要', 
            'urgent' : '紧急'
            }, 

        'status' : {
            'unprocessed' : '未处理', 
            'accepted' : '已接受', 
            'rejected' : '已拒绝', 
            'completed' : '已完成', 
            'expired' : '已过期', 
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
                'hour' : '时', 
                'minute' : '分', 
                'second' : '秒'
                }
            }
        }
    };


lang.log = {
    'folder' : '文件夹', 
    'file' : '文件', 
    'version' : '版本', 
    'user' : '用户', 
    'action' : '操作', 
    'time' : '时间', 
    'unlimited' : '不限', 
    'time-start' : '开始', 
    'time-end' : '结束', 
    
    'menu' : {
        'all' : '全部', 
        'created' : '我创建的', 
        'shared' : '与我共享的'
    }, 

    'button' : {
        'filter' : '筛选', 
        'query' : '查询'
        }, 

    'tips' : {
        'value' : {
            'keyword' : '用户账号、操作数据'
        }, 
        'input' : {
            'time-start' : '开始时间空值或格式错误', 
            'time-end' : '结束时间空值或格式错误'
            }
        }
    };


lang.login = {
    'login-id' : '用户账号 / 邮箱 / 手机', 
    'password' : '登录密码', 
    'forgot-password' : '忘记密码', 

    'button' : {
        'login' : '登录'
        }, 

    'tips' : {
        'input' : {
            'login-id' : '用户账号 / 邮箱 / 手机空值或格式错误', 
            'password' : '登录密码空值或格式错误'
            }, 
        'login-failure' : '没有该用户或登录密码错误', 
        'login-lock-ip' : '您已被锁定20分钟内禁止登录操作', 
        'logged-in' : '该用户已登录！请关闭浏览器重试', 
        'operation-failed' : '操作失败', 
        'html5-browser-note' : '当前浏览器太老旧了，请您使用支持HTML5的现代浏览器。'
        }
    };


lang.forgot = {
    'username' : '用户账号', 
    'password' : '登录密码', 
    'password-confirm' : '确认密码', 
    'email' : '电子邮箱', 
    'vericode' : '验证码', 

    'button' : {
        'get-vericode' : '获取验证码', 
        'reset-password' : '重设登录密码'
        }, 

    'tips' : {
        'value' : {
            'username' : '不能包含符号', 
            'password' : '建议英文+数字', 
            'password-confirm' : '重复一次登录密码', 
            'email' : 'username@mail.com', 
            'vericode' : '6位数字'
            }, 
        'side' : {
            'password' : '6~16个英文、数字或符号组合', 
            'vericode' : '验证码已发送到您的电子邮箱'
            }, 
        'input' : {
            'username' : '用户账号空值或格式错误', 
            'password' : '登录密码空值或格式错误', 
            'password-confirm' : '确认密码空值或格式错误', 
            'password-confirm-error' : '登录密码与确认密码不一致', 
            'email' : '电子邮箱空值或格式错误'
            }, 
        'user-info-error' : '用户账号或电子邮箱错误', 
        'forgot-lock-ip' : '您已被锁定20分钟内禁止重新操作', 
        'user-unexist' : '用户不存在', 
        'vericode-error' : '验证码错误', 
        'reset-success' : '登录密码重置成功', 
        'operation-failed' : '操作失败'
        }
    };


lang.main = {
    'tab' : {
        'explorer' : '文件管理', 
        'task' : '任务', 
        'discuss' : '讨论', 
        'activity' : '动态', 
        'recycle' : '回收站', 
        'admin' : '管理面板', 
        'refresh' : '刷新'
        }, 

    'tool' : {
        'sync' : '同步工具', 
        'help' : '使用帮助', 
        'about' : '关于', 
        'profile' : '账号设置', 
        'logout' : '退出'
        }, 

    'about' : {
        'version' : '当前版本', 
        'license' : '授权协议', 
        'website' : '官方网站'
        }
    };


lang.admin = {
    'menu' : {
        'statistic' : '统计信息', 
        'department' : '部门结构', 
        'role' : '角色分组', 
        'user' : '用户管理', 
        'user-list' : '用户列表', 
        'user-recycle' : '用户回收站', 
        'log' : '日志记录', 
        'config' : '系统配置'
        }, 

    'statistic' : {
        'user' : '用户', 
        'folder' : '文件夹', 
        'file' : '文件', 
        'occupy' : '占用空间', 
        'today' : '今天', 
        'yesterday' : '昨天', 
        'week' : '本周', 
        'month' : '本月', 
        'upload' : '上传', 
        'download' : '下载', 

        tips : {
            'data-exception' : '数据异常'
            }
        }, 

    'config' : {
        'app-name' : '应用名称', 
        'upload-extension' : '上传扩展限制', 
        'upload-size' : '上传大小限制', 
        'version-count' : '版本数量限制', 
        'mail-address' : '服务邮箱地址', 
        'mail-server' : '邮件服务器地址', 
        'mail-port' : '邮件服务器端口', 
        'mail-ssl' : '邮箱服务器安全', 
        'mail-ssl-connect' : '使用 SSL 连接', 
        'mail-username' : '邮箱登录账号', 
        'mail-password' : '邮箱登录密码', 

        'button' : {
            'confirm' : '确定'
            }, 

        'tips' : {
            'value' : {
                'upload-extension' : '允许上传的文件格式\<br \/\>扩展名不带“\.”符号\<br \/\>多个扩展名使用回车换行分隔'
                }, 
            'input' : {
                'app-name' : '应用名称空值或格式错误', 
                'upload-extension' : '上传扩展限制空值或格式错误', 
                'upload-size' : '上传大小限制空值或格式错误', 
                'upload-size-range' : '上传大小限制数值范围1~2048', 
                'version-count' : '版本数量限制空值或格式错误', 
                'version-count-range' : '版本数量限制数值范围10~1024', 
                'mail-address' : '服务邮箱地址空值或格式错误', 
                'mail-server' : '邮件服务器地址空值或格式错误', 
                'mail-port' : '邮件服务器端口空值或格式错误', 
                'mail-username' : '服务邮箱账号空值或格式错误', 
                'mail-password' : '邮箱服务密码空值或格式错误'
                }, 
            're-login' : '3秒后重新登录', 
            'config-read-failed' : '配置读取失败', 
            'operation-failed' : '操作失败'
            }
        }
    };


lang.ui = {
    'button' : {
        'confirm' : '确定', 
        'cancel' : '取消'
        }
    };


lang.list = {
    'no-data' : '暂无数据', 
    'data-count' : '已加载 [count] 个项目'
    };


lang.type = {
    'folder' : '文件夹', 
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
    'code' : '代码', 
    'image' : '图片', 
    'drawing' : '图纸', 
    'audio' : '音频', 
    'video' : '视频', 
    'zip' : '压缩', 
    'other' : '其它'
    };


lang.action = {
    'upload' : '上传', 
    'download' : '下载', 
    'add' : '添加', 
    'modify' : '修改', 
    'edit' : '编辑', 
    'view' : '查看', 
    'rename' : '重命名', 
    'remark' : '备注', 
    'copy' : '复制', 
    'move' : '移动', 
    'remove' : '移除', 
    'restore' : '还原', 
    'delete' : '删除', 
    'upversion' : '上传新版本', 
    'replace' : '替换版本', 
    'lock' : '锁定', 
    'unlock' : '解除锁定', 
    'share' : '设置共享', 
    'unshare' : '取消共享', 
    'sync' : '设置同步', 
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
    '100mb-more': '100MB或以上'
    };


lang.timestamp = {
    'this-day' : '今天', 
    '1-day-ago' : '昨天', 
    '2-day-ago' : '前天', 
    'this-week' : '本周', 
    '1-week-ago' : '一周前', 
    '2-week-ago' : '两周前', 
    'this-month' : '本月', 
    '1-month-ago' : '一月前', 
    '2-month-ago' : '两月前', 
    'this-year' : '本年', 
    '1-year-ago' : '一年前', 
    '2-year-ago' : '两年前'
    };


lang.timespan = {
    'second' : '秒前', 
    'minute' : '分钟前', 
    'hour' : '小时前', 
    'day' : '天前', 
    'month' : '月前', 
    'year' : '年前'
    };