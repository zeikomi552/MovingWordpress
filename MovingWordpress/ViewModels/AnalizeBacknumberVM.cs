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
                }
            }
        }
        #endregion

        #region ブログ解析オブジェクト(バックアップ用)[AnalizerBk]プロパティ
        /// <summary>
        /// ブログ解析オブジェクト(バックアップ用)[AnalizerBk]プロパティ用変数
        /// </summary>
        WpBlogAnalizerM _AnalizerBk = new WpBlogAnalizerM();
        /// <summary>
        /// ブログ解析オブジェクト(バックアップ用)[AnalizerBk]プロパティ
        /// </summary>
        public WpBlogAnalizerM AnalizerBk
        {
            get
            {
                return _AnalizerBk;
            }
            set
            {
                if (_AnalizerBk == null || !_AnalizerBk.Equals(value))
                {
                    _AnalizerBk = value;
                    NotifyPropertyChanged("AnalizerBk");
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

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       new Action(() =>
                       {
                           // 解析オブジェクトのセット
                           this.Analizer = an;

                           // バックアップ
                           this.AnalizerBk = new WpBlogAnalizerM();
                           this.AnalizerBk.RankItems = this.Analizer.RankIntemClone();

                           // 品詞情報をセット
                           var tmp = (from x in this.Analizer.RankItems.Items
                                      select x.PartsOfSpeech).Distinct().ToList<string>();

                           this.PartsOfSpeechSelector.Items = new ObservableCollection<string>(tmp);

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

        #region コンテンツの読み込み処理
        /// <summary>
        /// コンテンツの読み込み処理
        /// </summary>
        public void LoadContents()
        {
            try
            {
                this.BlogContentsManager.LoadContents();
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
                this.Analizer.LoadRank();

                // バックアップ
                this.AnalizerBk = new WpBlogAnalizerM();
                this.AnalizerBk.RankItems = this.Analizer.RankIntemClone();

                var tmp = (from x in this.Analizer.RankItems.Items
                           select x.PartsOfSpeech).Distinct().ToList<string>();

                this.PartsOfSpeechSelector.Items = new ObservableCollection<string>(tmp);
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
                this.Analizer.SaveRank();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
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
                if (this.PartsOfSpeechSelector != null && this.PartsOfSpeechSelector.SelectedItem != null)
                {
                    var tmp = (from x in this.AnalizerBk.RankItems.Items
                               where x.PartsOfSpeech.Equals(this.PartsOfSpeechSelector.SelectedItem)
                               select x).ToList<MecabRankM>();

                    this.Analizer.RankItems.Items = new ObservableCollection<MecabRankM>(tmp);

                    var tmp2 = (from x in this.Analizer.RankItems.Items
                               select x.PartsOfSpeech2).Distinct().ToList<string>();

                    this.PartsOfSpeechSelector2.Items = new ObservableCollection<string>(tmp2);
                }
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
                if (this.PartsOfSpeechSelector2 != null && this.PartsOfSpeechSelector2.SelectedItem != null)
                {
                    var tmp = (from x in this.AnalizerBk.RankItems.Items
                               where x.PartsOfSpeech.Equals(this.PartsOfSpeechSelector.SelectedItem)
                               select x).ToList<MecabRankM>();

                    var tmp2 = (from x in tmp
                                where x.PartsOfSpeech2.Equals(this.PartsOfSpeechSelector2.SelectedItem)
                                select x).ToList<MecabRankM>();

                    this.Analizer.RankItems.Items = new ObservableCollection<MecabRankM>(tmp2);
                }
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
