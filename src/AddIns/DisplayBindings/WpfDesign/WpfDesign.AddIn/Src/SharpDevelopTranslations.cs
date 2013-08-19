// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using System;
using ICSharpCode.WpfDesign.Designer;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Description of SharpDevelopTranslations.
	/// </summary>
	public class SharpDevelopTranslations : Translations
	{
		public override string PressAltText {
			get { return StringParser.Parse("${res:AddIns.WpfDesign.AddIn.PressAltText}"); }
		}
		
		public static void Init() {
			Instance = new SharpDevelopTranslations();
		}
	}
}
