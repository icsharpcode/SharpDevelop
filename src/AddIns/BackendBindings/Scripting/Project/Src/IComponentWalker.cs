// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;

namespace ICSharpCode.Scripting
{
	public interface IComponentWalker
	{
		IComponent CreateComponent(string code);
	}
}
