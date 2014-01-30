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
