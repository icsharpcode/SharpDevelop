// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
