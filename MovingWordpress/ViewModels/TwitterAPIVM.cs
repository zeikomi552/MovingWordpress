using Microsoft.Win32;
using MovingWordpress.Common;
using MovingWordpress.Models;
using MovingWordpress.Models.Tweet;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovingWordpress.ViewModels
{
    public class TwitterAPIVM : ViewModelBase
    {
        #region ブログ記事[WordpressContents]プロパティ
        /// <summary>
        /// ブログ記事[WordpressContents]プロパティ用変数
        /// </summary>
        ModelList<WpContentsM> _WordpressContents = new ModelList<WpContentsM>();
        /// <summary>
        /// ブログ記事[WordpressContents]プロパティ
        /// </summary>
        public ModelList<WpContentsM> WordpressContents
        {
            get
            {
                return _WordpressContents;
            }
            set
            {
                if (_WordpressContents == null || !_WordpressContents.Equals(value))
                {
                    _WordpressContents = value;
                    NotifyPropertyChanged("WordpressContents");
                }
            }
        }
        #endregion

        #region TwitterAPI用オブジェクト[TwitterAPI]プロパティ
        /// <summary>
        /// TwitterAPI用オブジェクト[TwitterAPI]プロパティ用変数
        /// </summary>
        TwitterM _TwitterAPI = new TwitterM();
        /// <summary>
        /// TwitterAPI用オブジェクト[TwitterAPI]プロパティ
        /// </summary>
        public TwitterM TwitterAPI
        {
            get
            {
                return _TwitterAPI;
            }
            set
            {
                if (_TwitterAPI == null || !_TwitterAPI.Equals(value))
                {
                    _TwitterAPI = value;
                    NotifyPropertyChanged("TwitterAPI");
                }
            }
        }
        #endregion


        /// <summary>
        /// 初期化処理
        /// </summary>
        public virtual void Init()
        {
            try
            {
                // コンフィグファイルのロード
                this.TwitterAPI.Config.Load();
            }
            catch (Exception e)
            {

                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

        #region ツイート
        /// <summary>
        /// ツイート
        /// </summary>
        public void Tweet()
        {
            try
            {
                // 送信文字列をチェック
                if (!string.IsNullOrEmpty(this.TwitterAPI.Config.TempleteM.Message)
                    && this.WordpressContents.SelectedItem != null)
                {
                    // メッセージの送信処理
                    this.TwitterAPI.Tweet(this.TwitterAPI.Config.TempleteM.Message);
                    
                    ShowMessage.ShowNoticeOK("送信しました。", "通知");
                }
                else
                {
                    ShowMessage.ShowNoticeOK("送信データがありません", "通知");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region バックアップファイルを開く処理
        /// <summary>
        /// バックアップファイルを開く処理
        /// </summary>
        public void LoadBackup()
        {
            try
            {
                // ダイアログのインスタンスを生成
                var dialog = new OpenFileDialog();

                List<WpContentsM> ret = new List<WpContentsM>();

                // ファイルの種類を設定
                dialog.Filter = "データベースまたはタグカテファイル (*.sql.gz;*.wmcate)|*.sql.gz;*.wmcate";
                // ダイアログを表示する
                if (dialog.ShowDialog() == true)
                {
                    // 拡張子の取得
                    string ext = Path.GetExtension(dialog.FileName).ToLower();

                    if (ext.Equals(".wmcate"))
                    {
                        TagCateM tag = new TagCateM();
                        var tmp = tag.Load(dialog.FileName);    // ファイルのロード

                        this.WordpressContents.Items = tmp.BlogContents.Items;   // 個別記事の情報を取得

                        // 1件以上記事が存在する
                        if (tmp.BlogContents.Items.Count > 0)
                        {
                            this.WordpressContents.SelectedItem = tmp.BlogContents.Items.First();   // 先頭記事を選択
                        }
                    }
                    else
                    {
                        // データベースのバックアップファイルのロード
                        var tmp = FileAnalyzerM.LoadContents(dialog.FileName);

                        // コンテンツのセット
                        this.WordpressContents.Items
                            = new ObservableCollection<WpContentsM>(tmp);
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

        #region 設定値の保存処理
        /// <summary>
        /// 設定値の保存処理
        /// </summary>
        public void SaveSetting()
        {
            try
            {
                // コンフィグファイルのセーブ
                this.TwitterAPI.Config.Save();
                ShowMessage.ShowNoticeOK("設定を保存しました。", "通知");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 選択要素の変更
        /// <summary>
        /// 選択要素の変更
        /// </summary>
        public void SelectionChanged()
        {
            try
            {
                if (this.WordpressContents != null
                    && this.WordpressContents.SelectedItem != null)
                {
                    this.TwitterAPI.Config.TempleteM.Title = this.WordpressContents.SelectedItem.Post_title;
                    this.TwitterAPI.Config.TempleteM.URL = this.WordpressContents.SelectedItem.Guid;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        Random _rand = new Random();

        #region ランダム選択
        /// <summary>
        /// ランダム選択
        /// </summary>
        public void RandomSelect()
        {
            try
            {
                // nullチェック
                if (this.WordpressContents.Items != null && this.WordpressContents.Items.Count > 0)
                {
                    // カウントの取得
                    int count = this.WordpressContents.Items.Count;

                    // インデックスの取得
                    int index = _rand.Next(0, count - 1);

                    // 記事を選択
                    this.WordpressContents.SelectedItem = this.WordpressContents.ElementAt(index);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion


        #region 待ち処理を終了する
        /// <summary>
        /// 待ち処理を終了する
        /// </summary>
        protected static bool IsBreakWait { get; set; } = false;
        #endregion


    }
}