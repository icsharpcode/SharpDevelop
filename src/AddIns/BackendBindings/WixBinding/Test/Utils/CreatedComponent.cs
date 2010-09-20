// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Helper class that stores the component name and type.
	/// </summary>
	public class CreatedComponent 
	{
		string typeName = String.Empty;
		string name = String.Empty;
		
		public CreatedComponent(string typeName, string name)
		{
			this.typeName = typeName;
			this.name = name;
		}
		
		public string TypeName {
			get {
				return typeName;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
	}
}
