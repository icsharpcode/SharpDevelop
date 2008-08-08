using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor.Document;
using SharpRefactoring.Visitors;
using Dom = ICSharpCode.SharpDevelop.Dom;
using SharpRefactoring.Forms;
using System.Text.RegularExpressions;

namespace SharpRefactoring
{
	public class ExtractMethod : AbstractRefactoringCommand
	{
		ISelection currentSelection;
		IDocument currentDocument;
		ICSharpCode.SharpDevelop.Dom.Refactoring.CodeGenerator generator;
		ParametrizedNode parent;
		VariableDeclaration returnVar;
		IList<ISpecial> specialsList;
		
		protected override void Run(ICSharpCode.TextEditor.TextEditorControl textEditor, ICSharpCode.SharpDevelop.Dom.Refactoring.RefactoringProvider provider)
		{
			if (textEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
			{
				this.currentDocument = textEditor.Document;
				this.currentSelection = textEditor.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
				this.generator = ProjectService.CurrentProject.LanguageProperties.CodeGenerator;
				
				MethodDeclaration method = GetMethod(this.currentDocument, this.currentSelection);
				
				if (method == null)	return;
				
				Statement caller;
				InvocationExpression expr = new InvocationExpression(new IdentifierExpression(method.Name), CreateArgumentExpressions(method.Parameters));
				
				if (method.TypeReference.Type != "void") {
					if (parent is MethodDeclaration) {
						if (method.TypeReference == (parent as MethodDeclaration).TypeReference)
							caller = new ReturnStatement(expr);
						else {
							this.returnVar.Initializer = expr;
							caller = new LocalVariableDeclaration(this.returnVar);
						}
					} else {
						this.returnVar.Initializer = expr;
						caller = new LocalVariableDeclaration(this.returnVar);
					}
				} else {
					caller = new ExpressionStatement(expr);
				}
				
				TextEditorDocument doc = new TextEditorDocument(this.currentDocument);
				
				string line = this.currentDocument.GetText(this.currentDocument.GetLineSegment(this.currentSelection.StartPosition.Line));
				string indent = "";
				
				foreach (char c in line) {
					if ((c == ' ') || (c == '\t'))
						indent += c;
					else
						break;
				}
				
				textEditor.Document.UndoStack.StartUndoGroup();
				
				// TODO : Implement VB.NET support
				IOutputAstVisitor csOutput = new CSharpOutputVisitor();
				
				// FIXME : Problems with comments at the begin of the selection
				RemoveUnneededSpecials();
				
				using (SpecialNodesInserter.Install(this.specialsList, csOutput)) {
					method.AcceptVisitor(csOutput, null);
					string code = "\n\n" + csOutput.Text;
					
					code = code.TrimEnd('\n', ' ', '\t');
					
					Dom.IMember p = GetParentMember(textEditor, this.currentSelection.StartPosition.Line, this.currentSelection.StartPosition.Column);
					
					textEditor.Document.Insert(textEditor.Document.PositionToOffset(
						new ICSharpCode.TextEditor.TextLocation(
							p.BodyRegion.EndColumn - 1, p.BodyRegion.EndLine - 1)
					), code);
				}
				
				string call = indent + GenerateCode(new CSharpOutputVisitor(), caller);
				
				call += (textEditor.ActiveTextAreaControl.SelectionManager.SelectedText.EndsWith("\n")) ? "\n" : "";
				textEditor.Document.Replace(currentSelection.Offset, currentSelection.Length, call);
				textEditor.Document.FormattingStrategy.IndentLines(textEditor.ActiveTextAreaControl.TextArea, 0, textEditor.Document.TotalNumberOfLines - 1);

				textEditor.Document.UndoStack.EndUndoGroup();
				
				textEditor.ActiveTextAreaControl.SelectionManager.ClearSelection();
				
			}
		}
		
		void RemoveUnneededSpecials()
		{
			int i = 0;

			while (i < this.specialsList.Count) {
				ISpecial spec = this.specialsList[i];
				if (!IsInSel(spec.EndPosition, this.currentSelection) || !IsInSel(spec.StartPosition, this.currentSelection)) {
					this.specialsList.RemoveAt(i);
					continue;
				} else {
					if (spec is Comment) {
						Comment comment = spec as Comment;
						this.specialsList[i] = new Comment(comment.CommentType, comment.CommentText,
						                                   new Location(spec.StartPosition.Column, spec.StartPosition.Line - this.currentSelection.StartPosition.Line + 1),
						                                   new Location(spec.EndPosition.Column, spec.EndPosition.Line - this.currentSelection.StartPosition.Line + 1));
					} else {
						if (spec is PreprocessingDirective) {
							PreprocessingDirective ppd = spec as PreprocessingDirective;

							this.specialsList[i] = new PreprocessingDirective(ppd.Cmd, ppd.Arg,
							                                                  new Location(spec.StartPosition.Column, spec.StartPosition.Line - this.currentSelection.StartPosition.Line + 1),
							                                                  new Location(spec.EndPosition.Column, spec.EndPosition.Line - this.currentSelection.StartPosition.Line + 1));
						} else {
							this.specialsList[i] = new BlankLine(new Location(spec.StartPosition.Column, spec.StartPosition.Line - this.currentSelection.StartPosition.Line + 1));
						}
					}
				}

				i++;
			}
		}

		static List<Expression> CreateArgumentExpressions(List<ParameterDeclarationExpression> parameters)
		{
			List<Expression> expressions = new List<Expression>();
			
			foreach (ParameterDeclarationExpression pde in parameters)
			{
				expressions.Add(new DirectionExpression(
					(FieldDirection)Enum.Parse(typeof(FieldDirection),pde.ParamModifier.ToString()),
					new IdentifierExpression(pde.ParameterName)));
			}
			
			return expressions;
		}
		
		CompilationUnit GetCurrentCompilationUnit(IDocument doc)
		{
			using (IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(doc.TextContent))) {
				parser.Parse();
				this.specialsList = parser.Lexer.SpecialTracker.RetrieveSpecials();
				
				if (parser.Errors.Count > 0) {
					MessageService.ShowError(null, parser.Errors.ErrorOutput);
					return null;
				}
				
				return parser.CompilationUnit;
			}
		}
		
