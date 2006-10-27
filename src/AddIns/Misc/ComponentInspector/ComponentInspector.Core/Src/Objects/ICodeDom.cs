// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace NoGoop.Obj
{
	internal interface ICodeDom
	{

		// Adds the code associated with this object to the
		// specified collection.  This is useful when the
		// object needs to be added to the collection
		// multiple times
		void AddDomTo(IList parent);
	}
}
