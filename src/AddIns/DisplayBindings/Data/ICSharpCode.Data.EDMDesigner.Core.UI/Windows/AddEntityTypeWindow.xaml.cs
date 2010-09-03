// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.ComponentModel;
using System.Windows;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Windows
{
    public partial class AddEntityTypeWindow : Window, INotifyPropertyChanged
    {
        private NullValue _nullValue;

        public AddEntityTypeWindow(CSDLView view)
        {
            InitializeComponent();
            var nullValues = NullValue.GetValues("(none)");
            _nullValue = nullValues[0];
            _baseEntityTypeObj = _nullValue;
            nullEntityTypeCollection.Collection = nullValues;
            entityTypeCollection.Collection = view.EntityTypes;
        }

        private string _entityTypeName;
        public string EntityTypeName
        {
            get { return _entityTypeName; }
            set
            {
                _entityTypeName = value;
                OnPropertyChanged("AllowOK");
            }
        }

        private UIEntityType _baseEntityType;
        private object _baseEntityTypeObj;
        public object BaseEntityTypeObj
        {
            get
            {
                if (_baseEntityType == null)
                    return _nullValue;
                return _baseEntityType;
            }
            set
            {
                _baseEntityTypeObj = value;
                var uiEntityType = value as UIEntityType;
                if (uiEntityType != null)
                    _baseEntityType = uiEntityType;
                else
                    _baseEntityType = null;
                OnPropertyChanged("AllowOK");
            }
        }
        public UIEntityType BaseEntityType
        {
            get { return _baseEntityType; }
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
            entityTypeNameTextBox.Focus();
        }

        public bool AllowOk
        {
            get { return !(EntityTypeName == null || _baseEntityTypeObj == null); }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
