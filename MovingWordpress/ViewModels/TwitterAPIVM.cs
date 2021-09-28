using Microsoft.Win32;
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

namespace MovingWordpress.ViewModels
{
    public class TwitterAPIVM : ViewModelBase
    {
        #region ツイッターAPI用のコンフィグ[TwitterConfig]プロパティ
        /// <summary>
        /// ツイッターAPI用のコンフィグ[TwitterConfig]プロパティ用変数
        /// </summary>
        TwitterConfigM _TwitterConfig = new TwitterConfigM();
        /// <summary>
        /// ツイッターAPI用のコンフィグ[TwitterConfig]プロパティ
        /// </summary>
        public TwitterConfigM TwitterConfig
        {
            get
            {
                return _TwitterConfig;
            }
            set
            {
                if (_TwitterConfig == null || !_TwitterConfig.Equals(value))
                {
                    _TwitterConfig = value;
                    NotifyPropertyChanged("TwitterConfig");
                }
            }
        }
        #endregion



        #region ツイート内容[TweetContent]プロパティ
        /// <summary>
        /// ツイート内容[TweetContent]プロパティ用変数
        /// </summary>
        TwitterContentM _TweetContent = new TwitterContentM();
        /// <summary>
        /// ツイート内容[TweetContent]プロパティ
        /// </summary>
        public TwitterContentM TweetContent
        {
            get
            {
                return _TweetContent;
            }
            set
            {
                if (_TweetContent == null || !_TweetContent.Equals(value))
                {
                    _TweetContent = value;
                    NotifyPropertyChanged("TweetContent");
                }
            }
        }
        #endregion

        #region メッセージ[Message]プロパティ
        /// <summary>
        /// メッセージ[Message]プロパティ用変数
        /// </summary>
        string _Message = string.Empty;
        /// <summary>
        /// メッセージ[Message]プロパティ
        /// </summary>
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (_Message == null || !_Message.Equals(value))
                {
                    _Message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }
        #endregion

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

        #region TwitterAPI用オブジェクト
        /// <summary>
        /// TwitterAPI用オブジェクト
        /// </summary>
        public TwitterM TwitterAPI = new TwitterM();

        public void Init()
        {
            // コンフィグファイルのロード
            this.TwitterConfig.Load();

            // トークンの作成
            this.TwitterAPI.CreateToken(this.TwitterConfig.KeysM.ConsumerKey,
                this.TwitterConfig.KeysM.ConsumerSecretKey, this.TwitterConfig.KeysM.AccessToken, this.TwitterConfig.KeysM.AccessSecret);
        }
        #endregion

        #region ツイート
        /// <summary>
        /// ツイート
        /// </summary>
        /// <param name="message"></param>
        public void Tweet()
        {
            try
            {
                // メッセージの送信処理
                this.TwitterAPI.Tweet(this.Message);
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region ファイルを開く処理
        /// <summary>
        /// ファイルを開く処理
        /// </summary>
        public void OpenFile()
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
                    string ext = Path.GetExtension(dialog.FileName).ToLower();

                    if (ext.Equals(".gz"))
                    {
                        // データベースのバックアップファイルのロード
                        var tmp = FileAnalyzerM.LoadContents(dialog.FileName);

                        // コンテンツのセット
                        this.WordpressContents.Items
                            = new ObservableCollection<WpContentsM>(tmp);
                    }
                    else
                    {
                        TagCateM tag = new TagCateM();
                        var tmp = tag.Load(dialog.FileName);
                        this.WordpressContents.Items = tmp.BlogContents.Items;   // 個別記事の情報を取得
                    }
                }



            }
            catch (Exception e)
            {
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
                    string title = this.WordpressContents.SelectedItem.Post_title;
                    string url = "https://www.premium-tsubu-hero.net/" + this.WordpressContents.SelectedItem.Post_name;

                    this.Message = this.TweetContent.CreateTweetMessage(url, title);
                }
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        Random _rand = new Random();

        /// <summary>
        /// ランダム選択
        /// </summary>
        public void RandomSelect()
        {
            try
            {
                if (this.WordpressContents.Items != null && this.WordpressContents.Items.Count > 0)
                {
                    // カウントの取得
                    int count = this.WordpressContents.Items.Count;

                    // インデックスの取得
                    int index = _rand.Next(0, count - 1);

                    this.WordpressContents.SelectedItem = this.WordpressContents.ElementAt(index);
                }
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
    }
}