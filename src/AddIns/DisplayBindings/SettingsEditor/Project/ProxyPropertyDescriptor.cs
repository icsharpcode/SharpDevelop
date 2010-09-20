// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Reflection;

namespace ICSharpCode.SettingsEditor
{
	/// <summary>
	/// Forwards calls to the ProxyPropertyDescriptor to the base descriptor.
	/// </summary>
	public abstract class ProxyPropertyDescriptor : PropertyDescriptor
	{
		PropertyDescriptor baseDescriptor;
		
		protected ProxyPropertyDescriptor(PropertyDescriptor baseDescriptor)
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
