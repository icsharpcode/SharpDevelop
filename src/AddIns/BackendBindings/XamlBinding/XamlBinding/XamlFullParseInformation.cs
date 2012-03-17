// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.XamlBinding
{
	public class XamlFullParseInformation : ParseInformation
	{
		readonly AXmlDocument document;
		
		public XamlFullParseInformation(XamlParsedFile parsedFile, AXmlDocument document)
			: base(parsedFile, true)
		{
			if (parsedFile == null)
				throw new ArgumentNullException("parsedFile");
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
		}
		
		public new XamlParsedFile ParsedFile {
			get { return (XamlParsedFile)base.ParsedFile; }
		}
		
		public AXmlDocument Document {
			get { return document; }
		}
	}
}
