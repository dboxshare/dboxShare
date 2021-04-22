lang.object = {
    'folder' : 'Folder', 
    'file' : 'File', 
    'department' : 'Department', 
    'role' : 'Role', 
    'user' : 'User', 
    'discuss' : 'Discuss', 
    'task' : 'Task', 
    'log' : 'Log'
    };


lang.department = {
    'name' : 'Name', 
    'order' : 'Order', 
    'all' : 'All', 

    'button' : {
        'add' : 'Add department', 
        'modify' : 'Modify', 
        'delete' : 'Delete', 
        'move-up' : 'Move up', 
        'move-down' : 'Move down'
        }, 

    'tips' : {
        'confirm' : {
            'delete' : 'Are you sure you want to delete?'
            }, 
        'value' : {
            'name' : 'Cannot contain symbols\<br \/\>Length 2~24 characters\<br \/\>Line break separated\<br \/\>Limit 50 lines'
            }, 
        'input' : {
            'id' : 'Id error or illegal operation', 
            'name' : 'Name is empty or format error'
            }, 
        'name-existed' : 'Name already exists', 
        'unexist-or-error' : 'Data does not exist or is wrong', 
        'operation-failed' : 'Operation failed'
        }
    };


lang.role = {
    'name' : 'Name', 
    'order' : 'Order', 

    'button' : {
        'add' : 'Add role', 
        'modify' : 'Modify', 
        'delete' : 'Delete', 
        'move-up' : 'Move up', 
        'move-down' : 'Move down'
        }, 

    'tips' : {
        'confirm' : {
            'delete' : 'Are you sure you want to delete?'
            }, 
        'value' : {
            'name' : 'Cannot contain symbols\<br \/\>Length 2~24 characters\<br \/\>Line break separated\<br \/\>Limit 50 lines'
            }, 
        'input' : {
            'id' : 'Id error or illegal operation', 
            'name' : 'Name is empty or format error'
            }, 
        'name-existed' : 'Name already exists', 
        'unexist-or-error' : 'Data does not exist or is wrong', 
        'operation-failed' : 'Operation failed'
        }
    };


