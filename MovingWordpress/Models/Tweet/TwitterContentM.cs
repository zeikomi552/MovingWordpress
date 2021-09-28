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
		/// メッセージ[Message]プロパティ用変数
		/// </summary>
		string _Message = string.Empty;
		/// <summary>
		/// メッセージ[Message]プロパティ
		/// </summary>
		public string Message
		{
			get
			{
				return _Message;
			}
			set
			{
				if (_Message == null || !_Message.Equals(value))
				{
					_Message = value;
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
				}
			}
		}
		#endregion

		/// <summary>
		/// ツイートの作成
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="title">タイトル</param>
		/// <returns>ツイート</returns>
		public string CreateTweetMessage(string url, string title)
		{
			return title + "\n" + this.HashTags + "\n\n" + url;
		}


	}
}
