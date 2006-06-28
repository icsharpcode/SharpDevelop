// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.RegistryWalker
{
	using System;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;
	using ICSharpCode.Core;
	using MSHelpServices;

	public sealed class Help2RegistryWalker
	{
		public static bool BuildNamespacesList(ComboBox help2Collections, string selectedHelp2Collection)
		{
			if (help2Collections == null)
			{
				return false;
			}
			help2Collections.Items.Clear();
			help2Collections.BeginUpdate();
			bool result = true;

			try
			{
				string currentDescription = string.Empty;
				HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
				IHxRegNamespaceList namespaces = registryWalker.get_RegisteredNamespaceList("");

				foreach (IHxRegNamespace currentNamespace in namespaces)
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
			catch
			{
				result = false;
			}
			finally
			{
				help2Collections.EndUpdate();
			}

			return result;
		}

		public static string GetNamespaceName(string description)
		{
			try
			{
				HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
				IHxRegNamespaceList namespaces = registryWalker.get_RegisteredNamespaceList("");

				foreach (IHxRegNamespace currentNamespace in namespaces)
				{
					string currentDescription =
						(string)currentNamespace.GetProperty(HxRegNamespacePropId.HxRegNamespaceDescription);
					if (string.Compare(currentDescription, description) == 0)
					{
						return currentNamespace.Name;
					}
				}
			}
			catch
			{
				LoggingService.Error("Help 2.0: cannot find selected namespace name");
			}
			return string.Empty;
		}

		public static string GetFirstNamespace(string name)
		{
			try
			{
				HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
				IHxRegNamespaceList namespaces = registryWalker.get_RegisteredNamespaceList("");

				foreach (IHxRegNamespace currentNamespace in namespaces)
				{
					if (string.Compare(currentNamespace.Name, name) == 0)
					{
						return name;
					}
				}
				return namespaces.ItemAt(1).Name;
			}
			catch
			{
				return string.Empty;
			}
		}

		public static string GetFirstMatchingNamespaceName(string matchingName)
		{
			if (!string.IsNullOrEmpty(matchingName))
			{
				try
				{
					HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
					IHxRegNamespaceList namespaces = registryWalker.get_RegisteredNamespaceList("");

					foreach (IHxRegNamespace currentNamespace in namespaces)
					{
						if (PathMatchSpec(currentNamespace.Name, matchingName))
						{
							return currentNamespace.Name;
						}
					}
				}
				catch
				{
					// I don't care about an exception here ;-)
				}
			}

			return string.Empty;
		}

		#region PatchMatchSpec@Win32API
		[DllImport("shlwapi.dll")]
		static extern bool PathMatchSpec(string pwszFile, string pwszSpec);
		#endregion
	}
}
