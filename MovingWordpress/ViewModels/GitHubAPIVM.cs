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
                this.SearchDateRange.FromDateBase = DateTime.Today.AddMonths(-3);
                this.SearchDateRange.ToDateBase = null; // 終了日は無制限


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
        }


        /// <summary>
        /// リポジトリ検索処理
        /// </summary>
        public async void Search()
        {
            var client = new GitHubClient(new ProductHeaderValue(this.GitHubAPIConfig.ProductHeader));

            var tokenAuth = new Credentials(this.GitHubAPIConfig.AccessToken); // NOTE: not real token
            client.Credentials = tokenAuth;

            SearchRepositoriesRequest request = new SearchRepositoriesRequest();
#pragma warning disable CS0618 // 型またはメンバーが旧型式です

            // 値を持っているかどうかのチェック
            if (this.SearchDateRange.HasValue)
            {
                request.Created = new DateRange(this.SearchDateRange.FromDate, this.SearchDateRange.ToDate);
            }

            // スターの数
            request.Stars = new Octokit.Range(1,int.MaxValue);

            // スターの数でソート
            request.SortField = RepoSearchSort.Stars;

            request.Language = this.LanguageList.SelectedItem.UseLanguage;

            // 降順でソート
            request.Order = SortDirection.Descending;
#pragma warning restore CS0618 // 型またはメンバーが旧型式です

            var repostitories = await client.Search.SearchRepo(request);

            // 記事の作成
            this.Article = RepositorySearchResultM.GetArticle(this.SearchDateRange, request.Language, repostitories);
        }

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
