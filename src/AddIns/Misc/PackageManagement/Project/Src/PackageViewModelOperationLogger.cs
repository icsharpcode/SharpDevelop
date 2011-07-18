// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageViewModelOperationLogger : ILogger
	{
		ILogger logger;
		IPackage package;
		
		public PackageViewModelOperationLogger(ILogger logger, IPackage package)
		{
			this.logger = logger;
			this.package = package;
			
			GetMessageFormats();
		}

		void GetMessageFormats()
		{
			AddingPackageMessageFormat = "Installing...{0}";
			RemovingPackageMessageFormat = "Uninstalling...{0}";
			ManagingPackageMessageFormat = "Managing...{0}";
		}
		
		public string AddingPackageMessageFormat { get; set; }
		public string RemovingPackageMessageFormat { get; set; }
		public string ManagingPackageMessageFormat { get; set; }
		
		public void Log(MessageLevel level, string message, params object[] args)
		{
			logger.Log(level, message, args);
		}
		
		public void LogInformation(string message)
		{
			Log(MessageLevel.Info, message);
		}
		
		public void LogAfterPackageOperationCompletes()
		{
			LogEndMarkerLine();
			LogEmptyLine();
		}
		
		void LogEndMarkerLine()
		{
			string message = new String('=', 30);
			LogInformation(message);
		}

		void LogEmptyLine()
		{
			LogInformation(String.Empty);
		}
		
		public void LogAddingPackage()
		{
			string message = GetFormattedStartPackageOperationMessage(AddingPackageMessageFormat);
			LogInformation(message);			
		}
		
		string GetFormattedStartPackageOperationMessage(string format)
		{
			string message = String.Format(format, package.ToString());
			return GetStartPackageOperationMessage(message);
		}
		
		string GetStartPackageOperationMessage(string message)
		{
			return String.Format("------- {0} -------", message);
		}
		
		public void LogRemovingPackage()
		{
			string message =  GetFormattedStartPackageOperationMessage(RemovingPackageMessageFormat);
			LogInformation(message);
		}
		
		public void LogError(Exception ex)
		{
			LogInformation(ex.ToString());
		}
		
		public void LogManagingPackage()
		{
			string message =  GetFormattedStartPackageOperationMessage(ManagingPackageMessageFormat);
			LogInformation(message);			
		}
	}
}
