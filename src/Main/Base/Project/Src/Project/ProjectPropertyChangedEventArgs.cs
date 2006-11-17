// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectPropertyChangedEventArgs : EventArgs
	{
		string propertyName;
		string configuration, platform;
		string oldValue;
		PropertyStorageLocations location;
		
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
		/// Gets the old value before the property was changed. This value might not
		/// be available if the property changed location.
		/// </summary>
		public string OldValue {
			get { return oldValue; }
			set { oldValue = value; }
		}
		
		/// <summary>
		/// The location where the changed property was saved to.
		/// </summary>
		public PropertyStorageLocations Location {
			get { return location; }
			set { location = value; }
		}
	}
}
