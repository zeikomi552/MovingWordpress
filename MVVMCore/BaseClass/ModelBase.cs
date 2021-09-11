using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMCore.BaseClass
{
    public class ModelBase : INotifyPropertyChanged
    {
		#region シャローコピー
		/// <summary>
		/// シャローコピー
		/// </summary>
		/// <returns></returns>
		public T ShallowCopy<T>()
		{
			return (T)MemberwiseClone();
		}
		#endregion

		#region INotifyPropertyChanged 
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion
	}
}
