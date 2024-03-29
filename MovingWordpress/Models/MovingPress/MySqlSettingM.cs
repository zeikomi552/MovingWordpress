﻿using MVVMCore.BaseClass;
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
    public class MySqlSettingM : ConfigM
	{
		public MySqlSettingM()
		{
			this.ConfigDir = "Config";
			this.ConfigFileName = "MysqlSetting.conf";
		}

		#region 引っ越し後のコンフィグファイル情報をセットする関数
		/// <summary>
		/// 引っ越し後のコンフィグファイル情報をセットする関数
		/// </summary>
		public void SetAfterConfig()
		{
			this.ConfigDir = "Config";
			this.ConfigFileName = "MysqlSettingAfter.conf";
		}
		#endregion

		#region データベース名[Database]プロパティ
		/// <summary>
		/// データベース名[Database]プロパティ用変数
		/// </summary>
		string _Database = string.Empty;
		/// <summary>
		/// データベース名[Database]プロパティ
		/// </summary>
		public string Database
		{
			get
			{
				return _Database;
			}
			set
			{
				if (_Database == null || !_Database.Equals(value))
				{
					_Database = value;
					NotifyPropertyChanged("Database");
				}
			}
		}
		#endregion

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

		#region 保存処理
		/// <summary>
		/// 保存処理
		/// </summary>
		public void Save()
		{
			// カレントディレクトリがなければ再帰的に作成する
			DirectoryUtil.CreateCurrentDirectory(ConfigFilePath);

			// ファイルの保存処理
			base.Save<MySqlSettingM>(this.ConfigFilePath, this);
		}
		#endregion

		#region ロード処理
		/// <summary>
		/// ロード処理
		/// </summary>
		/// <returns></returns>
		public MySqlSettingM Load()
		{
			if (File.Exists(this.ConfigFilePath))
			{
				return base.Load<MySqlSettingM>(this.ConfigFilePath);
			}
			else
			{
				return new MySqlSettingM();
			}
		}
		#endregion
	}
}
