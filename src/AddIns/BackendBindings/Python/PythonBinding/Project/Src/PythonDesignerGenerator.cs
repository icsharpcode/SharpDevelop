// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Text;

using ICSharpCode.FormsDesigner;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PythonBinding
{	
	/// <summary>
	/// Form's designer generator for the Python language.
	/// </summary>
	public class PythonDesignerGenerator : ScriptingDesignerGenerator
	{
		public PythonDesignerGenerator(ITextEditorOptions textEditorOptions)
			: base(textEditorOptions)
		{
		}
		
		public override IScriptingCodeDomSerializer CreateCodeDomSerializer(ITextEditorOptions options)
		{
			return new PythonCodeDomSerializer(options.IndentationString);
		}
		
		/// <summary>
		/// Returns the generated event handler.
		/// </summary>
		public override string CreateEventHandler(string eventMethodName, string body, string indentation)
		{			
			if (String.IsNullOrEmpty(body)) {
				body = "pass";
			}

			StringBuilder eventHandler = new StringBuilder();
			
			eventHandler.Append(indentation);
			eventHandler.Append("def ");
			eventHandler.Append(eventMethodName);
			eventHandler.Append("(self, sender, e):");
			eventHandler.AppendLine();			
			eventHandler.Append(indentation);
			eventHandler.Append(TextEditorOptions.IndentationString);
			eventHandler.Append(body);
			
			return eventHandler.ToString();
		}
	
		/// <summary>
		/// Converts from the DOM region to a document region.
		/// </summary>
		public override DomRegion GetBodyRegionInDocument(IMethod method)
		{
			DomRegion bodyRegion = method.BodyRegion;
			return new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine + 1, 1);			
		}
		
		public override int InsertEventHandler(IDocument document, string eventHandler)
		{
			int line = document.TotalNumberOfLines;
			IDocumentLine lastLineSegment = document.GetLine(line);
			int offset = lastLineSegment.Offset + lastLineSegment.Length;

			string newContent = "\r\n" + eventHandler;
			if (lastLineSegment.Length > 0) {
				// Add an extra new line between the last line and the event handler.
				newContent = "\r\n" + newContent;
			}
			document.Insert(offset, newContent);
			
			// Set position so it points to the line
			// where the event handler was inserted.
			return line + 1;
		}
	}
}
