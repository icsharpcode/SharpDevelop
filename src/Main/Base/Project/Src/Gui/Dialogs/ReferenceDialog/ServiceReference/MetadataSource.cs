// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Serialization;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class MetadataSource
	{
		public MetadataSource()
		{
		}
		
		public MetadataSource(string url)
		{
			Address = url;
			Protocol = "http";
		}
		
		[XmlAttribute]
		public string Address { get; set; }
		
		[XmlAttribute]
		public string Protocol { get; set; }
		
		[XmlAttribute]
		public string SourceId { get; set; }
	}
}
