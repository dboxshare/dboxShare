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
            'name' : '不能包含符号<br />长度 2~24 个字符<br />多个名称使用回车换行分隔<br />每次限制 50 行'
            }, 
        'input' : {
            'id' : 'Id 错误或非法操作', 
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
            'name' : '不能包含符号<br />长度 2~24 个字符<br />多个名称使用回车换行分隔<br />每次限制 50 行'
            }, 
        'input' : {
            'id' : 'Id 错误或非法操作', 
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
    'name' : '真实姓名', 
    'code' : '员工编号', 
    'title' : '职称', 
    'email' : '电子邮箱', 
    'phone' : '手机号码', 
    'tel' : '座机号码', 
    'upload-size' : '上传大小限制', 
    'download-size' : '下载大小限制', 
    'admin' : '系统管理员', 
    'limit' : '限制', 
    'status' : '状态', 
    'freeze-account' : '冻结账号 / 离职', 
    'create-account-email' : '发送账号密码通知邮件', 
    'vericode' : '验证码', 
    'ad-username' : 'AD 域用户账号', 
    'all' : '全部', 
    'unlimited' : '不限', 
    'user-card' : '用户名片', 
    'not-logged-in' : '暂未登录', 
    'last-login' : '最近登录', 

    'button' : {
        'filter' : '筛选', 
        'add' : '新建用户', 
        'create' : '批量创建', 
        'modify' : '修改', 
        'change' : '更改', 
        'change-upload-size' : '上传大小限制', 
        'change-download-size' : '下载大小限制', 
        'classify' : '归入', 
        'classify-department' : '部门', 
        'classify-role' : '角色', 
        'transfer' : '移交', 
        'remove' : '移除', 
        'restore' : '还原', 
        'delete' : '彻底删除', 
        'get-vericode' : '获取验证码', 
        'reset-password' : '重设登录密码', 
        'bind-account' : '绑定账号', 
        'create-account' : '创建账号'
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
            'password' : '建议使用组合字符', 
            'password-add' : '空值表示随机密码', 
            'password-modify' : '空值表示不修改密码', 
            'password-confirm' : '重复一次登录密码', 
            'name' : '不能包含符号', 
            'code' : '英文数字组合', 
            'title' : '不能包含符号', 
            'email' : 'username@mail.com', 
            'phone' : '数字和“-”符号组合', 
            'tel' : '数字和“-”符号组合', 
            'vericode' : '6 位数字', 
            'bind-username' : '请输入绑定用户账号', 
            'bind-password' : '请输入绑定用户密码'
            }, 
        'side' : {
            'username' : '2~16 个英文、数字或中文组合', 
            'password' : '6~16 个英文、数字或符号组合', 
            'upload-size' : '0 表示按照系统设置', 
            'download-size' : '0 表示按照系统设置', 
            'vericode' : '验证码已发送到您的电子邮箱'
            }, 
        'input' : {
            'id' : 'Id 错误或非法操作', 
            'username' : '用户账号空值或包含非法符号', 
            'username-pure-number-error' : '用户账号不能使用纯数字', 
            'password' : '登录密码空值或包含非法符号', 
            'password-confirm' : '确认密码空值或包含非法符号', 
            'password-confirm-error' : '登录密码与确认密码不一致', 
            'password-and-email-empty' : '登录密码和电子邮箱必须填写其中一项', 
            'name' : '真实姓名空值或格式错误', 
            'code' : '员工编号空值或包含非法符号', 
            'title' : '职称空值或格式错误', 
            'email' : '电子邮箱空值或格式错误', 
            'phone' : '手机号码空值或格式错误', 
            'tel' : '座机号码空值或格式错误', 
            'upload-size' : '上传大小限制空值或格式错误', 
            'upload-size-range' : '上传大小限制数值范围 0~10240', 
            'download-size' : '下载大小限制空值或格式错误', 
            'download-size-range' : '下载大小限制数值范围 0~10240'
            }, 
        'username-existed' : '该用户账号已经存在', 
        'email-existed' : '该电子邮箱已经存在', 
        'phone-existed' : '该手机号码已经存在', 
        'please-select-item' : '请选择项目', 
        'please-select-user' : '请选择用户', 
        'username-email-error' : '用户账号或电子邮箱错误', 
        'forgot-lock-ip' : '您已被锁定 20 分钟内禁止重新操作', 
        'user-unexist' : '用户不存在', 
        'vericode-error' : '验证码错误', 
        'reset-success' : '登录密码重置成功', 
        'bind-login-failed' : '该用户不存在或登录密码错误', 
        'bind-success' : '用户绑定成功', 
        'create-success' : '用户创建成功', 
        'transfer-note' : '温馨提示：仅移交文件数据，不包含收藏、关注、日志等数据；', 
        'create-account-note' : '我没有本站用户账号？请点击<a href="javascript: userCreate();">创建用户账号</a>', 
        'unexist-or-error' : '数据不存在或错误', 
        'operation-failed' : '操作失败'
        }, 

    'data' : {
        'status' : {
            'on-job' : '在职', 
            'off-job' : '离职'
            }
        }
    };


