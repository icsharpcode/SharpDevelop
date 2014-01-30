// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
