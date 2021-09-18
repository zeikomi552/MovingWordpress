using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MovingWordpress.Common.Converters
{
	[System.Windows.Data.ValueConversion(typeof(bool), typeof(Visibility))]
	public class InverseBoolToVisibilityConverter : System.Windows.Data.IValueConverter
	{

		#region IValueConverter メンバ
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var target = (bool)value;

			if (target)
				return Visibility.Collapsed;
			else
				return Visibility.Visible;
		}

		// TwoWayの場合に使用する
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
