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
