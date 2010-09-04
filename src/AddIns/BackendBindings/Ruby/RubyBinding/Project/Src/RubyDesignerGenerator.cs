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

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Form's designer generator for the Ruby language.
	/// </summary>
	public class RubyDesignerGenerator : ScriptingDesignerGenerator
	{
		public RubyDesignerGenerator(ITextEditorOptions textEditorOptions)
			: base(textEditorOptions)
		{
		}
		
		public override IScriptingCodeDomSerializer CreateCodeDomSerializer(ITextEditorOptions options)
		{
			return new RubyCodeDomSerializer(options.IndentationString);
		}
		
		/// <summary>
		/// Returns the generated event handler.
		/// </summary>
		public override string CreateEventHandler(string eventMethodName, string body, string indentation)
		{			
			if (String.IsNullOrEmpty(body)) {
				body = String.Empty;
			}

			StringBuilder eventHandler = new StringBuilder();
			
			eventHandler.Append(indentation);
			eventHandler.Append("def ");
			eventHandler.Append(eventMethodName);
			eventHandler.Append("(sender, e)");
			eventHandler.AppendLine();
			eventHandler.Append(indentation);
			eventHandler.Append(TextEditorOptions.IndentationString);
			eventHandler.Append(body);
			eventHandler.AppendLine();
			eventHandler.Append(indentation);
			eventHandler.Append("end");
			
			return eventHandler.ToString();
		}
		
		/// <summary>
		/// Insert the event handler at the end of the class with an extra 
		/// new line before it.
		/// </summary>
		public override int InsertEventHandler(IDocument document, string eventHandler)
		{
			IDocumentLine classEndLine = GetClassEndLine(document);
			InsertEventHandlerBeforeLine(document, eventHandler, classEndLine);
				
			// Set position so it points to the line
			// where the event handler was inserted.
			return classEndLine.LineNumber;
		}
		
		IDocumentLine GetClassEndLine(IDocument doc)
		{
			int line = doc.TotalNumberOfLines;
			while (line > 0) {
				IDocumentLine documentLine = doc.GetLine(line);
				if (documentLine.Text.Trim() == "end") {
					return documentLine;
				}
				line--;
			}
			return doc.GetLine(doc.TotalNumberOfLines);
		}
		
		void InsertEventHandlerBeforeLine(IDocument doc, string eventHandler, IDocumentLine classEndLine)
		{
			string newContent = "\r\n" + eventHandler + "\r\n";
			int offset = classEndLine.Offset;
			doc.Insert(offset, newContent);
		}
		
		/// <summary>
		/// Converts from the DOM region to a document region.
		/// </summary>
		public override DomRegion GetBodyRegionInDocument(IMethod method)
		{
			DomRegion bodyRegion = method.BodyRegion;
			return new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine, 1);			
		}		
	}
}
