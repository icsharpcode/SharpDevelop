/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 28.02.2006
 * Time: 16:24
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using ICSharpCode.Core;

namespace ICSharpCode.CodeAnalysis
{
	public static class FxCopWrapper
	{
		static List<FxCopCategory> rules = new List<FxCopCategory>();
		static List<Action<List<FxCopCategory>>> callbacks = new List<Action<List<FxCopCategory>>>();
		
		/// <summary>
		/// Gets the rules supported by the current FxCop version. The rules are loaded on a separate
		/// thread, the callback is fired when the action has completed.
		/// Warning: the callback might be fired on the current thread if the rules are already loaded,
		/// or on another thread!
		/// </summary>
		public static void GetRuleList(Action<List<FxCopCategory>> callback)
		{
			int count;
			lock (rules) {
				count = rules.Count;
				if (count == 0) {
					callbacks.Add(callback);
					if (callbacks.Count == 1) {
						// Start the thread:
						System.Threading.ThreadPool.QueueUserWorkItem(RunGetRuleList);
					}
				}
			}
			if (count > 0) {
				callback(rules);
			}
		}
		
		public static string FindFxCopPath()
		{
			string fxCopPath = PropertyService.Get("CodeAnalysis.FxCopPath");
			if (fxCopPath.Length > 0 && File.Exists(Path.Combine(fxCopPath, "FxCopCommon.dll"))) {
				return fxCopPath;
			}
			// Code duplication: FxCop.cs in ICSharpCode.Build.Tasks
			fxCopPath = FromRegistry(Registry.CurrentUser.OpenSubKey(@"Software\Classes\FxCopProject\Shell\Open\Command"));
			if (fxCopPath.Length > 0 && File.Exists(Path.Combine(fxCopPath, "FxCopCommon.dll"))) {
				return fxCopPath;
			}
			fxCopPath = FromRegistry(Registry.ClassesRoot.OpenSubKey(@"FxCopProject\Shell\Open\Command"));
			if (fxCopPath.Length > 0 && File.Exists(Path.Combine(fxCopPath, "FxCopCommon.dll"))) {
				return fxCopPath;
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
			LoggingService.Debug("Trying to find FxCop rules");
			string fxCopPath = FindFxCopPath();
			if (fxCopPath != null) {
				try {
					GetRuleListAndSort(fxCopPath);
				} catch (Exception ex) {
					LoggingService.Warn(ex);
				}
			}
			Action<List<FxCopCategory>>[] callbacks_tmp;
			lock (rules) {
				callbacks_tmp = callbacks.ToArray();
				callbacks.Clear();
			}
			LoggingService.Debug("Finished getting FxCop rules, invoking " + callbacks_tmp.Length + " callback");
			foreach (Action<List<FxCopCategory>> callback in callbacks_tmp) {
				callback(rules);
			}
		}
		
		static void GetRuleListAndSort(string fxCopPath)
		{
			AppDomainSetup setup = new AppDomainSetup();
			setup.DisallowCodeDownload = true;
			setup.ApplicationBase = fxCopPath;
			AppDomain domain = AppDomain.CreateDomain("FxCop Rule Loading Domain", AppDomain.CurrentDomain.Evidence, setup);
			
			string[][] ruleTextList;
			try {
				ruleTextList = (string[][])AppDomainLaunchHelper.LaunchInAppDomain(domain, typeof(FxCopWrapper), "GetRuleListInCurrentAppDomain", fxCopPath);
			} finally {
				AppDomain.Unload(domain);
			}
			
			FxCopRule[] ruleList = new FxCopRule[ruleTextList.Length];
			for (int i = 0; i < ruleTextList.Length; i++) {
				ruleList[i] = new FxCopRule(ruleTextList[i][0], ruleTextList[i][1],
				                            ruleTextList[i][2], ruleTextList[i][3],
				                            ruleTextList[i][4]);
			}
			
			Array.Sort(ruleList);
			lock (rules) {
				FxCopCategory cat = null;
				foreach (FxCopRule rule in ruleList) {
					if (cat == null || cat.Name != rule.CategoryName) {
						cat = new FxCopCategory(rule.CategoryName);
						rules.Add(cat);
					}
					cat.Rules.Add(rule);
				}
			}
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
		
		public static string[][] GetRuleListInCurrentAppDomain(string fxCopPath)
		{
			Assembly asm = Assembly.LoadFrom(Path.Combine(fxCopPath, "FxCopCommon.dll"));
			
			Type fxCopOM = asm.GetType("Microsoft.FxCop.Common.FxCopOM");
			CallMethod(fxCopOM, "Initialize", BindingFlags.Static, null);
			
			object project = asm.CreateInstance("Microsoft.FxCop.Common.Project");
			fxCopOM.InvokeMember("Project", BindingFlags.Public | BindingFlags.Static | BindingFlags.SetProperty,
			                     null, null, new object[] { project });
			
			object exceptionList = CallMethod(project, "Initialize");
			foreach (Exception ex in ((IEnumerable)exceptionList)) {
				LoggingService.Warn(ex);
			}
			
			IEnumerable ruleList = (IEnumerable)GetProp(GetProp(project, "AllRules"), "Values");
			List<string[]> rules = new List<string[]>();
			foreach (object ruleContainer in ruleList) {
				object rule = GetProp(ruleContainer, "IRule");
				rules.Add(new string[] {
				          	GetSProp(rule, "CheckId"),
				          	GetSProp(rule, "Name"),
				          	GetSProp(rule, "Category"),
				          	GetSProp(rule, "Description"),
				          	GetSProp(rule, "Url")
				          });
			}
			
			return rules.ToArray();
		}
	}
}
