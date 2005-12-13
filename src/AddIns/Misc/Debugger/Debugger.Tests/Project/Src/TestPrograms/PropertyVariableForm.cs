// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace Debugger.Tests.TestPrograms
{
	public class PropertyVariableForm
	{
		public static void Main()
		{
			Form form = new Form();
			System.Diagnostics.Debugger.Break();
			System.Diagnostics.Debugger.Break();
		}
	}
}
