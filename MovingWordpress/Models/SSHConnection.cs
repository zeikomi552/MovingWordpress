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

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SSHConnection()
		{
		}

		public void SetParameters(string host_name, int port, string pass_word, string passphrase, string key_file_path)
		{
			this.HostName = host_name;
			this.Port = port;
			this.PassWord = PassWord;
			this.PassPhrase = passphrase;
			this.KeyFilePath = key_file_path;
		}

		/// <summary>
		/// 接続処理
		/// </summary>
		public void Connect()
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

		public string sshCommand(string command = "ls -lah")
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

		public void Download(string remote_dir, string local_path)
		{
			using (var sshclient = new SshClient(this.ConnNfo))
			{
				sshclient.Connect();
				using (var scpClient = new ScpClient(this.ConnNfo))
				{
					scpClient.Connect();

                    scpClient.Downloading += ScpClient_Downloading;
					scpClient.Download("/opt/bitnami/apps/wordpress/htdocs/wp-content/uploads.tar.gz",
						new System.IO.DirectoryInfo(@"C:\Work\test\20210911"));

					scpClient.Disconnect();
				}
				sshclient.Disconnect();
			}
		}

        private void ScpClient_Downloading(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {

            //throw new NotImplementedException();
        }
    }
}
