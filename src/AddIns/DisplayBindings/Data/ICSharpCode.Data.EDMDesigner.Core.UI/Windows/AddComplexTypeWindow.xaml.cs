// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.ComponentModel;
using System.Windows;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Windows
{
    /// <summary>
    /// Interaction logic for AddComplexTypeWindow.xaml
    /// </summary>
    public partial class AddComplexTypeWindow : Window, INotifyPropertyChanged
    {
        public AddComplexTypeWindow()
        {
            InitializeComponent();
        }

        private string _complexTypeName;
        public string ComplexTypeName
        {
            get { return _complexTypeName; }
            set
            {
                _complexTypeName = value;
                OnPropertyChanged("AllowOK");
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
            complexTypeNameTextBox.Focus();
        }

        public bool AllowOk
        {
            get { return ComplexTypeName != null; }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
