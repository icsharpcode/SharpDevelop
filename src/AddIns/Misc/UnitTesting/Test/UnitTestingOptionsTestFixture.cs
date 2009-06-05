// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests
{
	/// <summary>
	/// Tests the UnitTestingOptions class.
	/// </summary>
	[TestFixture]
	public class UnitTestingOptionsTestFixture
	{
		UnitTestingOptions defaultOptions;
		Properties p;
		
		[SetUp]
		public void Init()
		{
			p = new Properties();
			defaultOptions = new UnitTestingOptions(p);
		}
		
		[Test]
		public void DefaultNoShadow()
		{
			Assert.IsFalse(defaultOptions.NoShadow);
		}
		
		[Test]
		public void SetNoShadow()
		{
			defaultOptions.NoShadow = true;
			Assert.IsTrue(p.Get<bool>(UnitTestingOptions.NoShadowProperty, false));
		}
		
		[Test]
		public void NoShadowSetToTrueInProperties()
		{
			Properties newProperties = new Properties();
			newProperties.Set<bool>(UnitTestingOptions.NoShadowProperty, true);
			UnitTestingOptions options = new UnitTestingOptions(newProperties);
			
			Assert.IsTrue(options.NoShadow);
		}
		
		[Test]
		public void DefaultNoThread()
		{
			Assert.IsFalse(defaultOptions.NoThread);
		}		

		[Test]
		public void SetNoThread()
		{
			defaultOptions.NoThread = true;
			Assert.IsTrue(p.Get<bool>(UnitTestingOptions.NoThreadProperty, false));
		}
		
		[Test]
		public void NoThreadSetToTrueInProperties()
		{
			Properties newProperties = new Properties();
			newProperties.Set<bool>(UnitTestingOptions.NoThreadProperty, true);
			UnitTestingOptions options = new UnitTestingOptions(newProperties);
			
			Assert.IsTrue(options.NoThread);
		}
		
		[Test]
		public void DefaultNoLogo()
		{
			Assert.IsFalse(defaultOptions.NoLogo);
		}		

		[Test]
		public void SetNoLogo()
		{
			defaultOptions.NoLogo = true;
			Assert.IsTrue(p.Get<bool>(UnitTestingOptions.NoLogoProperty, false));
		}
		
		[Test]
		public void NoLogoSetToTrueInProperties()
		{
			Properties newProperties = new Properties();
			newProperties.Set<bool>(UnitTestingOptions.NoLogoProperty, true);
			UnitTestingOptions options = new UnitTestingOptions(newProperties);
			
			Assert.IsTrue(options.NoLogo);
		}
		
		[Test]
		public void DefaultNoDots()
		{
			Assert.IsFalse(defaultOptions.NoDots);
		}		

		[Test]
		public void SetNoDots()
		{
			defaultOptions.NoDots = true;
			Assert.IsTrue(p.Get<bool>(UnitTestingOptions.NoDotsProperty, false));
		}
		
		[Test]
		public void NoDotsSetToTrueInProperties()
		{
			Properties newProperties = new Properties();
			newProperties.Set<bool>(UnitTestingOptions.NoDotsProperty, true);
			UnitTestingOptions options = new UnitTestingOptions(newProperties);
			
			Assert.IsTrue(options.NoDots);
		}
		
		[Test]
		public void DefaultLabels()
		{
			Assert.IsFalse(defaultOptions.Labels);
		}				
		
		[Test]
		public void SetLabels()
		{
			defaultOptions.Labels = true;
			Assert.IsTrue(p.Get<bool>(UnitTestingOptions.LabelsProperty, false));
		}
		
		[Test]
		public void LabelsSetToTrueInProperties()
		{
			Properties newProperties = new Properties();
			newProperties.Set<bool>(UnitTestingOptions.LabelsProperty, true);
			UnitTestingOptions options = new UnitTestingOptions(newProperties);
			
			Assert.IsTrue(options.Labels);
		}
		
		[Test]
		public void SetCreateXmlOutputFileProperty()
		{
			defaultOptions.CreateXmlOutputFile = true;
			Assert.IsTrue(p.Get<bool>(UnitTestingOptions.CreateXmlOutputFileProperty, false));
		}
		
		[Test]
		public void CreateXmlOutputFileSetToTrueInProperties()
		{
			Properties newProperties = new Properties();
			newProperties.Set<bool>(UnitTestingOptions.CreateXmlOutputFileProperty, true);
			UnitTestingOptions options = new UnitTestingOptions(newProperties);
			
			Assert.IsTrue(options.CreateXmlOutputFile);
		}
	}
}
