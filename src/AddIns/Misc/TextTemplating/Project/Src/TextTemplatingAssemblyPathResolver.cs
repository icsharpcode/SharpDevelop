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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingAssemblyPathResolver : ITextTemplatingAssemblyPathResolver
	{
		IProject project;
		IGlobalAssemblyCacheService gac;
		ITextTemplatingPathResolver pathResolver;
		
		public TextTemplatingAssemblyPathResolver(
			IProject project,
			IGlobalAssemblyCacheService gac,
			ITextTemplatingPathResolver pathResolver)
		{
			this.project = project;
			this.gac = gac;
			this.pathResolver = pathResolver;
		}
		
		public TextTemplatingAssemblyPathResolver(IProject project)
			: this(
				project,
				SD.GlobalAssemblyCache,
				new TextTemplatingPathResolver())
		{
		}
		
		public string ResolvePath(string assemblyReference)
		{
			assemblyReference = pathResolver.ResolvePath(assemblyReference);
			if (Path.IsPathRooted(assemblyReference)) {
				return assemblyReference;
			}
			
			string resolvedAssemblyFileName = ResolveAssemblyFromProject(assemblyReference);
			if (resolvedAssemblyFileName == null) {
				resolvedAssemblyFileName = ResolveAssemblyFromGac(assemblyReference);
			}
			if (resolvedAssemblyFileName != null) {
				return resolvedAssemblyFileName;
			}
			return assemblyReference;
		}
		
		string ResolveAssemblyFromProject(string assemblyReference)
		{
			foreach (ReferenceProjectItem refProjectItem in project.GetItemsOfType(ItemType.Reference)) {
				if (IsMatch(refProjectItem, assemblyReference)) {
					return refProjectItem.FileName;
				}
			}
			return null;
		}
		
		bool IsMatch(ReferenceProjectItem refProjectItem, string assemblyReference)
		{
			return String.Equals(refProjectItem.Include, assemblyReference, StringComparison.InvariantCultureIgnoreCase);
		}
		
		string ResolveAssemblyFromGac(string assemblyReference)
		{
			var assemblyName = new DomAssemblyName(assemblyReference);
			DomAssemblyName foundAssemblyName = gac.FindBestMatchingAssemblyName(assemblyName);
			if (foundAssemblyName != null) {
				FileName fileName = gac.FindAssemblyInNetGac(foundAssemblyName);
				if (fileName != null) {
					return fileName.ToString();
				}
			}
			return null;
		}
	}
}
