// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// PropertyChangedEventHandler that includes the old and new value.
	/// </summary>
	public delegate void PropertyChangedEventHandler<T>(object sender, PropertyChangedEventArgs<T> e);
	
	/// <summary>
	/// PropertyChangedEventArgs that includes the old and new value.
	/// </summary>
	public class PropertyChangedEventArgs<T> : PropertyChangedEventArgs
	{
		readonly T oldValue;
		readonly T newValue;
		
		public PropertyChangedEventArgs(string propertyName, T oldValue, T newValue)
			: base(propertyName)
		{
			if (propertyName == null)
				throw new ArgumentNullException("propertyName");
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
