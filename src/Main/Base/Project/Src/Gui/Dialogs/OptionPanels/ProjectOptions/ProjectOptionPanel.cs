// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;
using System.Windows.Shapes;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Base class for project option panels with configuration picker.
	/// </summary>
	public class ProjectOptionPanel : UserControl, IOptionPanel, ICanBeDirty,INotifyPropertyChanged
	{
		
		static ProjectOptionPanel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ProjectOptionPanel), new FrameworkPropertyMetadata(typeof(ProjectOptionPanel)));
		}
		
		public ProjectOptionPanel()
		{
			this.DataContext = this;
		}
		
		ComboBox configurationComboBox;
		ComboBox platformComboBox;
		MSBuildBasedProject project;
		string activeConfiguration;
		string activePlatform;
		bool resettingIndex;
		
		StackPanel configStackPanel;
		Line headerline;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			foreach (IProjectProperty p in projectProperties.Values)
				p.Load(project, configuration, platform);
			this.IsDirty = false;
		}
		
		protected virtual bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			foreach (IProjectProperty p in projectProperties.Values)
				p.Save(project, configuration, platform);
			this.IsDirty = false;
			return true;
		}
		
		
		public void HideHeader ()
		{
			configStackPanel.Visibility = Visibility.Hidden;
			headerline.Visibility = Visibility.Hidden;
		}
		
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			configurationComboBox = Template.FindName("PART_configuration", this) as ComboBox;
			platformComboBox = Template.FindName("PART_platform", this) as ComboBox;

			headerline = Template.FindName("PART_headerline", this) as Line;
			configStackPanel = Template.FindName("PART_stackpanel", this) as StackPanel;
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
				List<string> configurations = project.ConfigurationNames.Union(new[] { project.ActiveConfiguration }).ToList();
				configurations.Sort();
				configurationComboBox.ItemsSource = configurations;
				configurationComboBox.SelectedItem = project.ActiveConfiguration;
				configurationComboBox.SelectionChanged += comboBox_SelectionChanged;
			}
			if (platformComboBox != null) {
				List<string> platforms = project.PlatformNames.Union(new[] { project.ActivePlatform }).ToList();
				platforms.Sort();
				platformComboBox.ItemsSource = platforms;
				platformComboBox.SelectedItem = project.ActivePlatform;
				platformComboBox.SelectionChanged += comboBox_SelectionChanged;
			}
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
				activeConfiguration = project.ActiveConfiguration;
			
			if (platformComboBox != null)
				activePlatform = (string)platformComboBox.SelectedItem;
			else
				activePlatform = project.ActivePlatform;
			
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
		
		public string BaseDirectory
		{
			get {return project.Directory;}
		}
			
		public event EventHandler IsDirtyChanged;
		
		#region Manage MSBuild properties
		Dictionary<string, IProjectProperty> projectProperties = new Dictionary<string, IProjectProperty>();
		
		public ProjectProperty<string> GetProperty(string propertyName, string defaultValue,
		                                           TextBoxEditMode textBoxEditMode = TextBoxEditMode.EditEvaluatedProperty,
		                                           PropertyStorageLocations defaultLocation = PropertyStorageLocations.Base)
		{
			IProjectProperty existingProperty;
			if (projectProperties.TryGetValue(propertyName, out existingProperty))
				return (ProjectProperty<string>)existingProperty;
			
			bool treatAsLiteral = (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty);
			ProjectProperty<string> newProperty = new ProjectProperty<string>(this, propertyName, defaultValue, defaultLocation, treatAsLiteral);
			projectProperties.Add(propertyName, newProperty);
			return newProperty;
		}
		
		public ProjectProperty<T> GetProperty<T>(string propertyName, T defaultValue,
		                                         PropertyStorageLocations defaultLocation = PropertyStorageLocations.Base)
		{
			IProjectProperty existingProperty;
			if (projectProperties.TryGetValue(propertyName, out existingProperty))
				return (ProjectProperty<T>)existingProperty;
			
			ProjectProperty<T> newProperty = new ProjectProperty<T>(this, propertyName, defaultValue, defaultLocation, true);
			projectProperties.Add(propertyName, newProperty);
			return newProperty;
		}
		
		interface IProjectProperty
		{
			void Load(MSBuildBasedProject project, string configuration, string platform);
			void Save(MSBuildBasedProject project, string configuration, string platform);
		}
		
		public class ProjectProperty<T> : IProjectProperty, INotifyPropertyChanged
		{
			readonly ProjectOptionPanel parentPanel;
			readonly string propertyName;
			readonly T defaultValue;
			readonly PropertyStorageLocations defaultLocation;
			readonly bool treatPropertyValueAsLiteral;
			T val;
			PropertyStorageLocations location;
			
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
				
				this.Value = GenericConverter.FromString(v, defaultValue);
				this.Location = newLocation;
			}
			
			public void Save(MSBuildBasedProject project, string configuration, string platform)
			{
				string newValue = GenericConverter.ToString(val);
				project.SetProperty(configuration, platform, propertyName, newValue, location, treatPropertyValueAsLiteral);
			}
		}
		#endregion
		
		#region INotifyPropertyChanged implementation
		
		protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
		{
			var propertyName = ExtractPropertyName(propertyExpresssion);
			//OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		/*
		protected virtual void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}
		*/
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
		
		
		#region OpenFileDialog
		
//		protected string OpenFile(string fileFilter)
//		{
//			var dialog = new OpenFileDialog();
//			
//			//dialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe");
//			var x = StringParser.Parse(fileFilter);
//			dialog.Filter = StringParser.Parse(x);
//			if (dialog.ShowDialog() ?? false) {
//				return  dialog.FileName;
//			}
//			return String.Empty;
//		}
//		
		
		protected string BrowseForFolder(string description,string startLocation,string relativeLocation)
		{
			
//			var startLocation = BaseDirectory;
//			if (textBoxEditMode == TextBoxEditMode.EditRawProperty)
//				string text =  StartWorkingDirectory.Value;
//				text = MSBuildInternals.Unescape(text);
//			
//			if (startLocation != null) {
//				string text = StartWorkingDirectory.Value;
//				startLocation = FileUtility.GetAbsolutePath(startLocation, text);
//			}
			string startAt = startLocation;
			if (!String.IsNullOrEmpty(relativeLocation)) {
			    	startAt = FileUtility.GetAbsolutePath(startLocation,relativeLocation);
			    }
			
			
			using (System.Windows.Forms.FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog(description,startAt))
			{
				if (fdiag.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					string path = fdiag.SelectedPath;
					if (BaseDirectory != null) {
						path = FileUtility.GetRelativePath(BaseDirectory, path);
					}
					if (!path.EndsWith("\\") && !path.EndsWith("/"))
						path += "\\";
					
//						if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
					////							panel.ControlDictionary[target].Text = path;
//						} else {
//							panel.ControlDictionary[target].Text = MSBuildInternals.Escape(path);
					////						}
					return path;
				}
				
			}
			return startLocation;
		}
		
		#endregion
	}
}
