// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.FormsDesigner
{
	public class FormsDesignerLoggingService
	{
		public static IFormsDesignerLoggingService Service;
		
		public static void Debug(string message)
		{
			Service.Debug(message);
		}
		
		public static void Info(string message)
		{
			Service.Info(message);
		}
		
		public static void Warn(string message)
		{
			Service.Warn(message);
		}
		
		public static void Error(Exception error)
		{
			Service.Error(error);
		}
		
		public static void Error(string message, Exception error)
		{
			Service.Error(message, error);
		}
		
		public static void DebugFormatted(string format, params object[] args)
		{
			Service.DebugFormatted(format, args);
		}
		
		public static void WarnFormatted(string format, params object[] args)
		{
			Service.WarnFormatted(format, args);
		}
	}
}
