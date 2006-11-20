// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.SharpDevelop.Dom.Refactoring
{
	public abstract class NRefactoryCodeGenerator : CodeGenerator
	{
		public abstract IOutputAstVisitor CreateOutputVisitor();
		
		public override string GenerateCode(AbstractNode node, string indentation)
		{
			IOutputAstVisitor visitor = CreateOutputVisitor();
			int indentCount = 0;
			foreach (char c in indentation) {
				if (c == '\t')
					indentCount += 4;
				else
					indentCount += 1;
			}
			visitor.OutputFormatter.IndentationLevel = indentCount / 4;
			if (node is Statement)
				visitor.OutputFormatter.Indent();
			node.AcceptVisitor(visitor, null);
			string text = visitor.Text;
			if (node is Statement && !text.EndsWith("\n"))
				text += Environment.NewLine;
			return text;
		}
	}
	
	public class CSharpCodeGenerator : NRefactoryCodeGenerator
	{
		public static readonly CSharpCodeGenerator Instance = new CSharpCodeGenerator();
		
		public override IOutputAstVisitor CreateOutputVisitor()
		{
			return new CSharpOutputVisitor();
		}
		
		/// <summary>
		/// Ensure that code is inserted correctly in {} code blocks - SD2-1180
		/// </summary>
		public override void InsertCodeAtEnd(DomRegion region, IDocument document, params AbstractNode[] nodes)
		{
			string beginLineIndentation = GetIndentation(document, region.BeginLine);
			int insertionLine = region.EndLine - 1;
			
			IDocumentLine endLine = document.GetLine(region.EndLine);
			string endLineText = endLine.Text;
			int originalPos = region.EndColumn - 2; // -1 for column coordinate => offset, -1 because EndColumn is after the '}'
			int pos = originalPos;
			if (pos >= endLineText.Length || endLineText[pos] != '}') {
				LoggingService.Warn("CSharpCodeGenerator.InsertCodeAtEnd: position is invalid (not pointing to '}')"
				                    + " endLineText=" + endLineText + ", pos=" + pos);
			} else {
				for (pos--; pos >= 0; pos--) {
					if (!char.IsWhiteSpace(endLineText[pos])) {
						// range before '}' is not empty: we cannot simply insert in the line before the '}', so
						// 
						pos++; // set pos to first whitespace character / the '{' character
						if (pos < originalPos) {
							// remove whitespace between last non-white character and the '}'
							document.Remove(endLine.Offset + pos, originalPos - pos);
						}
						// insert newline and same indentation as used in beginLine before the '}'
						document.Insert(endLine.Offset + pos, Environment.NewLine + beginLineIndentation);
						insertionLine++;
						
						pos = region.BeginColumn - 1;
						if (region.BeginLine == region.EndLine && pos >= 1 && pos < endLineText.Length) {
							// The whole block was in on a single line, e.g. "get { return field; }".
							// Insert an additional newline after the '{'.
							
							originalPos = pos = endLineText.IndexOf('{', pos);
							if (pos >= 0 && pos < region.EndColumn - 1) {
								// find next non-whitespace after originalPos
								originalPos++; // point to insertion position for newline after {
								for (pos++; pos < endLineText.Length; pos++) {
									if (!char.IsWhiteSpace(endLineText[pos])) {
										// remove all between originalPos and pos
										if (originalPos < pos) {
											document.Remove(endLine.Offset + originalPos, pos - originalPos);
										}
										document.Insert(endLine.Offset + originalPos, Environment.NewLine + beginLineIndentation + '\t');
										insertionLine++;
										break;
									}
								}
							}
						}
						break;
					}
				}
			}
			InsertCodeAfter(insertionLine, document, beginLineIndentation + '\t', nodes);
		}
	}
	
	public class VBNetCodeGenerator : NRefactoryCodeGenerator
	{
		public static readonly VBNetCodeGenerator Instance = new VBNetCodeGenerator();
		
		public override IOutputAstVisitor CreateOutputVisitor()
		{
			return new VBNetOutputVisitor();
		}
		
		public override PropertyDeclaration CreateProperty(IField field, bool createGetter, bool createSetter)
		{
			string propertyName = GetPropertyName(field.Name);
			if (string.Equals(propertyName, field.Name, StringComparison.InvariantCultureIgnoreCase)) {
				if (HostCallback.RenameMember(field, "m_" + field.Name)) {
					field = new DefaultField(field.ReturnType, "m_" + field.Name,
					                         field.Modifiers, field.Region, field.DeclaringType);
				}
			}
			return base.CreateProperty(field, createGetter, createSetter);
		}
	}
}
