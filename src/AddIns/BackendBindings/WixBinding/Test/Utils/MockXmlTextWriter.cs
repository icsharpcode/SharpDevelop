// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using System.Xml;

using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.WixBinding;

namespace WixBinding.Tests.Utils
{
	public class MockXmlTextWriter : WixTextWriter
	{
		StringBuilder xml = new StringBuilder();
		XmlWriterSettings settings;
		
		public MockXmlTextWriter(ITextEditorOptions options)
			: base(options)
		{
		}
		
		protected override XmlWriter Create(string fileName, XmlWriterSettings settings)
		{
			this.settings = settings;
			
			return XmlTextWriter.Create(xml, settings);
		}
		
		public string GetXmlWritten()
		{
			return xml.ToString();
		}
		
		public XmlWriterSettings XmlWriterSettingsPassedToCreateMethod {
			get { return settings; }
		}
	}
}
