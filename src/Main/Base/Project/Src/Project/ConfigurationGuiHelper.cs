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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Specifies whether the user enters the property value or the MSBuild code for
	/// the property value.
	/// </summary>
	public enum TextBoxEditMode
	{
		/// <summary>
		/// The user edits the MSBuild property value. It is not evaluated on loading,
		/// and not escaped when saving.
		/// The user can use MSBuild properties with $() in the text box.
		/// </summary>
		EditRawProperty,
		/// <summary>
		/// The user edits the property. When loading the value, it is evaluated;
		/// when saving the value, it is escaped.
		/// The user cannot use MSBuild properties with $() because the $ will be escaped.
		/// </summary>
		EditEvaluatedProperty
	}
	
	/// <summary>
	/// Class that helps connecting configuration GUI controls to MsBuild properties.
	/// </summary>
	[Obsolete]
	public class ConfigurationGuiHelper : ICanBeDirty
	{
		MSBuildBasedProject project;
		Dictionary<string, Control> controlDictionary;
		List<ConfigurationGuiBinding> bindings = new List<ConfigurationGuiBinding>();
		
		public ConfigurationGuiHelper(MSBuildBasedProject project, Dictionary<string, Control> controlDictionary)
		{
			this.project = project;
			this.controlDictionary = controlDictionary;
			this.configuration = project.ActiveConfiguration.Configuration;
			this.platform = project.ActiveConfiguration.Platform;
		}
		
		public MSBuildBasedProject Project {
			get {
				return project;
			}
		}
		
		internal Dictionary<string, Control> ControlDictionary {
			get {
				return controlDictionary;
			}
		}
		
		#region Get/Set properties
		public T GetProperty<T>(string propertyName, T defaultValue,
		                        bool treatPropertyValueAsLiteral)
		{
			string v;
			if (treatPropertyValueAsLiteral)
				v = project.GetProperty(configuration, platform, propertyName);
			else
				v = project.GetUnevalatedProperty(configuration, platform, propertyName);
			return GenericConverter.FromString(v, defaultValue);
		}
		
		public T GetProperty<T>(string propertyName, T defaultValue,
		                        bool treatPropertyValueAsLiteral,
		                        out PropertyStorageLocations location)
		{
			string v;
			if (treatPropertyValueAsLiteral)
				v = project.GetProperty(configuration, platform, propertyName, out location);
			else
				v = project.GetUnevalatedProperty(configuration, platform, propertyName, out location);
			return GenericConverter.FromString(v, defaultValue);
		}
		
		public void SetProperty<T>(string propertyName, T value,
		                           bool treatPropertyValueAsLiteral,
		                           PropertyStorageLocations location)
		{
			project.SetProperty(configuration, platform, propertyName,
			                    GenericConverter.ToString(value), location, treatPropertyValueAsLiteral);
		}
		#endregion
		
		#region Manage bindings
		/// <summary>
		/// Initializes the Property and Project properties on the binding and calls the load method on it.
		/// Registers the binding so that Save is called on it when Save is called on the ConfigurationGuiHelper.
		/// </summary>
		public void AddBinding(string property, ConfigurationGuiBinding binding)
		{
			binding.Property = property;
			binding.Helper = this;
			binding.Load();
			bindings.Add(binding);
		}
		
		public void Load()
		{
			if (Loading != null) {
				Loading(this, EventArgs.Empty);
			}
			foreach (ConfigurationGuiBinding binding in bindings) {
				binding.Load();
			}
			if (Loaded != null) {
				Loaded(this, EventArgs.Empty);
			}
			IsDirty = false;
		}
		
		public bool Save()
		{
			foreach (ConfigurationGuiBinding binding in bindings) {
				if (!binding.Save()) {
					return false;
				}
			}
			if (Saved != null) {
				Saved(this, EventArgs.Empty);
			}
			IsDirty = false;
			return true;
		}
		
		/// <summary>
		/// This event is raised when another configuration is beginning to load.
		/// </summary>
		public event EventHandler Loading;
		
		/// <summary>
		/// This event is raised when another configuration has been loaded.
		/// </summary>
		public event EventHandler Loaded;
		
		/// <summary>
		/// This event is raised after the configuration has been saved.
		/// </summary>
		public event EventHandler Saved;
		
		void ControlValueChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}
		
		bool dirty;
		
		public bool IsDirty {
			get {
				return dirty;
			}
			set {
				if (dirty != value) {
					dirty = value;
					if (IsDirtyChanged != null) {
						IsDirtyChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		
		public event EventHandler IsDirtyChanged;
		
		string configuration;
		
		public string Configuration {
			get {
				return configuration;
			}
			set {
				configuration = value;
			}
		}
		
		string platform;

		public string Platform {
			get {
				return platform;
			}
			set {
				platform = value;
			}
		}
		
		#region Bind bool to CheckBox
		public ConfigurationGuiBinding BindBoolean(string control, string property, bool defaultValue)
		{
			return BindBoolean(controlDictionary[control], property, defaultValue);
		}
		
		public ConfigurationGuiBinding BindBoolean(Control control, string property, bool defaultValue)
		{
			CheckBox checkBox = control as CheckBox;
			if (checkBox != null) {
				CheckBoxBinding binding = new CheckBoxBinding(checkBox, defaultValue);
				AddBinding(property, binding);
				checkBox.CheckedChanged += ControlValueChanged;
				return binding;
			} else {
				throw new ApplicationException("Cannot bind " + control.GetType().Name + " to bool property.");
			}
		}
		
		class CheckBoxBinding : ConfigurationGuiBinding
		{
			CheckBox control;
			bool defaultValue;
			
			public CheckBoxBinding(CheckBox control, bool defaultValue)
			{
				this.control = control;
				this.defaultValue = defaultValue;
			}
			
			public override void Load()
			{
				control.Checked = Get(defaultValue);
			}
			
			public override bool Save()
			{
				string oldValue = Get("True");
				if (oldValue == "true" || oldValue == "false") {
					// keep value in lower case
					Set(control.Checked.ToString().ToLowerInvariant());
				} else {
					Set(control.Checked.ToString());
				}
				return true;
			}
		}
		#endregion
		
		#region Bind string to TextBox or ComboBox
		static Func<string> GetEmptyString = delegate { return ""; };
		
		public ConfigurationGuiBinding BindString(string control, string property, TextBoxEditMode textBoxEditMode)
		{
			return BindString(controlDictionary[control], property, textBoxEditMode, GetEmptyString);
		}
		
		public ConfigurationGuiBinding BindString(Control control, string property, TextBoxEditMode textBoxEditMode)
		{
			return BindString(control, property, textBoxEditMode, GetEmptyString);
		}
		
		public ConfigurationGuiBinding BindString(Control control, string property, TextBoxEditMode textBoxEditMode, Func<string> defaultValueProvider)
		{
			if (control is TextBoxBase || control is ComboBox) {
				SimpleTextBinding binding = new SimpleTextBinding(control, defaultValueProvider);
				if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
					binding.TreatPropertyValueAsLiteral = true;
				} else {
					binding.TreatPropertyValueAsLiteral = false;
				}
				AddBinding(property, binding);
				control.TextChanged += ControlValueChanged;
				if (control is ComboBox) {
					control.KeyDown += ComboBoxKeyDown;
				}
				return binding;
			} else {
				throw new ApplicationException("Cannot bind " + control.GetType().Name + " to string property.");
			}
		}
		
		void ComboBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == (Keys.Control | Keys.S)) {
				e.Handled = true;
				new ICSharpCode.SharpDevelop.Commands.SaveFile().Run();
			}
		}
		
		class SimpleTextBinding : ConfigurationGuiBinding
		{
			Control control;
			Func<string> defaultValueProvider;
			
			public SimpleTextBinding(Control control, Func<string> defaultValueProvider)
			{
				this.defaultValueProvider = defaultValueProvider;
				this.control = control;
			}
			
			public override void Load()
			{
				control.Text = Get(defaultValueProvider());
			}
			
			public override bool Save()
			{
				if (control.Text == defaultValueProvider())
					Set("");
				else
					Set(control.Text);
				return true;
			}
		}
		#endregion
		
		#region Bind int to NumericUpDown
		public ConfigurationGuiBinding BindInt(string control, string property, int defaultValue)
		{
			return BindInt(controlDictionary[control], property, defaultValue);
		}

		public ConfigurationGuiBinding BindInt(Control control, string property, int defaultValue)
		{
			if (control is NumericUpDown) {
				SimpleIntBinding binding = new SimpleIntBinding((NumericUpDown)control, defaultValue);
				AddBinding(property, binding);
				control.TextChanged += ControlValueChanged;
				return binding;
			} else {
				throw new ApplicationException("Cannot bind " + control.GetType().Name + " to int property.");
			}
		}

		class SimpleIntBinding : ConfigurationGuiBinding
		{
			NumericUpDown control;
			int           defaultValue;
			
			public SimpleIntBinding(NumericUpDown control, int defaultValue)
			{
				this.control = control;
				this.defaultValue = defaultValue;
			}
			
			public override void Load()
			{
				int val;
				if (!int.TryParse(Get(defaultValue.ToString(NumberFormatInfo.InvariantInfo)), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out val)) {
					val = defaultValue;
				}
				control.Text = val.ToString();
			}
			
			public override bool Save()
			{
				string txt = control.Text.Trim();
				NumberStyles style = NumberStyles.Integer;
				int val;
				val = int.Parse(txt, style, NumberFormatInfo.InvariantInfo);
				Set(val.ToString(NumberFormatInfo.InvariantInfo));
				return true;
			}
		}
		#endregion
		
		#region Bind hex number to TextBox
		public ConfigurationGuiBinding BindHexadecimal(TextBoxBase textBox, string property, int defaultValue)
		{
			HexadecimalBinding binding = new HexadecimalBinding(textBox, defaultValue);
			AddBinding(property, binding);
			textBox.TextChanged += ControlValueChanged;
			return binding;
		}
		
		class HexadecimalBinding : ConfigurationGuiBinding
		{
			TextBoxBase textBox;
			int defaultValue;
			
			public HexadecimalBinding(TextBoxBase textBox, int defaultValue)
			{
				this.textBox = textBox;
				this.defaultValue = defaultValue;
			}
			
			public override void Load()
			{
				int val;
				if (!int.TryParse(Get(defaultValue.ToString(NumberFormatInfo.InvariantInfo)), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out val)) {
					val = defaultValue;
				}
				textBox.Text = "0x" + val.ToString("x", NumberFormatInfo.InvariantInfo);
			}
			
			public override bool Save()
			{
				string txt = textBox.Text.Trim();
				NumberStyles style = NumberStyles.Integer;
				if (txt.StartsWith("0x")) {
					txt = txt.Substring(2);
					style = NumberStyles.HexNumber;
				}
				int val;
				if (!int.TryParse(txt, style, NumberFormatInfo.InvariantInfo, out val)) {
					textBox.Focus();
					MessageService.ShowMessage("${res:Dialog.ProjectOptions.PleaseEnterValidNumber}");
					return false;
				}
				Set(val.ToString(NumberFormatInfo.InvariantInfo));
				return true;
			}
		}
		#endregion
		
		#region Bind enum to ComboBox
		/// <summary>
		/// Bind enum to ComboBox. Assumes the first enum member is the default.
		/// </summary>
		public ConfigurationGuiBinding BindEnum<T>(string control, string property, params T[] values) where T : struct
		{
			return BindEnum(controlDictionary[control], property, values);
		}
		
		/// <summary>
		/// Bind enum to ComboBox. Assumes the first enum member is the default.
		/// </summary>
		public ConfigurationGuiBinding BindEnum<T>(Control control, string property, params T[] values) where T : struct
		{
			Type type = typeof(T);
			if (values == null || values.Length == 0) {
				values = (T[])Enum.GetValues(type);
			}
			ComboBox comboBox = control as ComboBox;
			if (comboBox != null) {
				foreach (T element in values) {
					object[] attr = type.GetField(Enum.GetName(type, element)).GetCustomAttributes(typeof(DescriptionAttribute), false);
					string description;
					if (attr.Length > 0) {
						description = StringParser.Parse((attr[0] as DescriptionAttribute).Description);
					} else {
						description = Enum.GetName(type, element);
					}
					comboBox.Items.Add(description);
				}
				string[] valueNames = new string[values.Length];
				for (int i = 0; i < values.Length; i++)
					valueNames[i] = values[i].ToString();
				ComboBoxBinding binding = new ComboBoxBinding(comboBox, valueNames, valueNames[0]);
				AddBinding(property, binding);
				comboBox.SelectedIndexChanged += ControlValueChanged;
				comboBox.KeyDown += ComboBoxKeyDown;
				return binding;
			} else {
				throw new ApplicationException("Cannot bind " + control.GetType().Name + " to enum property.");
			}
		}
		
		/// <summary>
		/// Bind list of strings to ComboBox.
		/// entries: value -> Description
		/// </summary>
		public ConfigurationGuiBinding BindStringEnum(string control, string property, string defaultValue, params KeyValuePair<string, string>[] entries)
		{
			return BindStringEnum(controlDictionary[control], property, defaultValue, entries);
		}
		
		/// <summary>
		/// Bind list of strings to ComboBox.
		/// entries: value -> Description
		/// </summary>
		public ConfigurationGuiBinding BindStringEnum(Control control, string property, string defaultValue, params KeyValuePair<string, string>[] entries)
		{
			ComboBox comboBox = control as ComboBox;
			if (comboBox != null) {
				string[] valueNames = new string[entries.Length];
				for (int i = 0; i < entries.Length; i++) {
					valueNames[i] = entries[i].Key;
					comboBox.Items.Add(StringParser.Parse(entries[i].Value));
				}
				ComboBoxBinding binding = new ComboBoxBinding(comboBox, valueNames, defaultValue);
				AddBinding(property, binding);
				comboBox.SelectedIndexChanged += ControlValueChanged;
				comboBox.KeyDown += ComboBoxKeyDown;
				return binding;
			} else {
				throw new ApplicationException("Cannot bind " + control.GetType().Name + " to enum property.");
			}
		}
		
		class ComboBoxBinding : ConfigurationGuiBinding
		{
			ComboBox control;
			string[] values;
			string defaultValue;
			
			public ComboBoxBinding(ComboBox control, string[] values, string defaultValue)
			{
				this.control = control;
				this.values = values;
				this.defaultValue = defaultValue;
			}
			
			public override void Load()
			{
				string val = Get(defaultValue);
				int i;
				for (i = 0; i < values.Length; i++) {
					if (val.Equals(values[i], StringComparison.OrdinalIgnoreCase))
						break;
				}
				if (i == values.Length) i = 0;
				control.SelectedIndex = i;
			}
			
			public override bool Save()
			{
				Set(values[control.SelectedIndex]);
				return true;
			}
		}
		#endregion
		
		#region Bind enum to RadioButtons
		/// <summary>
		/// Bind enum to RadioButtons
		/// </summary>
		public ConfigurationGuiBinding BindRadioEnum<T>(string property, params KeyValuePair<T, RadioButton>[] values) where T : struct
		{
			RadioEnumBinding<T> binding = new RadioEnumBinding<T>(values);
			AddBinding(property, binding);
			foreach (KeyValuePair<T, RadioButton> pair in values) {
				pair.Value.CheckedChanged += ControlValueChanged;
			}
			return binding;
		}
		
		class RadioEnumBinding<T> : ConfigurationGuiBinding where T : struct
		{
			KeyValuePair<T, RadioButton>[] values;
			
			internal RadioEnumBinding(KeyValuePair<T, RadioButton>[] values)
			{
				this.values = values;
			}
			
			public override void Load()
			{
				T val = Get(values[0].Key);
				int i;
				for (i = 0; i < values.Length; i++) {
					if (val.Equals(values[i].Key))
						break;
				}
				if (i == values.Length) i = 0;
				values[i].Value.Checked = true;
			}
			
			public override bool Save()
			{
				foreach (KeyValuePair<T, RadioButton> pair in values) {
					if (pair.Value.Checked) {
						Set(pair.Key);
						break;
					}
				}
				return true;
			}
		}
		#endregion
		#endregion
		
		#region ConfigurationSelector
		/// <summary>
		/// Gets the height of the configuration selector in pixel.
		/// </summary>
		public const int ConfigurationSelectorHeight = 30;
		
		public Control CreateConfigurationSelector()
		{
			return new ConfigurationSelector(this);
		}
		
		public void AddConfigurationSelector(Control parent)
		{
			foreach (Control ctl in parent.Controls) {
				ctl.Top += ConfigurationSelectorHeight;
			}
			Control sel = CreateConfigurationSelector();
			sel.Width = parent.ClientSize.Width;
			parent.Controls.Add(sel);
			parent.Controls.SetChildIndex(sel, 0);
			sel.Anchor |= AnchorStyles.Right;
		}
		
		sealed class ConfigurationSelector : Panel
		{
			ConfigurationGuiHelper helper;
			Label    configurationLabel = new Label();
			ComboBox configurationComboBox = new ComboBox();
			Label    platformLabel = new Label();
			ComboBox platformComboBox = new ComboBox();
			Control  line = new Control();
			
			public ConfigurationSelector(ConfigurationGuiHelper helper)
			{
				const int marginTop  = 4;
				const int marginLeft = 4;
				this.helper = helper;
				this.Height = ConfigurationSelectorHeight;
				configurationLabel.Text      = StringParser.Parse("${res:Dialog.ProjectOptions.Configuration}:");
				configurationLabel.TextAlign = ContentAlignment.MiddleRight;
				configurationLabel.Location  = new Point(marginLeft, marginTop);
				configurationLabel.Width     = 80;
				configurationComboBox.Location      = new Point(4 + configurationLabel.Right, marginTop);
				configurationComboBox.Width         = 120;
				configurationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				platformLabel.Text      = StringParser.Parse("${res:Dialog.ProjectOptions.Platform}:");
				platformLabel.TextAlign = ContentAlignment.MiddleRight;
				platformLabel.Location  = new Point(4 + configurationComboBox.Right, marginTop);
				platformLabel.Width     = 68;
				platformComboBox.Location      = new Point(4 + platformLabel.Right, marginTop);
				platformComboBox.Width         = 120;
				platformComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				line.Bounds    = new Rectangle(marginLeft, ConfigurationSelectorHeight - 2, Width - marginLeft * 2, ConfigurationSelectorHeight - 2);
				line.BackColor = SystemColors.ControlDark;
				this.Controls.AddRange(new Control[] { configurationLabel, configurationComboBox, platformLabel, platformComboBox, line });
				line.Anchor |= AnchorStyles.Right;
				FillBoxes();
				configurationComboBox.SelectedIndexChanged += ConfigurationChanged;
				platformComboBox.SelectedIndexChanged      += ConfigurationChanged;
			}
			
			void FillBoxes()
			{
				List<string> items;
				configurationComboBox.Items.Clear();
				items = helper.Project.ConfigurationNames.ToList();
				items.Sort();
				configurationComboBox.Items.AddRange(items.ToArray());
				platformComboBox.Items.Clear();
				items = helper.Project.PlatformNames.ToList();
				items.Sort();
				platformComboBox.Items.AddRange(items.ToArray());
				ResetIndex();
			}
			
			bool resettingIndex;
			
			void ResetIndex()
			{
				resettingIndex = true;
				configurationComboBox.SelectedIndex = configurationComboBox.Items.IndexOf(helper.Configuration);
				platformComboBox.SelectedIndex      = platformComboBox.Items.IndexOf(helper.Platform);
				resettingIndex = false;
			}
			
			void ConfigurationChanged(object sender, EventArgs e)
			{
				if (resettingIndex) return;
				if (helper.IsDirty) {
					if (!MessageService.AskQuestion("${res:Dialog.ProjectOptions.ContinueSwitchConfiguration}")) {
						ResetIndex();
						return;
					}
					if (!helper.Save()) {
						ResetIndex();
						return;
					}
					helper.Project.Save();
				}
				helper.Configuration = (string)configurationComboBox.SelectedItem;
				helper.Platform      = (string)platformComboBox.SelectedItem;
				helper.Load();
			}
		}
		#endregion
	}
}
