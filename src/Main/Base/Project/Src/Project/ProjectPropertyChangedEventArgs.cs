// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
