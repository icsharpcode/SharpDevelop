// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
