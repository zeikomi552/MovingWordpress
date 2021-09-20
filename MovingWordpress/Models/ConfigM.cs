using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class ConfigM : ModelBase
    {
		#region アプリケーションフォルダの取得
		/// <summary>
		/// アプリケーションフォルダの取得
		/// </summary>
		/// <returns>アプリケーションフォルダパス</returns>
		public static string GetApplicationFolder()
		{
			var fv = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), fv.CompanyName, fv.ProductName);
		}
		#endregion

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
				// Configフォルダのパス取得
				string conf_dir = Path.Combine(GetApplicationFolder(), this.ConfigDir);
				// 存在確認
				if (!Directory.Exists(conf_dir))
				{
					// 存在しない場合は作成
					DirectoryUtil.CreateDirectory(conf_dir);
				}

				return Path.Combine(conf_dir, this.ConfigFileName);
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
