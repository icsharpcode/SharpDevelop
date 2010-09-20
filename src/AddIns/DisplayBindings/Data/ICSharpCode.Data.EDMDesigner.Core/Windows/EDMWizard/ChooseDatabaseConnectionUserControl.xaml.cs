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
using ICSharpCode.Data.Core.UI.UserControls;
using ICSharpCode.Data.Core.UI.Windows;
using ICSharpCode.Data.Core.Interfaces;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.Windows.EDMWizard
{
    /// <summary>
    /// Interaction logic for ChooseDatabaseConnectionUserControl.xaml
    /// </summary>
    public partial class ChooseDatabaseConnectionUserControl : WizardUserControl
    {
        public ChooseDatabaseConnectionUserControl()
        {
            InitializeComponent();
        }

        public override int Index
        {
            get
            {
                return 0;
            }
        }

        public override bool CanFinish
        {
            get
            {
                return false;
            }
        }

        public override string Title
        {
            get
            {
                return "Choose database connection";
            }
        }

        private void btnNewConnection_Click(object sender, RoutedEventArgs e)
        {
            ConnectionWizardWindow connectionWizardWindow = new ConnectionWizardWindow();
            connectionWizardWindow.Owner = WizardWindow;
            connectionWizardWindow.ShowDialog();

            if (connectionWizardWindow.DialogResult.HasValue && connectionWizardWindow.DialogResult.Value)
            {
                EDMWizardWindow edmWizardWindow = WizardWindow as EDMWizardWindow;
                edmWizardWindow.Databases.Add(connectionWizardWindow.SelectedDatabase);
                edmWizardWindow.SelectedDatabase = connectionWizardWindow.SelectedDatabase;
            }
        }

        private void cboDatabases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0 && e.AddedItems[0] is IDatabase)
                IsReadyForNextStep = true;
        }
    }
}
