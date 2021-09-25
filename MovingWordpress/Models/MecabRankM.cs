using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class MecabRankM : ModelBase
    {

		#region 表示[Surface]プロパティ
		/// <summary>
		/// 表示[Surface]プロパティ用変数
		/// </summary>
		string _Surface = string.Empty;
		/// <summary>
		/// 表示[Surface]プロパティ
		/// </summary>
		public string Surface
		{
			get
			{
				return _Surface;
			}
			set
			{
				if (_Surface == null || !_Surface.Equals(value))
				{
					_Surface = value;
					NotifyPropertyChanged("Surface");
				}
			}
		}
		#endregion

		#region 品詞情報[Feature]プロパティ
		/// <summary>
		/// 品詞情報[Feature]プロパティ用変数
		/// </summary>
		string _Feature = string.Empty;
		/// <summary>
		/// 品詞情報[Feature]プロパティ
		/// </summary>
		public string Feature
		{
			get
			{
				return _Feature;
			}
			set
			{
				if (_Feature == null || !_Feature.Equals(value))
				{
					_Feature = value;
					NotifyPropertyChanged("Feature");
					NotifyPropertyChanged("PartsOfSpeech");
				}
			}
		}
		#endregion

		/// <summary>
		/// 品詞
		/// </summary>
		public string PartsOfSpeech
        {
			get
			{
				string[] tmp = this.Feature.Split(',');

				return tmp.ElementAt(0);
			}
		}

		#region 出現回数[Count]プロパティ
		/// <summary>
		/// 出現回数[Count]プロパティ用変数
		/// </summary>
		int _Count = 0;
		/// <summary>
		/// 出現回数[Count]プロパティ
		/// </summary>
		public int Count
		{
			get
			{
				return _Count;
			}
			set
			{
				if (!_Count.Equals(value))
				{
					_Count = value;
					NotifyPropertyChanged("Count");
				}
			}
		}
		#endregion


	}
}
