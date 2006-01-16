using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

namespace SharpReportCore {
	/// <summary>
	/// This class supports data binding
	/// </summary>
	public class SharpPropertyDescriptor : PropertyDescriptor{
		bool readOnly = false;
		Type componentType;
		Type propertyType;
		PropertyInfo prop;

		public SharpPropertyDescriptor (string name, Type componentType, Type propertyType)
			: base (name, null)
		{
			this.componentType = componentType;
			this.propertyType = propertyType;
		}


		public override object GetValue (object component)
		{			
			if (!componentType.IsAssignableFrom(component.GetType()))
			{
				return null;
			}

			if (prop == null)
				prop = componentType.GetProperty (Name);
			object o = prop.GetValue (component, null);
			if (o is IList)
			{
				PropertyTypeHash.Instance[componentType, Name] = SharpArrayList.GetElementType((IList)o, componentType, Name);
			}
			return o;
		}

		public override void SetValue(object component,	object value) 
		{
			if (IsReadOnly)
				return;

			if (prop == null)
				prop = componentType.GetProperty (Name);

			prop.SetValue (component, value, null);
		}

		public override void ResetValue(object component) 
		{
			return;
		}

		public override bool CanResetValue(object component) 
		{
			return false;
		}

		public override bool ShouldSerializeValue(object component) 
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return componentType; }
		}

		public override bool IsReadOnly
		{
			get { return readOnly; }
		}

		public override Type PropertyType
		{
			get { return propertyType; }
		}
	}
}

