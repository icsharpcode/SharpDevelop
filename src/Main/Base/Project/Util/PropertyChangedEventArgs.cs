// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
