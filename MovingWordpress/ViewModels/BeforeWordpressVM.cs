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
                _logger.Error(e.Message);
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
                // リモートディレクトリの設定がおこなわれているかを確認
                if (string.IsNullOrWhiteSpace(this.SSHConnection.FolderSetting.RemoteDirectory))
                {
                    ShowMessage.ShowNoticeOK("リモートパスの設定が行われていません。", "通知");
                }
                else
                {
                    StringBuilder message = new StringBuilder();
                    ExecuteCommandList(@"CommandFiles\before_compress.mw", "荷づくり", message);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
            finally
            {
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
                StringBuilder message = new StringBuilder();
                ExecuteCommandList(@"CommandFiles\before_cleanup.mw", "後片付け", message);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
            finally
            {
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
        public void ExecuteDownload()
        {
            try
            {
                this.IsExecute = true;
                this._DownloadTemporaryMessage.Clear();

                // 初期化処理
                this.SSHConnection.CreateConnection();

                string local_dir = this.SSHConnection.FolderSetting.LocalDirectory;
                string remote_dir = "/tmp";

                Task.Run(() =>
                {
                    // SFTPでファイル一覧の取得
                    var list = this.SSHConnection.GetFileList(remote_dir);

                    this.LogMessage.UpdateMessage($"====== ダウンロード Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    this.LogMessage.AppendMessage($"====== 取得ファイルサイズの確認 ======");

                    foreach (var file in list)
                    {
                        if (file.FullName.Equals(remote_dir + "/" + _UploadGz) || file.FullName.Equals(remote_dir + "/" + _PluginsGz)
                            || file.FullName.Equals(remote_dir + "/" + _ThemesGz) || file.FullName.Equals(remote_dir + "/" + _DumpSqlGz))
                        {
                            this.LogMessage.AppendMessage($" {file.Name} : Size->{file.Length.ToString()}");
                        }
                    }

                    this.LogMessage.AppendMessage($"====== ファイルダウンロード ======");
                    foreach (var file in list)
                    {
                        if (file.FullName.Equals(remote_dir + "/" + _UploadGz) || file.FullName.Equals(remote_dir + "/" + _PluginsGz)
                        || file.FullName.Equals(remote_dir + "/" + _ThemesGz) || file.FullName.Equals(remote_dir + "/" + _DumpSqlGz))
                        {
                            Download(local_dir, remote_dir, file.Name, (ulong)file.Length);                     // ダウンロード処理
                        }
                    }

                    // メッセージの更新
                    this.LogMessage.AppendMessage($"====== ダウンロード End {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");

                    this.IsExecute = false;
                }
                );
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
                this.IsExecute = false;
            }
        }
        #endregion

        #region ダウンロード処理
        /// <summary>
        /// ダウンロード処理
        /// </summary>
        /// <param name="local_dir">ローカルディレクトリ</param>
        /// <param name="remote_dir">リモートディレクトリ</param>
        /// <param name="file_name">ファイル名</param>
        /// <param name="scp_f">true:SCP false:SFTP</param>
        private void Download(string local_dir, string remote_dir, string file_name, ulong max_size, bool scp_f = false)
        {
            string remote_file_path = $"{remote_dir}/{file_name}";              // リモートファイルパスの作成
            string local_file_path = Path.Combine(local_dir, $"{file_name}");   // ローカルファイルパスの作成

            Action<ulong> del_func_inst = delegate (ulong a) {
                this.LogMessage.AppendMessage($"FileName = {file_name}  Size => {a.ToString()} / {max_size.ToString()} ({(ulong)(a / (double)max_size * 100)}%) ======", false);
            };

            // ダウンロード処理の実行
            this.SSHConnection.Download(remote_file_path, local_file_path, ScpClient_Downloading_upload, del_func_inst, scp_f);

            // メッセージの確定
            this.LogMessage.CommitMessage();
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
            this.LogMessage.AppendMessage($" FileName = {e.Filename} Size => {e.Downloaded.ToString()} / {e.Size.ToString()} ({(int)(e.Downloaded / (double)e.Size * 100)}%)", false);
        }
        #endregion
    }
}