lang.user = {
    'department' : 'Department', 
    'role' : 'Role', 
    'username' : 'Username', 
    'password' : 'Password', 
    'password-confirm' : 'Confirm password', 
    'code' : 'Job number', 
    'title' : 'Job title', 
    'email' : 'Email', 
    'phone' : 'Phone', 
    'tel' : 'Tel', 
    'admin' : 'Admin', 
    'status' : 'Status', 
    'freeze-account' : 'Account freeze / Leave job', 
    'create-account-email' : 'Send account password to email', 
    'vericode' : 'Vericode', 
    'ad-username' : 'AD domain username', 
    'all' : 'All', 
    'unlimited' : 'Unlimited', 
    'user-card' : 'User card', 
    'not-logged-in' : 'Not logged in', 
    'last-login' : 'Last login', 

    'button' : {
        'filter' : 'Filter', 
        'add' : 'Add user', 
        'create' : 'Batch create', 
        'modify' : 'Modify', 
        'classify' : 'Classify', 
        'classify-department' : 'Department', 
        'classify-role' : 'Role', 
        'transfer' : 'Transfer', 
        'remove' : 'Remove', 
        'restore' : 'Restore', 
        'delete' : 'Delete', 
        'get-vericode' : 'Get vericode', 
        'reset-password' : 'Reset password', 
        'bind-account' : 'Bind account', 
        'create-account' : 'Create account'
        }, 

    'tips' : {
        'confirm' : {
            'remove' : 'Are you sure you want to remove?', 
            'remove-all' : 'Are you sure you want to remove the selected items?', 
            'restore' : 'Are you sure you want to restore?', 
            'restore-all' : 'Are you sure you want to restore the selected items?', 
            'delete' : 'Are you sure you want to delete?', 
            'delete-all' : 'Are you sure you want to delete the selected items?', 
            'transfer' : 'Are you sure you want to transfer user data?'
            }, 
        'value' : {
            'keyword' : 'Username, email, phone', 
            'username' : 'Cannot contain symbols', 
            'password' : 'Use character combinations', 
            'password-add' : 'Empty is a random password', 
            'password-modify' : 'Empty does not change', 
            'password-confirm' : 'Repeat the password once', 
            'code' : 'English and numbers', 
            'title' : 'Cannot contain symbols', 
            'email' : 'username@mail.com', 
            'phone' : 'Numbers and "-" symbol', 
            'tel' : 'Numbers and "-" symbol', 
            'vericode' : '6 digits', 
            'bind-username' : 'Enter the binding username', 
            'bind-password' : 'Enter the binding password'
            }, 
        'side' : {
            'username' : '2~16 English and numbers', 
            'password' : '6~16 English, numbers or symbols', 
            'vericode' : 'Vericode has been sent to your email'
            }, 
        'input' : {
            'id' : 'Id error or illegal operation', 
            'username' : 'Username is empty or contains illegal symbols', 
            'username-pure-number-error' : 'Numeric username not allowed', 
            'password' : 'Password is empty or contains illegal symbols', 
            'password-confirm' : 'Confirm password is empty or contains illegal symbols', 
            'password-confirm-error' : 'Two passwords do not match', 
            'password-and-email-empty' : 'Login password and email cannot be empty at the same time', 
            'code' : 'Job number is empty or contains illegal symbols', 
            'title' : 'Job title is empty or format error', 
            'email' : 'Email is empty or format error', 
            'phone' : 'Phone is empty or format error', 
            'tel' : 'Tel is empty or format error'
            }, 
        'username-existed' : 'Username already exists', 
        'email-existed' : 'Email already exists', 
        'phone-existed' : 'Phone already exists', 
        'please-select-item' : 'Please select item', 
        'please-select-user' : 'Please select user', 
        'username-email-error' : 'Username or email error', 
        'forgot-lock-ip' : 'Re-operation is not allowed for 20 minutes', 
        'user-unexist' : 'User does not exist', 
        'vericode-error' : 'Vericode error', 
        'reset-success' : 'Login password reset successful', 
        'bind-login-failed' : 'User does not exist or the login password is wrong', 
        'bind-success' : 'User binding succeeded', 
        'create-success' : 'User created successfully', 
        'unexist-or-error' : 'Data does not exist or is wrong', 
        'operation-failed' : 'Operation failed', 
        'transfer-note' : 'Tips: only transfer file data, not including task, log and other data;', 
        'create-account-note' : 'I don\'t have a user account for this website? Please click <a href="javascript: userCreate();">create user account</a>'
        }, 

    'data' : {
        'status' : {
            'job-on' : 'On job', 
            'job-off' : 'Off job'
            }
        }
    };


