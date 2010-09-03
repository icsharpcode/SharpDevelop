// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

using ICSharpCode.PythonBinding;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the PythonConsole's GetUnreadLines method.
	/// </summary>
	[TestFixture]
	public class PythonConsoleUnreadLinesTestFixture : PythonConsoleTestsBase
	{
		[SetUp]
		public void Init()
		{
			base.CreatePythonConsole();
		}
		
		[Test]
		public void NoUnreadLinesAtStart()
		{
			Assert.AreEqual(0, TestablePythonConsole.GetUnreadLines().Length);
		}
	
		[Test]
		public void HasUnreadLines()
		{
			Assert.IsFalse(TestablePythonConsole.IsLineAvailable);
		}
		
		[Test]
		public void AddOneLine()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(System.Windows.Input.Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(System.Windows.Input.Key.Enter);
			
			string[] lines = TestablePythonConsole.GetUnreadLines();
			string[] expectedLines = new string[] {"A"};
			
			Assert.AreEqual(expectedLines, lines);
			Assert.IsTrue(TestablePythonConsole.IsLineAvailable);
		}
	}
}
