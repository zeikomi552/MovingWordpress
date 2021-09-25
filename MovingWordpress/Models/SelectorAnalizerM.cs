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

        /// <summary>
        /// MeCabの結果をセットする
        /// </summary>
        /// <param name="result">結果データ</param>
        public void SetAnalizeResult(WpBlogAnalizerM result)
        {
            // 解析オブジェクトのセット
            this.Analizer = result;

            // 品詞情報をセット
            var tmp = (from x in this.Analizer.RankItems.Items
                       select x.PartsOfSpeech).Distinct().ToList<string>();

            this.PartsOfSpeechSelector.Items = new ObservableCollection<string>(tmp);
        }

    }
}
