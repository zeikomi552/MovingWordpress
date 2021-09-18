using MovingWordpress.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Common
{
    public class MovingWordpressUtilities
    {
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
            command = command.Replace("{mw:DatabaseUserID}", ssh_manager.MySQLSetting.MySQLUserID);
            command = command.Replace("{mw:SSHHostName}", ssh_manager.SSHSetting.HostName);
            command = command.Replace("{mw:SSHUserName}", ssh_manager.SSHSetting.UserName);
            command = command.Replace("{mw:SSHPassWord}", ssh_manager.SSHSetting.PassWord);
            command = command.Replace("{mw:SSHKeyFilePath}", ssh_manager.SSHSetting.KeyFilePath);
            command = command.Replace("{mw:SSHKeyFilePathPassPhrase}", ssh_manager.SSHSetting.PassPhrase);
            return command;
        }
    }
}
