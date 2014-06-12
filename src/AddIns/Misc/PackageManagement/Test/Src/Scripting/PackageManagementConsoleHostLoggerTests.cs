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
using System.Management.Automation;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageManagementConsoleHostLoggerTests
	{
		FakeCmdletLogger cmdletLogger;
		PackageManagementConsoleHostLogger consoleHostLogger;
		PackageManagementEvents packageEvents;
		
		void CreateConsoleHostLogger()
		{
			cmdletLogger = new FakeCmdletLogger();
			packageEvents = new PackageManagementEvents();
			consoleHostLogger = new PackageManagementConsoleHostLogger(cmdletLogger, packageEvents);
		}
		
		[Test]
		public void OnPackageOperationMessageLogged_DebugMessage_WrittenAsVerboseMessageToCmdlet()
		{
			CreateConsoleHostLogger();
			
			packageEvents.OnPackageOperationMessageLogged(MessageLevel.Debug, "test");
			
			Assert.AreEqual("test", cmdletLogger.VerboseMessageLogged);
		}
		
		[Test]
		public void OnPackageOperationMessageLogged_WarningMessage_WrittenAsWarningMessageToCmdlet()
		{
			CreateConsoleHostLogger();
			
			packageEvents.OnPackageOperationMessageLogged(MessageLevel.Warning, "test");
			
			Assert.AreEqual("test", cmdletLogger.WarningMessageLogged);
		}
		
		[Test]
		public void OnPackageOperationMessageLogged_InfoMessage_WrittenAsLineToCmdlet()
		{
			CreateConsoleHostLogger();
			
			packageEvents.OnPackageOperationMessageLogged(MessageLevel.Info, "test");
			
			Assert.AreEqual("test", cmdletLogger.LineLogged);
		}
		
		[Test]
		public void OnPackageOperationMessageLogged_ErrorMessage_WrittenAsErrorRecordCmdlet()
		{
			CreateConsoleHostLogger();
			
			packageEvents.OnPackageOperationMessageLogged(MessageLevel.Error, "test");
			
			Assert.IsNotNull(cmdletLogger.ErrorRecordLogged);
			Assert.AreEqual("test", cmdletLogger.ErrorRecordLogged.Exception.Message);
			Assert.AreEqual(ErrorCategory.NotSpecified, cmdletLogger.ErrorRecordLogged.CategoryInfo.Category);
		}
	}
}
