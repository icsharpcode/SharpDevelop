// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.ILSpyAddIn
{
	public class NavigateToDecompiledEntityService : INavigateToEntityService
	{
		public bool NavigateToEntity(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			// Get the underlying entity for generic instance members
			if (entity is IMember)
				entity = ((IMember)entity).MemberDefinition;
			
			ITypeDefinition declaringType = (entity as ITypeDefinition) ?? entity.DeclaringTypeDefinition;
			if (declaringType == null)
				return false;
			// get the top-level type
			while (declaringType.DeclaringTypeDefinition != null)
				declaringType = declaringType.DeclaringTypeDefinition;
			
			FileName assemblyLocation = declaringType.ParentAssembly.GetRuntimeAssemblyLocation();
			if (assemblyLocation != null && File.Exists(assemblyLocation)) {
				NavigateTo(assemblyLocation, declaringType.ReflectionName, IdStringProvider.GetIdString(entity));
				return true;
			}
			return false;
		}
		
		public static void NavigateTo(FileName assemblyFile, string typeName, string entityIdString)
		{
			if (assemblyFile == null)
				throw new ArgumentNullException("assemblyFile");
			if (string.IsNullOrEmpty(typeName))
				throw new ArgumentException("typeName is null or empty");
			
			var type = new TopLevelTypeName(typeName);
			var target = new DecompiledTypeReference(assemblyFile, type);
			
			foreach (var viewContent in SD.Workbench.ViewContentCollection.OfType<DecompiledViewContent>()) {
				var viewContentName = viewContent.DecompiledTypeName;
				if (viewContentName.AssemblyFile == assemblyFile && type == viewContentName.Type) {
					viewContent.WorkbenchWindow.SelectWindow();
					viewContent.JumpToEntity(entityIdString);
					return;
				}
			}
			SD.Workbench.ShowView(new DecompiledViewContent(target, entityIdString));
		}
	}
}
