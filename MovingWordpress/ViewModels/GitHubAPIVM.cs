using MovingWordpress.Common;
using MovingWordpress.Models;
using MovingWordpress.Models.GitHub;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovingWordpress.ViewModels
{
    public class GitHubAPIVM : ViewModelBase
    {
        #region コンフィグ[GitHubAPIConfig]プロパティ
        /// <summary>
        /// コンフィグ[GitHubAPIConfig]プロパティ用変数
        /// </summary>
        GitHubConfigM _GitHubAPIConfig = new GitHubConfigM();
        /// <summary>
        /// コンフィグ[GitHubAPIConfig]プロパティ
        /// </summary>
        public GitHubConfigM GitHubAPIConfig
        {
            get
            {
                return _GitHubAPIConfig;
            }
            set
            {
                if (_GitHubAPIConfig == null || !_GitHubAPIConfig.Equals(value))
                {
                    _GitHubAPIConfig = value;
                    NotifyPropertyChanged("GitHubAPIConfig");
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
                // 検索開始日
                this.SearchDateRange.FromDateBase = null;   // 開始日は無制限
                this.SearchDateRange.ToDateBase = DateTime.Today;     // 終了日は無制限


                this.GitHubAPIConfig.Load();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 検索日付範囲[SearchDateRange]プロパティ
        /// <summary>
        /// 検索日付範囲[SearchDateRange]プロパティ用変数
        /// </summary>
        DateRangeM _SearchDateRange = new DateRangeM();
        /// <summary>
        /// 検索日付範囲[SearchDateRange]プロパティ
        /// </summary>
        public DateRangeM SearchDateRange
        {
            get
            {
                return _SearchDateRange;
            }
            set
            {
                if (_SearchDateRange == null || !_SearchDateRange.Equals(value))
                {
                    _SearchDateRange = value;
                    NotifyPropertyChanged("SearchDateRange");
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

        #region 開発言語リスト[LanguageList]プロパティ
        /// <summary>
        /// 開発言語リスト[LanguageList]プロパティ用変数
        /// </summary>
        ModelList<GitHubLanguageM> _LanguageList = new ModelList<GitHubLanguageM>();
        /// <summary>
        /// 開発言語リスト[LanguageList]プロパティ
        /// </summary>
        public ModelList<GitHubLanguageM> LanguageList
        {
            get
            {
                return _LanguageList;
            }
            set
            {
                if (_LanguageList == null || !_LanguageList.Equals(value))
                {
                    _LanguageList = value;
                    NotifyPropertyChanged("LanguageList");
                }
            }
        }
        #endregion

        #region リポジトリの検索結果[SearchResult]プロパティ
        /// <summary>
        /// リポジトリの検索結果[SearchResult]プロパティ用変数
        /// </summary>
        SearchRepositoryResult _SearchResult = new SearchRepositoryResult();
        /// <summary>
        /// リポジトリの検索結果[SearchResult]プロパティ
        /// </summary>
        public SearchRepositoryResult SearchResult
        {
            get
            {
                return _SearchResult;
            }
            set
            {
                if (_SearchResult == null || !_SearchResult.Equals(value))
                {
                    _SearchResult = value;
                    NotifyPropertyChanged("SearchResult");
                }
            }
        }
        #endregion

        #region ページ[Page]プロパティ
        /// <summary>
        /// ページ[Page]プロパティ用変数
        /// </summary>
        int _Page = 1;
        /// <summary>
        /// ページ[Page]プロパティ
        /// </summary>
        public int Page
        {
            get
            {
                return _Page;
            }
            set
            {
                if (!_Page.Equals(value))
                {
                    _Page = value;
                    NotifyPropertyChanged("Page");
                }
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GitHubAPIVM()
        {
            // 全件の要素を登録する
            GitHubLanguageM all = new GitHubLanguageM() { DisplayName = "-", UseLanguage = null };
            this.LanguageList.Items.Add(all);

            foreach (Language val in Enum.GetValues(typeof(Language)))
            {
                string lang_name = Enum.GetName(typeof(Language), val);
                GitHubLanguageM tmp = new GitHubLanguageM() { DisplayName = lang_name, UseLanguage = val };

                this.LanguageList.Items.Add(tmp);
            }

            // 1つ目の値を取り出す
            this.LanguageList.SelectedItem = this.LanguageList.Items.First();

            // 言語一覧の取り出し
            //string test = GetAllLanguageMarkdown();
        }
        #endregion

        #region 全言語マークダウン取得用
        /// <summary>
        /// 全言語マークダウン取得用
        /// </summary>
        /// <returns>マークダウン</returns>
        private string GetAllLanguageMarkdown()
        {
            StringBuilder test = new StringBuilder();
            test.AppendLine($"|言語|検索|");
            test.AppendLine($"|---|---|");
            foreach (Language val in Enum.GetValues(typeof(Language)))
            {
                string lang_name = Enum.GetName(typeof(Language), val);
                test.AppendLine($"|{lang_name}|[[Google](https://www.google.com/search?q={lang_name})] [[Qiita](https://qiita.com/search?q={lang_name})] [[Wiki(日)](https://ja.wikipedia.org/wiki/{lang_name})] [[Wiki(米)](https://en.wikipedia.org/wiki/{lang_name})]|");
            }
            return test.ToString();
        }
        #endregion

        #region 検索処理
        /// <summary>
        /// 検索処理
        /// </summary>
        /// <param name="page">検索するページ</param>
        private async void Search(int page)
        {
            // GitHub Clientの作成
            var client = new GitHubClient(new ProductHeaderValue(this.GitHubAPIConfig.ProductHeader));

            // トークンの取得
            var tokenAuth = new Credentials(this.GitHubAPIConfig.AccessToken);
            client.Credentials = tokenAuth;

            SearchRepositoriesRequest request = new SearchRepositoriesRequest();
#pragma warning disable CS0618 // 型またはメンバーが旧型式です

            // 値を持っているかどうかのチェック
            if (this.SearchDateRange.HasValue)
            {
                request.Created = new DateRange(this.SearchDateRange.FromDate, this.SearchDateRange.ToDate);
            }

            // スターの数
            request.Stars = new Octokit.Range(1, int.MaxValue);

            // 読み込むページ
            request.Page = page;

            // スターの数でソート
            request.SortField = RepoSearchSort.Stars;

            request.Language = this.LanguageList.SelectedItem.UseLanguage;

            // 降順でソート
            request.Order = SortDirection.Descending;
#pragma warning restore CS0618 // 型またはメンバーが旧型式です

            this.SearchResult = await client.Search.SearchRepo(request);

            // 記事の作成
            this.Article = RepositorySearchResultM.GetArticle(this.SearchDateRange, request.Language, this.SearchResult);
        }
        #endregion

        #region リポジトリ検索処理
        /// <summary>
        /// リポジトリ検索処理
        /// </summary>
        public void Search()
        {
            try
            {
                this.Page = 1;
                Search(this.Page);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 次のページへ移動
        /// <summary>
        /// 次のページへ移動
        /// </summary>
        public void SearchNext()
        {
            try
            {
                if (this.Page < 10)
                {
                    this.Page++;
                    Search(this.Page);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 前のページへ移動
        /// <summary>
        /// 前のページへ移動
        /// </summary>
        public void SearchPrev()
        {
            try
            {
                if (this.Page > 1)
                {
                    this.Page--;
                    Search(this.Page);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 行ダブルクリック処理
        /// <summary>
        /// 行ダブルクリック処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public void RowDoubleClick(object sender, MouseButtonEventArgs ev)
        {
            try
            {
                var dg = sender as DataGrid;
                if (null != dg.SelectedItem)
                {
                    var ctrl = dg.ItemContainerGenerator.ContainerFromItem(dg.SelectedItem) as DataGridRow;
                    if (null != ctrl)
                    {
                        if (null != ctrl.InputHitTest(ev.GetPosition(ctrl)))
                        {
                            var data = ctrl.DataContext as Repository;

                            if (data != null)
                            {
                                if (((Keyboard.GetKeyStates(Key.LeftCtrl) | Keyboard.GetKeyStates(Key.RightCtrl)) & KeyStates.Down) > 0)
                                {
                                    string url = data.Homepage;

                                    if (!string.IsNullOrEmpty(url))
                                    {
                                        MovingWordpressUtilities.OpenUrl(url);
                                    }
                                }
                                else
                                {
                                    string url = data.HtmlUrl;
                                    MovingWordpressUtilities.OpenUrl(url);
                                }
                            }
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

        #region 保存処理
        /// <summary>
        /// 保存処理
        /// </summary>
        public void Save()
        {
            try
            {
                this.GitHubAPIConfig.Save();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region クリップボードにコピーする
        /// <summary>
        /// クリップボードにコピーする
        /// </summary>
        public void CopyClipBoard()
        {
            try
            {
                Clipboard.SetText(this.Article);
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
