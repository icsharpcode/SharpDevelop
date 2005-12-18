// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public interface INewFileCreator
	{
		bool IsFilenameAvailable(string fileName);
		
		void SaveFile(string filename, string content, string languageName, bool showFile);
	}
}
