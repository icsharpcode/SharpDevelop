// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using AST = Boo.Lang.Compiler.Ast;

namespace Grunwald.BooBinding.CodeCompletion
{
	public class BooResolver : IResolver
	{
		#region Fields and properties
		ICompilationUnit cu;
		IProjectContent pc;
		int caretLine;
		int caretColumn;
		IClass callingClass;
		IMethodOrProperty callingMember;
		
		public IClass CallingClass {
			get {
				return callingClass;
			}
		}
		
		public IMethodOrProperty CallingMember {
			get {
				return callingMember;
			}
		}
		
		public int CaretLine {
			get {
				return caretLine;
			}
		}
		
		public int CaretColumn {
			get {
				return caretColumn;
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				return pc;
			}
		}
		
		public ICompilationUnit CompilationUnit {
			get {
				return cu;
			}
		}
		
		/// <summary>
		/// Gets if duck typing is enabled for the Boo project.
		/// </summary>
		public bool IsDucky {
			get {
				BooProject p = pc.Project as BooProject;
				if (p != null)
					return p.Ducky;
				else
					return false;
			}
		}
		#endregion
		
		#region Initialization
		bool Initialize(ParseInformation parseInfo, int caretLine, int caretColumn)
		{
			if (parseInfo == null) {
				return false;
			}
			this.cu = parseInfo.CompilationUnit;
			if (cu == null) {
				return false;
			}
			this.pc = cu.ProjectContent;
			this.caretLine = caretLine;
			this.caretColumn = caretColumn;
			this.callingClass = GetCallingClass(pc);
			callingMember = ResolveCurrentMember(callingClass);
			if (callingMember == null) {
				if (cu != parseInfo.CompilationUnit) {
					IClass olderClass = GetCallingClass(parseInfo.CompilationUnit.ProjectContent);
					if (olderClass != null && callingClass == null) {
						this.callingClass = olderClass;
					}
					callingMember = ResolveCurrentMember(olderClass);
				}
			}
			if (callingMember != null) {
				if (caretLine > callingMember.BodyRegion.EndLine) {
					this.caretLine = callingMember.BodyRegion.EndLine;
					this.caretColumn = callingMember.BodyRegion.EndColumn - 1;
				} else if (caretLine == callingMember.BodyRegion.EndLine && caretColumn >= callingMember.BodyRegion.EndColumn) {
					this.caretColumn = callingMember.BodyRegion.EndColumn - 1;
				}
			}
			return true;
		}
		
		IClass GetCallingClass(IProjectContent pc)
		{
			IClass callingClass = cu.GetInnermostClass(caretLine, caretColumn);
			if (callingClass == null) {
				if (cu.Classes.Count == 0) return null;
				callingClass = cu.Classes[cu.Classes.Count - 1];
				if (!callingClass.Region.IsEmpty) {
					if (callingClass.Region.BeginLine > caretLine)
						callingClass = null;
				}
			}
			return callingClass;
		}
		
		IMethodOrProperty ResolveCurrentMember(IClass callingClass)
		{
			//LoggingService.DebugFormatted("Getting current method... caretLine = {0}, caretColumn = {1}", caretLine, caretColumn);
			if (callingClass == null) return null;
			IMethodOrProperty best = null;
			int line = 0;
			foreach (IMethod m in callingClass.Methods) {
				if (m.Region.BeginLine <= caretLine && m.Region.BeginLine > line) {
					line = m.Region.BeginLine;
					best = m;
				}
			}
			foreach (IProperty m in callingClass.Properties) {
				if (m.Region.BeginLine <= caretLine && m.Region.BeginLine > line) {
					line = m.Region.BeginLine;
					best = m;
				}
			}
			if (callingClass.Region.IsEmpty) {
				// maybe we are in Main method?
				foreach (IMethod m in callingClass.Methods) {
					if (m.Region.IsEmpty && !m.IsSynthetic) {
						// the main method
						if (best == null || best.BodyRegion.EndLine < caretLine)
							return m;
					}
				}
			}
			return best;
		}
		#endregion
		
		#region GetTypeOfExpression
		public IReturnType GetTypeOfExpression(AST.Expression expr, IClass callingClass)
		{
			AST.Node node = expr;
			AST.LexicalInfo lexInfo;
			do {
				if (node == null) return null;
				lexInfo = node.LexicalInfo;
				node = node.ParentNode;
			} while (lexInfo == null || lexInfo.FileName == null);
			if (!Initialize(ParserService.GetParseInformation(lexInfo.FileName), lexInfo.Line, lexInfo.Column))
				return null;
			if (callingClass != null)
				this.callingClass = callingClass;
			ResolveVisitor visitor = new ResolveVisitor(this);
			visitor.Visit(expr);
			if (visitor.ResolveResult == null)
				return null;
			else
				return visitor.ResolveResult.ResolvedType;
		}
		#endregion
		
