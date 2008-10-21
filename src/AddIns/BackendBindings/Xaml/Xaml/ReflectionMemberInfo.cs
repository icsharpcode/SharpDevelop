using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Windows;

namespace ICSharpCode.Xaml
{
	public abstract class ReflectionMemberInfo
	{
		public abstract string Name { get; }
		public abstract Type ValueType { get; }
		public abstract Type OwnerType { get; }
		public abstract object GetValue(object instance);
		public abstract void SetValue(object instance, object value);
		public abstract void ResetValue(object instance);
		public abstract bool IsReadOnly { get; }
		public abstract PropertyDescriptor PropertyDescriptor { get; }
		protected abstract object EqualiltyCore { get; }

		public DependencyProperty DependencyProperty
		{
			get
			{
				var dpd = DependencyPropertyDescriptor.FromProperty(PropertyDescriptor);
				if (dpd != null) {
					return dpd.DependencyProperty;
				}
				return null;
			}
		}

		public override bool Equals(object obj)
		{
			var other = obj as ReflectionMemberInfo;
			if (other != null) {
				return EqualiltyCore == other.EqualiltyCore;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return EqualiltyCore.GetHashCode();
		}
	}

	class ReflectionPropertyInfo : ReflectionMemberInfo
	{
		public ReflectionPropertyInfo(PropertyDescriptor pd)
		{
			this.pd = pd;
		}

		PropertyDescriptor pd;

		public override string Name
		{
			get { return pd.Name; }
		}

		public override Type ValueType
		{
			get { return pd.PropertyType; }
		}

		public override Type OwnerType
		{
			get { return pd.ComponentType; }
		}

		public override object GetValue(object instance)
		{
			return pd.GetValue(instance);
		}

		public override void SetValue(object instance, object value)
		{
			pd.SetValue(instance, value);
		}

		public override void ResetValue(object instance)
		{
			if (pd.CanResetValue(instance)) {
				pd.ResetValue(instance);
			}
			else {
				pd.SetValue(instance, null);
			}
		}

		protected override object EqualiltyCore
		{
			get { return pd; }
		}

		public override bool IsReadOnly
		{
			get { return pd.IsReadOnly; }
		}

		public override PropertyDescriptor PropertyDescriptor
		{
			get { return pd; }
		}
	}

	class ReflectionAttachedPropertyInfo : ReflectionMemberInfo
	{
		public ReflectionAttachedPropertyInfo(MethodInfo getter, MethodInfo setter)
		{
			this.getter = getter;
			this.setter = setter;
		}

		MethodInfo getter;
		MethodInfo setter;

		public override string Name
		{
			get { return setter.Name.Substring(3); }
		}

		public override Type ValueType
		{
			get { return setter.GetParameters()[1].ParameterType; }
		}

		public override Type OwnerType
		{
			get { return setter.DeclaringType; }
		}

		public override object GetValue(object instance)
		{
			if (getter != null) {
				return getter.Invoke(instance, null);
			}
			throw new NotImplementedException();
		}

		public override void SetValue(object instance, object value)
		{
			setter.Invoke(instance, new[] { instance, value });
		}

		public override void ResetValue(object instance)
		{
			throw new NotImplementedException();
		}

		protected override object EqualiltyCore
		{
			get { return setter; }
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override PropertyDescriptor PropertyDescriptor
		{
			get { return null; }
		}
	}

	class DependencyPropertyInfo : ReflectionMemberInfo
	{
		public DependencyPropertyInfo(DependencyProperty dp)
		{
			this.dp = dp;
			if (typeof(DependencyObject).IsAssignableFrom(dp.OwnerType)) {
				this.dpd = DependencyPropertyDescriptor.FromProperty(dp, dp.OwnerType);
			}
		}

		DependencyProperty dp;
		DependencyPropertyDescriptor dpd;

		public override string Name
		{
			get { return dp.Name; }
		}

		public override Type ValueType
		{
			get { return dp.PropertyType; }
		}

		public override Type OwnerType
		{
			get { return dp.OwnerType; }
		}

		public override object GetValue(object instance)
		{
			var d = instance as DependencyObject;
			return d.GetValue(dp);
		}

		public override void SetValue(object instance, object value)
		{
			var d = instance as DependencyObject;
			d.SetValue(dp, value);
		}

		public override void ResetValue(object instance)
		{
			var d = instance as DependencyObject;
			d.ClearValue(dp);
		}

		protected override object EqualiltyCore
		{
			get { return dp; }
		}

		public override bool IsReadOnly
		{
			get { return dp.ReadOnly; }
		}

		public override PropertyDescriptor PropertyDescriptor
		{
			get { return dpd; }
		}
	}

	class ReflectionEventInfo : ReflectionMemberInfo
	{
		public ReflectionEventInfo(EventInfo eventInfo)
		{
			this.eventInfo = eventInfo;
		}

		EventInfo eventInfo;

		public override string Name
		{
			get { return eventInfo.Name; }
		}

		public override Type ValueType
		{
			get { return eventInfo.EventHandlerType; }
		}

		public override Type OwnerType
		{
			get { return eventInfo.DeclaringType; }
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

		protected override object EqualiltyCore
		{
			get { return eventInfo; }
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override PropertyDescriptor PropertyDescriptor
		{
			get { return null; }
		}
	}
}
