// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.Reflection;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Designer
{
	/// <summary>
	/// Description of IDesignerTypeResolutionService.
	/// </summary>
	public interface IDesignerTypeResolutionService : ITypeResolutionService
	{
		Assembly LoadAssembly(IProject pc);
	}
}
