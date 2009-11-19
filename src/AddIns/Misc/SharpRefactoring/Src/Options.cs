// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
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
		
		public static bool AddInterface {
			get { return properties.Get("AddInterface", false); }
			set { properties.Set("AddInterface", value); }
		}
	}
}
