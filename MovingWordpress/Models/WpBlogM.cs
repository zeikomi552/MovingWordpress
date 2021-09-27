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

		#region バックナンバー作成
		/// <summary>
		/// バックナンバー作成
		/// </summary>
		public void CreateBackNumber()
		{

			// ダイアログのインスタンスを生成
			var dialog = new SaveFileDialog();

			// ファイルの種類を設定
			dialog.Filter = "マークダウン (*.md)|*.md";
			dialog.FileName = $"バックナンバー-{DateTime.Today.ToString("yyyyMMdd")}";

			// ダイアログを表示する
			if (dialog.ShowDialog() == true)
			{
				string text = CreateBackNumberForMonth();
				// バックアップの保存
				File.WriteAllText(dialog.FileName, text);
			}

		}
		#endregion

		#region タイトル順に並べて出力
		/// <summary>
		/// タイトル順に並べて出力
		/// </summary>
		/// <returns>タイトル順のバックナンバー</returns>
		public string CreateBackNumberForTitle()
		{
			StringBuilder text = new StringBuilder();

			var sort_contents = this.BlogContents.Items.OrderBy(x => x.Post_title);

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

		public string CreateBackNumberForMonth()
		{
			StringBuilder text = new StringBuilder();

			var sort_contents = this.BlogContents.Items.OrderBy(x => x.Post_date);

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
	}
}
