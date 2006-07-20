//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.HelperClasses
{
	using System;
	using System.Reflection;
	using System.IO;
	using System.Xml;
	using System.Xml.Schema;

	public sealed class XmlValidator
	{
		private static bool validationSuccess  = true;
		private static bool beQuiet;
		private static string me =
			Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "register.xsd");

		XmlValidator()
		{
		}
		
		public static bool XsdFileDoesExist
		{
			get { return File.Exists(me); }
		}

		public static bool Validate(string xmlCommandFile, bool silentMode)
		{
			beQuiet = silentMode;

			XmlReaderSettings xsd = new XmlReaderSettings();
			xsd.Schemas.Add(ApplicationHelpers.Help2NamespaceUri, me);
			xsd.ValidationType = ValidationType.Schema;
			xsd.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);

			XmlReader reader = XmlReader.Create(xmlCommandFile, xsd);
			while(reader.Read()) { }

			return validationSuccess;
		}

		private static void ValidationCallback(object sender, ValidationEventArgs e)
		{
			if(!beQuiet) Console.WriteLine(e.Message);
			validationSuccess = false;
		}
	}
}
