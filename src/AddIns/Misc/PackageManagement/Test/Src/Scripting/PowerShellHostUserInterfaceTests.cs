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
using System.Collections.ObjectModel;
using System.Management.Automation.Host;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PowerShellHostUserInterfaceTests
	{
		PowerShellHostUserInterface hostUI;
		FakeScriptingConsole scriptingConsole;
		
		void CreateHostUserInterface()
		{
			scriptingConsole = new FakeScriptingConsole();
			hostUI = new PowerShellHostUserInterface(scriptingConsole);
		}
		
		[Test]
		public void WriteWarningLine_ShowWarningMessage_MessageWrittenToConsole()
		{
			CreateHostUserInterface();
			hostUI.WriteWarningLine("Test");
			
			Assert.AreEqual("Test", scriptingConsole.TextPassedToWriteLine);
		}
		
		[Test]
		public void WriteWarningLine_ShowWarningMessage_ScriptingStyleIsWarning()
		{
			CreateHostUserInterface();
			hostUI.WriteWarningLine("Test");
			
			Assert.AreEqual(ScriptingStyle.Warning, scriptingConsole.ScriptingStylePassedToWriteLine);
		}
		
		[Test]
		public void WriteVerboseLine_ShowVerboseMessage_MessageWrittenToConsole()
		{
			CreateHostUserInterface();
			hostUI.WriteVerboseLine("Test");
			
			Assert.AreEqual("Test", scriptingConsole.TextPassedToWriteLine);
		}
		
		[Test]
		public void WriteVerboseLine_ShowVerboseMessage_ScriptingStyleIsOut()
		{
			CreateHostUserInterface();
			hostUI.WriteVerboseLine("Test");
			
			Assert.AreEqual(ScriptingStyle.Out, scriptingConsole.ScriptingStylePassedToWriteLine);
		}
		
		[Test]
		public void WriteLine_ShowMessage_MessageWrittenToConsole()
		{
			CreateHostUserInterface();
			hostUI.WriteLine("Test");
			
			Assert.AreEqual("Test", scriptingConsole.TextPassedToWriteLine);
		}
		
		[Test]
		public void WriteLine_ShowMessage_ScriptingStyleIsOut()
		{
			CreateHostUserInterface();
			hostUI.WriteLine("Test");
			
			Assert.AreEqual(ScriptingStyle.Out, scriptingConsole.ScriptingStylePassedToWriteLine);
		}
		
		[Test]
		public void WriteErrorLine_ShowErrorMessage_MessageWrittenToConsole()
		{
			CreateHostUserInterface();
			hostUI.WriteErrorLine("Test");
			
			Assert.AreEqual("Test", scriptingConsole.TextPassedToWriteLine);
		}
		
		[Test]
		public void WriteErrorLine_ShowErrorMessage_ScriptingStyleIsError()
		{
			CreateHostUserInterface();
			hostUI.WriteErrorLine("Test");
			
			Assert.AreEqual(ScriptingStyle.Error, scriptingConsole.ScriptingStylePassedToWriteLine);
		}
		
		[Test]
		public void WriteDebugLine_ShowMessage_MessageWrittenToConsole()
		{
			CreateHostUserInterface();
			hostUI.WriteDebugLine("Test");
			
			Assert.AreEqual("Test", scriptingConsole.TextPassedToWriteLine);
		}
		
		[Test]
		public void WriteDebugLine_ShowMessage_ScriptingStyleIsOut()
		{
			CreateHostUserInterface();
			hostUI.WriteDebugLine("Test");
			
			Assert.AreEqual(ScriptingStyle.Out, scriptingConsole.ScriptingStylePassedToWriteLine);
		}
		
		[Test]
		public void Write_ShowMessage_MessageWrittenToConsole()
		{
			CreateHostUserInterface();
			hostUI.Write("Test");
			
			Assert.AreEqual("Test", scriptingConsole.TextPassedToWrite);
		}
		
		[Test]
		public void Write_ShowMessage_ScriptingStyleIsOut()
		{
			CreateHostUserInterface();
			hostUI.Write("Test");
			
			Assert.AreEqual(ScriptingStyle.Out, scriptingConsole.ScriptingStylePassedToWrite);
		}
		
		[Test]
		public void Write_ConsoleColorsSpecified_MessageWrittenToConsole()
		{
			CreateHostUserInterface();
			hostUI.Write(ConsoleColor.Black, ConsoleColor.Red, "Test");
			
			Assert.AreEqual("Test", scriptingConsole.TextPassedToWrite);
		}
		
		[Test]
		public void Write_ConsoleColorsSpecified_ScriptingStyleIsOut()
		{
			CreateHostUserInterface();
			hostUI.Write(ConsoleColor.Black, ConsoleColor.Red, "Test");
			
			Assert.AreEqual(ScriptingStyle.Out, scriptingConsole.ScriptingStylePassedToWrite);
		}
		
		[Test]
		public void PromptForChoice_NoChoices_ReturnsMinusOne()
		{
			CreateHostUserInterface();
			var choices = new Collection<ChoiceDescription>();
			int result = hostUI.PromptForChoice("caption", "message", choices, 2);
			
			Assert.AreEqual(-1, result);
		}
		
		[Test]
		public void RawUIBufferSize_ScriptingConsoleHas90MaximumVisibleColumns_ReturnsSizeWithWidthOf90()
		{
			CreateHostUserInterface();
			scriptingConsole.MaximumVisibleColumns = 90;
			
			Size actualSize = hostUI.RawUI.BufferSize;
			
			Size expectedSize = new Size() {
				Width = 90,
				Height = 0
			};
			
			Assert.AreEqual(expectedSize, actualSize);
		}
		
		[Test]
		public void RawUIBufferSize_ScriptingConsoleHas79MaxVisibleColumns_ReturnsSizeWithMinimumWidthOf80()
		{
			CreateHostUserInterface();
			scriptingConsole.MaximumVisibleColumns = 79;
			
			Size actualSize = hostUI.RawUI.BufferSize;
			
			Size expectedSize = new Size() {
				Width = 80,
				Height = 0
			};
			
			Assert.AreEqual(expectedSize, actualSize);
		}
		
		[Test]
		public void RawUIForegroundColor_SetForegroundColor_DoesNotThrowException()
		{
			CreateHostUserInterface();
			Assert.DoesNotThrow(() => hostUI.RawUI.ForegroundColor = ConsoleColor.Black);
		}
		
		[Test]
		public void RawUIForegroundColor_GetForegroundColor_ReturnsNoConsoleColor()
		{
			CreateHostUserInterface();
			ConsoleColor color = hostUI.RawUI.ForegroundColor;
			ConsoleColor expectedColor = PowerShellHostRawUserInterface.NoConsoleColor;
			
			Assert.AreEqual(expectedColor, color);
		}
		
		[Test]
		public void RawUIForegroundColor_SetBackgroundColor_DoesNotThrowException()
		{
			CreateHostUserInterface();
			Assert.DoesNotThrow(() => hostUI.RawUI.BackgroundColor = ConsoleColor.Black);
		}
		
		[Test]
		public void RawUIForegroundColor_GetBackgroundColor_ReturnsNoColor()
		{
			CreateHostUserInterface();
			ConsoleColor color = hostUI.RawUI.BackgroundColor;
			ConsoleColor expectedColor = PowerShellHostRawUserInterface.NoConsoleColor;
			
			Assert.AreEqual(expectedColor, color);
		}
	}
}
