using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	/// <summary>
	/// View-Model class for the property grid.
	/// </summary>
	public class PropertyNode : INotifyPropertyChanged
	{
		// don't warn on missing XML comments in View-Model
		#pragma warning disable 1591
		
		static object Unset = new object();

		public DesignItemProperty[] Properties { get; private set; }
		bool raiseEvents = true;
		bool hasStringConverter;

		public string Name { get { return FirstProperty.Name; } }
		public bool IsEvent { get { return FirstProperty.IsEvent; } }
		public ServiceContainer Services { get { return FirstProperty.DesignItem.Services; } }
		public FrameworkElement Editor { get; private set; }
		public DesignItemProperty FirstProperty { get { return Properties[0]; } }

		public PropertyNode Parent { get; private set; }
		public int Level { get; private set; }
		public Category Category { get; set; }
		public ObservableCollection<PropertyNode> Children { get; private set; }
		public ObservableCollection<PropertyNode> MoreChildren { get; private set; }

		bool isExpanded;

		public bool IsExpanded {
			get {
				return isExpanded;
			}
			set {
				isExpanded = value;
				UpdateChildren();
				RaisePropertyChanged("IsExpanded");
			}
		}

		public bool HasChildren {
			get { return Children.Count > 0 || MoreChildren.Count > 0; }
		}

		public object Description {
			get {
				IPropertyDescriptionService s = Services.GetService<IPropertyDescriptionService>();
				if (s != null) {
					return s.GetDescription(FirstProperty);
				}
				return null;
			}
		}

		public object Value {
			get {
				if (IsAmbiguous) return null;
				var result = FirstProperty.ValueOnInstance;
				if (result == DependencyProperty.UnsetValue) return null;
				return result;
			}
			set {
				SetValueCore(value);
			}
		}

		public string ValueString {
			get {
				if (ValueItem == null || ValueItem.Component is MarkupExtension) {
					if (Value == null) return null;
					if (hasStringConverter) {					
						return FirstProperty.TypeConverter.ConvertToInvariantString(Value);
					}
					return "(" + Value.GetType().Name + ")";
				}
				return "(" + ValueItem.ComponentType.Name + ")";
			}
			set {
				// TODO: Doesn't work for some reason
				try {
					Value = FirstProperty.TypeConverter.ConvertFromInvariantString(value);
				} catch {
					OnValueOnInstanceChanged();
				}
			}
		}

		public bool IsEnabled {
			get {
				return ValueItem == null && hasStringConverter;
			}
		}

		public bool IsSet {
			get {
				foreach (var p in Properties) {
					if (p.IsSet) return true;
				}
				return false;
			}
		}

		public FontWeight FontWeight {
			get {
				return IsSet ? FontWeights.Bold : FontWeights.Normal;
			}
		}

		public Brush NameForeground {
			get {
				if (ValueItem != null) {
					if (ValueItem.Component is BindingBase) 
						return Brushes.DarkGoldenrod;
					if (ValueItem.Component is StaticResourceExtension || 
						ValueItem.Component is DynamicResourceExtension) 
						return Brushes.DarkGreen;
				}
				return SystemColors.WindowTextBrush;
			}
		}

		public DesignItem ValueItem {
			get {
				if (Properties.Length == 1) {
					return FirstProperty.Value;
				}
				return null;
			}
		}

		public bool IsAmbiguous {
			get {
				foreach (var p in Properties) {
					if (!object.Equals(p.ValueOnInstance, FirstProperty.ValueOnInstance)) {
						return true;
					}
				}
				return false;
			}
		}

		bool isVisible;

		public bool IsVisible
		{
			get
			{
				return isVisible;
			}
			set
			{
				isVisible = value;
				RaisePropertyChanged("IsVisible");
			}
		}

		public bool CanReset {
			get { return IsSet; }
		}

		public void Reset()
		{
			SetValueCore(Unset);
		}

		public void CreateBinding()
		{
			Value = new Binding();
			IsExpanded = true;
		}

		void SetValueCore(object value)
		{
			raiseEvents = false;
			if (value == Unset) {
				foreach (var p in Properties) {
					p.Reset();
				}
			}
			else {
				foreach (var p in Properties) {
					p.SetValue(value);
				}
			}
			raiseEvents = true;
			OnValueChanged();
		}

		void OnValueChanged()
		{
			RaisePropertyChanged("IsSet");
			RaisePropertyChanged("Value");
			RaisePropertyChanged("ValueString");
			RaisePropertyChanged("IsAmbiguous");
			RaisePropertyChanged("FontWeight");
			RaisePropertyChanged("IsEnabled");
			RaisePropertyChanged("NameForeground");

			UpdateChildren();
		}

		void OnValueOnInstanceChanged()
		{
			RaisePropertyChanged("Value");
			RaisePropertyChanged("ValueString");
		}

		public PropertyNode()
		{
			Children = new ObservableCollection<PropertyNode>();
			MoreChildren = new ObservableCollection<PropertyNode>();
		}

		PropertyNode(DesignItemProperty[] properties, PropertyNode parent) : this()
		{
			this.Parent = parent;
			this.Level = parent == null ? 0 : parent.Level + 1;
			Load(properties);
		}

		public void Load(DesignItemProperty[] properties)
		{
			if (Properties != null) {
				foreach (var property in Properties) {
					property.ValueChanged -= new EventHandler(property_ValueChanged);
					property.ValueOnInstanceChanged -= new EventHandler(property_ValueOnInstanceChanged);
				}
			}

			this.Properties = properties;			

			foreach (var property in properties) {
				property.ValueChanged += new EventHandler(property_ValueChanged);
				property.ValueOnInstanceChanged += new EventHandler(property_ValueOnInstanceChanged);
			}

			if (Editor == null)
				Editor = EditorManager.CreateEditor(FirstProperty);

			hasStringConverter =
			    FirstProperty.TypeConverter.CanConvertFrom(typeof(string)) &&
			    FirstProperty.TypeConverter.CanConvertTo(typeof(string));

			OnValueChanged();
		}

		void property_ValueOnInstanceChanged(object sender, EventArgs e)
		{
			if (raiseEvents) OnValueOnInstanceChanged();
		}

		void property_ValueChanged(object sender, EventArgs e)
		{
			if (raiseEvents) OnValueChanged();
		}

		void UpdateChildren()
		{
			Children.Clear();
			MoreChildren.Clear();

			if (Parent == null || Parent.IsExpanded) {
				if (ValueItem != null) {
					var list = TypeHelper.GetAvailableProperties(ValueItem.ComponentType)
						.OrderBy(d => d.Name)
						.Select(d => new PropertyNode(new[] { ValueItem.Properties[d.Name] }, this));

					foreach (var node in list) {
						if (Metadata.IsBrowsable(node.FirstProperty)) {
							node.IsVisible = true;
							if (Metadata.IsPopularProperty(node.FirstProperty)) {
								Children.Add(node);							
							} else {
								MoreChildren.Add(node);
							}
						}
					}
				}
			}

			RaisePropertyChanged("HasChildren");
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion
	}
}
