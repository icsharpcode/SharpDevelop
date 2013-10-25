// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;

namespace ICSharpCode.Reporting.Expressions
{
	/// <summary>
	/// Description of TypeNormalizer.
	/// </summary>
	public class TypeNormalizer
	{
		
		public static void NormalizeTypes(ref object left,ref object right)
		{
			NormalizeTypes(ref left,ref right,0);
		}

		public static void NormalizeTypes(ref object left,ref object right,object nullValue)
		{
			if (left == null)
				left = 0;
			if (right == null)
				right = 0;

			if (left.GetType() == right.GetType())
				return;

			try
			{
				right = Convert.ChangeType(right,left.GetType());
			}
			catch
			{
				try
				{
					left = Convert.ChangeType(left, right.GetType());
				}
				catch
				{
					throw new Exception(String.Format("Error converting from {0} type to {1}", left.GetType().FullName, right.GetType().FullName));
				}
			}
		}
		
		public static void EnsureTypes(ref object[] values,Type targetType)
		{
			object nullValue = null;
			if (targetType.IsValueType)
				nullValue = Activator.CreateInstance(targetType);
			EnsureTypes(ref values,targetType,nullValue);
			
		}

		public static void EnsureTypes(ref object[] values,Type targetType,object nullValue)
		{
			for (int i=0;i<values.Length;i++)
			{
				values[i] = EnsureType(values[i],targetType,nullValue);
			}
		}

		public static T EnsureType<T>(object value)
		{
			return EnsureType<T>(value, default(T));
		}

		public static T EnsureType<T>(object value,object nullValue)
		{
			return (T) EnsureType(value, typeof (T), nullValue);
		}

		public static object EnsureType(object value, Type targetType)
		{
			if (value != null && value.GetType() == targetType)
				return value;

			object defaultValue = null;
			if (targetType.IsValueType)
				defaultValue = Activator.CreateInstance(targetType);
			return EnsureType(value, targetType, defaultValue);
		}

		public static object EnsureType(object value,Type targetType,object nullValue)
		{
			if (value == null)
				return nullValue;
			
			if (targetType == typeof(object))
				return value;

			if (value.GetType() == targetType)
				return value;

			try {
				return Convert.ChangeType(value, targetType);
			} catch (Exception e) {
				
				Console.WriteLine("TypeNormalizer {0} - {1}",value.ToString(),e.Message);
				return value.ToString();
				//throw new Exception()String.Format("TypeNormalizer for  <{0}> - {1}",value.ToString(),e.Message));
			}
		}
		
	}
}

