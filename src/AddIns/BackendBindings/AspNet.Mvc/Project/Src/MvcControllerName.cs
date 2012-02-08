// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcControllerName
	{
		string name = String.Empty;
		string rootName = String.Empty;
		
		public string Name {
			get { return name; }
			set {
				name = UseEmptyStringIfNull(value);
				UpdateRootName();
			}
		}
		
		string UseEmptyStringIfNull(string value)
		{
			if (value != null) {
				return value;
			}
			return String.Empty;
		}
		
		public string RootName {
			get { return rootName; }
			set { rootName = UseEmptyStringIfNull(value); }
		}
		
		void UpdateRootName()
		{
			rootName = GetRootName();
		}
		
		string GetRootName()
		{
			int index = name.IndexOf("Controller", StringComparison.InvariantCultureIgnoreCase);
			if (index > 0) {
				return name.Substring(0, index);
			}
			return name;
		}
	}
}
