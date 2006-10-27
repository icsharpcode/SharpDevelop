// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//	 <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Reflection;

using NoGoop.Util;

namespace NoGoop.Obj
{
	internal class ObjectInfoFactory
	{
		// Gets the appropriate type of object info
		internal static ObjectInfo GetObjectInfo(bool comObject)
		{
			return new ComObjectInfo();
		}

		// Gets the appropriate type of object info
		internal static ObjectInfo GetObjectInfo(bool comObject, MemberInfo m, Type parentType)
		{
			return new ComObjectInfo(m, parentType);
		}

		// Gets the appropriate type of object info
		internal static ObjectInfo GetObjectInfo(bool comObject, Object obj)
		{
			ObjectInfo objInfo = ObjectInfo.GetObjectInfo(obj);
			if (objInfo != null) {
				return objInfo;
			}

			if (TraceUtil.If(typeof(ObjectInfoFactory), TraceLevel.Verbose)) {
				TraceUtil.WriteLineVerbose(null, "getObjInfo: " 
										   + obj + " type: " 
										   + obj.GetType().FullName);
			}
			return new ComObjectInfo(obj);
		}
	}
}
