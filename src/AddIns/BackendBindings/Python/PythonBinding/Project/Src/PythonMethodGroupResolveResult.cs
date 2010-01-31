// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonMethodGroupResolveResult : MethodGroupResolveResult
	{
		public PythonMethodGroupResolveResult(IClass containingClass, string methodName)
			: base(null, null, containingClass.DefaultReturnType, methodName)
		{
		}
	}
}
