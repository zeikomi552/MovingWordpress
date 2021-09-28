using ClosedXML.Excel;
using Microsoft.Win32;
using MovingWordpress.Common;
using MovingWordpress.Models;
using MovingWordpress.Views;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
                    NotifyPropertyChanged("IsExecute");
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
                    NotifyPropertyChanged("IsExecute");

                }
            }
        }
        #endregion

        #region 実行中フラグ
        /// <summary>
        /// 実行中フラグ
        /// </summary>
        public bool IsExecute
        {
            get
            {
                return this.IsExecuteAnaize || this.IsExecuteContentAnaize;
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
                // データベースのバックアップファイルのロード
                var tmp = FileAnalyzerM.LoadContents();

                // コンテンツのセット
                this.BlogContentsManager.BlogContents.Items
                    = new ObservableCollection<WpContentsM>(tmp);
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

                           var category_recommend = from x in this.SelectorAnalizer.Analizer.RankItems.Items
                                                    where x.PartsOfSpeech.Equals("名詞") && x.PartsOfSpeech2.Equals("一般")
                                                    select x;

                           foreach (var content in this.BlogContentsManager.BlogContents.Items)
                           {
                               content.SelectorAnalizer.SetRecommendCategory(this.SelectorAnalizer);
                           }

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
                TagCateM tag = new TagCateM();

                var tmp = tag.Load();
                this.BlogContentsManager.BlogContents = tmp.BlogContents;   // 個別記事の情報を取得
                this.SelectorAnalizer.Analizer.RankItems = tmp.AllContents; // 全記事の情報を取得


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
                // 各記事のカテゴリ・タグ情報の取り出し
                var blog_contents = this.BlogContentsManager.BlogContents;

                // 全記事のカテゴリ・タグ情報の取り出し
                var all_contents = this.SelectorAnalizer.Analizer.RankItems;

                TagCateM tag = new TagCateM()
                {
                    BlogContents = blog_contents,
                    AllContents = all_contents
                };

                tag.Save(tag);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 全記事分の分析結果をエクセルに出力する
        /// <summary>
        /// 全記事分の分析結果をエクセルに出力する
        /// </summary>
        /// <param name="workbook">ワークブック</param>
        private void SaveExcelAll(XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("全体");

            worksheet.Cell("A1").Value = "出現回数";
            worksheet.Cell("B1").Value = "単語";
            worksheet.Cell("C1").Value = "形態素解析結果";
            worksheet.Cell("D1").Value = "品詞";
            worksheet.Cell("E1").Value = "品詞2";
            worksheet.Cell("F1").Value = "URL";

            int row = 2;

            foreach (var tmp in this.SelectorAnalizer.Analizer.RankItems.Items)
            {
                worksheet.Cell($"A{row}").Value = tmp.Count;
                worksheet.Cell($"B{row}").Value = tmp.Surface;
                worksheet.Cell($"C{row}").Value = tmp.Feature;
                worksheet.Cell($"D{row}").Value = tmp.PartsOfSpeech;
                worksheet.Cell($"E{row}").Value = tmp.PartsOfSpeech2;
                row++;
            }
        }
        #endregion

        #region 個別の記事の分析結果をエクセルに出力する
        /// <summary>
        /// 個別の記事の分析結果をエクセルに出力する
        /// </summary>
        /// <param name="workbook">ワークブック</param>
        private void SaveExcelContents(XLWorkbook workbook)
        {
            var worksheet = workbook.Worksheets.Add("個別");

            worksheet.Cell("A1").Value = "記事タイトル";
            worksheet.Cell("B1").Value = "出現回数";
            worksheet.Cell("C1").Value = "単語";
            worksheet.Cell("D1").Value = "形態素解析結果";
            worksheet.Cell("E1").Value = "品詞";
            worksheet.Cell("F1").Value = "品詞2";
            worksheet.Cell("G1").Value = "URL";

            int row = 2;

            foreach (var content in this.BlogContentsManager.BlogContents.Items)
            {
                foreach (var tmp in content.SelectorAnalizer.Analizer.RankItems.Items)
                {
                    worksheet.Cell($"A{row}").Value = content.Post_title;
                    worksheet.Cell($"B{row}").Value = tmp.Count;
                    worksheet.Cell($"C{row}").Value = tmp.Surface;
                    worksheet.Cell($"D{row}").Value = tmp.Feature;
                    worksheet.Cell($"E{row}").Value = tmp.PartsOfSpeech;
                    worksheet.Cell($"F{row}").Value = tmp.PartsOfSpeech2;
                    worksheet.Cell($"G{row}").Value = content.Guid;
                    row++;
                }
            }
        }
        #endregion

        #region エクセル出力
        /// <summary>
        /// エクセル出力
        /// </summary>
        public void SaveExcel()
        {
            try
            {
                // ダイアログのインスタンスを生成
                var dialog = new SaveFileDialog();

                // ファイルの種類を設定
                dialog.Filter = "エクセルファイル(*.xlsx)|*.xlsx";

                // ダイアログを表示する
                if (dialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        SaveExcelAll(workbook);
                        SaveExcelContents(workbook);
                        workbook.SaveAs(dialog.FileName);

                        ShowMessage.ShowNoticeOK("レポート出力が完了しました。", "通知");

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

        #region バックナンバー作成
        /// <summary>
        /// バックナンバー作成
        /// </summary>
        public void CreateBackNumber()
        {
            try
            {
                // バックナンバー記事の作成
                this.BlogContentsManager.CreateBackNumber();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 記事内容を結合して出力
        /// <summary>
        /// 記事内容を結合して出力
        /// </summary>
        public void CombinePage()
        {
            try
            {
                // ダイアログのインスタンスを生成
                var dialog = new SaveFileDialog();

                // ファイルの種類を設定
                dialog.Filter = "テキストファイル(*.txt)|*.txt";

                // ダイアログを表示する
                if (dialog.ShowDialog() == true)
                {
                    StringBuilder text = new StringBuilder();
                    foreach (var article in this.BlogContentsManager.BlogContents.Items)
                    {
                        text.AppendLine(article.Post_content_Except);
                    }

                    File.WriteAllText(dialog.FileName, text.ToString());

                    ShowMessage.ShowNoticeOK("出力しました。\r\nKH Coderを用いて形態素解析をかけると面白いかもしれません。", "通知");
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        public void AnalizeNewArticle()
        {
            try
            {
                var wnd = new AnalizeNewContentV();
                var vm = wnd.DataContext as AnalizeNewContentVM;

                vm.SelectorAnalizer = this.SelectorAnalizer;

                if (wnd.ShowDialog() == true)
                {

                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
    }
}
