using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
	public class OutputTypeElementM : ModelBase
	{
		public enum OutputTypeEnum
		{ 
			/// <summary>
			/// 名前順
			/// </summary>
			NameAsc,
			/// <summary>
			/// 名前逆順
			/// </summary>
			NameDesc,
			/// <summary>
			/// 日付順
			/// </summary>
			DateAsc,
			/// <summary>
			/// 日付逆順
			/// </summary>
			DateDesc
		}

		#region 出力タイプ[OutputType]プロパティ
		/// <summary>
		/// 出力タイプ[OutputType]プロパティ用変数
		/// </summary>
		OutputTypeEnum _OutputType = OutputTypeEnum.DateAsc;
		/// <summary>
		/// 出力タイプ[OutputType]プロパティ
		/// </summary>
		public OutputTypeEnum OutputType
		{
			get
			{
				return _OutputType;
			}
			set
			{
				if (!_OutputType.Equals(value))
				{
					_OutputType = value;
					NotifyPropertyChanged("OutputType");
				}
			}
		}
		#endregion

		#region 表示文字列[DisplayType]プロパティ
		/// <summary>
		/// 表示文字列[DisplayType]プロパティ用変数
		/// </summary>
		string _DisplayType = string.Empty;
		/// <summary>
		/// 表示文字列[DisplayType]プロパティ
		/// </summary>
		public string DisplayType
		{
			get
			{
				return _DisplayType;
			}
			set
			{
				if (_DisplayType == null || !_DisplayType.Equals(value))
				{
					_DisplayType = value;
					NotifyPropertyChanged("DisplayType");
				}
			}
		}
		#endregion
	}
}
