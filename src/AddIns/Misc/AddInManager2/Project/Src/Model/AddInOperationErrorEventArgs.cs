// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Data for events indicating an exception or other errors during package-related operations.
	/// </summary>
	public class AddInOperationErrorEventArgs : EventArgs
	{
		private string _message = null;
		
		public AddInOperationErrorEventArgs(Exception exception)
		{
			Exception = exception;
		}
		
		public AddInOperationErrorEventArgs(string message)
		{
			Exception = null;
			_message = message;
		}
		
		public Exception Exception
		{
			get;
			private set;
		}
		
		public string Message
		{
			get
			{
				if (_message != null)
				{
					return _message;
				}
				if (Exception != null)
				{
					return Exception.Message;
				}
				
				return null;
			}
		}
	}
}
