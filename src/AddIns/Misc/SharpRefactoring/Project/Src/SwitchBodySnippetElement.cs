// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring
{
	/// <summary>
	/// The snippet element inserted inside the body curly braces.
	/// When snippet interactive mode ends, resolves type of switch condition and generates switch cases.
	/// </summary>
	public class SwitchBodySnippetElement : SnippetElement
	{
		InsertionContext context;
		TextAnchor anchor;
		ClassFinder classFinderContext;
		
		/// <summary>
		/// The ITextEditor in which this switch body element has been inserted.
		/// </summary>
		public ITextEditor Editor {
			get { return context.TextArea.GetService(typeof(ITextEditor)) as ITextEditor; }
		}
		
		/// <summary>
		/// Called when this switch body element is inserted to the editor.
		/// </summary>
		public override void Insert(InsertionContext context)
		{
			this.context = context;
			this.context.Deactivated += new EventHandler<SnippetEventArgs>(InteractiveModeCompleted);
			this.anchor = SetUpAnchorAtInsertion(context);
			this.classFinderContext = new ClassFinder(ParserService.ParseCurrentViewContent(), Editor.Document.Text, Editor.Caret.Offset);
		}

		TextAnchor SetUpAnchorAtInsertion(InsertionContext context)
		{
			var anchor = context.Document.CreateAnchor(context.InsertionPosition);
			anchor.MovementType = ICSharpCode.AvalonEdit.Document.AnchorMovementType.BeforeInsertion;
			return anchor;
		}
		
		int InsertionPosition { get { return this.anchor.Offset; } }
		
		/// <summary>
		/// Main logic of switch snippet. Called when user ends the snippet interactive mode.
		/// Inserts switch body depending on the type of switch condition:
		/// Inserts all cases if condition is enum, generic switch body with one case otherwise.
		/// </summary>
		void InteractiveModeCompleted(object sender, SnippetEventArgs e)
		{
			if (e.Reason != DeactivateReason.ReturnPressed)
				return;
			
			int offset;
			string conditionText = GetSwitchConditionText(this.context, out offset);
			
			IReturnType conditionType = ResolveConditionType(conditionText, offset);
			
			string switchBodyIndent = GetBodyIndent(this.Editor.Document, this.InsertionPosition);
			
			if (conditionType != null && IsEnum(conditionType)) {
				GenerateEnumSwitchBodyCode(this.InsertionPosition, conditionType, switchBodyIndent);
			} else {
				GenerateGenericSwitchBodyCode(this.InsertionPosition, switchBodyIndent);
			}
		}
		
		/// <summary>
		/// Inserts switch body for enum to the given offset.
		/// </summary>
		/// <param name="indent">Whole text will be indented by this value.</param>
		void GenerateEnumSwitchBodyCode(int offset, IReturnType enumType, string indent)
		{
			string switchBody = GetSwitchBodyCode(enumType, indent, GetCodeGeneratorForCurrentFile());
			this.Editor.Document.Insert(offset, switchBody);
		}
		
		/// <summary>
		/// Inserts generic switch body to the given offset.
		/// </summary>
		/// <param name="indent">Whole text will be indented by this value.</param>
		void GenerateGenericSwitchBodyCode(int offset, string indent)
		{
			string switchBody = GetGenericBodyCode(indent, GetCodeGeneratorForCurrentFile());
			this.Editor.Document.Insert(offset, switchBody);
		}
		
		// this should use CodeGenerator and build a BlockStatement to work for both C# and VB
		string GetSwitchBodyCode(IReturnType enumType, string indent, CodeGenerator generator)
		{
			if (generator == null)
				return string.Empty;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(this.Editor.FileName);
			var visitor = new CSharpOutputVisitor();
			CodeGenerator.ConvertType(enumType, this.classFinderContext).AcceptVisitor(visitor, null);
			var qualifiedEnumType = visitor.Text;
			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach (var enumCase in GetEnumCases(enumType)) {
				string qualifiedName = qualifiedEnumType + "." + enumCase.Name;
				sb.AppendLine(string.Format((first ? "" : indent) + "case {0}:", qualifiedName));
				sb.AppendLine(indent + context.Tab);
				sb.AppendLine(indent + context.Tab + "break;");
				first = false;
			}
			sb.AppendLine(indent + "default:");
			sb.Append(string.Format(indent + context.Tab + "throw new Exception(\"Invalid value for {0}\");", enumType.Name));
			return sb.ToString();
		}
		
		// this should use CodeGenerator and build a BlockStatement to work for both C# and VB
		string GetGenericBodyCode(string indent, CodeGenerator generator)
		{
			if (generator == null)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(/*indent +*/ "case :");
			sb.AppendLine(indent + context.Tab);
			sb.AppendLine(indent + context.Tab + "break;");
			sb.AppendLine(indent + "default:");
			sb.AppendLine(indent + context.Tab);
			sb.Append(indent + "\tbreak;");
			return sb.ToString();
		}
		
		bool IsEnum(IReturnType type)
		{
			var typeClass = type.GetUnderlyingClass();
			if (typeClass == null)
				// eg. MethodGroup type has no UnderlyingClass
				return false;
			return typeClass.ClassType == ICSharpCode.SharpDevelop.Dom.ClassType.Enum;
		}
		
		/// <summary>
		/// Gets enum values out of enum type.
		/// </summary>
		IEnumerable<IField> GetEnumCases(IReturnType enumType)
		{
			var typeClass = enumType.GetUnderlyingClass();
			if (typeClass == null) {
				// eg. MethodGroup type has no UnderlyingClass
				return Enumerable.Empty<IField>();
			}
			return typeClass.Fields;
		}

		/// <summary>
		/// Assuming that interactive mode of 'switch' snippet has currently finished in context,
		/// returns the switch condition that user entered.
		/// </summary>
		string GetSwitchConditionText(InsertionContext context, out int conditionEndOffset)
		{
			var snippetActiveElements = context.ActiveElements.ToList();
			if (snippetActiveElements.Count == 0)
				throw new InvalidOperationException("Switch snippet should have at least one active element");
			var switchConditionElement = snippetActiveElements[0] as IReplaceableActiveElement;
			if (switchConditionElement == null)
				throw new InvalidOperationException("Switch snippet condition should be " + typeof(IReplaceableActiveElement).Name);
			if (switchConditionElement.Segment  == null)
				throw new InvalidOperationException("Swith condition should have a start offset");
			conditionEndOffset = switchConditionElement.Segment.EndOffset - 1;
			return switchConditionElement.Text;
		}
		
		/// <summary>
		/// Resolves the Dom.IReturnType of expression ending at offset, ie. the switch condition expression.
		/// </summary>
		IReturnType ResolveConditionType(string conditionExpression, int conditionEndOffset)
		{
			ExpressionResult expressionResult = new ExpressionResult(conditionExpression);
			Location location = this.Editor.Document.OffsetToPosition(conditionEndOffset);
			var result = ParserService.Resolve(expressionResult, location.Line, location.Column, this.Editor.FileName, this.Editor.Document.Text);
			return result.ResolvedType;
		}
		
		/// <summary>
		/// Returns CodeGenerator for C# or VB depending on the current file where snippet is being inserted.
		/// </summary>
		CodeGenerator GetCodeGeneratorForCurrentFile()
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(this.Editor.FileName);
			if (parseInfo != null) {
				return parseInfo.CompilationUnit.Language.CodeGenerator;
			}
			return null;
		}
		
		/// <summary>
		/// Get start of the next line (that is, from offset, go one line down and to its start).
		/// </summary>
		int GetNextLineStart(IDocument document, int offset)
		{
			int nextLineNuber = document.GetLineForOffset(offset).LineNumber + 1;
			return document.GetLine(nextLineNuber).Offset;
		}
		
		/// <summary>
		/// Indents the switch body by the same indent the whole snippet has and adds one TAB.
		/// </summary>
		string GetBodyIndent(IDocument document, int offset)
		{
			int lineStart = document.GetLineForOffset(offset).Offset;
			string indent = DocumentUtilitites.GetWhitespaceAfter(document, lineStart);
			return indent;
		}
	}
}
