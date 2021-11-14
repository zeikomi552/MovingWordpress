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
        #region フォロバリスト作成中フラグ[FollowBackSearchF]プロパティ
        /// <summary>
        /// フォロバリスト作成中フラグ[FollowBackSearchF]プロパティ用変数
        /// </summary>
        bool _FollowBackSearchF = false;
        /// <summary>
        /// フォロバリスト作成中フラグ[FollowBackSearchF]プロパティ
        /// </summary>
        public bool FollowBackSearchF
        {
            get
            {
                return _FollowBackSearchF;
            }
            set
            {
                if (!_FollowBackSearchF.Equals(value))
                {
                    _FollowBackSearchF = value;
                    NotifyPropertyChanged("FollowBackSearchF");
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

        #region 自動フォロー管理オブジェクト[FollowManage]プロパティ
        /// <summary>
        /// 自動フォロー管理オブジェクト[FollowManage]プロパティ用変数
        /// </summary>
        FollowManageM _FollowManage = new FollowManageM();
        /// <summary>
        /// 自動フォロー管理オブジェクト[FollowManage]プロパティ
        /// </summary>
        public FollowManageM FollowManage
        {
            get
            {
                return _FollowManage;
            }
            set
            {
                if (_FollowManage == null || !_FollowManage.Equals(value))
                {
                    _FollowManage = value;
                    NotifyPropertyChanged("FollowManage");
                }
            }
        }
        #endregion

        #region ユーザーリスト[UserList]プロパティ
        /// <summary>
        /// ユーザーリスト[UserList]プロパティ用変数
        /// </summary>
        ModelList<TwitterUserBase> _UserList = new ModelList<TwitterUserBase>();
        /// <summary>
        /// ユーザーリスト[UserList]プロパティ
        /// </summary>
        public ModelList<TwitterUserBase> UserList
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

                // データベースからユーザーのリストを取得する
                ReadDatabaseUser();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region データベースに登録されているユーザーリストの読み込み
        /// <summary>
        /// データベースに登録されているユーザーリストの読み込み
        /// </summary>
        public void ReadDatabaseUser()
        {
            // SQLiteファイルの存在を保証する
            using (var db = new SQLiteDataContext())
            {
                db.Database.EnsureCreated();
            }

            // クエリの発行
            var sql_user_list = TwitterUserBaseEx.Select();

            // データの保持
            this.UserList.Items = new ObservableCollection<TwitterUserBase>(sql_user_list);

        }
        #endregion

        #region フォローしている人を再取得してリストを修正する
        /// <summary>
        /// フォローしている人を再取得してリストを修正する
        /// </summary>
        public void AdjustFriends()
        {
            try
            {
                // フォロー情報をTwitterAPIで取得しデータベースを更新する
                GetFriendList();

                // フォロワー情報をTwitter APIで取得しデータベースを更新する
                GetFollowerList();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region フォローしている人の一覧を取得しデータベースへ保存する
        /// <summary>
        /// フォローしている人の一覧を取得しデータベースへ保存する
        /// </summary>
        private void GetFriendList()
        {
            try
            {
                // フォローしている人の情報をTwitter APIで取得する
                var user_list = this.TwitterAPI.GetUserAll(this.TwitterAPI.Config.MyScreenName, true);

                // ユーザー数分まわす
                foreach (var user in user_list)
                {
                    var first_item = (from x in this.UserList.Items
                                      where x.Id.Equals(user.Id)
                                      select x).FirstOrDefault();

                    // 既にデータベースに登録されているならば
                    if (first_item != null)
                    {
                        // 更新する
                        TwitterUserBaseEx.Upsert(user, first_item.IsFriend, first_item.IsFollower);
                    }
                    else
                    {
                        // 追加する
                        TwitterUserBaseEx.Upsert(user, true, false);
                    }
                }

                // リストを更新する
                ReadDatabaseUser();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region フォロワーリストの取得とデータベース登録
        /// <summary>
        /// フォロワーリストの取得とデータベース登録
        /// </summary>
        private void GetFollowerList()
        {
            try
            {
                // フォロワーの情報をTwitter APIで取得する
                var user_list = this.TwitterAPI.GetUserAll(this.TwitterAPI.Config.MyScreenName, false);

                // Twitterから取得したユーザー数分ループする
                foreach (var user in user_list)
                {
                    var first_item = (from x in this.UserList.Items
                                      where x.Id.Equals(user.Id)
                                      select x).FirstOrDefault();

                    // 既にデータベースに登録されているならば
                    if (first_item != null)
                    {
                        // 更新する
                        TwitterUserBaseEx.Upsert(user, first_item.IsFriend, first_item.IsFollower);
                    }
                    else
                    {
                        // 挿入する
                        TwitterUserBaseEx.Upsert(user, false, true);
                    }
                }

                // リストを更新する
                ReadDatabaseUser();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        Random _Rand = new Random();

        #region ランダムにユーザーを取り出す
        /// <summary>
        /// ランダムにユーザーを取り出す
        /// </summary>
        /// <returns>ユーザー</returns>
        private TwitterUserBase GetRandomUser()
        {
            // カウントの取得
            int count = this.UserList.Items.Count;

            // ランダムにインデックスの取得
            int index = this._Rand.Next(0, count);

            // ユーザーの取得
            return this.UserList.Items.ElementAt(index);
        }
        #endregion

        /// <summary>
        /// フォロバリストの作成
        /// </summary>
        public void FollowBackSearch()
        {
            try
            {
                Task.Run(() =>
                {
                    while (this.FollowBackSearchF)
                    {
                        // ユーザーをランダムに取得する
                        var rand_user = GetRandomUser();

                        // ユーザーのフォローを取得する
                        GetTwitterUserFriends(rand_user.ScreenName);

                        // 1minの待ち
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

        private void GetTwitterUserFriends(string screen_name)
        {
            // フォロワーの情報をTwitter APIで最初の100人だけ取得する
            var user_list = this.TwitterAPI.GetUser(-1, screen_name, true);

            // Twitterから取得したユーザー数分ループする
            foreach (var user in user_list)
            {
                var first_item = (from x in this.UserList.Items
                                  where x.Id.Equals(user.Id)
                                  select x).FirstOrDefault();

                // 既にデータベースに登録されているならば
                if (first_item != null)
                {
                    // 更新する
                    TwitterUserBaseEx.Upsert(user, first_item.IsFriend, first_item.IsFollower);
                }
                else
                {
                    string[] keys = this.UserMatch.DescriptionKeys.Split(",");

                    // キーをチェックする
                    foreach (var key in keys)
                    {
                        // キーが含まれている場合
                        if (user.Description.Contains(key.Trim()))
                        {
                            // 挿入する
                            TwitterUserBaseEx.Upsert(user, false, false);
                            break;
                        }
                    }
                }
            }

            // スレッドセーフな呼び出し
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
               new Action(() =>
               {
                   // リストを更新する
                   ReadDatabaseUser();
               })).Wait();
        }

        //#region フォロー・フォロワー・フォロバリストの更新処理
        ///// <summary>
        ///// フォロー・フォロワー・フォロバリストの更新処理
        ///// </summary>
        //public void RenewList()
        //{
        //	using (var db = new SQLiteDataContext())
        //	{
        //		db.Database.EnsureCreated();
        //	}

        //	// フォロバリストをデータベースから取得
        //	this.FilterdList.Items = this.TwitterAPI.FollowList.Items = new ObservableCollection<TwitterUserM>(TwitterUserM.ToTwitterUserM(TwitterUserBaseEx.Select()));

        //	// 自分のフォローリストをデータベースから取得
        //	this.TwitterAPI.MyFollowList.Items = new ObservableCollection<TwitterUserM>(TwitterUserM.ToTwitterUserM(MyFollowUserBaseEx.Select()));

        //	// フィルタを実行する
        //	RatioMatchFilter();
        //}
        //#endregion

        //#region リストの更新
        ///// <summary>
        ///// 指定したユーザーのフォローリストを取り出しフォロバリストを更新する
        ///// </summary>
        ///// <param name="screen_name">検索するScreenName</param>
        ///// <param name="user_limit">ユーザー数限界</param>
        //private void UpdateList(string screen_name, int user_limit)
        //{
        //	// フォロバリストの取得
        //	var result = this.TwitterAPI.GetUser(-1, screen_name, true);

        //	var tmp_user = new ModelList<TwitterUserM>();

        //	int user_num = this.TwitterAPI.FollowList.Items.Count + result.Count;
        //	int rm_count = user_num - user_limit;

        //	foreach (var tmp in this.TwitterAPI.FollowList.Items)
        //	{
        //		if (rm_count > 0 && !UserMatch.CheckFollowRatio(tmp))
        //		{
        //			// 削除数を減らす
        //			rm_count--;

        //			// データベースから削除
        //			TwitterUserBaseEx.Delete(new TwitterUserBase()
        //			{
        //				Id = tmp.Id.Value
        //			});
        //		}
        //		else
        //		{
        //			// フォロー候補リストを一時変数に保管
        //			tmp_user.Items.Add(tmp);
        //		}
        //	}

        //	foreach (var tmp in result)
        //	{
        //		var tuser = new TwitterUserM(tmp, true, false);

        //		// 期待する文字列が含まれていて、自分のフォローに含まれていない、かつ自分ではない場合に登録する
        //		if (this.UserMatch.CheckDescription(tuser)
        //			&& !this.UserMatch.CheckMyFollow(tuser, this.TwitterAPI.MyFollowList.Items.ToList<TwitterUserM>())
        //			&& !this.TwitterAPI.MyScreenName.Equals(tuser.ScreenName))
        //		{
        //			// ユーザーリストのUpsert
        //			TwitterUserBaseEx.Upsert(tmp);
        //		}
        //	}

        //	// スレッドセーフな呼び出し
        //	Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
        //	   new Action(() =>
        //	   {
        //		   // 画面に表示
        //		   this.TwitterAPI.FollowList.Items =
        //				new ObservableCollection<TwitterUserM>(TwitterUserM.ToTwitterUserM(TwitterUserBaseEx.Select()));

        //		   // フィルターフォローリストに追加
        //		   this.FilterdList.Items = this.TwitterAPI.FollowList.Items;

        //		   //// ユーザー数を確認
        //		   //CheckUser();

        //		   // フィルタを実行する
        //		   RatioMatchFilter();

        //	   })).Wait();
        //}
        //#endregion

        //#region 検索
        ///// <summary>
        ///// 検索
        ///// </summary>
        //private void Search()
        //{
        //	// フォローリスト数を確認
        //	int count = this.TwitterAPI.FollowList.Items.Count();

        //	ModelList<TwitterUserM> tmp_user_list;

        //	// フォロー候補リストが0以下の場合
        //	if (count <= 0)
        //	{
        //		// マイフォローリストが0以下の場合
        //		if (this.TwitterAPI.MyFollowList.Items.Count <= 0)
        //		{
        //			GetMyFollows();
        //		}

        //		// フォロー候補がない場合、マイフォローリストを使用する
        //		tmp_user_list = this.TwitterAPI.MyFollowList;

        //		if (tmp_user_list.Items.Count <= 0)
        //		{
        //			ShowMessage.ShowNoticeOK("数名フォローしてから実行してください。", "通知");
        //			return;
        //		}
        //	}
        //	else
        //	{
        //		// フォロー候補リストを使用する
        //		tmp_user_list = this.TwitterAPI.FollowList;
        //	}

        //	count = tmp_user_list.Items.Count;

        //	// ランダムで抜き出す
        //	int index = _Rand.Next(0, count);

        //	// ユーザーをランダムで取り出し
        //	var user = tmp_user_list.ElementAt(index);

        //	// 待ち処理
        //	while (this.RepeatSearch && !this.TwitterAPI.Wait()) ;

        //	// ユーザーの追加
        //	UpdateList(user.ScreenName, 5000);

        //	// フィルタを実行する
        //	RatioMatchFilter();
        //}
        //#endregion

        //#region 自分のフォロワーに含まれているかどうかをチェックする
        ///// <summary>
        ///// 自分のフォロワーに含まれているかどうかをチェックする
        ///// </summary>
        ///// <param name="user">ユーザー</param>
        ///// <returns>true:含まれる false:含まれない</returns>
        //public bool ExistMyFollow(TwitterUserM user)
        //{
        //	return (from x in this.TwitterAPI.MyFollowList.Items
        //			where x.Id.Equals(user.Id)
        //			select x).Count() > 0;
        //}
        //#endregion

        //#region 自分のフォローを更新する
        ///// <summary>
        ///// 自分のフォローを更新する
        ///// </summary>
        //public void GetMyFollows()
        //{
        //	try
        //	{
        //		this.TwitterAPI.RefreshMyFollow();
        //	}
        //	catch (Exception e)
        //	{
        //		_logger.Error(e.Message);
        //		ShowMessage.ShowErrorOK(e.Message, "Error");
        //	}
        //}
        //#endregion

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
                            var data = ctrl.DataContext as TwitterUserBase;

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


        //#region フォローリストの作成
        ///// <summary>
        ///// フォローリストの作成
        ///// </summary>
        //public void CreateFollowList()
        //      {
        //	try
        //	{
        //              Task.Run(() =>
        //              {
        //			while (this.RepeatSearch)
        //			{
        //				try
        //				{
        //					//GetLimit();

        //					// 検索処理
        //					Search();

        //				}
        //				catch { }

        //				// 60秒に一度実行する
        //				System.Threading.Thread.Sleep(60 * 1000);
        //			}
        //              });
        //          }
        //	catch (Exception e)
        //	{
        //		_logger.Error(e.Message);
        //		ShowMessage.ShowErrorOK(e.Message, "Error");
        //	}
        //}
        //#endregion

        //      #region 説明文でフィルタする
        //      /// <summary>
        //      /// 説明文でフィルタする
        //      /// </summary>
        //      public void KeysMatchFilter()
        //{
        //	try
        //	{
        //		this.FilterdList.Items 
        //			= new ObservableCollection<TwitterUserM>(this.UserMatch.FilterdKeys(this.TwitterAPI.FollowList.Items.ToList<TwitterUserM>()));
        //	}
        //	catch (Exception e)
        //	{
        //		_logger.Error(e.Message);
        //		ShowMessage.ShowErrorOK(e.Message, "Error");
        //	}
        //}
        //#endregion

        //#region フォロー率でフィルタする
        ///// <summary>
        ///// フォロー率でフィルタする
        ///// </summary>
        //public void RatioMatchFilter()
        //{
        //	try
        //	{
        //		this.FilterdList.Items = new ObservableCollection<TwitterUserM>((from x in this.TwitterAPI.FollowList.Items
        //																		 where this.UserMatch.CheckFollowRatio(x)
        //																		 select x).ToList<TwitterUserM>());
        //	}
        //	catch (Exception e)
        //	{
        //		_logger.Error(e.Message);
        //		ShowMessage.ShowErrorOK(e.Message, "Error");
        //	}
        //}
        //#endregion

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

        //#region クリア
        ///// <summary>
        ///// クリア
        ///// </summary>
        //public void ClearFollowBackList()
        //{
        //	try
        //	{
        //		if (ShowMessage.ShowQuestionYesNo("フォロバリストを削除してもよろしいですか？", "確認") == MessageBoxResult.Yes)
        //		{
        //			// データベースの削除
        //			TwitterUserBaseEx.Delete();

        //			// 要素のクリア
        //			this.TwitterAPI.MyFollowList.Items = new ObservableCollection<TwitterUserM>();
        //		}
        //	}
        //	catch (Exception e)
        //	{
        //		_logger.Error(e.Message);
        //		ShowMessage.ShowErrorOK(e.Message, "Error");
        //	}

        //}
        //#endregion

        //#region マイフォローリストの削除
        ///// <summary>
        ///// マイフォローリストの削除
        ///// </summary>
        //public void ClearMyFollow()
        //{
        //	try
        //	{
        //		if (ShowMessage.ShowQuestionYesNo("マイフォローリストを削除してもよろしいですか？", "確認") == MessageBoxResult.Yes)
        //		{
        //			// 自分のフォローユーザーの情報をクリア
        //			MyFollowUserBaseEx.Delete();

        //			// 要素のクリア
        //			this.TwitterAPI.MyFollowList.Items = new ObservableCollection<TwitterUserM>();
        //		}
        //	}
        //	catch (Exception e)
        //	{
        //		_logger.Error(e.Message);
        //		ShowMessage.ShowErrorOK(e.Message, "Error");
        //	}
        //}
        //#endregion

        //#region 全記事分の分析結果をエクセルに出力する
        ///// <summary>
        ///// 全記事分の分析結果をエクセルに出力する
        ///// </summary>
        ///// <param name="workbook">ワークブック</param>
        //private void SaveExcelSub(XLWorkbook workbook)
        //{
        //	var worksheet = workbook.Worksheets.Add("全体");

        //	worksheet.Cell("A1").Value = "ID";
        //	worksheet.Cell("B1").Value = "スクリーン名";
        //	worksheet.Cell("C1").Value = "フォロー数";
        //	worksheet.Cell("D1").Value = "フォロワー数";
        //	worksheet.Cell("E1").Value = "フォロー率";
        //	worksheet.Cell("F1").Value = "説明文";
        //	worksheet.Cell("G1").Value = "アカウントURL";

        //	int row = 2;

        //	foreach (var tmp in this.FilterdList.Items)
        //	{
        //		worksheet.Cell($"A{row}").Value = tmp.Id;
        //		worksheet.Cell($"B{row}").Value = tmp.ScreenName;
        //		worksheet.Cell($"C{row}").Value = tmp.FriendsCount;
        //		worksheet.Cell($"D{row}").Value = tmp.FollowersCount;
        //		worksheet.Cell($"E{row}").Value = tmp.FriendshipRatio;
        //		worksheet.Cell($"F{row}").Value = tmp.Description;
        //		worksheet.Cell($"G{row}").Value = $"https://twitter.com/{tmp.ScreenName}";
        //		row++;
        //	}
        //}
        //#endregion

        //#region Excelを保存する
        ///// <summary>
        ///// Excelを保存する
        ///// </summary>
        //public void SaveExcel()
        //{
        //	try
        //	{
        //		// ダイアログのインスタンスを生成
        //		var dialog = new SaveFileDialog();

        //		// ファイルの種類を設定
        //		dialog.Filter = "エクセルファイル(*.xlsx)|*.xlsx";

        //		// ダイアログを表示する
        //		if (dialog.ShowDialog() == true)
        //		{
        //			using (var workbook = new XLWorkbook())
        //			{
        //				SaveExcelSub(workbook);
        //				workbook.SaveAs(dialog.FileName);

        //				ShowMessage.ShowNoticeOK("レポート出力が完了しました。", "通知");

        //			}
        //		}
        //	}
        //	catch (Exception e)
        //	{
        //		_logger.Error(e.Message);
        //		ShowMessage.ShowErrorOK(e.Message, "Error");
        //	}
        //}
        //#endregion

        //#region 自動フォローフラグ[AutoFollowF]プロパティ
        ///// <summary>
        ///// 自動フォローフラグ[AutoFollowF]プロパティ用変数
        ///// </summary>
        //bool _AutoFollowF = false;
        ///// <summary>
        ///// 自動フォローフラグ[AutoFollowF]プロパティ
        ///// </summary>
        //public bool AutoFollowF
        //{
        //	get
        //	{
        //		return _AutoFollowF;
        //	}
        //	set
        //	{
        //		if (!_AutoFollowF.Equals(value))
        //		{
        //			_AutoFollowF = value;
        //			NotifyPropertyChanged("AutoFollowF");
        //		}
        //	}
        //}
        //#endregion

        //#region 選択行のフォロー
        ///// <summary>
        ///// 選択行のフォロー
        ///// </summary>
        //public void SelectedRowFollow()
        //{
        //	try
        //	{
        //		// 選択行のフォロー
        //		var user = FilterdList.SelectedItem;

        //		// フォロー作成処理
        //		CreateFollow(user);
        //	}
        //	catch (Exception e)
        //	{
        //		_logger.Error(e.Message);
        //		ShowMessage.ShowErrorOK(e.Message, "Error");
        //	}
        //}
        //#endregion

        //#region 選択行のフォロー
        ///// <summary>
        ///// 選択行のフォロー
        ///// </summary>
        //private void CreateFollow(TwitterUserM user)
        //{
        //	// リストから削除する
        //	TwitterUserBaseEx.Delete(new TwitterUserBase() { Id = user.Id.Value });

        //	// 自分のフォローリストに加える
        //	MyFollowUserBaseEx.Upsert(user);
        //}
        //#endregion

        //#region 自動フォロー
        ///// <summary>
        ///// 自動フォロー
        ///// </summary>
        //public void AutoFollow()
        //{
        //	try
        //	{
        //		Task.Run(() =>
        //		{
        //			// タイマー割り込みにしないとうまくいかない！！！


        //			var tmp = new ObservableCollection<TwitterUserM>((from x in this.TwitterAPI.FollowList.Items
        //															  where this.UserMatch.CheckFollowRatio(x)
        //															  select x).ToList<TwitterUserM>());
        //			foreach (var user in tmp)
        //			{


        //				// 自動フォローフラグがONの場合は抜ける
        //				if (!this.AutoFollowF)
        //					break;

        //				if (user.Id.HasValue)
        //				{
        //					// フォローを実行する
        //					CreateFollow(user);

        //					// 自動フォローフラグがONの場合は抜ける
        //					if (!this.AutoFollowF)
        //						break;

        //					int msec = _Rand.Next(this.FollowManage.FromWait * 1000, this.FollowManage.ToWait * 1000);

        //					while (this.FollowManage.CheckWait(msec))
        //					{
        //						if (msec > 1000)
        //						{
        //							// 1min～10minの間でランダムにスリープ
        //							System.Threading.Thread.Sleep(1000);
        //							msec = msec - 1000;
        //						}
        //						else
        //						{
        //							// 1min～10minの間でランダムにスリープ
        //							System.Threading.Thread.Sleep(msec);
        //						}

        //						// 自動フォローフラグがONの場合は抜ける
        //						if (!this.AutoFollowF)
        //							break;
        //					}
        //				}
        //			}
        //		});
        //	}
        //	catch (Exception e)
        //	{
        //		_logger.Error(e.Message);
        //		ShowMessage.ShowErrorOK(e.Message, "Error");
        //	}
        //}
        //#endregion

        //public void GetLimit()
        //{

        //	var tmp = this.TwitterAPI.GetRateLimit();

        //	StringBuilder text = new StringBuilder();

        //	text.AppendLine("|リソース群|タイトル|制限(15分でリセット)|");
        //	text.AppendLine("|---|---|---|");
        //	foreach (var key in tmp.Keys)
        //	{
        //		foreach (var key2 in tmp)
        //		{
        //			foreach (var key3 in key2.Value)
        //			{
        //				text.AppendLine($"|{key2.Key}|{key3.Key}|{key3.Value.Limit}|");

        //			}

        //		}
        //	}
        //	string limit_md = text.ToString();
        //}

    }
}
