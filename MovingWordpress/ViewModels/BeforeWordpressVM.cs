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


        string _DumpSqlGz = "dump.sql.gz";
        string _UploadGz = "uploads.tar.gz";
        string _PluginsGz = "plugins.tar.gz";
        string _ThemesGz = "themes.tar.gz";

        #region SCPによるダウンロードの実行
        /// <summary>
        /// SCPによるダウンロードの実行
        /// </summary>
        public void ExecuteScp()
        {
            try
            {
                // 初期化処理
                this.SSHConnection.Initialize();

                // SCPによるダウンロード
                this.SSHConnection.SCPDownload($"/tmp/{_PluginsGz}",
                    this.SSHConnection.LocalDirectory, ScpClient_Downloading);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region SSHによるコマンドの実行
        /// <summary>
        /// SSHによるコマンドの実行
        /// </summary>
        public void ExecuteSsh()
        {
            try
            {
                // 初期化処理
                this.SSHConnection.Initialize();

                // SSHによるコマンド実行
                this.Message += this.SSHConnection.SshCommand("cd " + this.SSHConnection.RemoteDirectory + ";" + $"tar zcvf /tmp/{_UploadGz} uploads;");
                this.Message += this.SSHConnection.SshCommand("cd " + this.SSHConnection.RemoteDirectory + ";" + $"tar zcvf /tmp/{_PluginsGz} plugins;");
                this.Message += this.SSHConnection.SshCommand("cd " + this.SSHConnection.RemoteDirectory + ";" + $"tar zcvf /tmp/{_ThemesGz} themes;");
                this.Message += this.SSHConnection.SshCommand($"mysqldump -u {this.SSHConnection.MySQLUserID} -p{this.SSHConnection.MySQLPassword} -h localhost bitnami_wordpress | gzip > /tmp/{_DumpSqlGz}");
                this.Message += this.SSHConnection.SshCommand("cd " + this.SSHConnection.RemoteDirectory + ";" + $"cd /tmp/;ls -lh;");

            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion


        private void ScpClient_Downloading(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {

        }
    }
}
