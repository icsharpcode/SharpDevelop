// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: trecio
 * Data: 2009-07-09
 * Godzina: 11:15
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using ICSharpCode.SharpDevelop.Project;
using System.Windows.Forms;

namespace ICSharpCode.CppBinding.Project
{
	
	/// <summary>
	/// Configuration gui binding that connects a given control and a specified metadata value in Item Definition Group element.
	///</summary>
	public class ItemDefinitionGroupBinding<ControlType> : ConfigurationGuiBinding
		where ControlType : Control
	{
		public delegate string GetValueDelegate(ControlType c);
		public delegate void SetValueDelegate(ControlType c, string val);
		
		public ItemDefinitionGroupBinding(ControlType c, string elementName, string metadataName) :
			this(c, elementName, metadataName, null, null)
		{			
		}
		
		/// <summary>
		/// Creates the binding.
		/// </summary>
		/// <param name="c">control which is being bind</param>
		/// <param name="elementName">element name in the item definition group</param>
		/// <param name="metadataName">name of the element metadata which value is bind to control</param>
		/// <param name="getValue">function used to get string value of configuration attribute from control</param>
		/// <param name="setValue">function used to set controls' state from string</param>
		public ItemDefinitionGroupBinding(ControlType c, string elementName, string metadataName, 
		                                  GetValueDelegate getValue, SetValueDelegate setValue) {
			if (getValue == null)
				getValue = DefaultGetValue;
			if (setValue == null)
				setValue = DefaultSetValue;
			this.control = c;
			this.elementName = elementName;
			this.metadataName = metadataName;
			this.getControlValue = getValue;
			this.setControlValue = setValue;
			c.TextChanged += SetHelperDirty;
		}
		
		public override void Load()
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(Project,
			                                                                  Helper.Configuration, Helper.Platform);
			setControlValue(control, group.GetElementMetadata(elementName, metadataName));
		}
		
		public override bool Save()
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(Project,
			                                                                  Helper.Configuration, Helper.Platform);
			string controlValue = getControlValue(control);
			group.SetElementMetadata(elementName, metadataName, controlValue);
			return true;
		}
		
		protected void SetHelperDirty(object o, EventArgs e)
		{
			Helper.IsDirty = true;
		}
			
		ControlType control;
		string elementName;
		string metadataName;	
		GetValueDelegate getControlValue;
		SetValueDelegate setControlValue;		
		
		private string DefaultGetValue(ControlType c) { return c.Text; }
		private void DefaultSetValue(ControlType c, string val) { c.Text = val; }
	}
	
	public class CheckBoxItemDefinitionGroupBinding : ItemDefinitionGroupBinding<CheckBox> {
		public CheckBoxItemDefinitionGroupBinding(CheckBox c, string elementName, string metadataName) :
			base(c, elementName, metadataName, 
			     	delegate (CheckBox checkBox) {
			     		return checkBox.Checked ? "true" : "false";
				},
				delegate (CheckBox checkBox, string val) {
			     		bool check;
			     		if (bool.TryParse(val, out check))
			     			checkBox.Checked = check;
			    	})
		{
			c.TextChanged -= SetHelperDirty;
			c.CheckedChanged += SetHelperDirty;
		}
	}
}