lang.file = {
    'name' : 'Name', 
    'version' : 'Version', 
    'type' : 'Type', 
    'size' : 'Size', 
    'space-usage' : 'Space usage', 
    'contain' : 'Contain', 
    'contain-folder' : 'Folders', 
    'contain-file' : 'Files', 
    'path' : 'Path', 
    'remark' : 'Remark', 
    'share' : 'Share', 
    'lock' : 'Lock', 
    'sync' : 'Synchronize with parent', 
    'owner' : 'Owner', 
    'create' : 'Create', 
    'update' : 'Update', 
    'time' : 'Time', 
    'folder-inherit' : 'Inherit parent directory permissions', 
    'folder-share' : 'Share folder', 
    'create-time' : 'Create time', 
    'update-time' : 'Update time', 
    'remove-time' : 'Remove time', 
    'upload-time' : 'Upload time', 
    'all' : 'All', 
    'unlimited' : 'Unlimited', 
    'exit-query' : 'Exit query', 
    'only-look-me' : 'Only look at me', 
    'no-remark' : 'No remark', 
    'event-create' : 'Created in', 
    'event-update' : 'Updated in', 
    'event-remove' : 'Removed in', 
    'event-upload' : 'Uploaded in', 

    'button' : {
        'filter' : 'Filter', 
        'new' : 'New', 
        'add' : 'Add folder', 
        'modify' : 'Modify', 
        'upload' : 'Upload', 
        'upversion' : 'Upload new version', 
        'download' : 'Download', 
        'move' : 'Move', 
        'remove' : 'Remove', 
        'restore' : 'Restore', 
        'delete' : 'Delete', 
        'confirm' : 'Confirm', 
        'cancel' : 'Cancel', 
        'select' : 'Select', 
        'unpack' : 'Unpack', 
        'open-share' : 'Open share', 
        'department-add' : '+ Department', 
        'role-add' : '+ Role', 
        'user-add' : '+ User', 
        'purview-change' : 'Change subfolder', 
        'upload-picker' : 'Select files', 
        'upload-start' : 'Start upload', 
        'upload-pause' : 'Pause upload', 
        'upload-continue' : 'Continue upload', 
        'upload-renew' : 'New version upload', 
        'view-previous' : 'Previous', 
        'view-next' : 'Next'
        }, 

    'tips' : {
        'confirm' : {
            'lock' : 'Are you sure you want to lock?', 
            'unlock' : 'Are you sure you want to unlock?', 
            'replace' : 'Are you sure you want to replace?', 
            'move' : 'Are you sure you want to move?', 
            'move-all' : 'Are you sure you want to move the selected items?', 
            'remove' : 'Are you sure you want to remove?', 
            'remove-all' : 'Are you sure you want to remove the selected items?', 
            'restore' : 'Are you sure you want to restore?', 
            'restore-all' : 'Are you sure you want to restore the selected items?', 
            'delete' : 'Are you sure you want to delete?', 
            'delete-all' : 'Are you sure you want to delete the selected items?'
            }, 
        'value' : {
            'keyword' : 'Please enter file keywords', 
            'name' : 'Maximum is 50 characters', 
            'remark' : 'Maximum is 100 characters', 
            'new-folder' : 'New folder', 
            'unzip-key' : 'Enter unzip password'
            }, 
        'input' : {
            'id' : 'Id error or illegal operation', 
            'name' : 'Name is empty or contains illegal symbols', 
            'remark' : 'Remark is empty or format error', 
            'unzip-key' : 'Please enter the unzip password'
            }, 
        'query-folder' : 'Find in current folder', 
        'name-existed' : 'Name already exists', 
        'current-folder-removed' : 'Current folder has been removed', 
        'current-file-removed' : 'Current file has been removed', 
        'please-select-item' : 'Please select item', 
        'please-select-folder' : 'Please select folder', 
        'please-select-file' : 'Please select file', 
        'no-permission' : 'You don\'t have this permission', 
        'unallow-select-item' : 'This item is not allowed to be selected', 
        'not-allow-delete' : 'Not allowed to delete', 
        'upload-task-waiting' : 'Waiting...', 
        'upload-task-uploading' : 'Uploading...', 
        'upload-task-error' : 'Error', 
        'upload-task-success' : 'Success', 
        'upload-extension-error' : 'Extension not allowed', 
        'upload-note' : 'Tips:<br />Support drag and drop folder or file to this window to add list;<br />The file cannot be larger than \{size\} MB;', 
        'upload-browser-unsupport' : 'Your browser does not support HTML5 and Flash', 
        'uploading-not-close' : 'Uploading: please do not refresh or close the page.', 
        'download-waiting' : 'Waiting for downloading...', 
        'download-status-packing' : 'Packing...', 
        'download-status-start' : 'Start downloading... close the window after 3 seconds', 
        'unpack-status-unpacking' : 'Unpacking...', 
        'unpack-status-complete' : 'Unpack completed...close the window after 3 seconds', 
        'unpack-complete' : 'Unpack completed! <a href="javascript: unpackComplete_GotoView();">click to view</a>', 
        'zip-file-error' : 'Zip file error or exception', 
        'copy-complete' : 'Copy completed! <a href="javascript: listGoto(\{folderid\});">click to view</a>', 
        'move-complete' : 'Move completed! <a href="javascript: listGoto(\{folderid\});">click to view</a>', 
        'purview-add-ok' : 'Added successfully', 
        'purview-modify-ok' : 'Modified successfully', 
        'purview-share-ok' : 'Set shared successfully', 
        'purview-unshare-ok' : 'Unshared successfully', 
        'purview-sync-ok' : 'Set synced successfully', 
        'purview-unsync-ok' : 'Unsynced successfully', 
        'purview-change-ok' : 'Changed successfully', 
        'view-waiting-convert' : 'File preview is waiting for conversion', 
        'view-reach-first' : 'Reach the first', 
        'view-reach-last' : 'Reach the last', 
        'view-browser-unsupport' : 'Online preview must use a new browser that supports HTML5', 
        'unexist-or-error' : 'Data does not exist or is wrong', 
        'operation-failed' : 'Operation failed', 
        'recycle-note' : 'Tips: removed shared items less than 30 days is not allowed to completely delete;'
        }, 

    'context' : {
        'download' : 'Download', 
        'upversion' : 'Upload new version', 
        'replace' : 'Replace version', 
        'lock' : 'Lock', 
        'unlock' : 'Unlock', 
        'rename' : 'Rename', 
        'remark' : 'Remark', 
        'modify' : 'Modify', 
        'purview' : 'Share purview', 
        'copy' : 'Copy to...', 
        'move' : 'Move to...', 
        'remove' : 'Remove', 
        'restore' : 'Restore', 
        'delete' : 'Delete', 
        'task' : 'Task assign', 
        'discuss' : 'Discuss', 
        'version' : 'Version', 
        'log' : 'Log', 
        'detail' : 'Detail'
        }, 

    'data' : {
        'role' : {
            'viewer' : 'Viewer', 
            'downloader' : 'Downloader', 
            'uploader' : 'Uploader', 
            'editor' : 'Editor', 
            'manager' : 'Manager'
            }, 

        'status' : {
            'yes' : 'Yes', 
            'no' : 'No'
            }
        }
    };


