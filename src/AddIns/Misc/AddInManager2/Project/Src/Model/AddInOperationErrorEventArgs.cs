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
