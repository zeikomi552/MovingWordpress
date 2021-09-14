using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class StartEndTimeM : ModelBase
    {
		#region [StartTime]プロパティ
		/// <summary>
		/// [StartTime]プロパティ用変数
		/// </summary>
		DateTime? _StartTime = null;
		/// <summary>
		/// [StartTime]プロパティ
		/// </summary>
		public DateTime? StartTime
		{
			get
			{
				return _StartTime;
			}
			set
			{
				if (_StartTime == null || !_StartTime.Equals(value))
				{
					_StartTime = value;
					NotifyPropertyChanged("StartTime");
				}
			}
		}
		#endregion

		#region [EndTime]プロパティ
		/// <summary>
		/// [EndTime]プロパティ用変数
		/// </summary>
		DateTime? _EndTime = null;
		/// <summary>
		/// [EndTime]プロパティ
		/// </summary>
		public DateTime? EndTime
		{
			get
			{
				return _EndTime;
			}
			set
			{
				if (_EndTime == null || !_EndTime.Equals(value))
				{
					_EndTime = value;
					NotifyPropertyChanged("EndTime");
				}
			}
		}
		#endregion
	}
}
