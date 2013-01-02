// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Completion
{
	sealed class CSharpCompletionDataFactory : ICompletionDataFactory, IParameterCompletionDataFactory
	{
		readonly CSharpCompletionBinding binding;
		readonly ITextEditor editor;
		readonly CSharpTypeResolveContext contextAtCaret;
		
		public CSharpCompletionDataFactory(CSharpCompletionBinding binding, ITextEditor editor, CSharpTypeResolveContext contextAtCaret)
		{
			Debug.Assert(binding != null);
			Debug.Assert(editor != null);
			Debug.Assert(contextAtCaret != null);
			this.binding = binding;
			this.editor = editor;
			this.contextAtCaret = contextAtCaret;
		}
		
		#region ICompletionDataFactory implementation
		ICompletionData ICompletionDataFactory.CreateEntityCompletionData(IEntity entity)
		{
			return new EntityCompletionData(entity);
		}
		
		ICompletionData ICompletionDataFactory.CreateEntityCompletionData(IEntity entity, string text)
		{
			return new EntityCompletionData(entity) {
				CompletionText = text,
				DisplayText = text
			};
		}
		
		ICompletionData ICompletionDataFactory.CreateTypeCompletionData(IType type, bool showFullName, bool isInAttributeContext)
		{
			var typeDef = type.GetDefinition();
			if (typeDef != null)
				return new EntityCompletionData(typeDef);
			else
				return new CompletionData(type.Name);
		}
		
		ICompletionData ICompletionDataFactory.CreateMemberCompletionData(IType type, IEntity member)
		{
			return new CompletionData(type.Name + "." + member.Name);
		}
		
		ICompletionData ICompletionDataFactory.CreateLiteralCompletionData(string title, string description, string insertText)
		{
			return new CompletionData(title) {
				Description = description,
				CompletionText = insertText ?? title,
				Image = ClassBrowserIconService.Keyword
			};
		}
		
		ICompletionData ICompletionDataFactory.CreateNamespaceCompletionData(INamespace name)
		{
			return new CompletionData(name.Name) {
				Image = ClassBrowserIconService.Namespace
			};
		}
		
		ICompletionData ICompletionDataFactory.CreateVariableCompletionData(IVariable variable)
		{
			return new CompletionData(variable.Name) {
				Image = ClassBrowserIconService.LocalVariable
			};
		}
		
		ICompletionData ICompletionDataFactory.CreateVariableCompletionData(ITypeParameter parameter)
		{
			return new CompletionData(parameter.Name);
		}
		
		ICompletionData ICompletionDataFactory.CreateEventCreationCompletionData(string varName, IType delegateType, IEvent evt, string parameterDefinition, IUnresolvedMember currentMember, IUnresolvedTypeDefinition currentType)
		{
			return new CompletionData("TODO: event creation");
		}
		
		ICompletionData ICompletionDataFactory.CreateNewOverrideCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IMember m)
		{
			return new OverrideCompletionData(declarationBegin, m, contextAtCaret);
		}
		
		ICompletionData ICompletionDataFactory.CreateNewPartialCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IUnresolvedMember m)
		{
			return new CompletionData("TODO: partial completion");
		}
		
		IEnumerable<ICompletionData> ICompletionDataFactory.CreateCodeTemplateCompletionData()
		{
			yield break;
		}
		
		IEnumerable<ICompletionData> ICompletionDataFactory.CreatePreProcessorDefinesCompletionData()
		{
			yield break;
		}
		
		ICompletionData ICompletionDataFactory.CreateImportCompletionData(IType type, bool useFullName)
		{
			return new CompletionData("TODO: import completion");
		}
		#endregion
		
		#region IParameterCompletionDataFactory implementation
		IParameterDataProvider CreateMethodDataProvider(int startOffset, IEnumerable<IParameterizedMember> methods)
		{
			return new CSharpMethodInsight(binding, editor, startOffset, from m in methods where m != null select new CSharpInsightItem(m));
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateConstructorProvider(int startOffset, IType type)
		{
			return CreateMethodDataProvider(startOffset, type.GetConstructors());
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateConstructorProvider(int startOffset, IType type, AstNode thisInitializer)
		{
			return CreateMethodDataProvider(startOffset, type.GetConstructors());
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateMethodDataProvider(int startOffset, IEnumerable<IMethod> methods)
		{
			return CreateMethodDataProvider(startOffset, methods);
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateDelegateDataProvider(int startOffset, IType type)
		{
			return CreateMethodDataProvider(startOffset, new[] { type.GetDelegateInvokeMethod() });
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateIndexerParameterDataProvider(int startOffset, IType type, AstNode resolvedNode)
		{
			return CreateMethodDataProvider(startOffset, type.GetProperties(p => p.IsIndexer));
		}
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateTypeParameterDataProvider(int startOffset, IEnumerable<IType> types)
		{
			return null;
		}
		#endregion
	}
}
