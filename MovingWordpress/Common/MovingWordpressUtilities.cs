using MovingWordpress.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Common
{
    public class MovingWordpressUtilities
    {
        #region コマンドリストを読み込む
        /// <summary>
        /// コマンドリストを読み込む
        /// </summary>
        /// <param name="file_path"></param>
        /// <returns></returns>
        public static List<string> ReadCommandList(string file_path)
        {
            List<string> command_list = new List<string>();
            using (StreamReader sr = new StreamReader(file_path, Encoding.UTF8))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || (line.Length >= 2 && line.ElementAt(0) == '/' && line.ElementAt(1) == '/'))
                    {
                        continue;
                    }

                    command_list.Add(line);
                }
            }
            return command_list;
        }
        #endregion

        #region コマンド絡みのタグを変換する処理
        /// <summary>
        /// コマンド絡みのタグを変換する処理
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <param name="ssh_manager">SSHマネージャー</param>
        /// <returns>文字列</returns>
        public static string ConvertCommandTags(string command, SSHManagerM ssh_manager)
        {
            command = command.Replace("{mw:RemoteDirecotry}", ssh_manager.FolderSetting.RemoteDirectory);
            command = command.Replace("{mw:LocalDirecotry}", ssh_manager.FolderSetting.LocalDirectory);
            command = command.Replace("{mw:DatabasePassword}", ssh_manager.MySQLSetting.MySQLPassword);
            command = command.Replace("{mw:Database}", ssh_manager.MySQLSetting.Database);
            command = command.Replace("{mw:DatabaseUserID}", ssh_manager.MySQLSetting.MySQLUserID);
            command = command.Replace("{mw:SSHHostName}", ssh_manager.SSHSetting.HostName);
            command = command.Replace("{mw:SSHUserName}", ssh_manager.SSHSetting.UserName);
            command = command.Replace("{mw:SSHPassWord}", ssh_manager.SSHSetting.PassWord);
            command = command.Replace("{mw:SSHKeyFilePath}", ssh_manager.SSHSetting.KeyFilePath);
            command = command.Replace("{mw:SSHKeyFilePathPassPhrase}", ssh_manager.SSHSetting.PassPhrase);
            return command;
        }
        #endregion

        #region バイトのコピー処理
        /// <summary>
        /// バイトのコピー処理
        /// </summary>
        /// <param name="src">コピー元</param>
        /// <param name="dest">コピー先</param>
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }
        #endregion

        #region 解凍処理
        /// <summary>
        /// 解凍処理
        /// </summary>
        /// <param name="file_path">ファイルパス .sql.gz</param>
        /// <returns>解凍後取り出せた文字列列</returns>
        public static string Decompress(string file_path)
        {
            //展開する書庫のパス
            string gzipFile = file_path;

            //展開する書庫のFileStreamを作成する
            using (var gzipFileStrm = new System.IO.FileStream(
                gzipFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //圧縮解除モードのGZipStreamを作成する
                using (var gzipStrm =
                    new System.IO.Compression.GZipStream(gzipFileStrm,
                        System.IO.Compression.CompressionMode.Decompress))
                {
                    using (var mso = new MemoryStream())
                    {
                        // メモリストリームへコピー
                        CopyTo(gzipStrm, mso);

                        // テキストへ変換
                        return Encoding.UTF8.GetString(mso.ToArray());
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// NullやStringEmptyが入ってきたらpadding_textに置き換える
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="padding_text"></param>
        /// <returns></returns>
        public static string NullPadding(string text, string padding_text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return padding_text;
            }
            else
            {
                return text;
            }
        }

        #region URLを既定のブラウザで開く
        /// <summary>
        /// URLを既定のブラウザで開く
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>Process</returns>
        public static Process OpenUrl(string url)
        {
            ProcessStartInfo pi = new ProcessStartInfo()
            {
                FileName = url,
                UseShellExecute = true,
            };

            return Process.Start(pi);
        }
        #endregion
    }
}
