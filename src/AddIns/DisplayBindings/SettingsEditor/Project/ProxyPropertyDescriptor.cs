// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