lang.file = {
    'name' : '名称', 
    'version' : '版本', 
    'type' : '类型', 
    'size' : '大小', 
    'occupy-total' : '占用', 
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
    'folder-share' : '创建共享文件夹', 
    'create-time' : '创建时间', 
    'update-time' : '更新时间', 
    'remove-time' : '移除时间', 
    'used-time' : '使用时间', 
    'all' : '全部', 
    'unlimited' : '不限', 
    'exit-query' : '退出搜索', 
    'only-look-me' : '只看我的', 
    'no-remark' : '暂无备注', 
    'event-create' : '创建于', 
    'event-update' : '更新于', 
    'event-remove' : '移除于', 
    'and-other-file' : '等文件', 

    'menu' : {
        'all' : '全部', 
        'personal' : '个人空间', 
        'share' : '共享空间', 
        'collect' : '收藏文件', 
        'follow' : '关注文件', 
        'recent' : '最近使用', 
        'link' : '链接分享', 
        'recycle' : '回收站'
        }, 

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
        'purview-change' : '更改', 
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
            'unlock' : '确定取消锁定吗？', 
            'replace' : '确定替换吗？', 
            'move' : '确定移动吗？', 
            'move-all' : '确定移动选中的项目吗？', 
            'remove' : '确定移除吗？', 
            'remove-all' : '确定移除选中的项目吗？', 
            'restore' : '确定还原吗？', 
            'restore-all' : '确定还原选中的项目吗？', 
            'delete' : '确定彻底删除吗？', 
            'delete-all' : '确定彻底删除选中的项目吗？'
            }, 
        'value' : {
            'keyword' : '在当前目录中搜索文件', 
            'name' : '50 个字符以内', 
            'remark' : '100 个字符以内', 
            'new-folder' : '新建文件夹', 
            'unzip-key' : '请输入解压密码'
            }, 
        'input' : {
            'id' : 'Id 错误或非法操作', 
            'name' : '名称空值或包含非法符号', 
            'remark' : '备注空值或长度超出限制', 
            'unzip-key' : '请输入解压密码'
            }, 
        'name-existed' : '该名称已经存在', 
        'current-folder-removed' : '当前文件夹已经移除', 
        'current-file-removed' : '当前文件已经移除', 
        'please-select-item' : '请选择项目', 
        'please-select-folder' : '请选择文件夹', 
        'please-select-file' : '请选择文件', 
        'no-permission' : '您没有权限执行该操作', 
        'unallow-select-item' : '不允许选择此项目', 
        'not-allow-delete' : '不允许彻底删除', 
        'upload-task-waiting' : '等待上传...', 
        'upload-task-uploading' : '正在上传...', 
        'upload-task-error' : '上传错误', 
        'upload-task-success' : '上传成功', 
        'upload-size-limit' : '上传文件大小限制 \{size\} MB', 
        'upload-extension-error' : '不允许该扩展名', 
        'upload-note' : '温馨提示：<br />支持拖放文件夹、文件到此窗口添加列表；<br />文件大小限制 \{size\} MB；', 
        'upload-browser-unsupport' : '您的浏览器不支持 HTML5 和 Flash', 
        'uploading-not-close' : '正在上传请勿刷新或关闭页面', 
        'download-waiting' : '正在等待下载...', 
        'download-status-packing' : '正在打包...', 
        'download-status-start' : '开始下载...3 秒后关闭窗口', 
        'download-size-limit' : '下载文件大小限制 \{size\} MB', 
        'unpack-status-unpacking' : '正在解压...', 
        'unpack-status-complete' : '解压完成...3 秒后关闭窗口', 
        'unpack-complete' : '解压已完成！<a href="javascript: unpackComplete_GotoView();">点击查看</a>', 
        'zip-file-error' : '压缩文件错误或异常', 
        'copy-complete' : '复制已完成！<a href="javascript: listGoto(\{folderid\});">点击查看</a>', 
        'move-complete' : '移动已完成！<a href="javascript: listGoto(\{folderid\});">点击查看</a>', 
        'purview-add-ok' : '添加成功', 
        'purview-modify-ok' : '修改成功', 
        'purview-share-ok' : '设置共享成功', 
        'purview-unshare-ok' : '取消共享成功', 
        'purview-sync-ok' : '设置同步成功', 
        'purview-unsync-ok' : '取消同步成功', 
        'purview-change-ok' : '更改成功', 
        'purview-sync-change' : '同步更改子文件夹', 
        'view-waiting-convert' : '文件正在等待转换暂时无法预览', 
        'view-reach-first' : '到达最前一个', 
        'view-reach-last' : '到达最后一个', 
        'view-previous' : '左方向键切换上一个', 
        'view-next' : '右方向键切换下一个', 
        'view-browser-unsupport' : '在线预览必须使用支持 HTML5 的新式浏览器', 
        'recycle-note' : '温馨提示：移除少于 \{days\} 天的共享项目不允许彻底删除；', 
        'unexist-or-error' : '数据不存在或错误', 
        'operation-failed' : '操作失败'
        }, 

    'context' : {
        'download' : '下载', 
        'collect' : '收藏', 
        'uncollect' : '取消收藏', 
        'follow' : '关注', 
        'unfollow' : '取消关注', 
        'link' : '链接分享', 
        'rename' : '重命名', 
        'remark' : '备注', 
        'purview' : '共享权限', 
        'copy' : '复制到...', 
        'move' : '移动到...', 
        'remove' : '移除到回收站', 
        'restore' : '还原', 
        'delete' : '彻底删除', 
        'lock' : '锁定', 
        'unlock' : '取消锁定', 
        'version' : '版本管理', 
        'upversion' : '上传新版本', 
        'replace' : '替换该版本', 
        'log' : '操作日志', 
        'detail' : '详细属性'
        }, 

    'data' : {
        'purview' : {
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


lang.link = {
    'folder' : '文件夹', 
    'file' : '文件', 
    'title' : '标题', 
    'deadline' : '截止时间', 
    'expiry-date' : '有效期', 
    'left-time' : '剩余时间', 
    'password' : '提取密码', 
    'count' : '分享次数', 
    'user' : '发布用户', 
    'time' : '发布时间', 
    'email' : '邮件通知', 
    'message' : '邮件留言', 
    'status' : '状态', 
    'qrcode' : '二维码', 
    'link' : '分享链接', 
    'list-home' : '主页', 

    'button' : {
        'share' : '分享链接', 
        'copy' : '复制链接', 
        'refresh' : '刷新', 
        'revoke' : '撤消', 
        'log' : '日志', 
        'extract' : '提取', 
        'download' : '打包下载'
        }, 

    'tips' : {
        'confirm' : {
            'revoke' : '确定撤消吗？'
            }, 
        'value' : {
            'keyword' : '用户账号', 
            'title' : '100 个字符以内', 
            'email' : '每个 email 地址使用“;”符号分隔', 
            'message' : '200 个字符以内', 
            'password' : '请输入提取密码'
        }, 
        'input' : {
            'title' : '标题空值或长度超出限制', 
            'message' : '留言空值或长度超出限制', 
            'password' : '请输入提取密码'
            }, 
        'link-copy-text' : '给你分享了 \{title\}\n点击链接查看 \{link\}\n提取密码 \{password\}', 
        'link-expired' : '该分享链接已过期', 
        'link-revoked' : '该分享链接已被撤消', 
        'link-login-failed' : '提取密码错误或已失效', 
        'link-lock-ip' : '您已被锁定 20 分钟内禁止提取操作', 
        'unexist-or-error' : '数据不存在或错误', 
        'illegal-operation' : '非法操作', 
        'operation-failed' : '操作失败', 
        }, 

    'log' : {
        'ip' : 'IP', 
        'time' : '时间'
        }, 

    'data' : {
        'status' : {
            'sharing' : '分享中', 
            'expired' : '已过期', 
            'revoked' : '已撤消'
            }, 

        'time' : {
            'day' : '天', 
            'hour' : '时', 
            'minute' : '分', 
            'second' : '秒'
            }
        }
    };


lang.log = {
    'folder' : '文件夹', 
    'file' : '文件', 
    'user' : '用户', 
    'action' : '操作', 
    'time' : '时间', 
    'unlimited' : '不限', 
    'time-start' : '开始', 
    'time-end' : '结束', 
    
    'menu' : {
        'all' : '全部', 
        'created' : '我创建的', 
        'collected' : '我收藏的', 
        'followed' : '我关注的', 
        'shared' : '与我共享的'
        }, 

    'button' : {
        'filter' : '筛选', 
        'export' : '导出', 
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
    'ad-user-bind' : 'AD 域用户账号绑定', 

    'button' : {
        'login' : '登录'
        }, 

    'tips' : {
        'input' : {
            'login-id' : '用户账号 / 邮箱 / 手机空值或格式错误', 
            'password' : '登录密码空值或格式错误'
            }, 
        'login-failed' : '该用户不存在或登录密码错误', 
        'login-lock-ip' : '您已被锁定 20 分钟内禁止登录操作', 
        'logged-in' : '该用户已登录！请关闭浏览器重试', 
        'html5-browser-note' : '当前浏览器太老旧了，请您使用支持 HTML5 的现代浏览器。', 
        'operation-failed' : '操作失败'
        }
    };


lang.main = {
    'tab' : {
        'file' : '文件', 
        'activity' : '动态', 
        'admin' : '系统管理', 
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
        'link' : '链接分享', 
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

        'tips' : {
            'data-exception' : '数据异常'
            }
        }, 

    'config' : {
        'app-name' : '应用名称', 
        'upload-extension' : '上传扩展限制', 
        'upload-size' : '上传大小限制', 
        'download-size' : '下载大小限制', 
        'version-count' : '版本数量限制', 
        'mail-address' : '服务邮箱地址', 
        'mail-username' : '邮箱登录账号', 
        'mail-password' : '邮箱登录密码', 
        'mail-server' : '邮件服务器地址', 
        'mail-server-port' : '邮件服务器端口', 
        'mail-server-security' : '邮件服务器安全', 
        'mail-server-ssl-connection' : '启用 SSL 连接', 

        'button' : {
            'confirm' : '确定'
            }, 

        'tips' : {
            'value' : {
                'upload-extension' : '允许上传的文件格式<br />扩展名不带“\.”符号<br />多个扩展名使用回车换行分隔'
                }, 
            'input' : {
                'app-name' : '应用名称空值或格式错误', 
                'upload-extension' : '上传扩展限制空值或格式错误', 
                'upload-size' : '上传大小限制空值或格式错误', 
                'upload-size-range' : '上传大小限制数值范围 1~10240', 
                'download-size' : '下载大小限制空值或格式错误', 
                'download-size-range' : '下载大小限制数值范围 1~10240', 
                'version-count' : '版本数量限制空值或格式错误', 
                'version-count-range' : '版本数量限制数值范围 50~500', 
                'mail-address' : '服务邮箱地址空值或格式错误', 
                'mail-username' : '邮箱登录账号空值或格式错误', 
                'mail-password' : '邮箱登录密码空值或格式错误', 
                'mail-server' : '邮件服务器地址空值或格式错误', 
                'mail-server-port' : '邮件服务器端口空值或格式错误'
                }, 
            're-login' : '3 秒后重新登录', 
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
    'link' : '链接分享', 
    'lock' : '锁定', 
    'unlock' : '取消锁定', 
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
    '100mb-more': '100MB 或以上'
    };


lang.time = {
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