// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Media;

namespace PackageManagement.Tests.Helpers
{
	public static class ColorHelper
	{
		public static Color ConvertToColor(uint oleColor)
		{
			System.Drawing.Color color = System.Drawing.ColorTranslator.FromOle((int)oleColor);
			return (Color)ColorConverter.ConvertFromString(color.Name);
		}
		
		public static UInt32 ConvertToOleColor(System.Drawing.Color color)
		{
			return (UInt32)System.Drawing.ColorTranslator.ToOle(color);
		}
	}
}
