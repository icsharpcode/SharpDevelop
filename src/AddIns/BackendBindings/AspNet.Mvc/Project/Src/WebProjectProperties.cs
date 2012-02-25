// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ICSharpCode.AspNet.Mvc
{
	public class WebProjectProperties
	{
		public WebProjectProperties()
		{
			this.CustomServerUrl = String.Empty;
			this.DevelopmentServerVPath = String.Empty;
			this.IISUrl = String.Empty;
		}
		
		public WebProjectProperties(XElement element)
		{
			Load(element);
		}
		
		void Load(XElement element)
		{
			AutoAssignPort = GetChildElementBoolean("AutoAssignPort", element);
			CustomServerUrl = GetChildElementString("CustomServerUrl", element);
			DevelopmentServerPort = GetChildElementInteger("DevelopmentServerPort", element);
			DevelopmentServerVPath = GetChildElementString("DevelopmentServerVPath", element);
			IISUrl = GetChildElementString("IISUrl", element);
			NTLMAuthentication = GetChildElementBoolean("NTLMAuthentication", element);
			SaveServerSettingsInUserFile = GetChildElementBoolean("SaveServerSettingsInUserFile", element);
			UseCustomServer = GetChildElementBoolean("UseCustomServer", element);
			UseIIS = GetChildElementBoolean("UseIIS", element);
		}
		
		string GetChildElementString(string name, XElement element)
		{
			XElement childElement = element.Element(name);
			if (childElement != null) {
				return childElement.Value;
			}
			return String.Empty;
		}
		
		int GetChildElementInteger(string name, XElement element)
		{
			string value = GetChildElementString(name, element);
			int result = 0;
			Int32.TryParse(value, out result);
			return result;
		}
		
		bool GetChildElementBoolean(string name, XElement element)
		{
			string value = GetChildElementString(name, element);
			bool result = false;
			Boolean.TryParse(value, out result);
			return result;
		}
		
		public override bool Equals(object obj)
		{
			var properties = obj as WebProjectProperties;
			if (properties != null) {
				return Equals(properties);
			}
			return false;
		}
		
		bool Equals(WebProjectProperties properties)
		{
			return
				(AutoAssignPort == properties.AutoAssignPort) &&
				(UseIIS == properties.UseIIS) &&
				(DevelopmentServerVPath == properties.DevelopmentServerVPath) &&
				(DevelopmentServerPort == properties.DevelopmentServerPort) &&
				(CustomServerUrl == properties.CustomServerUrl) &&
				(UseCustomServer == properties.UseCustomServer) &&
				(SaveServerSettingsInUserFile == properties.SaveServerSettingsInUserFile) &&
				(UseIISExpress == properties.UseIISExpress) &&
				(NTLMAuthentication == properties.NTLMAuthentication) &&
				(IISUrl == properties.IISUrl);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return String.Format(
				"[WebProjectProperties IISUrl=\"{0}\", UseIISExpress={1}, UseIIS={2}, SaveServerSettingsInUserFile={3}]",
				IISUrl,
				UseIISExpress,
				UseIIS,
				SaveServerSettingsInUserFile);
		}

		public bool AutoAssignPort { get; set; }
		public string DevelopmentServerVPath { get; set; }
		public int DevelopmentServerPort { get; set; }

		public bool NTLMAuthentication { get; set; }

		public bool UseIIS { get; set; }
		public string IISUrl { get; set; }
		public bool UseIISExpress { get; set; }

		public bool SaveServerSettingsInUserFile { get; set; }

		public bool UseCustomServer { get; set; }
		public string CustomServerUrl { get; set; }
		
		public XElement ToXElement()
		{
 			return new XElement(
 				"WebProjectProperties",
 				CreateBooleanXElement("UseIIS", UseIIS),
 				CreateBooleanXElement("AutoAssignPort", AutoAssignPort),
 				new XElement("DevelopmentServerPort", DevelopmentServerPort),
 				new XElement("DevelopmentServerVPath", DevelopmentServerVPath),
 				new XElement("IISUrl", IISUrl),
 				CreateBooleanXElement("NTLMAuthentication", NTLMAuthentication),
 				CreateBooleanXElement("UseCustomServer", UseCustomServer),
 				new XElement("CustomServerUrl", CustomServerUrl),
 				CreateBooleanXElement("SaveServerSettingsInUserFile", SaveServerSettingsInUserFile));	
		}
		
		XElement CreateBooleanXElement(string name, bool value)
		{
			return new XElement(name, value.ToString());
		}
		
		public bool IsConfigured()
		{
			return 
				(UseIIS || UseIISExpress) && IsValidIISUrl();
		}
		
		bool IsValidIISUrl()
		{
			return !String.IsNullOrEmpty(IISUrl);
		}
	}
}
