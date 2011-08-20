// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.FormsDesigner
{
	public interface IFormsDesignerLoggingService
	{
		void Debug(string message);
		void Info(string message);
		void Warn(string message);
		void Error(Exception error);
		void Error(string message, Exception error);
		void DebugFormatted(string format, params object[] args);
		void WarnFormatted(string format, params object[] args);
	}
	
	public class FormsDesignerLoggingServiceProxy : MarshalByRefObject, IFormsDesignerLoggingService
	{
		IFormsDesignerLoggingService service;
		
		public FormsDesignerLoggingServiceProxy(IFormsDesignerLoggingService service)
		{
			this.service = service;
		}
		
		public void Debug(string message)
		{
			service.Debug(message);
		}
		
		public void Info(string message)
		{
			service.Info(message);
		}
		
		public void Warn(string message)
		{
			service.Warn(message);
		}
		
		public void Error(Exception error)
		{
			service.Error(error);
		}
		
		public void Error(string message, Exception error)
		{
			service.Error(message, error);
		}
		
		public void DebugFormatted(string format, params object[] args)
		{
			service.Debug(string.Format(format, args));
		}
		
		public void WarnFormatted(string format, params object[] args)
		{
			service.Warn(string.Format(format, args));
		}
	}
	
	public interface IDesignerLoaderProvider
	{
		IDesignerLoader CreateLoader(IDesignerGenerator generator);
	}
	
}
