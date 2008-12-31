using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpDevelop.XamlDesigner.PropertyGrid.Editors;
using SharpDevelop.XamlDesigner.Extensibility;
using SharpDevelop.XamlDesigner.Extensibility.Attributes;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	public class PropertyNode : ViewModel, IHasContext
	{
		internal PropertyNode(PropertyGridModel propertyGrid, MemberId member)
		{
			PropertyGrid = propertyGrid;
			MemberId = member;
			CreateEditor();
		}

		public PropertyGridModel PropertyGrid { get; private set; }
		public MemberId MemberId { get; private set; }
		public CategoryModel Category { get; internal set; }
		public FrameworkElement Editor { get; private set; }

		DesignProperty[] properties;

		public DesignContext Context
		{
			get { return PropertyGrid.Context; }
		}

		object value;

		public object Value
		{
			get { return value; }
			set { SetValue(value); }
		}

		string valueString;

		public string ValueString
		{
			get { return valueString; }
			set
			{
				if (MemberId.ValueSerializer != null) {
					if (MemberId.ValueSerializer.CanConvertFromString(value, null)) {
						var newValue = MemberId.ValueSerializer.ConvertFromString(value, null);
						SetValue(newValue);
					}
				}
				else {
					throw new Exception();
				}
			}
		}

		bool isVisible;

		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				isVisible = value;
				RaisePropertyChanged("IsVisible");
			}
		}

		bool isMixed;

		public bool IsMixed
		{
			get { return isMixed; }
			private set
			{
				if (isMixed != value) {
					isMixed = value;
					RaisePropertyChanged("IsMixed");
				}
			}
		}

		bool isSet;

		public bool IsSet
		{
			get { return isSet; }
			private set
			{
				if (isSet != value) {
					isSet = value;
					RaisePropertyChanged("IsSet");
				}
			}
		}

		void SetLocalValue(object value)
		{
			if (this.value != value) {
				this.value = value;
				RaisePropertyChanged("Value");
			}
		}

		void SetLocalValueString(string text)
		{
			if (valueString != text) {
				valueString = text;
				RaisePropertyChanged("ValueString");
			}
		}

		internal void BindValue()
		{
			if (PropertyGrid.Selection != null) {
				properties = PropertyGrid.Selection.Select(item => item.Property(MemberId)).ToArray();
				foreach (var p in properties) {
					p.PropertyChanged += OnePropertyChanged;
				}
				CalculateValue();
			}
		}

		internal void UnbindValue()
		{
			foreach (var p in properties) {
				p.PropertyChanged -= OnePropertyChanged;
			}
			properties = null;
		}

		void OnePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ValueObject") {
				CalculateValue();
			}
		}

		void CalculateValue()
		{
			var newIsSet = false;
			var newIsMixed = false;
			var firstValue = properties[0].ValueObject;

			foreach (var property in properties.Skip(1)) {
				if (!object.Equals(property.ValueObject, firstValue)) {
					newIsMixed = true;
					break;
				}
			}

			foreach (var property in properties) {
				if (property.IsSet) {
					newIsSet = true;
					break;
				}
			}

			IsSet = newIsSet;
			IsMixed = newIsMixed;

			if (IsMixed) {
				SetLocalValue(null);
				SetLocalValueString(null);
			}
			else {
				SetLocalValue(firstValue);

				if (MemberId.ValueSerializer != null) {
					if (value != null && MemberId.ValueSerializer.CanConvertToString(value, null)) {
						var text = MemberId.ValueSerializer.ConvertToString(value, null);
						SetLocalValueString(text);
					}
					else {
						SetLocalValueString(null);
					}
				}
			}			
		}

		void SetValue(object value)
		{
			if (this.value != value) {
				var opened = Context.UndoManager.OpenTransaction();
				foreach (var property in properties) {
					property.SetValue(value);
				}
				if (opened) {
					Context.UndoManager.CommitTransaction();
				}
			}
		}

		public void Reset()
		{
			var opened = Context.UndoManager.OpenTransaction();
			foreach (var property in properties) {
				property.Reset();
			}
			if (opened) {
				Context.UndoManager.CommitTransaction();
			}
		}

		void CreateEditor()
		{
			DataTemplate editorTemplate = null;

			var propertyAttribute = MetadataStore.GetAttributes<PropertyEditorAttribute>(MemberId).FirstOrDefault();
			if (propertyAttribute != null) {
				editorTemplate = propertyAttribute.EditorTemplate;
			}

			if (editorTemplate == null) {
				var typeAttribute = MetadataStore.GetAttributes<PropertyEditorAttribute>(MemberId.ValueType).FirstOrDefault();
				if (typeAttribute != null) {
					editorTemplate = typeAttribute.EditorTemplate;
				}
			}

			List<StandardValue> standardValues = null;

			if (editorTemplate == null) {
				if (MemberId.ValueType.IsEnum) {
					standardValues = GetEnumStandardValues(MemberId.ValueType).ToList();
				}
				else {
					foreach (var a in MetadataStore.GetAttributes<StandardValuesAttribute>(MemberId.ValueType)) {
						if (standardValues == null) {
							standardValues = new List<StandardValue>();
						}
						standardValues.AddRange(a.StandardValues);
					}
				}
				if (standardValues != null) {
					editorTemplate = EditorTemplates.ComboBoxEditor;
				}
			}

			if (editorTemplate == null) {
				if (MemberId.ValueSerializer != null) {
					editorTemplate = EditorTemplates.TextBoxEditor;
				}
			}

			if (editorTemplate == null) {
				editorTemplate = EditorTemplates.ObjectEditor;
			}

			Editor = editorTemplate.LoadContent() as FrameworkElement;
			Editor.DataContext = this;

			if (standardValues != null) {
				var comboBox = Editor as ComboBox;
				comboBox.ItemsSource = standardValues;
			}
		}

		static IEnumerable<StandardValue> GetEnumStandardValues(Type enumType)
		{
			return Enum.GetValues(enumType).Cast<object>()
				.Select(v => new StandardValue() { Instance = v, Text = Enum.GetName(enumType, v) });
		}
	}
}
