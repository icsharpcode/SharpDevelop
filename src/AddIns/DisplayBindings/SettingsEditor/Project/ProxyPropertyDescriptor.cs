/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 10/29/2006
 * Time: 8:30 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Reflection;

namespace ICSharpCode.SettingsEditor
{
	public class ProxyPropertyDescriptor : PropertyDescriptor
	{
		PropertyDescriptor baseDescriptor;
		
		public ProxyPropertyDescriptor(PropertyDescriptor baseDescriptor)
			: base(baseDescriptor)
		{
			this.baseDescriptor = baseDescriptor;
		}
		
		public override Type ComponentType {
			get {
				return baseDescriptor.ComponentType;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return baseDescriptor.IsReadOnly;
			}
		}
		
		public override bool CanResetValue(object component)
		{
			return baseDescriptor.CanResetValue(component);
		}
		
		public override object GetValue(object component)
		{
			return baseDescriptor.GetValue(component);
		}
		
		public override void ResetValue(object component)
		{
			baseDescriptor.ResetValue(component);
		}
		
		public override void SetValue(object component, object value)
		{
			baseDescriptor.SetValue(component, value);
		}
		
		public override bool ShouldSerializeValue(object component)
		{
			return baseDescriptor.ShouldSerializeValue(component);
		}
		
		public override Type PropertyType {
			get {
				return baseDescriptor.PropertyType;
			}
		}
	}
}
