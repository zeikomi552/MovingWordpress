using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MovingWordpress.Common.Actions
{
	#region DataGridのスクロールバーを追従させるアクション 
	/// <summary> 
	/// DataGridのスクロールバーを追従させるアクション 
	/// </summary> 
	public class DataGridTopRowAction : TriggerAction<DataGrid>
	{

		public static readonly DependencyProperty DataGridTopRowProperty =
		DependencyProperty.Register("DataGridTopRow", typeof(int), typeof(DataGridTopRowAction), new UIPropertyMetadata());

		public int DataGridTopRow
		{
			get { return (int)GetValue(DataGridTopRowProperty); }
			set { SetValue(DataGridTopRowProperty, value); }
		}

        protected override void Invoke(object obj)
		{
			DataGrid dg = this.AssociatedObject as DataGrid; 

			if (dg != null)
			{
				dg.ScrollIntoView(dg.Items[dg.Items.Count - 1]); //scroll to last
				dg.UpdateLayout();
				dg.ScrollIntoView(dg.SelectedItem);
			}
		}
	}
	#endregion

}
