// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using SharpRefactoring.Visitors;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace SharpRefactoring
{
	/// <summary>
	/// Description of MethodExtractorBase.
	/// </summary>
	public abstract class MethodExtractorBase
	{
		protected ITextEditor textEditor;
		protected IDocument currentDocument;
		protected MethodDeclaration extractedMethod;
		protected AttributedNode parentNode;
		protected Statement caller;
		protected List<LocalVariableDeclaration> beforeCallDeclarations;
		protected VariableDeclaration returnedVariable;
		protected List<ISpecial> specialsList;
		
		protected Dom.IClass currentClass;
		protected Dom.IProjectContent currentProjectContent;
		
		protected Location start, end;
		
		public Statement Caller {
			get { return caller; }
		}
		
		public MethodDeclaration ExtractedMethod {
			get { return extractedMethod; }
		}
		
		public MethodExtractorBase(ITextEditor textEditor)
		{
			this.currentDocument = textEditor.Document;
			this.textEditor = textEditor;
			
			this.start = this.currentDocument.OffsetToPosition(this.textEditor.SelectionStart);
			this.end = this.currentDocument.OffsetToPosition(this.textEditor.SelectionStart + this.textEditor.SelectionLength);
		}
		
		protected static Statement CreateCaller(AttributedNode parent, MethodDeclaration method, VariableDeclaration returnVariable)
		{
			Statement caller;
			InvocationExpression expr = new InvocationExpression(new IdentifierExpression(method.Name), CreateArgumentExpressions(method.Parameters));

			if (method.TypeReference.Type != "System.Void") {
				TypeReference parentType = GetParentReturnType(parent);
				if (method.TypeReference == parentType)
					caller = new ReturnStatement(expr);
				else {
					returnVariable.Initializer = expr;
					caller = new LocalVariableDeclaration(returnVariable);
				}
			} else {
				caller = new ExpressionStatement(expr);
			}
			return caller;
		}
		
		protected void CreateReturnStatement(MethodDeclaration newMethod, List<VariableDeclaration> possibleReturnValues)
		{
			HasReturnStatementVisitor hrsv = new HasReturnStatementVisitor();

			newMethod.AcceptVisitor(hrsv, null);

			if (hrsv.HasReturn) {
				if (this.parentNode is MethodDeclaration)
					newMethod.TypeReference = (this.parentNode as MethodDeclaration).TypeReference;
				if (this.parentNode is PropertyDeclaration)
					newMethod.TypeReference = (this.parentNode as PropertyDeclaration).TypeReference;
				if (this.parentNode is OperatorDeclaration)
					newMethod.TypeReference = (this.parentNode as OperatorDeclaration).TypeReference;
			} else {
				if (possibleReturnValues.Count > 0) {
					newMethod.TypeReference = possibleReturnValues[possibleReturnValues.Count - 1].TypeReference;
					newMethod.Body.Children.Add(new ReturnStatement(new IdentifierExpression(possibleReturnValues[possibleReturnValues.Count - 1].Name)));
					this.returnedVariable = possibleReturnValues[possibleReturnValues.Count - 1];
				} else {
					newMethod.TypeReference = new TypeReference("System.Void", true);
					this.returnedVariable = null;
				}
			}
		}
		
		public void InsertCall()
		{
			string call = GenerateCode(CreateCaller(this.parentNode, this.extractedMethod, this.returnedVariable), false);
			StringBuilder builder = new StringBuilder();
			
			foreach (LocalVariableDeclaration v in this.beforeCallDeclarations) {
				builder.AppendLine(GenerateCode(v, false));
			}
			
			this.currentDocument.Replace(this.textEditor.SelectionStart, this.textEditor.SelectionLength, builder.ToString() + "\r\n" + call);
		}
		
		public void InsertAfterCurrentMethod()
		{
			IOutputAstVisitor outputVisitor = this.GetOutputVisitor();
			
			using (SpecialNodesInserter.Install(this.specialsList, outputVisitor)) {
				string code = "\r\n\r\n" + GenerateCode(this.extractedMethod, true);

				code = code.TrimEnd('\r', '\n', ' ', '\t');

				Dom.IMember p = GetParentMember(this.textEditor, start.Line, start.Column);
				
				int offset = textEditor.Document.PositionToOffset(p.BodyRegion.EndLine, p.BodyRegion.EndColumn);

				textEditor.Document.Insert(offset, code);
			}
		}
		
		protected ErrorKind CheckForJumpInstructions(MethodDeclaration method)
		{
			FindJumpInstructionsVisitor fjiv = new FindJumpInstructionsVisitor(method);
			
			method.AcceptVisitor(fjiv, null);
			return fjiv.DoCheck();
		}
		
		protected bool IsInCurrentSelection(Location location)
		{
			if (location.IsEmpty)
				return false;
			return IsInCurrentSelection(textEditor.Document.PositionToOffset(location.Line, location.Column));
		}
		
		protected bool IsInCurrentSelection(int offset)
		{
			return (offset >= textEditor.SelectionStart &&
			        offset < (textEditor.SelectionStart + textEditor.SelectionLength));
		}
		
		protected static BlockStatement GetBlock(string data)
		{
			data = "class Temp { public void t() {" + data + "} }";
			
			using (IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(data))) {
				parser.Parse();
				
				if (parser.Errors.Count > 0) {
					MessageService.ShowError("${res:AddIns.SharpRefactoring.ExtractMethod.InvalidSelection}");
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
					(FieldDirection)Enum.Parse(typeof(FieldDirection), pde.ParamModifier.ToString()),
					new IdentifierExpression(pde.ParameterName)));
			}
			
			return expressions;
		}

		protected abstract string GenerateCode(INode unit, bool installSpecials);
		
		protected Dom.IMember GetParentMember(ITextEditor textEditor, Location location)
		{
			return GetParentMember(textEditor, location.Line, location.Column);
		}
		
		protected Dom.IMember GetParentMember(ITextEditor textEditor, int line, int column)
		{
			Dom.ParseInformation parseInfo = ParserService.GetParseInformation(textEditor.FileName);
			if (parseInfo != null) {
				return parseInfo.CompilationUnit.GetInnermostMember(line, column);
			}
			
			return null;
		}
		
		protected AttributedNode GetParentMember(Location start, Location end)
		{
			using (IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(this.currentDocument.Text))) {
				parser.Parse();
				
				if (parser.Errors.Count > 0) {
					MessageService.ShowException(null, parser.Errors.ErrorOutput);
					return null;
				}
				
				FindMemberVisitor fmv = new FindMemberVisitor(start, end);
				
				parser.CompilationUnit.AcceptVisitor(fmv, null);
				
				return fmv.Member;
			}
		}
		
		protected static bool HasOccurrencesAfter(StringComparer nameComparer, AttributedNode member, Location location, string name, Location start, Location end)
		{
			FindReferenceVisitor frv = new FindReferenceVisitor(nameComparer, name, start, end);
			
			member.AcceptVisitor(frv, null);
			
			foreach (IdentifierExpression identifier in frv.Identifiers) {
				if (identifier.StartLocation > location)
					return true;
			}
			
			return false;
		}
		
		protected static bool HasAssignment(MethodDeclaration method, Variable variable)
		{
			HasAssignmentsVisitor hav = new HasAssignmentsVisitor(variable.Name, variable.Type, variable.StartPos, variable.EndPos);
			
			method.AcceptVisitor(hav, null);
			
			return hav.HasAssignment;
		}
		
		public abstract IOutputAstVisitor GetOutputVisitor();
		
		public abstract bool Extract();
		
		public abstract Dom.IResolver GetResolver();
		
		static TypeReference GetParentReturnType(AttributedNode parent)
		{
			if (parent is MemberNode)
				return (parent as MemberNode).TypeReference;
			
			return null;
		}
	}
	
	public class Variable {
		public TypeReference Type { get; set; }
		public string Name { get; set; }
		public Location StartPos { get; set; }
		public Location EndPos { get; set; }
		public Expression Initializer { get; set; }
		public bool IsReferenceType { get; set; }
		public bool WasOutParam { get; set; }
		public bool WasRefParam { get; set; }
		
		public Variable(LocalLookupVariable v)
		{
			this.Type = v.TypeRef;
			this.Name = v.Name;
			this.StartPos = v.StartPos;
			this.EndPos = v.EndPos;
			this.Initializer = v.Initializer;
		}
		
		public Variable(BlockStatement block, ParameterDeclarationExpression param)
			: this(block.StartLocation, block.EndLocation, param)
		{
		}
		
		public Variable(Location start, Location end, ParameterDeclarationExpression param)
		{
			this.Type = param.TypeReference;
			this.Name = param.ParameterName;
			this.StartPos = start;
			this.EndPos = end;
			this.Initializer = param.DefaultValue;
			this.WasOutParam = (param.ParamModifier & ParameterModifiers.Out) == ParameterModifiers.Out;
			this.WasRefParam = (param.ParamModifier & ParameterModifiers.Ref) == ParameterModifiers.Ref;
		}
		
		public override string ToString()
		{
			return "[ " + this.Type.Type + " " + this.Name + " ]";
		}
	}
}
