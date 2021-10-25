using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.db
{
	/// <summary>
	/// 自分のフォローテーブル
	/// MyFollowUserテーブルをベースに作成しています
	/// 作成日：2021/10/19 作成者gohya
	/// </summary>
	[Table("MyFollowUser")]
	public class MyFollowUserBase : INotifyPropertyChanged
	{
		#region パラメータ
		#region ID[Id]プロパティ
		/// <summary>
		/// ID[Id]プロパティ用変数
		/// </summary>
		long _Id = 0;
		/// <summary>
		/// ID[Id]プロパティ
		/// </summary>
		[Key]
		[Column("Id")]
		public long Id
		{
			get
			{
				return _Id;
			}
			set
			{
				if (!_Id.Equals(value))
				{
					_Id = value;
					NotifyPropertyChanged("Id");
				}
			}
		}
		#endregion

		#region 追加日時[InserDateTime]プロパティ
		/// <summary>
		/// 追加日時[InserDateTime]プロパティ用変数
		/// </summary>
		DateTime _InserDateTime = DateTime.MinValue;
		/// <summary>
		/// 追加日時[InserDateTime]プロパティ
		/// </summary>
		[Column("InserDateTime")]
		public DateTime InserDateTime
		{
			get
			{
				return _InserDateTime;
			}
			set
			{
				if (!_InserDateTime.Equals(value))
				{
					_InserDateTime = value;
					NotifyPropertyChanged("InserDateTime");
				}
			}
		}
		#endregion

		#region 更新日時[UpdateDateTime]プロパティ
		/// <summary>
		/// 更新日時[UpdateDateTime]プロパティ用変数
		/// </summary>
		DateTime _UpdateDateTime = DateTime.MinValue;
		/// <summary>
		/// 更新日時[UpdateDateTime]プロパティ
		/// </summary>
		[Column("UpdateDateTime")]
		public DateTime UpdateDateTime
		{
			get
			{
				return _UpdateDateTime;
			}
			set
			{
				if (!_UpdateDateTime.Equals(value))
				{
					_UpdateDateTime = value;
					NotifyPropertyChanged("UpdateDateTime");
				}
			}
		}
		#endregion

		#region スクリーン名[ScreenName]プロパティ
		/// <summary>
		/// スクリーン名[ScreenName]プロパティ用変数
		/// </summary>
		String _ScreenName = string.Empty;
		/// <summary>
		/// スクリーン名[ScreenName]プロパティ
		/// </summary>
		[Column("ScreenName")]
		public String ScreenName
		{
			get
			{
				return _ScreenName;
			}
			set
			{
				if (!_ScreenName.Equals(value))
				{
					_ScreenName = value;
					NotifyPropertyChanged("ScreenName");
				}
			}
		}
		#endregion

		#region フォロワー数[FollowersCount]プロパティ
		/// <summary>
		/// フォロワー数[FollowersCount]プロパティ用変数
		/// </summary>
		long _FollowersCount = 0;
		/// <summary>
		/// フォロワー数[FollowersCount]プロパティ
		/// </summary>
		[Column("FollowersCount")]
		public long FollowersCount
		{
			get
			{
				return _FollowersCount;
			}
			set
			{
				if (!_FollowersCount.Equals(value))
				{
					_FollowersCount = value;
					NotifyPropertyChanged("FollowersCount");
				}
			}
		}
		#endregion

		#region フォロー数[FriendsCount]プロパティ
		/// <summary>
		/// フォロー数[FriendsCount]プロパティ用変数
		/// </summary>
		long _FriendsCount = 0;
		/// <summary>
		/// フォロー数[FriendsCount]プロパティ
		/// </summary>
		[Column("FriendsCount")]
		public long FriendsCount
		{
			get
			{
				return _FriendsCount;
			}
			set
			{
				if (!_FriendsCount.Equals(value))
				{
					_FriendsCount = value;
					NotifyPropertyChanged("FriendsCount");
				}
			}
		}
		#endregion

		#region 説明[Description]プロパティ
		/// <summary>
		/// 説明[Description]プロパティ用変数
		/// </summary>
		String _Description = string.Empty;
		/// <summary>
		/// 説明[Description]プロパティ
		/// </summary>
		[Column("Description")]
		public String Description
		{
			get
			{
				return _Description;
			}
			set
			{
				if (!_Description.Equals(value))
				{
					_Description = value;
					NotifyPropertyChanged("Description");
				}
			}
		}
		#endregion


		#endregion

		#region 関数
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MyFollowUserBase()
		{

		}
		#endregion

		#region コピーコンストラクタ
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="item">コピー内容</param>
		public MyFollowUserBase(MyFollowUserBase item)
		{
			// 要素のコピー
			Copy(item);
		}
		#endregion

		#region コピー
		/// <summary>
		/// コピー
		/// </summary>
		/// <param name="item">コピー内容</param>
		public void Copy(MyFollowUserBase item)
		{
			this.Id = item.Id;

			this.InserDateTime = item.InserDateTime;

			this.UpdateDateTime = item.UpdateDateTime;

			this.ScreenName = item.ScreenName;

			this.FollowersCount = item.FollowersCount;

			this.FriendsCount = item.FriendsCount;

			this.Description = item.Description;


		}
		#endregion

		#region Insert処理
		/// <summary>
		/// Insert処理
		/// </summary>
		/// <param name="item">Insertする要素</param>
		public static void Insert(MyFollowUserBase item)
		{
			using (var db = new SQLiteDataContext())
			{
				// Insert
				db.Add<MyFollowUserBase>(item);

				// コミット
				db.SaveChanges();
			}
		}
		#endregion

		#region Update処理
		/// <summary>
		/// Update処理
		/// </summary>
		/// <param name="pk_item">更新する主キー（主キーの値のみ入っていれば良い）</param>
		/// <param name="update_item">テーブル更新後の状態</param>
		public static void Update(MyFollowUserBase pk_item, MyFollowUserBase update_item)
		{
			using (var db = new SQLiteDataContext())
			{
				var item = db.DbSet_MyFollowUser.SingleOrDefault(x => x.Id.Equals(pk_item.Id));

				if (item != null)
				{
					item.Copy(update_item);
					db.SaveChanges();
				}
			}
		}
		#endregion

		#region Delete処理
		/// <summary>
		/// Delete処理
		/// </summary>
		/// <param name="pk_item">削除する主キー（主キーの値のみ入っていれば良い）</param>
		public static void Delete(MyFollowUserBase pk_item)
		{
			using (var db = new SQLiteDataContext())
			{
				var item = db.DbSet_MyFollowUser.SingleOrDefault(x => x.Id.Equals(pk_item.Id));
				if (item != null)
				{
					db.DbSet_MyFollowUser.Remove(item);
					db.SaveChanges();
				}
			}
		}
		#endregion

		#region Select処理
		/// <summary>
		/// Select処理
		/// </summary>
		/// <returns>全件取得</returns>
		public static List<MyFollowUserBase> Select()
		{
			using (var db = new SQLiteDataContext())
			{
				return db.DbSet_MyFollowUser.ToList<MyFollowUserBase>();
			}
		}
		#endregion
		#endregion

		#region INotifyPropertyChanged 
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion
	}
}
