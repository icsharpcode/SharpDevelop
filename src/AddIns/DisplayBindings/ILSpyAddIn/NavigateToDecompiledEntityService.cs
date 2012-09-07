// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
			
			foreach (var viewContent in WorkbenchSingleton.Workbench.ViewContentCollection.OfType<DecompiledViewContent>()) {
				if (viewContent.AssemblyFile == assemblyFile && typeName == viewContent.FullTypeName) {
					viewContent.WorkbenchWindow.SelectWindow();
					viewContent.JumpToEntity(entityIdString);
					return;
				}
			}
			WorkbenchSingleton.Workbench.ShowView(new DecompiledViewContent(assemblyFile, typeName, entityIdString));
		}
		
		/*
		public bool NavigateToMember(FileName assemblyFile, string typeName, string entityTag, int lineNumber, bool updateMarker)
		{
			if (string.IsNullOrEmpty(assemblyFile))
				throw new ArgumentException("assemblyFile is null or empty");
			
			if (string.IsNullOrEmpty(typeName))
				throw new ArgumentException("typeName is null or empty");
			
			// jump to line number if the decompiled view content exists - no need for a new decompilation
			foreach (var viewContent in WorkbenchSingleton.Workbench.ViewContentCollection.OfType<DecompiledViewContent>()) {
				if (string.Equals(viewContent.AssemblyFile, assemblyFile, StringComparison.OrdinalIgnoreCase) && typeName == viewContent.FullTypeName) {
					if (updateMarker) {
						viewContent.UpdateDebuggingUI();
					}
					if (lineNumber > 0)
						viewContent.JumpToLineNumber(lineNumber);
					else
						viewContent.JumpToEntity(entityTag);
					viewContent.WorkbenchWindow.SelectWindow();
					return true;
				}
			}
			
			// create a new decompiled view
			var decompiledView = new DecompiledViewContent(assemblyFile, typeName, entityTag);
			decompiledView.DecompilationFinished += delegate {
				if (updateMarker) {
					decompiledView.UpdateDebuggingUI();
				}
				if (lineNumber > 0)
					decompiledView.JumpToLineNumber(lineNumber);
				else
					decompiledView.JumpToEntity(entityTag);
			};
			WorkbenchSingleton.Workbench.ShowView(decompiledView);
			return true;
		}
		*/
	}
}
