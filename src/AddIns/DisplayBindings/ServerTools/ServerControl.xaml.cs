using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.OleDb;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;

using ICSharpCode.DataTools;

namespace ICSharpCode.ServerTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ServerControl : UserControl
    {
        public ServerControl()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            ConnectionStringSettingsCollection c
            = OleDbConnectionUtil.GetConnectionSettingsCollection();
            this.dbTree.Items.Clear();
            foreach (ConnectionStringSettings s in c)
            {
                TreeViewItem n = new TreeViewItem();
                //n.Name = s.Name;
                n.Header = s.Name;
                this.dbTree.Items.Add(n);                
            }
        }

        /// <summary>
        /// Uses the DataLink dialog to define a new Oledb connection string,
        /// tests it, and adds it to the exe config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            MSDASC.DataLinks mydlg = new MSDASC.DataLinks();
            OleDbConnection oleCon = new OleDbConnection();
            ADODB._Connection aDOcon;

            //Cast the generic object that PromptNew returns to an ADODB._Connection.
            aDOcon = (ADODB._Connection)mydlg.PromptNew();

            oleCon.ConnectionString = aDOcon.ConnectionString;
            oleCon.Open();

            if (oleCon.State.ToString() == "Open")
            {
                // TODO: add the oledb connection to the local config and
                // refresh
                OleDbConnectionUtil.Put("any name", oleCon.ConnectionString);
                this.Refresh();
                oleCon.Close();
            }
            else
            {
                MessageBox.Show("Connection Failed");
            }
        }
    }
}
