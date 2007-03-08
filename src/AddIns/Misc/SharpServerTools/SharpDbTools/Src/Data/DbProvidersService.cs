// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision: 1697 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace SharpDbTools.Data
{
	/// <summary>
	/// A utility class that caches the DbProviderFactory and DbConnectionString
	/// objects whose state is stored in the current processes config space.
	/// </summary>
	public class DbProvidersService
	{
		private static DbProvidersService me = new DbProvidersService();
		private static Boolean initialized = false;
		private Dictionary<string, DbProviderFactory> factories = new Dictionary<string, DbProviderFactory>();
		private Dictionary<string, DbProviderFactory> factoriesByInvariantName = new Dictionary<string, DbProviderFactory>();
		
		// This is only valid witin one session - do not persist
		private Dictionary<string, string> invariantByNameLookup = new Dictionary<string, string>();
		private List<string> names = new List<string>();
		
		private DbProvidersService()
		{
		}
		
		private void Initialize()
		{
			// get a complete list of config data for DbProviderFactories, indexed by name
			DataTable providerFactoriesTable = DbProviderFactories.GetFactoryClasses();
			DataRow[] rows = providerFactoriesTable.Select();
			
			foreach(DataRow row in rows)
			{
				// TODO: factor out string literals for column names
				string name = (string)row["Name"];
				string invariantName = (string)row["InvariantName"];
				LoggingService.Debug("adding lookup for: " + name + " to: + " + invariantName);
				invariantByNameLookup.Add(name, invariantName);
				//factoryData.Add(name, row);
				try {
					LoggingService.Debug("retrieving DbProviderFactory for Name: " 
					                     + name + " InvariantName: " + invariantName);
					DbProviderFactory factory = DbProviderFactories.GetFactory(row);
					names.Add(name);
					factories.Add(name, factory);
					factoriesByInvariantName.Add(invariantName, factory);
				} catch (Exception) {
					MessageBox.Show("Unable to load DbProviderFactory for: " + name + ", this will be unavailable." +
					                "\nCheck *.config files for invalid ado.net config elements, or config" +
					                "for assemblies that are not available.",
					                "Exception loading DbProviderFactory", MessageBoxButtons.OK, 
					                MessageBoxIcon.Error);
				}
			}

			initialized = true;
		}
		
		public List<string> Names
		{
			get
			{
				return names;
			}
		}
		
		public string this[int i]
		{
			get
			{
				return names[i];
			}
		}
		
		public string GetInvariantName(string name) {
			string invariantName = null;
			invariantByNameLookup.TryGetValue(name, out invariantName);
			return invariantName;
		}
		
		public DbProviderFactory this[string name]
		{
			get
			{
				return factories[name];
			}
			set
			{
				factories[name] = value;
			}
		}
		
		public DbProviderFactory GetFactoryByInvariantName(string invariantName)
		{
			DbProviderFactory factory = null;
			this.factoriesByInvariantName.TryGetValue(invariantName, out factory);
			return factory;
		}
		
		public static DbProvidersService GetDbProvidersService()
		{
			lock(me)
			{
				if (!initialized)
				{
					me.Initialize();
				}
			}
			return me;
		}
	}
}
