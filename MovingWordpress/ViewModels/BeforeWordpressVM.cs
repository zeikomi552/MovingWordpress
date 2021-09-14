using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MovingWordpress.Models;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MovingWordpress.ViewModels
{
    public class BeforeWordpressVM : ViewModelBase
    {
        #region SSH接続オブジェクト[SSHConnection]プロパティ
        /// <summary>
        /// SSH接続オブジェクト[SSHConnection]プロパティ用変数
        /// </summary>
        SSHManagerM _SSHConnection = new SSHManagerM();
        /// <summary>
        /// SSH接続オブジェクト[SSHConnection]プロパティ
        /// </summary>
        public SSHManagerM SSHConnection
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

        #region 実行中フラグ(true:実行中 false:停止中)[IsExecute]プロパティ
        /// <summary>
        /// 実行中フラグ(true:実行中 false:停止中)[IsExecute]プロパティ用変数
        /// </summary>
        bool _IsExecute = false;
        /// <summary>
        /// 実行中フラグ(true:実行中 false:停止中)[IsExecute]プロパティ
        /// </summary>
        public bool IsExecute
        {
            get
            {
                return _IsExecute;
            }
            set
            {
                if (!_IsExecute.Equals(value))
                {
                    _IsExecute = value;
                    NotifyPropertyChanged("IsExecute");
                }
            }
        }
        #endregion

        #region ダウンロードの進行状況[DownloadProgress_plugin]プロパティ
        /// <summary>
        /// ダウンロードの進行状況[DownloadProgress_plugin]プロパティ用変数
        /// </summary>
        string _DownloadProgress_plugin = string.Empty;
        /// <summary>
        /// ダウンロードの進行状況[DownloadProgress_plugin]プロパティ
        /// </summary>
        public string DownloadProgress_plugin
        {
            get
            {
                return _DownloadProgress_plugin;
            }
            set
            {
                if (_DownloadProgress_plugin == null || !_DownloadProgress_plugin.Equals(value))
                {
                    _DownloadProgress_plugin = value;
                    NotifyPropertyChanged("DownloadProgress_plugin");
                }
            }
        }
        #endregion

        #region ダウンロードの進行状況[DownloadProgress_themes]プロパティ
        /// <summary>
        /// ダウンロードの進行状況[DownloadProgress_themes]プロパティ用変数
        /// </summary>
        string _DownloadProgress_themes = string.Empty;
        /// <summary>
        /// ダウンロードの進行状況[DownloadProgress_themes]プロパティ
        /// </summary>
        public string DownloadProgress_themes
        {
            get
            {
                return _DownloadProgress_themes;
            }
            set
            {
                if (_DownloadProgress_themes == null || !_DownloadProgress_themes.Equals(value))
                {
                    _DownloadProgress_themes = value;
                    NotifyPropertyChanged("DownloadProgress_themes");
                }
            }
        }
        #endregion

        #region ダウンロードの進行状況[DownloadProgress_upload]プロパティ
        /// <summary>
        /// ダウンロードの進行状況[DownloadProgress_upload]プロパティ用変数
        /// </summary>
        string _DownloadProgress_upload = string.Empty;
        /// <summary>
        /// ダウンロードの進行状況[DownloadProgress_upload]プロパティ
        /// </summary>
        public string DownloadProgress_upload
        {
            get
            {
                return _DownloadProgress_upload;
            }
            set
            {
                if (_DownloadProgress_upload == null || !_DownloadProgress_upload.Equals(value))
                {
                    _DownloadProgress_upload = value;
                    NotifyPropertyChanged("DownloadProgress_upload");
                }
            }
        }
        #endregion

        #region ダウンロードの進行状況[DownloadProgress_sql]プロパティ
        /// <summary>
        /// ダウンロードの進行状況[DownloadProgress_sql]プロパティ用変数
        /// </summary>
        string _DownloadProgress_sql = string.Empty;
        /// <summary>
        /// ダウンロードの進行状況[DownloadProgress_sql]プロパティ
        /// </summary>
        public string DownloadProgress_sql
        {
            get
            {
                return _DownloadProgress_sql;
            }
            set
            {
                if (_DownloadProgress_sql == null || !_DownloadProgress_sql.Equals(value))
                {
                    _DownloadProgress_sql = value;
                    NotifyPropertyChanged("DownloadProgress_sql");
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

        #region 初期化処理
        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Init()
        {
            try
            {
                this.SSHConnection.Load();
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 設定ファイル保存処理
        /// <summary>
        /// 設定ファイル保存処理
        /// </summary>
        public void SaveSetting()
        {
            try
            {
                this.SSHConnection.Save();
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        string _DumpSqlGz = "dump.sql.gz";
        string _UploadGz = "uploads.tar.gz";
        string _PluginsGz = "plugins.tar.gz";
        string _ThemesGz = "themes.tar.gz";



        /// <summary>
        /// メッセージ更新処理
        /// </summary>
        /// <param name="message"></param>
        private void UpdateMessage(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() =>
               {
                   this.Message = message.ToString();
               }));

        }


        /// <summary>
        /// コマンド実行処理
        /// </summary>
        /// <param name="cmd"></param>
        private void ExecuteCommand(string cmd)
        {
            try
            {
                // 初期化処理
                this.SSHConnection.Initialize();

                Task.Run(() =>
                {
                    StringBuilder message = new StringBuilder();

                    // コマンド開始のメモ
                    message.AppendLine($"====== Command Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");

                    // コマンド内容のセット
                    message.AppendLine("Command ==> " + cmd);

                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.AppendLine(this.SSHConnection.SshCommand(cmd));
                    message.Append($"====== Command End {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");

                    // メッセージの更新
                    UpdateMessage(message.ToString());
                }
                );
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

        #region バックアップ用のディレクトリを探す
        /// <summary>
        /// バックアップ用のディレクトリを探す
        /// </summary>
        public void SearchDir()
        {
            try
            {
                // コマンド
                string cmd = "find /opt -name wp-content;";

                // コマンドの発行処理
                ExecuteCommand(cmd);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region ワードプレスのパスワードおよびユーザー名の確認コマンド
        /// <summary>
        /// ワードプレスのパスワードおよびユーザー名の確認コマンド
        /// </summary>
        public void CheckWordpressUserPassword()
        {
            try
            {
                // コマンド
                string cmd = "sudo cat /home/bitnami/bitnami_credentials;";

                // コマンドの発酵処理
                ExecuteCommand(cmd);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        public void CheckMySQLPassword()
        {
            try
            {
                // コマンド
                string cmd = $"cd {this.SSHConnection.FolderSetting.RemoteDirectory};cd ..;cat wp-config.php;";

                // コマンドの発酵処理
                ExecuteCommand(cmd);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

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


                Task.Run(() =>
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine($"====== Command Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    // SSHによるコマンド実行
                    message.AppendLine(this.SSHConnection.SshCommand("cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"tar zcvf /tmp/{_UploadGz} uploads;"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.AppendLine(this.SSHConnection.SshCommand("cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"tar zcvf /tmp/{_PluginsGz} plugins;"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.AppendLine(this.SSHConnection.SshCommand("cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"tar zcvf /tmp/{_ThemesGz} themes;"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.AppendLine(this.SSHConnection.SshCommand($"mysqldump -u {this.SSHConnection.MySQLSetting.MySQLUserID} -p{this.SSHConnection.MySQLSetting.MySQLPassword} -h localhost bitnami_wordpress | gzip > /tmp/{_DumpSqlGz}"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.AppendLine(this.SSHConnection.SshCommand("cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"cd /tmp/;ls -lh;"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.Append($"====== Command End {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                }
                );

            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        /// <summary>
        /// 後片付け実行処理
        /// </summary>
        public void ExecuteSshClearn()
        {
            try
            {
                // 初期化処理
                this.SSHConnection.Initialize();


                Task.Run(() =>
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine($"====== 後片付け Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    // SSHによるコマンド実行
                    message.AppendLine(this.SSHConnection.SshCommand("cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"rm -f /tmp/{_UploadGz};"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.AppendLine(this.SSHConnection.SshCommand("cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"rm -f /tmp/{_PluginsGz};"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.AppendLine(this.SSHConnection.SshCommand("cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"rm -f /tmp/{_ThemesGz};"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.AppendLine(this.SSHConnection.SshCommand("cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"rm -f /tmp/{_DumpSqlGz};"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.AppendLine(this.SSHConnection.SshCommand("cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"cd /tmp;ls -lh;"));
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    message.Append($"====== 後片付け End {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    // メッセージの更新
                    UpdateMessage(message.ToString());
                }
                );

            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

        /// <summary>
        /// ダウンロード用のメッセージ(一時保存用)
        /// </summary>
        StringBuilder _DownloadTemporaryMessage = new StringBuilder();

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

                string local_dir = this.SSHConnection.FolderSetting.LocalDirectory;

                Task.Run(() =>
                {
                    this._DownloadTemporaryMessage.AppendLine($"====== ダウンロード Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    this._DownloadTemporaryMessage.AppendLine($"/tmp/{_UploadGz} ------> {local_dir} 計:4ファイル");

                    // メッセージの更新
                    UpdateMessage(this._DownloadTemporaryMessage.ToString());

                    // SCPによるダウンロード
                    this.SSHConnection.SCPDownload($"/tmp/{_UploadGz}",
                        local_dir, ScpClient_Downloading_upload);

                    this._DownloadTemporaryMessage.AppendLine(this.DownloadProgress_upload);
                    // メッセージの更新
                    UpdateMessage(this._DownloadTemporaryMessage.ToString());

                    // SCPによるダウンロード
                    this.SSHConnection.SCPDownload($"/tmp/{_PluginsGz}",
                        local_dir, ScpClient_Downloading_plugin);

                    this._DownloadTemporaryMessage.AppendLine(this.DownloadProgress_plugin);
                    // メッセージの更新
                    UpdateMessage(this._DownloadTemporaryMessage.ToString());

                    // SCPによるダウンロード
                    this.SSHConnection.SCPDownload($"/tmp/{_ThemesGz}",
                        local_dir, ScpClient_Downloading_themes);

                    this._DownloadTemporaryMessage.AppendLine(this.DownloadProgress_themes);
                    // メッセージの更新
                    UpdateMessage(this._DownloadTemporaryMessage.ToString());

                    // SCPによるダウンロード
                    this.SSHConnection.SCPDownload($"/tmp/{_DumpSqlGz}",
                        local_dir, ScpClient_Downloading_sql);

                    this._DownloadTemporaryMessage.AppendLine(this.DownloadProgress_sql);

                    // メッセージの更新
                    UpdateMessage(this._DownloadTemporaryMessage.ToString());

                    this._DownloadTemporaryMessage.AppendLine($"====== ダウンロード End {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    // メッセージの更新
                    UpdateMessage(this._DownloadTemporaryMessage.ToString());
                }
                );
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion
        #region plugins.tar.gzフォルダのダウンロード進捗
        /// <summary>
        /// plugins.tar.gzフォルダのダウンロード進捗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScpClient_Downloading_plugin(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            this.DownloadProgress_plugin = $" FileName = {e.Filename} Size => {e.Downloaded.ToString()} / {e.Size.ToString()} ({(int)(e.Downloaded/ (double)e.Size * 100)}%)";
            // メッセージの更新
            UpdateMessage(this._DownloadTemporaryMessage.ToString() + this.DownloadProgress_plugin);
        }
        #endregion


        #region uploads.tar.gzのダウンロード進捗
        /// <summary>
        /// uploads.tar.gzのダウンロード進捗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScpClient_Downloading_upload(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            this.DownloadProgress_upload = $" FileName = {e.Filename} Size => {e.Downloaded.ToString()} / {e.Size.ToString()} ({(int)(e.Downloaded / (double)e.Size * 100)}%)";
            // メッセージの更新
            UpdateMessage(this._DownloadTemporaryMessage.ToString() + this.DownloadProgress_upload);
        }
        #endregion

        #region themes.tar.gzフォルダのダウンロード進捗
        /// <summary>
        /// themes.tar.gzフォルダのダウンロード進捗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScpClient_Downloading_themes(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            this.DownloadProgress_themes = $" FileName = {e.Filename} Size => {e.Downloaded.ToString()} / {e.Size.ToString()} ({(int)(e.Downloaded / (double)e.Size * 100)}%)";
            // メッセージの更新
            UpdateMessage(this._DownloadTemporaryMessage.ToString() + this.DownloadProgress_themes);
        }
        #endregion

        #region dump.sql.gzのダウンロード進捗
        /// <summary>
        /// dump.sql.gzのダウンロード進捗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScpClient_Downloading_sql(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            this.DownloadProgress_sql = $" FileName = {e.Filename} Size => {e.Downloaded.ToString()} / {e.Size.ToString()} ({(int)(e.Downloaded / (double)e.Size * 100)}%)";
            // メッセージの更新
            UpdateMessage(this._DownloadTemporaryMessage.ToString() + this.DownloadProgress_sql);
        }
        #endregion
    }
}
