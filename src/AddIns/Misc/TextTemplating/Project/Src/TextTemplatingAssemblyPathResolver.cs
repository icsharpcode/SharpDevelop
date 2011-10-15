// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingAssemblyPathResolver : ITextTemplatingAssemblyPathResolver
	{
		IProject project;
		IAssemblyParserService assemblyParserService;
		ITextTemplatingPathResolver pathResolver;
		
		public TextTemplatingAssemblyPathResolver(
			IProject project,
			IAssemblyParserService assemblyParserService,
			ITextTemplatingPathResolver pathResolver)
		{
			this.project = project;
			this.assemblyParserService = assemblyParserService;
			this.pathResolver = pathResolver;
		}
		
		public TextTemplatingAssemblyPathResolver(IProject project)
			: this(
				project,
				new TextTemplatingAssemblyParserService(),
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
			IReflectionProjectContent projectContent = GetProjectContent(assemblyReference);
			if (projectContent != null) {
				return projectContent.AssemblyLocation;
			}
			return null;
		}
		
		IReflectionProjectContent GetProjectContent(string assemblyReference)
		{
			var reference = new ReferenceProjectItem(project, assemblyReference);
			return GetProjectContent(reference);
		}
		
		IReflectionProjectContent GetProjectContent(ReferenceProjectItem refProjectItem)
		{
			return assemblyParserService.GetReflectionProjectContentForReference(refProjectItem);
		}
	}
}