		MethodDeclaration GetMethod(ICSharpCode.TextEditor.Document.IDocument doc, ISelection sel)
		{
			MethodDeclaration newMethod = new MethodDeclaration();
			
			CompilationUnit unit = GetCurrentCompilationUnit(doc);
			
			if (unit == null)
				return null;
			
			// Initialise new method
            newMethod.Body = GetBlock(sel.SelectedText);
			newMethod.Body.StartLocation = new Location(0,0);

			this.parent = GetParentMember(unit, sel.StartPosition.Line, sel.StartPosition.Column, sel.EndPosition.Line, sel.EndPosition.Column);
			
			if (parent == null) {
				MessageService.ShowError("Invalid selection! Please select a valid range.");
				return null;
			}
			
			if (!CheckForJumpInstructions(newMethod, sel))
				return null;
			
			newMethod.Modifier = parent.Modifier;

			newMethod.Modifier &= ~(Modifiers.Internal | Modifiers.Protected | Modifiers.Private | Modifiers.Public);

			List<VariableDeclaration> possibleReturnValues = new List<VariableDeclaration>();
			
			foreach (ParameterDeclarationExpression pde in parent.Parameters)
			{
				FindReferenceVisitor frv = new FindReferenceVisitor(pde.ParameterName, pde.TypeReference);
				
				newMethod.AcceptVisitor(frv, null);
				
				if (frv.Identifiers.Count > 0) {
					bool isIn = true;
					foreach (IdentifierExpression identifier in frv.Identifiers) {
						if (!IsInSel(identifier.StartLocation, sel))
							isIn = false;
					}
					
					if (isIn) {
						possibleReturnValues.Add(new VariableDeclaration(pde.ParameterName, null, pde.TypeReference));
					}
					
					bool hasOccurrences = HasOccurrencesAfter(parent, new Location(sel.EndPosition.Column + 1, sel.EndPosition.Line + 1), pde.ParameterName, pde.TypeReference);						if (hasOccurrences)
						newMethod.Parameters.Add(new ParameterDeclarationExpression(pde.TypeReference, pde.ParameterName, ParameterModifiers.Ref));
					else
						newMethod.Parameters.Add(new ParameterDeclarationExpression(pde.TypeReference, pde.ParameterName, ParameterModifiers.In));
				}
			}
			
			FindLocalVariablesVisitor flvv = new FindLocalVariablesVisitor();
			
			parent.AcceptVisitor(flvv, null);
			
			foreach (VariableDeclaration lvd in flvv.Variables)
			{
				FindReferenceVisitor frv = new FindReferenceVisitor(lvd.Name, lvd.TypeReference);
				
				newMethod.AcceptVisitor(frv, null);
				
				if (IsInSel(lvd.StartLocation, sel) && HasOccurrencesAfter(parent, new Location(sel.EndPosition.Column + 1, sel.EndPosition.Line + 1), lvd.Name, lvd.TypeReference))
					possibleReturnValues.Add(new VariableDeclaration(lvd.Name, null, lvd.TypeReference));
				
				if ((frv.Identifiers.Count > 0) && (!(IsInSel(lvd.StartLocation, sel) || IsInSel(lvd.EndLocation, sel)))) {
					bool hasOccurrences = HasOccurrencesAfter(parent, new Location(sel.EndPosition.Column + 1, sel.EndPosition.Line + 1), lvd.Name, lvd.TypeReference);
					bool isInitialized = IsInitializedVariable(parent, lvd);
					bool hasAssignment = HasAssignment(newMethod, lvd);
					if (hasOccurrences && isInitialized)
						newMethod.Parameters.Add(new ParameterDeclarationExpression(lvd.TypeReference, lvd.Name, ParameterModifiers.Ref));
					else {
						if (hasOccurrences && hasAssignment)
							newMethod.Parameters.Add(new ParameterDeclarationExpression(lvd.TypeReference, lvd.Name, ParameterModifiers.Out));
						else {
							if (!hasOccurrences && !isInitialized)
								newMethod.Body.Children.Insert(0, new LocalVariableDeclaration(lvd));
							else
								newMethod.Parameters.Add(new ParameterDeclarationExpression(lvd.TypeReference, lvd.Name, ParameterModifiers.In));
						}
					}
				}
			}
			
			HasReturnStatementVisitor hrsv = new HasReturnStatementVisitor();
			
			newMethod.AcceptVisitor(hrsv, null);
			
			if (hrsv.HasReturn) {
				if (parent is MethodDeclaration)
					newMethod.TypeReference = (parent as MethodDeclaration).TypeReference;
				if (parent is PropertyDeclaration)
					newMethod.TypeReference = (parent as PropertyDeclaration).TypeReference;
				if (parent is OperatorDeclaration)
					newMethod.TypeReference = (parent as OperatorDeclaration).TypeReference;
			} else {
				if (possibleReturnValues.Count > 0) {
					newMethod.TypeReference = possibleReturnValues[0].TypeReference;
					newMethod.Body.Children.Add(new ReturnStatement(new IdentifierExpression(possibleReturnValues[0].Name)));
					this.returnVar = possibleReturnValues[0];
				} else
					newMethod.TypeReference = new TypeReference("void");
			}

                IOutputAstVisitor output = new CSharpOutputVisitor();

                newMethod.Name = "NewMethod";

                BlockStatement body = newMethod.Body;
                newMethod.Body = new BlockStatement();

                newMethod.AcceptVisitor(output, null);

                string preview = output.Text;

                ExtractMethodForm form = new ExtractMethodForm("NewMethod", preview);

                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    newMethod.Name = form.Text;
                    newMethod.Body = body;
                }
                else
                {
                    return null;
                }
			
