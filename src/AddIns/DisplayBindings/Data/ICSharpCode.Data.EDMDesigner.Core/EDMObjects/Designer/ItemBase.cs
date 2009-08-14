#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer
{
    public abstract class ItemBase<T>
    {
        public string Text { get; internal set; }
        public T Value { get; internal set; }
    }
}
