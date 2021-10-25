using ClosedXML.Excel;
using Microsoft.Win32;
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

		#region ユーザーがマッチするかどうかをチェックするオブジェクト[UserMatch]プロパティ
		/// <summary>
		/// ユーザーがマッチするかどうかをチェックするオブジェクト[UserMatch]プロパティ用変数
		/// </summary>
		UserMatchM _UserMatch = new UserMatchM();
		/// <summary>
		/// ユーザーがマッチするかどうかをチェックするオブジェクト[UserMatch]プロパティ
		/// </summary>
		public UserMatchM UserMatch
		{
			get
			{
				return _UserMatch;
			}
			set
			{
				if (_UserMatch == null || !_UserMatch.Equals(value))
				{
					_UserMatch = value;
					NotifyPropertyChanged("UserMatch");
				}
			}
		}
		#endregion

		#region 説明文に期待するキーワードが含まれる人数[KeysMatchUserCount]プロパティ
		/// <summary>
		/// 説明文に期待するキーワードが含まれる人数[KeysMatchUserCount]プロパティ用変数
		/// </summary>
		int _KeysMatchUserCount = 0;
		/// <summary>
		/// 説明文に期待するキーワードが含まれる人数[KeysMatchUserCount]プロパティ
		/// </summary>
		public int KeysMatchUserCount
		{
			get
			{
				return _KeysMatchUserCount;
			}
			set
			{
				if (!_KeysMatchUserCount.Equals(value))
				{
					_KeysMatchUserCount = value;
					NotifyPropertyChanged("KeysMatchUserCount");
				}
			}
		}
		#endregion

		#region フォロー率が期待する範囲いある人数[RatioMatchUserCount]プロパティ
		/// <summary>
		/// フォロー率が期待する範囲いある人数[RatioMatchUserCount]プロパティ用変数
		/// </summary>
		int _RatioMatchUserCount = 0;
		/// <summary>
		/// フォロー率が期待する範囲いある人数[RatioMatchUserCount]プロパティ
		/// </summary>
		public int RatioMatchUserCount
		{
			get
			{
				return _RatioMatchUserCount;
			}
			set
			{
				if (!_RatioMatchUserCount.Equals(value))
				{
					_RatioMatchUserCount = value;
					NotifyPropertyChanged("RatioMatchUserCount");
				}
			}
		}
		#endregion

		#region 自分がフォローしていない人数[NonFollowCount]プロパティ
		/// <summary>
		/// 自分がフォローしていない人数[NonFollowCount]プロパティ用変数
		/// </summary>
		int _NonFollowCount = 0;
		/// <summary>
		/// 自分がフォローしていない人数[NonFollowCount]プロパティ
		/// </summary>
		public int NonFollowCount
		{
			get
			{
				return _NonFollowCount;
			}
			set
			{
				if (!_NonFollowCount.Equals(value))
				{
					_NonFollowCount = value;
					NotifyPropertyChanged("NonFollowCount");
				}
			}
		}
		#endregion

		#region フォロー候補[FilterdList]プロパティ
		/// <summary>
		/// フォロー候補[FilterdList]プロパティ用変数
		/// </summary>
		ModelList<TwitterUserM> _FilterdList = new ModelList<TwitterUserM>();
		/// <summary>
		/// フォロー候補[FilterdList]プロパティ
		/// </summary>
		public ModelList<TwitterUserM> FilterdList
		{
			get
			{
				return _FilterdList;
			}
			set
			{
				if (_FilterdList == null || !_FilterdList.Equals(value))
				{
					_FilterdList = value;
					NotifyPropertyChanged("FilterdList");
				}
			}
		}
		#endregion

		#region 初期化処理
		/// <summary>
		/// 初期化処理
		/// </summary>
		public override void Init()
		{
			try
			{
				base.Init();

				// ユーザーのマッチ条件をロード
				this.UserMatch.Load();

				using (var db = new SQLiteDataContext())
				{
					db.Database.EnsureCreated();
				}

				// フォロバリストをデータベースから取得
				this.FilterdList.Items = this.TwitterAPI.FollowList.Items = new ObservableCollection<TwitterUserM>(TwitterUserM.ToTwitterUserM(TwitterUserBaseEx.Select()));

				// 自分のフォローリストをデータベースから取得
				this.TwitterAPI.MyFollowList.Items = new ObservableCollection<TwitterUserM>(TwitterUserM.ToTwitterUserM(MyFollowUserBaseEx.Select()));

				// ユーザー数を確認
				CheckUser();
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region リストの更新
		/// <summary>
		/// 指定したユーザーのフォローリストを取り出しフォロバリストを更新する
		/// </summary>
		/// <param name="screen_name">検索するScreenName</param>
		/// <param name="user_limit">ユーザー数限界</param>
		private void UpdateList(string screen_name, int user_limit)
		{
			// フォロワーの取得
			var result = this.TwitterAPI.GetUser(-1, screen_name, true);

			var tmp_user = new ModelList<TwitterUserM>();

			int user_num = this.TwitterAPI.FollowList.Items.Count + result.Count;
			int rm_count = user_num - user_limit;

			foreach (var tmp in this.TwitterAPI.FollowList.Items)
			{
				if (rm_count > 0 && !UserMatch.CheckFollowRatio(tmp))
				{
					// 削除数を減らす
					rm_count--;

					// データベースから削除
					TwitterUserBaseEx.Delete(new TwitterUserBase()
					{
						Id = tmp.Id.Value
					});
				}
				else
				{
					// フォロー候補リストを一時変数に保管
					tmp_user.Items.Add(tmp);
				}
			}

			foreach (var tmp in result)
			{
				var tuser = new TwitterUserM(tmp);

				// 期待する文字列が含まれていて、自分のフォローに含まれていない、かつ自分ではない場合に登録する
				if (this.UserMatch.CheckDescription(tuser)
					&& !this.UserMatch.CheckMyFollow(tuser, this.TwitterAPI.MyFollowList.Items.ToList<TwitterUserM>())
					&& !this.TwitterAPI.MyScreenName.Equals(tuser.ScreenName))
				{
					// ユーザーリストのUpsert
					TwitterUserBaseEx.Upsert(tmp);
				}
			}

			// スレッドセーフな呼び出し
			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
			   new Action(() =>
			   {
				   // 画面に表示
				   this.TwitterAPI.FollowList.Items =
						new ObservableCollection<TwitterUserM>(TwitterUserM.ToTwitterUserM(TwitterUserBaseEx.Select()));

				   // フィルターフォローリストに追加
				   this.FilterdList.Items = this.TwitterAPI.FollowList.Items;

				   // ユーザー数を確認
				   CheckUser();
			   })).Wait();
		}
		#endregion

		#region 検索
		/// <summary>
		/// 検索
		/// </summary>
		private void Search()
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

				if (tmp_user_list.Items.Count <= 0)
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

			// 待ち処理
			while (this.RepeatSearch && !this.TwitterAPI.Wait()) ;

			// ユーザーの追加
			UpdateList(user.ScreenName, 5000);
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
                Task.Run(() =>
                {
					while (this.RepeatSearch)
					{
						try
						{
							//GetLimit();

							// 検索処理
							Search();

						}
						catch { }

						// 60秒に一度実行する
						System.Threading.Thread.Sleep(60 * 1000);
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

		#region ユーザーの内容をチェックする
		/// <summary>
		/// ユーザーの内容をチェックする
		/// </summary>
		public void CheckUser()
		{
			this.KeysMatchUserCount = (from x in this.TwitterAPI.FollowList.Items
									  where this.UserMatch.CheckDescription(x)
									  select x).Count();

			this.RatioMatchUserCount = (from x in this.TwitterAPI.FollowList.Items
									   where this.UserMatch.CheckFollowRatio(x)
									   select x).Count();

			this.NonFollowCount = (from x in this.TwitterAPI.FollowList.Items
										where !this.UserMatch.CheckMyFollow(x, this.TwitterAPI.MyFollowList.Items.ToList<TwitterUserM>())
										select x).Count();

		}
		#endregion

        #region 説明文でフィルタする
        /// <summary>
        /// 説明文でフィルタする
        /// </summary>
        public void KeysMatchFilter()
		{
			try
			{
				this.FilterdList.Items 
					= new ObservableCollection<TwitterUserM>(this.UserMatch.FilterdKeys(this.TwitterAPI.FollowList.Items.ToList<TwitterUserM>()));
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region フォロー率でフィルタする
		/// <summary>
		/// フォロー率でフィルタする
		/// </summary>
		public void RatioMatchFilter()
		{
			try
			{
				this.FilterdList.Items = new ObservableCollection<TwitterUserM>((from x in this.TwitterAPI.FollowList.Items
																				 where this.UserMatch.CheckFollowRatio(x)
																				 select x).ToList<TwitterUserM>());
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region 画面を閉じる時の処理
		/// <summary>
		/// 画面を閉じる時の処理
		/// </summary>
		public void Close()
		{
			try
			{
				// 保存処理
				this.UserMatch.Save();
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region クリア
		/// <summary>
		/// クリア
		/// </summary>
		public void ClearFollowBackList()
		{
			try
			{
				if (ShowMessage.ShowQuestionYesNo("フォロバリストを削除してもよろしいですか？", "確認") == MessageBoxResult.Yes)
				{
					// データベースの削除
					TwitterUserBaseEx.Delete();

					// 要素のクリア
					this.TwitterAPI.MyFollowList.Items = new ObservableCollection<TwitterUserM>();
				}
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}

		}
		#endregion

		#region マイフォローリストの削除
		/// <summary>
		/// マイフォローリストの削除
		/// </summary>
		public void ClearMyFollow()
		{
			try
			{
				if (ShowMessage.ShowQuestionYesNo("マイフォローリストを削除してもよろしいですか？", "確認") == MessageBoxResult.Yes)
				{
					// 自分のフォローユーザーの情報をクリア
					MyFollowUserBaseEx.Delete();

					// 要素のクリア
					this.TwitterAPI.MyFollowList.Items = new ObservableCollection<TwitterUserM>();
				}
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region 全記事分の分析結果をエクセルに出力する
		/// <summary>
		/// 全記事分の分析結果をエクセルに出力する
		/// </summary>
		/// <param name="workbook">ワークブック</param>
		private void SaveExcelSub(XLWorkbook workbook)
		{
			var worksheet = workbook.Worksheets.Add("全体");

			worksheet.Cell("A1").Value = "ID";
			worksheet.Cell("B1").Value = "スクリーン名";
			worksheet.Cell("C1").Value = "フォロー数";
			worksheet.Cell("D1").Value = "フォロワー数";
			worksheet.Cell("E1").Value = "フォロー率";
			worksheet.Cell("F1").Value = "説明文";
			worksheet.Cell("G1").Value = "アカウントURL";

			int row = 2;

			foreach (var tmp in this.TwitterAPI.FollowList.Items)
			{
				worksheet.Cell($"A{row}").Value = tmp.Id;
				worksheet.Cell($"B{row}").Value = tmp.ScreenName;
				worksheet.Cell($"C{row}").Value = tmp.FriendsCount;
				worksheet.Cell($"D{row}").Value = tmp.FollowersCount;
				worksheet.Cell($"E{row}").Value = tmp.FriendshipRatio;
				worksheet.Cell($"F{row}").Value = tmp.Description;
				worksheet.Cell($"G{row}").Value = $"https://twitter.com/{tmp.ScreenName}";
				row++;
			}
		}
		#endregion

		#region Excelを保存する
		/// <summary>
		/// Excelを保存する
		/// </summary>
		public void SaveExcel()
		{
			try
			{
				// ダイアログのインスタンスを生成
				var dialog = new SaveFileDialog();

				// ファイルの種類を設定
				dialog.Filter = "エクセルファイル(*.xlsx)|*.xlsx";

				// ダイアログを表示する
				if (dialog.ShowDialog() == true)
				{
					using (var workbook = new XLWorkbook())
					{
						SaveExcelSub(workbook);
						workbook.SaveAs(dialog.FileName);

						ShowMessage.ShowNoticeOK("レポート出力が完了しました。", "通知");

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

		#region 自動フォローフラグ[AutoFollowF]プロパティ
		/// <summary>
		/// 自動フォローフラグ[AutoFollowF]プロパティ用変数
		/// </summary>
		bool _AutoFollowF = false;
		/// <summary>
		/// 自動フォローフラグ[AutoFollowF]プロパティ
		/// </summary>
		public bool AutoFollowF
		{
			get
			{
				return _AutoFollowF;
			}
			set
			{
				if (!_AutoFollowF.Equals(value))
				{
					_AutoFollowF = value;
					NotifyPropertyChanged("AutoFollowF");
				}
			}
		}
		#endregion

		#region 自動フォロー
		/// <summary>
		/// 自動フォロー
		/// </summary>
		public void AutoFollow()
		{
			try
			{
				Task.Run(() =>
				{
					var tmp = new ObservableCollection<TwitterUserM>((from x in this.TwitterAPI.FollowList.Items
																	  where this.UserMatch.CheckFollowRatio(x)
																	  select x).ToList<TwitterUserM>());
					foreach (var user in tmp)
					{
						// 自動フォローフラグがONの場合は抜ける
						if (!this.AutoFollowF)
							break;

						if (user.Id.HasValue)
						{
							this.TwitterAPI.CreateFollow(user.Id.Value);

							// 自動フォローフラグがONの場合は抜ける
							if (!this.AutoFollowF)
								break;

							// リストから削除する
							TwitterUserBaseEx.Delete(new TwitterUserBase() { Id = user.Id.Value });

							// フォロバリストをデータベースから取得
							this.FilterdList.Items = this.TwitterAPI.FollowList.Items
								= new ObservableCollection<TwitterUserM>(TwitterUserM.ToTwitterUserM(TwitterUserBaseEx.Select()));

							// ユーザー数を確認
							CheckUser();


							// 自動フォローフラグがONの場合は抜ける
							if (!this.AutoFollowF)
								break;

							// 60秒に一度実行する
							System.Threading.Thread.Sleep(3 * 60 * 1000);
						}
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

		public void GetLimit()
		{

			var tmp = this.TwitterAPI.GetRateLimit();

			StringBuilder text = new StringBuilder();

			text.AppendLine("|リソース群|タイトル|制限(15分でリセット)|");
			text.AppendLine("|---|---|---|");
			foreach (var key in tmp.Keys)
			{
				foreach (var key2 in tmp)
				{
					foreach (var key3 in key2.Value)
					{
						text.AppendLine($"|{key2.Key}|{key3.Key}|{key3.Value.Limit}|");

					}

				}
			}
			string limit_md = text.ToString();
		}

	}
}
