// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class LocalizedProperty : PropertyDescriptor
	{
		string category;
		string description;
		string name;
		string type;
		string localizedName;
		
		TypeConverter typeConverterObject = null;
		object        defaultValue        = null;
		
		public TypeConverter TypeConverterObject {
			get {
				return typeConverterObject;
			}
			set {
				typeConverterObject = value;
			}
		}
		
		public object DefaultValue {
			get {
				return defaultValue;
			}
			set {
				defaultValue = value;
			}
		}
		
		public string LocalizedName {
			get {
				if (localizedName == null) {
					return null;
				}
				return StringParser.Parse(localizedName);
			}
			set {
				localizedName = value;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		#region PropertyDescriptor 
		public override string DisplayName {
			get  {
				if (localizedName != null && localizedName.Length > 0) {
					return LocalizedName;
				}
				return Name;
			}
		}
		
		public override string Category {
			get {
				return StringParser.Parse(category);
			}
		}
		
		public override string Description {
			get {
				return StringParser.Parse(description);
			}
		}
		
		public override Type PropertyType {
			get {
				return Type.GetType(this.type); 
			}
		}
		
		public override Type ComponentType {
			get {
				return Type.GetType(this.type); 
			}
		}
		
		public override TypeConverter Converter {
			get {
				if (typeConverterObject != null) {
					return typeConverterObject;
				}
				return base.Converter;
			}
		}
		
		public override object GetValue(object component)
		{
			string propertyValue = StringParser.Properties["Properties." + Name];
			
			if (typeConverterObject is BooleanTypeConverter) {
				return Boolean.Parse(propertyValue); 
			}
			return propertyValue;
		}
		
		public override void SetValue(object component, object val)
		{
			if (typeConverterObject != null) {
				StringParser.Properties["Properties." + Name] = typeConverterObject.ConvertFrom(val).ToString();
			} else {
				StringParser.Properties["Properties." + Name] = val.ToString();
			}
		}
		
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
		
		public override bool CanResetValue(object component)
		{
			return defaultValue != null;
		}
		
		public override void ResetValue(object component)
		{
			SetValue(component, defaultValue);
		}
		#endregion
		
		public LocalizedProperty(string name, string type, string category, string description) : base(name, null)
		{
			this.category = category;
			this.description = description;
			this.name = name;
			this.type = type;
		}
	}
}
