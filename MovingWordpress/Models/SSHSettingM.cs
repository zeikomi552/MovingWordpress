using Microsoft.Win32;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class SSHSettingM : ConfigM
    {
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SSHSettingM()
		{
			this.ConfigDir = "Config";
			this.ConfigFileName = "SSHSetting.conf";
		}
		#endregion

		#region 引っ越し後のコンフィグファイル情報をセットする関数
		/// <summary>
		/// 引っ越し後のコンフィグファイル情報をセットする関数
		/// </summary>
		public void SetAfterConfig()
		{
			this.ConfigDir = "Config";
			this.ConfigFileName = "SSHSettingAfter.conf";
		}
		#endregion

		#region ホスト名[HostName]プロパティ
		/// <summary>
		/// ホスト名[HostName]プロパティ用変数
		/// </summary>
		string _HostName = string.Empty;
		/// <summary>
		/// ホスト名[HostName]プロパティ
		/// </summary>
		public string HostName
		{
			get
			{
				return _HostName;
			}
			set
			{
				if (_HostName == null || !_HostName.Equals(value))
				{
					_HostName = value;
					NotifyPropertyChanged("HostName");
				}
			}
		}
		#endregion

		#region ポート番号[Port]プロパティ
		/// <summary>
		/// ポート番号[Port]プロパティ用変数
		/// </summary>
		int _Port = 22;
		/// <summary>
		/// ポート番号[Port]プロパティ
		/// </summary>
		public int Port
		{
			get
			{
				return _Port;
			}
			set
			{
				if (!_Port.Equals(value))
				{
					_Port = value;
					NotifyPropertyChanged("Port");
				}
			}
		}
		#endregion

		#region ユーザー名[UserName]プロパティ
		/// <summary>
		/// ユーザー名[UserName]プロパティ用変数
		/// </summary>
		string _UserName = string.Empty;
		/// <summary>
		/// ユーザー名[UserName]プロパティ
		/// </summary>
		public string UserName
		{
			get
			{
				return _UserName;
			}
			set
			{
				if (_UserName == null || !_UserName.Equals(value))
				{
					_UserName = value;
					NotifyPropertyChanged("UserName");
				}
			}
		}
		#endregion

		#region パスワード[PassWord]プロパティ
		/// <summary>
		/// パスワード[PassWord]プロパティ用変数
		/// </summary>
		string _PassWord = string.Empty;
		/// <summary>
		/// パスワード[PassWord]プロパティ
		/// </summary>
		public string PassWord
		{
			get
			{
				return _PassWord;
			}
			set
			{
				if (_PassWord == null || !_PassWord.Equals(value))
				{
					_PassWord = value;
					NotifyPropertyChanged("PassWord");
				}
			}
		}
		#endregion

		#region パスフレーズ[PassPhrase]プロパティ
		/// <summary>
		/// パスフレーズ[PassPhrase]プロパティ用変数
		/// </summary>
		string _PassPhrase = string.Empty;
		/// <summary>
		/// パスフレーズ[PassPhrase]プロパティ
		/// </summary>
		public string PassPhrase
		{
			get
			{
				return _PassPhrase;
			}
			set
			{
				if (_PassPhrase == null || !_PassPhrase.Equals(value))
				{
					_PassPhrase = value;
					NotifyPropertyChanged("PassPhrase");
				}
			}
		}
		#endregion

		#region 秘密鍵のパス[KeyFilePath]プロパティ
		/// <summary>
		/// 秘密鍵のパス[KeyFilePath]プロパティ用変数
		/// </summary>
		string _KeyFilePath = string.Empty;
		/// <summary>
		/// 秘密鍵のパス[KeyFilePath]プロパティ
		/// </summary>
		public string KeyFilePath
		{
			get
			{
				return _KeyFilePath;
			}
			set
			{
				if (_KeyFilePath == null || !_KeyFilePath.Equals(value))
				{
					_KeyFilePath = value;
					NotifyPropertyChanged("KeyFilePath");
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
			base.Save<SSHSettingM>(this.ConfigFilePath, this);
		}
		#endregion

		#region ロード処理
		/// <summary>
		/// ロード処理
		/// </summary>
		/// <returns></returns>
		public SSHSettingM Load()
		{
			if (File.Exists(this.ConfigFilePath))
			{
				return base.Load<SSHSettingM>(this.ConfigFilePath);
			}
			else
			{
				return new SSHSettingM();
			}
		}
		#endregion

		/// <summary>
		/// 秘密鍵ファイルを開くダイアログ
		/// </summary>
		public void OpenPemFileDialog()
		{
			// ダイアログのインスタンスを生成
			var dialog = new OpenFileDialog();

			// ファイルの種類を設定
			dialog.Filter = "秘密鍵ファイル (*.pem)|*.pem|全てのファイル (*.*)|*.*";

			// ダイアログを表示する
			if (dialog.ShowDialog() == true)
			{
				// ファイルパスのセット
				this.KeyFilePath = dialog.FileName;
			}
		}

	}
}
