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
		public static void Upsert(CoreTweet.User user)
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
						FriendsCount = user.FriendsCount
					},
					new TwitterUserBase()
					{
						Id = user.Id.Value,
						InserDateTime = item.ElementAt(0).InserDateTime,
						UpdateDateTime = DateTime.Now,
						ScreenName = user.ScreenName,
						Description = user.Description,
						FollowersCount = user.FollowersCount,
						FriendsCount = user.FriendsCount
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
						FriendsCount = user.FriendsCount
					}
					);
			}
		}
		#endregion
	}
}
