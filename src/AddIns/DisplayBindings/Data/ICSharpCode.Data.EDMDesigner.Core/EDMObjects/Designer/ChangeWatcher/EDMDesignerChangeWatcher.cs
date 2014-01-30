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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.ChangeWatcher
{
    public class EDMDesignerChangeWatcher
    {
        #region Field

        private static List<IEDMDesignerChangeWatcherObserver> _observers = new List<IEDMDesignerChangeWatcherObserver>();
        private static bool _init = true;

        #endregion

        #region Properties

        public static bool Init
        {
            get { return EDMDesignerChangeWatcher._init; }
            set { EDMDesignerChangeWatcher._init = value; }
        }

        #endregion

        #region Methods

        public static void AddEDMDesignerViewContent(IEDMDesignerChangeWatcherObserver observer)
        {
            _observers.Add(observer);
        }

        public static void RemoveEDMDesignerViewContent(IEDMDesignerChangeWatcherObserver observer)
        {
            _observers.Remove(observer);
        }

        public static void ObjectChanged(EDMObjectBase edmObjectBase)
        {
            if (_init)
                return;
            
            foreach (IEDMDesignerChangeWatcherObserver observer in _observers)
            {
                if (observer.ObjectChanged(edmObjectBase))
                    break;
            }
        }

        public static void ObjectChanged(ITypeDesigner typeDesigner)
        {
            if (_init)
                return;

            foreach (IEDMDesignerChangeWatcherObserver observer in _observers)
            {
                if (observer.ObjectChanged(typeDesigner))
                    break;
            }
        }

        #endregion
    }
}
