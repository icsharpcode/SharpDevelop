// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Util;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class NoSuchExecutableTestFixture
	{
		ProcessRunner runner;
		
		[SetUp]
		public void Init()
		{
			runner = new ProcessRunner();
		}
		
		[Test]
		[ExpectedException(typeof(Win32Exception))]
		//  "The system cannot find the file specified")] - Message depends on system language.
		public void Run()
		{
			runner.Start("foo.exe");
		}
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		//                   "Cannot start process because a file name has not been provided.")]
		//					 - Message depends on system language.
		public void RunBlankProcessFilename()
		{
			runner.Start("");
		}
	}
}
