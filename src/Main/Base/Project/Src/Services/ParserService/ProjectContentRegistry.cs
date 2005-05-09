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
			lock (contents) {
				if (contents.ContainsKey("mscorlib")) {
					return contents["mscorlib"];
				}
				#if DEBUG
				Console.WriteLine("Loading mscorlib...");
				int time = Environment.TickCount;
				#endif
				
				contents["mscorlib"] = DefaultProjectContent.Create(typeof(object).Assembly);
				
				#if DEBUG
				Console.WriteLine("mscorlib loaded in {0} ms", Environment.TickCount - time);
				#endif
				return contents["mscorlib"];
			}
		}
		
		public static IProjectContent GetProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			lock (contents) {
				if (contents.ContainsKey(item.FileName)) {
					return contents[item.FileName];
				}
				if (contents.ContainsKey(item.Include)) {
					return contents[item.Include];
				}
				
				string shortName = item.Include;
				int pos = shortName.IndexOf(',');
				if (pos > 0)
					shortName = shortName.Substring(0, pos);
				
				StatusBarService.ProgressMonitor.BeginTask("Loading " + shortName + "...", 100);
				#if DEBUG
				int time = Environment.TickCount;
				#endif
				Assembly assembly = null;
				try {
					assembly = Assembly.ReflectionOnlyLoadFrom(item.FileName);
					if (assembly != null) {
						contents[item.FileName] = DefaultProjectContent.Create(assembly);
						return contents[item.FileName];
					}
				} catch (Exception) {
					try {
						assembly = LoadGACAssembly(item.Include, true);
						if (assembly != null) {
							contents[item.Include] = DefaultProjectContent.Create(assembly);
							return contents[item.Include];
						}
					} catch (Exception e) {
						Console.WriteLine("Can't load assembly '{0}' : " + e.Message, item.Include);
					}
				} finally {
					#if DEBUG
					Console.WriteLine("Loaded {0} in {1}ms", item.Include, Environment.TickCount - time);
					#endif
					StatusBarService.ProgressMonitor.Done();
				}
			}
			return null;
		}
		
		public static Assembly LoadGACAssembly(string partialName, bool reflectionOnly)
		{
			#pragma warning disable 618
			return Assembly.LoadWithPartialName(partialName);
			#pragma warning restore 618
		}
	}
}
