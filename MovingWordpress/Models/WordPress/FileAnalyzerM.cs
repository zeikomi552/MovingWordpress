using Microsoft.Win32;
using MovingWordpress.Common;
using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingWordpress.Models
{
    public class FileAnalyzerM : ModelBase
    {
        #region コンテンツのロード処理
        /// <summary>
        /// コンテンツのロード処理
        /// </summary>
        /// <returns>コンテンツ</returns>
        public static List<WpContentsM> LoadContents()
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            List<WpContentsM> ret = new List<WpContentsM>();

            // ファイルの種類を設定
            dialog.Filter = "データベースバックアップファイル (*.sql.gz)|*.sql.gz";
            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                ret = LoadContents(dialog.FileName);
            }

            return ret;
        }

        /// <summary>
        /// コンテンツのロード処理
        /// </summary>
        /// <param name="file_path">ファイルパス</param>
        /// <returns>コンテンツ</returns>
        public static List<WpContentsM> LoadContents(string file_path)
        {
            var query = MovingWordpressUtilities.Decompress(file_path);

            // 取得したクエリを分解しINSERT分のみ取得
            var query_contents = FileAnalyzerM.GetQueryParameters(query);

            List<WpContentsM> ret = new List<WpContentsM>();

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
                    ret.Add(wp_content);
                }
            }

            return ret;
        }
        #endregion

        #region wp_contentsへのINSERT文のパラメータをクエリから抜き出す
        /// <summary>
        /// wp_contentsへのINSERT文のパラメータをクエリから抜き出す
        /// </summary>
        /// <param name="file_path">ファイルパス</param>
        /// <returns>パラメータのリスト</returns>
        public static List<string> GetQueryParameters(string text)
        {
            List<string> contents = new List<string>();

            // 文字列からStringReaderインスタンスを作成
            using (var rs = new System.IO.StringReader(text))
            {
                bool content_query_F = false;

                // ストリームの末端まで繰り返す
                while (rs.Peek() > -1)
                {
                    // 1行読み出し
                    string line = rs.ReadLine();

                    if (content_query_F)
                    {
                        string match_pattern
                            = @"\((\d*?),(\d*?),('\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d'),('\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d'),('.*?'),('.*?'),('.*?'),('.*?'),('.*?'),('.*?'),('.*?'),('.*?'),('.*?'),('.*?'),('\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d'),('\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d'),('.*?'),(.*?),('.*?'),(.*?),('.*?'),('.*?'),(.*?)\)";// (.*?),\d*\)";

                        //Insert文の()内の挿入句をすべて抽出する
                        System.Text.RegularExpressions.MatchCollection mc =
                            System.Text.RegularExpressions.Regex.Matches(
                            line, match_pattern);

                        foreach (var tmp in mc)
                        {
                            contents.Add(tmp.ToString());
                        }

                    }

                    if (line.Contains("LOCK TABLES `wp_posts` WRITE;"))
                    {
                        content_query_F = true;
                    }
                }
            }

            return contents;
        }
        #endregion

        #region パラメータの分解処理
        /// <summary>
        /// パラメータの分解処理
        /// </summary>
        /// <param name="full_text">クエリ全文</param>
        /// <returns>クエリ分解結果</returns>
        public static WpContentsM DivParameters(string full_text)
        {
            WpContentsM ret = new WpContentsM();
            string div_text = full_text = full_text.Trim().Replace("\\r", "").Replace("\\n", "");
            div_text = div_text.Replace("(", "").Replace(")", "");

            ret.FullText = full_text;

            string match_pattern1 = @"([0-9]+?),";
            string match_pattern2 = @"('\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d'),";
            string match_pattern3 = @"('.*?'),";

            //Insert文の()内の挿入句をすべて抽出する
            System.Text.RegularExpressions.MatchCollection mc1 =
                System.Text.RegularExpressions.Regex.Matches(
                full_text, match_pattern1);

            ret.ID = long.Parse(mc1.ElementAt(0).ToString().Replace(",", ""));
            ret.Post_author = long.Parse(mc1.ElementAt(1).ToString().Replace(",", ""));
            ret.Post_parent = long.Parse(mc1.ElementAt(2).ToString().Replace(",", ""));
            ret.Menu_order = int.Parse(mc1.ElementAt(3).ToString().Replace(",", ""));
            //ret.Comment_count = long.Parse(mc1.ElementAt(4).ToString().Replace(")", ""));


            //Insert文の()内の挿入句をすべて抽出する
            System.Text.RegularExpressions.MatchCollection mc2 =
                System.Text.RegularExpressions.Regex.Matches(
                full_text, match_pattern2);

            DateTime tmp_date = DateTime.MinValue;
            DateTime.TryParseExact(mc2.ElementAt(0).ToString().Replace(",", "").Replace("'", ""), "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out tmp_date);
            ret.Post_date = tmp_date;
            DateTime.TryParseExact(mc2.ElementAt(1).ToString().Replace(",", "").Replace("'", ""), "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out tmp_date);
            ret.Post_date_gmt = tmp_date;
            DateTime.TryParseExact(mc2.ElementAt(2).ToString().Replace(",", "").Replace("'", ""), "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out tmp_date);
            ret.Post_modified = tmp_date;
            DateTime.TryParseExact(mc2.ElementAt(3).ToString().Replace(",", "").Replace("'", ""), "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out tmp_date);
            ret.Post_modified_gmt = tmp_date;

            full_text = full_text.Replace(mc1.ElementAt(0).ToString(), "");
            full_text = full_text.Replace(mc1.ElementAt(1).ToString(), "");
            full_text = full_text.Replace(mc1.ElementAt(2).ToString(), "");
            full_text = full_text.Replace(mc1.ElementAt(3).ToString(), "");
            //full_text = full_text.Replace(mc1.ElementAt(4).ToString(), "");

            full_text = full_text.Replace(mc2.ElementAt(0).ToString(), "");
            full_text = full_text.Replace(mc2.ElementAt(1).ToString(), "");
            full_text = full_text.Replace(mc2.ElementAt(2).ToString(), "");
            full_text = full_text.Replace(mc2.ElementAt(3).ToString(), "");


            //Insert文の()内の挿入句をすべて抽出する
            System.Text.RegularExpressions.MatchCollection mc3 =
                System.Text.RegularExpressions.Regex.Matches(
                full_text, match_pattern3);

            int index = mc3.Count - 1;
            ret.Post_mime_type = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Post_type = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Guid = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Post_content_filtered = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Pinged = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.To_ping = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Post_name = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Post_password = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Ping_status = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Comment_status = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Post_status = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Post_excerpt = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");
            ret.Post_title = mc3.ElementAt(index--).ToString().Replace(",", "").Replace("'", "");

            for (int index_tmp = index; index_tmp >= 0; index_tmp--)
            {
                ret.Post_content += mc3.ElementAt(index_tmp).ToString().Replace(",", "").Replace("'", "");
            }

            return ret;
        }
        #endregion

        #region Htmlを取り除く処理
        /// <summary>
        /// Htmlを取り除く処理
        /// </summary>
        /// <param name="text">記事内容</param>
        /// <returns>htmlタグを取り除いた結果</returns>
        public static string ExceptHtmlTags(string text)
        {
            string match_pattern = "<(\".*?\"|'.*?'|[^'\"])*?>";
            //Insert文の()内の挿入句をすべて抽出する
            System.Text.RegularExpressions.MatchCollection mc =
                System.Text.RegularExpressions.Regex.Matches(
                text, match_pattern);

            string tmp_txt = text;
            foreach (var html_tag in mc)
            {
                tmp_txt = tmp_txt.Replace(html_tag.ToString(), "");
            }

            return tmp_txt;
        }
        #endregion
    }
}
