using Microsoft.Win32;
using MovingWordpress.Common;
using MovingWordpress.Models;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // ブログの解析オブジェクト
                this.Analizer = new WpBlogAnalizerM();

                // 形態素解析
                this.Analizer.UseMecab(this.BlogContentsManager.GetAllText());
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
                this.Analizer.LoadRank();
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
    }
}
