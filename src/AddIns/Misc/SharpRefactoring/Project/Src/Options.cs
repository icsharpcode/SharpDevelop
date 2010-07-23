// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;

namespace SharpRefactoring
{
	/// <summary>
	/// Description of Options.
	/// </summary>
	public static class Options
	{
		static Properties properties;
		
		public static Properties Properties {
			get {
				Debug.Assert(properties != null);
				return properties;
			}
		}
		
		static Options()
		{
			properties = PropertyService.Get("SharpRefactoringOptions", new Properties());
		}
		
		public static bool AddIEquatableInterface {
			get { return properties.Get("AddIEquatableInterface", false); }
			set { properties.Set("AddIEquatableInterface", value); }
		}
		
		public static bool SurroundWithRegion {
			get { return properties.Get("SurroundWithRegion", true); }
			set { properties.Set("SurroundWithRegion", value); }
		}
		
		public static bool AddOperatorOverloads {
			get { return properties.Get("AddOperatorOverloads", true); }
			set { properties.Set("AddOperatorOverloads", value); }
		}
	}
}
