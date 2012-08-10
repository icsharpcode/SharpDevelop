// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SourceAnalysis
{
	public static class StyleCopWrapper
	{		
	    public static readonly string MasterSettingsFileName = Path.Combine(PropertyService.ConfigDirectory, @"Settings.SourceAnalysis");
	    public const string STYLE_COP_FILE = "StyleCop.dll";
	    
		public static bool IsStyleCopPath(string styleCopPath)
		{
			if (string.IsNullOrEmpty(styleCopPath))
				return false;
			
			return File.Exists(styleCopPath);
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
		    var resource = typeof(StyleCopWrapper).Assembly.GetManifestResourceStream("ICSharpCode.SourceAnalysis.Resources.Settings.SourceAnalysis");
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
