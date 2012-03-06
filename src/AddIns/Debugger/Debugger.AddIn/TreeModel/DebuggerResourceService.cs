// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.SharpDevelop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// Gets resources the way suitable for Debugger.AddIn.
	/// </summary>
	public static class DebuggerResourceService
	{
		/// <summary>
		/// Gets image with given name from resources.
		/// </summary>
		/// <param name="resourceName">Resource name of the image.</param>
		public static IImage GetImage(string resourceName)
		{
			return new ResourceServiceImage(resourceName);
		}
	}
}
