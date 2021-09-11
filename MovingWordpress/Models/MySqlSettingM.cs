using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MovingWordpress.Models
{
    public class MySqlSettingM : ModelBase
	{
		/// <summary>
		/// コンフィグファイル
		/// </summary>
		const string ConfigDir = "Config";
		/// <summary>
		/// Mysql用コンフィグファイル
		/// </summary>
		static string ConfigFile_Path = Path.Combine(ConfigDir, "MySQLSetting.config");

		#region MySQLのユーザーID[MySQLUserID]プロパティ
		/// <summary>
		/// MySQLのユーザーID[MySQLUserID]プロパティ用変数
		/// </summary>
		string _MySQLUserID = string.Empty;
		/// <summary>
		/// MySQLのユーザーID[MySQLUserID]プロパティ
		/// </summary>
		public string MySQLUserID
		{
			get
			{
				return _MySQLUserID;
			}
			set
			{
				if (_MySQLUserID == null || !_MySQLUserID.Equals(value))
				{
					_MySQLUserID = value;
					NotifyPropertyChanged("MySQLUserID");
				}
			}
		}
		#endregion

		#region MySQLのパスワード[MySQLPassword]プロパティ
		/// <summary>
		/// MySQLのパスワード[MySQLPassword]プロパティ用変数
		/// </summary>
		string _MySQLPassword = string.Empty;
		/// <summary>
		/// MySQLのパスワード[MySQLPassword]プロパティ
		/// </summary>
		[XmlIgnoreAttribute]
		public string MySQLPassword
		{
			get
			{
				return _MySQLPassword;
			}
			set
			{
				if (_MySQLPassword == null || !_MySQLPassword.Equals(value))
				{
					_MySQLPassword = value;
					NotifyPropertyChanged("MySQLPassword");
				}
			}
		}
		#endregion

		#region 暗号化したパターンのパスワード
		/// <summary>
		/// 暗号化したパターンのパスワード
		/// </summary>
		[XmlElement("MySQLPassword")]
		public string MySQLPasswordCrypt
		{
			get
			{
				if (!string.IsNullOrEmpty(this.MySQLPassword))
				{
					// 暗号化して返却する
					return EncryptionUtil.EncryptString(this.MySQLPassword, "Zeikomi552");
				}
				else
				{
					return string.Empty;
				}
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					// 複合化して保持する
					this.MySQLPassword = EncryptionUtil.DecryptString(value, "Zeikomi552");
				}
				else
				{
					this.MySQLPassword = string.Empty;
				}
			}
		}
		#endregion

		#region 設定ファイルファイルの保存処理
		/// <summary>
		/// 設定ファイルファイルの保存処理
		/// </summary>
		/// <param name="file_path">ファイルパス</param>
		/// <param name="setting">設定データ</param>
		public static void Save(string file_path, MySqlSettingM setting)
		{
			// フォルダの存在確認
			if (!Directory.Exists(ConfigDir))
			{
				// フォルダの作成
				Directory.CreateDirectory(ConfigDir);
			}

			XMLUtil.Seialize<MySqlSettingM>(ConfigFile_Path, setting);
		}
		#endregion

		#region 設定ファイルのロード処理
		/// <summary>
		/// 設定ファイルのロード処理
		/// </summary>
		/// <returns>読み込んだデータ</returns>
		public static MySqlSettingM Load()
		{
			// ファイルの存在確認
			if (File.Exists(ConfigFile_Path))
			{
				return XMLUtil.Deserialize<MySqlSettingM>(ConfigFile_Path);
			}
			else
			{
				return new MySqlSettingM();
			}
		}
		#endregion
	}
}
