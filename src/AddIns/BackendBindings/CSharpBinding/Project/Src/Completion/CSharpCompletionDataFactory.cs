// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace CSharpBinding.Completion
{
	class CSharpCompletionDataFactory : ICompletionDataFactory
	{
		public ICompletionData CreateEntityCompletionData(IUnresolvedEntity entity)
		{
			return new CompletionData(entity.Name) {
				Image = ClassBrowserIconService.GetIcon(entity)
			};
		}
		
		public ICompletionData CreateEntityCompletionData(IUnresolvedEntity entity, string text)
		{
			return new CompletionData(text) {
				Image = ClassBrowserIconService.GetIcon(entity)
			};
		}
		
		public ICompletionData CreateEntityCompletionData(IEntity entity)
		{
			return new EntityCompletionData(entity);
		}
		
		public ICompletionData CreateEntityCompletionData(IEntity entity, string text)
		{
			return new EntityCompletionData(entity) {
				CompletionText = text,
				DisplayText = text
			};
		}
		
		public ICompletionData CreateTypeCompletionData(IType type, string shortType)
		{
			return new CompletionData(shortType);
		}
		
		public ICompletionData CreateTypeCompletionData(IUnresolvedTypeDefinition type, string shortType)
		{
			return new CompletionData(shortType) {
				Image = ClassBrowserIconService.GetIcon(type)
			};
		}
		
		public ICompletionData CreateLiteralCompletionData(string title, string description, string insertText)
		{
			return new CompletionData(title) {
				Description = description,
				CompletionText = insertText ?? title,
				Image = ClassBrowserIconService.Keyword
			};
		}
		
		public ICompletionData CreateNamespaceCompletionData(string name)
		{
			return new CompletionData(name) {
				Image = ClassBrowserIconService.Namespace
			};
		}
		
		public ICompletionData CreateVariableCompletionData(IVariable variable)
		{
			return new CompletionData(variable.Name) { 
				Image = ClassBrowserIconService.LocalVariable
			};
		}
		
		public ICompletionData CreateVariableCompletionData(IUnresolvedTypeParameter parameter)
		{
			return new CompletionData(parameter.Name);
		}
		
		public ICompletionData CreateEventCreationCompletionData(string varName, IType delegateType, IEvent evt, string parameterDefinition, IUnresolvedMember currentMember, IUnresolvedTypeDefinition currentType)
		{
			throw new NotImplementedException();
		}
		
		public ICompletionData CreateNewOverrideCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IMember m)
		{
			throw new NotImplementedException();
		}
		
		public ICompletionData CreateNewPartialCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IUnresolvedMember m)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<ICompletionData> CreateCodeTemplateCompletionData()
		{
			yield break;
		}
		
		public IEnumerable<ICompletionData> CreatePreProcessorDefinesCompletionData()
		{
			yield break;
		}
	}
}
