// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
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
		public abstract TypeConverter TypeConverter { get; }
		public abstract string FullyQualifiedName { get; }
		public abstract bool IsCollection { get; }
		internal abstract void AddValue(object collectionInstance, XamlPropertyValue newElement);
	}
	
	internal sealed class XamlAttachedPropertyInfo : XamlPropertyInfo
	{
		MethodInfo _getMethod;
		MethodInfo _setMethod;
		
		public XamlAttachedPropertyInfo(MethodInfo getMethod, MethodInfo setMethod)
		{
			this._getMethod = getMethod;
			this._setMethod = setMethod;
		}
		
		public override TypeConverter TypeConverter {
			get {
				return TypeDescriptor.GetConverter(_getMethod.ReturnType);
			}
		}
		
		public override string FullyQualifiedName {
			get {
				return _getMethod.DeclaringType.FullName + "." + _getMethod.Name;
			}
		}
		
		public override bool IsCollection {
			get {
				return false;
			}
		}
		
		public override object GetValue(object instance)
		{
			return _getMethod.Invoke(null, new object[] { instance });
		}
		
		public override void SetValue(object instance, object value)
		{
			_setMethod.Invoke(null, new object[] { instance, value });
		}
		
		internal override void AddValue(object collectionInstance, XamlPropertyValue newElement)
		{
			throw new NotSupportedException();
		}
	}
	
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
		
		public override bool IsCollection {
			get {
				return CollectionSupport.IsCollectionType(_propertyDescriptor.PropertyType);
			}
		}
		
		internal override void AddValue(object collectionInstance, XamlPropertyValue newElement)
		{
			_propertyDescriptor.PropertyType.InvokeMember(
				"Add", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
				null, collectionInstance,
				new object[] {
					newElement.GetValueFor(null)
				}, CultureInfo.InvariantCulture);
		}
	}
	
	static class CollectionSupport
	{
		public static bool IsCollectionType(Type type)
		{
			return typeof(IList).IsAssignableFrom(type)
				|| typeof(IDictionary).IsAssignableFrom(type)
				|| type.IsArray
				|| typeof(IAddChild).IsAssignableFrom(type);
		}
		
		//public static
	}
}
