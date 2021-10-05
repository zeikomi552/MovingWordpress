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
        public void CreateToken(string consumer_key, string consumer_secret, string access_token, string access_secret)
        {
            try
            {
                this.Tokens = CoreTweet.Tokens.Create(consumer_key,
                        consumer_secret,
                        access_token,
                        access_secret);

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
            if (this.IsSuccessToken)
            {
                this.Tokens.Statuses.Update(status => message);
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}
