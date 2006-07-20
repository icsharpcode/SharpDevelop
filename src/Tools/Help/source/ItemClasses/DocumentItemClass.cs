//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.ItemClasses
{
	using System;
	using System.Globalization;
	using System.Xml;
	using System.Xml.XPath;
	using HtmlHelp2Registration.HelperClasses;

	public class DocumentItemClass
	{
		private string id;
		private string hxs;
		private string hxi;
		private string hxq;
		private string hxr;
		private int languageId;
		private int hxsMediaId;
		private int hxqMediaId;
		private int hxrMediaId;
		private int sampleMediaId;

		public DocumentItemClass(XPathNavigator rootFilenode)
		{
			if (rootFilenode == null)
			{
				throw new ArgumentNullException("rootFilenode");
			}

			this.id            = XmlHelperClass.GetXmlStringValue(rootFilenode, "@Id");
			this.hxs           = XmlHelperClass.GetXmlStringValue(rootFilenode, "@HxS");
			this.hxi           = XmlHelperClass.GetXmlStringValue(rootFilenode, "@HxI");
			this.hxq           = XmlHelperClass.GetXmlStringValue(rootFilenode, "@HxQ");
			this.hxr           = XmlHelperClass.GetXmlStringValue(rootFilenode, "@HxR");
			this.languageId    = XmlHelperClass.GetXmlIntValue(rootFilenode, "@LangId", 1033);
			this.hxsMediaId    = XmlHelperClass.GetXmlIntValue(rootFilenode, "@HxSMediaId", 0);
			this.hxqMediaId    = XmlHelperClass.GetXmlIntValue(rootFilenode, "@HxQMediaId", 0);
			this.hxrMediaId    = XmlHelperClass.GetXmlIntValue(rootFilenode, "@HxRMediaId", 0);
			this.sampleMediaId = XmlHelperClass.GetXmlIntValue(rootFilenode, "@SampleMediaId", 0);
		}

		#region Properties
		public string Id
		{
			get { return this.id; }
		}

		public string Hxs
		{
			get { return this.hxs; }
		}

		public string Hxi
		{
			get{ return this.hxi; }
		}

		public string Hxq
		{
			get { return this.hxq; }
		}

		public string Hxr
		{
			get { return this.hxr; }
		}

		public int LanguageId
		{
			get { return this.languageId; }
		}

		public int HxsMediaId
		{
			get { return this.hxsMediaId; }
		}

		public int HxqMediaId
		{
			get { return this.hxqMediaId; }
		}

		public int HxrMediaId
		{
			get { return this.hxrMediaId; }
		}

		public int SampleMediaId
		{
			get { return this.sampleMediaId; }
		}
		#endregion

		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "[Help 2.0 Document; {0}, {1}]", this.id, this.languageId);
		}
	}
}
