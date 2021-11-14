using MovingWordpress.Models.db;
using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.Tweet
{
    public class TwitterUserM : ModelBase
    {
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TwitterUserM()
		{

		}
		#endregion

		#region ID[Id]プロパティ
		/// <summary>
		/// ID[Id]プロパティ用変数
		/// </summary>
		long? _Id = null;
		/// <summary>
		/// ID[Id]プロパティ
		/// </summary>
		public long? Id
		{
			get
			{
				return _Id;
			}
			set
			{
				if (_Id == null || !_Id.Equals(value))
				{
					_Id = value;
					NotifyPropertyChanged("Id");
				}
			}
		}
		#endregion

		#region スクリーン名[ScreenName]プロパティ
		/// <summary>
		/// スクリーン名[ScreenName]プロパティ用変数
		/// </summary>
		string _ScreenName = string.Empty;
		/// <summary>
		/// スクリーン名[ScreenName]プロパティ
		/// </summary>
		public string ScreenName
		{
			get
			{
				return _ScreenName;
			}
			set
			{
				if (_ScreenName == null || !_ScreenName.Equals(value))
				{
					_ScreenName = value;
					NotifyPropertyChanged("ScreenName");
				}
			}
		}
		#endregion
		
		#region フォロワー数[FollowersCount]プロパティ
		/// <summary>
		/// フォロワー数[FollowersCount]プロパティ用変数
		/// </summary>
		long _FollowersCount = new long();
		/// <summary>
		/// フォロワー数[FollowersCount]プロパティ
		/// </summary>
		public long FollowersCount
		{
			get
			{
				return _FollowersCount;
			}
			set
			{
				if (!_FollowersCount.Equals(value))
				{
					_FollowersCount = value;
					NotifyPropertyChanged("FollowersCount");
				}
			}
		}
		#endregion

		#region フォロー数[FriendsCount]プロパティ
		/// <summary>
		/// フォロー数[FriendsCount]プロパティ用変数
		/// </summary>
		long _FriendsCount = new long();
		/// <summary>
		/// フォロー数[FriendsCount]プロパティ
		/// </summary>
		public long FriendsCount
		{
			get
			{
				return _FriendsCount;
			}
			set
			{
				if (!_FriendsCount.Equals(value))
				{
					_FriendsCount = value;
					NotifyPropertyChanged("FriendsCount");
				}
			}
		}
		#endregion

		#region フォローしているかどうかを示すフラグ[IsFriend]プロパティ
		/// <summary>
		/// フォローしているかどうかを示すフラグ[IsFriend]プロパティ用変数
		/// </summary>
		bool _IsFriend = false;
		/// <summary>
		/// フォローしているかどうかを示すフラグ[IsFriend]プロパティ
		/// </summary>
		public bool IsFriend
		{
			get
			{
				return _IsFriend;
			}
			set
			{
				if (!_IsFriend.Equals(value))
				{
					_IsFriend = value;
					NotifyPropertyChanged("IsFriend");
				}
			}
		}
		#endregion

		#region フォロワーかどうかをしめすフラグ[IsFollower]プロパティ
		/// <summary>
		/// フォロワーかどうかをしめすフラグ[IsFollower]プロパティ用変数
		/// </summary>
		bool _IsFollower = false;
		/// <summary>
		/// フォロワーかどうかをしめすフラグ[IsFollower]プロパティ
		/// </summary>
		public bool IsFollower
		{
			get
			{
				return _IsFollower;
			}
			set
			{
				if (!_IsFollower.Equals(value))
				{
					_IsFollower = value;
					NotifyPropertyChanged("IsFollower");
				}
			}
		}
		#endregion

		#region 説明[Description]プロパティ
		/// <summary>
		/// 説明[Description]プロパティ用変数
		/// </summary>
		string _Description = string.Empty;
		/// <summary>
		/// 説明[Description]プロパティ
		/// </summary>
		public string Description
		{
			get
			{
				return _Description;
			}
			set
			{
				if (_Description == null || !_Description.Equals(value))
				{
					_Description = value;
					NotifyPropertyChanged("Description");
				}
			}
		}
		#endregion

		#region フォロー数に対するフォロワー数割合を出す
		/// <summary>
		/// フォロー数に対するフォロワー数割合を出す
		/// </summary>
		public double FriendshipRatio
		{
            get
            {
				// 0割回避
				if (this.FriendsCount > 0)
				{
					// 割合の算出
					return ((double)this.FollowersCount / (double)this.FriendsCount) * 100.0;
				}
				else
				{
					return 0.0;
				}
			}
		}
		#endregion

		#region 説明に期待する文字列が含まれているかのチェック
		/// <summary>
		/// 説明に期待する文字列が含まれているかのチェック
		/// </summary>
		/// <param name="nouns_list">単語リスト</param>
		/// <returns>true:含まれる false:含まれない</returns>
		public bool CheckDescription(string[] nouns_list)
		{
			// 説明の取り出し
			string descrinption = this.Description;

			// 文字リストから要素を取り出し
			foreach (var nouns in nouns_list)
			{
				// 説明に期待する文字が含まれるかどうかのチェック
				if (descrinption.Contains(nouns.Trim()))
				{
					return true;
				}
			}

			return false;
		}
		#endregion

		#region CoreTweet型からTwitterUserMに変換する
		/// <summary>
		/// CoreTweet型からTwitterUserMに変換する
		/// </summary>
		/// <param name="user_list">ユーザーリスト</param>
		/// <returns>ユーザーリスト</returns>
		public static List<TwitterUserM> ToTwitterUserM(CoreTweet.Cursored<CoreTweet.User> user_list)
		{
			var ret = new List<TwitterUserM>();
			foreach (var user in user_list)
			{
				ret.Add(new TwitterUserM()
				{
					Id = user.Id,
					Description = user.Description,
					FollowersCount = user.FollowersCount,
					FriendsCount = user.FriendsCount,
					ScreenName = user.ScreenName
				}
					);
			}
			return ret;
		}
		#endregion

		#region TwitterUserMに変換する
		/// <summary>
		/// TwitterUserMに変換する
		/// </summary>
		/// <param name="user_list">ユーザーリスト</param>
		/// <returns>TwitterUserMのリスト</returns>
		public static List<TwitterUserM> ToTwitterUserM(List<TwitterUserBase> user_list)
		{
			var ret = new List<TwitterUserM>();
			foreach (var user in user_list)
			{
				ret.Add(new TwitterUserM()
				{
					Id = user.Id,
					Description = user.Description,
					FollowersCount = user.FollowersCount,
					FriendsCount = user.FriendsCount,
					ScreenName = user.ScreenName
				}
					);
			}
			return ret;
		}
		#endregion
	}
}
