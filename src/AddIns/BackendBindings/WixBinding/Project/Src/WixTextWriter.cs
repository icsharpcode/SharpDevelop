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
using System.IO;
using System.Xml;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.WixBinding
{
	public class WixTextWriter
	{
		ITextEditorOptions textEditorOptions;
		
		public WixTextWriter(ITextEditorOptions textEditorOptions)
		{
			this.textEditorOptions = textEditorOptions;
		}
		
		public XmlWriterSettings CreateXmlWriterSettings()
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CloseOutput = true;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.NewLineChars = "\r\n";
			xmlWriterSettings.OmitXmlDeclaration = true;
			
			if (textEditorOptions.ConvertTabsToSpaces) {
				string space = " ";
				xmlWriterSettings.IndentChars = space.PadRight(textEditorOptions.IndentationSize);
			} else {
				xmlWriterSettings.IndentChars = "\t";
			}
			return xmlWriterSettings;
		}
		
		public XmlWriter Create(TextWriter textWriter)
		{
			XmlWriterSettings settings = CreateXmlWriterSettings();
			return XmlTextWriter.Create(textWriter, settings);
		}
		
		public XmlWriter Create(string fileName)
		{
			XmlWriterSettings settings = CreateXmlWriterSettings();
			return Create(fileName, settings);
		}
		
		protected virtual XmlWriter Create(string fileName, XmlWriterSettings settings)
		{
			return XmlTextWriter.Create(fileName, settings);
		}
	}
}
