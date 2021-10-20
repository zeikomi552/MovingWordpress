﻿using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.Tweet
{
    public class TwitterUserM : ModelBase
    {
		public TwitterUserM()
		{

		}
		public TwitterUserM(CoreTweet.User user)
		{
			this.Id = user.Id;
			this.ScreenName = user.ScreenName;
			this.FollowersCount = user.FollowersCount;
			this.FriendsCount = user.FriendsCount;
			this.Description = user.Description;
		}
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
					return (double)this.FollowersCount / (double)this.FriendsCount;
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
				if (descrinption.Contains(nouns))
				{
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}
