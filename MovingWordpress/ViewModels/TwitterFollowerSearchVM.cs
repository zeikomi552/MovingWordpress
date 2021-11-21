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
        #region ランダム発生用変数
        /// <summary>
        /// ランダム発生用変数
        /// </summary>
        Random _Rand = new Random();
        #endregion

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
                        TwitterUserBaseEx.Upsert(user, true, first_item.IsFollower);
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
                        TwitterUserBaseEx.Upsert(user, first_item.IsFriend, true);
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

        #region フォロバリストの作成
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
                        try
                        {
                            // ユーザーをランダムに取得する
                            var rand_user = GetRandomUser();

                            // ユーザーのフォローを取得する
                            GetTwitterUserFriends(rand_user.ScreenName);
                        }
                        catch { }

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
        #endregion

        #region 指定したユーザーのフォロー者を取得する関数
        /// <summary>
        /// 指定したユーザーのフォロー者を取得する関数
        /// </summary>
        /// <param name="screen_name">スクリーン名</param>
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

        #region 次回のフォロー時刻[NextFollowTime]プロパティ
        /// <summary>
        /// 次回のフォロー時刻[NextFollowTime]プロパティ用変数
        /// </summary>
        DateTime? _NextFollowTime = null;
        /// <summary>
        /// 次回のフォロー時刻[NextFollowTime]プロパティ
        /// </summary>
        public DateTime? NextFollowTime
        {
            get
            {
                return _NextFollowTime;
            }
            set
            {
                if (_NextFollowTime == null || !_NextFollowTime.Equals(value))
                {
                    _NextFollowTime = value;
                    NotifyPropertyChanged("NextFollowTime");
                }
            }
        }
        #endregion

        #region 前回のフォロー時刻[BeforeFollowTime]プロパティ
        /// <summary>
        /// 前回のフォロー時刻[BeforeFollowTime]プロパティ用変数
        /// </summary>
        DateTime? _BeforeFollowTime = null;
        /// <summary>
        /// 前回のフォロー時刻[BeforeFollowTime]プロパティ
        /// </summary>
        public DateTime? BeforeFollowTime
        {
            get
            {
                return _BeforeFollowTime;
            }
            set
            {
                if (_BeforeFollowTime == null || !_BeforeFollowTime.Equals(value))
                {
                    _BeforeFollowTime = value;
                    NotifyPropertyChanged("BeforeFollowTime");
                }
            }
        }
        #endregion

        #region 自動フォロー処理
        /// <summary>
        /// 自動フォロー処理
        /// </summary>
        public void AutoFollow()
        {
            Task.Run(() =>
            {
                while (AutoFollowF)
                {
                    // 自動フォロー処理
                    AutoFollowSub();


                    // 待ち時間をランダムで算出
                    int wait_tm = _Rand.Next(FollowManage.FromWait * 60 * 1000, FollowManage.ToWait * 60 * 1000);

                    // スレッドセーフな呼び出し
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       new Action(() =>
                       {
                           // 現在の時刻をセット
                           this.BeforeFollowTime = DateTime.Now;

                           // 次回のフォロー時刻
                           this.NextFollowTime = this.BeforeFollowTime.Value.AddMilliseconds(wait_tm);
                       })).Wait();

                    // 自動フォローのフラグチェック
                    while (AutoFollowF)
                    {
                        int wait_base = 1000;

                        if (wait_tm > wait_base)
                        {
                            // 待つ
                            System.Threading.Thread.Sleep(wait_base);

                            // 待ち時間を1秒減らす
                            wait_tm -= wait_base;
                        }
                        else
                        {
                            // 待つ
                            System.Threading.Thread.Sleep(wait_tm);
                            break;

                        }
                    }
                }
                // スレッドセーフな呼び出し
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                   new Action(() =>
                   {
                       this.NextFollowTime = this.BeforeFollowTime = null;
                   })).Wait();                
            });
        }
        #endregion

        #region 条件に応じてフォローする人を選定する
        /// <summary>
        /// 条件に応じてフォローする人を選定する
        /// </summary>
        /// <returns>フォロー対象ユーザー</returns>
        private TwitterUserBase GetFollowTarget()
        {
            // SQLiteファイルの存在を保証する
            using (var db = new SQLiteDataContext())
            {
                db.Database.EnsureCreated();
            }

            // クエリの発行
            var sql_user_list = TwitterUserBaseEx.SelectRangeData(
                this.UserMatch.MinRatio, this.UserMatch.MaxRatio,
                this.FollowManage.FollowRange);

            var users = sql_user_list.OrderBy(x => x.InserDateTime);

            TwitterUserBase user = null;
            foreach (var tmp_user in users)
            {
                var tmp = this.TwitterAPI.GetUserFromID(tmp_user.Id).FirstOrDefault();

                // アカウントロックされているユーザーの除外
                if (tmp.IsFollowRequestSent.HasValue && tmp.IsFollowRequestSent.Value.Equals(true))
                {
                    continue;
                }

                // プライベートユーザーの除外
                if (tmp.IsProtected.Equals(true))
                {
                    continue;
                }

                // 最終ツイート時刻からの経過日数を確認
                if (tmp.Status == null 
                    || (DateTime.Today - tmp.Status.CreatedAt.DateTime.Date).Days > this.FollowManage.ElapsedDate)
                {
                    continue;
                }

                user = tmp_user;
                break;
            }

            return user;
        }
        #endregion

        #region 自動フォロー処理
        /// <summary>
        /// 自動フォロー処理
        /// </summary>
        private void AutoFollowSub()
        {
            try
            {
                var user = GetFollowTarget();

                // 見つからなかったらリターン
                if (user == null) return;

                // フォロー
                this.TwitterAPI.CreateFollow(user.Id);

                using (var db = new SQLiteDataContext())
                {
                    // トランザクション
                    db.Database.BeginTransaction();

                    var item = db.DbSet_TwitterUser.SingleOrDefault(x => x.Id.Equals(user.Id));

                    if (item != null)
                    {
                        try
                        {
                            item.IsFriend = true;

                            // ログのインサート
                            db.Add<FollowLogBase>(new FollowLogBase()
                            {
                                RegTime = DateTime.Now,
                                Action = 0,
                                Id = user.Id
                            });

                            // データの登録
                            db.SaveChanges();

                            // コミット
                            db.Database.CommitTransaction();
                        }
                        catch
                        {
                            // 失敗したのでロールバック
                            db.Database.RollbackTransaction();
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
                    this.UserList.Items = new ObservableCollection<TwitterUserBase>();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }

        }
        #endregion

        public void GetLastTweet()
        {
            
        }

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
            worksheet.Cell("C1").Value = "フォローフラグ";
            worksheet.Cell("D1").Value = "フォロワーフラグ";
            worksheet.Cell("E1").Value = "フォロー数";
            worksheet.Cell("F1").Value = "フォロワー数";
            worksheet.Cell("G1").Value = "フォロー率";
            worksheet.Cell("H1").Value = "最終ツイート";
            worksheet.Cell("I1").Value = "最終ツイート日";
            worksheet.Cell("J1").Value = "説明文";
            worksheet.Cell("K1").Value = "アカウントURL";

            int row = 2;

            foreach (var tmp in this.UserList.Items)
            {
                worksheet.Cell($"A{row}").Value = tmp.Id;
                worksheet.Cell($"B{row}").Value = tmp.ScreenName;
                worksheet.Cell($"C{row}").Value = tmp.IsFriend;
                worksheet.Cell($"D{row}").Value = tmp.IsFollower;
                worksheet.Cell($"E{row}").Value = tmp.FriendsCount;
                worksheet.Cell($"F{row}").Value = tmp.FollowersCount;
                worksheet.Cell($"G{row}").Value = (((double)tmp.FriendsCount / (double)tmp.FollowersCount)*100.0).ToString("0.00");
                worksheet.Cell($"H{row}").Value = tmp.LastTweetDateTime.HasValue ? tmp.LastTweetDateTime.Value.ToString("yyyy/MM/dd") : string.Empty;
                worksheet.Cell($"I{row}").Value = string.IsNullOrEmpty(tmp.LastTweet) ? string.Empty : tmp.LastTweet;
                worksheet.Cell($"J{row}").Value = tmp.Description;
                worksheet.Cell($"K{row}").Value = $"https://twitter.com/{tmp.ScreenName}";
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

    }
}
