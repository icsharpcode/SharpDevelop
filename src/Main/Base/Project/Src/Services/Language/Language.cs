// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop
{
	public class Language
	{
		string name;
		string code;
		string imagePath;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Code {
			get {
				return code;
			}
		}
		
		public string ImagePath {
			get {
				return imagePath;
			}
		}		
		
		public Language(string name, string code, string imagePath)
		{
			this.name       = name;
			this.code       = code;
			this.imagePath  = imagePath;
		}
	}
}
