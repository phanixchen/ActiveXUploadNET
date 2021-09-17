
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

//necessary for guid
using System.Runtime.InteropServices;

using System.Threading;

namespace ActiveXUpload
{
    [Guid("9551B223-6188-4387-B293-C7D9D8173E3C")]
    [ProgId("ActiveXDownload.DownloadControl")]
    [ComVisible(true)]
    public partial class DownloadControl : UserControl, IObjectSafety
    {
        public DownloadControl()
        {
            InitializeComponent();

            bDownloadDone = false;
            bDownloading = false;
        }

        #region implement of IObjectSafety
        private const string _IID_IDispatch = "{00020400-0000-0000-C000-000000000046}";
        private const string _IID_IDispatchEx = "{a6ef9860-c720-11d0-9337-00a0c90dcaa9}";
        private const string _IID_IPersistStorage = "{0000010A-0000-0000-C000-000000000046}";
        private const string _IID_IPersistStream = "{00000109-0000-0000-C000-000000000046}";
        private const string _IID_IPersistPropertyBag = "{37D84F60-42CB-11CE-8135-00AA004BB851}";

        private const int INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001;
        private const int INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002;
        private const int S_OK = 0;
        private const int E_FAIL = unchecked((int)0x80004005);
        private const int E_NOINTERFACE = unchecked((int)0x80004002);

        private bool _fSafeForScripting = true;
        private bool _fSafeForInitializing = true;

        public int GetInterfaceSafetyOptions(ref Guid riid,
                             ref int pdwSupportedOptions,
                             ref int pdwEnabledOptions)
        {
            int Rslt = E_FAIL;

            string strGUID = riid.ToString("B");
            pdwSupportedOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA;
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForScripting == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForInitializing == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_DATA;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }

        public int SetInterfaceSafetyOptions(ref Guid riid,
                             int dwOptionSetMask,
                             int dwEnabledOptions)
        {
            int Rslt = E_FAIL;

            string strGUID = riid.ToString("B");
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_CALLER) &&
                         (_fSafeForScripting == true))
                        Rslt = S_OK;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_DATA) &&
                         (_fSafeForInitializing == true))
                        Rslt = S_OK;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }
        #endregion


        
        private bool bDownloadDone;
        private string strDownloadPath;
        private bool bDownloading;
        private Thread t;
        private string[] paths;

        //private Queue q = new Queue();
        private Queue<string> q = new Queue<string>();

        
        private void button2_Click(object sender, EventArgs e)
        {
            tbPath.Text = "DownloadPath: " + strDownloadPath +
                          "\r\nDownloadFinish: " + bDownloadDone.ToString();
        }

        
        private void DownloadAll()
        {
            string path;

            foreach (string strpath in paths)
            {
                path = strpath;
                while (path.Substring(path.Length - 1) == "\\")
                {
                    path = path.Substring(0, path.Length - 1);
                }

                CopyFolder(path, strDownloadPath + "\\" + path.Split('\\').GetValue(path.Split('\\').Length - 1), true);

                // store information
            }

            bDownloadDone = true;
            bDownloading = false;

            label1.Text = "檔案傳輸完畢！";

            Thread.CurrentThread.Abort();
        }

        private void CopyFolder(string sourceFolder, string destFolder, bool bOverWrite)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, bOverWrite);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest, bOverWrite);
            }
        }

        //public void setMsg(string msg)
        //{
        //    label1.Text = msg;
        //}

        //public string gegMsg()
        //{
        //    return label1.Text;
        //}

        //public String UserText
        //{
        //    get { return tbPath.Text; }
        //    set
        //    {
        //        //mStr_UserText = value;
        //        //Update the text box control value also.
        //        tbPath.Text = value;
        //    }
        //}

        //檢查是否下載完畢
        public String DownloadFinish
        {
            get
            {
                if (bDownloadDone == true)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
        }


        ////設定Account, using property
        //public String UserAccount
        //{
        //    get { return strUserAccount; }
        //    set
        //    {
        //        //mStr_UserText = value;
        //        //Update the text box control value also.
        //        strUserAccount = value;
        //    }
        //}

        ////設定Account, using function
        //public void setUserAccount(string account)
        //{
        //    strUserAccount = account;
        //}

        //設定下載路徑, using property
        public String DownloadPath
        {
            get { return strDownloadPath; }
            set
            {
                //mStr_UserText = value;
                //Update the text box control value also.
                strDownloadPath = value;
            }
        }

        //設定下載路徑, using function
        public void setDownloadPath(string path)
        {
            strDownloadPath = path;

            while (strDownloadPath.Substring(strDownloadPath.Length - 1) == "\\")
            {
                strDownloadPath = strDownloadPath.Substring(0, strDownloadPath.Length - 1);
            }
        }

        //設定背景色, using property
        public String bgcolor
        {
            get { return System.Drawing.ColorTranslator.ToHtml(this.BackColor); }
            set
            {
                //mStr_UserText = value;
                //Update the text box control value also.
                this.BackColor = System.Drawing.ColorTranslator.FromHtml(value);
            }
        }


        //增加下載路徑
        public void addPath(string path)
        {
            q.Enqueue(path);
        }

        //開始下載先前被設定好的下載路徑資料
        public void StartDownload()
        {
            if (bDownloading == true)
            {
                label1.Text = "目前檔案傳輸中，請等待完成後再進行其他操作！";
                return;
            }

            //執行下載
            bDownloading = true;
            bDownloadDone = false;
            label1.Text = "目前檔案傳輸中，請等待完成後再進行其他操作！";

            //paths = (string[])q.ToArray();
            paths = q.ToArray();

//            label1.Text = label1.Text + "aa";

            t = new Thread(new ThreadStart(DownloadAll));

            t.Start();
        }

        //設定 label1
        public void SetCaption(string caption)
        {
            label1.Text = caption;
        }
    }

}
