using log4net.Config;
using MovingWordpress.Models;
using MovingWordpress.Views;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MovingWordpress.ViewModels
{

    public class MainWindowVM : ViewModelBase
    {
        #region 初期化処理
        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Init()
        {
            try
            {
                // log4netの設定ファイル読み込み処理
                XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }
        #endregion

        #region 荷づくり画面への遷移処理
        /// <summary>
        /// 荷づくり画面への遷移処理
        /// </summary>
        public void BeforeWordpressV()
        {
            try
            {
                var wnd = new BeforeWordpressV();

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
        #endregion

        #region 引っ越し画面への遷移処理
        /// <summary>
        /// 引っ越し画面への遷移処理
        /// </summary>
        public void AfterWordpressV()
        {
            try
            {
                var wnd = new AfterWordpressV();

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
        #endregion

        #region 引っ越し画面への遷移処理
        /// <summary>
        /// 引っ越し画面への遷移処理
        /// </summary>
        public void AnalizeBacknumberV()
        {
            try
            {
                var wnd = new AnalizeBacknumberV();

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
        #endregion
    }
}
