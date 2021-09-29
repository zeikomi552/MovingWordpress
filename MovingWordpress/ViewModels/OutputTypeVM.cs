using MovingWordpress.Models;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.ViewModels
{
    public class OutputTypeVM : ViewModelBase
    {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OutputTypeVM()
		{
			this.TypeItems = new ModelList<OutputTypeElementM>();
			this.TypeItems.Items.Add(new OutputTypeElementM() { OutputType = OutputTypeElementM.OutputTypeEnum.DateAsc, DisplayType = "日付順", });
			this.TypeItems.Items.Add(new OutputTypeElementM() { OutputType = OutputTypeElementM.OutputTypeEnum.DateDesc, DisplayType = "日付逆順", });
			this.TypeItems.Items.Add(new OutputTypeElementM() { OutputType = OutputTypeElementM.OutputTypeEnum.NameAsc, DisplayType = "名前順", });
			this.TypeItems.Items.Add(new OutputTypeElementM() { OutputType = OutputTypeElementM.OutputTypeEnum.NameDesc, DisplayType = "名前逆順", });
		}

		#region タイトル[Title]プロパティ
		/// <summary>
		/// タイトル[Title]プロパティ用変数
		/// </summary>
		string _Title = string.Empty;
		/// <summary>
		/// タイトル[Title]プロパティ
		/// </summary>
		public string Title
		{
			get
			{
				return _Title;
			}
			set
			{
				if (_Title == null || !_Title.Equals(value))
				{
					_Title = value;
					NotifyPropertyChanged("Title");
				}
			}
		}
		#endregion

		#region 要素[TypeItems]プロパティ
		/// <summary>
		/// 要素[TypeItems]プロパティ用変数
		/// </summary>
		ModelList<OutputTypeElementM> _TypeItems = new ModelList<OutputTypeElementM>();
		/// <summary>
		/// 要素[TypeItems]プロパティ
		/// </summary>
		public ModelList<OutputTypeElementM> TypeItems
		{
			get
			{
				return _TypeItems;
			}
			set
			{
				if (_TypeItems == null || !_TypeItems.Equals(value))
				{
					_TypeItems = value;
					NotifyPropertyChanged("TypeItems");
				}
			}
		}
		#endregion

		#region 初期化処理
		/// <summary>
		/// 初期化処理
		/// </summary>
		public void Init()
        {
			try
			{


			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region OKボタンを押した場合の処理
		/// <summary>
		/// OKボタンを押した場合の処理
		/// </summary>
		public void OnOK()
		{
			try
			{
				this.DialogResult = true;

			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region キャンセルボタンを押した場合の処理
		/// <summary>
		/// キャンセルボタンを押した場合の処理
		/// </summary>
		public void OnCancel()
		{
			try
			{
				this.DialogResult = false;
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion
	}
}
