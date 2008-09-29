// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3287 $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using SharpRefactoring.Visitors;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace SharpRefactoring
{
	/// <summary>
	/// Description of MethodExtractorBase.
	/// </summary>
	public class MethodExtractorBase
	{
		protected ICSharpCode.TextEditor.TextEditorControl textEditor;
		protected ISelection currentSelection;
		protected IDocument currentDocument;
		protected MethodDeclaration extractedMethod;
		protected ParametrizedNode parentNode;
		protected Statement caller;
		protected List<LocalVariableDeclaration> beforeCallDeclarations;
		protected IOutputAstVisitor output;
		protected VariableDeclaration returnedVariable;
		protected List<ISpecial> specialsList;
		
		protected Dom.IClass currentClass;
		protected Dom.IProjectContent currentProjectContent;
		
		public Statement Caller {
			get { return caller; }
		}
		
		public MethodDeclaration ExtractedMethod {
			get { return extractedMethod; }
		}
		
		public MethodExtractorBase(ICSharpCode.TextEditor.TextEditorControl textEditor, ISelection selection, IOutputAstVisitor output)
		{
			this.currentDocument = textEditor.Document;
			this.textEditor = textEditor;
			this.currentSelection = selection;
			this.output = output;
		}
		
		protected static Statement CreateCaller(ParametrizedNode parent, MethodDeclaration method, VariableDeclaration returnVariable)
		{
			Statement caller;
			InvocationExpression expr = new InvocationExpression(new IdentifierExpression(method.Name), CreateArgumentExpressions(method.Parameters));

			if (method.TypeReference.Type != "void") {
				if (parent is MethodDeclaration) {
					if (method.TypeReference == (parent as MethodDeclaration).TypeReference)
						caller = new ReturnStatement(expr);
					else {
						returnVariable.Initializer = expr;
						caller = new LocalVariableDeclaration(returnVariable);
					}
				}
				else {
					returnVariable.Initializer = expr;
					caller = new LocalVariableDeclaration(returnVariable);
				}
			}
			else {
				caller = new ExpressionStatement(expr);
			}
			return caller;
		}
		
		protected void CreateReturnStatement(MethodDeclaration newMethod, List<VariableDeclaration> possibleReturnValues)
		{
			HasReturnStatementVisitor hrsv = new HasReturnStatementVisitor();

			newMethod.AcceptVisitor(hrsv, null);

			if (hrsv.HasReturn) {
				if (this.parentNode is MethodDeclaration) newMethod.TypeReference = (this.parentNode as MethodDeclaration).TypeReference;
				if (this.parentNode is PropertyDeclaration) newMethod.TypeReference = (this.parentNode as PropertyDeclaration).TypeReference;
				if (this.parentNode is OperatorDeclaration) newMethod.TypeReference = (this.parentNode as OperatorDeclaration).TypeReference;
			} else {
				if (possibleReturnValues.Count > 0) {
					newMethod.TypeReference = possibleReturnValues[possibleReturnValues.Count - 1].TypeReference;
					newMethod.Body.Children.Add(new ReturnStatement(new IdentifierExpression(possibleReturnValues[possibleReturnValues.Count - 1].Name)));
					this.returnedVariable = possibleReturnValues[possibleReturnValues.Count - 1];
				} else {
					newMethod.TypeReference = new TypeReference("void");
					this.returnedVariable = null;
				}
			}
		}
		
		public string CreatePreview()
		{
			BlockStatement body = this.extractedMethod.Body;
			this.extractedMethod.Body = new BlockStatement();

			this.extractedMethod.AcceptVisitor(output, null);
			
			this.extractedMethod.Body = body;

			return output.Text;
		}
		
		public void InsertCall()
		{
			string call = GenerateCode(CreateCaller(this.parentNode, this.extractedMethod, this.returnedVariable), false);
			StringBuilder builder = new StringBuilder();
			
			foreach (LocalVariableDeclaration v in this.beforeCallDeclarations) {
				builder.AppendLine(GenerateCode(v, false));
			}
			
			this.currentDocument.Replace(this.currentSelection.Offset, this.currentSelection.Length, builder.ToString() + "\r\n" + call);
		}
		
		public void InsertAfterCurrentMethod()
		{
			using (SpecialNodesInserter.Install(this.specialsList, this.output)) {
				string code = "\r\n\r\n" + GenerateCode(this.extractedMethod, true);

				code = code.TrimEnd('\r', '\n', ' ', '\t');

				Dom.IMember p = GetParentMember(this.textEditor, this.currentSelection.StartPosition.Line, this.currentSelection.StartPosition.Column);
				
				TextLocation loc = new ICSharpCode.TextEditor.TextLocation(
					p.BodyRegion.EndColumn - 1, p.BodyRegion.EndLine - 1);
				
				int offset = textEditor.Document.PositionToOffset(loc);

				textEditor.Document.Insert(offset, code);
			}
		}
		
		protected static bool CheckForJumpInstructions(MethodDeclaration method, ISelection selection)
		{
			FindJumpInstructionsVisitor fjiv = new FindJumpInstructionsVisitor(method, selection);
			
			method.AcceptVisitor(fjiv, null);
			
			return fjiv.IsOk;
		}
		
		protected static bool IsInSel(Location location, ISelection sel)
		{
			bool result = (sel.ContainsPosition(new ICSharpCode.TextEditor.TextLocation(location.Column - 1, location.Line - 1)));
			return result;
		}
		
		protected static BlockStatement GetBlock(string data)
		{
			data = "class Temp { public void t() {" + data + "} }";
			
			using (IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(data))) {
				parser.Parse();
				
				if (parser.Errors.Count > 0) {
					MessageService.ShowError("Invalid selection! Please select a valid range!");
				}
				
				MethodDeclaration method = (MethodDeclaration)(parser.CompilationUnit.Children[0].Children[0]);
				
				return method.Body;
			}
		}
		
		protected static string GetIndentation(string line)
		{
			string indent = "";

			foreach (char c in line) {
				if ((c == ' ') || (c == '\t'))
					indent += c;
				else
					break;
			}
			return indent;
		}

		protected static List<Expression> CreateArgumentExpressions(List<ParameterDeclarationExpression> parameters)
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

		protected virtual string GenerateCode(INode unit, bool installSpecials)
		{
			throw new InvalidOperationException("Cannot use plain MethodExtractor, please use a language specific implementation!");
		}
		
		protected Dom.IMember GetParentMember(ICSharpCode.TextEditor.TextEditorControl textEditor, TextLocation location)
		{
			return GetParentMember(textEditor, location.Line, location.Column);
		}
		
		protected Dom.IMember GetParentMember(ICSharpCode.TextEditor.TextEditorControl textEditor, int line, int column)
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
		
		protected ParametrizedNode GetParentMember(int startLine, int startColumn, int endLine, int endColumn)
		{
			using (IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(this.currentDocument.TextContent))) {
				parser.Parse();
				
				if (parser.Errors.Count > 0) {
					MessageService.ShowError(null, parser.Errors.ErrorOutput);
					return null;
				}
				
				FindMemberVisitor fmv = new FindMemberVisitor(startColumn, startLine, endColumn, endLine);
				
				parser.CompilationUnit.AcceptVisitor(fmv, null);
				
				return fmv.Member;
			}
		}
		
		protected static bool HasOccurrencesAfter(bool caseSensitive, ParametrizedNode member, Location location, string name, Location start, Location end)
		{
			FindReferenceVisitor frv = new FindReferenceVisitor(caseSensitive, name, start, end);
			
			member.AcceptVisitor(frv, null);
			
			foreach (IdentifierExpression identifier in frv.Identifiers)
			{
				if (identifier.StartLocation > location)
					return true;
			}
			
			return false;
		}
		
		protected bool IsInitializedVariable(bool caseSensitive, ParametrizedNode member, LocalLookupVariable variable)
		{
			if (!(variable.Initializer.IsNull)) {
				return true;
			} else {
				FindReferenceVisitor frv = new FindReferenceVisitor(caseSensitive, variable.Name, variable.StartPos, variable.EndPos);
				
				member.AcceptVisitor(frv, null);
				
				foreach (IdentifierExpression expr in frv.Identifiers) {
					if ((expr.EndLocation < new Location(currentSelection.StartPosition.Column, currentSelection.StartPosition.Line)) &&
					    !(expr.IsNull))
						return true;
				}
			}
			
			return false;
		}
		
		protected static bool HasAssignment(MethodDeclaration method, LocalLookupVariable variable)
		{
			HasAssignmentsVisitor hav = new HasAssignmentsVisitor(variable.Name, variable.TypeRef, variable.StartPos, variable.EndPos);
			
			method.AcceptVisitor(hav, null);
			
			return hav.HasAssignment;
		}

		
		public virtual bool Extract()
		{
			throw new InvalidOperationException("Cannot use plain MethodExtractor, please use a language specific implementation!");
		}
	}
	

}
