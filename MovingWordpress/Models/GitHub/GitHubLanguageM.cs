using MVVMCore.BaseClass;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models.GitHub
{
    public class GitHubLanguageM : ModelBase
    {
		#region 表示名[DisplayName]プロパティ
		/// <summary>
		/// 表示名[DisplayName]プロパティ用変数
		/// </summary>
		string _DisplayName = string.Empty;
		/// <summary>
		/// 表示名[DisplayName]プロパティ
		/// </summary>
		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
			set
			{
				if (_DisplayName == null || !_DisplayName.Equals(value))
				{
					_DisplayName = value;
					NotifyPropertyChanged("DisplayName");
				}
			}
		}
		#endregion

		#region 使用言語[UseLanguage]プロパティ
		/// <summary>
		/// 使用言語[UseLanguage]プロパティ用変数
		/// </summary>
		Language? _UseLanguage = null;
		/// <summary>
		/// 使用言語[UseLanguage]プロパティ
		/// </summary>
		public Language? UseLanguage
		{
			get
			{
				return _UseLanguage;
			}
			set
			{
				if (_UseLanguage == null || !_UseLanguage.Equals(value))
				{
					_UseLanguage = value;
					NotifyPropertyChanged("UseLanguage");
				}
			}
		}
		#endregion


	}
}
