﻿using MovingWordpress.Models;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MovingWordpress.ViewModels
{
    public class BaseWordpressVM : ViewModelBase
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

        #region バックアップファイル名
        /// <summary>
        /// SQLダンプのファイル名
        /// </summary>
        protected string _DumpSqlGz = "dump.sql.gz";
        /// <summary>
        /// Uploadsバックアップファイル名
        /// </summary>
        protected string _UploadGz = "uploads.tar.gz";
        /// <summary>
        /// Pluginsバックアップファイル名
        /// </summary>
        protected string _PluginsGz = "plugins.tar.gz";
        /// <summary>
        /// Themesバックアップファイル名
        /// </summary>
        protected string _ThemesGz = "themes.tar.gz";
        #endregion

        #region 初期化処理
        /// <summary>
        /// 初期化処理
        /// </summary>
        public virtual void Init()
        {

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

        #region メッセージ更新処理
        /// <summary>
        /// メッセージ更新処理
        /// </summary>
        /// <param name="message"></param>
        protected void UpdateMessage(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() =>
               {
                   this.Message = message.ToString();
               }));

        }
        #endregion

        #region コメント付きのコマンド実行処理
        /// <summary>
        /// コメント付きのコマンド実行処理
        /// </summary>
        /// <param name="cmd">コマンド</param>
        protected void ExecuteCommand(StringBuilder message, string cmd)
        {
            // コマンド内容のセット
            message.AppendLine("Command ==> " + cmd);

            // メッセージの更新
            UpdateMessage(message.ToString());

            message.AppendLine(this.SSHConnection.SshCommand(cmd));

            // メッセージの更新
            UpdateMessage(message.ToString());
        }
        #endregion

        #region コマンド実行処理
        /// <summary>
        /// コマンド実行処理
        /// </summary>
        /// <param name="cmd"></param>
        protected void ExecuteCommand(string cmd)
        {
            try
            {
                // 初期化処理
                this.SSHConnection.CreateConnection();

                Task.Run(() =>
                {
                    StringBuilder message = new StringBuilder();

                    // コマンド開始のメモ
                    message.AppendLine($"====== Command Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");

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
        #endregion
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

        #region MySQLデータベースのパスワード確認
        /// <summary>
        /// MySQLデータベースのパスワード確認
        /// </summary>
        public void CheckMySQLPassword()
        {
            try
            {
                // コマンド
                string cmd = $"cd {this.SSHConnection.FolderSetting.RemoteDirectory};cd ..;cat wp-config.php;";

                // コマンドの発行処理
                ExecuteCommand(cmd);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
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
                // 初期化処理
                this.SSHConnection.CreateConnection();


                Task.Run(() =>
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine($"====== 後片付け Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");
                    // メッセージの更新
                    UpdateMessage(message.ToString());

                    string cmd = "cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"rm -f /tmp/{_UploadGz};";
                    ExecuteCommand(message, cmd);

                    cmd = "cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"rm -f /tmp/{_PluginsGz};";
                    ExecuteCommand(message, cmd);

                    cmd = "cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"rm -f /tmp/{_ThemesGz};";
                    ExecuteCommand(message, cmd);

                    cmd = "cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"rm -f /tmp/{_DumpSqlGz};";
                    ExecuteCommand(message, cmd);

                    cmd = "cd " + this.SSHConnection.FolderSetting.RemoteDirectory + ";" + $"cd /tmp;ls -lh;";
                    ExecuteCommand(message, cmd);

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
        #endregion
    }
}
