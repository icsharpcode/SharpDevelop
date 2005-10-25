// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpAssembly.Assembly
{
	/// <summary>
	/// Is thrown when the given assembly name could not be found.
	/// </summary>
	public class AssemblyNameNotFoundException : Exception
	{
		public AssemblyNameNotFoundException(string name) : base("Could not find assembly named " + name + " in the Global Assembly Cache.")
		{
		}
	}
}
