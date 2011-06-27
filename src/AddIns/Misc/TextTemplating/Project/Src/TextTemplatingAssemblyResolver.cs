// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingAssemblyResolver : ITextTemplatingAssemblyResolver
	{
		IProject project;
		
		public TextTemplatingAssemblyResolver(IProject project)
		{
			this.project = project;
		}
		
		public string Resolve(string assemblyReference)
		{
			foreach (ReferenceProjectItem refProjectItem in project.GetItemsOfType(ItemType.Reference)) {
				if (IsMatch(refProjectItem, assemblyReference)) {
					return refProjectItem.FileName;
				}
			}
			return assemblyReference;
		}
		
		bool IsMatch(ReferenceProjectItem refProjectItem, string assemblyReference)
		{
			return String.Equals(refProjectItem.Include, assemblyReference, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
