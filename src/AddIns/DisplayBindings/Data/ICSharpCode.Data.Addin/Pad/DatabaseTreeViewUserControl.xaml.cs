// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.Addin.Commands;

#endregion

namespace ICSharpCode.Data.Addin.Pad
{
    /// <summary>
    /// Interaction logic for DatabaseTreeViewUserControl.xaml
    /// </summary>
    public partial class DatabaseTreeViewUserControl : UserControl
    {
        #region Properties

        public new DockPanel Content
        {
            get { return dockPanel; }
        }

        #endregion

        #region Constructor

        public DatabaseTreeViewUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void btnAddConnection_Click(object sender, RoutedEventArgs e)
        {
            AddDatabaseCommand addDatabaseCommand = new AddDatabaseCommand();
            addDatabaseCommand.Owner = this;
            addDatabaseCommand.Run();
        }

        #endregion
    }
}
