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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// PropertyChangedEventHandler with the old and new value.
	/// </summary>
	public delegate void PropertyChangedEventHandler<T>(object sender, PropertyChangedEventArgs<T> e);
	
	/// <summary>
	/// PropertyChangedEventArgs that contains the old and new value.
	/// </summary>
	public class PropertyChangedEventArgs<T> : EventArgs
	{
		readonly T oldValue;
		readonly T newValue;
		
		public PropertyChangedEventArgs(T oldValue, T newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		
		public T OldValue {
			get { return oldValue; }
		}
		
		public T NewValue {
			get { return newValue; }
		}

	}
}
