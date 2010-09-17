#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.ChangeWatcher
{
    public interface IEDMDesignerChangeWatcherObserver
    {
        bool ObjectChanged(object changedObject);
    }
}
