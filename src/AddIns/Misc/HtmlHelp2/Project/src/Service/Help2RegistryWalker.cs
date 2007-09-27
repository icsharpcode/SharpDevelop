// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.Environment
{
	using System;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;
	
	using MSHelpServices;

	public sealed class Help2RegistryWalker
	{
		Help2RegistryWalker()
		{
		}

		public static bool BuildNamespacesList(ComboBox help2Collections, string selectedHelp2Collection)
		{
			if (help2Collections == null)
			{
				throw new ArgumentNullException("help2Collections");
			}

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

			if (registryWalker == null || help2Namespaces == null || help2Namespaces.Count == 0)
			{
				return false;
			}

			help2Collections.Items.Clear();
			help2Collections.BeginUpdate();
			try
			{
				string currentDescription = string.Empty;

				foreach (IHxRegNamespace currentNamespace in help2Namespaces)
				{
					help2Collections.Items.Add
						((string)currentNamespace.GetProperty(HxRegNamespacePropId.HxRegNamespaceDescription));

					if (!string.IsNullOrEmpty(selectedHelp2Collection) &&
					    string.Compare(selectedHelp2Collection, currentNamespace.Name) == 0)
					{
						currentDescription =
							(string)currentNamespace.GetProperty(HxRegNamespacePropId.HxRegNamespaceDescription);
					}
				}

				if (!string.IsNullOrEmpty(currentDescription))
					help2Collections.SelectedIndex = help2Collections.Items.IndexOf(currentDescription);
				else
					help2Collections.SelectedIndex = 0;
			}
			finally
			{
				help2Collections.EndUpdate();
			}
			return true;
		}

		public static string GetNamespaceName(string description)
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

			if (registryWalker == null || help2Namespaces == null || help2Namespaces.Count == 0)
			{
				return string.Empty;
			}
			foreach (IHxRegNamespace currentNamespace in help2Namespaces)
			{
				string currentDescription =
					(string)currentNamespace.GetProperty(HxRegNamespacePropId.HxRegNamespaceDescription);
				if (string.Compare(currentDescription, description) == 0)
				{
					return currentNamespace.Name;
				}
			}
			return string.Empty;
		}

		public static string GetFirstNamespace(string name)
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

			if (registryWalker == null || help2Namespaces == null || help2Namespaces.Count == 0)
			{
				return string.Empty;
			}
			foreach (IHxRegNamespace currentNamespace in help2Namespaces)
			{
				if (string.Compare(currentNamespace.Name, name) == 0)
				{
					return name;
				}
			}
			return help2Namespaces.ItemAt(1).Name;
		}

		public static string GetFirstMatchingNamespaceName(string matchingName)
		{
			HxRegistryWalkerClass registryWalker;
			IHxRegNamespaceList help2Namespaces;
			try
			{
				registryWalker = new HxRegistryWalkerClass();
				help2Namespaces = registryWalker.get_RegisteredNamespaceList("");

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
			catch (System.Runtime.InteropServices.COMException)
			{
				return string.Empty;
			}
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
