// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	public class VBNetOutputVisitor : IOutputASTVisitor
	{
		Errors                  errors             = new Errors();
		VBNetOutputFormatter    outputFormatter;
		VBNetPrettyPrintOptions prettyPrintOptions = new VBNetPrettyPrintOptions();
		NodeTracker             nodeTracker;
		TypeDeclaration         currentType;
		
		Stack  exitTokenStack = new Stack();
		
		public string Text {
			get {
				return outputFormatter.Text;
			}
		}
		
		public Errors Errors {
			get {
				return errors;
			}
		}
		
		public object Options {
			get {
				return prettyPrintOptions;
			}
			set {
				prettyPrintOptions = value as VBNetPrettyPrintOptions;
			}
		}
		
		public NodeTracker NodeTracker {
			get {
				return nodeTracker;
			}
		}
		
		public VBNetOutputFormatter OutputFormatter {
			get {
				return outputFormatter;
			}
		}
		
		public VBNetOutputVisitor()
		{
			outputFormatter = new VBNetOutputFormatter(prettyPrintOptions);
			nodeTracker     = new NodeTracker(this);
		}
		
		#region ICSharpCode.NRefactory.Parser.IASTVisitor interface implementation
		public object Visit(INode node, object data)
		{
			errors.Error(-1, -1, String.Format("Visited INode (should NEVER HAPPEN), node is : {0}", node.ToString()));
			return node.AcceptChildren(this, data);
		}
		
		public object Visit(CompilationUnit compilationUnit, object data)
		{
			nodeTracker.TrackedVisitChildren(compilationUnit, data);
			outputFormatter.EndFile();
			return null;
		}
		
		string ConvertTypeString(string typeString)
		{
			switch (typeString) {
				case "System.Boolean":
					return "Boolean";
				case "System.String":
					return "String";
				case "System.Char":
					return "Char";
				case "System.Double":
					return "Double";
				case "System.Single":
					return "Single";
				case "System.Decimal":
					return "Decimal";
				case "System.DateTime":
					return "Date";
				case "System.Int64":
					return "Long";
				case "System.Int32":
					return "Integer";
				case "System.Int16":
					return "Short";
				case "System.Byte":
					return "Byte";
				case "System.Void":
					return "Void";
				case "System.Object":
					return "Object";
					
				case "System.UInt64":
					return "System.UInt64";
				case "System.UInt32":
					return "System.UInt32";
				case "System.UInt16":
					return "System.UInt16";
				case "System.SByte":
					return "System.SByte";
			}
			return typeString;
		}

		public object Visit(TypeReference typeReference, object data)
		{
			if (typeReference.Type == null || typeReference.Type.Length ==0) {
				outputFormatter.PrintText("Void");
			} else {
				if (typeReference.SystemType.Length > 0) {
					outputFormatter.PrintText(ConvertTypeString(typeReference.SystemType));
				} else {
					outputFormatter.PrintIdentifier(typeReference.Type);
				}
			}
			if (typeReference.GenericTypes != null && typeReference.GenericTypes.Count > 0) {
				outputFormatter.PrintToken(Tokens.OpenParenthesis);
				outputFormatter.PrintToken(Tokens.Of);
				outputFormatter.Space();
				AppendCommaSeparatedList(typeReference.GenericTypes);
				outputFormatter.PrintToken(Tokens.CloseParenthesis);
			}
			for (int i = 0; i < typeReference.PointerNestingLevel; ++i) {
				outputFormatter.PrintToken(Tokens.Times);
			}
			if (typeReference.IsArrayType) {
				for (int i = 0; i < typeReference.RankSpecifier.Length; ++i) {
					outputFormatter.PrintToken(Tokens.OpenParenthesis);
					for (int j = 1; j < typeReference.RankSpecifier[i]; ++j) {
						outputFormatter.PrintToken(Tokens.Comma);
					}
					outputFormatter.PrintToken(Tokens.CloseParenthesis);
				}
			}
			return null;
		}
		
		#region Global scope
		public object Visit(AttributeSection attributeSection, object data)
		{
			outputFormatter.Indent();
			outputFormatter.PrintText("<");
			if (attributeSection.AttributeTarget != null && attributeSection.AttributeTarget != String.Empty) {
				outputFormatter.PrintIdentifier(attributeSection.AttributeTarget);
				outputFormatter.PrintToken(Tokens.Colon);
				outputFormatter.Space();
			}
			Debug.Assert(attributeSection.Attributes != null);
			for (int j = 0; j < attributeSection.Attributes.Count; ++j) {
				nodeTracker.TrackedVisit((INode)attributeSection.Attributes[j], data);
				if (j + 1 < attributeSection.Attributes.Count) {
					outputFormatter.PrintToken(Tokens.Comma);
				}
			}
			if ("assembly".Equals(attributeSection.AttributeTarget, StringComparison.InvariantCultureIgnoreCase)
			    || "module".Equals(attributeSection.AttributeTarget, StringComparison.InvariantCultureIgnoreCase)) {
				outputFormatter.PrintText(">");
			} else {
				outputFormatter.PrintText("> _");
			}
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(ICSharpCode.NRefactory.Parser.AST.Attribute attribute, object data)
		{
			outputFormatter.PrintIdentifier(attribute.Name);
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			this.AppendCommaSeparatedList(attribute.PositionalArguments);
			
			if (attribute.NamedArguments != null && attribute.NamedArguments.Count > 0) {
				if (attribute.PositionalArguments.Count > 0) {
					outputFormatter.PrintToken(Tokens.Comma);
				}
				for (int i = 0; i < attribute.NamedArguments.Count; ++i) {
					nodeTracker.TrackedVisit((INode)attribute.NamedArguments[i], data);
					if (i + 1 < attribute.NamedArguments.Count) {
						outputFormatter.PrintToken(Tokens.Comma);
					}
				}
			}
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			return null;
		}
		
		public object Visit(NamedArgumentExpression namedArgumentExpression, object data)
		{
			outputFormatter.PrintIdentifier(namedArgumentExpression.Name);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Assign);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(namedArgumentExpression.Expression, data);
			return null;
		}
		
		public object Visit(Using u, object data)
		{
			Debug.Fail("Should never be called. The usings should be handled in Visit(UsingDeclaration)");
			return null;
		}
		
		public object Visit(UsingDeclaration usingDeclaration, object data)
		{
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Imports);
			outputFormatter.Space();
			for (int i = 0; i < usingDeclaration.Usings.Count; ++i) {
				if (((Using)usingDeclaration.Usings[i]).IsAlias) {
					outputFormatter.PrintIdentifier(((Using)usingDeclaration.Usings[i]).Alias);
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.As);
					outputFormatter.Space();
				}
				outputFormatter.PrintIdentifier(((Using)usingDeclaration.Usings[i]).Name);
				if (i + 1 < usingDeclaration.Usings.Count) {
					outputFormatter.PrintToken(Tokens.Comma);
					outputFormatter.Space();
				}
			}
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(NamespaceDeclaration namespaceDeclaration, object data)
		{
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Namespace);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(namespaceDeclaration.Name);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			nodeTracker.TrackedVisitChildren(namespaceDeclaration, data);
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Namespace);
			outputFormatter.NewLine();
			return null;
		}
		
		int GetTypeToken(TypeDeclaration typeDeclaration)
		{
			switch (typeDeclaration.Type) {
				case Types.Class:
					return Tokens.Class;
				case Types.Enum:
					return Tokens.Enum;
				case Types.Interface:
					return Tokens.Interface;
				case Types.Struct:
					// FIXME: This should be better in VBNetRefactory class because it is an AST transformation, but currently I'm too lazy
					if (TypeHasOnlyStaticMembers(typeDeclaration)) {
						goto case Types.Class;
					}
					return Tokens.Structure;
			}
			return Tokens.Class;
		}
		
		void PrintTemplates(List<TemplateDefinition> templates)
		{
			if (templates != null && templates.Count > 0) {
				outputFormatter.PrintToken(Tokens.OpenParenthesis);
				outputFormatter.PrintToken(Tokens.Of);
				outputFormatter.Space();
				AppendCommaSeparatedList(templates);
				outputFormatter.PrintToken(Tokens.CloseParenthesis);
			}
		}
		
		public object Visit(TypeDeclaration typeDeclaration, object data)
		{
			VisitAttributes(typeDeclaration.Attributes, data);
			
			outputFormatter.Indent();
			OutputModifier(typeDeclaration.Modifier);
			
			int typeToken = GetTypeToken(typeDeclaration);
			outputFormatter.PrintToken(typeToken);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(typeDeclaration.Name);
			
			PrintTemplates(typeDeclaration.Templates);
			
			outputFormatter.NewLine();
			
			if (typeDeclaration.BaseTypes != null) {
				foreach (string baseType in typeDeclaration.BaseTypes) {
					outputFormatter.Indent();
					
					bool baseTypeIsInterface = baseType.StartsWith("I") && (baseType.Length <= 1 || Char.IsUpper(baseType[1]));
					
					if (!baseTypeIsInterface || typeDeclaration.Type == Types.Interface) {
						outputFormatter.PrintToken(Tokens.Inherits);
					} else {
						outputFormatter.PrintToken(Tokens.Implements);
					}
					outputFormatter.Space();
					outputFormatter.PrintIdentifier(baseType);
					outputFormatter.NewLine();
				}
			}
			
			++outputFormatter.IndentationLevel;
			TypeDeclaration oldType = currentType;
			currentType = typeDeclaration;
			
			nodeTracker.TrackedVisitChildren(typeDeclaration, data);
			currentType = oldType;
			
			--outputFormatter.IndentationLevel;
			
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(typeToken);
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(TemplateDefinition templateDefinition, object data)
		{
			outputFormatter.PrintIdentifier(templateDefinition.Name);
			if (templateDefinition.Bases.Count > 0) {
				outputFormatter.PrintText(" As ");
				if (templateDefinition.Bases.Count == 1) {
					nodeTracker.TrackedVisit(templateDefinition.Bases[0], data);
				} else {
					outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
					AppendCommaSeparatedList(templateDefinition.Bases);
					outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
				}
			}
			return null;
		}
		
		public object Visit(DelegateDeclaration delegateDeclaration, object data)
		{
			VisitAttributes(delegateDeclaration.Attributes, data);
			OutputModifier(delegateDeclaration.Modifier);
			outputFormatter.PrintToken(Tokens.Delegate);
			outputFormatter.Space();
			
			bool isFunction = (delegateDeclaration.ReturnType.Type != "void");
			if (isFunction) {
				outputFormatter.PrintToken(Tokens.Function);
				outputFormatter.Space();
			} else {
				outputFormatter.PrintToken(Tokens.Sub);
				outputFormatter.Space();
			}
			outputFormatter.PrintIdentifier(delegateDeclaration.Name);
			
			PrintTemplates(delegateDeclaration.Templates);
			
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(delegateDeclaration.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			
			if (isFunction) {
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.As);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(delegateDeclaration.ReturnType, data);
			}
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(OptionDeclaration optionDeclaration, object data)
		{
			outputFormatter.PrintToken(Tokens.Option);
			outputFormatter.Space();
			switch (optionDeclaration.OptionType) {
				case OptionType.Strict:
					outputFormatter.PrintToken(Tokens.Strict);
					if (!optionDeclaration.OptionValue) {
						outputFormatter.Space();
						outputFormatter.PrintToken(Tokens.Off);
					}
					break;
				case OptionType.Explicit:
					outputFormatter.PrintToken(Tokens.Explicit);
					outputFormatter.Space();
					if (!optionDeclaration.OptionValue) {
						outputFormatter.Space();
						outputFormatter.PrintToken(Tokens.Off);
					}
					break;
				case OptionType.CompareBinary:
					outputFormatter.PrintToken(Tokens.Compare);
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.Binary);
					break;
				case OptionType.CompareText:
					outputFormatter.PrintToken(Tokens.Compare);
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.Text);
					break;
			}
			outputFormatter.NewLine();
			return null;
		}
		#endregion
		
		#region Type level
		TypeReference currentVariableType;
		public object Visit(FieldDeclaration fieldDeclaration, object data)
		{
			
			VisitAttributes(fieldDeclaration.Attributes, data);
			outputFormatter.Indent();
			if (fieldDeclaration.Modifier == Modifier.None) {
				outputFormatter.PrintToken(Tokens.Private);
				outputFormatter.Space();
			} else {
				OutputModifier(fieldDeclaration.Modifier);
			}
			currentVariableType = fieldDeclaration.TypeReference;
			AppendCommaSeparatedList(fieldDeclaration.Fields);
			currentVariableType = null;
			
			outputFormatter.NewLine();

			return null;
		}
		
		public object Visit(VariableDeclaration variableDeclaration, object data)
		{
			outputFormatter.PrintIdentifier(variableDeclaration.Name);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.As);
			outputFormatter.Space();
			
			if (variableDeclaration.TypeReference.IsNull && currentVariableType != null) {
				nodeTracker.TrackedVisit(currentVariableType, data);
			} else {
				nodeTracker.TrackedVisit(variableDeclaration.TypeReference, data);
			}
			
			if (!variableDeclaration.Initializer.IsNull) {
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.Assign);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(variableDeclaration.Initializer, data);
			}
			return null;
		}
		
		public object Visit(PropertyDeclaration propertyDeclaration, object data)
		{
			VisitAttributes(propertyDeclaration.Attributes, data);
			outputFormatter.Indent();
			OutputModifier(propertyDeclaration.Modifier);
			
			if (propertyDeclaration.IsReadOnly) {
				outputFormatter.PrintToken(Tokens.ReadOnly);
				outputFormatter.Space();
			} else if (propertyDeclaration.IsWriteOnly) {
				outputFormatter.PrintToken(Tokens.WriteOnly);
				outputFormatter.Space();
			}
			
			outputFormatter.PrintToken(Tokens.Property);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(propertyDeclaration.Name);
			
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(propertyDeclaration.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.As);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(propertyDeclaration.TypeReference, data);
			
			outputFormatter.NewLine();
			++outputFormatter.IndentationLevel;
			exitTokenStack.Push(Tokens.Property);
			nodeTracker.TrackedVisit(propertyDeclaration.GetRegion, data);
			nodeTracker.TrackedVisit(propertyDeclaration.SetRegion, data);
			exitTokenStack.Pop();
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Property);
			outputFormatter.Space();
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(PropertyGetRegion propertyGetRegion, object data)
		{
			VisitAttributes(propertyGetRegion.Attributes, data);
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Get);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			nodeTracker.TrackedVisit(propertyGetRegion.Block, data);
			--outputFormatter.IndentationLevel;
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Get);
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(PropertySetRegion propertySetRegion, object data)
		{
			VisitAttributes(propertySetRegion.Attributes, data);
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Set);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			nodeTracker.TrackedVisit(propertySetRegion.Block, data);
			--outputFormatter.IndentationLevel;
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Set);
			outputFormatter.NewLine();
			return null;
		}
		
		TypeReference currentEventType = null;
		public object Visit(EventDeclaration eventDeclaration, object data)
		{
			if (eventDeclaration.VariableDeclarators.Count > 0) {
				foreach (VariableDeclaration var in eventDeclaration.VariableDeclarators) {
					VisitAttributes(eventDeclaration.Attributes, data);
					outputFormatter.Indent();
					OutputModifier(eventDeclaration.Modifier);
					outputFormatter.PrintToken(Tokens.Event);
					outputFormatter.Space();
					outputFormatter.PrintIdentifier(var.Name);
					
					if (eventDeclaration.Parameters.Count > 0) {
						outputFormatter.PrintToken(Tokens.OpenParenthesis);
						this.AppendCommaSeparatedList(eventDeclaration.Parameters);
						outputFormatter.PrintToken(Tokens.CloseParenthesis);
					}
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.As);
					outputFormatter.Space();
					nodeTracker.TrackedVisit(eventDeclaration.TypeReference, data);
					outputFormatter.NewLine();
				}
				
			} else {
				bool customEvent = eventDeclaration.HasAddRegion  || eventDeclaration.HasRemoveRegion;
				
				VisitAttributes(eventDeclaration.Attributes, data);
				outputFormatter.Indent();
				OutputModifier(eventDeclaration.Modifier);
				if (customEvent) {
					outputFormatter.PrintText("Custom");
					outputFormatter.Space();
				}
				
				outputFormatter.PrintToken(Tokens.Event);
				outputFormatter.Space();
				outputFormatter.PrintIdentifier(eventDeclaration.Name);
				
				if (eventDeclaration.Parameters.Count > 0) {
					outputFormatter.PrintToken(Tokens.OpenParenthesis);
					this.AppendCommaSeparatedList(eventDeclaration.Parameters);
					outputFormatter.PrintToken(Tokens.CloseParenthesis);
				}
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.As);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(eventDeclaration.TypeReference, data);
				outputFormatter.NewLine();
				
				if (customEvent) {
					++outputFormatter.IndentationLevel;
					currentEventType = eventDeclaration.TypeReference;
					exitTokenStack.Push(Tokens.Sub);
					nodeTracker.TrackedVisit(eventDeclaration.AddRegion, data);
					nodeTracker.TrackedVisit(eventDeclaration.RemoveRegion, data);
					exitTokenStack.Pop();
					--outputFormatter.IndentationLevel;
					
					outputFormatter.Indent();
					outputFormatter.PrintToken(Tokens.End);
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.Event);
					outputFormatter.NewLine();
				}
			}
			
			return null;
		}
		
		public object Visit(EventAddRegion eventAddRegion, object data)
		{
			VisitAttributes(eventAddRegion.Attributes, data);
			outputFormatter.Indent();
			outputFormatter.PrintText("AddHandler(");
			if (eventAddRegion.Parameters.Count == 0) {
				outputFormatter.PrintToken(Tokens.ByVal);
				outputFormatter.Space();
				outputFormatter.PrintIdentifier("value");
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.As);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(currentEventType, data);
			} else {
				this.AppendCommaSeparatedList(eventAddRegion.Parameters);
			}
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			nodeTracker.TrackedVisit(eventAddRegion.Block, data);
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintText("AddHandler");
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(EventRemoveRegion eventRemoveRegion, object data)
		{
			VisitAttributes(eventRemoveRegion.Attributes, data);
			outputFormatter.Indent();
			outputFormatter.PrintText("RemoveHandler");
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			if (eventRemoveRegion.Parameters.Count == 0) {
				outputFormatter.PrintToken(Tokens.ByVal);
				outputFormatter.Space();
				outputFormatter.PrintIdentifier("value");
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.As);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(currentEventType, data);
			} else {
				this.AppendCommaSeparatedList(eventRemoveRegion.Parameters);
			}
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			nodeTracker.TrackedVisit(eventRemoveRegion.Block, data);
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintText("RemoveHandler");
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			VisitAttributes(parameterDeclarationExpression.Attributes, data);
			OutputModifier(parameterDeclarationExpression.ParamModifier);
			outputFormatter.PrintIdentifier(parameterDeclarationExpression.ParameterName);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.As);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(parameterDeclarationExpression.TypeReference, data);
			return null;
		}
		
		public object Visit(MethodDeclaration methodDeclaration, object data)
		{
			VisitAttributes(methodDeclaration.Attributes, data);
			outputFormatter.Indent();
			OutputModifier(methodDeclaration.Modifier);
			
			bool isSub = methodDeclaration.TypeReference.IsNull ||
				methodDeclaration.TypeReference.SystemType == "System.Void";
			
			if (isSub) {
				outputFormatter.PrintToken(Tokens.Sub);
			} else {
				outputFormatter.PrintToken(Tokens.Function);
			}
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(methodDeclaration.Name);
			
			PrintTemplates(methodDeclaration.Templates);
			
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(methodDeclaration.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			
			if (!isSub) {
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.As);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(methodDeclaration.TypeReference, data);
			}
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			exitTokenStack.Push(isSub ? Tokens.Sub : Tokens.Function);
			nodeTracker.TrackedVisit(methodDeclaration.Body, data);
			exitTokenStack.Pop();
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			if (isSub) {
				outputFormatter.PrintToken(Tokens.Sub);
			} else {
				outputFormatter.PrintToken(Tokens.Function);
			}
			outputFormatter.NewLine();
			
			return null;
		}
		
		public object Visit(ConstructorDeclaration constructorDeclaration, object data)
		{
			VisitAttributes(constructorDeclaration.Attributes, data);
			outputFormatter.Indent();
			OutputModifier(constructorDeclaration.Modifier);
			outputFormatter.PrintToken(Tokens.Sub);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.New);
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(constructorDeclaration.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			exitTokenStack.Push(Tokens.Sub);
			nodeTracker.TrackedVisit(constructorDeclaration.Body, data);
			exitTokenStack.Pop();
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Sub);
			outputFormatter.Space();
			outputFormatter.NewLine();
			
			return null;
		}
		
		public object Visit(ConstructorInitializer constructorInitializer, object data)
		{
			errors.Error(-1, -1, String.Format("ConstructorInitializer not supported."));
			return null;
		}
		
		public object Visit(IndexerDeclaration indexerDeclaration, object data)
		{
			VisitAttributes(indexerDeclaration.Attributes, data);
			outputFormatter.Indent();
			OutputModifier(indexerDeclaration.Modifier);
			outputFormatter.PrintToken(Tokens.Default);
			outputFormatter.Space();
			if (indexerDeclaration.IsReadOnly) {
				outputFormatter.PrintToken(Tokens.ReadOnly);
				outputFormatter.Space();
			} else if (indexerDeclaration.IsWriteOnly) {
				outputFormatter.PrintToken(Tokens.WriteOnly);
				outputFormatter.Space();
			}
			
			outputFormatter.PrintToken(Tokens.Property);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier("ConvertedIndexer");
			
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(indexerDeclaration.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.As);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(indexerDeclaration.TypeReference, data);
			
			outputFormatter.NewLine();
			++outputFormatter.IndentationLevel;
			exitTokenStack.Push(Tokens.Property);
			nodeTracker.TrackedVisit(indexerDeclaration.GetRegion, data);
			nodeTracker.TrackedVisit(indexerDeclaration.SetRegion, data);
			exitTokenStack.Pop();
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Property);
			outputFormatter.Space();
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(DestructorDeclaration destructorDeclaration, object data)
		{
			outputFormatter.Indent();
			
			outputFormatter.PrintToken(Tokens.Overrides);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Protected);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier("Finalize");
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			exitTokenStack.Push(Tokens.Sub);
			nodeTracker.TrackedVisit(destructorDeclaration.Body, data);
			exitTokenStack.Pop();
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Sub);
			outputFormatter.Space();
			outputFormatter.NewLine();
			
			return null;
		}
		
		public object Visit(OperatorDeclaration operatorDeclaration, object data)
		{
			// TODO: VB.NET 2.0 operators
			errors.Error(-1, -1, String.Format("operators not supported."));
			return null;
		}
		
		public object Visit(DeclareDeclaration declareDeclaration, object data)
		{
			VisitAttributes(declareDeclaration.Attributes, data);
			outputFormatter.Indent();
			OutputModifier(declareDeclaration.Modifier);
			outputFormatter.PrintToken(Tokens.Declare);
			outputFormatter.Space();
			
			switch (declareDeclaration.Charset) {
				case CharsetModifier.Auto:
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.Auto);
					outputFormatter.Space();
					break;
				case CharsetModifier.Unicode:
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.Unicode);
					outputFormatter.Space();
					break;
				case CharsetModifier.ANSI:
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.Ansi);
					outputFormatter.Space();
					break;
			}
			
			if (declareDeclaration.TypeReference.IsNull) {
				outputFormatter.PrintToken(Tokens.Sub);
			} else {
				outputFormatter.PrintToken(Tokens.Function);
			}
			outputFormatter.Space();
			
			outputFormatter.PrintIdentifier(declareDeclaration.Name);
			
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Lib);
			outputFormatter.Space();
			outputFormatter.PrintText('"' + declareDeclaration.Library + '"');
			outputFormatter.Space();
			
			if (declareDeclaration.Alias.Length > 0) {
				outputFormatter.PrintToken(Tokens.Alias);
				outputFormatter.Space();
				outputFormatter.PrintText('"' + declareDeclaration.Alias + '"');
				outputFormatter.Space();
			}
			
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(declareDeclaration.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			
			if (!declareDeclaration.TypeReference.IsNull) {
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.As);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(declareDeclaration.TypeReference, data);
			}
			
			outputFormatter.NewLine();
			
			return null;
		}
		#endregion
		
		#region Statements
		public object Visit(BlockStatement blockStatement, object data)
		{
			foreach (Statement stmt in blockStatement.Children) {
				outputFormatter.Indent();
				nodeTracker.TrackedVisit(stmt, data);
				outputFormatter.NewLine();
			}
			return null;
		}
		
		public object Visit(AddHandlerStatement addHandlerStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.AddHandler);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(addHandlerStatement.EventExpression, data);
			outputFormatter.PrintToken(Tokens.Comma);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(addHandlerStatement.HandlerExpression, data);
			return null;
		}
		
		public object Visit(RemoveHandlerStatement removeHandlerStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.RemoveHandler);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(removeHandlerStatement.EventExpression, data);
			outputFormatter.PrintToken(Tokens.Comma);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(removeHandlerStatement.HandlerExpression, data);
			return null;
		}
		
		public object Visit(RaiseEventStatement raiseEventStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.RaiseEvent);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(raiseEventStatement.EventName);
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(raiseEventStatement.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			return null;
		}
		
		public object Visit(EraseStatement eraseStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Erase);
			outputFormatter.Space();
			AppendCommaSeparatedList(eraseStatement.Expressions);
			return null;
		}
		
		public object Visit(ErrorStatement errorStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Error);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(errorStatement.Expression, data);
			return null;
		}
		
		public object Visit(OnErrorStatement onErrorStatement, object data)
		{
			// TODO: implement me!
			return null;
		}
		
		public object Visit(ReDimStatement reDimStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.ReDim);
			outputFormatter.Space();
			if (reDimStatement.IsPreserve) {
				outputFormatter.PrintToken(Tokens.Preserve);
				outputFormatter.Space();
			}
			
			AppendCommaSeparatedList(reDimStatement.ReDimClauses);
			return null;
		}
		
		public object Visit(StatementExpression statementExpression, object data)
		{
			nodeTracker.TrackedVisit(statementExpression.Expression, data);
			return null;
		}
		
		public object Visit(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			if (localVariableDeclaration.Modifier != Modifier.None) {
				OutputModifier(localVariableDeclaration.Modifier);
			}
			outputFormatter.PrintToken(Tokens.Dim);
			outputFormatter.Space();
			currentVariableType = localVariableDeclaration.TypeReference;
			
			AppendCommaSeparatedList(localVariableDeclaration.Variables);
			currentVariableType = null;
			
			return null;
		}
		
		public object Visit(EmptyStatement emptyStatement, object data)
		{
			outputFormatter.NewLine();
			return null;
		}
		
		public virtual object Visit(YieldStatement yieldStatement, object data)
		{
			Debug.Assert(yieldStatement != null);
			Debug.Assert(yieldStatement.Statement != null);
			// TODO: yieldStatement
			return null;
		}
		
		public object Visit(ReturnStatement returnStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Return);
			if (!returnStatement.Expression.IsNull) {
				outputFormatter.Space();
				nodeTracker.TrackedVisit(returnStatement.Expression, data);
			}
			return null;
		}
		
		public object Visit(IfElseStatement ifElseStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.If);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(ifElseStatement.Condition, data);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Then);
			outputFormatter.NewLine();
			++outputFormatter.IndentationLevel;
			foreach (Statement stmt in ifElseStatement.TrueStatement) {
				nodeTracker.TrackedVisit(stmt, data);
			}
			--outputFormatter.IndentationLevel;
			
			foreach (ElseIfSection elseIfSection in ifElseStatement.ElseIfSections) {
				nodeTracker.TrackedVisit(elseIfSection, data);
			}
			
			if (ifElseStatement.HasElseStatements) {
				outputFormatter.Indent();
				outputFormatter.PrintToken(Tokens.Else);
				outputFormatter.NewLine();
				++outputFormatter.IndentationLevel;
				foreach (Statement stmt in ifElseStatement.FalseStatement) {
					outputFormatter.Indent();
					nodeTracker.TrackedVisit(stmt, data);
					outputFormatter.NewLine();
				}
				--outputFormatter.IndentationLevel;
			}
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.If);
			return null;
		}
		
		public object Visit(ElseIfSection elseIfSection, object data)
		{
			outputFormatter.PrintToken(Tokens.ElseIf);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(elseIfSection.Condition, data);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Then);
			outputFormatter.NewLine();
			++outputFormatter.IndentationLevel;
			outputFormatter.Indent();
			nodeTracker.TrackedVisit(elseIfSection.EmbeddedStatement, data);
			outputFormatter.NewLine();
			--outputFormatter.IndentationLevel;
			return null;
		}
		
		public object Visit(ForStatement forStatement, object data)
		{ // Is converted to {initializer} while <Condition> {Embedded} {Iterators} end while
			exitTokenStack.Push(Tokens.While);
			outputFormatter.NewLine();
			foreach (INode node in forStatement.Initializers) {
				outputFormatter.Indent();
				nodeTracker.TrackedVisit(node, data);
				outputFormatter.NewLine();
			}
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.While);
			outputFormatter.Space();
			if (forStatement.Condition.IsNull) {
				outputFormatter.PrintToken(Tokens.True);
			} else {
				nodeTracker.TrackedVisit(forStatement.Condition, data);
			}
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			nodeTracker.TrackedVisit(forStatement.EmbeddedStatement, data);
			
			foreach (Statement stmt in forStatement.Iterator) {
				outputFormatter.Indent();
				nodeTracker.TrackedVisit(stmt, data);
				outputFormatter.NewLine();
			}
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.While);
			outputFormatter.NewLine();
			exitTokenStack.Pop();
			return null;
		}
		
		public object Visit(LabelStatement labelStatement, object data)
		{
			outputFormatter.PrintIdentifier(labelStatement.Label);
			outputFormatter.PrintToken(Tokens.Colon);
			return null;
		}
		
		public object Visit(GotoStatement gotoStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.GoTo);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(gotoStatement.Label);
			return null;
		}
		
		public object Visit(SwitchStatement switchStatement, object data)
		{
			exitTokenStack.Push(Tokens.Select);
			outputFormatter.PrintToken(Tokens.Select);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(switchStatement.SwitchExpression, data);
			outputFormatter.NewLine();
			++outputFormatter.IndentationLevel;
			foreach (SwitchSection section in switchStatement.SwitchSections) {
				nodeTracker.TrackedVisit(section, data);
			}
			--outputFormatter.IndentationLevel;
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Select);
			exitTokenStack.Pop();
			return null;
		}
		
		public object Visit(SwitchSection switchSection, object data)
		{
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Case);
			outputFormatter.Space();
			this.AppendCommaSeparatedList(switchSection.SwitchLabels);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			
			foreach (Statement stmt in switchSection.Children) {
				outputFormatter.Indent();
				nodeTracker.TrackedVisit(stmt, data);
				outputFormatter.NewLine();
			}
			--outputFormatter.IndentationLevel;
			return null;
		}
		
		public object Visit(CaseLabel caseLabel, object data)
		{
			if (caseLabel.IsDefault) {
				outputFormatter.PrintToken(Tokens.Else);
			} else {
				if (caseLabel.BinaryOperatorType != BinaryOperatorType.None) {
					switch (caseLabel.BinaryOperatorType) {
						case BinaryOperatorType.Equality:
							outputFormatter.PrintToken(Tokens.Assign);
							break;
						case BinaryOperatorType.InEquality:
							outputFormatter.PrintToken(Tokens.LessThan);
							outputFormatter.PrintToken(Tokens.GreaterThan);
							break;
							
						case BinaryOperatorType.GreaterThan:
							outputFormatter.PrintToken(Tokens.GreaterThan);
							break;
						case BinaryOperatorType.GreaterThanOrEqual:
							outputFormatter.PrintToken(Tokens.GreaterEqual);
							break;
						case BinaryOperatorType.LessThan:
							outputFormatter.PrintToken(Tokens.LessThan);
							break;
						case BinaryOperatorType.LessThanOrEqual:
							outputFormatter.PrintToken(Tokens.LessEqual);
							break;
					}
					outputFormatter.Space();
				}
				
				nodeTracker.TrackedVisit(caseLabel.Label, data);
				if (!caseLabel.ToExpression.IsNull) {
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.To);
					outputFormatter.Space();
					nodeTracker.TrackedVisit(caseLabel.ToExpression, data);
				}
			}
			
			return null;
		}
		
		public object Visit(BreakStatement breakStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Exit);
			if (exitTokenStack.Count > 0) {
				outputFormatter.Space();
				outputFormatter.PrintToken((int)exitTokenStack.Peek());
			}
			return null;
		}
		
		public object Visit(StopStatement stopStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Stop);
			return null;
		}
		
		public object Visit(ResumeStatement resumeStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Resume);
			outputFormatter.Space();
			if (resumeStatement.IsResumeNext) {
				outputFormatter.PrintToken(Tokens.Next);
			} else {
				outputFormatter.PrintIdentifier(resumeStatement.LabelName);
			}
			return null;
		}
		
		public object Visit(EndStatement endStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.End);
			return null;
		}
		
		public object Visit(ContinueStatement continueStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Continue);
			return null;
		}
		
		public object Visit(GotoCaseStatement gotoCaseStatement, object data)
		{
			outputFormatter.PrintText("goto case ");
			if (gotoCaseStatement.IsDefaultCase) {
				outputFormatter.PrintText("default");
			} else {
				nodeTracker.TrackedVisit(gotoCaseStatement.Expression, null);
			}
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(DoLoopStatement doLoopStatement, object data)
		{
			exitTokenStack.Push(Tokens.Do);
			if (doLoopStatement.ConditionPosition == ConditionPosition.None) {
				errors.Error(-1, -1, String.Format("Unknown condition position for loop : {0}.", doLoopStatement));
			}
			
			if (doLoopStatement.ConditionPosition == ConditionPosition.Start) {
				switch (doLoopStatement.ConditionType) {
					case ConditionType.DoWhile:
						outputFormatter.PrintToken(Tokens.Do);
						outputFormatter.Space();
						outputFormatter.PrintToken(Tokens.While);
						break;
					case ConditionType.While:
						outputFormatter.PrintToken(Tokens.While);
						break;
					case ConditionType.Until:
						outputFormatter.PrintToken(Tokens.Do);
						outputFormatter.Space();
						outputFormatter.PrintToken(Tokens.While);
						break;
				}
				outputFormatter.Space();
				nodeTracker.TrackedVisit(doLoopStatement.Condition, null);
			} else {
				outputFormatter.PrintToken(Tokens.Do);
			}
			
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			if (doLoopStatement.EmbeddedStatement is BlockStatement) {
				nodeTracker.TrackedVisit(doLoopStatement.EmbeddedStatement, false);
			} else {
				outputFormatter.Indent();
				nodeTracker.TrackedVisit(doLoopStatement.EmbeddedStatement, data);
				outputFormatter.NewLine();
			}
			--outputFormatter.IndentationLevel;
			
			outputFormatter.PrintToken(Tokens.Loop);
			
			if (doLoopStatement.ConditionPosition == ConditionPosition.End) {
				outputFormatter.Space();
				switch (doLoopStatement.ConditionType) {
					case ConditionType.While:
					case ConditionType.DoWhile:
						outputFormatter.PrintToken(Tokens.While);
						outputFormatter.Space();
						break;
					case ConditionType.Until:
						outputFormatter.PrintToken(Tokens.Until);
						outputFormatter.Space();
						break;
				}
				outputFormatter.Space();
				nodeTracker.TrackedVisit(doLoopStatement.Condition, null);
			}
			exitTokenStack.Pop();
			return null;
		}
		
		public object Visit(ForeachStatement foreachStatement, object data)
		{
			exitTokenStack.Push(Tokens.For);
			outputFormatter.PrintToken(Tokens.For);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Each);
			outputFormatter.Space();
			
			// loop control variable
			outputFormatter.PrintIdentifier(foreachStatement.VariableName);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.As);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(foreachStatement.TypeReference, data);
			
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.In);
			outputFormatter.Space();
			
			nodeTracker.TrackedVisit(foreachStatement.Expression, data);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			if (foreachStatement.EmbeddedStatement is BlockStatement) {
				nodeTracker.TrackedVisit(foreachStatement.EmbeddedStatement, false);
			} else {
				outputFormatter.Indent();
				nodeTracker.TrackedVisit(foreachStatement.EmbeddedStatement, data);
				outputFormatter.NewLine();
			}
			--outputFormatter.IndentationLevel;
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Next);
			if (!foreachStatement.NextExpression.IsNull) {
				outputFormatter.Space();
				nodeTracker.TrackedVisit(foreachStatement.NextExpression, data);
			}
			exitTokenStack.Pop();
			return null;
		}
		
		public object Visit(LockStatement lockStatement, object data)
		{
			errors.Error(-1, -1, String.Format("LockStatement is unsupported"));
			return null;
		}
		
		public object Visit(UsingStatement usingStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Using);
			outputFormatter.Space();
			
			nodeTracker.TrackedVisit(usingStatement.ResourceAcquisition, data);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			nodeTracker.TrackedVisit(usingStatement.EmbeddedStatement, data);
			--outputFormatter.IndentationLevel;
			outputFormatter.NewLine();
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Using);
			outputFormatter.Space();
			outputFormatter.NewLine();
			
			return null;
		}
		
		public object Visit(WithStatement withStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.With);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(withStatement.Expression, data);
			outputFormatter.NewLine();
			
			nodeTracker.TrackedVisit(withStatement.Body, data);
			outputFormatter.NewLine();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.With);
			outputFormatter.Space();
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(TryCatchStatement tryCatchStatement, object data)
		{
			exitTokenStack.Push(Tokens.Try);
			outputFormatter.PrintToken(Tokens.Try);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			nodeTracker.TrackedVisit(tryCatchStatement.StatementBlock, data);
			--outputFormatter.IndentationLevel;
			
			foreach (CatchClause catchClause in tryCatchStatement.CatchClauses) {
				nodeTracker.TrackedVisit(catchClause, data);
			}
			
			if (!tryCatchStatement.FinallyBlock.IsNull) {
				outputFormatter.Indent();
				outputFormatter.PrintToken(Tokens.Finally);
				outputFormatter.NewLine();
				++outputFormatter.IndentationLevel;
				nodeTracker.TrackedVisit(tryCatchStatement.FinallyBlock, data);
				--outputFormatter.IndentationLevel;
			}
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.End);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Try);
			outputFormatter.NewLine();
			exitTokenStack.Pop();
			return null;
		}
		
		public object Visit(CatchClause catchClause, object data)
		{
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Catch);
			
			if (!catchClause.TypeReference.IsNull) {
				outputFormatter.Space();
				if (catchClause.VariableName.Length > 0) {
					outputFormatter.PrintIdentifier(catchClause.VariableName);
				} else {
					outputFormatter.PrintIdentifier("generatedExceptionName");
				}
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.As);
				outputFormatter.Space();
				outputFormatter.PrintIdentifier(catchClause.TypeReference.Type);
			}
			
			if (!catchClause.Condition.IsNull)  {
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.When);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(catchClause.Condition, data);
			}
			outputFormatter.NewLine();

			++outputFormatter.IndentationLevel;
			nodeTracker.TrackedVisit(catchClause.StatementBlock, data);
			--outputFormatter.IndentationLevel;
			
			return null;
		}
		
		public object Visit(ThrowStatement throwStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Throw);
			if (!throwStatement.Expression.IsNull) {
				outputFormatter.Space();
				nodeTracker.TrackedVisit(throwStatement.Expression, data);
			}
			return null;
		}
		
		public object Visit(FixedStatement fixedStatement, object data)
		{
			errors.Error(-1, -1, String.Format("FixedStatement is unsupported"));
			return null;
		}
		
		public object Visit(UnsafeStatement unsafeStatement, object data)
		{
			errors.Error(-1, -1, String.Format("UnsafeStatement is unsupported"));
			return null;
		}
		
		public object Visit(CheckedStatement checkedStatement, object data)
		{
			errors.Error(-1, -1, String.Format("CheckedStatement is unsupported"));
			return null;
		}
		
		public object Visit(UncheckedStatement uncheckedStatement, object data)
		{
			errors.Error(-1, -1, String.Format("UncheckedStatement is unsupported"));
			return null;
		}
		
		public object Visit(ExitStatement exitStatement, object data)
		{
			outputFormatter.PrintToken(Tokens.Exit);
			if (exitStatement.ExitType != ExitType.None) {
				outputFormatter.Space();
				switch (exitStatement.ExitType) {
					case ExitType.Sub:
						outputFormatter.PrintToken(Tokens.Sub);
						break;
					case ExitType.Function:
						outputFormatter.PrintToken(Tokens.Function);
						break;
					case ExitType.Property:
						outputFormatter.PrintToken(Tokens.Property);
						break;
					case ExitType.Do:
						outputFormatter.PrintToken(Tokens.Do);
						break;
					case ExitType.For:
						outputFormatter.PrintToken(Tokens.For);
						break;
					case ExitType.Try:
						outputFormatter.PrintToken(Tokens.Try);
						break;
					case ExitType.While:
						outputFormatter.PrintToken(Tokens.While);
						break;
					case ExitType.Select:
						outputFormatter.PrintToken(Tokens.Select);
						break;
					default:
						errors.Error(-1, -1, String.Format("Unsupported exit type : {0}", exitStatement.ExitType));
						break;
				}
			}
			
			return null;
		}
		
		public object Visit(ForNextStatement forNextStatement, object data)
		{
			exitTokenStack.Push(Tokens.For);
			outputFormatter.PrintToken(Tokens.For);
			outputFormatter.Space();
			
			outputFormatter.PrintIdentifier(forNextStatement.VariableName);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.As);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(forNextStatement.TypeReference, data);
			
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Assign);
			outputFormatter.Space();
			
			nodeTracker.TrackedVisit(forNextStatement.Start, data);
			
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.To);
			outputFormatter.Space();
			
			nodeTracker.TrackedVisit(forNextStatement.End, data);
			
			if (!forNextStatement.Step.IsNull) {
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.Step);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(forNextStatement.Step, data);
			}
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			if (forNextStatement.EmbeddedStatement is BlockStatement) {
				nodeTracker.TrackedVisit(forNextStatement.EmbeddedStatement, false);
			} else {
				outputFormatter.Indent();
				nodeTracker.TrackedVisit(forNextStatement.EmbeddedStatement, data);
				
			}
			--outputFormatter.IndentationLevel;
			outputFormatter.NewLine();
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Next);
			
			if (forNextStatement.NextExpressions.Count > 0) {
				outputFormatter.Space();
				AppendCommaSeparatedList(forNextStatement.NextExpressions);
			}
			exitTokenStack.Pop();
			return null;
		}
		#endregion
		
		#region Expressions
		
		public object Visit(ClassReferenceExpression classReferenceExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.MyClass);
			return null;
		}
		
		
		string ConvertCharLiteral(char ch)
		{
			if (Char.IsControl(ch)) {
				return "Microsoft.VisualBasic.Chr(" + ((int)ch) + ")";
			} else {
				if (ch == '"') {
					return "\"\"\"\"C";
				}
				return String.Concat("\"", ch.ToString(), "\"C");
			}
		}
		
		string ConvertString(string str)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char ch in str) {
				if (char.IsControl(ch)) {
					sb.Append("\" & Microsoft.VisualBasic.Chr(" + ((int)ch) + ") & \"");
				} else if (ch == '"') {
					sb.Append("\"\"");
				} else {
					sb.Append(ch);
				}
			}
			return sb.ToString();
		}
		
		public object Visit(PrimitiveExpression primitiveExpression, object data)
		{
			if (primitiveExpression.Value == null) {
				outputFormatter.PrintToken(Tokens.Nothing);
				return null;
			}
			if (primitiveExpression.Value is bool) {
				if ((bool)primitiveExpression.Value) {
					outputFormatter.PrintToken(Tokens.True);
				} else {
					outputFormatter.PrintToken(Tokens.False);
				}
				return null;
			}
			
			if (primitiveExpression.Value is string) {
				outputFormatter.PrintText('"' + ConvertString(primitiveExpression.Value.ToString()) + '"');
				return null;
			}
			
			if (primitiveExpression.Value is char) {
				outputFormatter.PrintText(ConvertCharLiteral((char)primitiveExpression.Value));
				return null;
			}

			if (primitiveExpression.Value is decimal) {
				outputFormatter.PrintText(((decimal)primitiveExpression.Value).ToString(NumberFormatInfo.InvariantInfo) + "D");
				return null;
			}
			
			if (primitiveExpression.Value is float) {
				outputFormatter.PrintText(((float)primitiveExpression.Value).ToString(NumberFormatInfo.InvariantInfo) + "F");
				return null;
			}
			
			outputFormatter.PrintIdentifier(primitiveExpression.Value.ToString());
			return null;
		}
		
		public object Visit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
			int  op = 0;
			switch (binaryOperatorExpression.Op) {
				case BinaryOperatorType.Add:
					op = Tokens.Plus;
					break;
					
				case BinaryOperatorType.Subtract:
					op = Tokens.Minus;
					break;
					
				case BinaryOperatorType.Multiply:
					op = Tokens.Times;
					break;
					
				case BinaryOperatorType.Divide:
					op = Tokens.Div;
					break;
					
				case BinaryOperatorType.Modulus:
					op = Tokens.Mod;
					break;
					
				case BinaryOperatorType.ShiftLeft:
					op = Tokens.ShiftLeft;
					break;
					
				case BinaryOperatorType.ShiftRight:
					op = Tokens.ShiftRight;
					break;
					
				case BinaryOperatorType.BitwiseAnd:
					op = Tokens.And;
					break;
				case BinaryOperatorType.BitwiseOr:
					op = Tokens.Or;
					break;
				case BinaryOperatorType.ExclusiveOr:
					op = Tokens.Xor;
					break;
					
				case BinaryOperatorType.LogicalAnd:
					op = Tokens.AndAlso;
					break;
				case BinaryOperatorType.LogicalOr:
					op = Tokens.OrElse;
					break;
				case BinaryOperatorType.ReferenceEquality:
					op = Tokens.Is;
					break;
				case BinaryOperatorType.ReferenceInequality:
					op = Tokens.IsNot;
					break;
					
				case BinaryOperatorType.AsCast:
					outputFormatter.PrintText("TryCast(");
					nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
					outputFormatter.PrintText(", ");
					nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
					outputFormatter.PrintText(")");
					return null;
				case BinaryOperatorType.TypeCheck:
					outputFormatter.PrintText("TypeOf ");
					nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
					outputFormatter.PrintText(" Is ");
					nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
					return null;
					
				case BinaryOperatorType.Equality:
					op = Tokens.Assign;
					break;
				case BinaryOperatorType.GreaterThan:
					op = Tokens.GreaterThan;
					break;
				case BinaryOperatorType.GreaterThanOrEqual:
					op = Tokens.GreaterEqual;
					break;
				case BinaryOperatorType.InEquality:
					nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.LessThan);
					outputFormatter.PrintToken(Tokens.GreaterThan);
					outputFormatter.Space();
					nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
					return null;
				case BinaryOperatorType.LessThan:
					op = Tokens.LessThan;
					break;
				case BinaryOperatorType.LessThanOrEqual:
					op = Tokens.LessEqual;
					break;
			}
			
			nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
			outputFormatter.Space();
			outputFormatter.PrintToken(op);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
			
			return null;
		}
		
		public object Visit(ParenthesizedExpression parenthesizedExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			nodeTracker.TrackedVisit(parenthesizedExpression.Expression, data);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			return null;
		}
		
		public object Visit(InvocationExpression invocationExpression, object data)
		{
			nodeTracker.TrackedVisit(invocationExpression.TargetObject, data);
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(invocationExpression.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			return null;
		}
		
		public object Visit(IdentifierExpression identifierExpression, object data)
		{
			outputFormatter.PrintIdentifier(identifierExpression.Identifier);
			return null;
		}
		
		public object Visit(TypeReferenceExpression typeReferenceExpression, object data)
		{
			nodeTracker.TrackedVisit(typeReferenceExpression.TypeReference, data);
			return null;
		}
		
		public object Visit(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			switch (unaryOperatorExpression.Op) {
				case UnaryOperatorType.Not:
				case UnaryOperatorType.BitNot:
					outputFormatter.PrintToken(Tokens.Not);
					outputFormatter.Space();
					nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
					return null;
					
				case UnaryOperatorType.Decrement:
					outputFormatter.PrintText("System.Threading.Interlocked.Decrement(");
					nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
					outputFormatter.PrintText(")");
					return null;
					
				case UnaryOperatorType.Increment:
					outputFormatter.PrintText("System.Threading.Interlocked.Increment(");
					nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
					outputFormatter.PrintText(")");
					return null;
					
				case UnaryOperatorType.Minus:
					outputFormatter.PrintToken(Tokens.Minus);
					outputFormatter.Space();
					nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
					return null;
					
				case UnaryOperatorType.Plus:
					outputFormatter.PrintToken(Tokens.Plus);
					outputFormatter.Space();
					nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
					return null;
					
				case UnaryOperatorType.PostDecrement:
					outputFormatter.PrintText("System.Math.Max(System.Threading.Interlocked.Decrement(");
					nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
					outputFormatter.PrintText("),");
					nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
					outputFormatter.PrintText(" + 1)");
					return null;
					
				case UnaryOperatorType.PostIncrement:
					outputFormatter.PrintText("System.Math.Max(System.Threading.Interlocked.Increment(");
					nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
					outputFormatter.PrintText("),");
					nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
					outputFormatter.PrintText(" - 1)");
					return null;
					
				case UnaryOperatorType.Star:
				case UnaryOperatorType.BitWiseAnd:
					break;
			}
			throw new System.NotSupportedException();
		}
		
		public object Visit(AssignmentExpression assignmentExpression, object data)
		{
			int  op = 0;
			bool unsupportedOpAssignment = false;
			switch (assignmentExpression.Op) {
				case AssignmentOperatorType.Assign:
					op = Tokens.Assign;
					break;
				case AssignmentOperatorType.Add:
					op = Tokens.PlusAssign;
					if (IsEventHandlerCreation(assignmentExpression.Right)) {
						outputFormatter.PrintToken(Tokens.AddHandler);
						outputFormatter.Space();
						nodeTracker.TrackedVisit(assignmentExpression.Left, data);
						outputFormatter.PrintToken(Tokens.Comma);
						outputFormatter.Space();
						outputFormatter.PrintToken(Tokens.AddressOf);
						outputFormatter.Space();
						nodeTracker.TrackedVisit(assignmentExpression.Right, data);
						return null;
					}
					break;
				case AssignmentOperatorType.Subtract:
					op = Tokens.MinusAssign;
					if (IsEventHandlerCreation(assignmentExpression.Right)) {
						outputFormatter.PrintToken(Tokens.RemoveHandler);
						outputFormatter.Space();
						nodeTracker.TrackedVisit(assignmentExpression.Left, data);
						outputFormatter.PrintToken(Tokens.Comma);
						outputFormatter.Space();
						outputFormatter.PrintToken(Tokens.AddressOf);
						outputFormatter.Space();
						nodeTracker.TrackedVisit(assignmentExpression.Right, data);
						return null;
					}
					break;
				case AssignmentOperatorType.Multiply:
					op = Tokens.TimesAssign;
					break;
				case AssignmentOperatorType.Divide:
					op = Tokens.DivAssign;
					break;
				case AssignmentOperatorType.ShiftLeft:
					op = Tokens.ShiftLeftAssign;
					break;
				case AssignmentOperatorType.ShiftRight:
					op = Tokens.ShiftRightAssign;
					break;
					
				case AssignmentOperatorType.ExclusiveOr:
					op = Tokens.Xor;
					unsupportedOpAssignment = true;
					break;
				case AssignmentOperatorType.Modulus:
					op = Tokens.Mod;
					unsupportedOpAssignment = true;
					break;
				case AssignmentOperatorType.BitwiseAnd:
					op = Tokens.And;
					unsupportedOpAssignment = true;
					break;
				case AssignmentOperatorType.BitwiseOr:
					op = Tokens.Or;
					unsupportedOpAssignment = true;
					break;
			}
			
			nodeTracker.TrackedVisit(assignmentExpression.Left, data);
			outputFormatter.Space();
			
			if (unsupportedOpAssignment) { // left = left OP right
				outputFormatter.PrintToken(Tokens.Assign);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(assignmentExpression.Left, data);
				outputFormatter.Space();
			}
			
			outputFormatter.PrintToken(op);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(assignmentExpression.Right, data);
			
			return null;
		}
		
		public object Visit(SizeOfExpression sizeOfExpression, object data)
		{
			errors.Error(-1, -1, String.Format("SizeOfExpression is unsupported"));
			return null;
		}
		
		public object Visit(TypeOfExpression typeOfExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.GetType);
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			nodeTracker.TrackedVisit(typeOfExpression.TypeReference, data);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			return null;
		}
		
		public object Visit(TypeOfIsExpression typeOfIsExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.TypeOf);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(typeOfIsExpression.TypeReference, data);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Is);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(typeOfIsExpression.Expression, data);
			return null;
		}
		
		public object Visit(AddressOfExpression addressOfExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.AddressOf);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(addressOfExpression.Expression, data);
			return null;
		}
		
		public object Visit(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			errors.Error(-1, -1, String.Format("AnonymousMethodExpression is unsupported"));
			return null;
		}
		
		public object Visit(CheckedExpression checkedExpression, object data)
		{
			errors.Error(-1, -1, String.Format("CheckedExpression is unsupported"));
			return null;
		}
		
		public object Visit(UncheckedExpression uncheckedExpression, object data)
		{
			errors.Error(-1, -1, String.Format("UncheckedExpression is unsupported"));
			return null;
		}
		
		public object Visit(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			errors.Error(-1, -1, String.Format("PointerReferenceExpression is unsupported"));
			return null;
		}
		
		public object Visit(CastExpression castExpression, object data)
		{
			if (castExpression.IsSpecializedCast) {
				switch (castExpression.CastTo.Type) {
					case "System.Boolean":
						outputFormatter.PrintToken(Tokens.CBool);
						break;
					case "System.Byte":
						outputFormatter.PrintToken(Tokens.CByte);
						break;
					case "System.Char":
						outputFormatter.PrintToken(Tokens.CChar);
						break;
					case "System.DateTime":
						outputFormatter.PrintToken(Tokens.CDate);
						break;
					case "System.Decimal":
						outputFormatter.PrintToken(Tokens.CDec);
						break;
					case "System.Double":
						outputFormatter.PrintToken(Tokens.CDbl);
						break;
					case "System.Int32":
						outputFormatter.PrintToken(Tokens.CInt);
						break;
					case "System.Int64":
						outputFormatter.PrintToken(Tokens.CLng);
						break;
					case "System.Object":
						outputFormatter.PrintToken(Tokens.CObj);
						break;
					case "System.Int16":
						outputFormatter.PrintToken(Tokens.CShort);
						break;
					case "System.Single":
						outputFormatter.PrintToken(Tokens.CSng);
						break;
					case "System.String":
						outputFormatter.PrintToken(Tokens.CStr);
						break;
					default:
						errors.Error(-1, -1, String.Format("Specialized cast of type {0} is unsupported", castExpression.CastTo.Type));
						break;
				}
				outputFormatter.PrintToken(Tokens.OpenParenthesis);
				nodeTracker.TrackedVisit(castExpression.Expression, data);
				outputFormatter.PrintToken(Tokens.CloseParenthesis);
			} else {
				outputFormatter.PrintToken(Tokens.CType);
				outputFormatter.PrintToken(Tokens.OpenParenthesis);
				nodeTracker.TrackedVisit(castExpression.Expression, data);
				outputFormatter.PrintToken(Tokens.Comma);
				outputFormatter.Space();
				nodeTracker.TrackedVisit(castExpression.CastTo, data);
				outputFormatter.PrintToken(Tokens.CloseParenthesis);
			}
			return null;
		}
		
		public object Visit(StackAllocExpression stackAllocExpression, object data)
		{
			errors.Error(-1, -1, String.Format("StackAllocExpression is unsupported"));
			return null;
		}
		
		public object Visit(IndexerExpression indexerExpression, object data)
		{
			nodeTracker.TrackedVisit(indexerExpression.TargetObject, data);
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(indexerExpression.Indices);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			return null;
		}
		
		public object Visit(ThisReferenceExpression thisReferenceExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.Me);
			return null;
		}
		
		public object Visit(BaseReferenceExpression baseReferenceExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.MyBase);
			return null;
		}
		
		public object Visit(GlobalReferenceExpression globalReferenceExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.Global);
			outputFormatter.PrintToken(Tokens.Dot);
			return null;
		}
		
		public object Visit(ObjectCreateExpression objectCreateExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.New);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(objectCreateExpression.CreateType, data);
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(objectCreateExpression.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			return null;
		}
		
		public object Visit(ArrayCreateExpression arrayCreateExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.New);
			outputFormatter.Space();
			nodeTracker.TrackedVisit(arrayCreateExpression.CreateType, data);
			
			for (int i = 0; i < arrayCreateExpression.Parameters.Count; ++i) {
				outputFormatter.PrintToken(Tokens.OpenParenthesis);
				nodeTracker.TrackedVisit((INode)arrayCreateExpression.Parameters[i], data);
				outputFormatter.PrintToken(Tokens.CloseParenthesis);
			}
			
			if (!arrayCreateExpression.ArrayInitializer.IsNull) {
				outputFormatter.Space();
				nodeTracker.TrackedVisit(arrayCreateExpression.ArrayInitializer, data);
			}
			return null;
		}
		
		public object Visit(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
			this.AppendCommaSeparatedList(arrayInitializerExpression.CreateExpressions);
			outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
			return null;
		}
		
		public object Visit(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			nodeTracker.TrackedVisit(fieldReferenceExpression.TargetObject, data);
			outputFormatter.PrintToken(Tokens.Dot);
			outputFormatter.PrintIdentifier(fieldReferenceExpression.FieldName);
			return null;
		}
		
		public object Visit(DirectionExpression directionExpression, object data)
		{
			// TODO: is this correct for VB ? (ref/out parameters in method invocation)
			nodeTracker.TrackedVisit(directionExpression.Expression, data);
			return null;
		}
		
		
		public object Visit(ConditionalExpression conditionalExpression, object data)
		{
			// No representation in VB.NET, but VB conversion is possible.
			outputFormatter.PrintText("Microsoft.VisualBasic.IIf");
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			nodeTracker.TrackedVisit(conditionalExpression.Condition, data);
			outputFormatter.PrintToken(Tokens.Comma);
			nodeTracker.TrackedVisit(conditionalExpression.TrueExpression, data);
			outputFormatter.PrintToken(Tokens.Comma);
			nodeTracker.TrackedVisit(conditionalExpression.FalseExpression, data);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			return null;
		}
		
		public object Visit(ArrayCreationParameter arrayCreationParameter, object data)
		{
			if (arrayCreationParameter.IsExpressionList) {
				AppendCommaSeparatedList(arrayCreationParameter.Expressions);
			} else {
				for (int j = 0; j < arrayCreationParameter.Dimensions; ++j) {
					outputFormatter.PrintToken(Tokens.Comma);
				}
			}
			return null;
		}
		#endregion
		#endregion
		
		
		void OutputModifier(ParamModifier modifier)
		{
			switch (modifier) {
				case ParamModifier.None:
				case ParamModifier.In:
					outputFormatter.PrintToken(Tokens.ByVal);
					break;
				case ParamModifier.Out:
					errors.Error(-1, -1, String.Format("Out parameter converted to ByRef"));
					outputFormatter.PrintToken(Tokens.ByRef);
					break;
				case ParamModifier.Params:
					outputFormatter.PrintToken(Tokens.ParamArray);
					break;
				case ParamModifier.Ref:
					outputFormatter.PrintToken(Tokens.ByRef);
					break;
				case ParamModifier.Optional:
					outputFormatter.PrintToken(Tokens.Optional);
					break;
				default:
					errors.Error(-1, -1, String.Format("Unsupported modifier : {0}", modifier));
					break;
			}
			outputFormatter.Space();
		}
		
		void OutputModifier(Modifier modifier)
		{
			if ((modifier & Modifier.Public) == Modifier.Public) {
				outputFormatter.PrintToken(Tokens.Public);
				outputFormatter.Space();
			} else if ((modifier & Modifier.Private) == Modifier.Private) {
				outputFormatter.PrintToken(Tokens.Private);
				outputFormatter.Space();
			} else if ((modifier & (Modifier.Protected | Modifier.Internal)) == (Modifier.Protected | Modifier.Internal)) {
				outputFormatter.PrintToken(Tokens.Protected);
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.Friend);
				outputFormatter.Space();
			} else if ((modifier & Modifier.Internal) == Modifier.Internal) {
				outputFormatter.PrintToken(Tokens.Friend);
				outputFormatter.Space();
			} else if ((modifier & Modifier.Protected) == Modifier.Protected) {
				outputFormatter.PrintToken(Tokens.Protected);
				outputFormatter.Space();
			}
			
			if ((modifier & Modifier.Static) == Modifier.Static) {
				outputFormatter.PrintToken(Tokens.Shared);
				outputFormatter.Space();
			}
			if ((modifier & Modifier.Virtual) == Modifier.Virtual) {
				outputFormatter.PrintToken(Tokens.Overridable);
				outputFormatter.Space();
			}
			if ((modifier & Modifier.Abstract) == Modifier.Abstract) {
				outputFormatter.PrintToken(Tokens.MustOverride);
				outputFormatter.Space();
			}
			if ((modifier & Modifier.Override) == Modifier.Override) {
				outputFormatter.PrintToken(Tokens.Overloads);
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.Overrides);
				outputFormatter.Space();
			}
			if ((modifier & Modifier.New) == Modifier.New) {
				outputFormatter.PrintToken(Tokens.Shadows);
				outputFormatter.Space();
			}
			
			if ((modifier & Modifier.Sealed) == Modifier.Sealed) {
				outputFormatter.PrintToken(Tokens.NotInheritable);
				outputFormatter.Space();
			}
			
			if ((modifier & Modifier.Readonly) == Modifier.Readonly) {
				outputFormatter.PrintToken(Tokens.ReadOnly);
				outputFormatter.Space();
			}
			if ((modifier & Modifier.Const) == Modifier.Const) {
				outputFormatter.PrintToken(Tokens.Const);
				outputFormatter.Space();
			}
			
			// TODO : Extern
			if ((modifier & Modifier.Extern) == Modifier.Extern) {
				errors.Error(-1, -1, String.Format("'Extern' modifier not convertable"));
			}
			
			// TODO : Volatile
			if ((modifier & Modifier.Volatile) == Modifier.Volatile) {
				errors.Error(-1, -1, String.Format("'Volatile' modifier not convertable"));
			}
			
			// TODO : Unsafe
			if ((modifier & Modifier.Unsafe) == Modifier.Unsafe) {
				errors.Error(-1, -1, String.Format("'Unsafe' modifier not convertable"));
			}
		}
		
		void AppendCommaSeparatedList(IList list)
		{
			if (list != null) {
				for (int i = 0; i < list.Count; ++i) {
					nodeTracker.TrackedVisit(((INode)list[i]), null);
					if (i + 1 < list.Count) {
						outputFormatter.PrintToken(Tokens.Comma);
						outputFormatter.Space();
					}
					if ((i + 1) % 6 == 0) {
						outputFormatter.PrintText("_ ");
						outputFormatter.NewLine();
						outputFormatter.Indent();
						outputFormatter.PrintText("\t");
					}
				}
			}
		}
		
		void VisitAttributes(ICollection attributes, object data)
		{
			if (attributes == null || attributes.Count <= 0) {
				return;
			}
			foreach (AttributeSection section in attributes) {
				nodeTracker.TrackedVisit(section, data);
			}
		}
		
		
		bool IsEventHandlerCreation(Expression expr)
		{
			if (expr is ObjectCreateExpression) {
				ObjectCreateExpression oce = (ObjectCreateExpression) expr;
				if (oce.Parameters.Count == 1) {
					expr = (Expression)oce.Parameters[0];
					string methodName = null;
					if (expr is IdentifierExpression) {
						methodName = ((IdentifierExpression)expr).Identifier;
					} else if (expr is FieldReferenceExpression) {
						methodName = ((FieldReferenceExpression)expr).FieldName;
					}
					if (methodName != null) {
						foreach (object o in this.currentType.Children) {
							if (o is MethodDeclaration && ((MethodDeclaration)o).Name == methodName) {
								return true;
							}
						}
					}
					
				}
			}
			
			return false;
		}

		bool TypeHasOnlyStaticMembers(TypeDeclaration typeDeclaration)
		{
			foreach (object o in typeDeclaration.Children) {
				if (o is MethodDeclaration) {
					if ((((MethodDeclaration)o).Modifier & Modifier.Static) != Modifier.Static) {
						return false;
					}
				} else if (o is PropertyDeclaration) {
					if ((((PropertyDeclaration)o).Modifier & Modifier.Static) != Modifier.Static) {
						return false;
					}
				} else if (o is FieldDeclaration) {
					if ((((FieldDeclaration)o).Modifier & Modifier.Static) != Modifier.Static) {
						return false;
					}
				} else if (o is EventDeclaration) {
					if ((((EventDeclaration)o).Modifier & Modifier.Static) != Modifier.Static) {
						return false;
					}
				}
			}
			return true;
		}
	}
}
