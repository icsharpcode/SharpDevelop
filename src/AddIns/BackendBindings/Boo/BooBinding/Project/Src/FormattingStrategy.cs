// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
	
namespace Grunwald.BooBinding
{
	public class BooFormattingStrategy : DefaultFormattingStrategy
	{
		protected override int SmartIndentLine(TextArea area, int line)
		{
			IDocument document = area.Document;
			LineSegment previousLine = document.GetLineSegment(line-1);
			
			if (document.GetText(previousLine).EndsWith(":")) {
				LineSegment currentLine = document.GetLineSegment(line);
				string indentation = GetIndentation(area, line-1);
				indentation += Tab.GetIndentationString(document);
				document.Replace(currentLine.Offset,
				                 currentLine.Length,
				                 indentation + document.GetText(currentLine));
				return indentation.Length;
			}
			
			return base.SmartIndentLine(area, line);
		}
		
		// Deactivate indenting multiple lines with Ctrl-I
		public override void IndentLines(TextArea textArea, int begin, int end)
		{
		}
	}
}
