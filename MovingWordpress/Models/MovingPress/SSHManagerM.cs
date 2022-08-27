using Renci.SshNet;
using System;
using System.IO;
using System.Text;
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
		public void Download(string remote_file_path, string local_file_path,
			EventHandler<Renci.SshNet.Common.ScpDownloadEventArgs> scp_handler, Action<ulong> sftp_action, bool scp_f)
		{
			using (var sshclient = new SshClient(this.ConnNfo))
			{
				sshclient.Connect();

				// SCPを使用する
				if (scp_f)
				{
					// SCPクライアントの作成
					using (var scpClient = new ScpClient(this.ConnNfo))
					{
						// ダウンロード処理
						DownLoadSCP(scpClient, remote_file_path, local_file_path, scp_handler);
					}
				}
				// SFTPを使用する
				else
				{
					// SFTPクライアントの作成
					using (var sftpCilent = new SftpClient(this.ConnNfo))
					{
						DownLoadSFTP(sftpCilent, remote_file_path, local_file_path, sftp_action);
					}
				}

				sshclient.Disconnect();
			}
		}


		/// <summary>
		/// SCPを使用したダウンロード処理
		/// </summary>
		/// <param name="scpClient">SCPクライアント</param>
		/// <param name="remote_file_path">リモートファイルパス</param>
		/// <param name="local_file_path">ローカルディレクトリパス</param>
		/// <param name="scp_handler">イベント受信用ハンドラ</param>
		public void DownLoadSCP(ScpClient scpClient, string remote_file_path, string local_file_path,
			EventHandler<Renci.SshNet.Common.ScpDownloadEventArgs> scp_handler)
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

		/// <summary>
		/// SFTPを使用したダウンロード処理
		/// </summary>
		/// <param name="sftpClient">SFTPクライアント</param>
		/// <param name="remote_file_path">リモートファイルパス</param>
		/// <param name="local_file_path">ローカルファイルパス（保存ファイル名）</param>
		public void DownLoadSFTP(SftpClient sftpClient, string remote_file_path, string local_file_path, Action<ulong> del_func)
		{
			using (var downloadStream = File.OpenWrite(local_file_path))
			{
				// 接続
				sftpClient.Connect();

				var dir_path = System.IO.Path.GetDirectoryName(remote_file_path).Replace(@"\", @"/");

				var list = sftpClient.ListDirectory(dir_path);

				// ダウンロード処理
				sftpClient.DownloadFile(remote_file_path, downloadStream, del_func);

				// 切断
				sftpClient.Disconnect();
			}
		}


		public System.Collections.Generic.IEnumerable<Renci.SshNet.Sftp.SftpFile> GetFileList(string dir_path)
		{
			System.Collections.Generic.IEnumerable<Renci.SshNet.Sftp.SftpFile> ret;

			using (var sshclient = new SshClient(this.ConnNfo))
			{
				sshclient.Connect();

				// SFTPクライアントの作成
				using (var sftpClient = new SftpClient(this.ConnNfo))
				{
					// 接続
					sftpClient.Connect();
					ret = sftpClient.ListDirectory(dir_path);
					// 切断
					sftpClient.Disconnect();
				}

				sshclient.Disconnect();
			}

			return ret;
		}


		/// <summary>
		/// SCPによるファイルアップロード処理を行う
		/// </summary>
		/// <param name="remote_file_path">リモートサーバー側のファイルの保管パス</param>
		/// <param name="local_file_path">ローカルファイルパスの保管場所</param>
		/// <param name="scp_f">true:SCPでのアップロード false:SFTPでのダウンロード</param>
		public void SCPUpload(string remote_file_path, string local_file_path,
			EventHandler<Renci.SshNet.Common.ScpUploadEventArgs> scp_handler, bool scp_f)
		{
			using (var sshclient = new SshClient(this.ConnNfo))
			{
				sshclient.Connect();
				if (scp_f)
				{
					using (var scpClient = new ScpClient(this.ConnNfo))
					{
						uploadFileScp(scpClient, scp_handler, local_file_path, remote_file_path);
					}
				}
				else
				{
					using (var sftp = new SftpClient(ConnNfo))
					{
						uploadFileSFTP(sftp, local_file_path, remote_file_path);
					}
				}
				sshclient.Disconnect();
			}
		}

		private void uploadFileScp(ScpClient scpClient, 
			EventHandler<Renci.SshNet.Common.ScpUploadEventArgs> scp_handler,
			string local_file_path, string remote_file_path)
		{
			scpClient.Connect();

			// イベントのセット
			scpClient.Uploading += scp_handler;
			scpClient.RemotePathTransformation = RemotePathTransformation.ShellQuote;
			// アップロード処理
			scpClient.Upload(new System.IO.FileInfo(local_file_path), remote_file_path);

			// イベントの解除
			scpClient.Uploading -= scp_handler;

			scpClient.Disconnect();
		}


		// ファイルのアップロード
		private void uploadFileSFTP(
			SftpClient sftp,       // sftpクライアント
			string local_file_path,    // アップロードパス
			string remote_file_path     // アップロードファイル名
			)
		{
			//// カレントディレクトリ変更
			//sftp.ChangeDirectory(uploadPath);
			//// アップロード先パス
			//var remotePath = uploadPath + "/" + Path.GetFileName(uploadFile);

			using (var uploadStream = File.OpenRead(local_file_path))
			{
				sftp.UploadFile(uploadStream, remote_file_path, true);
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
			// フォルダのコンフィグ存在確認
			if (File.Exists(this.FolderSetting.ConfigFilePath))
            {
				// フォルダ設定の保存処理
				this.FolderSetting = this.FolderSetting.Load();
			}

			// SSHのコンフィグ存在確認
			if (File.Exists(this.SSHSetting.ConfigFilePath))
			{
				// SSH設定の保存処理
				this.SSHSetting = this.SSHSetting.Load();
			}

			// MySQLのコンフィグ存在確認
			if (File.Exists(this.MySQLSetting.ConfigFilePath))
			{
				// MySQL設定の保存処理
				this.MySQLSetting = this.MySQLSetting.Load();
			}
		}
	}
}
