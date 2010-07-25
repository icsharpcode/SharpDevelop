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
	// Used to keep track of the property that contains the
	// name of an object for a given type
	internal class NamePropCache
	{
		internal static Hashtable           _classes;
		protected const String EMPTY = "Empty";

		static NamePropCache()
		{
			_classes = new Hashtable();
		}

		internal static PropertyInfo GetNameProp(Type type)
		{
			Object retValue;
			lock (_classes)
			{
				retValue = _classes[type];
				if (retValue == null)
				{
					retValue = FindNameProp(type);
					if (retValue == null)
						_classes.Add(type, EMPTY);
					else
						_classes.Add(type, retValue);

				}
				TraceUtil.WriteLineInfo(typeof(NamePropCache),
										"NamePropCache: " + type + " "
										+ retValue);
				if (retValue == null || retValue.Equals(EMPTY))
					return null;
				return (PropertyInfo)retValue;
			}
		}

		protected static PropertyInfo FindNameProp(Type type)
		{
			PropertyInfo propInfo;

			// FIXME - keep all of the properties that are acutally
			// found in an array and see which ones have values at runtime

			// FIXME - parameterize these
			propInfo = ReflectionHelper.FindPropByName(type, "nameProp");
			if (propInfo != null)
				return propInfo;
			propInfo = ReflectionHelper.FindPropByName(type, "nodeName");
			if (propInfo != null)
				return propInfo;
			propInfo = ReflectionHelper.FindPropByName(type, "Name");
			if (propInfo != null)
				return propInfo;
			propInfo = ReflectionHelper.FindPropByName(type, "name");
			if (propInfo != null)
				return propInfo;
			return null;
		}										
	}
}
