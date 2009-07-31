using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ICSharpCode.Core.Presentation
{
    public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged
    {
    }
}
