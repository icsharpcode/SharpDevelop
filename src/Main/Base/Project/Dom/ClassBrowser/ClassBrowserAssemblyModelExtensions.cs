// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public static class ClassBrowserAssemblyModelExtensions
	{
		/// <summary>
		/// Checks if an assembly model is added to the unpinned (temporary) list of ClassBrowser service.
		/// </summary>
		/// <param name="model">Assembly model to check.</param>
		/// <returns>True if in unpinned list, false otherwise.</returns>
		public static bool IsUnpinned(this IAssemblyModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			
			var classBrowser = SD.GetService<IClassBrowser>();
			if (classBrowser != null) {
				// Look in unpinned list
				return classBrowser.UnpinnedAssemblies.Assemblies.Any(m => m.Location == model.Location);
			}
			
			return false;
		}
	}
}
