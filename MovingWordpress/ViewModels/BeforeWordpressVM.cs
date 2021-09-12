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

        #region [MySQLSetting]プロパティ
        /// <summary>
        /// [MySQLSetting]プロパティ用変数
        /// </summary>
        MySqlSettingM _MySQLSetting = new MySqlSettingM();
        /// <summary>
        /// [MySQLSetting]プロパティ
        /// </summary>
        public MySqlSettingM MySQLSetting
        {
            get
            {
                return _MySQLSetting;
            }
            set
            {
                if (_MySQLSetting == null || !_MySQLSetting.Equals(value))
                {
                    _MySQLSetting = value;
                    NotifyPropertyChanged("MySQLSetting");
                }
            }
        }
        #endregion

        #region フォルダ設定[FolderSetting]プロパティ
        /// <summary>
        /// フォルダ設定[FolderSetting]プロパティ用変数
        /// </summary>
        FolderSettingM _FolderSetting = new FolderSettingM();
        /// <summary>
        /// フォルダ設定[FolderSetting]プロパティ
        /// </summary>
        public FolderSettingM FolderSetting
        {
            get
            {
                return _FolderSetting;
            }
            set
            {
                if (_FolderSetting == null || !_FolderSetting.Equals(value))
                {
                    _FolderSetting = value;
                    NotifyPropertyChanged("FolderSetting");
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
                this.SSHConnection.Load();
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
                this.SSHConnection.Save();
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

                Task.Run(() =>
                {
                    // SCPによるダウンロード
                    this.SSHConnection.SCPDownload($"/tmp/{_PluginsGz}",
                        this.FolderSetting.LocalDirectory, ScpClient_Downloading_plugin);
                    // SCPによるダウンロード
                    this.SSHConnection.SCPDownload($"/tmp/{_ThemesGz}",
                        this.FolderSetting.LocalDirectory, ScpClient_Downloading_themes);
                    // SCPによるダウンロード
                    this.SSHConnection.SCPDownload($"/tmp/{_UploadGz}",
                        this.FolderSetting.LocalDirectory, ScpClient_Downloading_upload);
                    // SCPによるダウンロード
                    this.SSHConnection.SCPDownload($"/tmp/{_DumpSqlGz}",
                        this.FolderSetting.LocalDirectory, ScpClient_Downloading_sql);
                }
                );
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
                this.Message += this.SSHConnection.SshCommand("cd " + this.FolderSetting.RemoteDirectory + ";" + $"tar zcvf /tmp/{_UploadGz} uploads;");
                this.Message += this.SSHConnection.SshCommand("cd " + this.FolderSetting.RemoteDirectory + ";" + $"tar zcvf /tmp/{_PluginsGz} plugins;");
                this.Message += this.SSHConnection.SshCommand("cd " + this.FolderSetting.RemoteDirectory + ";" + $"tar zcvf /tmp/{_ThemesGz} themes;");
                this.Message += this.SSHConnection.SshCommand($"mysqldump -u {this.MySQLSetting.MySQLUserID} -p{this.MySQLSetting.MySQLPassword} -h localhost bitnami_wordpress | gzip > /tmp/{_DumpSqlGz}");
                this.Message += this.SSHConnection.SshCommand("cd " + this.FolderSetting.RemoteDirectory + ";" + $"cd /tmp/;ls -lh;");

            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        public void FolderSelectDialog()
        {

        }


        private void ScpClient_Downloading_plugin(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() => {
                   this.DownloadProgress_plugin = $" FileName = {e.Filename} Size => {e.Downloaded.ToString()} / {e.Size.ToString()}";
               }));
        }

        private void ScpClient_Downloading_upload(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() => {
                   this.DownloadProgress_upload = $" FileName = {e.Filename} Size => {e.Downloaded.ToString()} / {e.Size.ToString()}";
               }));
        }

        private void ScpClient_Downloading_themes(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() => {
                   this.DownloadProgress_themes = $" FileName = {e.Filename} Size => {e.Downloaded.ToString()} / {e.Size.ToString()}";
               }));
        }

        private void ScpClient_Downloading_sql(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() => {
                   this.DownloadProgress_sql = $" FileName = {e.Filename} Size => {e.Downloaded.ToString()} / {e.Size.ToString()}";
               }));
        }
    }
}
