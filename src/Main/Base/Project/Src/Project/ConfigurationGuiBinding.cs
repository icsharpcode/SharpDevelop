// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public abstract class ConfigurationGuiBinding
	{
		ConfigurationGuiHelper helper;
		string property;
		
		public MSBuildProject Project {
			get {
				return helper.Project;
			}
		}
		
		public ConfigurationGuiHelper Helper {
			get {
				return helper;
			}
			internal set {
				helper = value;
			}
		}
		
		public string Property {
			get {
				return property;
			}
			internal set {
				property = value;
			}
		}
		
		PropertyStorageLocations defaultLocation = PropertyStorageLocations.Base;

		public PropertyStorageLocations DefaultLocation {
			get {
				return defaultLocation;
			}
			set {
				defaultLocation = value;
			}
		}
		
		PropertyStorageLocations location;
		
		public PropertyStorageLocations Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		}
		
		public T Get<T>(T defaultValue)
		{
			T result = helper.GetProperty(property, defaultValue, out location);
			if (location == PropertyStorageLocations.Unknown) {
				location = defaultLocation;
			}
			return result;
		}
		
		public void Set<T>(T value)
		{
			helper.SetProperty(property, value, location);
		}
		
		public abstract void Load();
		public abstract bool Save();
	}
}
