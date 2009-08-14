#region Usings

using System.ComponentModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common
{
    public abstract class UIBusinessType<BusinessType> : EDMObjectBase, INotifyPropertyChanged where BusinessType : EDMObjectBase, INotifyPropertyChanged
    {
        protected UIBusinessType(BusinessType businessInstance)
        {
            BusinessInstance = businessInstance;
            businessInstance.PropertyChanged += BusinessInstancePropertyChanged;
        }

        protected virtual void BusinessInstancePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    OnPropertyChanged("Name");
                    break;
            }
        }

        public BusinessType BusinessInstance { get; private set; }

        public virtual string Name
        {
            get { return BusinessInstance.Name; }
            set { BusinessInstance.Name = value; }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
    }
}
