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

		public override string ToString()
		{
			return string.Format(@"{0}/{1}/{2}", productCode, productVersion, productLocale);
		}
	}
}