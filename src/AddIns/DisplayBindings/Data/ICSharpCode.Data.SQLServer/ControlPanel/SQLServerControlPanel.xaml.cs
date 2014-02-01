// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
