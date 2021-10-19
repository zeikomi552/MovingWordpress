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


		/// <summary>
		/// TwitterAPIからの戻り値を保存できる形式に変換する
		/// </summary>
		/// <param name="user_list">データベースから取得したユーザーリスト</param>
		/// <returns>ユーザーリスト</returns>
		public List<TwitterUserM> ToTwitterUserM(List<MyFollowUserBase> user_list )
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
		public List<TwitterUserM> ToTwitterUserM(List<MyFollowerUserBase> user_list)
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
		public List<TwitterUserM> ToTwitterUserM(List<TwitterUserBase> user_list)
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

		#region 初期化処理
		/// <summary>
		/// 初期化処理
		/// </summary>
		public override void Init()
		{
			try
			{
				base.Init();

				this.TwitterAPI.FollowList.Items = new ObservableCollection<TwitterUserM>(ToTwitterUserM(TwitterUserBaseEx.Select()));
				//this.TwitterAPI.MyFollowerList.Items = new ObservableCollection<TwitterUserM>(ToTwitterUserM(MyFollowerUserBaseEx.Select()));
				this.TwitterAPI.MyFollowList.Items = new ObservableCollection<TwitterUserM>(ToTwitterUserM(MyFollowUserBaseEx.Select()));

			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region 検索
		/// <summary>
		/// 検索
		/// </summary>
		private void Search()
		{
			Task.Run(() =>
			{
				// フォローリスト数を確認
				int count = this.TwitterAPI.FollowList.Items.Count();

				ModelList<TwitterUserM> tmp_user_list;

				// フォロー候補リストが0以下の場合
				if (count <= 0)
				{
					// マイフォローリストが0以下の場合
					if (this.TwitterAPI.MyFollowList.Items.Count <= 0)
					{
						GetMyFollows();
					}

					// フォロー候補がない場合、マイフォローリストを使用する
					tmp_user_list = this.TwitterAPI.MyFollowList;

					if(tmp_user_list.Items.Count <= 0)
                    {
						ShowMessage.ShowNoticeOK("数名フォローしてから実行してください。", "通知");
						return;
                    }
				}
				else
				{
					// フォロー候補リストを使用する
					tmp_user_list = this.TwitterAPI.FollowList;
				}

				count = tmp_user_list.Items.Count;
				// ランダムで抜き出す
				int index = _Rand.Next(0, count);

				// ユーザーをランダムで取り出し
				var user = tmp_user_list.ElementAt(index);

				string screen_name = user.ScreenName;

				// 待ち処理
				this.TwitterAPI.Wait();

				// フォロワーの取得
				var result = this.TwitterAPI.GetUser(-1, screen_name, true);

				var tmp_user = new ModelList<TwitterUserM>();

				foreach (var tmp in this.TwitterAPI.FollowList.Items)
				{
					tmp_user.Items.Add(tmp);
				}

				foreach (var tmp in result)
				{
					tmp_user.Items.Add(new TwitterUserM(tmp));
				}

				// スレッドセーフな呼び出し
				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
				   new Action(() =>
				   {
					   this.TwitterAPI.FollowList = tmp_user;
				   })).Wait();
			});
		}
		#endregion

		#region 自分のフォロワーに含まれているかどうかをチェックする
		/// <summary>
		/// 自分のフォロワーに含まれているかどうかをチェックする
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <returns>true:含まれる false:含まれない</returns>
		public bool ExistMyFollow(TwitterUserM user)
		{
			return (from x in this.TwitterAPI.MyFollowList.Items
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
		private List<TwitterUserM> MatchUser(List<TwitterUserM> user_list, bool follower_check = true, bool ratio_check = true, bool word_check = true)
		{
			string[] nous_list = this.DescriptionKeys.Split(',');
			var list = user_list;

			if (ratio_check)
			{
				// 上限と下限の範囲内に存在するかを確認する
				list = (from x in list
						where this.TwitterAPI.CheckFriendShipRatio(x, this.MinRatio, this.MaxRatio)
						select x).ToList<TwitterUserM>();
			}

			if (word_check)
			{
				// 条件に該当するユーザーを取得する
				list = (from x in list
						where this.TwitterAPI.CheckDescription(nous_list, x)
						select x).ToList<TwitterUserM>();
			}

			if (follower_check)
			{
				// 自分のフォロワーに含まれているかどうかをチェックする
				list = (from x in list
						where !ExistMyFollow(x)
						select x).ToList<TwitterUserM>();
			}
			return list;
		}
		#endregion

		#region 自分のフォローを更新する
		/// <summary>
		/// 自分のフォローを更新する
		/// </summary>
		public void GetMyFollows()
		{
			try
			{
				this.TwitterAPI.RefreshMyFollow();
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
							var data = ctrl.DataContext as TwitterUserM;

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

		Random _Rand = new Random();
		#region フォローリストの作成
		/// <summary>
		/// フォローリストの作成
		/// </summary>
		public void CreateFollowList()
        {
			try
			{
				Search();
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
