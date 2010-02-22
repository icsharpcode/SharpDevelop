// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets
{
	public static class DesignHelper
	{
		static int shadowStatus;
		
		/// <summary>
		/// Adds a shadow to the create params if it is supported by the operating system.
		/// </summary>
		public static void AddShadowToWindow(CreateParams createParams)
		{
			if (shadowStatus == 0) {
				// Test OS version
				shadowStatus = -1; // shadow not supported
				if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
					Version ver = Environment.OSVersion.Version;
					if (ver.Major > 5 || ver.Major == 5 && ver.Minor >= 1) {
						shadowStatus = 1;
					}
				}
			}
			if (shadowStatus == 1) {
				createParams.ClassStyle |= 0x00020000; // set CS_DROPSHADOW
			}
		}
	}
}
