// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace ICSharpCode.Core
{
	public class Language
	{
		string name;
		string code;
		int    imageIndex;
		
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
		
		public int ImageIndex {
			get {
				return imageIndex;
			}
		}
		
		public Language(string name, string code, int imageIndex)
		{
			this.name       = name;
			this.code       = code;
			this.imageIndex = imageIndex;
		}
	}
}
