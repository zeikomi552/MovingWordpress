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
        public void Init()
        {
            try
            {

            }
            catch (Exception e)
            {
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }
        }

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
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }

        }

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
                ShowMessage.ShowErrorOK(e.Message, "Error");
            }

        }

    }
}
