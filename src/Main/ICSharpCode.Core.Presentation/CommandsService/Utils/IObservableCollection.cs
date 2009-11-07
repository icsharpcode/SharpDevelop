using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Interfaces describing observable collection
	/// </summary>
    public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged
    {
    }
}
