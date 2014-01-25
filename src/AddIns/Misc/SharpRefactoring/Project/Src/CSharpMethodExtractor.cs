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
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;
using SharpRefactoring.Visitors;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace SharpRefactoring
{
	public class CSharpMethodExtractor : MethodExtractorBase
	{
		static readonly StringComparer CSharpNameComparer = StringComparer.Ordinal;
		
		public CSharpMethodExtractor(ITextEditor textEditor)
			: base(textEditor)
		{
		}
		
		protected override string GenerateCode(INode unit, bool installSpecials)
		{
			CSharpOutputVisitor visitor = new CSharpOutputVisitor();
			
			if (installSpecials) {
				SpecialNodesInserter.Install(this.specialsList, visitor);
			}
			
			unit.AcceptVisitor(visitor, null);
			return visitor.Text;
		}
		
		public override bool Extract()
		{
			using (var parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader("class Tmp { void Test() {\n " + this.textEditor.SelectedText + "\n}}"))) {
				parser.Parse();
				
				if (parser.Errors.Count > 0) {
					MessageService.ShowError("${res:AddIns.SharpRefactoring.ExtractMethod.ParseErrors}");
					
					return false;
				}
				
				this.specialsList = parser.Lexer.SpecialTracker.RetrieveSpecials();
			}
			
			this.currentProjectContent = ParserService.GetProjectContent(ProjectService.CurrentProject);
			
			MethodDeclaration newMethod = new MethodDeclaration();
			List<VariableDeclaration> possibleReturnValues = new List<VariableDeclaration>();
			List<VariableDeclaration> otherReturnValues = new List<VariableDeclaration>();
			
			// Initialise new method
			newMethod.Body = GetBlock(this.textEditor.SelectedText);
			newMethod.Body.StartLocation = new Location(0,0);
			
			this.parentNode = GetParentMember(start, end);
			
			Dom.IMember member = GetParentMember(textEditor, textEditor.Caret.Line, textEditor.Caret.Column);
			
			if (parentNode == null || member == null) {
				MessageService.ShowError("${res:AddIns.SharpRefactoring.ExtractMethod.InvalidSelection}");
				return false;
			}
			
			this.currentClass = member.DeclaringType;
			
			ErrorKind kind = CheckForJumpInstructions(newMethod);
			if (kind != ErrorKind.None) {
				switch (kind) {
					case ErrorKind.ContainsBreak:
						MessageService.ShowError("${res:AddIns.SharpRefactoring.ExtractMethod.ContainsBreakError}");
						break;
					case ErrorKind.ContainsContinue:
						MessageService.ShowError("${res:AddIns.SharpRefactoring.ExtractMethod.ContainsContinueError}");
						break;
					case ErrorKind.ContainsGoto:
						MessageService.ShowError("${res:AddIns.SharpRefactoring.ExtractMethod.ContainsGotoError}");
						break;
				}
				return false;
			}
			
			newMethod.Modifier = parentNode.Modifier;
			
			newMethod.Modifier &= ~(Modifiers.Internal | Modifiers.Protected | Modifiers.Private | Modifiers.Public | Modifiers.Override);
			
			LookupTableVisitor ltv = new LookupTableVisitor(SupportedLanguage.CSharp);
			
			parentNode.AcceptVisitor(ltv, null);
			
			var variablesList = (from list in ltv.Variables.Values from item in list select new Variable(item))
				.Where(v => !(v.StartPos > end || v.EndPos < start) &&
				       (HasReferencesInSelection(newMethod, v) || 
				        HasOccurrencesAfter(CSharpNameComparer, this.parentNode, end, v.Name, v.StartPos, v.EndPos)))
				.Union(FromParameters(newMethod))
				.Select(va => ResolveVariable(va));
			
			foreach (var variable in variablesList) {
				LoggingService.Debug(variable);
				
				bool hasOccurrencesAfter = HasOccurrencesAfter(CSharpNameComparer, this.parentNode, end, variable.Name, variable.StartPos, variable.EndPos);
				bool isInitialized = (variable.Initializer != null) ? !variable.Initializer.IsNull : false;
				bool hasAssignment = HasAssignment(newMethod, variable);
				
				if (IsInCurrentSelection(variable.StartPos) && hasOccurrencesAfter) {
					possibleReturnValues.Add(new VariableDeclaration(variable.Name, variable.Initializer, variable.Type));
					otherReturnValues.Add(new VariableDeclaration(variable.Name, variable.Initializer, variable.Type));
				}
				
				if (!(IsInCurrentSelection(variable.StartPos) || IsInCurrentSelection(variable.EndPos))) {
					ParameterDeclarationExpression newParam = null;

					if ((hasOccurrencesAfter && isInitialized) || variable.WasRefParam)
						newParam = new ParameterDeclarationExpression(variable.Type, variable.Name, ParameterModifiers.Ref);
					else {
						if ((hasOccurrencesAfter && hasAssignment) || variable.WasOutParam)
							newParam = new ParameterDeclarationExpression(variable.Type, variable.Name, ParameterModifiers.Out);
						else {
							if (!hasOccurrencesAfter)
								newParam = new ParameterDeclarationExpression(variable.Type, variable.Name, ParameterModifiers.None);
							else {
								if (!hasOccurrencesAfter && !isInitialized)
									newMethod.Body.Children.Insert(0, new LocalVariableDeclaration(new VariableDeclaration(variable.Name, variable.Initializer, variable.Type)));
								else
									newParam = new ParameterDeclarationExpression(variable.Type, variable.Name, ParameterModifiers.In);
							}
						}
					}
					if (newParam != null)
						newMethod.Parameters.Add(newParam);
				}
			}
			
			List<VariableDeclaration> paramsAsVarDecls = new List<VariableDeclaration>();
			this.beforeCallDeclarations = new List<LocalVariableDeclaration>();
			
			for (int i = 0; i < otherReturnValues.Count - 1; i++) {
				VariableDeclaration varDecl = otherReturnValues[i];
				paramsAsVarDecls.Add(varDecl);
				ParameterDeclarationExpression p = new ParameterDeclarationExpression(varDecl.TypeReference, varDecl.Name);
				p.ParamModifier = ParameterModifiers.Out;
				if (!newMethod.Parameters.Contains(p)) {
					newMethod.Parameters.Add(p);
				} else {
					this.beforeCallDeclarations.Add(new LocalVariableDeclaration(varDecl));
				}
			}
			
			CreateReturnStatement(newMethod, possibleReturnValues);
			
			newMethod.Name = "NewMethod";
			
			this.extractedMethod = newMethod;
			
			return true;
		}
		
		bool HasReferencesInSelection(MethodDeclaration newMethod, Variable variable)
		{
			FindReferenceVisitor frv = new FindReferenceVisitor(CSharpNameComparer, variable.Name,
			                                                    newMethod.Body.StartLocation, newMethod.Body.EndLocation);
			
			newMethod.AcceptVisitor(frv, null);
			return frv.Identifiers.Count > 0;
		}

		Variable ResolveVariable(Variable variable)
		{
			Dom.ParseInformation info = ParserService.GetParseInformation(this.textEditor.FileName);
			Dom.ExpressionResult res = new Dom.ExpressionResult(variable.Name,
			                                                    Dom.DomRegion.FromLocation(variable.StartPos, variable.EndPos),
			                                                    Dom.ExpressionContext.Default, null);
			Dom.ResolveResult result = this.GetResolver().Resolve(res, info, this.textEditor.Document.Text);
			
			Dom.IReturnType type = currentProjectContent.SystemTypes.Object;
			Dom.ClassFinder finder = new Dom.ClassFinder(currentClass, textEditor.Caret.Line, textEditor.Caret.Column);
			
			if (result != null && result.ResolvedType != null)
				type = result.ResolvedType;
			
			if (variable.Type.Type == "var")
				variable.Type = Dom.Refactoring.CodeGenerator.ConvertType(type, finder);
			
			variable.IsReferenceType = type.IsReferenceType == true;
			
			return variable;
		}
		
		IEnumerable<Variable> FromParameters(MethodDeclaration newMethod)
		{
			if (parentNode is ParametrizedNode) {
				foreach (ParameterDeclarationExpression pde in (parentNode as ParametrizedNode).Parameters) {
					FindReferenceVisitor frv = new FindReferenceVisitor(CSharpNameComparer, pde.ParameterName, newMethod.Body.StartLocation, newMethod.Body.EndLocation);
					
					newMethod.AcceptVisitor(frv, null);
					if (frv.Identifiers.Count > 0) {
						pde.ParamModifier &= ~(ParameterModifiers.Params);
						
						if (parentNode is MethodDeclaration) {
							yield return new Variable((parentNode as MethodDeclaration).Body, pde);
						} else if (parentNode is ConstructorDeclaration) {
							yield return new Variable((parentNode as ConstructorDeclaration).Body, pde);
						} else if (parentNode is PropertyDeclaration) {
							var p = parentNode as PropertyDeclaration;
							yield return new Variable(p.BodyStart, p.BodyEnd, pde);
						} else {
							throw new NotSupportedException("not supported!");
						}
					}
				}
			}
			
			if (parentNode is PropertyDeclaration && IsInSetter(parentNode as PropertyDeclaration)) {
				PropertyDeclaration pd = parentNode as PropertyDeclaration;
				yield return new Variable(
					new LocalLookupVariable(
						"value", pd.TypeReference,
						pd.SetRegion.StartLocation, pd.SetRegion.EndLocation,
						false, false, null, null, false
					)
				);
			}
		}
		
		public override IOutputAstVisitor GetOutputVisitor()
		{
			return new CSharpOutputVisitor();
		}
		
		public override Dom.IResolver GetResolver()
		{
			return new NRefactoryResolver(Dom.LanguageProperties.CSharp);
		}
		
		bool IsInSetter(PropertyDeclaration property)
		{
			if (!property.HasSetRegion)
				return false;
			
			int startOffset = textEditor.Document.PositionToOffset(property.SetRegion.StartLocation.Line,
			                                                       property.SetRegion.StartLocation.Column);
			
			int endOffset = textEditor.Document.PositionToOffset(property.SetRegion.EndLocation.Line,
			                                                     property.SetRegion.EndLocation.Column);
			
			int selectionEnd = textEditor.SelectionStart + textEditor.SelectionLength;
			
			return textEditor.SelectionStart >= startOffset &&
				textEditor.SelectionStart <= endOffset &&
				selectionEnd >= startOffset &&
				selectionEnd <= endOffset;
		}
	}
}
