// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Base interface for a single assembly reference.
	/// </summary>
	public interface IAssemblyReferenceModel
	{
		DomAssemblyName AssemblyName { get; }
		IAssemblyModel ParentAssemblyModel { get; }
	}
}
