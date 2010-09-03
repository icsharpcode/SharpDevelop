// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using Microsoft.Win32;

namespace ICSharpCode.CodeAnalysis
{
	public class FxCopWrapper : MarshalByRefObject
	{
		static Dictionary<string[], List<FxCopCategory>> ruleDict = new Dictionary<string[], List<FxCopCategory>>(new ArrayHashCodeProvider());
		
		class ArrayHashCodeProvider : IEqualityComparer<string[]>
		{
			public bool Equals(string[] x, string[] y)
			{
				if (x == y) return true;
				if (x == null || y == null) return false;
				if (x.Length != y.Length) return false;
				for (int i = 0; i < x.Length; i++) {
					if (StringComparer.OrdinalIgnoreCase.Equals(x[i], y[i])) return false;
				}
				return true;
			}
			public int GetHashCode(string[] obj)
			{
				int hashcode = 0;
				foreach (string e in obj) {
					hashcode ^= StringComparer.OrdinalIgnoreCase.GetHashCode(e);
				}
				return hashcode;
			}
		}
		
		class Request
		{
			public string[] ruleAssemblies;
			public Action<List<FxCopCategory>> callback;
			public Request(string[] ruleAssemblies, Action<List<FxCopCategory>> callback)
			{
				this.ruleAssemblies = ruleAssemblies;
				this.callback = callback;
			}
		}
		
		/// <summary>
		/// Gets the rules supported by the current FxCop version. The rules are loaded on a separate
		/// thread, the callback is fired when the action has completed.
		/// Warning: the callback might be fired on the current thread if the rules are already loaded,
		/// or on another thread!
		/// </summary>
		public static void GetRuleList(string[] ruleAssemblies, Action<List<FxCopCategory>> callback)
		{
			if (ruleAssemblies == null)
				throw new ArgumentNullException("ruleAssemblies");
			List<FxCopCategory> rules = null;
			lock (ruleDict) {
				if (!ruleDict.TryGetValue(ruleAssemblies, out rules)) {
					// Start the thread:
					System.Threading.ThreadPool.QueueUserWorkItem(RunGetRuleList, new Request(ruleAssemblies, callback));
				}
			}
			if (rules != null) {
				callback(rules);
			}
		}
		
		public static bool IsFxCopPath(string fxCopPath)
		{
			if (string.IsNullOrEmpty(fxCopPath))
				return false;
			else
				return File.Exists(Path.Combine(fxCopPath, "FxCopCommon.dll"));
		}
		
		public static string FindFxCopPath()
		{
			string fxCopPath = AnalysisIdeOptionsPanel.FxCopPath;
			if (string.IsNullOrEmpty(fxCopPath)) {
				// Code duplication: FxCop.cs in ICSharpCode.Build.Tasks
				using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0\Setup\EDev")) {
					if (key != null) {
						fxCopPath = key.GetValue("FxCopDir") as string;
					}
				}
				if (IsFxCopPath(fxCopPath)) {
					return fxCopPath;
				}
				using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\8.0\Setup\EDev")) {
					if (key != null) {
						fxCopPath = key.GetValue("FxCopDir") as string;
					}
				}
				if (IsFxCopPath(fxCopPath)) {
					return fxCopPath;
				}

