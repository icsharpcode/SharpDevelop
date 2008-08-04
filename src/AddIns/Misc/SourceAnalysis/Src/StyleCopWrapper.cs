// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matt Everson" email="ti.just.me@gmail.com"/>
//     <version>$Revision$</version>
// </file>
	
using System;
using System.IO;

using ICSharpCode.Core;

namespace MattEverson.SourceAnalysis
{
	public static class StyleCopWrapper
	{		
		public static bool IsStyleCopPath(string styleCopPath)
		{
			if (string.IsNullOrEmpty(styleCopPath))
				return false;
			else
				return File.Exists(Path.Combine(styleCopPath, "Microsoft.SourceAnalysis.dll"));
		}
		
		public static string FindStyleCopPath()
		{
			string styleCopPath = AnalysisIdeOptionsPanel.StyleCopPath;				
			if (IsStyleCopPath(styleCopPath)) {
				return styleCopPath;
			}
			return null;
		}
	}
}