			return newMethod;
		}
		
		Dom.IMember GetParentMember(ICSharpCode.TextEditor.TextEditorControl textEditor, int line, int column)
		{
			Dom.ParseInformation parseInfo = ParserService.GetParseInformation(textEditor.FileName);
			if (parseInfo != null) {
				Dom.IClass c = parseInfo.MostRecentCompilationUnit.GetInnermostClass(line, column);
				if (c != null) {
					foreach (Dom.IMember member in c.Properties) {
						if (member.BodyRegion.IsInside(line, column)) {
							return member;
						}
					}
					foreach (Dom.IMember member in c.Methods) {
						if (member.BodyRegion.IsInside(line, column)) {
							return member;
						}
					}
				}
			}
			
			return null;
		}
		
		static bool CheckForJumpInstructions(MethodDeclaration method, ISelection selection)
		{
			FindJumpInstructionsVisitor fjiv = new FindJumpInstructionsVisitor(method, selection);
			
			method.AcceptVisitor(fjiv, null);
			
			return fjiv.IsOk;
		}
		
		static bool IsInSel(Location location, ISelection sel)
		{
			bool result = (sel.ContainsPosition(new ICSharpCode.TextEditor.TextLocation(location.Column - 1, location.Line - 1)));
			return result;
		}
		
