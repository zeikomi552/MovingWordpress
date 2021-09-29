using MovingWordpress.Models;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MovingWordpress.ViewModels
{
    public class AnalizeNewContentVM : ViewModelBase
    {
        #region 選択要素を含むアナライザ[SelectorAnalizer]プロパティ
        /// <summary>
        /// 選択要素を含むアナライザ[SelectorAnalizer]プロパティ用変数
        /// </summary>
        SelectorAnalizerM _SelectorAnalizer = new SelectorAnalizerM();
        /// <summary>
        /// 選択要素を含むアナライザ[SelectorAnalizer]プロパティ
        /// </summary>
        public SelectorAnalizerM SelectorAnalizer
        {
            get
            {
                return _SelectorAnalizer;
            }
            set
            {
                if (_SelectorAnalizer == null || !_SelectorAnalizer.Equals(value))
                {
                    _SelectorAnalizer = value;
                    NotifyPropertyChanged("SelectorAnalizer");
                }
            }
        }
        #endregion

        #region 現在の記事のアナライザ[CurrentAnaizer]プロパティ
        /// <summary>
        /// 現在の記事のアナライザ[CurrentAnaizer]プロパティ用変数
        /// </summary>
        SelectorAnalizerM _CurrentAnaizer = new SelectorAnalizerM();
        /// <summary>
        /// 現在の記事のアナライザ[CurrentAnaizer]プロパティ
        /// </summary>
        public SelectorAnalizerM CurrentAnaizer
        {
            get
            {
                return _CurrentAnaizer;
            }
            set
            {
                if (_CurrentAnaizer == null || !_CurrentAnaizer.Equals(value))
                {
                    _CurrentAnaizer = value;
                    NotifyPropertyChanged("CurrentAnaizer");
                }
            }
        }
        #endregion

        #region 記事[Article]プロパティ
        /// <summary>
        /// 記事[Article]プロパティ用変数
        /// </summary>
        string _Article = string.Empty;
        /// <summary>
        /// 記事[Article]プロパティ
        /// </summary>
        public string Article
        {
            get
            {
                return _Article;
            }
            set
            {
                if (_Article == null || !_Article.Equals(value))
                {
                    _Article = value;
                    NotifyPropertyChanged("Article");
                }
            }
        }
        #endregion

        #region 解析処理
        /// <summary>
        /// 解析処理
        /// </summary>
        public void ExecuteAnalize()
        {
            try
            {
                var an = new WpBlogAnalizerM();
                // 形態素解析
                an.UseMecab(this.Article);

                // 解析結果をセット
                this.CurrentAnaizer.SetAnalizeResult(an);

                // 推奨カテゴリのセット
                this.CurrentAnaizer.SetRecommendCategory(this.SelectorAnalizer);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region クリップボードにカテゴリをコピーする
        /// <summary>
        /// クリップボードにカテゴリをコピーする
        /// </summary>
        public void CopyCategory()
        {
            try
            {
                //クリップボードに文字列をコピーする
                Clipboard.SetText(this.CurrentAnaizer.RecomendCategory);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region クリップボードにカテゴリをコピーする
        /// <summary>
        /// クリップボードにカテゴリをコピーする
        /// </summary>
        public void CopyTags()
        {
            try
            {
                //クリップボードに文字列をコピーする
                Clipboard.SetText(this.CurrentAnaizer.ReccomendTags);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion
    }
}
