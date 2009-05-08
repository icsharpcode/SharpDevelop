// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3287 $</version>
// </file>

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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Document;
using SharpRefactoring.Visitors;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace SharpRefactoring
{
	public class CSharpMethodExtractor : MethodExtractorBase
	{
		static readonly StringComparer CSharpNameComparer = StringComparer.Ordinal;
		
		public CSharpMethodExtractor(ICSharpCode.TextEditor.TextEditorControl textEditor, ISelection selection)
			: base(textEditor, selection)
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
			using (IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader("class Tmp { void Test() {\n " + this.currentSelection.SelectedText + "\n}}"))) {
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
			newMethod.Body = GetBlock(this.currentSelection.SelectedText);
			newMethod.Body.StartLocation = new Location(0,0);
			
			this.parentNode = GetParentMember(this.currentSelection.StartPosition.Line, this.currentSelection.StartPosition.Column, this.currentSelection.EndPosition.Line, this.currentSelection.EndPosition.Column);
			
			if (parentNode == null) {
				MessageService.ShowError("${res:AddIns.SharpRefactoring.ExtractMethod.InvalidSelection}");
				return false;
			}
			
			if (!CheckForJumpInstructions(newMethod, this.currentSelection))
				return false;
			
			newMethod.Modifier = parentNode.Modifier;
			
			newMethod.Modifier &= ~(Modifiers.Internal | Modifiers.Protected | Modifiers.Private | Modifiers.Public | Modifiers.Override);
			
			LookupTableVisitor ltv = new LookupTableVisitor(SupportedLanguage.CSharp);
			
			parentNode.AcceptVisitor(ltv, null);
			
			var variablesList = (from list in ltv.Variables.Values from item in list select new Variable(item))
				.Where(v => !(v.StartPos > end || v.EndPos < start) && HasReferencesInSelection(newMethod, v))
				.Union(FromParameters(newMethod))
				.Select(va => ResolveVariable(va));
			
			foreach (var variable in variablesList) {
				LoggingService.Debug(variable);
				
				bool hasOccurrencesAfter = HasOccurrencesAfter(CSharpNameComparer, this.parentNode, end, variable.Name, variable.StartPos, variable.EndPos);
				bool isInitialized = (variable.Initializer != null) ? !variable.Initializer.IsNull : false;
				bool hasAssignment = HasAssignment(newMethod, variable);
				
				if (IsInSel(variable.StartPos, this.currentSelection) && hasOccurrencesAfter) {
					possibleReturnValues.Add(new VariableDeclaration(variable.Name, variable.Initializer, variable.Type));
					otherReturnValues.Add(new VariableDeclaration(variable.Name, variable.Initializer, variable.Type));
				}
				
				if (!(IsInSel(variable.StartPos, this.currentSelection) || IsInSel(variable.EndPos, this.currentSelection))) {
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
			Dom.ResolveResult result = this.GetResolver().Resolve(res, info, this.textEditor.Document.TextContent);

			if (variable.Type.Type == "var")
				variable.Type = Dom.Refactoring.CodeGenerator.ConvertType(result.ResolvedType, new Dom.ClassFinder(result.CallingMember));

			variable.IsReferenceType = result.ResolvedType.IsReferenceType == true;
			
			return variable;
		}
		
		IEnumerable<Variable> FromParameters(MethodDeclaration newMethod)
		{
			foreach (ParameterDeclarationExpression pde in parentNode.Parameters) {
				FindReferenceVisitor frv = new FindReferenceVisitor(CSharpNameComparer, pde.ParameterName, newMethod.Body.StartLocation, newMethod.Body.EndLocation);
				
				newMethod.AcceptVisitor(frv, null);
				if (frv.Identifiers.Count > 0) {
					pde.ParamModifier &= ~(ParameterModifiers.Params);
					
					if (parentNode is MethodDeclaration) {
						yield return new Variable((parentNode as MethodDeclaration).Body, pde);
					} else {
						throw new NotSupportedException("not supported!");
					}
				}
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
	}
}
