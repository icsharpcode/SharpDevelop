// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Security;
using System.Security.Permissions;

using System.Windows.Forms;

using MSjogren.GacTool.FusionNative;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.ProjectImportExporter.Commands;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	public class SolutionOutputConverterTool
	{
		public ArrayList copiedFiles    = new ArrayList();
		public ArrayList configurations = new ArrayList();
		public Hashtable projectGUIDHash;
		public Hashtable projectTypeGUIDHash;
		
		public string AddConfig(string config)
		{
			configurations.Add(config);
			return config;
		}
		
		public string VerifyFileLocation(string itemFile)
		{
			if (itemFile.Length == 0) {
				return String.Empty;
			}
			copiedFiles.Add(itemFile);
			if (itemFile.StartsWith(@".\")) {
				itemFile = itemFile.Substring(2);
			}
			return itemFile;
		}
		
		public string ConvertBuildAction(string buildAction)
		{
			switch (buildAction) {
				case "EmbedAsResource":
					return "EmbeddedResource";
			}
			return buildAction;
		}
		
		public string FileNameWithoutExtension(string txt)
		{
			return Path.GetFileNameWithoutExtension(txt);
		}
		
		public string Negate(string txt)
		{
			if (txt.ToUpper() == "TRUE") {
				return false.ToString();
			}
			return true.ToString();
		}
		
		public string GetProjectGUID(string projectFileName)
		{
			string result = (string)projectGUIDHash[projectFileName];
			if (result == null) {
				result = String.Concat('{', Guid.NewGuid().ToString().ToUpper(), '}');
				projectGUIDHash[projectFileName] = result;
			}
			return result;
		}
		
		public string GetPackageGUID(string projectFileName)
		{
			string result = (string)projectTypeGUIDHash[projectFileName];
			if (result == null) {
				result = String.Concat('{', Guid.NewGuid().ToString().ToUpper(), '}');
				projectTypeGUIDHash[projectFileName] = result;
			}
			return result;
		}
	}
}
