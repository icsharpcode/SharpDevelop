using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	public class ProjectContentRegistry
	{
		static Dictionary<string, IProjectContent> contents = new Dictionary<string, IProjectContent>();
		
		public static IProjectContent GetMscorlibContent()
		{
			if (contents.ContainsKey("mscorlib")) {
				return contents["mscorlib"];
			}
			contents["mscorlib"] = CaseSensitiveProjectContent.Create(typeof(object).Assembly);
			return contents["mscorlib"];
		}
		
		public static IProjectContent GetProjectContentForReference(ReferenceProjectItem item)
		{
			if (contents.ContainsKey(item.FileName)) {
				Console.WriteLine("Get Content for : " + item.FileName);
				return contents[item.FileName];
			}
			if (contents.ContainsKey(item.Include)) {
				Console.WriteLine("Get Content for : " + item.Include);
				return contents[item.Include];
			}
			Assembly assembly = null;
			
			try {
				assembly = Assembly.ReflectionOnlyLoadFrom(item.FileName);
				Console.WriteLine(assembly.Location);
				if (assembly != null) {
					contents[item.FileName] = CaseSensitiveProjectContent.Create(assembly);
					return contents[item.FileName];
				}
			} catch (Exception) {
				try {
					assembly = Assembly.LoadWithPartialName(item.Include);
					Console.WriteLine(assembly.Location);
					if (assembly != null) {
						contents[item.Include] = CaseSensitiveProjectContent.Create(assembly);
						return contents[item.Include];
					}
				} catch (Exception e) {
					Console.WriteLine("Can't load assembly '{0}' : " + e.Message, item.Include);
				}
			}
			
			return null;
		}
	}
}