				fxCopPath = FromRegistry(Registry.ClassesRoot.OpenSubKey(@"FxCop.Project.9.0\Shell\Open\Command"));
				if (IsFxCopPath(fxCopPath)) {
					return fxCopPath;
				}
				fxCopPath = FromRegistry(Registry.CurrentUser.OpenSubKey(@"Software\Classes\FxCopProject\Shell\Open\Command"));
				if (IsFxCopPath(fxCopPath)) {
					return fxCopPath;
				}
				fxCopPath = FromRegistry(Registry.ClassesRoot.OpenSubKey(@"FxCopProject\Shell\Open\Command"));
				if (IsFxCopPath(fxCopPath)) {
					return fxCopPath;
				}
				return null;
			} else {
				if (IsFxCopPath(fxCopPath)) {
					return fxCopPath;
				}
			}
			return null;
		}
		
		static string FromRegistry(RegistryKey key)
		{
			// Code duplication: FxCop.cs in ICSharpCode.Build.Tasks
			if (key == null) return string.Empty;
			using (key) {
				string cmd = key.GetValue("").ToString();
				int pos;
				if (cmd.StartsWith("\""))
					pos = cmd.IndexOf('"', 1);
				else
					pos = cmd.IndexOf(' ');
				try {
					if (cmd.StartsWith("\""))
						return Path.GetDirectoryName(cmd.Substring(1, pos - 1));
					else
						return Path.GetDirectoryName(cmd.Substring(0, pos));
				} catch (ArgumentException ex) {
					LoggingService.Warn(cmd);
					LoggingService.Warn(ex);
					return string.Empty;
				}
			}
		}
		
		static void RunGetRuleList(object state)
		{
			Request request = (Request)state;
			
			LoggingService.Debug("Trying to find FxCop rules");
			string fxCopPath = FindFxCopPath();
			List<FxCopCategory> rules;
			if (fxCopPath != null) {
				try {
					rules = GetRuleListAndSort(fxCopPath, request.ruleAssemblies);
				} catch (Exception ex) {
					LoggingService.Warn(ex);
					rules = new List<FxCopCategory>();
				}
			} else {
				rules = new List<FxCopCategory>();
			}
			LoggingService.Debug("Finished getting FxCop rules, invoking callback");
			request.callback(rules);
		}
		
		static List<FxCopCategory> GetRuleListAndSort(string fxCopPath, string[] ruleAssemblies)
		{
			AppDomainSetup setup = new AppDomainSetup();
			setup.DisallowCodeDownload = true;
			setup.ApplicationBase = fxCopPath;
			AppDomain domain = AppDomain.CreateDomain("FxCop Rule Loading Domain", AppDomain.CurrentDomain.Evidence, setup);
			
			FxCopRule[] ruleList;
			try {
				FxCopWrapper wrapper = (FxCopWrapper)domain.CreateInstanceFromAndUnwrap(typeof(FxCopWrapper).Assembly.Location, typeof(FxCopWrapper).FullName);
				
				ruleList = wrapper.GetRuleListInstanceMethod(fxCopPath, ruleAssemblies);
			} finally {
				AppDomain.Unload(domain);
			}
			
			Array.Sort(ruleList);
			List<FxCopCategory> rules = new List<FxCopCategory>();
			lock (ruleDict) {
				FxCopCategory cat = null;
				foreach (FxCopRule rule in ruleList) {
					if (cat == null || cat.Name != rule.CategoryName) {
						cat = new FxCopCategory(rule.CategoryName);
						rules.Add(cat);
					}
					cat.Rules.Add(rule);
				}
				ruleDict[ruleAssemblies] = rules;
			}
			return rules;
		}
		
		// We don't want to reference the FxCop assembly
		
		static object CallMethod(object instance, string name, params object[] args)
		{
			return instance.GetType().InvokeMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, instance, args);
		}
		
		static object CallMethod(Type type, string name, BindingFlags flags, object instance, params object[] args)
		{
			return type.InvokeMember(name, flags | BindingFlags.Public | BindingFlags.InvokeMethod,
			                         null, instance, args);
		}
		
		static object GetProp(object instance, string name)
		{
			return instance.GetType().InvokeMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
			                                       null, instance, null);
		}
		
		static string GetSProp(object instance, string name)
		{
			object v = GetProp(instance, name);
			if (v == null)
				return string.Empty;
			else
				return v.ToString();
		}
		
		FxCopRule[] GetRuleListInstanceMethod(string fxCopPath, string[] ruleAssemblies)
		{
			Assembly asm = Assembly.LoadFrom(Path.Combine(fxCopPath, "FxCopCommon.dll"));
			
			Type fxCopOM = asm.GetType("Microsoft.FxCop.Common.FxCopOM");
			CallMethod(fxCopOM, "Initialize", BindingFlags.Static, null);
			
			object engines = fxCopOM.InvokeMember("Engines", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, null, null);
			((System.Collections.Specialized.StringCollection)GetProp(engines, "RuleDirectories"))
				.AddRange(ruleAssemblies);
			
			object project = asm.CreateInstance("Microsoft.FxCop.Common.Project");
			fxCopOM.InvokeMember("Project", BindingFlags.Public | BindingFlags.Static | BindingFlags.SetProperty,
			                     null, null, new object[] { project });
			
			object exceptionList = CallMethod(project, "Initialize");
			foreach (Exception ex in ((IEnumerable)exceptionList)) {
				Console.WriteLine(ex.ToString());
			}
			
			IEnumerable ruleList = (IEnumerable)GetProp(GetProp(project, "AllRules"), "Values");
			
			List<FxCopRule> rules = new List<FxCopRule>();
			foreach (object ruleContainer in ruleList) {
				object rule = GetProp(ruleContainer, "IRule");
				rules.Add(new FxCopRule(
					GetSProp(rule, "CheckId"),
					GetSProp(rule, "Name"),
					GetSProp(rule, "Category"),
					GetSProp(rule, "Description"),
					GetSProp(rule, "Url")
				));
			}
			
			return rules.ToArray();
		}
	}
}
