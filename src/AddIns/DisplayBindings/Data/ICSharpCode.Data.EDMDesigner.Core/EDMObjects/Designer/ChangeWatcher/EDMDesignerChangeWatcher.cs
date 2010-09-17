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
