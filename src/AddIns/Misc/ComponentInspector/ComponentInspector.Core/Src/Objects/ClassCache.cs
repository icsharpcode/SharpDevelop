// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;

using NoGoop.Util;

namespace NoGoop.Obj
{
	// Used to keep track of the members associated with a class
	internal class ClassCache
	{
		internal static Hashtable           _classes;

		static ClassCache()
		{
			_classes = new Hashtable();
		}
			
		internal static ArrayList GetMembers(Type type)
		{
			ArrayList retValue;
			lock (_classes)
			{
				retValue = (ArrayList)_classes[type];
				if (retValue == null)
				{
					retValue = GetMembersInternal(type);
					_classes.Add(type, retValue);
				}
				TraceUtil.WriteLineInfo(typeof(ClassCache),
										"Class cache: " + type);
				return retValue;
			}
		}


		protected static ArrayList GetMembersInternal(Type type)
		{

			ArrayList members = new ArrayList();

			Array localMembers = type.
				GetMembers(ReflectionHelper.ALL_STATIC_BINDINGS);
			members.AddRange(localMembers);

			if (type.IsInterface)
			{
				// Get the members in the inherited interfaces
				Array interfaces = type.GetInterfaces();
				foreach (Type ifType in interfaces)
				{
					Array ifMembers = ifType.
						GetMembers(ReflectionHelper.ALL_BINDINGS);
					members.AddRange(ifMembers);
				}
			}
			else
			{
				// Get all of the members of the base types
				Type baseType = type.BaseType;
				while (baseType != null)
				{
					Array baseMembers = baseType.
						GetMembers(ReflectionHelper.ALL_BINDINGS);
					/***
					foreach (MemberInfo member in baseMembers)
					{
						if (member is MethodInfo)
						{
							// Skip the override methods
							if ((MethodAttributes.ReuseSlot &
								 ((MethodInfo)member).Attributes) != 0)
								continue;
							members.Add(member);
						}
						else
						{
							members.Add(member);
						}
					}
					*****/
					members.AddRange(baseMembers);
					baseType = baseType.BaseType;
				}
			}

			members.Sort(new ObjectInfo.MemberCompare());

			// Now get rid of the duplicates - by name
			ArrayList outMembers = new ArrayList();
			Object prevMember = null;
			foreach (Object member in members)
			{
				if (prevMember != null &&
					member.ToString().Equals(prevMember.ToString()))
					continue;
				outMembers.Add(member);
				prevMember = member;
			}
			
			// This needs to be better parameterized and moved out of this
			// class, but its late in the release, lets do that next time
			// around.

			// See if this has any potential properties to deny
			// auto invocation

			String[] props;
			props = new String[] { "Cells", "Formula" };
			CheckAutoInvoke(type, outMembers, "_Worksheet", props);
			props = new String[] { "Cells", "FormulaLocal" };
			CheckAutoInvoke(type, outMembers, "_Worksheet", props);
			props = new String[] { "Cells", "FormulaR1C1" };
			CheckAutoInvoke(type, outMembers, "_Worksheet", props);
			props = new String[] { "Cells", "FormulaR1C1Local" };
			CheckAutoInvoke(type, outMembers, "_Worksheet", props);
			props = new String[] { "Cells", "Value" };
			CheckAutoInvoke(type, outMembers, "_Worksheet", props);
			props = new String[] { "Cells", "Value2" };
			CheckAutoInvoke(type, outMembers, "_Worksheet", props);

			return outMembers;
		}

		// Note: the Object[] is 
		// the parent members of the member which need to match
		// in order to deny auto invoke.  The first element is
		// the member info, the next is
		// its parent and so on.  The last element is a type.

		// K(MemberInfo of property) V(Object[] - members/type)
		protected static Hashtable          _autoInvokeHash 
			= Hashtable.Synchronized(new Hashtable());

		protected static void CheckAutoInvoke(Type type,
											  ArrayList outMembers,
											  String startType,
											  String[] props)
		{

			// The members + the root type
			MemberInfo[] membersType = new MemberInfo[props.Length + 1];
			
			if (type.Name.Equals(startType))
			{
				MemberInfo member = 
					LoadAutoInvokeTypes(outMembers, membersType, props, 0);

				if (member == null)
					return;

				// Last entry in this array is the type
				membersType[membersType.Length - 1] = type;

				// Synchronization not necessary because this is already
				// under the classes lock
				_autoInvokeHash.Add(member, membersType);

				TraceUtil.WriteLineWarning
					(typeof(ClassCache),
					 "ClassCache - found match for " + startType);
				foreach (Object obj in membersType)
					TraceUtil.WriteLineWarning(typeof(ClassCache), "  " + obj);
					
			}

		}


		// Returns the member to be used as the key to the hash, this
		// is the member of the lowest property.  Null is returned if
		// this set of members is not a match for the given properties.
		// Also populates the membersType array with the members to be used 
		// in the comparison.
		protected static MemberInfo LoadAutoInvokeTypes
			(ArrayList members,
			 MemberInfo[] membersType,
			 String[] props,
			 int index)
		{
			foreach (Object member in members)
			{
				if (!(member is PropertyInfo))
					continue;
				PropertyInfo prop = (PropertyInfo)member;
				if (prop.Name.Equals(props[index]))
				{
					// Populate this from the top
					membersType[membersType.Length - 2 - index] = prop;
					members = GetMembers(prop.PropertyType);
					if (index + 1 < props.Length)
					{
						return LoadAutoInvokeTypes(members, membersType,
												   props, index + 1);
					}
					return prop;
				}
			}
			return null;
		}

		// Returns true if this property should be auto invoked
		internal static bool ShouldAutoInvokeProp(IObjectNode node)
		{
			MemberInfo[] membersType = 
				(MemberInfo[])_autoInvokeHash[node.ObjectInfo.ObjMemberInfo];

			// Not found, so its ok to auto invoke
			if (membersType == null)
				return true;
			
			for (int i = 0; i < membersType.Length; i++)
			{
				if (node == null)
					return true;

				if (i == membersType.Length - 1)
				{
					// Found a match, don't invoke the property
					if (((Type)membersType[i]).IsAssignableFrom(node.ObjType))
						return false;
				}
					
				if (!ReflectionHelper.IsMemberEqual
					(node.ObjectInfo.ObjMemberInfo,
					 membersType[i]))
					return true;
				node = node.ParentObjectNode;
			}
			return true;
		}								
	}
}
