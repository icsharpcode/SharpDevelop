using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	public class PropertyNode : INotifyPropertyChanged
	{
		public PropertyNode(DesignItemProperty[] properties)
			: this(properties, null)
		{
		}

		PropertyNode(DesignItemProperty[] properties, PropertyNode parent)
		{
			this.Properties = properties;
			this.Parent = parent;
			this.Level = parent == null ? 0 : parent.Level + 1;

			foreach (var property in properties) {
				property.ValueChanged += new EventHandler(property_ValueChanged);
			}

			Editor = EditorManager.CreateEditor(FirstProperty);
			Children = new ObservableCollection<PropertyNode>();
			MoreChildren = new ObservableCollection<PropertyNode>();
			UpdateChildren();
		}

		void property_ValueChanged(object sender, EventArgs e)
		{
			if (raiseEvents) {
				OnValueChanged();
			}
		}

		static object Unset = new object();

		public DesignItemProperty[] Properties { get; private set; }
		bool raiseEvents = true;

		public string Name { get { return FirstProperty.Name; } }
		public bool IsEvent { get { return FirstProperty.IsEvent; } }
		public ServiceContainer Services { get { return FirstProperty.DesignItem.Services; } }
		public FrameworkElement Editor { get; private set; }
		public DesignItemProperty FirstProperty { get { return Properties[0]; } }

		public PropertyNode Parent { get; private set; }
		public int Level { get; private set; }
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
				return FirstProperty.ValueOnInstance;
			}
			set {
				SetValueCore(value);
			}
		}

		public string ValueString {
			get {
				if (Value == null) return null;
				return FirstProperty.TypeConverter.ConvertToString(Value);
			}
			set {
				Value = FirstProperty.TypeConverter.ConvertFromString(value);
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

			UpdateChildren();
		}

		void UpdateChildren()
		{
			Children.Clear();
			MoreChildren.Clear();

			if (Parent == null || Parent.IsExpanded) {
				if (IsAmbiguous || FirstProperty.IsCollection || FirstProperty.Value == null) {}
				else {
					var item = FirstProperty.Value;
					var list = TypeHelper.GetAvailableProperties(item.ComponentType)
						.OrderBy(d => d.Name)
						.Select(d => new PropertyNode(new[] { item.Properties[d.Name] }, this));

					foreach (var node in list) {
						if (Metadata.IsAdvanced(node.FirstProperty)) {
							MoreChildren.Add(node);
						} else {
							Children.Add(node);
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
