// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;
namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	public abstract class AbstractOutputConverter
	{
		public abstract string FormatName {
			get;
		}
		
		public abstract void ConvertCombine(IProgressMonitor progressMonitor, string inputCombine, string outputPath);
		
		public abstract void ConvertProject(IProgressMonitor progressMonitor, string inputProject, string outputPath);
	}
}
