// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.XamlBinding
{
	public class XamlFullParseInformation : ParseInformation
	{
		readonly AXmlDocument document;
		readonly ITextSource text;
		
		public XamlFullParseInformation(XamlParsedFile parsedFile, AXmlDocument document, ITextSource text)
			: base(parsedFile, true)
		{
			if (parsedFile == null)
				throw new ArgumentNullException("parsedFile");
			if (document == null)
				throw new ArgumentNullException("document");
			if (text == null)
				throw new ArgumentNullException("text");
			this.document = document;
			this.text = text;
		}
		
		public new XamlParsedFile ParsedFile {
			get { return (XamlParsedFile)base.ParsedFile; }
		}
		
		public AXmlDocument Document {
			get { return document; }
		}
		
		public ITextSource Text {
			get { return text; }
		}
	}
}
