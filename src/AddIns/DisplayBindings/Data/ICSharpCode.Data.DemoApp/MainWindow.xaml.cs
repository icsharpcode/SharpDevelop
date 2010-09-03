// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
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
using ICSharpCode.Data.EDMDesigner.Core.Windows;
using ICSharpCode.Data.EDMDesigner.Core.Windows.EDMWizard;
using System.Data.Sql;
using System.Data;

#endregion

namespace ICSharpCode.Data.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreateNewEDM_Click(object sender, RoutedEventArgs e)
        {
            //EDMWizardWindow edmWizardWindow = new EDMWizardWindow("C:\\TEMP\\test.ssdl");
            //EDMWizardWindow edmWizardWindow = new EDMWizardWindow(new OpenedFile()
            //edmWizardWindow.ShowDialog();
            MessageBox.Show("Test");
            DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
            MessageBox.Show(dt.Rows.Count.ToString());
        }
    }
}
