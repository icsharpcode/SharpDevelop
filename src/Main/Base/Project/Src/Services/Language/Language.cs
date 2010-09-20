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
		bool isRightToLeft;
		
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
		
		public bool IsRightToLeft {
			get { return isRightToLeft; }
		}
		
		public Language(string name, string code, string imagePath, bool isRightToLeft)
		{
			this.name       = name;
			this.code       = code;
			this.imagePath  = imagePath;
			this.isRightToLeft = isRightToLeft;
		}
	}
}
