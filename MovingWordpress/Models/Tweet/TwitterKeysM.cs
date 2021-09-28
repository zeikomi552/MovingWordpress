using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class TwitterKeysM : ModelBase
	{
		#region コンシューマーキー[ConsumerKey]プロパティ
		/// <summary>
		/// コンシューマーキー[ConsumerKey]プロパティ用変数
		/// </summary>
		string _ConsumerKey = string.Empty;
		/// <summary>
		/// コンシューマーキー[ConsumerKey]プロパティ
		/// </summary>
		public string ConsumerKey
		{
			get
			{
				return _ConsumerKey;
			}
			set
			{
				if (_ConsumerKey == null || !_ConsumerKey.Equals(value))
				{
					_ConsumerKey = value;
					NotifyPropertyChanged("ConsumerKey");
				}
			}
		}
		#endregion

		#region コンシューマーシークレットキー[ConsumerSecretKey]プロパティ
		/// <summary>
		/// コンシューマーシークレットキー[ConsumerSecretKey]プロパティ用変数
		/// </summary>
		string _ConsumerSecretKey = string.Empty;
		/// <summary>
		/// コンシューマーシークレットキー[ConsumerSecretKey]プロパティ
		/// </summary>
		public string ConsumerSecretKey
		{
			get
			{
				return _ConsumerSecretKey;
			}
			set
			{
				if (_ConsumerSecretKey == null || !_ConsumerSecretKey.Equals(value))
				{
					_ConsumerSecretKey = value;
					NotifyPropertyChanged("ConsumerSecretKey");
				}
			}
		}
		#endregion

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

		#region アクセスシークレット[AccessSecret]プロパティ
		/// <summary>
		/// アクセスシークレット[AccessSecret]プロパティ用変数
		/// </summary>
		string _AccessSecret = string.Empty;
		/// <summary>
		/// アクセスシークレット[AccessSecret]プロパティ
		/// </summary>
		public string AccessSecret
		{
			get
			{
				return _AccessSecret;
			}
			set
			{
				if (_AccessSecret == null || !_AccessSecret.Equals(value))
				{
					_AccessSecret = value;
					NotifyPropertyChanged("AccessSecret");
				}
			}
		}
		#endregion

		/// <summary>
		/// コンシューマーキーやその他アクセスに必要なキーがセットされているかどうかを確認する
		/// true:セットされている false:セットされていない
		/// </summary>
		public bool CheckKeys()
		{
			// キーの存在チェック
			if (string.IsNullOrEmpty(this.ConsumerKey)
				|| string.IsNullOrEmpty(this.ConsumerSecretKey)
				|| string.IsNullOrEmpty(this.AccessToken)
				|| string.IsNullOrEmpty(this.AccessSecret))
			{
				return false;
			}
			else
			{
				return true;
			}
		}


	}
}
