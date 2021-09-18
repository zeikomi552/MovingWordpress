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
    public class BeforeWordpressVM : BaseWordpressVM
    {

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

        #region 初期化処理
        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Init()
        {
            try
            {
                // 引っ越し前のパラメータを取得するように設定
                this.SSHConnection.Initialize(true);

                this.SSHConnection.Load();
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
                this.IsExecute = true;
                StringBuilder message = new StringBuilder();
                ExecuteCommandList(@"CommandFiles\before_compress.mw", "荷づくり", message);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
            finally
            {
                this.IsExecute = false;
            }
        }
        #endregion

        #region 後片付け実行処理
        /// <summary>
        /// 後片付け実行処理
        /// </summary>
        public void ExecuteSshClearn()
        {
            try
            {
                this.IsExecute = true;
                StringBuilder message = new StringBuilder();
                ExecuteCommandList(@"CommandFiles\before_cleanup.mw", "後片付け", message);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
            finally
            {
                this.IsExecute = false;
            }
        }
        #endregion

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
                this.IsExecute = true;
                // 初期化処理
                this.SSHConnection.CreateConnection();

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

                    this.IsExecute = false;
                }
                );
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
                this.IsExecute = false;
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
