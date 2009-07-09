// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Utils
{
	/// <summary>
	/// Anything that that has recursive children. Used by <see cref="TreeFlattener">.
	/// </summary>
	public interface ITreeNode<T>
	{
		IEnumerable<T> Children { get; }
	}
}
