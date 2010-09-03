// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Anything that has recursive children. Used by <see cref="TreeFlattener">.
	/// </summary>
	public interface ITreeNode<T>
	{
		IEnumerable<T> Children { get; }
	}
}
