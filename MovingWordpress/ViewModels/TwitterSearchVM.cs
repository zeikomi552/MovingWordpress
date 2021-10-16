using MovingWordpress.Common;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovingWordpress.ViewModels
{
    public class TwitterSearchVM : TwitterAPIVM
    {
		#region フィルタ[Filter]プロパティ
		/// <summary>
		/// フィルタ[Filter]プロパティ用変数
		/// </summary>
		string _Filter = string.Empty;
		/// <summary>
		/// フィルタ[Filter]プロパティ
		/// </summary>
		public string Filter
		{
			get
			{
				return _Filter;
			}
			set
			{
				if (_Filter == null || !_Filter.Equals(value))
				{
					_Filter = value;
					NotifyPropertyChanged("Filter");
				}
			}
		}
		#endregion

		#region フィルタリスト[FilteredList]プロパティ
		/// <summary>
		/// フィルタリスト[FilteredList]プロパティ用変数
		/// </summary>
		ModelList<CoreTweet.Status> _FilteredList = new ModelList<CoreTweet.Status>();
		/// <summary>
		/// フィルタリスト[FilteredList]プロパティ
		/// </summary>
		public ModelList<CoreTweet.Status> FilteredList
		{
			get
			{
				return _FilteredList;
			}
			set
			{
				if (_FilteredList == null || !_FilteredList.Equals(value))
				{
					_FilteredList = value;
					NotifyPropertyChanged("FilteredList");
				}
			}
		}
		#endregion

		#region 検索キーワード[SearchKeyword]プロパティ
		/// <summary>
		/// 検索キーワード[SearchKeyword]プロパティ用変数
		/// </summary>
		string _SearchKeyword = string.Empty;
		/// <summary>
		/// 検索キーワード[SearchKeyword]プロパティ
		/// </summary>
		public string SearchKeyword
		{
			get
			{
				return _SearchKeyword;
			}
			set
			{
				if (_SearchKeyword == null || !_SearchKeyword.Equals(value))
				{
					_SearchKeyword = value;
					NotifyPropertyChanged("SearchKeyword");
				}
			}
		}
		#endregion

		#region 検索結果[StatusList]プロパティ
		/// <summary>
		/// 検索結果[StatusList]プロパティ用変数
		/// </summary>
		ModelList<CoreTweet.Status> _StatusList = new ModelList<CoreTweet.Status>();
		/// <summary>
		/// 検索結果[StatusList]プロパティ
		/// </summary>
		public ModelList<CoreTweet.Status> StatusList
		{
			get
			{
				return _StatusList;
			}
			set
			{
				if (_StatusList == null || !_StatusList.Equals(value))
				{
					_StatusList = value;
					NotifyPropertyChanged("StatusList");
				}
			}
		}
		#endregion

		#region 検索処理
		/// <summary>
		/// 検索処理
		/// </summary>
		public void Search()
		{
			try
			{
				// トークンの作成
				this.TwitterAPI.CreateToken(this.TwitterConfig.KeysM.ConsumerKey,
					this.TwitterConfig.KeysM.ConsumerSecretKey, this.TwitterConfig.KeysM.AccessToken, this.TwitterConfig.KeysM.AccessSecret);

				string keyword = this.SearchKeyword;

				if (string.IsNullOrWhiteSpace(this.SearchKeyword))
                {
					ShowMessage.ShowNoticeOK("キーワードは入力必須です", "通知");
					return;
                }

				var result = this.TwitterAPI.Tokens.Search.Tweets(count => 100, q => keyword, lang => "ja");

				this.RateLimit = result.RateLimit;

				StringBuilder tweet_text = new StringBuilder();
				foreach (var value in result)
				{
					var tmp = (from x in this.StatusList.Items
							   where x.Id.Equals(value.Id)
							   select x).Count() > 0;

					if (!tmp)
					{
						this.StatusList.Items.Add(value);

						var status = (from x in this.StatusList.Items
									  select x);
						this.FilteredList.Items 
							= new ObservableCollection<CoreTweet.Status>(status);
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region フィルタ押下処理
		/// <summary>
		/// フィルタ押下処理
		/// </summary>
		public void OnFilter()
		{
            try
            {
				if (!string.IsNullOrWhiteSpace(this.Filter))
				{
					var tmp = (from x in this.StatusList.Items
							   where x.Text.Contains(this.Filter) || x.User.Description.Contains(this.Filter)
							   select x);
					this.FilteredList.Items
						= new ObservableCollection<CoreTweet.Status>(tmp);
				}
				else
				{
					var status = (from x in this.StatusList.Items select x);
					this.FilteredList.Items
						= new ObservableCollection<CoreTweet.Status>(status);
				}
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion


		#region 選択行のURLへ移動する
		/// <summary>
		/// 選択行のURLへ移動する
		/// </summary>
		public void MoveProfiel()
		{
			try
			{
				if(this.StatusList.SelectedItem != null)
                {
					string url = "https://twitter.com/" + this.StatusList.SelectedItem.User.ScreenName;

					MovingWordpressUtilities.OpenUrl(url);
				}
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion

		#region 行クリア
		/// <summary>
		/// 行クリア
		/// </summary>
		public void Clear()
		{
			try
			{
				this.StatusList.Items.Clear();
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}
		#endregion


		#region 行ダブルクリック処理
		/// <summary>
		/// 行ダブルクリック処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="ev"></param>
		public void RowDoubleClick(object sender, MouseButtonEventArgs ev)
		{
			try
			{
				var dg = sender as DataGrid;
				if (null != dg.SelectedItem)
				{
					var ctrl = dg.ItemContainerGenerator.ContainerFromItem(dg.SelectedItem) as DataGridRow;
					if (null != ctrl)
					{
						if (null != ctrl.InputHitTest(ev.GetPosition(ctrl)))
						{
							var data = ctrl.DataContext as CoreTweet.Status;

							if (data != null)
							{
								string url = "https://twitter.com/" + data.User.ScreenName;
								MovingWordpressUtilities.OpenUrl(url);
							}
						}
					}
				}
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
