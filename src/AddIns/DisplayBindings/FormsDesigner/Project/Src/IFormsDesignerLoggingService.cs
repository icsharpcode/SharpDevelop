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
	
	public interface IDesignerLoaderProvider
	{
		IDesignerLoader CreateLoader(IDesignerGenerator generator);
	}
	
}
