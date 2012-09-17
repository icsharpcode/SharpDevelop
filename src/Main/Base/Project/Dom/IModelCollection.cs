// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A read-only collection that provides change notifications.
	/// </summary>
	public interface IModelCollection<out T> : IReadOnlyCollection<T>, INotifyCollectionChanged
	{
	}
}
