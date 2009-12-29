// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RubyBinding.Tests.Utils
{
	public class MockPropertyDescriptor : PropertyDescriptor
	{
		object propertyValue;
		bool shouldSerializeValue;
		object setValueComponent;
		
		public MockPropertyDescriptor(string name, object value, bool shouldSerializeValue)
			: base(name, null)
		{
			this.propertyValue = value;
			this.shouldSerializeValue = shouldSerializeValue;
		}
		
		public override Type ComponentType {
			get { return typeof(Form); }
		}
		
		public override bool IsReadOnly {
			get { return false; }
		}
		
		public override Type PropertyType {
			get { return typeof(String); }
		}
		
		public override bool CanResetValue(object component)
		{
			return true;
		}
		
		public override object GetValue(object component)
		{
			return this.propertyValue;
		}
		
		public override void ResetValue(object component)
		{
		}
		
		public override void SetValue(object component, object value)
		{
			setValueComponent = component;
			propertyValue = value;
		}
		
		public object GetSetValueComponent()
		{
			return setValueComponent;
		}
		
		public override bool ShouldSerializeValue(object component)
		{
			return shouldSerializeValue;
		}		
	}
}
