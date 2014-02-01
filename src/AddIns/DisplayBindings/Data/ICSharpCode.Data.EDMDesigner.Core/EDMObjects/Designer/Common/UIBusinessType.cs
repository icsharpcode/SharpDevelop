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

        public override string Name
        {
            get { return BusinessInstance.Name; }
            set { BusinessInstance.Name = value; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