lang.discuss = {
    'menu' : {
        'all' : 'All', 
        'created' : 'My created', 
        'shared' : 'Shared with me'
    }, 

    'button' : {
        'post' : 'Post', 
        'revoke' : 'Revoke', 
        'enter' : 'Enter discuss'
        }, 

    'tips' : {
        'confirm' : {
            'revoke' : 'Are you sure you want to revoke?'
            }, 
        'value' : {
            'content' : 'Maximum is 200 characters'
        }, 
        'input' : {
            'content' : 'Content empty or length exceeds limit'
            }, 
        'message-revoked' : 'Message has been revoked', 
        'operation-failed' : 'Operation failed'
        }
    };


lang.task = {
    'file' : 'Folder / File', 
    'user' : 'Dispatcher', 
    'member' : 'Members', 
    'content' : 'Content', 
    'level' : 'Level', 
    'deadline' : 'Deadline', 
    'left-time' : 'Left time', 
    'time' : 'Post time', 
    'status' : 'Status', 
    'cause' : 'Cause', 
    'reason' : 'Reason', 
    'remark' : 'Remark', 

    'menu' : {
        'inbox' : 'Inbox', 
        'outbox' : 'Outbox', 
        'all' : 'All', 
        'unprocessed' : 'Unprocessed', 
        'accepted' : 'Accepted', 
        'rejected' : 'Rejected', 
        'completed' : 'Completed', 
        'expired' : 'Expired', 
        'revoked' : 'Revoked', 
        'processing' : 'Processing'
    }, 

    'button' : {
        'assign' : 'Task assign', 
        'select' : 'Select', 
        'revoke' : 'Revoke', 
        'pending' : 'Pending', 
        'accept' : 'Accept', 
        'reject' : 'Reject', 
        'completed' : 'Completed', 
        'processing' : 'Processing', 
        'detail' : 'Detail'
        }, 

    'tips' : {
        'confirm' : {
            'revoke' : 'I am sure to revoke the task', 
            'accept' : 'I have read the detailed task content and confirmed accept', 
            'reject' : 'I have read the detailed task content and confirmed reject', 
            'completed' : 'I have reviewed the task and confirmed complete'
            }, 
        'value' : {
            'content' : 'Maximum is 500 characters', 
            'cause' : 'Please enter the cause for revoke (required)<br />Maximum is 200 characters', 
            'reason' : 'Please enter the reason for reject (required)<br />Maximum is 200 characters', 
            'remark' : 'Please enter a remark (optional)<br />Maximum is 200 characters'
        }, 
        'input' : {
            'id' : 'Id error or illegal operation', 
            'content' : 'Content empty or length exceeds limit', 
            'cause' : 'Cause empty or length exceeds limit', 
            'reason' : 'Reason empty or length exceeds limit', 
            'remark' : 'Remark empty or length exceeds limit'
            }, 
        'please-select-user' : 'Please select user', 
        'deadline-time-error' : 'Deadline cannot be earlier than the current time', 
        'unexist-or-error' : 'Data does not exist or is wrong', 
        'operation-failed' : 'Operation failed'
        }, 

    'data' : {
        'level' : {
            'normal' : 'Normal', 
            'important' : 'Important', 
            'urgent' : 'Urgent'
            }, 

        'status' : {
            'unprocessed' : 'Unprocessed', 
            'accepted' : 'Accepted', 
            'rejected' : 'Rejected', 
            'completed' : 'Completed', 
            'expired' : 'Expired', 
            'revoked' : 'Revoked'
            }, 

        'deadline' : {
            'week' : {
                'monday' : 'Monday', 
                'tuesday' : 'Tuesday', 
                'wednesday' : 'Wednesday', 
                'thursday' : 'Thursday', 
                'friday' : 'Friday', 
                'saturday' : 'Saturday', 
                'sunday' : 'Sunday'
                }, 

            'time' : {
                'day' : 'Days', 
                'hour' : 'Hours', 
                'minute' : 'Minutes', 
                'second' : 'Seconds'
                }
            }
        }
    };


