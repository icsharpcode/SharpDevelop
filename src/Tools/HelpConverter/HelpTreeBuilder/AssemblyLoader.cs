using System;
using System.Reflection;

namespace ICSharpCode.HelpConverter.HelpTreeBuilder
{
	public class AssemblyLoader
	{
//		public AssemblyLoader()
//		{
//			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolve);
//		}
//		
//		Assembly resolve(object sender, ResolveEventArgs e)
//		{
//			System.Console.WriteLine("warning: assembly " + e.Name + " couln't be loaded.");
//			return null;
//		}
//		
		public Assembly[] LoadAssemblies(string[] names)
		{
			Assembly[] assemblies = new Assembly[names.Length];
			int count = 0;
			
			foreach(string name in names) {
				Console.WriteLine("Load assembly : >" + name + "<");
				Assembly assembly = Assembly.Load(name);
				if(assembly != null) {
					assemblies[count] = Assembly.Load(name);
					count++;
				} else {
					throw new Exception("Unable to load assembly " + name + ".");
				}
			}
			return assemblies;
		}
	}
}
