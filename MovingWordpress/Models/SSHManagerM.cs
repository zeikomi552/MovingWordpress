﻿using Microsoft.Win32;
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
		#region SSH設定[SSHSetting]プロパティ
		/// <summary>
		/// SSH設定[SSHSetting]プロパティ用変数
		/// </summary>
		SSHSettingM _SSHSetting = new SSHSettingM();
		/// <summary>
		/// SSH設定[SSHSetting]プロパティ
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

		#region MySQL設定[MySQLSetting]プロパティ
		/// <summary>
		/// MySQL設定[MySQLSetting]プロパティ用変数
		/// </summary>
		MySqlSettingM _MySQLSetting = new MySqlSettingM();
		/// <summary>
		/// MySQL設定[MySQLSetting]プロパティ
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

		/// <summary>
		/// 初期化処理
		/// </summary>
		/// <param name="isBefore">true:引っ越し前の情報を扱う false:引っ越し後の情報を扱う</param>
        public void Initialize(bool isBefore)
        {
            // 引っ越し後の情報を使用する？
            if (!isBefore)
            {
                this.SSHSetting.SetAfterConfig();
                this.FolderSetting.SetAfterConfig();
                this.MySQLSetting.SetAfterConfig();
            }
        }

        /// <summary>
        /// 接続情報の初期化処理
        /// </summary>
        /// <param name="isBefore">true:引っ越し前の処理 false:引っ越し後の処理</param>
        public void CreateConnection(bool isBefore = true)
		{

			// パスワード認証
			var pass_auth = new PasswordAuthenticationMethod(this.SSHSetting.UserName, this.SSHSetting.PassWord);

			// 秘密鍵認証
			var private_key = new PrivateKeyAuthenticationMethod(this.SSHSetting.UserName, new PrivateKeyFile[]{
						new PrivateKeyFile(this.SSHSetting.KeyFilePath, this.SSHSetting.PassPhrase)
					});

			// 接続情報の生成
			this.ConnNfo = new ConnectionInfo(this.SSHSetting.HostName, this.SSHSetting.Port, this.SSHSetting.UserName,
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
