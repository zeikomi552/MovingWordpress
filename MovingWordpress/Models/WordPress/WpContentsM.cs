using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MovingWordpress.Models
{
    public class WpContentsM : ModelBase
    {
        #region Insert文の1要素分の挿入句の全文[FullText]プロパティ
        /// <summary>
        /// Insert文の1要素分の挿入句の全文[FullText]プロパティ用変数
        /// </summary>
        string _FullText = string.Empty;
        /// <summary>
        /// Insert文の1要素分の挿入句の全文[FullText]プロパティ
        /// </summary>
        public string FullText
        {
            get
            {
                return _FullText;
            }
            set
            {
                if (!_FullText.Equals(value))
                {
                    _FullText = value;
                    NotifyPropertyChanged("FullText");
                }
            }
        }
        #endregion

        #region [ID]プロパティ
        /// <summary>
        /// [ID]プロパティ用変数
        /// </summary>
        Int64 _ID = 0;
        /// <summary>
        /// [ID]プロパティ
        /// </summary>
        public Int64 ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (!_ID.Equals(value))
                {
                    _ID = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }
        #endregion

        #region [Post_author]プロパティ
        /// <summary>
        /// [Post_author]プロパティ用変数
        /// </summary>
        Int64 _Post_author = 0;
        /// <summary>
        /// [Post_author]プロパティ
        /// </summary>
        public Int64 Post_author
        {
            get
            {
                return _Post_author;
            }
            set
            {
                if (!_Post_author.Equals(value))
                {
                    _Post_author = value;
                    NotifyPropertyChanged("Post_author");
                }
            }
        }
        #endregion

        #region [Post_date]プロパティ
        /// <summary>
        /// [Post_date]プロパティ用変数
        /// </summary>
        DateTime _Post_date = DateTime.MinValue;
        /// <summary>
        /// [Post_date]プロパティ
        /// </summary>
        public DateTime Post_date
        {
            get
            {
                return _Post_date;
            }
            set
            {
                if (!_Post_date.Equals(value))
                {
                    _Post_date = value;
                    NotifyPropertyChanged("Post_date");
                }
            }
        }
        #endregion

        #region [Post_date_gmt]プロパティ
        /// <summary>
        /// [Post_date_gmt]プロパティ用変数
        /// </summary>
        DateTime _Post_date_gmt = DateTime.MinValue;
        /// <summary>
        /// [Post_date_gmt]プロパティ
        /// </summary>
        public DateTime Post_date_gmt
        {
            get
            {
                return _Post_date_gmt;
            }
            set
            {
                if (!_Post_date_gmt.Equals(value))
                {
                    _Post_date_gmt = value;
                    NotifyPropertyChanged("Post_date_gmt");
                }
            }
        }
        #endregion

        #region [Post_content]プロパティ
        /// <summary>
        /// [Post_content]プロパティ用変数
        /// </summary>
        string _Post_content = string.Empty;
        /// <summary>
        /// [Post_content]プロパティ
        /// </summary>
        public string Post_content
        {
            get
            {
                return _Post_content;
            }
            set
            {
                if (!_Post_content.Equals(value))
                {
                    _Post_content = value;
                    NotifyPropertyChanged("Post_content");
                }
            }
        }
        #endregion

        #region [Post_title]プロパティ
        /// <summary>
        /// [Post_title]プロパティ用変数
        /// </summary>
        string _Post_title = string.Empty;
        /// <summary>
        /// [Post_title]プロパティ
        /// </summary>
        public string Post_title
        {
            get
            {
                return _Post_title;
            }
            set
            {
                if (!_Post_title.Equals(value))
                {
                    _Post_title = value;
                    NotifyPropertyChanged("Post_title");
                }
            }
        }
        #endregion

        #region [Post_excerpt]プロパティ
        /// <summary>
        /// [Post_excerpt]プロパティ用変数
        /// </summary>
        string _Post_excerpt = string.Empty;
        /// <summary>
        /// [Post_excerpt]プロパティ
        /// </summary>
        public string Post_excerpt
        {
            get
            {
                return _Post_excerpt;
            }
            set
            {
                if (!_Post_excerpt.Equals(value))
                {
                    _Post_excerpt = value;
                    NotifyPropertyChanged("Post_excerpt");
                }
            }
        }
        #endregion

        #region [Post_status]プロパティ
        /// <summary>
        /// [Post_status]プロパティ用変数
        /// </summary>
        string _Post_status = string.Empty;
        /// <summary>
        /// [Post_status]プロパティ
        /// </summary>
        public string Post_status
        {
            get
            {
                return _Post_status;
            }
            set
            {
                if (!_Post_status.Equals(value))
                {
                    _Post_status = value;
                    NotifyPropertyChanged("Post_status");
                }
            }
        }
        #endregion

        #region [Comment_status]プロパティ
        /// <summary>
        /// [Comment_status]プロパティ用変数
        /// </summary>
        string _Comment_status = string.Empty;
        /// <summary>
        /// [Comment_status]プロパティ
        /// </summary>
        public string Comment_status
        {
            get
            {
                return _Comment_status;
            }
            set
            {
                if (!_Comment_status.Equals(value))
                {
                    _Comment_status = value;
                    NotifyPropertyChanged("Comment_status");
                }
            }
        }
        #endregion

        #region [Ping_status]プロパティ
        /// <summary>
        /// [Ping_status]プロパティ用変数
        /// </summary>
        string _Ping_status = string.Empty;
        /// <summary>
        /// [Ping_status]プロパティ
        /// </summary>
        public string Ping_status
        {
            get
            {
                return _Ping_status;
            }
            set
            {
                if (!_Ping_status.Equals(value))
                {
                    _Ping_status = value;
                    NotifyPropertyChanged("Ping_status");
                }
            }
        }
        #endregion

        #region [Post_password]プロパティ
        /// <summary>
        /// [Post_password]プロパティ用変数
        /// </summary>
        string _Post_password = string.Empty;
        /// <summary>
        /// [Post_password]プロパティ
        /// </summary>
        public string Post_password
        {
            get
            {
                return _Post_password;
            }
            set
            {
                if (!_Post_password.Equals(value))
                {
                    _Post_password = value;
                    NotifyPropertyChanged("Post_password");
                }
            }
        }
        #endregion

        #region [Post_name]プロパティ
        /// <summary>
        /// [Post_name]プロパティ用変数
        /// </summary>
        string _Post_name = string.Empty;
        /// <summary>
        /// [Post_name]プロパティ
        /// </summary>
        public string Post_name
        {
            get
            {
                return _Post_name;
            }
            set
            {
                if (!_Post_name.Equals(value))
                {
                    _Post_name = value;
                    NotifyPropertyChanged("Post_name");
                }
            }
        }
        #endregion

        #region [To_ping]プロパティ
        /// <summary>
        /// [To_ping]プロパティ用変数
        /// </summary>
        string _To_ping = string.Empty;
        /// <summary>
        /// [To_ping]プロパティ
        /// </summary>
        public string To_ping
        {
            get
            {
                return _To_ping;
            }
            set
            {
                if (!_To_ping.Equals(value))
                {
                    _To_ping = value;
                    NotifyPropertyChanged("To_ping");
                }
            }
        }
        #endregion

        #region [Pinged]プロパティ
        /// <summary>
        /// [Pinged]プロパティ用変数
        /// </summary>
        string _Pinged = string.Empty;
        /// <summary>
        /// [Pinged]プロパティ
        /// </summary>
        public string Pinged
        {
            get
            {
                return _Pinged;
            }
            set
            {
                if (!_Pinged.Equals(value))
                {
                    _Pinged = value;
                    NotifyPropertyChanged("Pinged");
                }
            }
        }
        #endregion

        #region [Post_modified]プロパティ
        /// <summary>
        /// [Post_modified]プロパティ用変数
        /// </summary>
        DateTime _Post_modified = DateTime.MinValue;
        /// <summary>
        /// [Post_modified]プロパティ
        /// </summary>
        public DateTime Post_modified
        {
            get
            {
                return _Post_modified;
            }
            set
            {
                if (!_Post_modified.Equals(value))
                {
                    _Post_modified = value;
                    NotifyPropertyChanged("Post_modified");
                }
            }
        }
        #endregion

        #region [Post_modified_gmt]プロパティ
        /// <summary>
        /// [Post_modified_gmt]プロパティ用変数
        /// </summary>
        DateTime _Post_modified_gmt = DateTime.MinValue;
        /// <summary>
        /// [Post_modified_gmt]プロパティ
        /// </summary>
        public DateTime Post_modified_gmt
        {
            get
            {
                return _Post_modified_gmt;
            }
            set
            {
                if (!_Post_modified_gmt.Equals(value))
                {
                    _Post_modified_gmt = value;
                    NotifyPropertyChanged("Post_modified_gmt");
                }
            }
        }
        #endregion

        #region [Post_content_filtered]プロパティ
        /// <summary>
        /// [Post_content_filtered]プロパティ用変数
        /// </summary>
        string _Post_content_filtered = string.Empty;
        /// <summary>
        /// [Post_content_filtered]プロパティ
        /// </summary>
        public string Post_content_filtered
        {
            get
            {
                return _Post_content_filtered;
            }
            set
            {
                if (!_Post_content_filtered.Equals(value))
                {
                    _Post_content_filtered = value;
                    NotifyPropertyChanged("Post_content_filtered");
                }
            }
        }
        #endregion

        #region [Post_parent]プロパティ
        /// <summary>
        /// [Post_parent]プロパティ用変数
        /// </summary>
        Int64 _Post_parent = 0;
        /// <summary>
        /// [Post_parent]プロパティ
        /// </summary>
        public Int64 Post_parent
        {
            get
            {
                return _Post_parent;
            }
            set
            {
                if (!_Post_parent.Equals(value))
                {
                    _Post_parent = value;
                    NotifyPropertyChanged("Post_parent");
                }
            }
        }
        #endregion

        #region [Guid]プロパティ
        /// <summary>
        /// [Guid]プロパティ用変数
        /// </summary>
        string _Guid = string.Empty;
        /// <summary>
        /// [Guid]プロパティ
        /// </summary>
        public string Guid
        {
            get
            {
                return _Guid;
            }
            set
            {
                if (!_Guid.Equals(value))
                {
                    _Guid = value;
                    NotifyPropertyChanged("Guid");
                }
            }
        }
        #endregion

        #region [Menu_order]プロパティ
        /// <summary>
        /// [Menu_order]プロパティ用変数
        /// </summary>
        int _Menu_order = 0;
        /// <summary>
        /// [Menu_order]プロパティ
        /// </summary>
        public int Menu_order
        {
            get
            {
                return _Menu_order;
            }
            set
            {
                if (!_Menu_order.Equals(value))
                {
                    _Menu_order = value;
                    NotifyPropertyChanged("Menu_order");
                }
            }
        }
        #endregion

        #region [Post_type]プロパティ
        /// <summary>
        /// [Post_type]プロパティ用変数
        /// </summary>
        string _Post_type = string.Empty;
        /// <summary>
        /// [Post_type]プロパティ
        /// </summary>
        public string Post_type
        {
            get
            {
                return _Post_type;
            }
            set
            {
                if (!_Post_type.Equals(value))
                {
                    _Post_type = value;
                    NotifyPropertyChanged("Post_type");
                }
            }
        }
        #endregion

        #region [Post_mime_type]プロパティ
        /// <summary>
        /// [Post_mime_type]プロパティ用変数
        /// </summary>
        string _Post_mime_type = string.Empty;
        /// <summary>
        /// [Post_mime_type]プロパティ
        /// </summary>
        public string Post_mime_type
        {
            get
            {
                return _Post_mime_type;
            }
            set
            {
                if (!_Post_mime_type.Equals(value))
                {
                    _Post_mime_type = value;
                    NotifyPropertyChanged("Post_mime_type");
                }
            }
        }
        #endregion

        #region 。を改行に変更しHTMLタグを取り除いたもの
        /// <summary>
        /// 。を改行に変更しHTMLタグを取り除いたもの
        /// </summary>
        public string Post_content_Except
        {
            get
            {
                return FileAnalyzerM.ExceptHtmlTags(this.Post_content).Replace("。", "。\r\n");
            }
        }
        #endregion

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


    }
}
