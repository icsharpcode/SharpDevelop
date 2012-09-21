// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// A model collection implementation that is based on a ObservableCollection.
	/// </summary>
	public class SimpleModelCollection<T> : ObservableCollection<T>, IModelCollection<T>
	{
	}
}
