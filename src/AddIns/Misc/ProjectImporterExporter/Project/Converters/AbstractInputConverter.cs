// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	public abstract class AbstractInputConverter
	{
		public abstract string FormatName {
			get;
		}
		public abstract string OutputFile { 
			get;
		}
		
		public abstract bool CanConvert(string fileName);
		
		public abstract bool Convert(string inputFile, string outputPath);
	}
}
