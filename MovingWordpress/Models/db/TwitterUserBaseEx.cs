using MovingWordpress.Models.Tweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.db
{
    public class TwitterUserBaseEx : TwitterUserBase
    {
		#region Select処理
		/// <summary>
		/// Select処理
		/// </summary>
		/// <returns>全件取得</returns>
		public static List<TwitterUserBase> Select(long id)
		{
			using (var db = new SQLiteDataContext())
			{
				return db.DbSet_TwitterUser.Where(x => x.Id.Equals(id)).ToList<TwitterUserBase>();
			}
		}
		#endregion

		#region Delete処理
		/// <summary>
		/// Delete処理
		/// </summary>
		public static void Delete()
		{
			using (var db = new SQLiteDataContext())
			{
				var tmp = db.DbSet_TwitterUser.ToArray<TwitterUserBase>();
				db.DbSet_TwitterUser.RemoveRange(tmp);
				db.SaveChanges();
			}
		}
		#endregion
		
		#region Upsert処理
		/// <summary>
		/// Upsert処理
		/// </summary>
		/// <param name="user">ユーザーデータ</param>
		public static void Upsert(CoreTweet.User user, bool is_friend, bool is_follower)
		{
			// nullチェック
			if (user.Id == null)
				return;

			var item = TwitterUserBaseEx.Select(user.Id.Value);

			// データベースへ挿入
			if (item.Count > 0)
			{
				TwitterUserBase.Update(
					new TwitterUserBase()
					{
						Id = user.Id.Value,
						InserDateTime = item.ElementAt(0).InserDateTime,
						UpdateDateTime = DateTime.Now,
						ScreenName = user.ScreenName,
						Description = user.Description,
						FollowersCount = user.FollowersCount,
						FriendsCount = user.FriendsCount,
						IsFriend = is_friend,
						IsFollower = is_follower
					},
					new TwitterUserBase()
					{
						Id = user.Id.Value,
						InserDateTime = item.ElementAt(0).InserDateTime,
						UpdateDateTime = DateTime.Now,
						ScreenName = user.ScreenName,
						Description = user.Description,
						FollowersCount = user.FollowersCount,
						FriendsCount = user.FriendsCount,
						IsFriend = is_friend,
						IsFollower = is_follower
					}
					);
			}
			else
			{
				TwitterUserBase.Insert(
					new TwitterUserBase()
					{
						Id = user.Id.Value,
						InserDateTime = DateTime.Now,
						UpdateDateTime = DateTime.Now,
						ScreenName = user.ScreenName,
						Description = user.Description,
						FollowersCount = user.FollowersCount,
						FriendsCount = user.FriendsCount,
						IsFriend = is_friend,
						IsFollower = is_follower
					}
					);
			}
		}
		#endregion

		/// <summary>
		/// 指定範囲のデータを取得する
		/// </summary>
		/// <param name="from_ratio">ff比</param>
		/// <param name="to_ratio">ff比</param>
		/// <returns>リスト</returns>
		public static List<TwitterUserBase> SelectRangeData(double from_ratio, double to_ratio)
        {
			using (var db = new SQLiteDataContext())
			{
				return db.DbSet_TwitterUser.Where(x =>
					((x.FriendsCount / (double)x.FollowersCount) * 100.0 >= from_ratio)
					&& ((x.FriendsCount / (double)x.FollowersCount) * 100.0 <= to_ratio
					&& x.IsFollower.Equals(false) && x.IsFriend.Equals(false))
					).ToList<TwitterUserBase>();
			}

		}
	}
}
