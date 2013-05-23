// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using ICSharpCode.Core;
using ICSharpCode.Data.Addin.Pad;
using ICSharpCode.Data.Core.UI.Windows;
using ICSharpCode.SharpDevelop;

#endregion

namespace ICSharpCode.Data.Addin.Commands
{
    public class AddDatabaseCommand : AbstractCommand
    {
        public override void Run()
        {
            ConnectionWizardWindow connectionWizardWindow = new ConnectionWizardWindow();
            connectionWizardWindow.Owner = SD.Workbench.MainWindow;

            connectionWizardWindow.AddAction = new Action(delegate()
            {
                if (connectionWizardWindow.SelectedDatabase.LoadDatabase())
                {
                    DatabasesTreeViewPad.Instance.Databases.Add(connectionWizardWindow.SelectedDatabase);
                    connectionWizardWindow.Close();
                }
            });

            connectionWizardWindow.ShowDialog();
        }
    }

    public class RemoveDatabaseCommand : AbstractCommand 
    {
        public override void Run()
        {

        }
    }
}
