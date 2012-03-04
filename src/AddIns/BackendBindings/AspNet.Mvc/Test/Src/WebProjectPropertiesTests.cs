// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Linq;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class WebProjectPropertiesTests
	{
		WebProjectProperties lhs;
		WebProjectProperties rhs;
		
		void CreateTwoWebProjectProperties()
		{
			lhs = new WebProjectProperties();
			rhs = new WebProjectProperties();
		}
		
		[Test]
		public void Equals_AllPropertiesAreTheSame_ReturnsTrue()
		{
			CreateTwoWebProjectProperties();
			
			Assert.AreEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_UseIISPropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.UseIIS = false;
			rhs.UseIIS = true;
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_Null_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			bool result = lhs.Equals(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_AutoAssignPortPropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.AutoAssignPort = true;
			rhs.AutoAssignPort = false;
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_DevelopmentServerVPathPropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.DevelopmentServerVPath = "/";
			rhs.DevelopmentServerVPath = "/Test";
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_DevelopmentServerPortPropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.DevelopmentServerPort = 8080;
			rhs.DevelopmentServerPort = 7777;
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_NTLMAuthenticationPropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.NTLMAuthentication = false;
			rhs.NTLMAuthentication = true;
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_IISUrlPropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.IISUrl = "http://localhost/";
			rhs.IISUrl = "http://localhost:1888/";
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_UseIISExpressPropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.UseIISExpress = false;
			rhs.UseIISExpress = true;
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_SaveServerSettingsInUserFilePropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.SaveServerSettingsInUserFile = false;
			rhs.SaveServerSettingsInUserFile = true;
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_CustomServerUrlPropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.CustomServerUrl = "/a";
			rhs.CustomServerUrl = "/test";
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void Equals_UseCustomServerPropertyIsDifferent_ReturnsFalse()
		{
			CreateTwoWebProjectProperties();
			lhs.UseCustomServer = false;
			rhs.UseCustomServer = true;
			
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void ToString_NewInstance_ReturnsMainPropertyValues()
		{
			CreateTwoWebProjectProperties();
			lhs.UseIISExpress = false;
			lhs.UseIIS = true;
			lhs.IISUrl = "http://localhost";
			lhs.SaveServerSettingsInUserFile = true;
			
			string text = lhs.ToString();
			
			string expectedText =
				"[WebProjectProperties IISUrl=\"http://localhost\", UseIISExpress=False, UseIIS=True, SaveServerSettingsInUserFile=True]";
			
			Assert.AreEqual(expectedText, text);
		}
		
		[Test]
		public void Constructor_WebPropertyPropertiesXElementHasNoChildElements_ExceptionNotThrown()
		{
			var element = new XElement("WebPropertyProperties");
			var properties = new WebProjectProperties(element);
			
			Assert.AreEqual(String.Empty, properties.IISUrl);
			Assert.AreEqual(String.Empty, properties.CustomServerUrl);
		}
		
		[Test]
		public void Constructor_XElementHasCustomServerUrl_CustomServerUrlPropertyPopulated()
		{
			var properties = new WebProjectProperties() { CustomServerUrl = "/Test" };
			properties = new WebProjectProperties(properties.ToXElement());
			
			Assert.AreEqual("/Test", properties.CustomServerUrl);
		}
		
		[Test]
		public void Constructor_XElementHasAutoAssignPort_AutoAssignPortPropertyPopulated()
		{
			var properties = new WebProjectProperties() { AutoAssignPort = true };
			properties = new WebProjectProperties(properties.ToXElement());
			
			Assert.IsTrue(properties.AutoAssignPort);
		}
		
		[Test]
		public void Constructor_XElementHasDevelopmentServerVPath_DevelopmentServerVPathPropertyPopulated()
		{
			var properties = new WebProjectProperties() { DevelopmentServerVPath = "Test" };
			properties = new WebProjectProperties(properties.ToXElement());
			
			Assert.AreEqual("Test", properties.DevelopmentServerVPath);
		}
		
		[Test]
		public void Constructor_XElementHasNTLMAuthentication_NTLMAuthenticationPropertyPopulated()
		{
			var properties = new WebProjectProperties() { NTLMAuthentication = true };
			properties = new WebProjectProperties(properties.ToXElement());
			
			Assert.IsTrue(properties.NTLMAuthentication);
		}
		
		[Test]
		public void Constructor_XElementHasSaveServerSettingsInUserFile_SaveServerSettingsInUserFilePropertyPopulated()
		{
			var properties = new WebProjectProperties() { SaveServerSettingsInUserFile = true };
			properties = new WebProjectProperties(properties.ToXElement());
			
			Assert.IsTrue(properties.SaveServerSettingsInUserFile);
		}
		
		[Test]
		public void Constructor_XElementHasUseIIS_UseIISPropertyPopulated()
		{
			var properties = new WebProjectProperties() { UseIIS = true };
			properties = new WebProjectProperties(properties.ToXElement());
			
			Assert.IsTrue(properties.UseIIS);
		}
		
		[Test]
		public void Constructor_XElementHasUseCustomServer_UseCustomServerPropertyPopulated()
		{
			var properties = new WebProjectProperties() { UseCustomServer = true };
			properties = new WebProjectProperties(properties.ToXElement());
			
			Assert.IsTrue(properties.UseCustomServer);
		}
		
		[Test]
		public void IsConfigured_UseIISAndUseIISExpressAreBothFalse_ReturnsFalse()
		{
			var properties = new WebProjectProperties() { UseIISExpress = false, UseIIS = false };
			
			bool configured = properties.IsConfigured();
			
			Assert.IsFalse(configured);
		}
		
		[Test]
		public void IsConfigured_UseIISExpressIsTrueAndIISUrlIsValidUrl_ReturnsTrue()
		{
			var properties = new WebProjectProperties
			{
				UseIISExpress = true,
				IISUrl = "http://localhost:8080/"
			};
			
			bool configured = properties.IsConfigured();
			
			Assert.IsTrue(configured);
		}
		
		[Test]
		public void IsConfigured_UseIISExpressIsTrueAndIISUrlIsEmptyString_ReturnsFalse()
		{
			var properties = new WebProjectProperties
			{
				UseIISExpress = true,
				IISUrl = String.Empty
			};
			
			bool configured = properties.IsConfigured();
			
			Assert.IsFalse(configured);
		}
		
		[Test]
		public void IsConfigured_UseIISIsTrueAndIISUrlIsNull_ReturnsFalse()
		{
			var properties = new WebProjectProperties
			{
				UseIISExpress = true,
				IISUrl = null
			};
			
			bool configured = properties.IsConfigured();
			
			Assert.IsFalse(configured);
		}
	}
}
