using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MovingWordpress.Models
{
    public class LogMessageM : ModelBase
    {
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

        /// <summary>
        /// 直前のメッセージ
        /// </summary>
        public string _BackupMessage = string.Empty;

        #region メッセージ更新処理
        /// <summary>
        /// メッセージ更新処理
        /// </summary>
        /// <param name="message">更新するメッセージ</param>
        /// <param name="commit_f">false:最終行は未確定(次の更新時にその行を書き換える想定) true:最終行が確定</param>
        public void UpdateMessage(string message, bool commit_f = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() =>
               {
                   this.Message = message.ToString();
                   // 確定フラグ
                   if (commit_f)
                   {
                       CommitMessage();
                   }
               }));

        }
        #endregion

        #region メッセージの追加(改行コードを強制的に入れる)
        /// <summary>
        /// メッセージの追加(改行コードを強制的に入れる)
        /// </summary>
        /// <param name="message">追加するメッセージ</param>
        /// <param name="commit_f">false:最終行は未確定(次の更新時にその行を書き換える想定) true:最終行が確定</param>
        public void AppendMessage(string message, bool commit_f = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() =>
               {
                   this.Message = this._BackupMessage + "\r\n" + message.ToString();

                   // 確定フラグ
                   if (commit_f)
                   {
                       CommitMessage();
                   }
               }));
        }
        #endregion

        #region メッセージを確定させる
        /// <summary>
        /// メッセージを確定させる
        /// </summary>
        public void CommitMessage()
        {
            this._BackupMessage = this.Message;
        }
        #endregion
    }
}
