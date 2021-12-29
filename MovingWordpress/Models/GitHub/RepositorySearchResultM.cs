using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovingWordpress.Common.Extensions;

namespace MovingWordpress.Models.GitHub
{
    public class RepositorySearchResultM : ModelBase
    {

        private static string GetRangeDateText(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("yyyy/MM/dd") : "指定なし";
        }

        /// <summary>
        /// 記事
        /// </summary>
        /// <returns>記事</returns>
        public static string GetArticle(DateRangeM search_range, Language? search_language, SearchRepositoryResult repogitories)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine($"## GitHubサーベイ 調査日{DateTime.Today.ToString("yyyy/MM/dd")}");

            text.AppendLine($"### 検索条件");

            string start = search_range.FromDateBase.HasValue
                                ? search_range.FromDate.ToString("yyyy/MM/dd") : "指定なし";
            text.AppendLine($"- リポジトリ作成日 {GetRangeDateText(search_range.FromDateBase)} - {GetRangeDateText(search_range.ToDateBase)}");

            // 言語条件
            if (search_language.HasValue)
            {
                text.AppendLine($"- 開発言語 {search_language}");
            }

            text.AppendLine($"- ソート順：スター獲得数順");
            text.AppendLine();

            text.AppendLine($"### 検索結果");
            text.AppendLine($"|スター<br>(順位)|リポジトリ名<br>説明|使用言語|検索|");
            text.AppendLine($"|---|---|---|---|");
            int rank = 1;
            foreach (var repo in repogitories.Items)
            {
                string description = repo.Description.EmptyToText("-").CutText(50).Replace("|", "\\/");
                string language = repo.Language.EmptyToText("-").CutText(20);

                string homepage_url = !string.IsNullOrWhiteSpace(repo.Homepage) 
                    && (repo.Homepage.ToLower().Contains("http://") || repo.Homepage.ToLower().Contains("https://"))
                    ? $" [[Home Page]({repo.Homepage})]" : string.Empty;

                // 行情報の作成
                text.AppendLine($"|<center>{repo.StargazersCount}<br>({rank++}位)</center>|" +
                    $"[{repo.FullName}]({repo.HtmlUrl}){homepage_url}<br>{description}|" +
                    $"{language}|" +
                    $"[[google](https://www.google.com/search?q={repo.Name})] " +
                    $"[[Qiita](https://qiita.com/search?q={repo.Name})]|");
            }

            return text.ToString();
        }
    }
}
