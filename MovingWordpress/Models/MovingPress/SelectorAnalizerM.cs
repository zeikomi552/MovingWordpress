using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MovingWordpress.Models
{
    public class SelectorAnalizerM : ModelBase
    {
        #region ブログ解析オブジェクト[Analizer]プロパティ
        /// <summary>
        /// ブログ解析オブジェクト[Analizer]プロパティ用変数
        /// </summary>
        WpBlogAnalizerM _Analizer = new WpBlogAnalizerM();
        /// <summary>
        /// ブログ解析オブジェクト[Analizer]プロパティ
        /// </summary>
        public WpBlogAnalizerM Analizer
        {
            get
            {
                return _Analizer;
            }
            set
            {
                if (_Analizer == null || !_Analizer.Equals(value))
                {
                    _Analizer = value;
                    NotifyPropertyChanged("Analizer");
                    NotifyPropertyChanged("DisplayAnalizer");
                }
            }
        }
        #endregion

        #region 表示用アナライザ
        /// <summary>
        /// 表示用アナライザ
        /// </summary>
        [XmlIgnoreAttribute]
        public WpBlogAnalizerM DisplayAnalizer
        {
            get
            {
                WpBlogAnalizerM ret = new WpBlogAnalizerM();

                // nullチェック
                if (this.PartsOfSpeechSelector != null && this.PartsOfSpeechSelector.SelectedItem != null)
                {
                    var tmp = from x in this.Analizer.RankItems.Items
                              where x.PartsOfSpeech.Equals(this.PartsOfSpeechSelector.SelectedItem)
                              select x;

                    // 選択要素のチェック
                    if (this.PartsOfSpeechSelector2 != null && this.PartsOfSpeechSelector2.SelectedItem != null)
                    {
                        var tmp2 = from x in tmp
                                   where x.PartsOfSpeech2.Equals(this.PartsOfSpeechSelector2.SelectedItem)
                                   select x;

                        ret.RankItems.Items = new ObservableCollection<MecabRankM>(tmp2.ToList<MecabRankM>());

                        return ret;
                    }
                    else
                    {
                        ret.RankItems.Items = new ObservableCollection<MecabRankM>(tmp.ToList<MecabRankM>());
                        return ret;
                    }
                }
                else
                {
                    return this.Analizer;
                }
            }
        }
        #endregion

        #region 品詞選択項目[PartsOfSpeechSelector]プロパティ
        /// <summary>
        /// 品詞選択項目[PartsOfSpeechSelector]プロパティ用変数
        /// </summary>
        ModelList<string> _PartsOfSpeechSelector = new ModelList<string>();
        /// <summary>
        /// 品詞選択項目[PartsOfSpeechSelector]プロパティ
        /// </summary>
        public ModelList<string> PartsOfSpeechSelector
        {
            get
            {
                return _PartsOfSpeechSelector;
            }
            set
            {
                if (_PartsOfSpeechSelector == null || !_PartsOfSpeechSelector.Equals(value))
                {
                    _PartsOfSpeechSelector = value;
                    NotifyPropertyChanged("PartsOfSpeechSelector");
                }
            }
        }
        #endregion

        #region 品詞選択第2項目[PartsOfSpeechSelector2]プロパティ
        /// <summary>
        /// 品詞選択第2項目[PartsOfSpeechSelector2]プロパティ用変数
        /// </summary>
        ModelList<string> _PartsOfSpeechSelector2 = new ModelList<string>();
        /// <summary>
        /// 品詞選択第2項目[PartsOfSpeechSelector2]プロパティ
        /// </summary>
        public ModelList<string> PartsOfSpeechSelector2
        {
            get
            {
                return _PartsOfSpeechSelector2;
            }
            set
            {
                if (_PartsOfSpeechSelector2 == null || !_PartsOfSpeechSelector2.Equals(value))
                {
                    _PartsOfSpeechSelector2 = value;
                    NotifyPropertyChanged("PartsOfSpeechSelector2");
                }
            }
        }
        #endregion

        #region 推奨カテゴリ[RecomendCategory]プロパティ
        /// <summary>
        /// 推奨カテゴリ[RecomendCategory]プロパティ用変数
        /// </summary>
        string _RecomendCategory = string.Empty;
        /// <summary>
        /// 推奨カテゴリ[RecomendCategory]プロパティ
        /// </summary>
        public string RecomendCategory
        {
            get
            {
                return _RecomendCategory;
            }
            set
            {
                if (_RecomendCategory == null || !_RecomendCategory.Equals(value))
                {
                    _RecomendCategory = value;
                    NotifyPropertyChanged("RecomendCategory");
                }
            }
        }
        #endregion

        #region 推奨タグ[ReccomendTags]プロパティ
        /// <summary>
        /// 推奨タグ[ReccomendTags]プロパティ用変数
        /// </summary>
        string _ReccomendTags = string.Empty;
        /// <summary>
        /// 推奨タグ[ReccomendTags]プロパティ
        /// </summary>
        public string ReccomendTags
        {
            get
            {
                return _ReccomendTags;
            }
            set
            {
                if (_ReccomendTags == null || !_ReccomendTags.Equals(value))
                {
                    _ReccomendTags = value;
                    NotifyPropertyChanged("ReccomendTags");
                }
            }
        }
        #endregion

        #region 推奨タグのセット処理
        /// <summary>
        /// 推奨タグのセット処理
        /// </summary>
        public void SetTagRecommend()
        {
            StringBuilder nouns = new StringBuilder();

            int count = 0;  // カウント

            // 頻出名詞トップ10を取得しセットする
            foreach (var tmp in this.Analizer.RankItems.Items)
            {
                if (tmp.PartsOfSpeech.Equals("名詞") && tmp.PartsOfSpeech2.Equals("一般") && count < 10)
                {
                    // 一応区切り文字
                    if (count > 0) { nouns.Append(","); }

                    nouns.Append(tmp.Surface);  // 推奨名詞のセット
                    count++;
                }
            }

            this.ReccomendTags = nouns.ToString();
        }
        #endregion

        #region 推奨カテゴリのセット処理
        /// <summary>
        /// 推奨カテゴリのセット処理
        /// </summary>
        /// <param name="analizer">記事全体のアナライザ</param>
        public void SetRecommendCategory(SelectorAnalizerM analizer)
        {
            var tags = from x in this.Analizer.RankItems.Items
                       where x.PartsOfSpeech.Equals("名詞") && x.PartsOfSpeech2.Equals("一般")
                       select x;

            var cates = from x in analizer.Analizer.RankItems.Items
                        where x.PartsOfSpeech.Equals("名詞") && x.PartsOfSpeech2.Equals("一般")
                        select x;

            foreach (var tag in tags)
            {
                int index = 0;
                foreach (var cate in cates)
                {
                    if (index > 20) break;  // カテゴリの上位20を超えたら推奨カテゴリとはしない

                    // 上位20位以内のカテゴリと合致するかどうかを確認する
                    if (tag.Surface.Equals(cate.Surface))
                    {
                        this.RecomendCategory = tag.Surface;
                        return;
                    }

                    index++;
                }
            }
        }
        #endregion

        #region 品詞の選択変更
        /// <summary>
        /// 品詞の選択変更
        /// </summary>
        public void SelectionChanged()
        {
            try
            {
                this.PartsOfSpeechSelector2.Items = new ObservableCollection<string>((from x in this.Analizer.RankItems.Items
                                                                                      where x.PartsOfSpeech.Equals(this.PartsOfSpeechSelector.SelectedItem)
                                                                                      select x.PartsOfSpeech2).Distinct().ToList<string>());

                NotifyPropertyChanged("DisplayAnalizer");
                NotifyPropertyChanged("PartsOfSpeechSelector2");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 品詞2の選択変更
        /// <summary>
        /// 品詞2の選択変更
        /// </summary>
        public void Selection2Changed()
        {
            try
            {
                NotifyPropertyChanged("DisplayAnalizer");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region MeCabの結果をセットする
        /// <summary>
        /// MeCabの結果をセットする
        /// </summary>
        public void SetAnalizeResult(WpBlogAnalizerM result)
        {
            // 解析オブジェクトのセット
            this.Analizer = result;

            // 品詞情報をセット
            var tmp = (from x in this.Analizer.RankItems.Items
                       select x.PartsOfSpeech).Distinct().ToList<string>();

            this.PartsOfSpeechSelector.Items = new ObservableCollection<string>(tmp);

            // 推奨タグのセット
            SetTagRecommend();
        }
        #endregion

    }
}
