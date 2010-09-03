// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using System.Xml.Serialization;

namespace MSHelpSystem.Helper
{
	[XmlRoot("mshelpsystem")]
	public class Help3Configuration
	{
		public Help3Configuration()
		{
		}

		string activeCatalogId = string.Empty;
		bool offlineMode = true;
		bool externalHelp = false;

		[XmlElement("activeCatalog")]
		public string ActiveCatalogId
		{
			get { return activeCatalogId; }
			set { activeCatalogId = value; }
		}

		[XmlElement("offlineMode")]
		public bool OfflineMode
		{
			get { return offlineMode; }
			set { offlineMode = value; }
		}

		[XmlElement("externalHelp")]
		public bool ExternalHelp
		{
			get { return externalHelp; }
			set { externalHelp = value; }
		}
	}
}
