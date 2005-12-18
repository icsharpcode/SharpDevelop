// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using NUnit.Framework;
using ICSharpCode.NAntAddIn;

namespace ICSharpCode.NAntAddIn.Tests
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
		[ExpectedException(typeof(Win32Exception), 
		                   "The system cannot find the file specified")]
		public void Run()
		{
			runner.Start("foo.exe");
		}
		
		[Test]
		[ExpectedException(typeof(InvalidOperationException), 
		                   "Cannot start process because a file name has not been provided.")]
		public void RunBlankProcessFilename()
		{
			runner.Start("");
		}
	}
}
