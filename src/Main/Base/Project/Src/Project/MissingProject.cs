// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	public class MissingProject : AbstractProject
	{
		string typeGuid = "{00000000-0000-0000-0000-000000000000}";
		
		public override string TypeGuid {
			get {
				return typeGuid;
			}
			set {
				typeGuid = value;
			}
		}
		
		public MissingProject(string fileName)
		{
			Name     = Path.GetFileNameWithoutExtension(fileName);
			FileName = fileName;
		}
	}
}