		#region GetCurrentBooMethod
		AST.Node GetCurrentBooMethod()
		{
			if (callingMember == null)
				return null;
			// TODO: don't save boo's AST in userdata, but parse fileContent here
			return callingMember.UserData as AST.Node;
		}
		#endregion
		
		#region Resolve
		public ResolveResult Resolve(ExpressionResult expressionResult,
		                             ParseInformation parseInfo, string fileContent)
		{
			if (!Initialize(parseInfo, expressionResult.Region.BeginLine, expressionResult.Region.BeginColumn))
				return null;
			LoggingService.Debug("Resolve " + expressionResult.ToString());
			if (expressionResult.Expression == "__GlobalNamespace") { // used for "import" completion
				return new NamespaceResolveResult(callingClass, callingMember, "");
			}
			
			ResolveResult rr = CtrlSpaceResolveHelper.GetResultFromDeclarationLine(callingClass, callingMember as IMethodOrProperty, this.caretLine, this.caretColumn, expressionResult);
			if (rr != null) return rr;
			
			AST.Expression expr;
			try {
				expr = Boo.Lang.Parser.BooParser.ParseExpression("expression", expressionResult.Expression);
			} catch (Exception ex) {
				LoggingService.Debug("Boo expression parser: " + ex.Message);
				return null;
			}
			if (expr == null)
				return null;
			if (expr is AST.IntegerLiteralExpression)
				return new IntegerLiteralResolveResult(callingClass, callingMember, pc.SystemTypes.Int32);
			
			if (expressionResult.Context == ExpressionFinder.BooAttributeContext.Instance) {
				AST.MethodInvocationExpression mie = expr as AST.MethodInvocationExpression;
				if (mie != null)
					expr = mie.Target;
				string name = expr.ToCodeString();
				SearchTypeResult searchTypeResult = pc.SearchType(new SearchTypeRequest(name, 0, callingClass, cu, caretLine, caretColumn));
				IReturnType rt = searchTypeResult.Result;
				if (rt != null && rt.GetUnderlyingClass() != null)
					return new TypeResolveResult(callingClass, callingMember, rt);
				rt = pc.SearchType(new SearchTypeRequest(name + "Attribute", 0, callingClass, cu, caretLine, caretColumn)).Result;
				if (rt != null && rt.GetUnderlyingClass() != null)
					return new TypeResolveResult(callingClass, callingMember, rt);
				if (BooProject.BooCompilerPC != null) {
					IClass c = BooProject.BooCompilerPC.GetClass("Boo.Lang." + char.ToUpper(name[0]) + name.Substring(1) + "Attribute", 0);
					if (c != null)
						return new TypeResolveResult(callingClass, callingMember, c);
				}
				string namespaceName = searchTypeResult.NamespaceResult;
				if (namespaceName != null) {
					return new NamespaceResolveResult(callingClass, callingMember, namespaceName);
				}
				return null;
			} else {
				if (expr.NodeType == AST.NodeType.ReferenceExpression) {
					// this could be a macro
					if (BooProject.BooCompilerPC != null) {
						string name = ((AST.ReferenceExpression)expr).Name;
						IClass c = BooProject.BooCompilerPC.GetClass("Boo.Lang." + char.ToUpper(name[0]) + name.Substring(1) + "Macro", 0);
						if (c != null)
							return new TypeResolveResult(callingClass, callingMember, c);
					}
				}
			}
			
			ResolveVisitor visitor = new ResolveVisitor(this);
			visitor.Visit(expr);
			ResolveResult result = visitor.ResolveResult;
			if (expressionResult.Context == ExpressionContext.Type && result is MixedResolveResult)
				result = (result as MixedResolveResult).TypeResult;
			return result;
		}
		
		public IReturnType ConvertType(AST.TypeReference typeRef)
		{
			return ConvertVisitor.CreateReturnType(typeRef, callingClass, callingMember,
			                                       caretLine, caretColumn, pc);
		}
		
		public IField FindLocalVariable(string name, bool acceptImplicit)
		{
			VariableLookupVisitor vlv = new VariableLookupVisitor(this, name, acceptImplicit);
			vlv.Visit(GetCurrentBooMethod());
			return vlv.Result;
		}
		#endregion
		
		#region CtrlSpace
		static IClass GetPrimitiveClass(IProjectContent pc, string systemType, string newName)
		{
			IClass c = pc.GetClass(systemType, 0);
			if (c == null) {
				LoggingService.Warn("Could not find " + systemType);
				return null;
			}
			DefaultClass c2 = new DefaultClass(c.CompilationUnit, newName);
			c2.ClassType = c.ClassType;
			c2.Modifiers = c.Modifiers;
			c2.Documentation = c.Documentation;
			c2.BaseTypes.AddRange(c.BaseTypes);
			c2.Methods.AddRange(c.Methods);
			c2.Fields.AddRange(c.Fields);
			c2.Properties.AddRange(c.Properties);
			c2.Events.AddRange(c.Events);
			return c2;
		}
		
