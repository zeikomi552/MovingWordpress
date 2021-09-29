using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class TwitterContentM : ModelBase
	{
		#region メッセージ[Message]プロパティ
		/// <summary>
		/// メッセージ[Message]プロパティ
		/// </summary>
		public string Message
		{
			get
			{
				return CreateTweetMessage();
			}
		}
		#endregion

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
					NotifyPropertyChanged("Message");
				}
			}
		}
		#endregion

		#region URL[URL]プロパティ
		/// <summary>
		/// URL[URL]プロパティ用変数
		/// </summary>
		string _URL = string.Empty;
		/// <summary>
		/// URL[URL]プロパティ
		/// </summary>
		public string URL
		{
			get
			{
				return _URL;
			}
			set
			{
				if (_URL == null || !_URL.Equals(value))
				{
					_URL = value;
					NotifyPropertyChanged("URL");
					NotifyPropertyChanged("Message");
				}
			}
		}
		#endregion

		#region ハッシュタグ[HashTags]プロパティ
		/// <summary>
		/// ハッシュタグ[HashTags]プロパティ用変数
		/// </summary>
		string _HashTags = string.Empty;
		/// <summary>
		/// ハッシュタグ[HashTags]プロパティ
		/// </summary>
		public string HashTags
		{
			get
			{
				return _HashTags;
			}
			set
			{
				if (_HashTags == null || !_HashTags.Equals(value))
				{
					_HashTags = value;
					NotifyPropertyChanged("HashTags");
					NotifyPropertyChanged("Message");
				}
			}
		}
		#endregion

		#region ツイートの作成
		/// <summary>
		/// ツイートの作成
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <returns>ツイート</returns>
		public string CreateTweetMessage()
		{
			return this.Title + "\n" + this.HashTags + "\n\n" + this.URL;
		}
		#endregion

	}
}
