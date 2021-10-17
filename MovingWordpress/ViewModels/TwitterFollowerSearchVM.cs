using MovingWordpress.Common;
using MovingWordpress.Models.Tweet;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
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

		Random _Rand = new Random();

		private void SearchSub()
        {
            try
            {
				string screen_name = this.ScreenName;
				long next_cursor = -1;

				for (; ; )
				{
					// 繰り返し検索がキャンセルされた
					if (!this.RepeatSearch)
						break;

					// フォロワーの取得
					var result = this.TwitterAPI.GetFollower(next_cursor, screen_name);
					next_cursor = result.NextCursor;    // 次のカーソル取得
					this.RateLimit = result.RateLimit;  // リミット限界

					foreach (var tmp in result)
					{
						// 既に登録されているかチェック
						var user = (from x in this.FollowerList.Items
									where x.ScreenName.Equals(tmp.ScreenName)
									select x);

						// 未追加の場合追加
						if (user == null || user.ToList().Count() <= 0)
						{
							// スレッドセーフな呼び出し
							Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
							   new Action(() =>
							   {
								   this.FollowerList.Items.Add(tmp);
							   })).Wait();
						}
					}

					// フォロワー数ゼロの人にあたった場合
					if (this._FollowerList.Items.Count <= 0)
					{
						this.RepeatSearch = false;
						break;
					}

					// 次のカーソルが見つからなかった場合、次の人へ移動
					if (next_cursor == 0)
					{
						int index = this._Rand.Next(0, this._FollowerList.Items.Count - 1);
						screen_name = this._FollowerList.ElementAt(index).ScreenName;
						next_cursor = -1;
					}

					// 次のカーソルが無い場合は抜ける
					if (next_cursor == 0)
					{
						// スレッドセーフな呼び出し
						Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
						   new Action(() =>
						   {
							   this.RepeatSearch = false;
						   }));
						break;
					}

					// リセット時間を確認して待ち処理を入れる
					while (!this.TwitterAPI.Wait(this.RateLimit)) ;

					// 検索中かのチェック
					if (!this.RepeatSearch)
						break;

					System.Threading.Thread.Sleep(60000);
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
				// トークンの作成
				if (string.IsNullOrWhiteSpace(this.ScreenName))
				{
					ShowMessage.ShowNoticeOK("Screen Nameは入力必須です", "通知");
					return;
				}

				Task.Run(() =>
				{
					// 検索処理
					SearchSub();
				});

			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion


		/// <summary>
		/// 自分のフォロワーに含まれているかどうかをチェックする
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <returns>true:含まれる false:含まれない</returns>
		public bool ExistMyFollower(CoreTweet.User user)
        {
			return (from x in this._MyFollowers
					   where x.Id.Equals(user.Id)
					   select x).Count() > 0;
		}

		/// <summary>
		/// 条件に該当するユーザーをランダムで取得
		/// </summary>
		/// <returns>ユーザー</returns>
		private CoreTweet.User RandomGetUser()
		{
			string[] nous_list = this.DescriptionKeys.Split(',');

			// 上限と下限の範囲内に存在するかを確認する
			var list = (from x in this.FollowerList.Items
						where this.TwitterAPI.CheckFriendShipRatio(x, this.MinRatio, this.MaxRatio)
						select x).ToList<CoreTweet.User>();

			// 条件に該当するユーザーを取得する
			list = (from x in list
				   where this.TwitterAPI.CheckDescription(nous_list, x)
				   select x).ToList<CoreTweet.User>();

			// 自分のフォロワーに含まれているかどうかをチェックする
			list = (from x in list
				   where ExistMyFollower(x)
				   select x).ToList<CoreTweet.User>();

			// フォロワー数の取得
			int max = list.Count;

			if (max > 0)
			{
				// インデックスをランダムで取得
				int index = this._Rand.Next(1, max) - 1;


				// ユーザーの取り出し
				var user = this.FollowerList.Items.ElementAt(index);

				return user;
			}
			else
			{
				return null;
			}
		}
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

		private List<CoreTweet.User> _MyFollowers = null;
		/// <summary>
		/// ランダムフォロー
		/// </summary>
		public void RandomFollow()
		{
            try
            {
				this._MyFollowers = this.TwitterAPI.GetAllFollower(this.TwitterAPI.Config.MyScreenName);

				Task.Run(() =>
				{
					RandomFollowSub();
				}).Wait();
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}

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
					// ユーザーをランダムに取得する
					user = RandomGetUser();

					// ユーザーが取得できなかった
					if (user == null)
						return;

					// フォロー処理
					var result = this.TwitterAPI.CreateFollow(user.Id.Value);

					// 残りが0になったので解除されるまで待つ
					if (result.RateLimit.Remaining == 0)
					{
						// ローカル時刻と比較
						while (DateTime.Now.CompareTo(result.RateLimit.Reset.LocalDateTime) <= 0)
						{
							System.Threading.Thread.Sleep(1000);
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
