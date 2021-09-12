using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMCore.Common.Utilities
{
    public class DirectoryUtil
    {
        #region ディレクトリを再帰的に作成する
        /// <summary>
        /// ディレクトリを再帰的に作成する
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string dir_path)
        {
            if (!Directory.Exists(dir_path))
            {
                string parent = Directory.GetParent(dir_path).FullName;
                CreateDirectory(parent);
                Directory.CreateDirectory(dir_path);
            }
        }
        #endregion

        #region ファイルのカレントディレクトリを作成する
        /// <summary>
        /// ファイルのカレントディレクトリを作成する
        /// </summary>
        /// <param name="file_path">ファイルパス</param>
        public static void CreateCurrentDirectory(string file_path)
        {
            string parent = Directory.GetParent(file_path).FullName;
            if (!Directory.Exists(parent))
            {
                CreateDirectory(parent);
            }
        }
        #endregion
    }
}
