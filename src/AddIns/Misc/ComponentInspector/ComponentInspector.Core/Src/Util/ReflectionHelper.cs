// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;

namespace NoGoop.Util
{
	internal class ReflectionHelper
	{

		internal const BindingFlags     ALL_BINDINGS = 
			BindingFlags.Instance
			| BindingFlags.Public
			| BindingFlags.NonPublic;

		internal const BindingFlags     ALL_STATIC_BINDINGS = 
			ALL_BINDINGS
			| BindingFlags.Static
			| BindingFlags.FlattenHierarchy;

		// Returns the type of a given member
		internal static Type GetType(MemberInfo mem)
		{
			switch (mem.MemberType)
			{
			case MemberTypes.Field:
				return ((FieldInfo)mem).FieldType;
			case MemberTypes.Property:
				return ((PropertyInfo)mem).PropertyType;
			case MemberTypes.Method:
				return ((MethodInfo)mem).ReturnType;
			default:
				return null;
			}
		}

		internal static String GetTypeName(Type type)
		{
			if (type.IsNestedAssembly ||
				type.IsNestedFamANDAssem ||
				type.IsNestedFamily ||
				type.IsNestedFamORAssem ||
				type.IsNestedPrivate ||
				type.IsNestedPublic)
				return type.DeclaringType.Name + "." + type.Name;
			
			return type.Name;
		}

		internal static bool TypeEqualsObject(Type type)
		{
			if (type.FullName.StartsWith("System.Object"))
				return true;
			return false;
		}

		internal static bool TypeEqualsMarshalByRef(Type type)
		{
			if (type.FullName.StartsWith("System.MarshalByRefObject"))
				return true;
			return false;
		}


		// Compares two members.  The .Equals method on the member
		// does not work since the ReflectedType for the members might
		// be different and we don't care about that.
		internal static bool IsMemberEqual(MemberInfo m1,
										   MemberInfo m2)
		{
			if (!m1.GetType().Equals(m1.GetType()))
				return false;
			if (!m1.DeclaringType.Equals(m2.DeclaringType))
				return false;
			if (!m1.Name.Equals(m2.Name))
				return false;

			// All but constructors and methods are equal at this point
			if (!(m1 is MethodBase))
				return true;

			// The method handles check that the methods are actually
			// the same
			if (((MethodBase)m1).MethodHandle.
				Equals(((MethodBase)m2).MethodHandle))
				return true;
			return false;
		}

		internal static Type GetType(String typeName)
		{
			Type t = null;

			foreach (Assembly assembly in 
					 AppDomain.CurrentDomain.GetAssemblies())
			{
				t = assembly.GetType(typeName);
				if (t != null)
					break;
			}
			return t;
		}

		internal static bool DoesTypeHaveKids(Type type)
		{
			if (type == null)
				return false;
			if (! type.Equals(typeof(void))
				&& ! type.IsEnum
				&& ! type.IsPrimitive
				&& ! type.Equals(typeof(String)))
				return true;
			return false;
		}

		internal static bool IsStruct(Type type)
		{
			if (type == null)
				return false;
			if (type.IsValueType &&
				DoesTypeHaveKids(type))
				return true;
			return false;
		}


		// Finds the property with the specified name in the given
		// type
		internal static PropertyInfo FindPropByName(Type type,
													String propName)
		{
			PropertyInfo nameProp = null;

			Type baseType = type;
			while (baseType != null)
			{
				nameProp = baseType.
					GetProperty(propName, ReflectionHelper.ALL_BINDINGS);
				if (nameProp != null)
					return nameProp;
				baseType = baseType.BaseType;
			}

			if (nameProp == null)
			{
				Array interfaces = type.GetInterfaces();
				foreach (Type ifType in interfaces)
					{
						nameProp = ifType.
							GetProperty(propName, 
										ReflectionHelper.ALL_BINDINGS);
						if (nameProp != null)
							return nameProp;
					}
			}
			return nameProp;
		}
	}
}




