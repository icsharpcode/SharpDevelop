// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.ComponentModel;
using System.Windows;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Windows
{
    public partial class AddScalarPropertyWindow : Window, INotifyPropertyChanged
    {
        public AddScalarPropertyWindow()
        {
            InitializeComponent();
            typeComboBox.ItemsSource = Enum.GetValues(typeof(PropertyType));
        }

        private string _propertyName;
        public string PropertyName 
        {
            get { return _propertyName; }
            set
            {
                _propertyName = value;
                OnPropertyChanged("AllowOk");
            }
        }
        private PropertyType? _propertyType;
        public PropertyType? PropertyType 
        {
            get { return _propertyType; }
            set
            {
                _propertyType = value;
                OnPropertyChanged("AllowOk");
            }
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
            nameTextBox.Focus();
        }

        public bool AllowOk
        {
            get { return ! (PropertyName == null || PropertyType == null); }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
