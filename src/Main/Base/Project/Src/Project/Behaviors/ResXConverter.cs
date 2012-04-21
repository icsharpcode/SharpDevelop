// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class ResXConverter
	{
		public static string ConvertTypeName(Type type, string fileName, CompilableProject project = null)
		{
			Version version;
			if (project == null)
				version = GetTargetFrameworkVersionFrom(fileName);
			else
				version = ScanVersion(project.TargetFrameworkVersion);
			string name = type.AssemblyQualifiedName;
			if (type.Assembly.GlobalAssemblyCache && IsFrameworkAssembly(type.Assembly.GetName().GetPublicKeyToken()))
				name = type.AssemblyQualifiedName.Replace(", Version=4.0.0.0,", ", Version=" + PrintVersion(version) + ",");
			return name;
		}
		
		static readonly Version DotNet40 = new Version(4, 0);
		static readonly Version DotNet20 = new Version(2, 0);
		
		static Version GetTargetFrameworkVersionFrom(string fileName)
		{
			if (ProjectService.OpenSolution == null)
				return DotNet40;
			var project = ProjectService.OpenSolution.FindProjectContainingFile(fileName) as CompilableProject;
			if (project == null)
				return DotNet40;
			return ScanVersion(project.TargetFrameworkVersion);
		}

		static Version ScanVersion(string versionString)
		{
			if (versionString == null)
				return DotNet40;
			Version version = new Version(versionString.TrimStart('v'));
			if (version < DotNet40)
				return DotNet20;
			return DotNet40;
		}
		
		static readonly string[] publicKeys = new[] {
			"b77a5c561934e089", "b03f5f7f11d50a3a"
		};
		
		static bool IsFrameworkAssembly(byte[] publicKey)
		{
			string key = publicKey.Aggregate(new StringBuilder(), (sum, part) => sum.AppendFormat("{0:x2}", part)).ToString();
			return publicKeys.Contains(key);
		}
		
		static string PrintVersion(Version version)
		{
			return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build > 0 ? version.Build : 0, version.Revision > 0 ? version.Revision : 0);
		}
		
		/// <summary>
		/// Update all .resx in a project.
		/// </summary>
		public static void UpdateResourceFiles(CompilableProject project)
		{
			foreach (var resXFile in project.Items.OfType<FileProjectItem>().Where(f => ".resx".Equals(Path.GetExtension(f.FileName), StringComparison.OrdinalIgnoreCase))) {
				using (var buffer = new MemoryStream()) {
					using (var reader = new ResXResourceReader(resXFile.FileName) { UseResXDataNodes = true })
					using (var writer = new ResXResourceWriter(buffer, t => ConvertTypeName(t, resXFile.FileName, project))) {
						foreach (DictionaryEntry entry in reader) {
							writer.AddResource(entry.Key.ToString(), entry.Value);
						}
					}
					File.WriteAllBytes(resXFile.FileName, buffer.ToArray());
				}
			}
		}
	}
}

