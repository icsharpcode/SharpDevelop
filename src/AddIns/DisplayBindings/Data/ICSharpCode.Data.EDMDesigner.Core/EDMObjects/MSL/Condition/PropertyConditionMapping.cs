#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition
{
    public class PropertyConditionMapping : ConditionMapping
    {
        private CSDL.Property.ScalarProperty _csdlProperty;
        public CSDL.Property.ScalarProperty CSDLProperty
        {
            get { return _csdlProperty; }
            set
            {
                _csdlProperty = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged();
        }
        public event Action PropertyChanged;
    }
}
