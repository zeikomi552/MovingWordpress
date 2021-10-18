using MovingWordpress.Common;
using MovingWordpress.Models;
using MovingWordpress.Models.db;
using MovingWordpress.Models.Tweet;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MovingWordpress.ViewModels
{
    public class TwitterFollowerSearchVM : TwitterAPIVM
    {
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

		#region フォロワーリスト[FollowerList]プロパティ
		/// <summary>
		/// フォロワーリスト[FollowerList]プロパティ用変数
		/// </summary>
		ModelList<CoreTweet.User> _FollowerList = new ModelList<CoreTweet.User>();
		/// <summary>
		/// フォロワーリスト[FollowerList]プロパティ
		/// </summary>
		public ModelList<CoreTweet.User> FollowerList
		{
			get
			{
				return _FollowerList;
			}
			set
			{
				if (_FollowerList == null || !_FollowerList.Equals(value))
				{
					_FollowerList = value;
					NotifyPropertyChanged("FollowerList");
				}
			}
		}
		#endregion

		#region 繰り返し検索[RepeatSearch]プロパティ
		/// <summary>
		/// 繰り返し検索[RepeatSearch]プロパティ用変数
		/// </summary>
		bool _RepeatSearch = false;
		/// <summary>
		/// 繰り返し検索[RepeatSearch]プロパティ
		/// </summary>
		public bool RepeatSearch
		{
			get
			{
				return _RepeatSearch;
			}
			set
			{
				if (!_RepeatSearch.Equals(value))
				{
					_RepeatSearch = value;
					NotifyPropertyChanged("RepeatSearch");
				}
			}
		}
		#endregion

		#region 説明文に含まれる文字[DescriptionKeys]プロパティ
		/// <summary>
		/// 説明文に含まれる文字[DescriptionKeys]プロパティ用変数
		/// </summary>
		string _DescriptionKeys = string.Empty;
		/// <summary>
		/// 説明文に含まれる文字[DescriptionKeys]プロパティ
		/// </summary>
		public string DescriptionKeys
		{
			get
			{
				return _DescriptionKeys;
			}
			set
			{
				if (_DescriptionKeys == null || !_DescriptionKeys.Equals(value))
				{
					_DescriptionKeys = value;
					NotifyPropertyChanged("DescriptionKeys");
				}
			}
		}
		#endregion

		#region フォロー率の下限値[MinRatio]プロパティ
		/// <summary>
		/// フォロー率の下限値[MinRatio]プロパティ用変数
		/// </summary>
		double _MinRatio = 98.0;
		/// <summary>
		/// フォロー率の下限値[MinRatio]プロパティ
		/// </summary>
		public double MinRatio
		{
			get
			{
				return _MinRatio;
			}
			set
			{
				if (!_MinRatio.Equals(value))
				{
					_MinRatio = value;
					NotifyPropertyChanged("MinRatio");
				}
			}
		}
		#endregion

		#region フォロー率の上限値[MaxRatio]プロパティ
		/// <summary>
		/// フォロー率の上限値[MaxRatio]プロパティ用変数
		/// </summary>
		double _MaxRatio = 102.0;
		/// <summary>
		/// フォロー率の上限値[MaxRatio]プロパティ
		/// </summary>
		public double MaxRatio
		{
			get
			{
				return _MaxRatio;
			}
			set
			{
				if (!_MaxRatio.Equals(value))
				{
					_MaxRatio = value;
					NotifyPropertyChanged("MaxRatio");
				}
			}
		}
		#endregion

		#region マイフォローリスト[MyFollows]プロパティ
		/// <summary>
		/// マイフォローリスト[MyFollows]プロパティ用変数
		/// </summary>
		ModelList<CoreTweet.User> _MyFollows = new ModelList<CoreTweet.User>();
		/// <summary>
		/// マイフォローリスト[MyFollows]プロパティ
		/// </summary>
		public ModelList<CoreTweet.User> MyFollows
		{
			get
			{
				return _MyFollows;
			}
			set
			{
				if (_MyFollows == null || !_MyFollows.Equals(value))
				{
					_MyFollows = value;
					NotifyPropertyChanged("MyFollows");
				}
			}
		}
		#endregion

		#region フォロー数[FollowCount]プロパティ
		/// <summary>
		/// フォロー数[FollowCount]プロパティ用変数
		/// </summary>
		int _FollowCount = 0;
		/// <summary>
		/// フォロー数[FollowCount]プロパティ
		/// </summary>
		public int FollowCount
		{
			get
			{
				return _FollowCount;
			}
			set
			{
				if (!_FollowCount.Equals(value))
				{
					_FollowCount = value;
					NotifyPropertyChanged("FollowCount");
				}
			}
		}
		#endregion

		#region ユーザーリスト[UserList]プロパティ
		/// <summary>
		/// ユーザーリスト[UserList]プロパティ用変数
		/// </summary>
		TwitterUserCollectionM _UserList = new TwitterUserCollectionM();
		/// <summary>
		/// ユーザーリスト[UserList]プロパティ
		/// </summary>
		public TwitterUserCollectionM UserList
		{
			get
			{
				return _UserList;
			}
			set
			{
				if (_UserList == null || !_UserList.Equals(value))
				{
					_UserList = value;
					NotifyPropertyChanged("UserList");
				}
			}
		}
		#endregion



		/// <summary>
		/// ランダムフォロー実行中フラグ
		/// </summary>
		bool _ExecuteRandomFollow = false;

		Random _Rand = new Random();

		string ConfigFileName = "FollowList.xml";

		public string GetConfigFilePath()
		{
			ConfigM conf = new ConfigM();
			var tconf_dir = conf.ConfigDirPath;
			var tconf_path = Path.Combine(tconf_dir, ConfigFileName);
			return tconf_path;
		}

	public override void Init()
        {
            try
            {
				base.Init();

				// Configファイルパス
				string config_file_path = GetConfigFilePath();

				// ファイルの存在確認
				if (File.Exists(config_file_path))
				{
					// ファイルの読み込み
					this.UserList = XMLUtil.Deserialize<TwitterUserCollectionM>(config_file_path);
				}

			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}

		#region 検索
		/// <summary>
		/// 検索
		/// </summary>
		public void Search()
		{
			try
			{
				// 待ち処理
				this.TwitterAPI.Wait();

				// トークンの作成
				if (string.IsNullOrWhiteSpace(this.ScreenName))
				{
					ShowMessage.ShowNoticeOK("Screen Nameは入力必須です", "通知");
					return;
				}

				Task.Run(() =>
				{
					// フォロワーの取得
					var result = this.TwitterAPI.GetAllFollower(this.ScreenName);

					// スレッドセーフな呼び出し
					Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
					   new Action(() =>
					   {
						   this.GetAndSaveUserList(this.ScreenName, false);
					   })).Wait();
				});

			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region 自分のフォロワーに含まれているかどうかをチェックする
		/// <summary>
		/// 自分のフォロワーに含まれているかどうかをチェックする
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <returns>true:含まれる false:含まれない</returns>
		public bool ExistMyFollow(CoreTweet.User user)
        {
			return (from x in this.MyFollows.Items
					   where x.Id.Equals(user.Id)
					   select x).Count() > 0;
		}
		#endregion

		#region 合致するユーザーを抽出する
		/// <summary>
		/// 合致するユーザーを抽出する
		/// </summary>
		/// <param name="user_list">ユーザーリスト</param>
		/// <param name="follower_check">true:自分のフォローに含まれるかのチェックを行う</param>
		/// <param name="ratio_check">true:フォロー率のチェックを行う</param>
		/// <param name="word_check">単語のチェックを行う</param>
		/// <returns>条件で絞った後のリスト</returns>
		private List<CoreTweet.User> MatchUser(List<CoreTweet.User> user_list, bool follower_check = true, bool ratio_check = true, bool word_check = true)
		{
			string[] nous_list = this.DescriptionKeys.Split(',');
			var list = user_list;

			if (ratio_check)
			{
				// 上限と下限の範囲内に存在するかを確認する
				list = (from x in list
						where this.TwitterAPI.CheckFriendShipRatio(x, this.MinRatio, this.MaxRatio)
						select x).ToList<CoreTweet.User>();
			}

			if (word_check)
			{
				// 条件に該当するユーザーを取得する
				list = (from x in list
						where this.TwitterAPI.CheckDescription(nous_list, x)
						select x).ToList<CoreTweet.User>();
			}

			if (follower_check)
			{
				// 自分のフォロワーに含まれているかどうかをチェックする
				list = (from x in list
						where !ExistMyFollow(x)
						select x).ToList<CoreTweet.User>();
			}
			return list;
		}
		#endregion

		#region 条件に該当するユーザーをランダムで取得
		/// <summary>
		/// 条件に該当するユーザーをランダムで取得
		/// </summary>
		/// <returns>ユーザー</returns>
		private CoreTweet.User RandomGetUser(bool follower_check = true, bool ratio_check = true, bool word_check = true)
		{
			var list = MatchUser(this.FollowerList.Items.ToList<CoreTweet.User>(), follower_check, ratio_check, word_check);

			// フォロワー数の取得
			int max = list.Count;

			if (max > 0)
			{
				// インデックスをランダムで取得
				int index = this._Rand.Next(1, max) - 1;

				// ユーザーの取り出し
				var user = list.ElementAt(index);

				return user;
			}
			else
			{
				return null;
			}
		}
		#endregion

		#region Upsert処理
		/// <summary>
		/// Upsert処理
		/// </summary>
		/// <param name="user">ユーザーデータ</param>
		private void Upsert(CoreTweet.User user)
		{
			// nullチェック
			if (user.Id == null)
				return;

			var item = TwitterUserBase.Select(user.Id.Value);

			// データベースへ挿入
			if (item.Count > 0)
			{
				TwitterUserBase.Update(
					new TwitterUserBase()
					{
						Id = user.Id.Value,
						InserDateTime = item.ElementAt(0).InserDateTime,
						UpdateDateTime = DateTime.Now,
						ScreenName = user.ScreenName,
						Description = user.Description,
						FollowersCount = user.FollowersCount,
						FriendsCount = user.FriendsCount
					},
					new TwitterUserBase()
					{
						Id = user.Id.Value,
						InserDateTime = item.ElementAt(0).InserDateTime,
						UpdateDateTime = DateTime.Now,
						ScreenName = user.ScreenName,
						Description = user.Description,
						FollowersCount = user.FollowersCount,
						FriendsCount = user.FriendsCount
					}
					);
			}
			else
			{
				TwitterUserBase.Insert(
					new TwitterUserBase()
					{
						Id = user.Id.Value,
						InserDateTime = DateTime.Now,
						UpdateDateTime = DateTime.Now,
						ScreenName = user.ScreenName,
						Description = user.Description,
						FollowersCount = user.FollowersCount,
						FriendsCount = user.FriendsCount
					}
					);
			}
		}
		#endregion
		#region ユーザーリストの取得
		/// <summary>
		/// ユーザーリストの取得
		/// </summary>
		/// <param name="screen_name">検索するスクリーン名</param>
		/// <param name="follow_f">true:フォローの検索 false:フォロワーの検索</param>
		private void GetAndSaveUserList(string screen_name, bool follow_f = true)
		{
			try
			{
				List<CoreTweet.User> tmp;

				if (follow_f)
				{
					// フォローの取得
					tmp = this.TwitterAPI.GetAllFollows(screen_name);
				}
				else
				{
					// フォロワーの取得
					tmp = this.TwitterAPI.GetAllFollower(screen_name);
				}

				// 条件に合致するもののみ残す
				tmp = MatchUser(tmp);

				foreach (var user in tmp)
				{
					Upsert(user);
				}
			}
			catch { }

		}
		#endregion

		#region 自分のフォロワーを取得
		/// <summary>
		/// 自分のフォロワーを取得
		/// </summary>
		public void GetMyFollows()
		{
			try
			{
				Task.Run(() =>
				{
					// フォローの取得
					var tmp = this.TwitterAPI.GetAllFollows(this.ScreenName);

					// フォローリストの取得
					this.MyFollows = new ModelList<CoreTweet.User>
					{
						Items = new ObservableCollection<CoreTweet.User>(tmp)
					};
				}).Wait();
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region ランダムフォロー
		/// <summary>
		/// ランダムフォロー
		/// </summary>
		public void RandomFollow()
		{
            try
            {
				if (string.IsNullOrWhiteSpace(this.DescriptionKeys))
				{
					ShowMessage.ShowNoticeOK("キーワードは必須です", "通知");
					return;
				}

				// マイフォロワーの数を取得
				if (this.MyFollows.Items.Count <= 0)
				{
					// 自分のフォロワーを取得
					GetMyFollows();
				}

				Task.Run(() =>
				{
					// ランダムフォローが実行されていない時だけ実行する
					if (!this._ExecuteRandomFollow)
					{
						_ExecuteRandomFollow = true;
						RandomFollowSub();
						_ExecuteRandomFollow = false;
					}

					this.RepeatSearch = false;
				});
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region ランダムフォロー
		/// <summary>
		/// ランダムフォロー
		/// </summary>
		public void RandomFollowSub()
		{
			try
			{
				CoreTweet.User user = new CoreTweet.User();

				while (true)
				{
					// 繰り返し検索の中止
					if (!this.RepeatSearch)
					{
						break;
					}

					// 待ち確認
					this.TwitterAPI.Wait(60000);

					// ユーザーをランダムに取得する
					user = RandomGetUser();

					// ユーザーが取得できなかった
					if (user == null)
					{
						int max = this.UserList.Items.Count;
						int min = 1;
						int index = _Rand.Next(min, max) - 1;

						// 自分のフォローしている人の誰かを選ぶ
						var follow_user = this.MyFollows.ElementAt(index);

						// 次に検索するスクリーン名をセット
						this.ScreenName = follow_user.ScreenName;

						// フォロワーの取得
						var result = this.TwitterAPI.GetAllFollower(this.ScreenName);

                        // スレッドセーフな呼び出し
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                           new Action(() =>
                           {
                               this.FollowerList = new ModelList<CoreTweet.User>()
                               {
                                   Items = new ObservableCollection<CoreTweet.User>(result)
                               };
                           })).Wait();
                    }
                    else
                    {
						// フォロー処理
						var result = this.TwitterAPI.CreateFollow(user);

						// フォローカウントを保存
						this.FollowCount++;

						// スレッドセーフな呼び出し
						Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
						   new Action(() =>
						   {
							   this.MyFollows.Items.Add(user);
						   })).Wait();
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region 行ダブルクリック処理
		/// <summary>
		/// 行ダブルクリック処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="ev"></param>
		public void RowDoubleClick(object sender, MouseButtonEventArgs ev)
		{
			try
			{
				var dg = sender as DataGrid;
				if (null != dg.SelectedItem)
				{
					var ctrl = dg.ItemContainerGenerator.ContainerFromItem(dg.SelectedItem) as DataGridRow;
					if (null != ctrl)
					{
						if (null != ctrl.InputHitTest(ev.GetPosition(ctrl)))
						{
							var data = ctrl.DataContext as CoreTweet.User;

							if (data != null)
							{
								string url = "https://twitter.com/" + data.ScreenName;
								MovingWordpressUtilities.OpenUrl(url);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion


	}



}
