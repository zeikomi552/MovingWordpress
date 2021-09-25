using MeCab;
using Microsoft.Win32;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class WpBlogAnalizerM : ModelBase
	{
		#region MeCabの解析結果[MeCabItems]プロパティ
		/// <summary>
		/// MeCabの解析結果[MeCabItems]プロパティ用変数
		/// </summary>
		ModelList<MeCab.MeCabNode> _MeCabItems = new ModelList<MeCab.MeCabNode>();
		/// <summary>
		/// MeCabの解析結果[MeCabItems]プロパティ
		/// </summary>
		public ModelList<MeCab.MeCabNode> MeCabItems
		{
			get
			{
				return _MeCabItems;
			}
			set
			{
				if (_MeCabItems == null || !_MeCabItems.Equals(value))
				{
					_MeCabItems = value;
					NotifyPropertyChanged("MeCabItems");
				}
			}
		}
		#endregion

		#region 出現ランキング情報[RankItems]プロパティ
		/// <summary>
		/// 出現ランキング情報[RankItems]プロパティ用変数
		/// </summary>
		ModelList<MecabRankM> _RankItems = new ModelList<MecabRankM>();
		/// <summary>
		/// 出現ランキング情報[RankItems]プロパティ
		/// </summary>
		public ModelList<MecabRankM> RankItems
		{
			get
			{
				return _RankItems;
			}
			set
			{
				if (_RankItems == null || !_RankItems.Equals(value))
				{
					_RankItems = value;
					NotifyPropertyChanged("RankItems");
				}
			}
		}
		#endregion

		/// <summary>
		/// 頻出単語ランキングのクローン
		/// </summary>
		/// <returns>クローン結果</returns>
		public ModelList<MecabRankM> RankIntemClone()
		{
			ModelList<MecabRankM> ret = new ModelList<MecabRankM>();

			foreach (var tmp in this.RankItems.Items)
			{
				ret.Items.Add(tmp.ShallowCopy<MecabRankM>());
			}

			return ret;
		}


		#region MeCabを使用した処理
		/// <summary>
		/// MeCabを使用した処理
		/// </summary>
		public void UseMecab(string text)
		{
			MeCabParam mp = new MeCabParam();
			var tagger = MeCabTagger.Create();
			this.MeCabItems.Items.Clear();
			List<string> noun = new List<string>();

			// 記事毎の内容をMeCabで分析
			foreach (var node in tagger.ParseToNodes(text))
			{
				if (0 < node.CharType)
				{
					this.MeCabItems.Items.Add(node);
				}
			}

			var result = (from x in this.MeCabItems.Items
						  group x by new { x.Surface, x.Feature } into g
						  select new MecabRankM (){ Surface = g.Key.Surface, Feature = g.Key.Feature, Count = g.Count() })
							.OrderByDescending(x => x.Count).ToList<MecabRankM>();

			this.RankItems.Items = new ObservableCollection<MecabRankM>(result);
		}
		#endregion

		#region 頻出単語の保存処理
		/// <summary>
		/// 頻出単語の保存処理
		/// </summary>
		public void SaveRank()
		{
			// ダイアログのインスタンスを生成
			var dialog = new SaveFileDialog();

			// ファイルの種類を設定
			dialog.Filter = "過去記事データ (*.mwrnk)|*.mwrnk";

			// ダイアログを表示する
			if (dialog.ShowDialog() == true)
			{
				XMLUtil.Seialize<ModelList<MecabRankM>>(dialog.FileName, this.RankItems);
			}
		}
		#endregion

		#region 頻出単語のロード処理
		/// <summary>
		/// 頻出単語のロード処理
		/// </summary>
		public void LoadRank()
		{
			// ダイアログのインスタンスを生成
			var dialog = new OpenFileDialog();

			// ファイルの種類を設定
			dialog.Filter = "過去記事データ (*.mwrnk)|*.mwrnk";

			// ダイアログを表示する
			if (dialog.ShowDialog() == true)
			{
				this.RankItems = XMLUtil.Deserialize<ModelList<MecabRankM>>(dialog.FileName);
			}
		}
		#endregion
	}
}
