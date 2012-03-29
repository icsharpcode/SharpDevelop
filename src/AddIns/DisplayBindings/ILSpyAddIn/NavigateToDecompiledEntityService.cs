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
	public class NavigateToDecompiledEntityService : INavigateToEntityService, INavigateToMemberService
	{
		public bool NavigateToEntity(IEntity entity)
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
					NavigateTo(assemblyLocation, declaringType.DotNetName, ((AbstractEntity)entity).DocumentationTag);
					return true;
				}
			}
			return false;
		}
		
		public static void NavigateTo(string assemblyFile, string typeName, string entityTag)
		{
			if (string.IsNullOrEmpty(assemblyFile))
				throw new ArgumentException("assemblyFile is null or empty");
			
			if (string.IsNullOrEmpty(typeName))
				throw new ArgumentException("typeName is null or empty");
			
			foreach (var viewContent in WorkbenchSingleton.Workbench.ViewContentCollection.OfType<DecompiledViewContent>()) {
				if (string.Equals(viewContent.AssemblyFile, assemblyFile, StringComparison.OrdinalIgnoreCase) && typeName == viewContent.FullTypeName) {
					viewContent.WorkbenchWindow.SelectWindow();
					viewContent.JumpToEntity(entityTag);
					return;
				}
			}
			WorkbenchSingleton.Workbench.ShowView(new DecompiledViewContent(assemblyFile, typeName, entityTag));
		}
		
		public bool NavigateToMember(string assemblyFile, string typeName, string entityTag, int lineNumber, bool updateMarker)
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
	}
}
