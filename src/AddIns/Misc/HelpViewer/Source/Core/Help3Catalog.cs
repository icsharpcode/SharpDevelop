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

namespace MSHelpSystem.Core
{
	public class Help3Catalog
	{
		public Help3Catalog(string code, string version, string locale, string name, string catPath, string contPath, string brandPackage)
		{
			productCode = code;
			productVersion = version;
			productLocale = locale;
			displayName = (string.IsNullOrEmpty(name)) ? code:name;
			catalogPath = catPath;
			contentPath = contPath;
			brandingPackage = brandPackage;
		}

		string productCode;
		string productVersion;
		string productLocale;
		string displayName;
		string catalogPath;
		string contentPath;
		string brandingPackage;

		public string ProductCode
		{
			get { return productCode; }
		}
	
		public string ProductVersion
		{
			get { return productVersion; }
		}
	
		public string Locale
		{
			get { return productLocale.ToUpper(); }
		}
	
		public string DisplayName
		{
			get { return displayName; }
		}

		public string ShortName
		{
			get { return string.Format(@"{0}/{1}/{2}", productCode, productVersion, productLocale); }
		}
	
		public string CatalogPath
		{
			get { return catalogPath; }
		}
	
		public string ContentPath
		{
			get { return contentPath; }
		}
	
		public string BrandingPackage
		{
			get { return brandingPackage; }
		}

		public string AsMsXHelpParam
		{
			get { return string.Format("product={0}&productVersion={1}&locale={2}", productCode, productVersion, productLocale); }
		}

		public string AsCmdLineParam
		{
			get { return string.Format("/product {0} /version {1} /locale {2}", productCode, productVersion, productLocale); }
		}
	}
}
