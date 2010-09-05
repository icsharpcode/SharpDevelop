// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Utils
{
	public class PythonCodeDomSerializerHelper
	{
		public static PythonCodeDomSerializer CreateSerializer()
		{
			return new PythonCodeDomSerializer();
		}
	}
}
