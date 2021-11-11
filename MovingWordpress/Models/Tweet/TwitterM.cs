using CoreTweet;
using MovingWordpress.Models.db;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.Tweet
{
    public class TwitterM : ModelBase
    {
        /// <summary>
        /// トークン
        /// </summary>
        public Tokens Tokens { get; set; }

        /// <summary>
        /// トークンの作成成功 or 失敗
        /// </summary>
        public bool IsSuccessToken { get; set; } = false;

        #region 自分のフォロワーリスト[MyFollowerList]プロパティ
        /// <summary>
        /// 自分のフォロワーリスト[MyFollowerList]プロパティ用変数
        /// </summary>
        ModelList<TwitterUserM> _MyFollowerList = new ModelList<TwitterUserM>();
        /// <summary>
        /// 自分のフォロワーリスト[MyFollowerList]プロパティ
        /// </summary>
        public ModelList<TwitterUserM> MyFollowerList
        {
            get
            {
                return _MyFollowerList;
            }
            set
            {
                if (_MyFollowerList == null || !_MyFollowerList.Equals(value))
                {
                    _MyFollowerList = value;
                    NotifyPropertyChanged("MyFollowerList");
                }
            }
        }
        #endregion
        #region スクリーン名[MyScreenName]プロパティ
        /// <summary>
        /// スクリーン名[MyScreenName]プロパティ用変数
        /// </summary>
        string _MyScreenName = string.Empty;
        /// <summary>
        /// スクリーン名[MyScreenName]プロパティ
        /// </summary>
        public string MyScreenName
        {
            get
            {
                return _MyScreenName;
            }
            set
            {
                if (_MyScreenName == null || !_MyScreenName.Equals(value))
                {
                    _MyScreenName = value;
                    NotifyPropertyChanged("MyScreenName");
                }
            }
        }
        #endregion

        #region 自分のフォローリスト[MyFollowList]プロパティ
        /// <summary>
        /// 自分のフォローリスト[MyFollowList]プロパティ用変数
        /// </summary>
        ModelList<TwitterUserM> _MyFollowList = new ModelList<TwitterUserM>();
        /// <summary>
        /// 自分のフォローリスト[MyFollowList]プロパティ
        /// </summary>
        public ModelList<TwitterUserM> MyFollowList
        {
            get
            {
                return _MyFollowList;
            }
            set
            {
                if (_MyFollowList == null || !_MyFollowList.Equals(value))
                {
                    _MyFollowList = value;
                    NotifyPropertyChanged("MyFollowList");
                }
            }
        }
        #endregion

        #region フォロー候補[FollowList]プロパティ
        /// <summary>
        /// フォロー候補[FollowList]プロパティ用変数
        /// </summary>
        ModelList<TwitterUserM> _FollowList = new ModelList<TwitterUserM>();
        /// <summary>
        /// フォロー候補[FollowList]プロパティ
        /// </summary>
        public ModelList<TwitterUserM> FollowList
        {
            get
            {
                return _FollowList;
            }
            set
            {
                if (_FollowList == null || !_FollowList.Equals(value))
                {
                    _FollowList = value;
                    NotifyPropertyChanged("FollowList");
                }
            }
        }
        #endregion


        #region ツイッターAPI用のコンフィグ[Config]プロパティ
        /// <summary>
        /// ツイッターAPI用のコンフィグ[Config]プロパティ用変数
        /// </summary>
        TwitterConfigM _Config = new TwitterConfigM();
        /// <summary>
        /// ツイッターAPI用のコンフィグ[Config]プロパティ
        /// </summary>
        public TwitterConfigM Config
        {
            get
            {
                return _Config;
            }
            set
            {
                if (_Config == null || !_Config.Equals(value))
                {
                    _Config = value;
                    NotifyPropertyChanged("Config");
                }
            }
        }
        #endregion

        #region API使用制限[RateLimit]プロパティ
        /// <summary>
        /// API使用制限[RateLimit]プロパティ用変数
        /// </summary>
        CoreTweet.RateLimit _RateLimit = new CoreTweet.RateLimit();
        /// <summary>
        /// [RateLimit]プロパティ
        /// </summary>
        public CoreTweet.RateLimit RateLimit
        {
            get
            {
                return _RateLimit;
            }
            set
            {
                if (_RateLimit == null || !_RateLimit.Equals(value))
                {
                    _RateLimit = value;
                    NotifyPropertyChanged("RateLimit");
                }
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TwitterM()
        {
        }
        #endregion

        #region トークンの作成処理
        /// <summary>
        /// トークンの作成処理
        /// </summary>
        /// <returns></returns>
        public void CreateToken()
        {
            try
            {
                // トークンの作成
                this.Tokens = CoreTweet.Tokens.Create(Config.KeysM.ConsumerKey,
                        Config.KeysM.ConsumerSecretKey,
                        Config.KeysM.AccessToken,
                        Config.KeysM.AccessSecret);

                // トークンの作成成功
                this.IsSuccessToken = true;
            }
            catch
            {
                // トークンの作成失敗
                this.IsSuccessToken = false;
                throw;
            }
        }
        #endregion

        #region ツイート処理
        /// <summary>
        /// ツイート処理
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        /// <returns>true:ツイート成功 false:トークンが作成されていない それ以外はエクセプション</returns>
        public bool Tweet(string message)
        {
            try
            {
                // トークンの作成
                CreateToken();

                // ツイート
                var result = this.Tokens.Statuses.Update(status => message);

                // 限界値の取得
                this.RateLimit = result.RateLimit;

                // 成功
                return true;
            }
            catch
            {
                // 失敗
                return false;
            }
        }
        #endregion

        #region Tweetの検索処理
        /// <summary>
        /// Tweetの検索処理
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public CoreTweet.SearchResult TweetSearch(string keyword)
        {
            // トークンの作成
            CreateToken();

            // 検索
            var result = this.Tokens.Search.Tweets(count => 100, q => keyword, lang => "ja"); ;

            // リミットの保持
            this.RateLimit = result.RateLimit;
            return result;
        }
        #endregion

        #region 待ち処理
        /// <summary>
        /// 待ち処理
        /// </summary>
        /// <param name="limit">リミット情報</param>
        /// <param name="wait_ms">待ち時間(デフォルト60秒)</param>
        /// <returns>true:待ち時間のリセット false:待ち時間中</returns>
        public bool Wait(int wait_ms = 60000)
        {
            var limit = this.RateLimit;

            // 残り回数が0の場合は待つ
            if (limit.Remaining <= 0)
            {
                // 現在時刻がリセット時間を超えない限り待つ
                if (limit.Reset.CompareTo(DateTime.Now) > 0)
                {
                    System.Threading.Thread.Sleep(wait_ms);   // 待機
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region フォローを実行する
        /// <summary>
        /// フォローを実行する
        /// </summary>
        /// <param name="user">ユーザー情報</param>
        /// <returns>ステータス</returns>
        public UserResponse CreateFollow(long id)
        {
            // トークンの作成
            CreateToken();

            // フォローの実行
            var result = this.Tokens.Friendships.Create(id, true);

            // リミットの取得(フォロー追加の場合はnullが返ってくるので設定しない)
            //this.RateLimit = result.RateLimit;

            return result;
        }
        #endregion

        #region ユーザーリストの取得(全件)
        /// <summary>
        /// ユーザーリストの取得(全件)
        /// </summary>
        /// <param name="cursor">カーソル</param>
        /// <param name="screen_name">スクリーン名</param>
        /// <param name="get_type">取得するユーザーのタイプ　true:フォロー false:フォロワー</param>
        /// <returns>ユーザーリスト</returns>
        public List<CoreTweet.User> GetUserAll(string screen_name, bool get_type)
        {
            List<CoreTweet.User> list = new List<User>();

            // トークンの作成
            CreateToken();

            long next_cursor = -1;

            // 次のカーソルが無くなるまで繰り返す
            while (next_cursor != 0)
            {
                var result = GetUser(next_cursor, screen_name, get_type);
                next_cursor = result.NextCursor;

                foreach (var tmp in result)
                {
                    // リストの追加
                    list.Add(tmp);
                }

                if (next_cursor != 0)
                {
                    // 1min待つ
                    Wait(60000);
                }
            }
            return list;
        }
        #endregion

        #region RateLimit取得関数
        /// <summary>
        /// RateLimit取得関数
        /// </summary>
        /// <returns>RateLimit</returns>
        public CoreTweet.Core.DictionaryResponse<string, Dictionary<string, CoreTweet.RateLimit>> GetRateLimit()
        {
            // トークンの作成
            CreateToken();

            return this.Tokens.Application.RateLimitStatus();
        }
        #endregion

        #region フォロワーリストの取得
        /// <summary>
        /// フォロワーリストの取得
        /// </summary>
        /// <param name="cursor">カーソル</param>
        /// <param name="screen_name">スクリーン名</param>
        /// <param name="get_type">取得するユーザーのタイプ　true:フォロー false:フォロワー</param>
        /// <returns>フォロワーリスト</returns>
        public CoreTweet.Cursored<CoreTweet.User> GetUser(long cursor, string screen_name, bool get_type)
        {

            // トークンの作成
            CreateToken();

            // 結果の取得
            var result = get_type ? this.Tokens.Friends.List(screen_name, cursor, 100) : this.Tokens.Followers.List(screen_name, cursor, 100);

            // リミットの取得
            this.RateLimit = result.RateLimit;

            // ユーザー情報の取得
            return result;
        }
        #endregion

        #region フォローリストの取得
        /// <summary>
        /// フォローリストの取得
        /// </summary>
        /// <param name="cursor">カーソル</param>
        /// <param name="screen_name">スクリーン名</param>
        /// <returns>フォローリスト</returns>
        public CoreTweet.Cursored<CoreTweet.User> GetFollows(long cursor, string screen_name)
        {
            // トークンの作成
            CreateToken();

            // 結果の取得
            var result = this.Tokens.Friends.List(screen_name, cursor, 100);

            // リミットの取得
            this.RateLimit = result.RateLimit;

            // ユーザー情報の取得
            return result;
        }
        #endregion

        #region 自分のフォローを更新
        /// <summary>
        /// 自分のフォローを更新
        /// </summary>
        public void RefreshMyFollow()
        {
            // 自分のフォロワー情報を削除
            MyFollowUserBaseEx.Delete();

            // データベースからフォローを取得
            var follow_user = MyFollowUserBaseEx.Select();

            // Twitterからフォローの取得
            var tw_follow_user = this.GetUserAll(this.MyScreenName, true);

            var tmp_list = new ModelList<TwitterUserM>();

            foreach (var user in tw_follow_user)
            {
                tmp_list.Items.Add(new TwitterUserM(user, true, false));
            }

            // フォローリストの取得
            this.MyFollowList = tmp_list;

            // フォローユーザーをデータベースに保存する
            foreach (var user in tmp_list.Items)
            {
                MyFollowUserBaseEx.Upsert(user);
            }
        }
        #endregion
    }
}
