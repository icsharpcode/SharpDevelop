// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.ILSpyAddIn
{
	public class NavigateToDecompiledEntityService : INavigateToEntityService
	{
		public bool NavigateTo(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			// Get the underlying entity for generic instance members
			while ((entity is IMember) && ((IMember)entity).GenericMember != null)
				entity = ((IMember)entity).GenericMember;
			
			IClass declaringType = (entity as IClass) ?? entity.DeclaringType;
			if (declaringType == null)
				return false;
			// get the top-level type
			while (declaringType.DeclaringType != null)
				declaringType = declaringType.DeclaringType;
			
			ReflectionProjectContent rpc = entity.ProjectContent as ReflectionProjectContent;
			if (rpc != null) {
				string assemblyLocation = ILSpyController.GetAssemblyLocation(rpc);
				if (!string.IsNullOrEmpty(assemblyLocation) && File.Exists(assemblyLocation)) {
					NavigateTo(assemblyLocation, declaringType.FullyQualifiedName, ((AbstractEntity)entity).DocumentationTag);
					return true;
				}
			}
			return false;
		}
		
		public static void NavigateTo(string assemblyFile, string typeName, string entityTag)
		{
			foreach (var vc in WorkbenchSingleton.Workbench.ViewContentCollection.OfType<DecompiledViewContent>()) {
				if (string.Equals(vc.AssemblyFile, assemblyFile, StringComparison.OrdinalIgnoreCase) && typeName == vc.FullTypeName) {
					vc.WorkbenchWindow.SelectWindow();
					vc.JumpToEntity(entityTag);
					return;
				}
			}
			WorkbenchSingleton.Workbench.ShowView(new DecompiledViewContent(assemblyFile, typeName, entityTag));
		}
	}
}
