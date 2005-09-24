// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
		
		PropertyStorageLocations location = PropertyStorageLocations.Unknown;
		ChooseStorageLocationButton storageLocationButton;
		
		public PropertyStorageLocations Location {
			get {
				return location;
			}
			set {
				if (location != value) {
					location = value;
					if (storageLocationButton != null) {
						storageLocationButton.StorageLocation = value;
					}
					helper.IsDirty = true;
				}
			}
		}
		
		public ChooseStorageLocationButton CreateLocationButton()
		{
			ChooseStorageLocationButton btn = new ChooseStorageLocationButton();
			this.storageLocationButton = btn;
			if (location == PropertyStorageLocations.Unknown) {
				btn.StorageLocation = defaultLocation;
			} else {
				btn.StorageLocation = location;
			}
			btn.StorageLocationChanged += delegate(object sender, EventArgs e) {
				this.Location = ((ChooseStorageLocationButton)sender).StorageLocation;
			};
			return btn;
		}
		
		/// <summary>
		/// Moves the control '<paramref name="controlName"/>' a bit to the right and inserts a
		/// <see cref="ChooseStorageLocationButton"/>.
		/// </summary>
		public void CreateLocationButton(string controlName)
		{
			CreateLocationButton(Helper.ControlDictionary[controlName]);
		}
		
		/// <summary>
		/// Moves the <paramref name="replacedControl"/> a bit to the right and inserts a
		/// <see cref="ChooseStorageLocationButton"/>.
		/// </summary>
		public void CreateLocationButton(Control replacedControl)
		{
			ChooseStorageLocationButton btn = CreateLocationButton();
			btn.Location = new Point(replacedControl.Left, replacedControl.Top + (replacedControl.Height - btn.Height) / 2);
			replacedControl.Left  += btn.Width + 4;
			replacedControl.Width -= btn.Width + 4;
			replacedControl.Parent.Controls.Add(btn);
			replacedControl.Parent.Controls.SetChildIndex(btn, replacedControl.Parent.Controls.IndexOf(replacedControl));
		}
		
		public T Get<T>(T defaultValue)
		{
			return helper.GetProperty(property, defaultValue, out location);
		}
		
		public void Set<T>(T value)
		{
			if ((location & PropertyStorageLocations.UserFile) != 0) {
				System.Diagnostics.Debugger.Break();
			}
			helper.SetProperty(property, value, location);
		}
		
		public abstract void Load();
		public abstract bool Save();
	}
}