		static BlockStatement GetBlock(string data)
		{
			data = "class Temp { public void t() {" + data + "} }";
			
			using (IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(data))) {
				parser.Parse();
				
				if (parser.Errors.Count > 0) {
					throw new ArgumentException("Invalid selection! Please select a valid range!");
				}
				
				MethodDeclaration method = (MethodDeclaration)(parser.CompilationUnit.Children[0].Children[0]);
				
				return method.Body;
			}
		}

		static string GenerateCode(IOutputAstVisitor outputVisitor, INode unit)
		{
			unit.AcceptVisitor(outputVisitor, null);
			return outputVisitor.Text;
		}
		
		static ParametrizedNode GetParentMember(CompilationUnit unit, int startLine, int startColumn, int endLine, int endColumn)
		{
			FindMemberVisitor fmv = new FindMemberVisitor(startColumn, startLine, endColumn, endLine);
			
			unit.AcceptVisitor(fmv, null);
			
			return fmv.Member;
		}
		
		static bool HasOccurrencesAfter(ParametrizedNode member, Location location, string name, TypeReference type)
		{
			FindReferenceVisitor frv = new FindReferenceVisitor(name, type);
			
			member.AcceptVisitor(frv, null);
			
			foreach (IdentifierExpression identifier in frv.Identifiers)
			{
				if (identifier.StartLocation > location)
					return true;
			}
			
			return false;
		}
		
		bool IsInitializedVariable(ParametrizedNode member, VariableDeclaration variable)
		{
			if (!(variable.Initializer.IsNull)) {
				return true;
			} else {
				FindReferenceVisitor frv = new FindReferenceVisitor(variable.Name, variable.TypeReference);
				
				member.AcceptVisitor(frv, null);
				
				foreach (IdentifierExpression expr in frv.Identifiers) {
					if ((expr.EndLocation < new Location(currentSelection.StartPosition.Column, currentSelection.StartPosition.Line)) &&
					    !(expr.IsNull))
						return true;
				}
			}
			
			return false;
		}
		
		static bool HasAssignment(MethodDeclaration method, VariableDeclaration variable)
		{
			HasAssignmentsVisitor hav = new HasAssignmentsVisitor(variable.Name, variable.TypeReference);
			
			method.AcceptVisitor(hav, null);
			
			return hav.HasAssignment;
		}
	}
}
