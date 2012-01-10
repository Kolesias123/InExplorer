using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using InExplorer.Properties;
using InExplorer.Kernel;
using WebKit;

namespace InExplorer
{
    public partial class Explorer : Form
    {
        private Tab WebTab;
        private TabPage TheNewTab = null;
        private WebKitBrowser Browser = null;

        private int LastAddressClick = 0;

        public string Version = Application.ProductVersion;

        public Explorer()
        {
            InitializeComponent();

            NewTab();

            RegisterEvents();
            Tabs.SelectedIndexChanged += new EventHandler(SelectTab);

            GC.Collect();
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process,
        UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

        /* Funciones Generales */

        public void NewTab(string address = "", bool focus = false)
        {
            if (Tabs.TabCount >= 20)
            {
                MessageBox.Show("Ha ocurrido un error fatal.");
                Environment.Exit(1);
                return;
            }

            if(address == "")
                address = "http://www.infosmart.mx/?from=inexplorer";

            RemoveTheNewTab();
            Tab wPage;

            if (Browser != null)
                UnloadEvents();

            wPage = new Tab(address);
            Tabs.TabPages.Add(wPage);

            WebTab = wPage;
            Browser = wPage.Browser;

            RegisterEvents();
            AddTheNewTab();

            if (focus)
                Tabs.SelectTab(WebTab.Name);
        }

        public void AddTheNewTab()
        {
            if (TheNewTab != null)
                return;

            TabPage NT = new TabPage();
            NT.Text = "Agregar pestaña +";
            NT.Name = "NewTab";
            Tabs.TabPages.Add(NT);

            TheNewTab = NT;
        }

        public void RemoveTheNewTab()
        {
            if (TheNewTab == null)
                return;

            Tabs.TabPages.Remove(TheNewTab);
            TheNewTab = null;
        }

        public void SelectTab(object sender, EventArgs e)
        {
            TabPage Me = Tabs.SelectedTab;

            if (Me.Name == "NewTab")
            {
                NewTab("", true);
                return;
            }

            WebTab = (Tab)Me;
            Browser = WebTab.Browser;

            SetUrl();
            this.Text = Browser.DocumentTitle + " - " + Application.ProductName; ;

            Browser.Focus();
        }

        public void RegisterEvents()
        {
            Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(PageLoaded);
            Browser.Navigating += new WebBrowserNavigatingEventHandler(PageNavegate);
            //Browser.Navigated += new WebBrowserNavigatedEventHandler(PageNavigate);
            Browser.DocumentTitleChanged += new EventHandler(PageTitle);
            Browser.Error += new WebKitBrowserErrorEventHandler(PageError);

            Browser.NewWindowRequest += new NewWindowRequestEventHandler(NewWindow);
            //Browser.NewWindowCreated += new NewWindowCreatedEventHandler(CreateWindow);
        }

        public void UnloadEvents()
        {
            Browser.DocumentCompleted -= PageLoaded;
            Browser.Navigating -= PageNavegate;
            Browser.DocumentTitleChanged -= PageTitle;

            Browser.NewWindowRequest -= NewWindow;
        }

        public void SetUrl(string address = "")
        {
            if (Address.Focused)
                return;

            if (address == "" && Browser.Url != null)
                address = Browser.Url.ToString();

            Address.Text = address;
        }

        public void Loading(bool status = true)
        {
            WebTab.UseWaitCursor = status;
            Browser.UseWaitCursor = status;

            Status.Visible = status;

            if (status)
                ToolImage(Resources.stop);
            else
                ToolImage(Resources.rules);
        }

        public void ToolImage(Bitmap i)
        {
            Tool.Image = i;
        }

        /* Eventos - Funciones */

        public void PageLoaded(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            SetUrl();
            Loading(false);
        }

        public void PageNavegate(object sender, WebBrowserNavigatingEventArgs e)
        {
            Loading();
        }

        public void PageTitle(object sender, EventArgs e)
        {
            SetUrl();

            WebTab.Text = Browser.DocumentTitle;
            this.Text = Browser.DocumentTitle + " - " + Application.ProductName;
        }

        public void NewWindow(object sender, NewWindowRequestEventArgs e)
        {
            NewTab(e.Url);
        }

        public void PageError(object sender, WebKitBrowserErrorEventArgs e)
        {
            /*MessageBox.Show("Ha ocurrido un error " + e.Description);*/
        }

        /* Eventos */

        private void Address_Click(object sender, EventArgs e)
        {
            if (Address.ForeColor == Color.FromKnownColor(KnownColor.WindowFrame))
            {
                Address.Text = "";
                Address.ForeColor = Color.Black;
            }
            else if(LastAddressClick < Core.Time())
            {
                Address.Select();
                LastAddressClick = (Core.Time() + 30);
            }
        }

        private void Address_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() != "\r" && e.KeyChar.ToString() != "\n")
                return;

            e.Handled = true;

            string Ad = Address.Text;
            Browser.Navigate(Ad);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (Browser.CanGoBack)
                Browser.GoBack();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (Browser.CanGoForward)
                Browser.GoForward();
        }

        private void Tool_Click(object sender, EventArgs e)
        {
            if (Status.Visible)
            {
                Browser.Stop();
                Loading(false);
            }
            else
                Browser.Reload();
        }
    }
}
