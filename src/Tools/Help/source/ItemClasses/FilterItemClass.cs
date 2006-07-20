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

	public class FilterItemClass
	{
		private string name;
		private string query;
		
		public FilterItemClass(XPathNavigator rootFilternode)
		{
			if (rootFilternode == null)
			{
				throw new ArgumentNullException("rootFilternode");
			}

			this.name  = XmlHelperClass.GetXmlStringValue(rootFilternode, "@name");
			this.query = rootFilternode.Value;
		}

		#region Properties
		public string Name
		{
			get { return this.name; }
		}

		public string Query
		{
			get { return this.query; }
		}
		#endregion

		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "[Help 2.0 Filter; {0} = {1}]", this.name, this.query);
		}
	}
}
