using MovingWordpress.Models.GitHub;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Init()
        {
            try
            {
                this.GitHubAPIConfig.Load();
            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

        /// <summary>
        /// リポジトリ検索処理
        /// </summary>
        public async void Search()
        {
            //var github = new GitHubClient(new ProductHeaderValue("MyAmazingApp"));

            var client = new GitHubClient(new ProductHeaderValue(this.GitHubAPIConfig.ProductHeader));

            var tokenAuth = new Credentials(this.GitHubAPIConfig.AccessToken); // NOTE: not real token
            client.Credentials = tokenAuth;

            //github.Search.SearchRepo()

            SearchRepositoriesRequest request = new SearchRepositoriesRequest();
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            request.Created = new DateRange(DateTime.Today.AddMonths(-1), DateTime.Today);
            request.Stars = new Octokit.Range(1,int.MaxValue);
            request.SortField = RepoSearchSort.Stars;
            request.Order = SortDirection.Descending;
#pragma warning restore CS0618 // 型またはメンバーが旧型式です

            var repostitories = await client.Search.SearchRepo(request);
        }
    }
}
