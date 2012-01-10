using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using InExplorer.Kernel;
using WebKit;

namespace InExplorer
{
    public partial class Tab : TabPage
    {
        public WebKitBrowser Browser;

        public Tab(): this("")
        { }

        public Tab(string Address = "")
        {
            InitializeComponent();

            Browser = new WebKitBrowser();
            Browser.Visible = true;
            Browser.Dock = DockStyle.Fill;
            Browser.Name = Core.Random(15, true, true, false);
            Browser.UserAgent += " 'InExplorer/" + Application.ProductVersion;

            this.Controls.Add(Browser);
            this.Text = "Cargando...";
            this.Name = Core.Random(15, true, true, false);
            this.Cursor = Cursors.Hand;

            if (Address != "")
                Browser.Navigate(Address);
        }
    }
}
