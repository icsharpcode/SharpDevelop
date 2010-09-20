// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
