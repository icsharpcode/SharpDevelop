// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using Boo.Lang.Compiler;
using AST = Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Steps;
using NRResolver = ICSharpCode.SharpDevelop.Dom.NRefactoryResolver.NRefactoryResolver;

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
		IMember callingMember;
		
		public IClass CallingClass {
			get {
				return callingClass;
			}
		}
		
		public IMember CallingMember {
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
		#endregion
		
		#region Initialization
		bool Initialize(string fileName, int caretLine, int caretColumn)
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
			if (parseInfo == null) {
				return false;
			}
			this.cu = parseInfo.MostRecentCompilationUnit;
			this.pc = cu.ProjectContent;
			this.caretLine = caretLine;
			this.caretColumn = caretColumn;
			this.callingClass = cu.GetInnermostClass(caretLine, caretColumn);
			if (callingClass == null) {
				if (cu.Classes.Count == 0) return false;
				callingClass = cu.Classes[cu.Classes.Count - 1];
				if (!callingClass.Region.IsEmpty) {
					if (callingClass.Region.BeginLine > caretLine)
						return false;
				}
			}
			callingMember = ResolveCurrentMember();
			return true;
		}
		
		IMember ResolveCurrentMember()
		{
			LoggingService.DebugFormatted("Getting current method... caretLine = {0}, caretColumn = {1}", caretLine, caretColumn);
			if (callingClass == null) return null;
			IMember best = null;
			int line = 0;
			foreach (IMember m in callingClass.Methods) {
				if (m.Region.BeginLine <= caretLine && m.Region.BeginLine > line) {
					line = m.Region.BeginLine;
					best = m;
				}
			}
			foreach (IMember m in callingClass.Properties) {
				if (m.Region.BeginLine <= caretLine && m.Region.BeginLine > line) {
					line = m.Region.BeginLine;
					best = m;
				}
			}
			if (callingClass.Region.IsEmpty) {
				foreach (IMember m in callingClass.Methods) {
					if (best == null || best.Region.EndLine < caretLine)
						return m;
				}
			}
			return best;
		}
		#endregion
		
		#region Resolve
		public ResolveResult Resolve(ExpressionResult expressionResult,
		                             int caretLineNumber, int caretColumn,
		                             string fileName, string fileContent)
		{
			if (!Initialize(fileName, caretLineNumber, caretColumn))
				return null;
			AST.Expression expr = Boo.Lang.Parser.BooParser.ParseExpression("expression", expressionResult.Expression);
			if (expr is AST.IntegerLiteralExpression)
				return null; // no CC for "5."
			ResolveVisitor visitor = new ResolveVisitor(this);
			visitor.Visit(expr);
			return visitor.ResolveResult;
		}
		#endregion
		
		#region CtrlSpace
		IClass GetPrimitiveClass(string systemType, string newName)
		{
			IClass c = pc.GetClass(systemType);
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
		
		public ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent)
		{
			if (!Initialize(fileName, caretLine, caretColumn))
				return null;
			ArrayList result = GetImportedNamespaceContents();
			
			NRResolver.AddContentsFromCalling(result, callingClass, callingMember);
			
			return result;
		}
		
		// used by ctrl+space and resolve visitor (resolve identifier)
		public ArrayList GetImportedNamespaceContents()
		{
			ArrayList list = new ArrayList();
			IClass c;
			foreach (KeyValuePair<string, string> pair in BooAmbience.TypeConversionTable) {
				c = GetPrimitiveClass(pair.Key, pair.Value);
				if (c != null) list.Add(c);
			}
			NRResolver.AddImportedNamespaceContents(list, cu, callingClass);
			return list;
		}
		#endregion
	}
}
