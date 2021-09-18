﻿using MovingWordpress.Common;
using MovingWordpress.Models;
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

            // コマンド内容のセット
            message.AppendLine("result ==> " + cmd);

            // コマンドの実行
            var result = this.SSHConnection.SshCommand(cmd);

            // 実行結果をセット
            message.AppendLine(result);

            // メッセージの更新
            UpdateMessage(message.ToString());
        }
        #endregion

        #region コマンド実行処理
        /// <summary>
        /// コマンド実行処理
        /// </summary>
        /// <param name="cmd">コマンド</param>
        protected void ExecuteCommand(string cmd, StringBuilder message)
        {
            try
            {
                // 初期化処理
                this.SSHConnection.CreateConnection();

                // コマンド開始のメモ
                message.AppendLine($"====== Command Start {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");

                ExecuteCommand(message, cmd);

                message.AppendLine($"====== Command End {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} ======");

                // メッセージの更新
                UpdateMessage(message.ToString());
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
                StringBuilder message = new StringBuilder();
                ExecuteCommandList(@"CommandFiles\check_directory_info.mw", message);
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
                StringBuilder message = new StringBuilder();
                ExecuteCommandList(@"CommandFiles\check_wordpress_info.mw", message);
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
                StringBuilder message = new StringBuilder();
                ExecuteCommandList(@"CommandFiles\check_database_info.mw", message);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region ファイルパスを指定してコマンドを実行する
        /// <summary>
        /// ファイルパスを指定してコマンドを実行する
        /// </summary>
        /// <param name="command_file_path">コマンドファイルパス</param>
        private void ExecuteCommandList(string command_file_path, StringBuilder message)
        {
            var command_list = MovingWordpressUtilities.ReadCommandList(command_file_path);

            Task.Run(() =>
            {
                foreach (var command in command_list)
                {
                    // タグを変換
                    var tmp = MovingWordpressUtilities.ConvertCommandTags(command, this.SSHConnection);

                    // コマンドの発酵処理
                    ExecuteCommand(tmp, message);
                }
            });
        }
        #endregion
    }
}
