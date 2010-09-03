// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.ComponentModel;
using System.Windows;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Windows
{
    public partial class AddAssociationWindow : Window, INotifyPropertyChanged
    {
        public AddAssociationWindow()
        {
            InitializeComponent();
        }

        private string _associationName;
        public string AssociationName
        {
            get { return _associationName; }
            set
            {
                _associationName = value;
                OnPropertyChanged("AllowOk");
            }
        }

        public string NavigationProperty1Name
        {
            get { return navigationProperty1.NavigationPropertyName; }
        }
        public UIEntityType NavigationProperty1EntityType
        {
            get { return navigationProperty1.EntityType; }
            set { navigationProperty1.EntityType = value; }
        }
        public Cardinality? NavigationProperty1Cardinality
        {
            get { return navigationProperty1.Cardinality == null ? (Cardinality?)null : navigationProperty1.Cardinality.Value; }
        }

        public string NavigationProperty2Name
        {
            get { return navigationProperty2.NavigationPropertyName; }
        }
        public UIEntityType NavigationProperty2EntityType
        {
            get { return navigationProperty2.EntityType; }
        }
        public Cardinality? NavigationProperty2Cardinality
        {
            get { return navigationProperty2.Cardinality == null ? (Cardinality?)null : navigationProperty2.Cardinality.Value; }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            associationNameTextBox.Focus();
        }

        private void NavigationProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AllowOk")
                OnPropertyChanged("AllowOk");
        }

        public bool AllowOk
        {
            get { return AssociationName != null && navigationProperty1.AllowOk && navigationProperty2.AllowOk; }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
