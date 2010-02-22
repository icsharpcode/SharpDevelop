// SharpDevelop samples
// Copyright (c) 2010, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.HtmlSyntaxColorizer
{
	public class HtmlWriter
	{
		public bool AlternateLineBackground;
		public bool ShowLineNumbers;
		
		public string MainStyle = "font-size: small; font-family: Consolas, \"Courier New\", Courier, Monospace;";
		public string LineStyle = "margin: 0em;";
		public string AlternateLineStyle = "margin: 0em; width: 100%; background-color: #f0f0f0;";
		
		#region Stylesheet cache
		/// <summary>
		/// Specifies whether a CSS stylesheet should be created to reduce the size of the created code.
		/// The default value is true.
		/// </summary>
		public bool CreateStylesheet = true;
		
		/// <summary>
		/// Specifies the prefix to use in front of generate style class names.
		/// </summary>
		public string StyleClassPrefix = "code";
		
		Dictionary<string, string> stylesheetCache = new Dictionary<string, string>();
		StringBuilder stylesheet = new StringBuilder();
		
		/// <summary>
		/// Resets the CSS stylesheet cache. Stylesheet classes will be cached on GenerateHtml calls.
		/// If you want to reuse the HtmlWriter for multiple .html files.
		/// </summary>
		public void ResetStylesheetCache()
		{
			stylesheetCache.Clear();
		}
		
		string GetClass(string style)
		{
			string className;
			if (!stylesheetCache.TryGetValue(style, out className)) {
				className = StyleClassPrefix + stylesheetCache.Count;
				stylesheet.Append('.');
				stylesheet.Append(className);
				stylesheet.Append(" { ");
				stylesheet.Append(style);
				stylesheet.AppendLine(" }");
				stylesheetCache[style] = className;
			}
			return className;
		}
		#endregion
		
		public string GenerateHtml(string code, IHighlightingDefinition highlightDefinition)
		{
			return GenerateHtml(new TextDocument(code), highlightDefinition);
		}
		
		public string GenerateHtml(TextDocument document, IHighlightingDefinition highlightDefinition)
		{
			return GenerateHtml(document, new DocumentHighlighter(document, highlightDefinition.MainRuleSet));
		}
		
		public string GenerateHtml(TextDocument document, IHighlighter highlighter)
		{
			string myMainStyle = MainStyle;
			string LineNumberStyle = "color: #606060;";
			
			StringWriter output = new StringWriter();
			if (ShowLineNumbers || AlternateLineBackground) {
				output.Write("<div");
				WriteStyle(output, myMainStyle);
				output.WriteLine(">");
				
				int longestNumberLength = 1 + (int)Math.Log10(document.LineCount);
				
				for (int lineNumber = 1; lineNumber <= document.LineCount; lineNumber++) {
					HighlightedLine line = highlighter.HighlightLine(lineNumber);
					output.Write("<pre");
					if (AlternateLineBackground && (lineNumber % 2) == 0) {
						WriteStyle(output, AlternateLineStyle);
					} else {
						WriteStyle(output, LineStyle);
					}
					output.Write(">");
					
					if (ShowLineNumbers) {
						output.Write("<span");
						WriteStyle(output, LineNumberStyle);
						output.Write('>');
						output.Write(lineNumber.ToString().PadLeft(longestNumberLength));
						output.Write(":  ");
						output.Write("</span>");
					}
					
					PrintWords(output, line);
					output.WriteLine("</pre>");
				}
				output.WriteLine("</div>");
			} else {
				output.Write("<pre");
				WriteStyle(output, myMainStyle + LineStyle);
				output.WriteLine(">");
				for (int lineNumber = 1; lineNumber <= document.LineCount; lineNumber++) {
					HighlightedLine line = highlighter.HighlightLine(lineNumber);
					PrintWords(output, line);
					output.WriteLine();
				}
				output.WriteLine("</pre>");
			}
			if (CreateStylesheet && stylesheet.Length > 0) {
				string result = "<style type=\"text/css\">" + stylesheet.ToString() + "</style>" + output.ToString();
				stylesheet = new StringBuilder();
				return result;
			} else {
				return output.ToString();
			}
		}
		
		void PrintWords(TextWriter writer, HighlightedLine line)
		{
			writer.Write(line.ToHtml(new MyHtmlOptions(this)));
		}
		
		class MyHtmlOptions : HtmlOptions
		{
			readonly HtmlWriter htmlWriter;
			
			internal MyHtmlOptions(HtmlWriter htmlWriter)
			{
				this.htmlWriter = htmlWriter;
			}
			
			public override void WriteStyleAttributeForColor(TextWriter writer, HighlightingColor color)
			{
				htmlWriter.WriteStyle(writer, color.ToCss());
			}
		}
		
		void WriteStyle(TextWriter writer, string style)
		{
			if (CreateStylesheet) {
				writer.Write(" class=\"");
				writer.Write(GetClass(style));
				writer.Write('"');
			} else {
				writer.Write(" style='");
				writer.Write(style);
				writer.Write("'");
			}
		}
	}
}
