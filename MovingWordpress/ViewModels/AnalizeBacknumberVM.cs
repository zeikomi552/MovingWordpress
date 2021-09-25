using Microsoft.Win32;
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

        /// <summary>
        /// バイトのコピー処理
        /// </summary>
        /// <param name="src">コピー元</param>
        /// <param name="dest">コピー先</param>
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        #region 解凍処理
        /// <summary>
        /// 解凍処理
        /// </summary>
        /// <param name="file_path">ファイルパス .sql.gz</param>
        /// <returns>解凍後取り出せた文字列列</returns>
        public string Decompress(string file_path)
        {
            //展開する書庫のパス
            string gzipFile = file_path;

            //展開する書庫のFileStreamを作成する
            using (var gzipFileStrm = new System.IO.FileStream(
                gzipFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //圧縮解除モードのGZipStreamを作成する
                using (var gzipStrm =
                    new System.IO.Compression.GZipStream(gzipFileStrm,
                        System.IO.Compression.CompressionMode.Decompress))
                {
                    using (var mso = new MemoryStream())
                    {
                        // メモリストリームへコピー
                        CopyTo(gzipStrm, mso);

                        // テキストへ変換
                        return Encoding.UTF8.GetString(mso.ToArray());
                    }
                }
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
                    var query = Decompress(dialog.FileName);

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
                this.Analizer = new WpBlogAnalizerM();

                this.Analizer.UseMecab(this.BlogContentsManager.GetAllText());
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
