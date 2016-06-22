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
using System.Collections;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectPropertyChangedEventArgs : EventArgs
	{
		string propertyName;
		string configuration, platform;
		string oldValue, newValue;
		PropertyStorageLocations newLocation;
		PropertyStorageLocations oldLocation;
		
		public ProjectPropertyChangedEventArgs(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException("propertyName");
			this.propertyName = propertyName;
		}
		
		/// <summary>
		/// The name of the property that has changed.
		/// </summary>
		public string PropertyName {
			get { return propertyName; }
		}
		
		/// <summary>
		/// The configuration where the changed property was saved to.
		/// </summary>
		public string Configuration {
			get { return configuration; }
			set { configuration = value; }
		}
		
		/// <summary>
		/// The platform where the changed property was saved to.
		/// </summary>
		public string Platform {
			get { return platform; }
			set { platform = value; }
		}
		
		/// <summary>
		/// Gets the (unevaluated) old value before the property was changed. This value might not
		/// be available if the property changed location.
		/// </summary>
		public string OldValue {
			get { return oldValue; }
			set { oldValue = value; }
		}
		
		/// <summary>
		/// The new (escaped) value of the property.
		/// </summary>
		public string NewValue {
			get { return newValue; }
			set { newValue = value; }
		}
		
		/// <summary>
		/// The location where the changed property was saved to.
		/// </summary>
		public PropertyStorageLocations NewLocation {
			get { return newLocation; }
			set { newLocation = value; }
		}
		
		/// <summary>
		/// The location where the property was previously saved to.
		/// </summary>
		public PropertyStorageLocations OldLocation {
			get { return oldLocation; }
			set { oldLocation = value; }
		}
	}
}
