// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	public class PackageMessageLoggedEventArgs : EventArgs
	{
		public PackageMessageLoggedEventArgs(MessageLevel level, string message, params object[] args)
		{
			this.Message = new PackageOperationMessage(level, message, args);
		}
		
		public PackageOperationMessage Message
		{
			get;
			private set;
		}
	}
	
	public class PackageOperationMessage
	{
		string message;
		object[] args;
		
		public PackageOperationMessage(MessageLevel level, string message, params object[] args)
		{
			this.Level = level;
			this.message = message;
			this.args = args;
		}
		
		public MessageLevel Level
		{
			get; private set;
		}
		
		public override string ToString()
		{
			return String.Format(message, args);
		}
	}
}
