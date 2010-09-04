// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
