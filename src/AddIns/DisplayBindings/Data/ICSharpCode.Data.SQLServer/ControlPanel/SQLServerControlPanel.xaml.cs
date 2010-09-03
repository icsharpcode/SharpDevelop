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
using ICSharpCode.Data.Core.DatabaseDrivers.SQLServer;

#endregion

namespace ICSharpCode.Data.SQLServer.ControlPanel
{
    /// <summary>
    /// Interaction logic for SQLServerControlPanel.xaml
    /// </summary>
    public partial class SQLServerControlPanel : UserControl
    {
        #region Fields

        private SQLServerDatasource _datasource = null;

        #endregion

        #region Properties

        public SQLServerDatasource Datasource
        {
            get { return _datasource; }
        }

        #endregion

        #region Constructor

        public SQLServerControlPanel(SQLServerDatasource datasource)
        {
            _datasource = datasource;

            InitializeComponent();
        }

        #endregion

        #region Event handler

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _datasource.Password = (sender as PasswordBox).Password;
        }

        #endregion
    }
}
