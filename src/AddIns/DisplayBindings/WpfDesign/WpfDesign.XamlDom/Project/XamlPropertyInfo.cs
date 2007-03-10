// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Represents a property assignable in XAML.
	/// This can be a normal .NET property or an attached property.
	/// </summary>
	internal abstract class XamlPropertyInfo
	{
		public abstract object GetValue(object instance);
		public abstract void SetValue(object instance, object value);
		public abstract void ResetValue(object instance);
		public abstract TypeConverter TypeConverter { get; }
		public abstract Type TargetType { get; }
		public abstract Type ReturnType { get; }
		public abstract string Name { get; }
		public abstract string FullyQualifiedName { get; }
		public abstract bool IsAttached { get; }
		public abstract bool IsCollection { get; }
		public virtual bool IsEvent { get { return false; } }
		public abstract string Category { get; }
	}
	
	#region XamlDependencyPropertyInfo
	internal class XamlDependencyPropertyInfo : XamlPropertyInfo
	{
		readonly DependencyProperty property;
		readonly bool isAttached;
		
		public XamlDependencyPropertyInfo(DependencyProperty property, bool isAttached)
		{
			Debug.Assert(property != null);
			this.property = property;
			this.isAttached = isAttached;
		}
		
		public override TypeConverter TypeConverter {
			get {
				return TypeDescriptor.GetConverter(this.ReturnType);
			}
		}
		
		public override string FullyQualifiedName {
			get {
				return this.TargetType.FullName + "." + this.Name;
			}
		}
		
		public override Type TargetType {
			get { return property.OwnerType; }
		}
		
		public override Type ReturnType {
			get { return property.PropertyType; }
		}
		
		public override string Name {
			get { return property.Name; }
		}
		
		public override string Category {
			get { return "Misc"; }
		}
		
		public override bool IsAttached {
			get { return isAttached; }
		}
		
		public override bool IsCollection {
			get { return false; }
		}
		
		public override object GetValue(object instance)
		{
			return ((DependencyObject)instance).GetValue(property);
		}
		
		public override void SetValue(object instance, object value)
		{
			((DependencyObject)instance).SetValue(property, value);
		}
		
		public override void ResetValue(object instance)
		{
			((DependencyObject)instance).ClearValue(property);
		}
	}
	#endregion

	#region XamlNormalPropertyInfo
	internal sealed class XamlNormalPropertyInfo : XamlPropertyInfo
	{
		PropertyDescriptor _propertyDescriptor;
		
		public XamlNormalPropertyInfo(PropertyDescriptor propertyDescriptor)
		{
			this._propertyDescriptor = propertyDescriptor;
		}
		
		public override object GetValue(object instance)
		{
			return _propertyDescriptor.GetValue(instance);
		}
		
		public override void SetValue(object instance, object value)
		{
			_propertyDescriptor.SetValue(instance, value);
		}
		
		public override void ResetValue(object instance)
		{
			_propertyDescriptor.ResetValue(instance);
		}
		
		public override Type ReturnType {
			get { return _propertyDescriptor.PropertyType; }
		}
		
		public override Type TargetType {
			get { return _propertyDescriptor.ComponentType; }
		}
		
		public override string Category {
			get { return _propertyDescriptor.Category; }
		}
		
		public override TypeConverter TypeConverter {
			get {
				if (_propertyDescriptor.PropertyType == typeof(object))
					return null;
				else
					return _propertyDescriptor.Converter;
			}
		}
		
		public override string FullyQualifiedName {
			get {
				return _propertyDescriptor.ComponentType.FullName + "." + _propertyDescriptor.Name;
			}
		}
		
		public override string Name {
			get { return _propertyDescriptor.Name; }
		}
		
		public override bool IsAttached {
			get { return false; }
		}
		
		public override bool IsCollection {
			get {
				return CollectionSupport.IsCollectionType(_propertyDescriptor.PropertyType);
			}
		}
	}
	#endregion
	
	#region XamlEventPropertyInfo
	sealed class XamlEventPropertyInfo : XamlPropertyInfo
	{
		readonly EventDescriptor _eventDescriptor;
		
		public XamlEventPropertyInfo(EventDescriptor eventDescriptor)
		{
			this._eventDescriptor = eventDescriptor;
		}
		
		public override object GetValue(object instance)
		{
			return null;
		}
		
		public override void SetValue(object instance, object value)
		{
			
		}
		
		public override void ResetValue(object instance)
		{
			
		}
		
		public override Type ReturnType {
			get { return _eventDescriptor.EventType; }
		}
		
		public override Type TargetType {
			get { return _eventDescriptor.ComponentType; }
		}
		
		public override string Category {
			get { return _eventDescriptor.Category; }
		}
		
		public override TypeConverter TypeConverter {
			get { return null; }
		}
		
		public override string FullyQualifiedName {
			get {
				return _eventDescriptor.ComponentType.FullName + "." + _eventDescriptor.Name;
			}
		}
		
		public override string Name {
			get { return _eventDescriptor.Name; }
		}
		
		public override bool IsEvent {
			get { return true; }
		}
		
		public override bool IsAttached {
			get { return false; }
		}
		
		public override bool IsCollection {
			get { return false; }
		}
	}
	#endregion
}
