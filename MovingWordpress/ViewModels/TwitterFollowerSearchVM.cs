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

				string screen_name = this.ScreenName;

				Task.Run(() =>
				{
					long next_cursor = -1;

					for (; ; )
					{
						// 繰り返し検索がキャンセルされた
						if (!this.RepeatSearch)
							break;

						var result = this.TwitterAPI.GetFollower(next_cursor, screen_name);
						next_cursor = result.NextCursor;	// 次のカーソル取得
						this.RateLimit = result.RateLimit;	// リミット限界

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

						System.Threading.Thread.Sleep(1 * 60 * 1000);   // 1min待機
					}
				});



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
