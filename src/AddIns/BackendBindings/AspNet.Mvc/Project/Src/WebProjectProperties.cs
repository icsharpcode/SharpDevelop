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
