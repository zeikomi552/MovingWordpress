using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
	public class DateRangeM : ModelBase
	{
		#region 開始日[FromDateBase]プロパティ
		/// <summary>
		/// 開始日[FromDateBase]プロパティ用変数
		/// </summary>
		DateTime? _FromDateBase = null;
		/// <summary>
		/// 開始日[FromDateBase]プロパティ
		/// </summary>
		public DateTime? FromDateBase
		{
			get
			{
				return _FromDateBase;
			}
			set
			{
				if (_FromDateBase == null || !_FromDateBase.Equals(value))
				{
					_FromDateBase = value;
					NotifyPropertyChanged("FromDateBase");
				}
			}
		}
		#endregion

		#region 終了日[ToDateBase]プロパティ
		/// <summary>
		/// 終了日[ToDateBase]プロパティ用変数
		/// </summary>
		DateTime? _ToDateBase = null;
		/// <summary>
		/// 終了日[ToDateBase]プロパティ
		/// </summary>
		public DateTime? ToDateBase
		{
			get
			{
				return _ToDateBase;
			}
			set
			{
				if (_ToDateBase == null || !_ToDateBase.Equals(value))
				{
					_ToDateBase = value;
					NotifyPropertyChanged("ToDateBase");
				}
			}
		}
		#endregion

		public DateTime FromDate
		{
			get
			{
				return this.FromDateBase.HasValue ? this.FromDateBase.Value : new DateTime(1970, 1, 1);
			}
		}

		public DateTime ToDate
        {
			get
			{
				return this.ToDateBase.HasValue ? this.ToDateBase.Value : new DateTime(2970, 1, 1);

			}

		}

        #region 開始値が存在するかどうかのチェック
        /// <summary>
        /// 値が存在するかどうかのチェック
        /// </summary>
        public bool HasValue
		{
			get
			{
				return this.FromDateBase.HasValue || this.ToDateBase.HasValue;
			}
		}
		#endregion


	}
}
