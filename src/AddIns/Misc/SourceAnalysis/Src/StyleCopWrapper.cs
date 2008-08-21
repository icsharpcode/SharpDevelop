// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matt Everson" email="ti.just.me@gmail.com"/>
//     <version>$Revision$</version>
// </file>
	
using System;
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace MattEverson.SourceAnalysis
{
	public static class StyleCopWrapper
	{		
	    public static readonly string MasterSettingsFileName = Path.Combine(PropertyService.ConfigDirectory, @"Settings.SourceAnalysis");
	    
		public static bool IsStyleCopPath(string styleCopPath)
		{
			if (string.IsNullOrEmpty(styleCopPath))
				return false;
			else
				return File.Exists(Path.Combine(styleCopPath, "Microsoft.StyleCop.dll"));
		}
		
		public static string FindStyleCopPath()
		{
			string styleCopPath = AnalysisIdeOptionsPanel.StyleCopPath;				
			if (IsStyleCopPath(styleCopPath)) {
				return styleCopPath;
			}
			return null;
		}
		
		public static string GetMasterSettingsFile()
		{
		    var resource = typeof(StyleCopWrapper).Assembly.GetManifestResourceStream("MattEverson.SourceAnalysis.Resources.Settings.SourceAnalysis");
		    if(!File.Exists(MasterSettingsFileName))
		    {
		        var xmlDoc = new XmlDocument();
		        xmlDoc.Load(resource);
		        xmlDoc.PreserveWhitespace = true;
		        xmlDoc.Save(MasterSettingsFileName);
		    }
		    return MasterSettingsFileName;
		}
	}
}
