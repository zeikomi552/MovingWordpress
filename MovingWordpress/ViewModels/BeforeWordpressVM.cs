using Microsoft.Win32;
using MovingWordpress.Models;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.ViewModels
{
    public class BeforeWordpressVM : ViewModelBase
    {
        #region SSH接続オブジェクト[SSHConnection]プロパティ
        /// <summary>
        /// SSH接続オブジェクト[SSHConnection]プロパティ用変数
        /// </summary>
        SSHConnection _SSHConnection = new SSHConnection();
        /// <summary>
        /// SSH接続オブジェクト[SSHConnection]プロパティ
        /// </summary>
        public SSHConnection SSHConnection
        {
            get
            {
                return _SSHConnection;
            }
            set
            {
                if (_SSHConnection == null || !_SSHConnection.Equals(value))
                {
                    _SSHConnection = value;
                    NotifyPropertyChanged("SSHConnection");
                }
            }
        }
        #endregion

        #region 結果メッセージ[Message]プロパティ
        /// <summary>
        /// 結果メッセージ[Message]プロパティ用変数
        /// </summary>
        string _Message = string.Empty;
        /// <summary>
        /// 結果メッセージ[Message]プロパティ
        /// </summary>
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (_Message == null || !_Message.Equals(value))
                {
                    _Message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }
        #endregion

        #region [CommandList]プロパティ
        /// <summary>
        /// [CommandList]プロパティ用変数
        /// </summary>
        CommandListsM _CommandList = new CommandListsM();
        /// <summary>
        /// [CommandList]プロパティ
        /// </summary>
        public CommandListsM CommandList
        {
            get
            {
                return _CommandList;
            }
            set
            {
                if (_CommandList == null || !_CommandList.Equals(value))
                {
                    _CommandList = value;
                    NotifyPropertyChanged("CommandList");
                }
            }
        }
        #endregion


        public void Init()
        {
            try
            {
                // ファイルの存在確認
                if (File.Exists(this.ConfigFile_Path))
                {
                    this.SSHConnection = XMLUtil.Deserialize<SSHConnection>(this.ConfigFile_Path);
                }
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

        /// <summary>
        /// 秘密鍵ファイルを開くダイアログ
        /// </summary>
        public void OpenPemFileDialog()
        {
            try
            {
                // ダイアログのインスタンスを生成
                var dialog = new OpenFileDialog();

                // ファイルの種類を設定
                dialog.Filter = "秘密鍵ファイル (*.pem)|*.pem|全てのファイル (*.*)|*.*";

                // ダイアログを表示する
                if (dialog.ShowDialog() == true)
                {
                    // ファイルパスのセット
                    this.SSHConnection.KeyFilePath = dialog.FileName;
                }
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

        const string ConfigDir = "Config";

        string ConfigFile_Path = Path.Combine(ConfigDir, "Setting.config");

        /// <summary>
        /// 設定ファイル保存処理
        /// </summary>
        public void SaveSetting()
        {
            try
            {
                // フォルダの存在確認
                if (!Directory.Exists(ConfigDir))
                {
                    // フォルダの作成
                    Directory.CreateDirectory(ConfigDir);
                }

                XMLUtil.Seialize<SSHConnection>(this.ConfigFile_Path, this.SSHConnection);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

        /// <summary>
        /// 接続処理
        /// </summary>
        public void Connect()
        {
            try
            {
                this.SSHConnection.Connect();
                string result = this.SSHConnection.sshCommand("cd /opt/bitnami/apps/wordpress/htdocs/wp-content/;ls -lah;");
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

        public void Execute()
        {
            try
            {
                this.SSHConnection.Connect();
                foreach (var tmp in this.CommandList.Commands.Items)
                {
                    tmp.Result = this.SSHConnection.sshCommand(tmp.Command);
                }
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }

        }

        public void ExecuteScp()
        {
            try
            {
                this.SSHConnection.Connect();
                this.SSHConnection.Download("/opt/bitnami/apps/wordpress/htdocs/wp-content/uploads.tar.gz", @"C:\Work\test\20210911\uploads.tar.gz");
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }

        }
    }
}
