//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.HelperClasses
{
	using System;
	using System.Collections.Specialized;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Runtime.InteropServices;
	using MSHelpServices;

	public sealed class PluginSearch
	{
		PluginSearch()
		{
		}

		public static bool PluginDoesExist(string namespaceName, string pluginName)
		{
			if (string.IsNullOrEmpty(namespaceName) || string.IsNullOrEmpty(pluginName))
			{
				return false;
			}

			try
			{
				HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
				IHxRegNamespaceList help2Namespaces = registryWalker.get_RegisteredNamespaceList("");

				foreach (IHxRegNamespace currentNamespace in help2Namespaces)
				{
					if (string.Compare(currentNamespace.Name, namespaceName) == 0)
					{
						IHxRegPlugInList p =
							(IHxRegPlugInList)currentNamespace.GetProperty(HxRegNamespacePropId.HxRegNamespacePlugInList);
						foreach (IHxRegPlugIn plugin in p)
						{
							string currentName = (string)plugin.GetProperty(HxRegPlugInPropId.HxRegPlugInName);
							if (string.Compare(currentName, pluginName) == 0)
							{
								return true;
							}
						}
					}
				}
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			return false;
		}

		public static StringCollection FindPlugin(string pluginName)
		{
			StringCollection namespaces = new StringCollection();
			try
			{
				HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
				IHxRegNamespaceList help2Namespaces = registryWalker.get_RegisteredNamespaceList("");
				
				foreach (IHxRegNamespace currentNamespace in help2Namespaces)
				{
					IHxRegPlugInList p =
						(IHxRegPlugInList)currentNamespace.GetProperty(HxRegNamespacePropId.HxRegNamespacePlugInList);
					foreach (IHxRegPlugIn plugin in p)
					{
						string currentName = (string)plugin.GetProperty(HxRegPlugInPropId.HxRegPlugInName);
						if (string.Compare(currentName, pluginName) == 0)
						{
							namespaces.Add(currentNamespace.Name);
							break;
						}
					}
				}
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			return namespaces;
		}

		public static ReadOnlyCollection<string> FindPluginAsGenericList(string pluginName)
		{
			List<string> namespaces = new List<string>();
			try
			{
				HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
				IHxRegNamespaceList help2Namespaces = registryWalker.get_RegisteredNamespaceList("");
				
				foreach (IHxRegNamespace currentNamespace in help2Namespaces)
				{
					IHxRegPlugInList p =
						(IHxRegPlugInList)currentNamespace.GetProperty(HxRegNamespacePropId.HxRegNamespacePlugInList);
					foreach (IHxRegPlugIn plugin in p)
					{
						string currentName = (string)plugin.GetProperty(HxRegPlugInPropId.HxRegPlugInName);
						if (string.Compare(currentName, pluginName) == 0)
						{
							namespaces.Add(currentNamespace.Name);
							break;
						}
					}
				}
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			ReadOnlyCollection<string> roNamespaces = new ReadOnlyCollection<string>(namespaces);
			return roNamespaces;
		}

		public static string GetFirstMatchingNamespaceName(string matchingName)
		{
			HxRegistryWalkerClass registryWalker;
			IHxRegNamespaceList help2Namespaces;
			try
			{
				registryWalker = new HxRegistryWalkerClass();
				help2Namespaces = registryWalker.get_RegisteredNamespaceList("");
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				help2Namespaces = null;
				registryWalker = null;
			}

			if (registryWalker == null || help2Namespaces == null || help2Namespaces.Count == 0 || string.IsNullOrEmpty(matchingName))
			{
				return string.Empty;
			}
			foreach (IHxRegNamespace currentNamespace in help2Namespaces)
			{
				if (NativeMethods.PathMatchSpec(currentNamespace.Name, matchingName))
				{
					return currentNamespace.Name;
				}
			}
			return help2Namespaces.ItemAt(1).Name;
		}
	}

	internal class NativeMethods
	{
		NativeMethods()
		{
		}

		#region PatchMatchSpec
		[DllImport("shlwapi.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PathMatchSpec(string pwszFile, string pwszSpec);
		#endregion
	}
}
