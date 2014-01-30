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
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Windows
{
    public partial class AddAssociationEnd : UserControl, INotifyPropertyChanged
    {
        public AddAssociationEnd()
        {
            InitializeComponent();
        }

        private string _navigationPropertyName;
        public string NavigationPropertyName
        {
            get { return _navigationPropertyName; }
            set
            {
                _navigationPropertyName = value;
                OnPropertyChanged("AllowOk");
            }
        }

        private UIEntityType _entityType;
        public UIEntityType EntityType
        {
            get { return _entityType; }
            set
            {
                _entityType = value;
                OnPropertyChanged("AllowOk");
            }
        }

        private CardinalityItem _cardinality;
        public CardinalityItem Cardinality
        {
            get { return _cardinality; }
            set
            {
                _cardinality = value;
                OnPropertyChanged("AllowOk");
            }
        }

        public bool AllowOk
        {
            get { return !(EntityType == null || Cardinality == null || NavigationPropertyName == null); }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
