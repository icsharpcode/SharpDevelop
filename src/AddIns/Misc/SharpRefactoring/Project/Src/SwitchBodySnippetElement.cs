// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
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
	public class SwitchBodySnippetElement : SnippetElement//, IActiveElement would add this.Segment to know where to insert
	{
		InsertionContext context;
		
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
		}

		/// <summary>
		/// Main logic of switch spinnet. Called when user ends the snippet interactive mode.
		/// Inserts switch body depending on the type of switch condition:
		/// Inserts all cases if condition is enum, generic switch body with one case otherwise.
		/// </summary>
		void InteractiveModeCompleted(object sender, SnippetEventArgs e)
		{
			if (e.Reason != DeactivateReason.ReturnPressed)
				return;
			
			int offset;
			// conditionText is currently not needed, 
			// could be useful if NRefactory had a public method for resolving string expressions 
			// (NRefactoryResolver.ParseExpression is private)
			string conditionText = GetSwitchConditionText(this.context, out offset);
			IReturnType conditionType = ResolveConditionType(offset);
			
			int lineStart = this.Editor.Document.GetLineForOffset(offset).Offset;
			// indent the whole switch body appropriately
			string indent = DocumentUtilitites.GetWhitespaceAfter(this.Editor.Document, lineStart);
			indent += '\t';
			// switch body starts at next line
			int switchBodyOffset = GetNextLineStart(this.Editor.Document, offset);
			                                      
			if (conditionType != null && IsEnum(conditionType)) {
				GenerateEnumSwitchBodyCode(switchBodyOffset, conditionType, indent);
			} else {
				GenerateGenericSwitchBodyCode(switchBodyOffset, indent);
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
		
		// TODO this should use CodeGenerator and build a BlockStatement to work for both C# and VB
		string GetSwitchBodyCode(IReturnType enumType, string indent, CodeGenerator generator)
		{
			if (generator == null)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			foreach (var enumCase in GetEnumCases(enumType)) {
				sb.AppendLine(string.Format(indent + "case {0}:", enumCase.Name));
				sb.AppendLine(indent + '\t');
				sb.AppendLine(indent + "\tbreak;");
			}
			sb.AppendLine(indent + "default:");
			sb.AppendLine(string.Format(indent + "\tthrow new Exception(\"Invalid value for {0}\");", enumType.Name));
			return sb.ToString();
		}
		
		// TODO this should use CodeGenerator and build a BlockStatement to work for both C# and VB
		string GetGenericBodyCode(string indent, CodeGenerator generator)
		{
			if (generator == null)
				return string.Empty;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(indent + "case :");
			sb.AppendLine(indent + '\t');
			sb.AppendLine(indent + "\tbreak;");
			sb.AppendLine(indent + "default:");
			sb.AppendLine(indent + '\t');
			sb.AppendLine(indent + "\tbreak;");
			return sb.ToString();
		}
		
		bool IsEnum(IReturnType type)
		{
			var typeClass = type.GetUnderlyingClass();
			if (typeClass == null)
				// eg. MethodGroup type has no UnderlyingClass
				return false;
			return typeClass.BaseClass.DotNetName == "System.Enum";
		}
		
		/// <summary>
		/// Gets enum values out of enum type.
		/// </summary>
		IEnumerable<DefaultField> GetEnumCases(IReturnType enumType)
		{
			var typeClass = enumType.GetUnderlyingClass();
			if (typeClass == null)
				// eg. MethodGroup type has no UnderlyingClass
				yield break;
			foreach (var enumValue in typeClass.AllMembers) {
				yield return enumValue as DefaultField;
			}
		}

		/// <summary>
		/// Assuming that interactive mode of 'switch' snippet has currently finished in context,
		/// returns the switch condition that user entered.
		/// </summary>
		string GetSwitchConditionText(InsertionContext context, out int startOffset)
		{
			var snippetActiveElements = context.ActiveElements.ToList();
			if (snippetActiveElements.Count == 0)
				throw new InvalidOperationException("Switch snippet should have at least one active element");
			var switchConditionElement = snippetActiveElements[0] as IReplaceableActiveElement;
			if (switchConditionElement == null)
				throw new InvalidOperationException("Switch snippet condition should be " + typeof(IReplaceableActiveElement).Name);
			if (switchConditionElement.Segment  == null)
				throw new InvalidOperationException("Swith condition should have a start offset");
			startOffset = switchConditionElement.Segment.EndOffset - 1;
			return switchConditionElement.Text;
		}
		
		/// <summary>
		/// Resolves the Dom.IReturnType of expression ending at offset, ie. the switch condition expression.
		/// </summary>
		IReturnType ResolveConditionType(int offset)
		{
			// this could be just solved by making NRefactoryResolver.ParseExpression public and passing the string
			// - no need for offset and expressionFinder / ExpressionResult
			ITextEditor textEditor = this.Editor;
			if (textEditor == null)
				return null;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textEditor.FileName);
			if (expressionFinder == null)
				return null;
			ExpressionResult expressionResult = expressionFinder.FindFullExpression(textEditor.Document.Text, offset);
			
			Location location = textEditor.Document.OffsetToPosition(offset);
			var result = ParserService.Resolve(expressionResult, location.Line, location.Column, textEditor.FileName, textEditor.Document.Text);
			return result.ResolvedType;
			
			/*
			// alternate way to ParserService.Resolve
			var resolver = new NRefactoryResolver(textEditor.Language.Properties);
			ResolveResult rr = resolver.Resolve(expressionResult, ParserService.GetParseInformation(textEditor.FileName), textEditor.Document.Text);
			
			return rr.ResolvedType;*/
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
	}
}
