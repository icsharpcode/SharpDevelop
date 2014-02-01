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
