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
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementConsoleHostLogger : IDisposable
	{
		ICmdletLogger logger;
		IPackageManagementEvents packageEvents;
		
		public PackageManagementConsoleHostLogger(ICmdletLogger logger, IPackageManagementEvents packageEvents)
		{
			this.logger = logger;
			this.packageEvents = packageEvents;
			
			packageEvents.PackageOperationMessageLogged += PackageOperationMessageLogged;
		}
		
		void PackageOperationMessageLogged(object sender, PackageOperationMessageLoggedEventArgs e)
		{
			Log(e.Message.Level, e.Message.ToString());
		}
		
		void Log(MessageLevel level, string message)
		{
			switch (level) {
				case MessageLevel.Debug:
					logger.WriteVerbose(message);
					break;
				case MessageLevel.Warning:
					logger.WriteWarning(message);
					break;
				case MessageLevel.Error:
					logger.WriteError(CreateErrorRecord(message));
					break;
				default:
					logger.WriteLine(message);
					break;
			}
		}
		
		ErrorRecord CreateErrorRecord(string message)
		{
			return new ErrorRecord(
				new ApplicationException(message),
				"PackageManagementError",
				ErrorCategory.NotSpecified,
				null);
		}
		
		public void Dispose()
		{
			packageEvents.PackageOperationMessageLogged -= PackageOperationMessageLogged;
		}
	}
}
