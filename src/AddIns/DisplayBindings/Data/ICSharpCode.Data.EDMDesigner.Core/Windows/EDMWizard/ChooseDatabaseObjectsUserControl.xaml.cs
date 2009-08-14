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

namespace ICSharpCode.Data.EDMDesigner.Core.Windows.EDMWizard
{
    /// <summary>
    /// Interaction logic for ChooseDatabaseObjectsUserControl.xaml
    /// </summary>
    public partial class ChooseDatabaseObjectsUserControl : WizardUserControl
    {
        public override int Index
        {
            get
            {
                return 1;
            }
        }

        public override bool CanFinish
        {
            get
            {
                return true;
            }
        }

        public override string Title
        {
            get
            {
                return "Choose database objects";
            }
        }
        
        public ChooseDatabaseObjectsUserControl()
        {
            InitializeComponent();
        }

        public override void OnActivate()
        {
            EDMWizardWindow edmWizardWindow = WizardWindow as EDMWizardWindow;
            if (edmWizardWindow.SelectedDatabase != null)
            {
                edmWizardWindow.SelectedDatabase.LoadDatabase();
                IsReadyForNextStep = true;
            }
        }
    }
}
