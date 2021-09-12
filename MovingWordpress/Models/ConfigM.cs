using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class ConfigM : ModelBase
    {
		#region コンフィグディレクトリパス[ConfigDir]プロパティ
		/// <summary>
		/// コンフィグディレクトリパス[ConfigDir]プロパティ用変数
		/// </summary>
		string _ConfigDir = "Config";
		/// <summary>
		/// コンフィグディレクトリパス[ConfigDir]プロパティ
		/// </summary>
		public string ConfigDir
		{
			get
			{
				return _ConfigDir;
			}
			set
			{
				if (_ConfigDir == null || !_ConfigDir.Equals(value))
				{
					_ConfigDir = value;
					NotifyPropertyChanged("ConfigDir");
					NotifyPropertyChanged("ConfigFilePath");
				}
			}
		}
		#endregion

		#region コンフィグファイル名[ConfigFileName]プロパティ
		/// <summary>
		/// コンフィグファイル名[ConfigFileName]プロパティ用変数
		/// </summary>
		string _ConfigFileName = string.Empty;
		/// <summary>
		/// コンフィグファイル名[ConfigFileName]プロパティ
		/// </summary>
		public string ConfigFileName
		{
			get
			{
				return _ConfigFileName;
			}
			set
			{
				if (_ConfigFileName == null || !_ConfigFileName.Equals(value))
				{
					_ConfigFileName = value;
					NotifyPropertyChanged("ConfigFileName");
					NotifyPropertyChanged("ConfigFilePath");
				}
			}
		}
		#endregion

		#region コンフィグファイルパス[ConfigFilePath]プロパティ
		/// <summary>
		/// コンフィグファイルパス[ConfigFilePath]プロパティ
		/// </summary>
		public string ConfigFilePath
		{
			get
			{
				return Path.Combine(this.ConfigDir, this.ConfigFileName);
			}
		}
        #endregion


        #region 設定ファイルファイルの保存処理
        /// <summary>
        /// 設定ファイルファイルの保存処理
        /// </summary>
        /// <param name="file_path">ファイルパス</param>
        /// <param name="setting">設定データ</param>
        public void Save<T>(string file_path, T setting)
        {
            XMLUtil.Seialize<T>(this.ConfigFilePath, setting);
        }
        #endregion

        #region 設定ファイルのロード処理
        /// <summary>
        /// 設定ファイルのロード処理
        /// </summary>
        /// <returns>読み込んだデータ</returns>
        public T Load<T>(string file_path)
        {
			return XMLUtil.Deserialize<T>(file_path);
		}
		#endregion
	}
}
