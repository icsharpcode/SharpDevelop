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
		
		public XamlFullParseInformation(XamlUnresolvedFile unresolvedFile, AXmlDocument document, ITextSource text)
			: base(unresolvedFile, text.Version, true)
		{
			if (unresolvedFile == null)
				throw new ArgumentNullException("unresolvedFile");
			if (document == null)
				throw new ArgumentNullException("document");
			if (text == null)
				throw new ArgumentNullException("text");
			this.document = document;
			this.text = text;
		}
		
		public new XamlUnresolvedFile UnresolvedFile {
			get { return (XamlUnresolvedFile)base.UnresolvedFile; }
		}
		
		public AXmlDocument Document {
			get { return document; }
		}
		
		public ITextSource Text {
			get { return text; }
		}
		
		// XAML does not use IParser-based folding, but uses XML folding.
		public override bool SupportsFolding {
			get { return false; }
		}
	}
}
