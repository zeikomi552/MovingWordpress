using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.Tweet
{
    public class FollowManageM : ModelBase
	{
		#region 上限値[Max]プロパティ
		/// <summary>
		/// 上限値[Max]プロパティ用変数
		/// </summary>
		int _Max = 400;
		/// <summary>
		/// 上限値[Max]プロパティ
		/// </summary>
		public int Max
		{
			get
			{
				return _Max;
			}
			set
			{
				if (!_Max.Equals(value))
				{
					_Max = value;
					NotifyPropertyChanged("Max");
					NotifyPropertyChanged("Remain");
				}
			}
		}
		#endregion

		#region 現在値[Current]プロパティ
		/// <summary>
		/// 現在値[Current]プロパティ用変数
		/// </summary>
		int _Current = 0;
		/// <summary>
		/// 現在値[Current]プロパティ
		/// </summary>
		public int Current
		{
			get
			{
				return _Current;
			}
			set
			{
				if (!_Current.Equals(value))
				{
					_Current = value;
					NotifyPropertyChanged("Current");
					NotifyPropertyChanged("Remain");
				}
			}
		}
		#endregion

		#region 残り回数
		/// <summary>
		/// 残り回数
		/// </summary>
		public int Remain
        {
            get
            {
				return this.Max - this.Current;
            }
		}
		#endregion

		#region 最終フォロー時刻[LastFollowTime]プロパティ
		/// <summary>
		/// 最終フォロー時刻[LastFollowTime]プロパティ用変数
		/// </summary>
		DateTime? _LastFollowTime = null;
		/// <summary>
		/// 最終フォロー時刻[LastFollowTime]プロパティ
		/// </summary>
		public DateTime? LastFollowTime
		{
			get
			{
				return _LastFollowTime;
			}
			set
			{
				if (_LastFollowTime == null || !_LastFollowTime.Equals(value))
				{
					_LastFollowTime = value;
					NotifyPropertyChanged("LastFollowTime");
				}
			}
		}
		#endregion

		#region 待ち時間(分)の範囲(From)[FromWait]プロパティ
		/// <summary>
		/// 待ち時間(分)の範囲(From)[FromWait]プロパティ用変数
		/// </summary>
		int _FromWait = 30;
		/// <summary>
		/// 待ち時間(分)の範囲(From)[FromWait]プロパティ
		/// </summary>
		public int FromWait
		{
			get
			{
				return _FromWait;
			}
			set
			{
				if (!_FromWait.Equals(value))
				{
					_FromWait = value;
					NotifyPropertyChanged("FromWait");
				}
			}
		}
		#endregion

		#region 待ち時間(分)の範囲(to)[ToWait]プロパティ
		/// <summary>
		/// 待ち時間(分)の範囲(to)[ToWait]プロパティ用変数
		/// </summary>
		int _ToWait = 60;
		/// <summary>
		/// 待ち時間(分)の範囲(to)[ToWait]プロパティ
		/// </summary>
		public int ToWait
		{
			get
			{
				return _ToWait;
			}
			set
			{
				if (!_ToWait.Equals(value))
				{
					_ToWait = value;
					NotifyPropertyChanged("ToWait");
				}
			}
		}
		#endregion

		#region フォロー数の範囲[FollowRange]プロパティ
		/// <summary>
		/// フォロー数の範囲[FollowRange]プロパティ用変数
		/// </summary>
		IntRangeM _FollowRange = new IntRangeM()
			{
				MinValue = 0,
				MaxValue = 30000
			};

		/// <summary>
		/// フォロー数の範囲[FollowRange]プロパティ
		/// </summary>
		public IntRangeM FollowRange
		{
			get
			{
				return _FollowRange;
			}
			set
			{
				if (_FollowRange == null || !_FollowRange.Equals(value))
				{
					_FollowRange = value;
					NotifyPropertyChanged("FollowRange");
				}
			}
		}
		#endregion

		#region 経過日数[ElapsedDate]プロパティ
		/// <summary>
		/// 経過日数[ElapsedDate]プロパティ用変数
		/// </summary>
		int _ElapsedDate = 32;
		/// <summary>
		/// 経過日数[ElapsedDate]プロパティ
		/// </summary>
		public int ElapsedDate
		{
			get
			{
				return _ElapsedDate;
			}
			set
			{
				if (!_ElapsedDate.Equals(value))
				{
					_ElapsedDate = value;
					NotifyPropertyChanged("ElapsedDate");
				}
			}
		}
		#endregion



		/// <summary>
		/// 待ち処理
		/// </summary>
		/// <param name="waitsecond">待ち時間</param>
		/// <returns>true:まだ待つ false:待たなくて良い</returns>
		public bool CheckWait(int waitsecond)
		{

			if (this.LastFollowTime.HasValue)
			{
				// 最終更新日時が今日で残り回数が0ならば
				if (this.LastFollowTime.Value.Date.Equals(DateTime.Today) && this.Remain <= 0)
				{
					return true;
				}

				// 最終フォロー時刻が待ち時間を満たすならば
				if (this.LastFollowTime.Value < DateTime.Now.AddSeconds(waitsecond))
				{
					return true;
				}
			}

			return false;
		}

	}
}
