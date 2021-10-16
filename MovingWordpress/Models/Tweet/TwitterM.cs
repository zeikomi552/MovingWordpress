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


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TwitterM()
        {
        }


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
                this.Tokens.Statuses.Update(status => message);

                // 成功
                return true;
            }
            catch
            {
                // 失敗
                return false;
            }
        }

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

            // 検索処理
            return this.Tokens.Search.Tweets(count => 100, q => keyword, lang => "ja");
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

            // ユーザー情報の取得
            return this.Tokens.Followers.List(screen_name, cursor, 100);
        }
        #endregion

        #region 待ち処理
        /// <summary>
        /// 待ち処理
        /// </summary>
        /// <param name="limit">リミット情報</param>
        /// <param name="wait_ms">待ち時間(デフォルト60秒)</param>
        /// <returns>true:待ち時間のリセット false:待ち時間中</returns>
        public bool Wait(CoreTweet.RateLimit limit, int wait_ms = 60000)
        {
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

    }
}
