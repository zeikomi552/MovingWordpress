using Microsoft.Win32;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class TagCateM : ModelBase
    {
		#region 各記事のカテゴリとタグ[BlogContents]プロパティ
		/// <summary>
		/// 各記事のカテゴリとタグ[BlogContents]プロパティ用変数
		/// </summary>
		ModelList<WpContentsM> _BlogContents = new ModelList<WpContentsM>();
		/// <summary>
		/// 各記事のカテゴリとタグ[BlogContents]プロパティ
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

		#region 全記事のカテゴリとタグ[AllContents]プロパティ
		/// <summary>
		/// 全記事のカテゴリとタグ[AllContents]プロパティ用変数
		/// </summary>
		ModelList<MecabRankM> _AllContents = new ModelList<MecabRankM>();
		/// <summary>
		/// 全記事のカテゴリとタグ[AllContents]プロパティ
		/// </summary>
		public ModelList<MecabRankM> AllContents
		{
			get
			{
				return _AllContents;
			}
			set
			{
				if (_AllContents == null || !_AllContents.Equals(value))
				{
					_AllContents = value;
					NotifyPropertyChanged("AllContents");
				}
			}
		}
		#endregion

		#region 保存処理
		/// <summary>
		/// 保存処理
		/// </summary>
		/// <param name="tag_cate">タグカテ</param>
		public void Save(TagCateM tag_cate)
		{
			Save(tag_cate.AllContents, tag_cate.BlogContents);
		}
        #endregion

        #region 保存処理
        /// <summary>
        /// 保存処理
        /// </summary>
        /// <param name="all_contents">全記事のカテゴリ情報</param>
        /// <param name="blog_contents">個別記事のカテゴリ情報</param>
        public void Save(ModelList<MecabRankM> all_contents, ModelList<WpContentsM> blog_contents)
		{
			var backup = new TagCateM();
			backup.AllContents = all_contents;
			backup.BlogContents = blog_contents;

			// ダイアログのインスタンスを生成
			var dialog = new SaveFileDialog();

			// ファイルの種類を設定
			dialog.Filter = "タグカテファイル (*.wmcate)|*.wmcate";

			// ダイアログを表示する
			if (dialog.ShowDialog() == true)
			{
				// バックアップの保存
				XMLUtil.Seialize<TagCateM>(dialog.FileName, backup);
			}
		}
		#endregion

		#region ロード処理
		/// <summary>
		/// ロード処理
		/// </summary>
		/// <returns>読み込み結果</returns>
		public TagCateM Load()
		{
			// ダイアログのインスタンスを生成
			var dialog = new OpenFileDialog();

			// ファイルの種類を設定
			dialog.Filter = "タグカテファイル (*.wmcate)|*.wmcate";

			// ダイアログを表示する
			if (dialog.ShowDialog() == true)
			{
				// バックアップの保存
				return Load(dialog.FileName);
			}
			else
			{
				return null;
			}
		}
		#endregion

		#region ロード処理
		/// <summary>
		/// ロード処理
		/// </summary>
		/// <param name="file_path">ファイルパス</param>
		/// <returns>読み込み結果</returns>
		public TagCateM Load(string file_path)
		{
			return XMLUtil.Deserialize<TagCateM>(file_path);
		}
		#endregion
	}
}
