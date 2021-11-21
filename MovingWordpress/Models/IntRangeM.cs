using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class IntRangeM : ModelBase
	{
		#region 最小値[MinValue]プロパティ
		/// <summary>
		/// 最小値[MinValue]プロパティ用変数
		/// </summary>
		int _MinValue = 0;
		/// <summary>
		/// 最小値[MinValue]プロパティ
		/// </summary>
		public int MinValue
		{
			get
			{
				return _MinValue;
			}
			set
			{
				if (!_MinValue.Equals(value))
				{
					_MinValue = value;
					NotifyPropertyChanged("MinValue");
				}
			}
		}
		#endregion

		#region 最大値[MaxValue]プロパティ
		/// <summary>
		/// 最大値[MaxValue]プロパティ用変数
		/// </summary>
		int _MaxValue = 0;
		/// <summary>
		/// 最大値[MaxValue]プロパティ
		/// </summary>
		public int MaxValue
		{
			get
			{
				return _MaxValue;
			}
			set
			{
				if (!_MaxValue.Equals(value))
				{
					_MaxValue = value;
					NotifyPropertyChanged("MaxValue");
				}
			}
		}
		#endregion
	}
}
