// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace NoGoop.ObjBrowser
{
	// Used to assign numbers to components by class, so that each
	// class has its own set of numbers.  The first button gets 1, the
	// 2nd 2 and so on.
	internal class CompNumber
	{
		protected static Hashtable _typeHash = new Hashtable();
		internal static String GetCompName(Type type)
		{
			object numObj = _typeHash[type];
			int num = 1;
			if (numObj != null)
			{
				num = (int)numObj;
				num++;
				_typeHash.Remove(type);
			}
			// Implicitly boxed when added to hash
			_typeHash.Add(type, num);
			return type.Name + num;
		}
	}
}
