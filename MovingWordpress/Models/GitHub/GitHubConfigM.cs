using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.GitHub
{
    public class GitHubConfigM : ModelBase
    {
		const string ConfigFileName = "GitHubSetting.conf";

		#region アクセストークン[AccessToken]プロパティ
		/// <summary>
		/// アクセストークン[AccessToken]プロパティ用変数
		/// </summary>
		string _AccessToken = string.Empty;
		/// <summary>
		/// アクセストークン[AccessToken]プロパティ
		/// </summary>
		public string AccessToken
		{
			get
			{
				return _AccessToken;
			}
			set
			{
				if (_AccessToken == null || !_AccessToken.Equals(value))
				{
					_AccessToken = value;
					NotifyPropertyChanged("AccessToken");
				}
			}
		}
		#endregion

		#region 製品名(任意)[ProductHeader]プロパティ
		/// <summary>
		/// 製品名(任意)[ProductHeader]プロパティ用変数
		/// </summary>
		string _ProductHeader = string.Empty;
		/// <summary>
		/// 製品名(任意)[ProductHeader]プロパティ
		/// </summary>
		public string ProductHeader
		{
			get
			{
				return _ProductHeader;
			}
			set
			{
				if (_ProductHeader == null || !_ProductHeader.Equals(value))
				{
					_ProductHeader = value;
					NotifyPropertyChanged("ProductHeader");
				}
			}
		}
		#endregion


		#region ファイルの保存処理
		/// <summary>
		/// ファイルの保存処理
		/// </summary>
		/// <param name="tconf">TwitterSetting.conf</param>
		public void Save()
		{
			ConfigM conf = new ConfigM();
			var tconf_dir = conf.ConfigDirPath;
			var tconf_path = Path.Combine(tconf_dir, ConfigFileName);

			// ファイルの保存処理
			XMLUtil.Seialize<GitHubConfigM>(tconf_path, this);
		}
		#endregion

		#region コンフィグファイルのロード処理
		/// <summary>
		/// コンフィグファイルのロード処理
		/// </summary>
		/// <returns>コンフィグデータ</returns>
		public void Load()
		{
			ConfigM conf = new ConfigM();
			var tconf_dir = conf.ConfigDirPath;
			var tconf_path = Path.Combine(tconf_dir, ConfigFileName);

			// ファイルの存在確認
			if (File.Exists(tconf_path))
			{
				var tmp = XMLUtil.Deserialize<GitHubConfigM>(tconf_path);
				this.AccessToken = tmp.AccessToken;
				this.ProductHeader = tmp.ProductHeader;
			}
		}
		#endregion
	}
}
