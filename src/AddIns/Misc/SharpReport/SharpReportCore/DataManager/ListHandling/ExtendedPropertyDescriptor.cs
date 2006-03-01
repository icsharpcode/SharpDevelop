using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

namespace SharpReportCore {
	/// <summary>
	/// This class supports data binding
	/// </summary>
	public class SharpPropertyDescriptor : PropertyDescriptor{
	
		Type componentType;
		Type propertyType;
		PropertyInfo prop;

		public SharpPropertyDescriptor (string name, Type componentType, Type propertyType)
			: base (name, null)
		{
			this.componentType = componentType;
			this.propertyType = propertyType;
		}


		public override object GetValue (object component){			
			if (!componentType.IsAssignableFrom(component.GetType())){
				return null;
			}

			if (prop == null) {
				prop = componentType.GetProperty (Name);
			}
				
			object obj = prop.GetValue (component, null);
			if (obj != null) {
				if (obj is IList){
					PropertyTypeHash.Instance[componentType, Name] = SharpDataCollection<object>.GetElementType((IList)obj, componentType, Name);
				}
			} 
			return obj;
		}

		
		public override void SetValue(object component,	object value) {
			if (IsReadOnly){
				return;
			}
			if (prop == null){
				prop = componentType.GetProperty (Name);
			}
			prop.SetValue (component, value, null);
		}

		public override void ResetValue(object component) {
			return;
		}

		public override bool CanResetValue(object component) {
			return false;
		}

		public override bool ShouldSerializeValue(object component) {
			return false;
		}

		public override Type ComponentType{
			get { return componentType; }
		}

		public override bool IsReadOnly{
			get { return false; }
		}

		public override Type PropertyType{
			get { return propertyType; }
		}
	}
}

