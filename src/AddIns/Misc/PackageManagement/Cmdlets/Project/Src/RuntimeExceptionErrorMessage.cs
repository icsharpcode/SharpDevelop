// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using System.Text;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	public class RuntimeExceptionErrorMessage
	{
		StringBuilder message = new StringBuilder();
		
		public RuntimeExceptionErrorMessage(RuntimeException ex)
		{
			CreateErrorMessage(ex);
		}
		
		void CreateErrorMessage(RuntimeException ex)
		{
			message.AppendLine(ex.Message);
			AppendErrorRecord(ex.ErrorRecord);
		}
		
		void AppendErrorRecord(ErrorRecord errorRecord)
		{
			if (errorRecord != null) {
				AppendInvocationInfo(errorRecord.InvocationInfo);
			}
		}
		
		void AppendInvocationInfo(InvocationInfo invocationInfo)
		{
			if (invocationInfo != null) {
				message.AppendLine(invocationInfo.PositionMessage);
				message.AppendLine("Script Line: " + invocationInfo.ScriptLineNumber);
				message.AppendLine("Script Name: " + invocationInfo.ScriptName);
			}
		}
		
		public override string ToString()
		{
			return message.ToString();
		}
	}
}
