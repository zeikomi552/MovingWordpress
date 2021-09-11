using MVVMCore.BaseClass;
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
    public class SSHConnection : ModelBase
	{
		// 接続情報
		[XmlIgnoreAttribute] 
		public ConnectionInfo ConnNfo { private set; get; }

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


		#region リモートPC側のファイル保存ディレクトリ[RemoteDirectory]プロパティ
		/// <summary>
		/// リモートPC側のファイル保存ディレクトリ[RemoteDirectory]プロパティ用変数
		/// </summary>
		string _RemoteDirectory = "/opt/bitnami/apps/wordpress/htdocs/wp-content/";
		/// <summary>
		/// リモートPC側のファイル保存ディレクトリ[RemoteDirectory]プロパティ
		/// </summary>
		public string RemoteDirectory
		{
			get
			{
				return _RemoteDirectory;
			}
			set
			{
				if (_RemoteDirectory == null || !_RemoteDirectory.Equals(value))
				{
					// 最後の一文字を確認した上でセット
					_RemoteDirectory = CheckLastCharactor(value, "/");
					NotifyPropertyChanged("RemoteDirectory");
				}
			}
		}
		#endregion

		#region ローカルPC側のカレントディレクトリ[LocalDirectory]プロパティ
		/// <summary>
		/// ローカルPC側のカレントディレクトリ[LocalDirectory]プロパティ用変数
		/// </summary>
		string _LocalDirectory = string.Empty;
		/// <summary>
		/// ローカルPC側のカレントディレクトリ[LocalDirectory]プロパティ
		/// </summary>
		public string LocalDirectory
		{
			get
			{
				return _LocalDirectory;
			}
			set
			{
				if (_LocalDirectory == null || !_LocalDirectory.Equals(value))
				{
					// 最後の一文字を確認した上でセット
					_LocalDirectory = CheckLastCharactor(value, @"\");
					NotifyPropertyChanged("LocalDirectory");
				}
			}
		}
		#endregion


		/// <summary>
		/// 最後の文字列を確認して
		/// 指定した文字列でなければ付与して返却する。
		/// 文字数が0の場合は何もしない
		/// </summary>
		/// <param name="text"></param>
		/// <param name="last_caractor"></param>
		/// <returns></returns>
		private static string CheckLastCharactor(string text, string last_caractor)
		{

			if (text.Length > 0)
			{
				var tmp = text.Substring(text.Length - 1).Equals(last_caractor);
				if (!tmp)
				{
					text += last_caractor;
				}
			}

			return text;

		}


		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SSHConnection()
		{
		}
		#endregion

		/// <summary>
		/// パラメータの設定処理
		/// </summary>
		/// <param name="host_name">ホスト名</param>
		/// <param name="port">ポート番号</param>
		/// <param name="password">パスワード</param>
		/// <param name="passphrase">パスフレーズ</param>
		/// <param name="key_file_path">秘密鍵のファイルパス</param>
		public void SetParameters(string host_name, int port, string password, string passphrase, string key_file_path)
		{
			this.HostName = host_name;
			this.Port = port;
			this.PassWord = password;
			this.PassPhrase = passphrase;
			this.KeyFilePath = key_file_path;
		}

		/// <summary>
		/// 接続処理
		/// </summary>
		public void Initialize()
		{
			// パスワード認証
			var pass_auth = new PasswordAuthenticationMethod(this.UserName, this.PassWord);

			// 秘密鍵認証
			var private_key = new PrivateKeyAuthenticationMethod(UserName, new PrivateKeyFile[]{
						new PrivateKeyFile(this.KeyFilePath, this.PassPhrase)
					});

			// 接続情報の生成
			this.ConnNfo = new ConnectionInfo(this.HostName, this.Port, this.UserName,
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
		public string SshCommand(string command = "ls -lah")
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

	}
}
