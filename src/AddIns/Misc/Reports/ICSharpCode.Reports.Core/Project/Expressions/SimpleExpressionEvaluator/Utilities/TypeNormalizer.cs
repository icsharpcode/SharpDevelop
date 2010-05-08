#region Copyright note and modification history
/*
-----------------------------------------------------------------------------
Copyright (c) 2006 HSI Inc. All Rights Reserved.

PROPRIETARY NOTICE: This software has been provided pursuant to a
License Agreement that contains restrictions on its use. This software
contains valuable trade secrets and proprietary information of
HSI Inc and is protected by Federal copyright law. It may not
be copied or distributed in any form or medium, disclosed to third parties,
or used in any manner that is not provided for in said License Agreement,
except with the prior written authorization of HSI Inc.

-----------------------------------------------------------------------------
$Log: /CoreServices/Hsi/ExpressionEvaluator/TypeNormalizer.cs $
 * 
 * 4     9/27/07 9:20a Nathan_stults
 * 
 * 3     9/26/06 4:16p Nathan_stults
 * 
 * 2     8/23/06 8:52a Alex_wang
 * MOdified NormalizedTypes() method to throw exception if conversion
 * fails.
-----------------------------------------------------------------------------
*/
#endregion

using System;
using System.ComponentModel;

namespace SimpleExpressionEvaluator.Utilities
{
    /// <summary>
    /// Summary description for TypeNormalizer.
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

            /*
            TypeConverter converter = TypeDescriptor.GetConverter(targetType);
            if (converter != null && converter.CanConvertFrom(value.GetType()))
            {
            	return converter.ConvertFrom(value);
            }
            */
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch
            { }
            return nullValue;
        }
    }
}