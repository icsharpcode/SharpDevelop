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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Base class for project option panels with configuration picker.
	/// </summary>
	public class ProjectOptionPanel : UserControl, IOptionPanel, ICanBeDirty, INotifyPropertyChanged, IDisposable
	{
		
		static ProjectOptionPanel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ProjectOptionPanel), new FrameworkPropertyMetadata(typeof(ProjectOptionPanel)));
		}
		
		public ProjectOptionPanel()
		{
			this.DataContext = this;
		}
		
		/// <summary>
		/// Initializes the project option panel.
		/// This method is called after the project property was initialized,
		/// immediately before the first Load() call.
		/// </summary>
		protected virtual void Initialize()
		{
		}
		
		public virtual void Dispose()
		{
		}
		
		ComboBox configurationComboBox;
		ComboBox platformComboBox;
		MSBuildBasedProject project;
		string activeConfiguration;
		string activePlatform;
		bool resettingIndex;
		bool isLoaded;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		List<ILoadSaveCallback> loadSaveCallbacks = new List<ILoadSaveCallback>();

		public void RegisterLoadSaveCallback(ILoadSaveCallback callback)
		{
			if (callback == null)
				throw new ArgumentNullException("callback");
			loadSaveCallbacks.Add(callback);
		}
		
		protected virtual void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			// First load project properties; then invoke other callbacks
			// so that the callbacks can use the new values in the properties
			foreach (ILoadSaveCallback p in projectProperties.Values.Concat(loadSaveCallbacks))
				p.Load(project, configuration, platform);
			this.IsDirty = false;
		}
		
		protected virtual bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			// First invoke callbacks; then save project properties.
			// So that the callbacks can store values via the properties
			foreach (ILoadSaveCallback p in loadSaveCallbacks.Concat(projectProperties.Values)) {
				if (!p.Save(project, configuration, platform))
					return false;
			}
			this.IsDirty = false;
			return true;
		}
		
		
		public static readonly DependencyProperty HeaderVisibilityProperty =
			DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(ProjectOptionPanel),
			                            new FrameworkPropertyMetadata(Visibility.Visible));
		
		public Visibility HeaderVisibility {
			get { return (Visibility)GetValue(HeaderVisibilityProperty); }
			set { SetValue(HeaderVisibilityProperty, value); }
		}
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			configurationComboBox = Template.FindName("PART_configuration", this) as ComboBox;
			platformComboBox = Template.FindName("PART_platform", this) as ComboBox;
		}
		
		
		object owner;
		
		object IOptionPanel.Owner {
			get { return owner; }
			set { owner = value; }
		}
		
		object IOptionPanel.Control {
			get { return this; }
		}
		
		void IOptionPanel.LoadOptions()
		{
			ApplyTemplate();
			project = (MSBuildBasedProject)owner;
			if (configurationComboBox != null) {
				List<string> configurations = project.ConfigurationNames.Union(new[] { project.ActiveConfiguration.Configuration }).ToList();
				configurations.Sort();
				configurationComboBox.ItemsSource = configurations;
				configurationComboBox.SelectedItem = project.ActiveConfiguration.Configuration;
				configurationComboBox.SelectionChanged += comboBox_SelectionChanged;
			}
			if (platformComboBox != null) {
				List<string> platforms = project.PlatformNames.Union(new[] { project.ActiveConfiguration.Platform }).ToList();
				platforms.Sort();
				platformComboBox.ItemsSource = platforms;
				platformComboBox.SelectedItem = project.ActiveConfiguration.Platform;
				platformComboBox.SelectionChanged += comboBox_SelectionChanged;
			}
			Initialize();
			Load();
		}
		
		void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (resettingIndex)
				return;
			if (isDirty) {
				if (!MessageService.AskQuestion("${res:Dialog.ProjectOptions.ContinueSwitchConfiguration}")) {
					ResetComboBoxIndex();
					return;
				}
				if (!Save(project, activeConfiguration, activePlatform)) {
					ResetComboBoxIndex();
					return;
				}
				project.Save();
			}
			Load();
		}
		
		void ResetComboBoxIndex()
		{
			resettingIndex = true;
			if (configurationComboBox != null)
				configurationComboBox.SelectedItem = activeConfiguration;
			if (platformComboBox != null)
				platformComboBox.SelectedItem = activePlatform;
			resettingIndex = false;
		}
		
		void Load()
		{
			if (configurationComboBox != null)
				activeConfiguration = (string)configurationComboBox.SelectedItem;
			else
				activeConfiguration = project.ActiveConfiguration.Configuration;
			
			if (platformComboBox != null)
				activePlatform = (string)platformComboBox.SelectedItem;
			else
				activePlatform = project.ActiveConfiguration.Platform;
			
			isLoaded = true;
			Load(project, activeConfiguration, activePlatform);
		}
		
		bool IOptionPanel.SaveOptions()
		{
			return Save(project, activeConfiguration, activePlatform);
		}
		
		bool isDirty;
		
		public bool IsDirty {
			get { return isDirty; }
			set {
				if (isDirty != value) {
					isDirty = value;
					if (IsDirtyChanged != null)
						IsDirtyChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public MSBuildBasedProject Project {
			get { return project; }
		}
		
		public string BaseDirectory {
			get { return project.Directory; }
		}
		
		public event EventHandler IsDirtyChanged;
		
		#region Manage MSBuild properties
		Dictionary<string, ILoadSaveCallback> projectProperties = new Dictionary<string, ILoadSaveCallback>();
		
		public ProjectProperty<string> GetProperty(string propertyName, string defaultValue,
		                                           TextBoxEditMode textBoxEditMode = TextBoxEditMode.EditEvaluatedProperty,
		                                           PropertyStorageLocations defaultLocation = PropertyStorageLocations.Base)
		{
			ILoadSaveCallback existingProperty;
			if (projectProperties.TryGetValue(propertyName, out existingProperty))
				return (ProjectProperty<string>)existingProperty;
			
			bool treatAsLiteral = (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty);
			ProjectProperty<string> newProperty = new ProjectProperty<string>(this, propertyName, defaultValue, defaultLocation, treatAsLiteral);
			projectProperties.Add(propertyName, newProperty);
			if (isLoaded)
				newProperty.Load(project, activeConfiguration, activePlatform);
			return newProperty;
		}
		
		public ProjectProperty<T> GetProperty<T>(string propertyName, T defaultValue,
		                                         PropertyStorageLocations defaultLocation = PropertyStorageLocations.Base)
		{
			ILoadSaveCallback existingProperty;
			if (projectProperties.TryGetValue(propertyName, out existingProperty))
				return (ProjectProperty<T>)existingProperty;
			
			ProjectProperty<T> newProperty = new ProjectProperty<T>(this, propertyName, defaultValue, defaultLocation, true);
			projectProperties.Add(propertyName, newProperty);
			if (isLoaded)
				newProperty.Load(project, activeConfiguration, activePlatform);
			return newProperty;
		}
		
		public interface ILoadSaveCallback
		{
			void Load(MSBuildBasedProject project, string configuration, string platform);
			bool Save(MSBuildBasedProject project, string configuration, string platform);
		}
		
		public class ProjectProperty<T> : ILoadSaveCallback, INotifyPropertyChanged
		{
			readonly ProjectOptionPanel parentPanel;
			readonly string propertyName;
			readonly T defaultValue;
			readonly PropertyStorageLocations defaultLocation;
			readonly bool treatPropertyValueAsLiteral;
			T val;
			PropertyStorageLocations location;
			bool isLoading;
			
			public ProjectProperty(ProjectOptionPanel parentPanel, string propertyName, T defaultValue, PropertyStorageLocations defaultLocation, bool treatPropertyValueAsLiteral)
			{
				this.parentPanel = parentPanel;
				this.propertyName = propertyName;
				this.defaultValue = defaultValue;
				this.defaultLocation = defaultLocation;
				this.treatPropertyValueAsLiteral = treatPropertyValueAsLiteral;
				
				this.val = defaultValue;
				this.location = defaultLocation;
			}
			
			public TextBoxEditMode TextBoxEditMode {
				get { return treatPropertyValueAsLiteral ? TextBoxEditMode.EditEvaluatedProperty : TextBoxEditMode.EditRawProperty; }
			}
			
			public string PropertyName {
				get { return propertyName; }
			}
			
			public T Value {
				get { return val; }
				set {
					if (!object.Equals(val, value)) {
						val = value;
						if (PropertyChanged != null)
							PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Value"));
						
						if (!isLoading)
							parentPanel.IsDirty = true;
					}
				}
			}
			
			public PropertyStorageLocations Location {
				get { return location; }
				set {
					if (location != value) {
						location = value;
						if (PropertyChanged != null)
							PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Location"));
						
						if (!isLoading)
							parentPanel.IsDirty = true;
					}
				}
			}
			
			public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
			
			public void Load(MSBuildBasedProject project, string configuration, string platform)
			{
				PropertyStorageLocations newLocation;
				string v;
				if (treatPropertyValueAsLiteral)
					v = project.GetProperty(configuration, platform, propertyName, out newLocation);
				else
					v = project.GetUnevalatedProperty(configuration, platform, propertyName, out newLocation);
				
				if (newLocation == PropertyStorageLocations.Unknown)
					newLocation = defaultLocation;
				
				isLoading = true;
				try {
					this.Value = GenericConverter.FromString(v, defaultValue);
					this.Location = newLocation;
				} finally {
					isLoading = false;
				}
			}
			
			public bool Save(MSBuildBasedProject project, string configuration, string platform)
			{
				string newValue = GenericConverter.ToString(val);
				project.SetProperty(configuration, platform, propertyName, newValue, location, treatPropertyValueAsLiteral);
				return true;
			}
		}
		#endregion
		
		#region INotifyPropertyChanged implementation
		
		protected void RaisePropertyChanged(string propertyName)
		{
			RaiseInternal(propertyName);
		}
		
		
		protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
		{
			var propertyName = ExtractPropertyName(propertyExpresssion);
			RaiseInternal(propertyName);
		}
		
		
		private void RaiseInternal (string propertyName)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		private static String ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)
		{
			if (propertyExpresssion == null)
			{
				throw new ArgumentNullException("propertyExpresssion");
			}

			var memberExpression = propertyExpresssion.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException("The expression is not a member access expression.", "propertyExpresssion");
			}

			var property = memberExpression.Member as PropertyInfo;
			if (property == null)
			{
				throw new ArgumentException("The member access expression does not access a property.", "propertyExpresssion");
			}

			var getMethod = property.GetGetMethod(true);
			if (getMethod.IsStatic)
			{
				throw new ArgumentException("The referenced property is a static property.", "propertyExpresssion");
			}

			return memberExpression.Member.Name;
		}
		
		#endregion
		
		#region Browse Helper
		/// <summary>
		/// Shows the 'Browse for folder' dialog.
		/// </summary>
		public void BrowseForFolder(ProjectProperty<string> property, string description)
		{
			string path = BrowseForFolder(description, BaseDirectory, property.TextBoxEditMode);
			if (!String.IsNullOrEmpty(path)) {
				property.Value = path;
			}	
		}
		
		/// <summary>
		/// Shows the 'Browse for folder' dialog.
		/// </summary>
		/// <param name="description">A description shown inside the dialog.</param>
		/// <param name="startLocation">Start location, relative to the <see cref="BaseDirectory"/></param>
		/// <param name="textBoxEditMode">The TextBoxEditMode used for the text box containing the file name</param>
		/// <returns>Returns the location of the folder; or null if the dialog was cancelled.</returns>
		private string BrowseForFolder(string description, string startLocation, TextBoxEditMode textBoxEditMode)
		{
			string startAt = GetInitialDirectory(startLocation, textBoxEditMode, false);
			string path = SD.FileService.BrowseForFolder(description,startAt);
			if (String.IsNullOrEmpty(path)) {
				return null;
			} else {
				if (!String.IsNullOrEmpty(startLocation)) {
					path = FileUtility.GetRelativePath(startLocation, path);
				}
				if (!path.EndsWith("\\", StringComparison.Ordinal) && !path.EndsWith("/", StringComparison.Ordinal))
					path += "\\";
				if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
					return path;
				} else {
					return MSBuildInternals.Escape(path);
				}
			}
		}
		
		/// <summary>
		/// Shows an 'Open File' dialog.
		/// </summary>
		protected void BrowseForFile(ProjectProperty<string> property, string filter)
		{
			string newValue = BrowseForFile(filter, property.Value, property.TextBoxEditMode);
			if (newValue != null)
				property.Value = newValue;
		}
		
		/// <summary>
		/// Shows an 'Open File' dialog.
		/// </summary>
		/// <param name="filter">The filter string that determines which files are displayed.</param>
		/// <param name="startLocation">Start location, relative to the <see cref="BaseDirectory"/></param>
		/// <param name="textBoxEditMode">The TextBoxEditMode used for the text box containing the file name</param>
		/// <returns>Returns the location of the file; or null if the dialog was cancelled.</returns>
		protected string BrowseForFile(string filter, string startLocation, TextBoxEditMode textBoxEditMode)
		{
			var dialog = new OpenFileDialog();
			dialog.InitialDirectory = GetInitialDirectory(startLocation, textBoxEditMode, true);
			
			if (!String.IsNullOrEmpty(filter)) {
				dialog.Filter = StringParser.Parse(filter);
			}
			
			if (dialog.ShowDialog() == true) {
				string fileName = dialog.FileName;
				if (!String.IsNullOrEmpty(this.BaseDirectory)) {
					fileName = FileUtility.GetRelativePath(this.BaseDirectory, fileName);
				}
				if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
					return fileName;
				} else {
					return MSBuildInternals.Escape(fileName);
				}
			}
			return null;
		}
		
		string GetInitialDirectory(string relativeLocation, TextBoxEditMode textBoxEditMode, bool isFile)
		{
			if (textBoxEditMode == TextBoxEditMode.EditRawProperty)
				relativeLocation = MSBuildInternals.Unescape(relativeLocation);
			if (string.IsNullOrEmpty(relativeLocation))
				return this.BaseDirectory;
			
			try {
				string path = FileUtility.GetAbsolutePath(this.BaseDirectory, relativeLocation);
				if (FileUtility.IsValidPath(path))
					return isFile ? System.IO.Path.GetDirectoryName(path) : path;
			} catch (ArgumentException) {
				// can happen in GetAbsolutePath if the path contains invalid characters
			}
			return this.BaseDirectory;
		}
		#endregion
	}
}
