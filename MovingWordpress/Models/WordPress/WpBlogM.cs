using Microsoft.Win32;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class WpBlogM : ModelBase
	{
		#region ブログ要素[BlogContents]プロパティ
		/// <summary>
		/// ブログ要素[BlogContents]プロパティ用変数
		/// </summary>
		ModelList<WpContentsM> _BlogContents = new ModelList<WpContentsM>();
		/// <summary>
		/// ブログ要素[BlogContents]プロパティ
		/// </summary>
		public ModelList<WpContentsM> BlogContents
		{
			get
			{
				return _BlogContents;
			}
			set
			{
				if (_BlogContents == null || !_BlogContents.Equals(value))
				{
					_BlogContents = value;
					NotifyPropertyChanged("BlogContents");
				}
			}
		}
		#endregion

		#region 要素の追加処理
		/// <summary>
		/// 要素の追加処理
		/// </summary>
		/// <param name="element">コンテンツ</param>
		public void Add(WpContentsM element)
        {
			this.BlogContents.Items.Add(element);
        }
		#endregion

		#region 全テキストの返却
		/// <summary>
		/// 全テキストの返却
		/// </summary>
		/// <returns>全テキスト</returns>
		public string GetAllText()
		{
			StringBuilder text = new StringBuilder();

			// コンテンツデータを連結していく
			foreach (var tmp in this.BlogContents.Items)
			{
				text.AppendLine(tmp.Post_content_Except);
			}

			return text.ToString();
		}
		#endregion



		#region バックナンバーの出力処理
		/// <summary>
		/// バックナンバーの出力処理
		/// </summary>
		/// <param name="file_path">出力ファイルパス</param>
		/// <param name="type">出力タイプ</param>
		public void OutputBackNumber(string file_path, OutputTypeElementM.OutputTypeEnum type)
		{

			string text = string.Empty;
			switch (type)
			{
				// 日付で昇順に並べる
				case OutputTypeElementM.OutputTypeEnum.DateAsc:
					{
						// バックナンバー記事を日付で作成
						text = this.CreateBackNumberForMonth(false);
						break;
					}
				// 日付で降順に並べる
				case OutputTypeElementM.OutputTypeEnum.DateDesc:
					{
						// バックナンバー記事を日付で作成
						text = this.CreateBackNumberForMonth(true);
						break;
					}
				// 名前で昇順に並べる
				case OutputTypeElementM.OutputTypeEnum.NameAsc:
					{
						// バックナンバー記事を名前で作成
						text = this.CreateBackNumberForTitle(false);
						break;
					}
				// 名前で降順に並べる
				case OutputTypeElementM.OutputTypeEnum.NameDesc:
				default:
					{
						// バックナンバー記事を名前で作成
						text = this.CreateBackNumberForTitle(true);
						break;

					}
			}

			// バックアップの保存
			File.WriteAllText(file_path, text);
		}
		#endregion


		#region タイトル順に並べて出力
		/// <summary>
		/// タイトル順に並べて出力
		/// </summary>
		/// <returns>タイトル順のバックナンバー</returns>
		public string CreateBackNumberForTitle(bool desc = false)
		{
			StringBuilder text = new StringBuilder();

			var sort_contents = desc ?
				this.BlogContents.Items.OrderByDescending(x => x.Post_title)
				: this.BlogContents.Items.OrderBy(x => x.Post_title);
			
			text.AppendLine($"## バックナンバー(タイトル順)");
			text.Append($"{DateTime.Today.ToString("yyyy年MM月dd日")} 作成");
			text.AppendLine();

			foreach (var tmp in sort_contents)
			{
				if (tmp.Post_status.Equals("publish"))
				{
					text.AppendLine($"- [{tmp.Post_title}]({tmp.Guid})");
				}
			}

			return text.ToString();
		}
		#endregion

		#region 日付順に並べてバックナンバーを作成する
		/// <summary>
		/// 日付順に並べてバックナンバーを作成する
		/// </summary>
		/// <param name="desc">true:昇順 false:降順</param>
		/// <returns>バックナンバー記事</returns>
		public string CreateBackNumberForMonth(bool desc = false)
		{
			StringBuilder text = new StringBuilder();

			var sort_contents = desc ? 
				this.BlogContents.Items.OrderByDescending(x => x.Post_date)
				: this.BlogContents.Items.OrderBy(x => x.Post_date);

			text.AppendLine($"## バックナンバー(日付順)");
			text.Append($"{DateTime.Today.ToString("yyyy年MM月dd日")} 作成");
			text.AppendLine();

			int year = 0;
			int month = 0;
			foreach (var tmp in sort_contents)
			{
				if (year != tmp.Post_date.Year || month != tmp.Post_date.Month)
				{
					year = tmp.Post_date.Year;
					month = tmp.Post_date.Month;

					text.AppendLine();
					text.AppendLine($"### {year}年{month}月");
				}

				if (tmp.Post_status.Equals("publish"))
				{
					text.AppendLine($"- [{tmp.Post_title}]({tmp.Guid}) ({tmp.Post_date.ToString("dd日")})");
				}
			}

			return text.ToString();
		}
		#endregion
	}
}
