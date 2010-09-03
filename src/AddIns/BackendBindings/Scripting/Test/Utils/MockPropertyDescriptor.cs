// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.Scripting.Tests.Utils
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
