using Microsoft.Win32;
using MovingWordpress.Common;
using MovingWordpress.Models;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MovingWordpress.ViewModels
{
    public class AnalizeBacknumberVM : ViewModelBase
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

        #region ブログ管理[BlogContentsManager]プロパティ
        /// <summary>
        /// ブログ管理[BlogContentsManager]プロパティ用変数
        /// </summary>
        WpBlogM _BlogContentsManager = new WpBlogM();
        /// <summary>
        /// ブログ管理[BlogContentsManager]プロパティ
        /// </summary>
        public WpBlogM BlogContentsManager
        {
            get
            {
                return _BlogContentsManager;
            }
            set
            {
                if (_BlogContentsManager == null || !_BlogContentsManager.Equals(value))
                {
                    _BlogContentsManager = value;
                    NotifyPropertyChanged("BlogContentsManager");
                }
            }
        }
        #endregion

        #region 解析中フラグ[IsExecuteAnaize]プロパティ
        /// <summary>
        /// 解析中フラグ[IsExecuteAnaize]プロパティ用変数
        /// </summary>
        bool _IsExecuteAnaize = false;
        /// <summary>
        /// 解析中フラグ[IsExecuteAnaize]プロパティ
        /// </summary>
        public bool IsExecuteAnaize
        {
            get
            {
                return _IsExecuteAnaize;
            }
            set
            {
                if (!_IsExecuteAnaize.Equals(value))
                {
                    _IsExecuteAnaize = value;
                    NotifyPropertyChanged("IsExecuteAnaize");
                }
            }
        }
        #endregion

        #region 解析中フラグ[IsExecuteContentAnaize]プロパティ
        /// <summary>
        /// 解析中フラグ[IsExecuteContentAnaize]プロパティ用変数
        /// </summary>
        bool _IsExecuteContentAnaize = false;
        /// <summary>
        /// 解析中フラグ[IsExecuteContentAnaize]プロパティ
        /// </summary>
        public bool IsExecuteContentAnaize
        {
            get
            {
                return _IsExecuteContentAnaize;
            }
            set
            {
                if (!_IsExecuteContentAnaize.Equals(value))
                {
                    _IsExecuteContentAnaize = value;
                    NotifyPropertyChanged("IsExecuteContentAnaize");
                }
            }
        }
        #endregion

        #region 初期化処理
        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Init()
        {
            try
            {

            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region バックアップファイルの読み込み処理
        /// <summary>
        /// バックアップファイルの読み込み処理
        /// </summary>
        public void LoadBackup()
        {
            try
            {
                // ダイアログのインスタンスを生成
                var dialog = new OpenFileDialog();

                // ファイルの種類を設定
                dialog.Filter = "データベースバックアップファイル (*.sql.gz)|*.sql.gz";
                // ダイアログを表示する
                if (dialog.ShowDialog() == true)
                {
                    var query = MovingWordpressUtilities.Decompress(dialog.FileName);

                    // 取得したクエリを分解しINSERT分のみ取得
                    var query_contents = FileAnalyzerM.GetQueryParameters(query);

                    // クエリ要素を回す
                    foreach (var tmp in query_contents)
                    {
                        // 記事情報の抜き出し
                        var wp_content = FileAnalyzerM.DivParameters(tmp);

                        // revisionやautosaveが含まれている場合は履歴なので無視
                        // コンテンツに文字列が含まれてない場合は無視
                        // タイトルがない場合は無視
                        if (wp_content.Post_type.Equals("post"))
                        {
                            this.BlogContentsManager.Add(wp_content);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region コンテンツの形態素解析処理
        /// <summary>
        /// コンテンツの形態素解析処理
        /// </summary>
        public void AnalizeContents()
        {
            try
            {
                var an = new WpBlogAnalizerM();


                Task.Run(() =>
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       new Action(() =>
                       {
                           // 解析開始
                           this.IsExecuteAnaize = true;
                       }));

                    // 形態素解析
                    an.UseMecab(this.BlogContentsManager.GetAllText());

                    // UIスレッド対策
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       new Action(() =>
                       {
                           // 解析オブジェクトのセット
                           this.SelectorAnalizer.SetAnalizeResult(an);

                           // 解析終了
                           this.IsExecuteAnaize = false;
                       }));
                });
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 個別の記事を形態素解析にかける
        /// <summary>
        /// 個別の記事を形態素解析にかける
        /// </summary>
        public void ContentAnalize()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                   new Action(() =>
                   {
                                           // 解析開始
                                           this.IsExecuteContentAnaize = true;
                   }));

                // 記事の数だけ形態素解析にかける
                for (int index = 0; index < this.BlogContentsManager.BlogContents.Items.Count; index++)
                {
                    var elem_an = new WpBlogAnalizerM();
                    var elem = this.BlogContentsManager.BlogContents.Items.ElementAt(index);

                    elem_an.UseMecab(elem.Post_content_Except);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       new Action(() =>
                       {
                           // 頻出単語ランキングのセット
                           elem.SelectorAnalizer.SetAnalizeResult(elem_an);
                       }));
                }

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                   new Action(() =>
                   {
                       // 解析終了
                       this.IsExecuteContentAnaize = false;
                   }));
            });

        }
        #endregion

        #region コンテンツの読み込み処理
        /// <summary>
        /// コンテンツの読み込み処理
        /// </summary>
        public void LoadContents()
        {
            try
            {
                this.BlogContentsManager.LoadContents();
                NotifyPropertyChanged("DisplayAnalizer");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region コンテンツの保存処理
        /// <summary>
        /// コンテンツの保存処理
        /// </summary>
        public void SaveContents()
        {
            try
            {
                this.BlogContentsManager.SaveContents();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 頻出単語のロード処理
        /// <summary>
        /// 頻出単語のロード処理
        /// </summary>
        public void LoadRank()
        {
            try
            {
                // 解析オブジェクトのロード
                this.SelectorAnalizer.Analizer.LoadRank();


                var tmp = (from x in this.SelectorAnalizer.Analizer.RankItems.Items
                           select x.PartsOfSpeech).Distinct().ToList<string>();

                this.SelectorAnalizer.PartsOfSpeechSelector.Items = new ObservableCollection<string>(tmp);
                NotifyPropertyChanged("DisplayAnalizer");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 頻出単語の保存処理
        /// <summary>
        /// 頻出単語の保存処理
        /// </summary>
        public void SaveRank()
        {
            try
            {
                this.SelectorAnalizer.Analizer.SaveRank();
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