lang.log = {
    'folder' : 'Folder', 
    'file' : 'File', 
    'version' : 'Version', 
    'user' : 'User', 
    'action' : 'Action', 
    'time' : 'Time', 
    'unlimited' : 'Unlimited', 
    'time-start' : 'Start', 
    'time-end' : 'End', 
    
    'menu' : {
        'all' : 'All', 
        'created' : 'My created', 
        'shared' : 'Shared with me'
    }, 

    'button' : {
        'filter' : 'Filter', 
        'query' : 'Query'
        }, 

    'tips' : {
        'value' : {
            'keyword' : 'Username, operation data'
        }, 
        'input' : {
            'time-start' : 'Start time is empty or format error', 
            'time-end' : 'End time is empty or format error'
            }
        }
    };


lang.login = {
    'login-id' : 'Username / Email / Phone', 
    'password' : 'Password', 
    'forgot-password' : 'Forgot password', 
    'ad-user-bind' : 'AD domain user account binding', 

    'button' : {
        'login' : 'Login'
        }, 

    'tips' : {
        'input' : {
            'login-id' : 'Username / Email / Phone is empty or format error', 
            'password' : 'Password is empty or format error'
            }, 
        'login-failure' : 'User does not exist or the login password is wrong', 
        'login-lock-ip' : 'Re-login is not allowed for 20 minutes', 
        'logged-in' : 'This user is logged in! Please close your browser and try again', 
        'operation-failed' : 'Operation failed', 
        'html5-browser-note' : 'The current browser is too old, please use a modern browser that supports HTML5.'
        }
    };


lang.main = {
    'tab' : {
        'explorer' : 'File manage', 
        'task' : 'Task', 
        'discuss' : 'Discuss', 
        'activity' : 'Activity', 
        'recycle' : 'Recycle bin', 
        'admin' : 'Management panel', 
        'refresh' : 'Refresh'
        }, 

    'tool' : {
        'sync' : 'Sync tool', 
        'help' : 'User help', 
        'about' : 'About', 
        'profile' : 'Profile', 
        'logout' : 'Logout'
        }, 

    'about' : {
        'version' : 'Version', 
        'license' : 'License', 
        'website' : 'Website'
        }
    };


