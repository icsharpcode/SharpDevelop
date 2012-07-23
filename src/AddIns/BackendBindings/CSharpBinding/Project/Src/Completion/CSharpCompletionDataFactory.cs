// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace CSharpBinding.Completion
{
	class CSharpCompletionDataFactory : ICompletionDataFactory, IParameterCompletionDataFactory
	{
		readonly CSharpTypeResolveContext contextAtCaret;
		
		public CSharpCompletionDataFactory(CSharpTypeResolveContext contextAtCaret)
		{
			if (contextAtCaret == null)
				throw new ArgumentNullException("contextAtCaret");
			this.contextAtCaret = contextAtCaret;
		}
		
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
			return new CompletionData("TODO: event creation");
		}
		
		public ICompletionData CreateNewOverrideCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IMember m)
		{
			return new OverrideCompletionData(declarationBegin, m, contextAtCaret);
		}
		
		public ICompletionData CreateNewPartialCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IUnresolvedMember m)
		{
			return new CompletionData("TODO: partial completion");
		}
		
		public IEnumerable<ICompletionData> CreateCodeTemplateCompletionData()
		{
			yield break;
		}
		
		public IEnumerable<ICompletionData> CreatePreProcessorDefinesCompletionData()
		{
			yield break;
		}
		
		#region IParameterCompletionDataFactory implementation
		IParameterDataProvider IParameterCompletionDataFactory.CreateConstructorProvider(int startOffset, IType type)
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			return new CSharpParameterDataProvider(startOffset, type.GetConstructors().Select(m => new CSharpInsightItem(m, ambience)));
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateMethodDataProvider(int startOffset, IEnumerable<IMethod> methods)
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			return new CSharpParameterDataProvider(startOffset, methods.Select(m => new CSharpInsightItem(m, ambience)));
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateDelegateDataProvider(int startOffset, IType type)
		{
			return new CSharpParameterDataProvider(startOffset, Enumerable.Empty<CSharpInsightItem>());
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateIndexerParameterDataProvider(int startOffset, IType type, AstNode resolvedNode)
		{
			throw new NotImplementedException();
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateTypeParameterDataProvider(int startOffset, IEnumerable<IType> types)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
