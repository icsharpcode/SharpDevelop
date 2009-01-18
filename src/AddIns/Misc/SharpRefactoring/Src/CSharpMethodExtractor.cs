// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3287 $</version>
// </file>
using ICSharpCode.SharpDevelop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.AstBuilder;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Document;
using SharpRefactoring.Transformers;
using SharpRefactoring.Visitors;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace SharpRefactoring
{
	public class CSharpMethodExtractor : MethodExtractorBase
	{
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
					MessageService.ShowError("Errors occured during parsing! Cannot extract a new method!");
					
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
				MessageService.ShowError("Invalid selection! Please select a valid range.");
				return false;
			}
			
			if (!CheckForJumpInstructions(newMethod, this.currentSelection))
				return false;
			
			newMethod.Modifier = parentNode.Modifier;
			
			newMethod.Modifier &= ~(Modifiers.Internal | Modifiers.Protected | Modifiers.Private | Modifiers.Public | Modifiers.Override);
			
			foreach (ParameterDeclarationExpression pde in parentNode.Parameters)
			{
				FindReferenceVisitor frv = new FindReferenceVisitor(true, pde.ParameterName, newMethod.Body.StartLocation, newMethod.Body.EndLocation);
				
				newMethod.AcceptVisitor(frv, null);
				
				if (frv.Identifiers.Count > 0) {
					bool isIn = true;
					foreach (IdentifierExpression identifier in frv.Identifiers) {
						if (!IsInSel(identifier.StartLocation, this.currentSelection))
							isIn = false;
					}
					
					if (isIn) {
						possibleReturnValues.Add(new VariableDeclaration(pde.ParameterName, null, pde.TypeReference));
					}
					
					bool hasOccurrences = HasOccurrencesAfter(true, parentNode, new Location(this.currentSelection.EndPosition.Column + 1, this.currentSelection.EndPosition.Line + 1), pde.ParameterName, newMethod.Body.StartLocation, newMethod.Body.EndLocation);
					if (hasOccurrences)
						newMethod.Parameters.Add(new ParameterDeclarationExpression(pde.TypeReference, pde.ParameterName, ParameterModifiers.Ref));
					else
						newMethod.Parameters.Add(new ParameterDeclarationExpression(pde.TypeReference, pde.ParameterName, pde.ParamModifier));
				}
			}
			
			LookupTableVisitor ltv = new LookupTableVisitor(SupportedLanguage.CSharp);
			
			parentNode.AcceptVisitor(ltv, null);
			
			Location start = new Location(this.currentSelection.StartPosition.Column + 1, this.currentSelection.StartPosition.Line + 1);
			Location end = new Location(this.currentSelection.EndPosition.Column + 1, this.currentSelection.EndPosition.Line + 1);
			
			foreach (KeyValuePair<string, List<LocalLookupVariable>> pair in ltv.Variables) {
				foreach (LocalLookupVariable variable in pair.Value) {
					if (variable.StartPos > end || variable.EndPos < start)
						continue;
					
					if (IsInSel(variable.StartPos, this.currentSelection) && HasOccurrencesAfter(true, this.parentNode, new Location(this.currentSelection.EndPosition.Column + 1, this.currentSelection.EndPosition.Line + 1), variable.Name, variable.StartPos, variable.EndPos)) {
						possibleReturnValues.Add(new VariableDeclaration(variable.Name, variable.Initializer, variable.TypeRef));
						otherReturnValues.Add(new VariableDeclaration(variable.Name, variable.Initializer, variable.TypeRef));
					}

					FindReferenceVisitor frv = new FindReferenceVisitor(true, variable.Name, start, end);
					
					parentNode.AcceptVisitor(frv, null);
					
					if ((frv.Identifiers.Count > 0) && (!(IsInSel(variable.StartPos, this.currentSelection) || IsInSel(variable.EndPos, this.currentSelection)))) {
						bool hasOccurrencesAfter = HasOccurrencesAfter(true, this.parentNode, new Location(this.currentSelection.EndPosition.Column + 1, this.currentSelection.EndPosition.Line + 1), variable.Name, variable.StartPos, variable.EndPos);
						bool isInitialized = IsInitializedVariable(true, this.parentNode, variable);
						bool hasAssignment = HasAssignment(newMethod, variable);
						bool getsAssigned = pair.Value.Count > 0;
						
						if (hasOccurrencesAfter && isInitialized)
							newMethod.Parameters.Add(new ParameterDeclarationExpression(variable.TypeRef, variable.Name, ParameterModifiers.Ref));
						else {
							if (hasOccurrencesAfter && hasAssignment)
								newMethod.Parameters.Add(new ParameterDeclarationExpression(variable.TypeRef, variable.Name, ParameterModifiers.Out));
							else {
								if (!hasOccurrencesAfter && getsAssigned)
									newMethod.Parameters.Add(new ParameterDeclarationExpression(variable.TypeRef, variable.Name, ParameterModifiers.None));
								else {
									if (!hasOccurrencesAfter && !isInitialized)
										newMethod.Body.Children.Insert(0, new LocalVariableDeclaration(new VariableDeclaration(variable.Name, variable.Initializer, variable.TypeRef)));
									else
										newMethod.Parameters.Add(new ParameterDeclarationExpression(variable.TypeRef, variable.Name, ParameterModifiers.In));
								}
							}
						}
					}
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

			ReplaceUnnecessaryVariableDeclarationsTransformer t = new ReplaceUnnecessaryVariableDeclarationsTransformer(paramsAsVarDecls);
			
			newMethod.AcceptVisitor(t, null);
			
			CreateReturnStatement(newMethod, possibleReturnValues);
			
			newMethod.Name = "NewMethod";
			
			this.extractedMethod = newMethod;
			
			return true;
		}
		
		public override IOutputAstVisitor GetOutputVisitor()
		{
			return new CSharpOutputVisitor();
		}
	}
}
