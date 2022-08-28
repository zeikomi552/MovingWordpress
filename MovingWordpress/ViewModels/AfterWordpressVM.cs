﻿using MovingWordpress.Models;
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
    public class AfterWordpressVM : BaseWordpressVM
    {
        #region 初期化処理
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
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region SSHの実行処理
        /// <summary>
        /// SSHの実行処理
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
                    ExecuteCommandList(@"CommandFiles\after_decompress.mw", "荷ほどき", message);
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

        #region SCPによるアップロードの実行
        /// <summary>
        /// SCPによるアップロードの実行
        /// </summary>
        public void ExecuteUpload()
        {
            try
            {
                this.IsExecute = true;
                //this._UploadTemporaryMessage.Clear();

                // 初期化処理
                this.SSHConnection.CreateConnection();

                string local_dir = this.SSHConnection.FolderSetting.LocalDirectory;
                string remote_dir = "/tmp";

                List<string> file_list = new List<string>() {
                    _UploadGz,
                    _PluginsGz,
                    _ThemesGz,
                    _DumpSqlGz
                };
                

                Task.Run(() =>
                {
                    this.LogMessage.UpdateMessage($"====== アップロード Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    this.LogMessage.AppendMessage($"====== アップロードファイルサイズの確認 ======");

                    Dictionary<string, ulong> size_list = new Dictionary<string, ulong>();
                    // ファイルサイズの確認
                    foreach (var file in file_list)
                    {
                        FileInfo fileinf = new FileInfo(Path.Combine(local_dir, file));
                        long size = fileinf.Length;
                        size_list.Add(file, (ulong)size);

                        this.LogMessage.AppendMessage($" {file} : Size->{size}");
                    }

                    // メッセージの更新
                    this.LogMessage.AppendMessage($"====== ファイルアップロード ======");

                    foreach (var file in file_list)
                    {
                        // SFTPによるアップロード
                        Upload(local_dir, remote_dir, file, size_list[file], false);
                    }

                    this.LogMessage.AppendMessage($"====== アップロード End {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");

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

        #region アップロード処理
        /// <summary>
        /// アップロード処理
        /// </summary>
        /// <param name="local_dir">ローカルディレクトリ</param>
        /// <param name="remote_dir">リモートディレクトリ</param>
        /// <param name="file_name">ファイル名</param>
        /// <param name="max_size">最大サイズ</param>
        /// <param name="scp_f">true:SCP false:SFTP</param>
        private void Upload(string local_dir, string remote_dir, string file_name, ulong max_size, bool scp_f = false)
        {
            string remote_file_path = $"{remote_dir}/{file_name}";              // リモートファイルパスの作成
            string local_file_path = Path.Combine(local_dir, $"{file_name}");   // ローカルファイルパスの作成

            Action<ulong> del_func_inst = delegate (ulong size) {
                this.LogMessage.AppendMessage($"FileName = {file_name}  Size => {size.ToString()} / {max_size.ToString()} ({(ulong)(size / (double)max_size * 100)}%) ======", false);
            };

            // ダウンロード処理の実行
            this.SSHConnection.Upload(remote_file_path, local_file_path, ScpClient_Uploading_upload, del_func_inst, scp_f);

            // メッセージの確定
            this.LogMessage.CommitMessage();
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
                ExecuteCommandList(@"CommandFiles\after_cleanup.mw", "後片付け", message);

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

        #region uploads.tar.gzのアップロード進捗
        /// <summary>
        /// uploads.tar.gzのアップロード進捗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScpClient_Uploading_upload(object sender, Renci.SshNet.Common.ScpUploadEventArgs e)
        {
            this.LogMessage.AppendMessage($" FileName = {e.Filename} Size => {e.Uploaded.ToString()} / {e.Size.ToString()} ({(int)(e.Uploaded / (double)e.Size * 100)}%)", false);

        }
        #endregion
    }
}