lang.admin = {
    'menu' : {
        'statistic' : 'Statistic', 
        'department' : 'Department', 
        'role' : 'Role', 
        'user' : 'User', 
        'user-list' : 'User list', 
        'user-recycle' : 'User recycle bin', 
        'log' : 'Log', 
        'config' : 'Config'
        }, 

    'statistic' : {
        'user' : 'Users', 
        'folder' : 'Folders', 
        'file' : 'Files', 
        'occupy' : 'Occupied disk', 
        'today' : 'Today', 
        'yesterday' : 'Yesterday', 
        'week' : 'Week', 
        'month' : 'Month', 
        'upload' : 'Upload', 
        'download' : 'Download', 

        tips : {
            'data-exception' : 'Data exception'
            }
        }, 

    'config' : {
        'app-name' : 'App name', 
        'upload-extension' : 'Upload extension', 
        'upload-size' : 'Upload size', 
        'version-count' : 'Version count', 
        'mail-address' : 'Email address', 
        'mail-username' : 'Email username', 
        'mail-password' : 'Email password', 
        'mail-server' : 'Mail server address', 
        'mail-server-port' : 'Mail server port', 
        'mail-server-security' : 'Mail server security', 
        'mail-server-ssl-connection' : 'Enable SSL connection', 

        'button' : {
            'confirm' : 'Confirm'
            }, 

        'tips' : {
            'value' : {
                'upload-extension' : 'Allow upload file extensions\<br \/\>Does not contain the "\."\<br \/\>Line break separated'
                }, 
            'input' : {
                'app-name' : 'App name is empty or format error', 
                'upload-extension' : 'Upload extension is empty or format error', 
                'upload-size' : 'Upload size is empty or format error', 
                'upload-size-range' : 'Upload size value range 1~2048', 
                'version-count' : 'Version count is empty or format error', 
                'version-count-range' : 'Version count value range 50~500', 
                'mail-address' : 'Email address is empty or format error', 
                'mail-username' : 'Email username is empty or format error', 
                'mail-password' : 'Email password is empty or format error', 
                'mail-server' : 'Mail server address is empty or format error', 
                'mail-server-port' : 'Mail server port is empty or format error'
                }, 
            're-login' : 'Login again after 3 seconds', 
            'config-read-failed' : 'Configuration read failed', 
            'operation-failed' : 'Operation failed'
            }
        }
    };


lang.ui = {
    'button' : {
        'confirm' : 'OK', 
        'cancel' : 'Cancel'
        }
    };


lang.list = {
    'no-data' : 'No data', 
    'data-count' : 'Loaded [count] items'
    };


lang.type = {
    'folder' : 'Folder', 
    'word' : 'Word', 
    'excel' : 'Excel', 
    'powerpoint' : 'PowerPoint', 
    'project' : 'Project', 
    'publisher' : 'Publisher', 
    'visio' : 'Visio', 
    'openoffice' : 'OpenOffice', 
    'wps' : 'WPS', 
    'pdf' : 'PDF', 
    'text' : 'Text', 
    'code' : 'Code', 
    'image' : 'Image', 
    'drawing' : 'Drawing', 
    'audio' : 'Audio', 
    'video' : 'Video', 
    'zip' : 'Zip', 
    'other' : 'Other'
    };


lang.action = {
    'upload' : 'Upload', 
    'download' : 'Download', 
    'add' : 'Add', 
    'modify' : 'Modify', 
    'edit' : 'Edit', 
    'view' : 'View', 
    'rename' : 'Rename', 
    'remark' : 'Remark', 
    'copy' : 'Copy', 
    'move' : 'Move', 
    'remove' : 'Remove', 
    'restore' : 'Restore', 
    'delete' : 'Delete', 
    'upversion' : 'Upload new version', 
    'replace' : 'Replace version', 
    'lock' : 'Lock', 
    'unlock' : 'Unlock', 
    'share' : 'Share', 
    'unshare' : 'Unshare', 
    'sync' : 'Sync', 
    'unsync' : 'Unsync'
    };


lang.size = {
    '0kb-100kb': '0KB~100KB', 
    '100kb-500kb': '100~500KB', 
    '500kb-1mb': '500KB~1MB', 
    '1mb-5mb': '1MB~5MB', 
    '5mb-10mb': '5MB~10MB', 
    '10mb-50mb': '10MB~50MB', 
    '50mb-100mb': '50MB~100MB', 
    '100mb-more': '100MB or more'
    };


lang.timestamp = {
    'this-day' : 'This day', 
    '1-day-ago' : '1 Days ago', 
    '2-day-ago' : '2 Days ago', 
    'this-week' : 'This week', 
    '1-week-ago' : '1 Weeks ago', 
    '2-week-ago' : '2 Weeks ago', 
    'this-month' : 'This month', 
    '1-month-ago' : '1 Months ago', 
    '2-month-ago' : '2 Months ago', 
    'this-year' : 'This year', 
    '1-year-ago' : '1 Years ago', 
    '2-year-ago' : '2 Years ago'
    };


lang.timespan = {
    'second' : 'Seconds ago', 
    'minute' : 'Minutes ago', 
    'hour' : 'Hours ago', 
    'day' : 'Days ago', 
    'month' : 'Months ago', 
    'year' : 'Years ago'
    };