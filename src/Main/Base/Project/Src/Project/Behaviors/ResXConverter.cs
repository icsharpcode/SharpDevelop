// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class ResXConverter
	{
		public static string ConvertTypeName(Type type, FileName fileName, CompilableProject project = null)
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
		
		static Version GetTargetFrameworkVersionFrom(FileName fileName)
		{
			var project = SD.ProjectService.FindProjectContainingFile(fileName) as CompilableProject;
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
