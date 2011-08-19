// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Core.Services;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Description of FormsDesignerLoggingServiceImpl.
	/// </summary>
	public class FormsDesignerLoggingServiceImpl : MarshalByRefObject, IFormsDesignerLoggingService
	{
		public void Debug(string message)
		{
			LoggingService.Debug(message);
		}
		
		public void Info(string message)
		{
			LoggingService.Info(message);
		}
		
		public void Warn(string message)
		{
			LoggingService.Warn(message);
		}
		
		public void Error(Exception error)
		{
			LoggingService.Error(error);
		}
		
		public void Error(string message, Exception error)
		{
			LoggingService.Error(message, error);
		}
		
		public void DebugFormatted(string format, params object[] args)
		{
			LoggingService.DebugFormatted(format, args);
		}
		
		public void WarnFormatted(string format, params object[] args)
		{
			LoggingService.WarnFormatted(format, args);
		}
	}
}
