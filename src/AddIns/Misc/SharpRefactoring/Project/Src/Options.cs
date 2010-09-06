// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public static bool AddOtherMethod {
			get { return properties.Get("AddOtherMethod", true); }
			set { properties.Set("AddOtherMethod", value); }
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
