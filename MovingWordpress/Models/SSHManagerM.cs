using Microsoft.Win32;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MovingWordpress.Models
{
    public class SSHManagerM : ConfigM
	{
		#region SSH設定(引っ越し前)[SSHSetting]プロパティ
		/// <summary>
		/// SSH設定(引っ越し前)[SSHSetting]プロパティ用変数
		/// </summary>
		SSHSettingM _SSHSetting = new SSHSettingM();
		/// <summary>
		/// SSH設定(引っ越し前)[SSHSetting]プロパティ
		/// </summary>
		public SSHSettingM SSHSetting
		{
			get
			{
				return _SSHSetting;
			}
			set
			{
				if (_SSHSetting == null || !_SSHSetting.Equals(value))
				{
					_SSHSetting = value;
					NotifyPropertyChanged("SSHSetting");
				}
			}
		}
		#endregion


		#region SSH設定(引っ越し先)[AfterSSHSetting]プロパティ
		/// <summary>
		/// SSH設定(引っ越し先)[AfterSSHSetting]プロパティ用変数
		/// </summary>
		SSHSettingM _AfterSSHSetting = new SSHSettingM();
		/// <summary>
		/// SSH設定(引っ越し先)[AfterSSHSetting]プロパティ
		/// </summary>
		public SSHSettingM AfterSSHSetting
		{
			get
			{
				return _AfterSSHSetting;
			}
			set
			{
				if (_AfterSSHSetting == null || !_AfterSSHSetting.Equals(value))
				{
					_AfterSSHSetting = value;
					NotifyPropertyChanged("AfterSSHSetting");
				}
			}
		}
		#endregion

		#region フォルダ設定[FolderSetting]プロパティ
		/// <summary>
		/// フォルダ設定[FolderSetting]プロパティ用変数
		/// </summary>
		FolderSettingM _FolderSetting = new FolderSettingM();
		/// <summary>
		/// フォルダ設定[FolderSetting]プロパティ
		/// </summary>
		public FolderSettingM FolderSetting
		{
			get
			{
				return _FolderSetting;
			}
			set
			{
				if (_FolderSetting == null || !_FolderSetting.Equals(value))
				{
					_FolderSetting = value;
					NotifyPropertyChanged("FolderSetting");
				}
			}
		}
		#endregion

		#region MySQL設定(引っ越し前)[MySQLSetting]プロパティ
		/// <summary>
		/// MySQL設定(引っ越し前)[MySQLSetting]プロパティ用変数
		/// </summary>
		MySqlSettingM _MySQLSetting = new MySqlSettingM();
		/// <summary>
		/// MySQL設定(引っ越し前)[MySQLSetting]プロパティ
		/// </summary>
		public MySqlSettingM MySQLSetting
		{
			get
			{
				return _MySQLSetting;
			}
			set
			{
				if (_MySQLSetting == null || !_MySQLSetting.Equals(value))
				{
					_MySQLSetting = value;
					NotifyPropertyChanged("MySQLSetting");
				}
			}
		}
		#endregion

		#region MySQL設定(引っ越し先)[AfterMySQLSetting]プロパティ
		/// <summary>
		/// MySQL設定(引っ越し先)[AfterMySQLSetting]プロパティ用変数
		/// </summary>
		MySqlSettingM _AfterMySQLSetting = new MySqlSettingM();
		/// <summary>
		/// MySQL設定(引っ越し先)[AfterMySQLSetting]プロパティ
		/// </summary>
		public MySqlSettingM AfterMySQLSetting
		{
			get
			{
				return _AfterMySQLSetting;
			}
			set
			{
				if (_AfterMySQLSetting == null || !_AfterMySQLSetting.Equals(value))
				{
					_AfterMySQLSetting = value;
					NotifyPropertyChanged("AfterMySQLSetting");
				}
			}
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SSHManagerM()
		{

		}
		#endregion

		// 接続情報
		[XmlIgnoreAttribute] 
		public ConnectionInfo ConnNfo { private set; get; }


		[XmlIgnoreAttribute]
		public ConnectionInfo ConnNfoAfter { private set; get; }

		/// <summary>
		/// 接続処理
		/// </summary>
		public void Initialize()
		{
			// 接続情報の生成(引っ越し前)
			this.ConnNfo = CreateSSHConnector(this.SSHSetting);

			// 接続情報の生成(引っ越し後)
			this.ConnNfoAfter = CreateSSHConnector(this.AfterSSHSetting);    // 秘密鍵認証
		}

		/// <summary>
		/// SSHの接続用コネクタを作成する処理
		/// </summary>
		/// <param name="setting">SSH設定</param>
		/// <returns>コネクタ</returns>
		public ConnectionInfo CreateSSHConnector(SSHSettingM setting)
		{
			// パスワード認証
			var pass_auth = new PasswordAuthenticationMethod(setting.UserName, setting.PassWord);


			// 秘密鍵認証
			var private_key = new PrivateKeyAuthenticationMethod(setting.UserName, new PrivateKeyFile[]{
						new PrivateKeyFile(setting.KeyFilePath, setting.PassPhrase)
					});


			// 接続情報の生成
			return new ConnectionInfo(setting.HostName, setting.Port, setting.UserName,
				new AuthenticationMethod[]{
					pass_auth,          // パスワード認証
                    private_key        // 秘密鍵認証
                }
			);
		}

		/// <summary>
		/// SSHコマンド
		/// </summary>
		/// <param name="command">コマンド</param>
		/// <returns>結果</returns>
		public string SshCommand(string command)
		{
			StringBuilder result = new StringBuilder();
			using (var sshclient = new SshClient(this.ConnNfo))
			{
				sshclient.Connect();

				using (var cmd = sshclient.CreateCommand(command))
				{
					// コマンドの実行
					cmd.Execute();
					result.AppendLine(cmd.Result);
				}
				sshclient.Disconnect();
			}

			return result.ToString();
		}

		/// <summary>
		/// SCPによるファイルダウンロード処理を行う
		/// </summary>
		/// <param name="remote_file_path">リモートサーバー側のファイルの保管パス</param>
		/// <param name="local_file_path">ローカルファイルパスの保管場所</param>
		public void SCPDownload(string remote_file_path, string local_file_path, EventHandler<Renci.SshNet.Common.ScpDownloadEventArgs> scp_handler)
		{
			using (var sshclient = new SshClient(this.ConnNfo))
			{
				sshclient.Connect();
				using (var scpClient = new ScpClient(this.ConnNfo))
				{
					scpClient.Connect();

					// イベントのセット
					scpClient.Downloading += scp_handler;

					// イベントのダウンロード処理
					scpClient.Download(remote_file_path,
						new System.IO.DirectoryInfo(local_file_path));

					// イベントの解除
					scpClient.Downloading -= scp_handler;

					scpClient.Disconnect();
				}
				sshclient.Disconnect();
			}
		}


		/// <summary>
		/// Configファイルの保存処理
		/// </summary>
		public void Save()
		{
			// フォルダ設定の保存処理
			this.FolderSetting.Save();

			// SSH設定の保存処理
			this.SSHSetting.Save();

			// MySQL設定の保存処理
			this.MySQLSetting.Save();

		}

		/// <summary>
		/// Configファイルのロード処理
		/// </summary>
		public void Load()
		{
			// フォルダ設定の保存処理
			this.FolderSetting = this.FolderSetting.Load();

			// SSH設定の保存処理
			this.SSHSetting = this.SSHSetting.Load();

			// MySQL設定の保存処理
			this.MySQLSetting = this.MySQLSetting.Load();
		}
	}
}
