// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
