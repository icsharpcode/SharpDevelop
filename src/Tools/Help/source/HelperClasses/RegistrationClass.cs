//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.HelperClasses
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Xml;
	using MSHelpServices;

	public class Help2RegisterClass : IDisposable
	{
		HxRegisterSessionClass registerSession = null;
		IHxRegister register = null;
		IHxFilters filters = null;
		IHxPlugIn plugins = null;

		#region Constructor/Destructor
		public Help2RegisterClass()
		{
			try
			{
				registerSession = new HxRegisterSessionClass();
				registerSession.CreateTransaction("");

				register = (IHxRegister)registerSession.GetRegistrationObject(HxRegisterSession_InterfaceType.HxRegisterSession_IHxRegister);
				filters  = (IHxFilters)registerSession.GetRegistrationObject(HxRegisterSession_InterfaceType.HxRegisterSession_IHxFilters);
				plugins  = (IHxPlugIn)registerSession.GetRegistrationObject(HxRegisterSession_InterfaceType.HxRegisterSession_IHxPlugIn);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
		}

		~Help2RegisterClass()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (registerSession != null)
				{
					// PLEASE DO NOT CHANGE OR REMOVE THAT!!!

					registerSession.CommitTransaction();

					// It's very important to close the connection to the Help 2.0
					// environment. Trust me. I cannot explain because I don't know
					// anything about the Help 2.0 API. I was experimenting with the
					// Help 2.0 system and I knocked it out so many times ...
				}
			}
		}
		#endregion

		private static string GetXmlContent(string collectionFile, string xmlNode)
		{
			if (string.IsNullOrEmpty(collectionFile) || string.IsNullOrEmpty(xmlNode))
			{
				return string.Empty;
			}
			try
			{
				XmlDocument xmldoc = new XmlDocument();
				xmldoc.Load(collectionFile);
				XmlNodeList n = xmldoc.SelectNodes
					(string.Format(CultureInfo.InvariantCulture, "/HelpCollection/{0}/@File", xmlNode));

				if (n.Count > 0)
				{
					return n.Item(0).InnerText;
				}
			}
			catch (NullReferenceException)
			{
			}
			return string.Empty;
		}

		#region Register/Unregister
		public bool RegisterNamespace(string namespaceName, string collectionFile)
		{
			return this.RegisterNamespace(namespaceName, collectionFile, string.Empty, true);
		}

		public bool RegisterNamespace(string namespaceName, string collectionFile, string description)
		{
			return this.RegisterNamespace(namespaceName, collectionFile, description, true);
		}
		
		public bool RegisterNamespace(string namespaceName, string collectionFile, string description, bool overwrite)
		{
			if (register == null || string.IsNullOrEmpty(namespaceName) || string.IsNullOrEmpty(collectionFile))
			{
				return false;
			}
			try
			{
				// The default setting is to remove the namespace. But if you
				// just want to add some new help documents or filters, you
				// shouldn't remove it.
				if(overwrite && register.IsNamespace(namespaceName))
				{
					register.RemoveNamespace(namespaceName);
				}

				// If the namespace doesn't exist, create it
				if(!register.IsNamespace(namespaceName))
				{
					register.RegisterNamespace(namespaceName, collectionFile, description);
				}
				return true;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			return false;
		}

		public bool RemoveNamespace(string namespaceName)
		{
			if (register == null || string.IsNullOrEmpty(namespaceName))
			{
				return false;
			}
			try
			{
				if(register.IsNamespace(namespaceName))
				{
					register.RemoveNamespace(namespaceName);
				}

				return true;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			return false;
		}

		public bool RegisterHelpFile(string namespaceName, string helpFileId, int languageId, string hxsFile,
		                             string hxiFile, string hxqFile, string hxrFile, int hxsMediaId,
		                             int hxqMediaId, int hxrMediaId, int sampleMediaId)
		{
			if (register == null ||
			    string.IsNullOrEmpty(namespaceName) ||
			    string.IsNullOrEmpty(helpFileId) ||
			    string.IsNullOrEmpty(hxsFile))
			{
				return false;
			}
			try
			{
				if(register.IsNamespace(namespaceName))
				{
					register.RegisterHelpFileSet(namespaceName,		// Help 2.0 Collection Namespace
					                             helpFileId,		// internal Help document ID
					                             languageId,		// Language ID
					                             hxsFile,			// Help document
					                             hxiFile,			// external Help index
					                             hxqFile,			// merged query file
					                             hxrFile,			// combined attributes file
					                             hxsMediaId,
					                             hxqMediaId,
					                             hxrMediaId,
					                             sampleMediaId);

					// If you want to know something about those file types, I suggest you
					// take a look at Microsoft's VSHIK documentation.
				}

				return true;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			return false;
		}

		public bool RemoveHelpFile(string namespaceName, string helpFileId, int languageId)
		{
			if (register == null ||
			    string.IsNullOrEmpty(namespaceName) ||
			    string.IsNullOrEmpty(helpFileId))
			{
				return false;
			}
			try
			{
				if(register.IsNamespace(namespaceName))
				{
					register.RemoveHelpFile(namespaceName, helpFileId, languageId);
				}

				return true;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			return false;
		}

		public bool RegisterFilter(string namespaceName, string filterName, string filterQuery)
		{
			if (register == null ||
			    filters == null ||
			    string.IsNullOrEmpty(namespaceName) ||
			    string.IsNullOrEmpty(filterName))
			{
				return false;
			}
			try
			{
				filters.SetNamespace(namespaceName);
				filters.SetCollectionFiltersFlag(true);
				filters.RegisterFilter(filterName, filterQuery);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			
			// This function ALWAYS returns true. It's because an empty filter
			// query raises an exception but the filter will be created. A good
			// example is the known "(no filter)" filter.
			// So, don't change it.

			return true;
		}

		public bool RemoveFilter(string namespaceName, string filterName)
		{
			if (register == null ||
			    filters == null ||
			    string.IsNullOrEmpty(namespaceName) ||
			    string.IsNullOrEmpty(filterName))
			{
				return false;
			}
			try
			{
				filters.SetNamespace(namespaceName);
				filters.SetCollectionFiltersFlag(true);
				filters.RemoveFilter(filterName);

				return true;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			return false;
		}

		public bool RegisterPlugin(string parentNamespace, string childNamespace)
		{
			return this.PluginAction(parentNamespace, childNamespace, true);
		}

		public bool RemovePlugin(string parentNamespace, string childNamespace)
		{
			return this.PluginAction(parentNamespace, childNamespace, false);
		}

		private bool PluginAction(string parentNamespace, string childNamespace, bool registerIt)
		{
			if (register == null ||
			    plugins == null ||
			    string.IsNullOrEmpty(parentNamespace) ||
			    string.IsNullOrEmpty(childNamespace) ||
			    !register.IsNamespace(parentNamespace) ||
			    !register.IsNamespace(childNamespace))
			{
				return false;
			}

			// if you want to remove a plug-in, at least it should be there
			if(!registerIt && !PluginSearch.PluginDoesExist(parentNamespace, childNamespace))
			{
				return false;
			}

			try
			{
				// unregister plug-in
				if(!registerIt)
				{
					if(PluginSearch.PluginDoesExist(parentNamespace, childNamespace))
					{
						plugins.RemoveHelpPlugIn(parentNamespace, "", childNamespace, "", "");
						return true;
					}
				}

				// (re)register plug-in
				string path1 = string.Empty;

				// The function requires the names of the TOC files. I can take them from
				// the collection level files (*.HxC) of the collections.
				string parentToc = GetXmlContent(register.GetCollection(parentNamespace), "TOCDef");
				string childToc  = GetXmlContent(register.GetCollection(childNamespace), "TOCDef");
				string attr      = GetXmlContent(register.GetCollection(childNamespace), "AttributeDef");

				if(!string.IsNullOrEmpty(attr))
				{
					path1 = Path.Combine(Path.GetDirectoryName(register.GetCollection(childNamespace)), attr);
				}

				if(registerIt && !string.IsNullOrEmpty(parentToc) && !string.IsNullOrEmpty(childToc))
				{
					plugins.RegisterHelpPlugIn(parentNamespace, parentToc, childNamespace, childToc, path1, 0);
					return true;
				}
				else return false;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
			return false;
		}
		#endregion
	}
}
