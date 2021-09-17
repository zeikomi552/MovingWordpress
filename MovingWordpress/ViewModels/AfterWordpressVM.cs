using MovingWordpress.Models;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.ViewModels
{
    public class AfterWordpressVM : BaseWordpressVM
    {

        /// <summary>
        /// アップロード用のメッセージ(一時保存用)
        /// </summary>
        StringBuilder _UploadTemporaryMessage = new StringBuilder();

        #region アップロードの進行状況[DownloadProgress_plugin]プロパティ
        /// <summary>
        /// アップロードの進行状況[DownloadProgress_plugin]プロパティ用変数
        /// </summary>
        string _UploadProgress_plugin = string.Empty;
        /// <summary>
        /// アップロードの進行状況[DownloadProgress_plugin]プロパティ
        /// </summary>
        public string DownloadProgress_plugin
        {
            get
            {
                return _UploadProgress_plugin;
            }
            set
            {
                if (_UploadProgress_plugin == null || !_UploadProgress_plugin.Equals(value))
                {
                    _UploadProgress_plugin = value;
                    NotifyPropertyChanged("DownloadProgress_plugin");
                }
            }
        }
        #endregion

        #region アップロードの進行状況[DownloadProgress_themes]プロパティ
        /// <summary>
        /// アップロードの進行状況[DownloadProgress_themes]プロパティ用変数
        /// </summary>
        string _UploadProgress_themes = string.Empty;
        /// <summary>
        /// アップロードの進行状況[DownloadProgress_themes]プロパティ
        /// </summary>
        public string DownloadProgress_themes
        {
            get
            {
                return _UploadProgress_themes;
            }
            set
            {
                if (_UploadProgress_themes == null || !_UploadProgress_themes.Equals(value))
                {
                    _UploadProgress_themes = value;
                    NotifyPropertyChanged("DownloadProgress_themes");
                }
            }
        }
        #endregion

        #region アップロードの進行状況[DownloadProgress_upload]プロパティ
        /// <summary>
        /// アップロードの進行状況[DownloadProgress_upload]プロパティ用変数
        /// </summary>
        string _UploadProgress_upload = string.Empty;
        /// <summary>
        /// アップロードの進行状況[DownloadProgress_upload]プロパティ
        /// </summary>
        public string DownloadProgress_upload
        {
            get
            {
                return _UploadProgress_upload;
            }
            set
            {
                if (_UploadProgress_upload == null || !_UploadProgress_upload.Equals(value))
                {
                    _UploadProgress_upload = value;
                    NotifyPropertyChanged("DownloadProgress_upload");
                }
            }
        }
        #endregion

        #region アップロードの進行状況[DownloadProgress_sql]プロパティ
        /// <summary>
        /// アップロードの進行状況[DownloadProgress_sql]プロパティ用変数
        /// </summary>
        string _UploadProgress_sql = string.Empty;
        /// <summary>
        /// アップロードの進行状況[DownloadProgress_sql]プロパティ
        /// </summary>
        public string DownloadProgress_sql
        {
            get
            {
                return _UploadProgress_sql;
            }
            set
            {
                if (_UploadProgress_sql == null || !_UploadProgress_sql.Equals(value))
                {
                    _UploadProgress_sql = value;
                    NotifyPropertyChanged("DownloadProgress_sql");
                }
            }
        }
        #endregion

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Init()
        {
            try
            {
                // 引っ越し後のパラメータを取得するように設定
                this.SSHConnection.Initialize(false);

                // 各種ファイルのロード処理を行う
                this.SSHConnection.Load();

            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

        public void ExecuteSsh()
        {
            try
            {
                // 初期化処理
                this.SSHConnection.CreateConnection();

                Task.Run(() =>
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine($"====== Command Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    string cmd = "cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"tar zcvf /tmp/{_UploadGz} uploads;";
                    ExecuteCommand(message, cmd);

                    cmd = "cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"tar zcvf /tmp/{_PluginsGz} plugins;";
                    ExecuteCommand(message, cmd);

                    cmd = "cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"tar zcvf /tmp/{_ThemesGz} themes;";
                    ExecuteCommand(message, cmd);

                    cmd = $"mysqldump -u {this.SSHConnection.MySQLSetting.MySQLUserID} -p{this.SSHConnection.MySQLSetting.MySQLPassword} -h localhost bitnami_wordpress | gzip > /tmp/{_DumpSqlGz}";
                    ExecuteCommand(message, cmd);

                    cmd = "cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"cd /tmp/;ls -lh;";
                    ExecuteCommand(message, cmd);

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


        #region SCPによるアップロードの実行
        /// <summary>
        /// SCPによるアップロードの実行
        /// </summary>
        public void ExecuteScp()
        {
            try
            {
                // 初期化処理
                this.SSHConnection.CreateConnection();

                string local_dir = this.SSHConnection.FolderSetting.LocalDirectory;

                Task.Run(() =>
                {
                    this._UploadTemporaryMessage.AppendLine($"====== アップロード Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    this._UploadTemporaryMessage.AppendLine($"{local_dir} ------> /tmp/{_UploadGz} 計:4ファイル");

                    // メッセージの更新
                    UpdateMessage(this._UploadTemporaryMessage.ToString());

                    // SCPによるアップロード
                    this.SSHConnection.SCPUpload($"/tmp/" + _UploadGz,
                        local_dir + _UploadGz, ScpClient_Uploading_upload);

                    this._UploadTemporaryMessage.AppendLine(this.DownloadProgress_upload);
                    // メッセージの更新
                    UpdateMessage(this._UploadTemporaryMessage.ToString());

                    // SCPによるアップロード
                    this.SSHConnection.SCPUpload($"/tmp/{_PluginsGz}",
                        local_dir + _PluginsGz, ScpClient_Uploading_plugin);

                    this._UploadTemporaryMessage.AppendLine(this.DownloadProgress_plugin);
                    // メッセージの更新
                    UpdateMessage(this._UploadTemporaryMessage.ToString());

                    // SCPによるアップロード
                    this.SSHConnection.SCPUpload($"/tmp/{_ThemesGz}",
                        local_dir + _ThemesGz, ScpClient_Uploading_themes);

                    this._UploadTemporaryMessage.AppendLine(this.DownloadProgress_themes);
                    // メッセージの更新
                    UpdateMessage(this._UploadTemporaryMessage.ToString());

                    // SCPによるアップロード
                    this.SSHConnection.SCPUpload($"/tmp/{_DumpSqlGz}",
                        local_dir + _DumpSqlGz, ScpClient_Uploading_sql);

                    this._UploadTemporaryMessage.AppendLine(this.DownloadProgress_sql);

                    // メッセージの更新
                    UpdateMessage(this._UploadTemporaryMessage.ToString());

                    this._UploadTemporaryMessage.AppendLine($"====== アップロード End {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    // メッセージの更新
                    UpdateMessage(this._UploadTemporaryMessage.ToString());
                }
                );
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region plugins.tar.gzフォルダのアップロード進捗
        /// <summary>
        /// plugins.tar.gzフォルダのアップロード進捗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScpClient_Uploading_plugin(object sender, Renci.SshNet.Common.ScpUploadEventArgs e)
        {
            this.DownloadProgress_plugin = $" FileName = {e.Filename} Size => {e.Uploaded.ToString()} / {e.Size.ToString()} ({(int)(e.Uploaded / (double)e.Size * 100)}%)";
            // メッセージの更新
            UpdateMessage(this._UploadTemporaryMessage.ToString() + this.DownloadProgress_plugin);
        }
        #endregion


        #region uploads.tar.gzのアップロード進捗
        /// <summary>
        /// uploads.tar.gzのアップロード進捗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScpClient_Uploading_upload(object sender, Renci.SshNet.Common.ScpUploadEventArgs e)
        {
            this.DownloadProgress_upload = $" FileName = {e.Filename} Size => {e.Uploaded.ToString()} / {e.Size.ToString()} ({(int)(e.Uploaded / (double)e.Size * 100)}%)";
            // メッセージの更新
            UpdateMessage(this._UploadTemporaryMessage.ToString() + this.DownloadProgress_upload);
        }
        #endregion

        #region themes.tar.gzフォルダのアップロード進捗
        /// <summary>
        /// themes.tar.gzフォルダのアップロード進捗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScpClient_Uploading_themes(object sender, Renci.SshNet.Common.ScpUploadEventArgs e)
        {
            this.DownloadProgress_themes = $" FileName = {e.Filename} Size => {e.Uploaded.ToString()} / {e.Size.ToString()} ({(int)(e.Uploaded / (double)e.Size * 100)}%)";
            // メッセージの更新
            UpdateMessage(this._UploadTemporaryMessage.ToString() + this.DownloadProgress_themes);
        }
        #endregion

        #region dump.sql.gzのアップロード進捗
        /// <summary>
        /// dump.sql.gzのアップロード進捗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScpClient_Uploading_sql(object sender, Renci.SshNet.Common.ScpUploadEventArgs e)
        {
            this.DownloadProgress_sql = $" FileName = {e.Filename} Size => {e.Uploaded.ToString()} / {e.Size.ToString()} ({(int)(e.Uploaded / (double)e.Size * 100)}%)";
            // メッセージの更新
            UpdateMessage(this._UploadTemporaryMessage.ToString() + this.DownloadProgress_sql);
        }
        #endregion
    }
}
