#region Usings

using System;
using System.Windows.Interop;
using ICSharpCode.Core;
using ICSharpCode.Data.Addin.Pad;
using ICSharpCode.Data.Core.UI.Windows;
using ICSharpCode.SharpDevelop.Gui;

#endregion

namespace ICSharpCode.Data.Addin.Commands
{
    public class AddDatabaseCommand : AbstractCommand
    {
        public override void Run()
        {
            ConnectionWizardWindow connectionWizardWindow = new ConnectionWizardWindow();
            connectionWizardWindow.Owner = WorkbenchSingleton.MainWindow;

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
