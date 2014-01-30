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
