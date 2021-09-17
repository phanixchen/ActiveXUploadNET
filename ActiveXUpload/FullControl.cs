//#define snapupload

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
using CS_Class;

namespace ActiveXUpload
{
    [Guid("9551B223-6188-4387-B293-C7D9D8173E3B")]
    [ProgId("ActiveXUpload.FullControl")]
    [ComVisible(true)]
    public partial class FullControl : UserControl, IObjectSafety
    {
        public FullControl()
        {
            InitializeComponent();

            bUploadDone = false;
            bUploading = false;
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


        private string sql;
        private SQLConn cn = new SQLConn("10.91.21.40", "sa", "sa", "SE");

        private bool bUploadDone;
        private string strUploadPath;
        private string strUserAccount;
        private bool bUploading;
        private Thread t;
        private string[] paths;

        //private Queue q = new Queue();
        private Queue<string> q = new Queue<string>();

        private void FullControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void FullControl_DragDrop(object sender, DragEventArgs e)
        {
            if (bUploading == true)
            {
                label1.Text = "目前檔案傳輸中，請等待完成後再進行其他操作！";
                return;
            }

            //執行上傳
            bUploading = true;
            bUploadDone = false;
            label1.Text = "目前檔案傳輸中，請等待完成後再進行其他操作！";

            paths = (string[])e.Data.GetData(DataFormats.FileDrop);


#if snapupload
            t = new Thread(new ThreadStart(UploadSnap));
#else
            t = new Thread(new ThreadStart(UploadAll));
#endif

            t.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (string folder in Directory.GetDirectories("d:\\"))
            {
                tbPath.Text = tbPath.Text + folder + "\r\n";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            sql = "Select * from model_data";
            cn.Exec(sql, ref dt);

            label1.Text = dt.Rows.Count.ToString();
            tbPath.Text = "UserAccount: " + strUserAccount + "\r\nUploadPath: " + strUploadPath +
                          "\r\nUploadFinish: " + bUploadDone.ToString();
        }

        private void UploadSnap()
        {
            string path;

            foreach (string strpath in paths)
            {
                path = strpath;
                while (path.Substring(path.Length - 1) == "\\")
                {
                    path = path.Substring(0, path.Length - 1);
                }

                if (File.Exists(path + "\\Render\\small.jpg") == true)
                {

                    try
                    {
                        Directory.CreateDirectory(strUploadPath + "\\" + strUserAccount + "\\" + path.Split('\\').GetValue(path.Split('\\').Length - 1) + "\\Render");

                        //copy, and overwrite if exists
                        File.Copy(path + "\\Render\\small.jpg", strUploadPath + "\\" + strUserAccount + "\\" + path.Split('\\').GetValue(path.Split('\\').Length - 1) + "\\Render\\small.jpg", true);


                        //store information into database

                    }
                    catch
                    {
                    }
                }

            }

            bUploadDone = true;
            bUploading = false;

            label1.Text = "檔案傳輸完畢！";

            Thread.CurrentThread.Abort();
        }

        private void UploadAll()
        {
            string path;

            foreach (string strpath in paths)
            {
                path = strpath;
                while (path.Substring(path.Length - 1) == "\\")
                {
                    path = path.Substring(0, path.Length - 1);
                }

                CopyFolder(path, strUploadPath + "\\" + strUserAccount + "\\" + path.Split('\\').GetValue(path.Split('\\').Length - 1), true);

                // store information
            }

            bUploadDone = true;
            bUploading = false;

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

        public void setMsg(string msg)
        {
            label1.Text = msg;
        }

        public string gegMsg()
        {
            return label1.Text;
        }

        public String UserText
        {
            get { return tbPath.Text; }
            set
            {
                //mStr_UserText = value;
                //Update the text box control value also.
                tbPath.Text = value;
            }
        }

        //檢查是否上傳完畢
        public String UploadFinish
        {
            get
            {
                if (bUploadDone == true)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
        }


        //設定Account, using property
        public String UserAccount
        {
            get { return strUserAccount; }
            set
            {
                //mStr_UserText = value;
                //Update the text box control value also.
                strUserAccount = value;
            }
        }

        //設定Account, using function
        public void setUserAccount(string account)
        {
            strUserAccount = account;
        }

        //設定上傳路徑, using property
        public String UploadPath
        {
            get { return strUploadPath; }
            set
            {
                //mStr_UserText = value;
                //Update the text box control value also.
                strUploadPath = value;
            }
        }

        //設定上傳路徑, using function
        public void setUploadPath(string path)
        {
            strUploadPath = path;

            while (strUploadPath.Substring(strUploadPath.Length - 1) == "\\")
            {
                strUploadPath = strUploadPath.Substring(0, strUploadPath.Length - 1);
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


        //增加上傳路徑
        public void addPath(string path)
        {
            q.Enqueue(path);
        }

        //開始上傳先前被設定好的上傳路徑資料
        public void StartUpload()
        {
            if (bUploading == true)
            {
                label1.Text = "目前檔案傳輸中，請等待完成後再進行其他操作！";
                return;
            }

            //執行上傳
            bUploading = true;
            bUploadDone = false;
            label1.Text = "目前檔案傳輸中，請等待完成後再進行其他操作！";

            //paths = (string[])q.ToArray();
            paths = q.ToArray();

            label1.Text = label1.Text + "aa";

#if snapupload
            t = new Thread(new ThreadStart(UploadSnap));
#else
            t = new Thread(new ThreadStart(UploadAll));
#endif

            t.Start();
        }

        //設定 label1
        public void SetCaption(string caption)
        {
            label1.Text = caption;
        }
    }

}
