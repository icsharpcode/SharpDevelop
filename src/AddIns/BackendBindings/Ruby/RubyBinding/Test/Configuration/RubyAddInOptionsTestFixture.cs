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
