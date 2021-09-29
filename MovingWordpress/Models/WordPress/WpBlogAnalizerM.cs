using MeCab;
using Microsoft.Win32;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MovingWordpress.Models
{
    public class WpBlogAnalizerM : ModelBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public WpBlogAnalizerM()
		{
			// 初期化時にフォルダをコピーする
			CopyDic();
		}
		#endregion

		#region インストールフォルダのdicフォルダパス
		/// <summary>
		/// インストールフォルダのdicフォルダパス
		/// </summary>
		/// <returns></returns>
		public static string GetSourceDicDir()
		{
			// Configフォルダのパス取得
			Assembly myAssembly = Assembly.GetEntryAssembly();
			return Path.Combine(System.IO.Path.GetDirectoryName(myAssembly.Location), "dic");
		}
		#endregion

		#region AppDataのdicフォルダパス
		/// <summary>
		/// AppDataのdicフォルダパス
		/// </summary>
		/// <returns></returns>
		public static string GetAppDataDicDir()
		{
			// Configフォルダのパス取得
			return Path.Combine(ConfigM.GetApplicationFolder(), "dic");
		}
		#endregion

		#region 辞書ファイルをコピーする
		/// <summary>
		/// 辞書ファイルをコピーする
		/// </summary>
		public static void CopyDic()
		{
			// Configフォルダのパス取得
			string dic_dir = GetAppDataDicDir();
			// フォルダの存在確認
			if (!Directory.Exists(dic_dir))
			{
				// 存在しない場合は作成
				DirectoryUtil.CreateDirectory(dic_dir);
				DirectoryInfo sourceDirectory = new DirectoryInfo(GetSourceDicDir());
				DirectoryInfo destinationDirectory = new DirectoryInfo(dic_dir);

				//ファイルのコピー
				foreach (FileInfo fileInfo in sourceDirectory.GetFiles())
				{
					try
					{
						//同じファイルが存在していたら、常に上書きする
						fileInfo.CopyTo(Path.Combine(destinationDirectory.FullName,fileInfo.Name), true);
					}
					catch { }
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

		#region 頻出単語ランキングのクローン
		/// <summary>
		/// 頻出単語ランキングのクローン
		/// </summary>
		/// <returns>クローン結果</returns>
		public ModelList<MecabRankM> RankItemClone()
		{
			ModelList<MecabRankM> ret = new ModelList<MecabRankM>();

			foreach (var tmp in this.RankItems.Items)
			{
				ret.Items.Add(tmp.ShallowCopy<MecabRankM>());
			}

			return ret;
		}
		#endregion

		#region MeCabを使用した処理
		/// <summary>
		/// MeCabを使用した処理
		/// </summary>
		public void UseMecab(string text)
		{
			ModelList<MeCab.MeCabNode> mecab = new ModelList<MeCabNode>();
			MeCabParam mp = new MeCabParam();
			mp.DicDir = GetAppDataDicDir();

			var tagger = MeCabTagger.Create(mp);
			
			mecab.Items.Clear();
			List<string> noun = new List<string>();

			// 記事毎の内容をMeCabで分析
			foreach (var node in tagger.ParseToNodes(text))
			{
				if (0 < node.CharType)
				{
					mecab.Items.Add(node);
				}
			}

			var result = (from x in mecab.Items
						  group x by new { x.Surface, x.Feature } into g
						  select new MecabRankM (){ Surface = g.Key.Surface, Feature = g.Key.Feature, Count = g.Count() })
							.OrderByDescending(x => x.Count).ToList<MecabRankM>();

			this.RankItems.Items = new ObservableCollection<MecabRankM>(result);
		}
		#endregion
	}
}
