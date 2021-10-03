using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.ViewModels
{
    public class TwitterSearchVM : TwitterAPIVM
    {
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



		public void Search()
		{
			try
			{
				string keyword = this.SearchKeyword;

				if (string.IsNullOrWhiteSpace(this.SearchKeyword))
                {
					ShowMessage.ShowNoticeOK("キーワードは入力必須です", "通知");
					return;
                }

				var result = this.TwitterAPI.Tokens.Search.Tweets(count => 100, q => keyword, lang => "ja");


				StringBuilder tweet_text = new StringBuilder();
				foreach (var value in result)
				{
					var tmp = (from x in this.StatusList.Items
							   where x.Id.Equals(value.Id)
							   select x).Count() > 0;

					if (!tmp)
					{
						this.StatusList.Items.Add(value);
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}

		/// <summary>
		/// URLを既定のブラウザで開く
		/// </summary>
		/// <param name="url">URL</param>
		/// <returns>Process</returns>
		private Process OpenUrl(string url)
		{
			ProcessStartInfo pi = new ProcessStartInfo()
			{
				FileName = url,
				UseShellExecute = true,
			};

			return Process.Start(pi);
		}

		public void MoveProfiel()
		{
			try
			{
				if(this.StatusList.SelectedItem != null)
                {
					string url = "https://twitter.com/" + this.StatusList.SelectedItem.User.ScreenName;

					OpenUrl(url);
				}
			}
			catch (Exception e)
			{
				_logger.Error(e.Message);
				ShowMessage.ShowErrorOK(e.Message, "Error");
			}
		}

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
	}
}
