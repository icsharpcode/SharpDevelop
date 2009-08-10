// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.SharpDevelop;
using System;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// Gets resources the way suitable for Debugger.AddIn.
	/// </summary>
	public static class DebuggerResourceService
	{
		/// <summary>
		/// Gets image with given name from resources.
		/// </summary>
		/// <param name="resourceName">Resource name of the image.</param>
		public static IImage GetImage(string resourceName)
		{
			return new ResourceServiceImage(resourceName);
		}
	}
}
