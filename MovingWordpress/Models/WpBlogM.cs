﻿using Microsoft.Win32;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
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

		#region 過去記事のロード処理
		/// <summary>
		/// 過去記事のロード処理
		/// </summary>
		public void LoadContents()
		{
			// ダイアログのインスタンスを生成
			var dialog = new OpenFileDialog();

			// ファイルの種類を設定
			dialog.Filter = "過去記事データ (*.mwc)|*.mwc";

			// ダイアログを表示する
			if (dialog.ShowDialog() == true)
			{
				this.BlogContents = XMLUtil.Deserialize<ModelList<WpContentsM>>(dialog.FileName);
			}
		}
		#endregion

		#region 過去記事の保存処理
		/// <summary>
		/// 過去記事の保存処理
		/// </summary>
		public void SaveContents()
		{
			// ダイアログのインスタンスを生成
			var dialog = new SaveFileDialog();

			// ファイルの種類を設定
			dialog.Filter = "過去記事データ (*.mwc)|*.mwc";

			// ダイアログを表示する
			if (dialog.ShowDialog() == true)
			{
				XMLUtil.Seialize<ModelList<WpContentsM>>(dialog.FileName, this.BlogContents);
			}	
		}
		#endregion
	}
}
