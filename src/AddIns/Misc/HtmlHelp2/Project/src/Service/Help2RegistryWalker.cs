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
		private static HxRegNamespacePropId NamespaceDescription =
			HxRegNamespacePropId.HxRegNamespaceDescription;

		public Help2RegistryWalker()
		{
		}

		static Help2RegistryWalker()
		{
		}

		public static void BuildNamespacesList(ComboBox help2Collections, string selectedHelp2Collection)
		{
			if (help2Collections == null) return;
			help2Collections.Items.Clear();
			help2Collections.BeginUpdate();

			try
			{
				string currentDescription      = "";
				HxRegistryWalker regWalker     = new HxRegistryWalker();
				IHxRegNamespaceList namespaces = regWalker.get_RegisteredNamespaceList("");

				foreach (IHxRegNamespace currentNamespace in namespaces)
				{
					help2Collections.Items.Add((string)currentNamespace.GetProperty(NamespaceDescription));

					if (selectedHelp2Collection != "" &&
					    String.Compare(selectedHelp2Collection, currentNamespace.Name) == 0)
					{
						currentDescription =
							(string)currentNamespace.GetProperty(NamespaceDescription);
					}
				}

				if (currentDescription != "")
					help2Collections.SelectedIndex = help2Collections.Items.IndexOf(currentDescription);
				else
					help2Collections.SelectedIndex = 0;
			}
			catch(Exception ex)
			{
				LoggingService.Error("Help 2.0: cannot build namespaces list for Options dialog", ex);
			}

			help2Collections.EndUpdate();
		}

		public static string GetNamespaceName(string namespaceDescription)
		{
			try
			{
				HxRegistryWalker regWalker     = new HxRegistryWalker();
				IHxRegNamespaceList namespaces = regWalker.get_RegisteredNamespaceList("");

				foreach (IHxRegNamespace currentNamespace in namespaces)
				{
					string currentNamespaceName = (string)currentNamespace.GetProperty(NamespaceDescription);
					if (String.Compare(namespaceDescription, currentNamespaceName) == 0)
					{
						return currentNamespace.Name;
					}
				}
			}
			catch
			{
				LoggingService.Error("Help 2.0: cannot find selected namespace name");
			}
			return "";
		}

		public static string GetFirstNamespace(string namespaceName)
		{
			try
			{
				HxRegistryWalker regWalker     = new HxRegistryWalker();
				IHxRegNamespaceList namespaces = regWalker.get_RegisteredNamespaceList("");

				foreach (IHxRegNamespace currentNamespace in namespaces)
				{
					if (String.Compare(namespaceName, currentNamespace.Name) == 0)
					{
						return namespaceName;
					}
				}
			
				return namespaces.ItemAt(1).Name;
			}
			catch
			{
				return "";
			}
		}

		public static string GetFirstMatchingNamespaceName(string matchingNamespaceName)
		{
			if (matchingNamespaceName == "") return "";

			try
			{
				HxRegistryWalker regWalker = new HxRegistryWalker();
				IHxRegNamespaceList nl = regWalker.get_RegisteredNamespaceList("");

				foreach (IHxRegNamespace currentNamespace in nl)
				{
					if (PathMatchSpec(currentNamespace.Name, matchingNamespaceName))
					{
						return currentNamespace.Name;
					}
				}
			}
			catch {}

			return "";
		}

		#region PatchMatchSpec@Win32API
		[DllImport("shlwapi.dll")]
		static extern bool PathMatchSpec(string pwszFile, string pwszSpec);
		#endregion
	}
}
