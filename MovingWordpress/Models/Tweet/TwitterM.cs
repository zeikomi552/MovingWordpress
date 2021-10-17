using CoreTweet;
using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
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
        public UserResponse CreateFollow(CoreTweet.User user)
        {
            // トークンの作成
            CreateToken();

            // フォローの実行
            var result = this.Tokens.Friendships.Create(user.Id.Value, true);

            // リミットの取得(フォロー追加の場合はnullが返ってくるので設定しない)
            //this.RateLimit = result.RateLimit;

            return result;
        }
        #endregion

        #region フォロー数に対するフォロワー数割合を出す
        /// <summary>
        /// フォロー数に対するフォロワー数割合を出す
        /// </summary>
        /// <param name="user">ユーザー</param>
        /// <returns>割合</returns>
        public double FriendshipRatio(CoreTweet.User user)
        {
            // 0割回避
            if (user.FriendsCount > 0)
            {
                // 割合の算出
                return (double)user.FollowersCount / (double)user.FriendsCount;
            }
            else
            {
                return 0.0;
            }
        }
        #endregion

        #region 説明に期待する文字列が含まれているかのチェック
        /// <summary>
        /// 説明に期待する文字列が含まれているかのチェック
        /// </summary>
        /// <param name="nouns_list">単語リスト</param>
        /// <param name="user">チェックするユーザー</param>
        /// <returns>true:含まれる false:含まれない</returns>
        public bool CheckDescription(string[] nouns_list, CoreTweet.User user)
        {
            // 説明の取り出し
            string descrinption = user.Description;

            // 文字リストから要素を取り出し
            foreach (var nouns in nouns_list)
            {
                // 説明に期待する文字が含まれるかどうかのチェック
                if (descrinption.Contains(nouns))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region フォロー率の比率チェック
        /// <summary>
        /// フォロー率の比率チェック
        /// </summary>
        /// <param name="user">ユーザー</param>
        /// <param name="min_ratio">フォロー率の下限値</param>
        /// <param name="max_ratio">フォロー率の上限値</param>
        /// <returns>true:範囲内 false:範囲外</returns>
        public bool CheckFriendShipRatio(CoreTweet.User user, double min_ratio, double max_ratio)
        {
            // フォロー率
            var ratio = this.FriendshipRatio(user);

            // フォロー率チェック
            if (ratio >= (min_ratio / 100.0) && ratio <= (max_ratio / 100.0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region フォロワーリストの取得
        /// <summary>
        /// フォロワーリストの取得
        /// </summary>
        /// <param name="cursor">カーソル</param>
        /// <param name="screen_name">スクリーン名</param>
        /// <returns>フォロワーリスト</returns>
        public List<CoreTweet.User> GetAllFollower(string screen_name)
        {
            List<CoreTweet.User> list = new List<User>();

            // トークンの作成
            CreateToken();

            long next_cursor = -1;

            // 次のカーソルが無くなるまで繰り返す
            while (next_cursor != 0)
            {
                var result = GetFollower(next_cursor, screen_name);
                next_cursor = result.NextCursor;

                foreach (var tmp in result)
                {
                    // リストの追加
                    list.Add(tmp);
                }

                if(next_cursor != 0)
                {
                    // 1min待つ
                    Wait(60000);
                }
            }
            return list;
        }
        #endregion

        #region フォロワーリストの取得
        /// <summary>
        /// フォロワーリストの取得
        /// </summary>
        /// <param name="cursor">カーソル</param>
        /// <param name="screen_name">スクリーン名</param>
        /// <returns>フォロワーリスト</returns>
        public CoreTweet.Cursored<CoreTweet.User> GetFollower(long cursor, string screen_name)
        {
            // トークンの作成
            CreateToken();

            // 結果の取得
            var result = this.Tokens.Followers.List(screen_name, cursor, 100);

            // リミットの取得
            this.RateLimit = result.RateLimit;

            // ユーザー情報の取得
            return result;
        }
        #endregion

        #region フォロワーリストの取得
        /// <summary>
        /// フォロワーリストの取得
        /// </summary>
        /// <param name="cursor">カーソル</param>
        /// <param name="screen_name">スクリーン名</param>
        /// <returns>フォロワーリスト</returns>
        public List<CoreTweet.User> GetAllFollows(string screen_name)
        {
            List<CoreTweet.User> list = new List<User>();

            // トークンの作成
            CreateToken();

            long next_cursor = -1;

            // 次のカーソルが無くなるまで繰り返す
            while (next_cursor != 0)
            {
                var result = GetFollows(next_cursor, screen_name);
                next_cursor = result.NextCursor;

                foreach (var tmp in result)
                {
                    // リストの追加
                    list.Add(tmp);
                }

                // 1min待つ
                Wait(60000);
            }
            return list;
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
    }
}
