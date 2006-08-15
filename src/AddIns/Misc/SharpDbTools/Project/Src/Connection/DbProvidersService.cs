// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

/*
 * Responsibilities:<br>
 * <br>
 * Collaboration:<br>
 * <br>
 * User: Dickon Field
 * Date: 21/05/2006
 * Time: 23:19
 * 
 */

using System;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;

namespace SharpDbTools.Connection
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
				invariantByNameLookup.Add(name, invariantName);
				//factoryData.Add(name, row);
				DbProviderFactory factory = DbProviderFactories.GetFactory(row);
				names.Add(name);
				factories.Add(name, factory);
				factoriesByInvariantName.Add(invariantName, factory);
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
