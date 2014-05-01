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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	sealed class CSharpCompletionDataFactory : ICompletionDataFactory, IParameterCompletionDataFactory
	{
		readonly CSharpCompletionContext completionContext;
		readonly CSharpResolver contextAtCaret;
		readonly TypeSystemAstBuilder builder;
		
		public CSharpCompletionDataFactory(CSharpCompletionContext completionContext, CSharpResolver contextAtCaret)
		{
			Debug.Assert(completionContext != null);
			Debug.Assert(contextAtCaret != null);
			this.completionContext = completionContext;
			this.contextAtCaret = contextAtCaret;
			this.builder = new TypeSystemAstBuilder(contextAtCaret);
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
		
		ICompletionData ICompletionDataFactory.CreateTypeCompletionData(IType type, bool showFullName, bool isInAttributeContext, bool addForTypeCreation)
		{
			var data = new TypeCompletionData(type);
			if (showFullName) {
				string text = builder.ConvertType(type).ToString();
				data.CompletionText = text;
				data.DisplayText = text;
			}
			if (isInAttributeContext) {
				data.CompletionText = StripAttributeSuffix(data.CompletionText);
				data.DisplayText = StripAttributeSuffix(data.DisplayText);
			}
			return data;
		}
		
		static string StripAttributeSuffix(string text)
		{
			if (text.Length > "Attribute".Length && text.EndsWith("Attribute", StringComparison.Ordinal))
				return text.Substring(0, text.Length - "Attribute".Length);
			else
				return text;
		}
		
		ICompletionData ICompletionDataFactory.CreateMemberCompletionData(IType type, IEntity member)
		{
			string typeName = builder.ConvertType(type).ToString();
			return new CompletionData(typeName + "." + member.Name);
		}
		
		ICompletionData ICompletionDataFactory.CreateLiteralCompletionData(string title, string description, string insertText)
		{
			return new LiteralCompletionData(title) {
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
			return new EventCreationCompletionData(varName, delegateType, evt, parameterDefinition, currentMember, currentType, contextAtCaret);
		}
		
		ICompletionData ICompletionDataFactory.CreateNewOverrideCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IMember m)
		{
			if ((m.SymbolKind == SymbolKind.Method) && (m.Name == "ToString"))
				return new OverrideToStringCompletionData(declarationBegin, m, contextAtCaret);
			else if ((m.SymbolKind == SymbolKind.Method) && ((m.Name == "GetHashCode")
			                                                 || ((m.Name == "Equals") && ((((IMethod) m)).Parameters.Count == 1) && (((IMethod) m).Parameters.First().Type.FullName == "System.Object"))))
				return new OverrideEqualsGetHashCodeCompletionData(declarationBegin, m, contextAtCaret);
			else
				return new OverrideCompletionData(declarationBegin, m, contextAtCaret);
		}
		
		ICompletionData ICompletionDataFactory.CreateNewPartialCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IUnresolvedMember m)
		{
			return new PartialCompletionData(declarationBegin, m.Resolve(contextAtCaret.CurrentTypeResolveContext), contextAtCaret);
		}
		
		IEnumerable<ICompletionData> ICompletionDataFactory.CreateCodeTemplateCompletionData()
		{
			// We handle this in CSharpCompletionBinding, because there we have the full list
			// of keywords available in the given context.
			return ICSharpCode.NRefactory.EmptyList<ICompletionData>.Instance;
		}
		
		IEnumerable<ICompletionData> ICompletionDataFactory.CreatePreProcessorDefinesCompletionData()
		{
			return completionContext.ParseInformation.SyntaxTree.ConditionalSymbols.Select(def => new CompletionData(def));
		}
		
		ICompletionData ICompletionDataFactory.CreateImportCompletionData(IType type, bool useFullName, bool addForTypeCreation)
		{
			ITypeDefinition typeDef = type.GetDefinition();
			if (typeDef != null)
				return new ImportCompletionData(typeDef, contextAtCaret, useFullName);
			else
				throw new InvalidOperationException("Should never happen");
		}
		
		ICompletionData ICompletionDataFactory.CreateFormatItemCompletionData(string format, string description, object example)
		{
			return new FormatItemCompletionData(format, description, example);
		}

		ICompletionData ICompletionDataFactory.CreateXmlDocCompletionData(string tag, string description, string tagInsertionText)
		{
			return new XmlDocCompletionData(tag, description, tagInsertionText);
		}
		#endregion
		
		#region IParameterCompletionDataFactory implementation
		IParameterDataProvider CreateMethodDataProvider(int startOffset, IEnumerable<IParameterizedMember> methods)
		{
			return new CSharpMethodInsight(completionContext.Editor, startOffset, from m in methods where m != null select new CSharpInsightItem(m));
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
		
		IParameterDataProvider IParameterCompletionDataFactory.CreateIndexerParameterDataProvider(int startOffset, IType type, IEnumerable<IProperty> accessibleIndexers, AstNode resolvedNode)
		{
			return CreateMethodDataProvider(startOffset, accessibleIndexers);
		}

		IParameterDataProvider IParameterCompletionDataFactory.CreateTypeParameterDataProvider(int startOffset, IEnumerable<IMethod> methods)
		{
			return null;
		}

		IParameterDataProvider IParameterCompletionDataFactory.CreateTypeParameterDataProvider(int startOffset, IEnumerable<IType> types)
		{
			return null;
		}
		#endregion
	}
}
