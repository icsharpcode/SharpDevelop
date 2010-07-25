// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;

namespace NoGoop.ObjBrowser
{

    // Identifies the combination of an object with a member type
	internal interface IObjectMember
	{
        Object Obj { get; }

        MemberInfo Member { get; }
	}

}
