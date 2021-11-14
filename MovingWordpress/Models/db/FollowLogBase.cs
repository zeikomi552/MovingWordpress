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
	/// アクションログ
	/// FollowLogテーブルをベースに作成しています
	/// 作成日：2021/11/14 作成者gohya
	/// </summary>
	[Table("FollowLog")]
	public class FollowLogBase : INotifyPropertyChanged
	{
		#region パラメータ
		#region 登録日時[RegTime]プロパティ
		/// <summary>
		/// 登録日時[RegTime]プロパティ用変数
		/// </summary>
		DateTime _RegTime = DateTime.MinValue;
		/// <summary>
		/// 登録日時[RegTime]プロパティ
		/// </summary>
		[Key]
		[Column("RegTime")]
		public DateTime RegTime
		{
			get
			{
				return _RegTime;
			}
			set
			{
				if (!_RegTime.Equals(value))
				{
					_RegTime = value;
					NotifyPropertyChanged("RegTime");
				}
			}
		}
		#endregion

		#region 対象のユーザーID[Id]プロパティ
		/// <summary>
		/// 対象のユーザーID[Id]プロパティ用変数
		/// </summary>
		long _Id = 0;
		/// <summary>
		/// 対象のユーザーID[Id]プロパティ
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

		#region 0:フォロー 1:フォロー解除[Action]プロパティ
		/// <summary>
		/// 0:フォロー 1:フォロー解除[Action]プロパティ用変数
		/// </summary>
		Int32 _Action = 0;
		/// <summary>
		/// 0:フォロー 1:フォロー解除[Action]プロパティ
		/// </summary>
		[Column("Action")]
		public Int32 Action
		{
			get
			{
				return _Action;
			}
			set
			{
				if (!_Action.Equals(value))
				{
					_Action = value;
					NotifyPropertyChanged("Action");
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
		public FollowLogBase()
		{

		}
		#endregion

		#region コピーコンストラクタ
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="item">コピー内容</param>
		public FollowLogBase(FollowLogBase item)
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
		public void Copy(FollowLogBase item)
		{
			this.RegTime = item.RegTime;

			this.Id = item.Id;

			this.Action = item.Action;


		}
		#endregion

		#region Insert処理
		/// <summary>
		/// Insert処理
		/// </summary>
		/// <param name="item">Insertする要素</param>
		public static void Insert(FollowLogBase item)
		{
			using (var db = new SQLiteDataContext())
			{
				// Insert
				db.Add<FollowLogBase>(item);

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
		public static void Update(FollowLogBase pk_item, FollowLogBase update_item)
		{
			using (var db = new SQLiteDataContext())
			{
				var item = db.DbSet_FollowLog.SingleOrDefault(x => x.RegTime.Equals(pk_item.RegTime) && x.Id.Equals(pk_item.Id));

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
		public static void Delete(FollowLogBase pk_item)
		{
			using (var db = new SQLiteDataContext())
			{
				var item = db.DbSet_FollowLog.SingleOrDefault(x => x.RegTime.Equals(pk_item.RegTime) && x.Id.Equals(pk_item.Id));
				if (item != null)
				{
					db.DbSet_FollowLog.Remove(item);
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
		public static List<FollowLogBase> Select()
		{
			using (var db = new SQLiteDataContext())
			{
				return db.DbSet_FollowLog.ToList<FollowLogBase>();
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
