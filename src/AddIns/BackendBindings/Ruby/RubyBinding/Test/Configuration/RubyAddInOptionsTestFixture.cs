// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Configuration
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
		
		[Test]
		public void RubyLibraryPathTakenFromProperties()
		{
			Properties p = new Properties();
			RubyAddInOptions options = new RubyAddInOptions(p);
			string expectedRubyLibraryPath = @"c:\ruby\lib;c:\ruby\lib\lib-tk";
			p["RubyLibraryPath"] = expectedRubyLibraryPath;
			Assert.AreEqual(expectedRubyLibraryPath, options.RubyLibraryPath);
		}
		
		[Test]
		public void HasRubyLibraryPathReturnsTrueForNonEmptyPath()
		{
			Properties p = new Properties();
			RubyAddInOptions options = new RubyAddInOptions(p);
			options.RubyLibraryPath = @"c:\ruby\lib";
			Assert.IsTrue(options.HasRubyLibraryPath);
		}
		
		[Test]
		public void HasRubyLibraryPathReturnsFalseForEmptyPath()
		{
			Properties p = new Properties();
			RubyAddInOptions options = new RubyAddInOptions(p);
			options.RubyLibraryPath = String.Empty;
			Assert.IsFalse(options.HasRubyLibraryPath);
		}
		
		[Test]
		public void RubyLibraryPathTrimsPath()
		{
			Properties p = new Properties();
			RubyAddInOptions options = new RubyAddInOptions(p);
			options.RubyLibraryPath = "    ";
			Assert.AreEqual(String.Empty, options.RubyLibraryPath);
		}		
	}
}
