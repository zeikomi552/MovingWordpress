using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.Tweet
{
    public class TwitterConfigM : ModelBase
    {
		const string ConfigFileName = "TwitterSetting.conf";

		#region 記事テンプレートモデル[TempleteM]プロパティ
		/// <summary>
		/// 記事テンプレートモデル[TempleteM]プロパティ用変数
		/// </summary>
		TwitterContentM _TempleteM = new TwitterContentM();
		/// <summary>
		/// 記事テンプレートモデル[TempleteM]プロパティ
		/// </summary>
		public TwitterContentM TempleteM
		{
			get
			{
				return _TempleteM;
			}
			set
			{
				if (_TempleteM == null || !_TempleteM.Equals(value))
				{
					_TempleteM = value;
					NotifyPropertyChanged("TempleteM");
				}
			}
		}
		#endregion

		#region 各種キーモデル[KeysM]プロパティ
		/// <summary>
		/// 各種キーモデル[KeysM]プロパティ用変数
		/// </summary>
		TwitterKeysM _KeysM = new TwitterKeysM();
		/// <summary>
		/// 各種キーモデル[KeysM]プロパティ
		/// </summary>
		public TwitterKeysM KeysM
		{
			get
			{
				return _KeysM;
			}
			set
			{
				if (_KeysM == null || !_KeysM.Equals(value))
				{
					_KeysM = value;
					NotifyPropertyChanged("KeysM");
				}
			}
		}
		#endregion

		#region 自分のスクリーン名[MyScreenName]プロパティ
		/// <summary>
		/// 自分のスクリーン名[MyScreenName]プロパティ用変数
		/// </summary>
		string _MyScreenName = string.Empty;
		/// <summary>
		/// 自分のスクリーン名[MyScreenName]プロパティ
		/// </summary>
		public string MyScreenName
		{
			get
			{
				return _MyScreenName;
			}
			set
			{
				if (_MyScreenName == null || !_MyScreenName.Equals(value))
				{
					_MyScreenName = value;
					NotifyPropertyChanged("MyScreenName");
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
			XMLUtil.Seialize<TwitterConfigM>(tconf_path, this);
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
				var tmp = XMLUtil.Deserialize<TwitterConfigM>(tconf_path);

				// 各種キーの取り出し
				this.KeysM = tmp.KeysM;

				// テンプレートの取り出し
				this.TempleteM = tmp.TempleteM;

				// ScreenNameの取り出し
				this.MyScreenName = tmp.MyScreenName;
			}
		}
		#endregion
	}
}
