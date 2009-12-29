// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests
{
	/// <summary>
	/// Tests the AddInOptions class.
	/// </summary>
	[TestFixture]
	public class RubyAddInOptionsTestFixture
	{
		[Test]
		public void DefaultRubyConsoleFileName()
		{
			Properties p = new Properties();
			DerivedRubyAddInOptions options = new DerivedRubyAddInOptions(p);
			options.AddInPath = @"C:\Projects\SD\AddIns\Ruby";

			string expectedFileName = Path.Combine(options.AddInPath, "ir.exe");
			Assert.AreEqual(expectedFileName, options.RubyFileName);
			Assert.AreEqual("${addinpath:ICSharpCode.RubyBinding}", options.AddInPathRequested);
		}
				
		[Test]
		public void SetRubyConsoleFileNameToNull()
		{
			Properties p = new Properties();
			DerivedRubyAddInOptions options = new DerivedRubyAddInOptions(p);
			options.AddInPath = @"C:\Projects\SD\AddIns\Ruby";
			options.RubyFileName = null;

			string expectedFileName = Path.Combine(options.AddInPath, "ir.exe");
			Assert.AreEqual(expectedFileName, options.RubyFileName);
		}
		
		[Test]
		public void SetRubyConsoleFileNameToEmptyString()
		{
			Properties p = new Properties();
			DerivedRubyAddInOptions options = new DerivedRubyAddInOptions(p);
			options.AddInPath = @"C:\Projects\SD\AddIns\Ruby";
			options.RubyFileName = String.Empty;
			
			string expectedFileName = Path.Combine(options.AddInPath, "ir.exe");
			Assert.AreEqual(expectedFileName, options.RubyFileName);
		}
		
		[Test]
		public void SetRubyConsoleFileName()
		{
			Properties p = new Properties();
			RubyAddInOptions options = new RubyAddInOptions(p);
			string fileName = @"C:\IronRuby\ir.exe";
			options.RubyFileName = fileName;
			
			Assert.AreEqual(fileName, options.RubyFileName);
			Assert.AreEqual(fileName, p["RubyFileName"]);
		}
	}
}
