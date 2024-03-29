﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.db
{
    public class MyFollowerUserBaseEx : MyFollowerUserBase
	{
		#region Select処理
		/// <summary>
		/// Select処理
		/// </summary>
		/// <returns>全件取得</returns>
		public static List<MyFollowerUserBase> Select(long id)
		{
			using (var db = new SQLiteDataContext())
			{
				return db.DbSet_MyFollowerUser.Where(x => x.Id.Equals(id)).ToList<MyFollowerUserBase>();
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
				var tmp = db.DbSet_MyFollowerUser.ToArray<MyFollowerUserBase>();
				db.DbSet_MyFollowerUser.RemoveRange(tmp);
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

			var item = MyFollowerUserBaseEx.Select(user.Id.Value);

			// データベースへ挿入
			if (item.Count > 0)
			{
				MyFollowerUserBaseEx.Update(
					new MyFollowerUserBaseEx()
					{
						Id = user.Id.Value,
						InserDateTime = item.ElementAt(0).InserDateTime,
						UpdateDateTime = DateTime.Now,
						ScreenName = user.ScreenName,
						Description = user.Description,
						FollowersCount = user.FollowersCount,
						FriendsCount = user.FriendsCount
					},
					new MyFollowerUserBaseEx()
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
				MyFollowerUserBaseEx.Insert(
					new MyFollowerUserBaseEx()
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