		public List<ICompletionEntry> CtrlSpace(int caretLine, int caretColumn, ParseInformation parseInfo, string fileContent, ExpressionContext context)
		{
			List<ICompletionEntry> result = new List<ICompletionEntry>();
			
			if (!Initialize(parseInfo, caretLine, caretColumn))
				return null;
			if (context == ExpressionContext.Importable) {
				pc.AddNamespaceContents(result, "", pc.Language, true);
				CtrlSpaceResolveHelper.AddUsing(result, pc.DefaultImports, pc);
				return result;
			}
			
			CtrlSpaceResolveHelper.AddContentsFromCalling(result, callingClass, callingMember);
			AddImportedNamespaceContents(result);
			
			if (BooProject.BooCompilerPC != null) {
				if (context == ExpressionFinder.BooAttributeContext.Instance) {
					foreach (object o in BooProject.BooCompilerPC.GetNamespaceContents("Boo.Lang")) {
						IClass c = o as IClass;
						if (c != null && c.Name.EndsWith("Attribute") && !c.IsAbstract) {
							result.Add(GetPrimitiveClass(BooProject.BooCompilerPC, c.FullyQualifiedName, c.Name.Substring(0, c.Name.Length - 9).ToLowerInvariant()));
						}
					}
				} else {
					foreach (object o in BooProject.BooCompilerPC.GetNamespaceContents("Boo.Lang")) {
						IClass c = o as IClass;
						if (c != null && c.Name.EndsWith("Macro") && !c.IsAbstract) {
							result.Add(GetPrimitiveClass(BooProject.BooCompilerPC, c.FullyQualifiedName, c.Name.Substring(0, c.Name.Length - 5).ToLowerInvariant()));
						}
					}
				}
			}
			
			List<string> knownVariableNames = new List<string>();
			foreach (object o in result) {
				IMember m = o as IMember;
				if (m != null) {
					knownVariableNames.Add(m.Name);
				}
			}
			VariableListLookupVisitor vllv = new VariableListLookupVisitor(knownVariableNames, this);
			vllv.Visit(GetCurrentBooMethod());
			foreach (KeyValuePair<string, IReturnType> entry in vllv.Results) {
				result.Add(new DefaultField.LocalVariableField(entry.Value, entry.Key, DomRegion.Empty, callingClass));
			}
			
			return result;
		}
		
		// used by ctrl+space and resolve visitor (resolve identifier)
		public List<ICompletionEntry> GetImportedNamespaceContents()
		{
			List<ICompletionEntry> result = new List<ICompletionEntry>();
			AddImportedNamespaceContents(result);
			return result;
		}
		
		void AddImportedNamespaceContents(List<ICompletionEntry> list)
		{
			IClass c;
			foreach (KeyValuePair<string, string> pair in BooAmbience.ReverseTypeConversionTable) {
				c = GetPrimitiveClass(pc, pair.Value, pair.Key);
				if (c != null) list.Add(c);
			}
			list.Add(new DuckClass(cu));
			CtrlSpaceResolveHelper.AddImportedNamespaceContents(list, cu, callingClass);
		}
		
		internal class DuckClass : DefaultClass
		{
			public DuckClass(ICompilationUnit cu) : base(cu, "duck")
			{
				Documentation = "Use late-binding to access members of this type.<br/>\n'If it walks like a duck and quacks like a duck, it must be a duck.'";
				Modifiers = ModifierEnum.Public;
				Freeze();
			}
			
			protected override IReturnType CreateDefaultReturnType()
			{
				return new DuckReturnType(this);
			}
		}
		
		internal class DuckReturnType : AbstractReturnType
		{
			IClass c;
			public DuckReturnType(IClass c) {
				this.c = c;
				FullyQualifiedName = c.FullyQualifiedName;
			}
			public override IClass GetUnderlyingClass() {
				return c;
			}
			public override List<IMethod> GetMethods() {
				return new List<IMethod>();
			}
			public override List<IProperty> GetProperties() {
				return new List<IProperty>();
			}
			public override List<IField> GetFields() {
				return new List<IField>();
			}
			public override List<IEvent> GetEvents() {
				return new List<IEvent>();
			}
		}
		#endregion
	}
	
	sealed class BooCtrlSpaceCompletionItemProvider : CtrlSpaceCompletionItemProvider
	{
		public BooCtrlSpaceCompletionItemProvider() {}
		public BooCtrlSpaceCompletionItemProvider(ExpressionContext overrideContext) : base(overrideContext) {}
		
		protected override List<ICompletionEntry> CtrlSpace(ITextEditor editor, ExpressionContext context)
		{
			return new BooResolver().CtrlSpace(
				editor.Caret.Line,
				editor.Caret.Column,
				ParserService.GetParseInformation(editor.FileName),
				editor.Document.Text,
				context);
		}
	}
}
