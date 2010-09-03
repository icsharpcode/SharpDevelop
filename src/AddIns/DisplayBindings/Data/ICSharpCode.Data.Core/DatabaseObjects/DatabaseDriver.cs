// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ICSharpCode.Data.Core.DatabaseObjects;
using ICSharpCode.Data.Core.Interfaces;
using System.IO;
using System.Reflection;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    /// <summary>
    /// Holds all available database drivers.
    /// </summary>
    public static class DatabaseDriver
    {
        #region Static fields

        private static List<IDatabaseDriver> _databaseDrivers = null;

        #endregion

        #region Static properties

        /// <summary>
        /// Gets all available database drivers.
        /// </summary>
        public static List<IDatabaseDriver> DatabaseDrivers
        {
            get { return _databaseDrivers; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor which loads all available database drivers.
        /// </summary>
        static DatabaseDriver()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(CurrentDomain_ReflectionOnlyAssemblyResolve);

            // Get all assumed plug in assemblies
            _databaseDrivers = new List<IDatabaseDriver>();
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string[] files = Directory.GetFiles(fileInfo.Directory.FullName, "ICSharpCode.Data.*.dll");

            // Iterate through all found files and search for IDatabaseDriver interface
            foreach (string file in files)
            {
                try
                {
                    Assembly assembly = Assembly.ReflectionOnlyLoadFrom(file);

                    Type[] types = assembly.GetTypes();

                    foreach (Type type in types)
                    {
                        if (type.GetInterface("IDatabaseDriver") != null)
                        {
                            if (!type.IsAbstract)
                            {
                                // Create an instance of the driver
                                Type loadedType = Assembly.LoadFrom(file).GetType(type.FullName);
                                _databaseDrivers.Add(Activator.CreateInstance(loadedType) as IDatabaseDriver);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        static Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        #endregion
    }
    
    /// <summary>
    /// Description of DatabaseDriver.
    /// </summary>
    public abstract class DatabaseDriver<T> : DatabaseObjectBase, IDatabaseDriver where T : IDatasource
    {
        #region Fields

        private DatabaseObjectsCollection<T> _datasources = new DatabaseObjectsCollection<T>();
        
        #endregion
                
        #region Properties

        /// <summary>
        /// Gets or sets the datasources of this database driver.
        /// </summary>
        public DatabaseObjectsCollection<T> Datasources
        {
            get { return _datasources; }
            protected set
            {
                _datasources = value;
                OnPropertyChanged("Datasources");
                OnPropertyChanged("IDatasources");
            }
        }

        /// <summary>
        /// Gets or sets the datasources of this database driver.
        /// </summary>
        public DatabaseObjectsCollection<IDatasource> IDatasources
        {
            get { return _datasources.Cast<IDatasource>().ToDatabaseObjectsCollection(); }
        }

        /// <summary>
        /// Gets the provider name of this database driver.
        /// </summary>
        public virtual string ProviderName
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the ODBC provider name of this database driver.
        /// </summary>
        public virtual string ODBCProviderName
        {
            get { throw new NotImplementedException(); }
        }
        
        #endregion

        #region Public methods

        /// <summary>
        /// Creates a new datasource for this driver.
        /// </summary>
        /// <param name="datasourceName">Location name or IP address</param>
        /// <returns>New datasource</returns>
        public IDatasource CreateNewIDatasource(string datasourceName)
        {
            return CreateNewDatasource(datasourceName);
        }

        /// <summary>
        /// Creates a new datasource for this driver.
        /// </summary>
        /// <param name="name">Location name or IP address</param>
        /// <returns>New datasource</returns>
        public T CreateNewDatasource(string datasourceName)
        {
            T newDatasource = (T)Activator.CreateInstance(typeof(T), new object[]{ this });
            newDatasource.Name = datasourceName;
            return newDatasource;
        }

        /// <summary>
        /// Adds a new datasource for this driver.
        /// </summary>
        /// <param name="datasourceName">Location name or IP address</param>
        /// <returns>Added new datasource</returns>
        public IDatasource AddNewDatasource(string datasourceName)
        {
            T existingDatasource = Datasources.FirstOrDefault(datasource => datasource.Name.ToUpper() == datasourceName.ToUpper());
            if (existingDatasource != null)
                return existingDatasource;
            
            T newDatasource = CreateNewDatasource(datasourceName);
            _datasources.Add(newDatasource);
            return newDatasource;
        }

        /// <summary>
        /// Remove datasource by its name.
        /// </summary>
        /// <param name="datasourceName">Location name or IP address</param>
        public void RemoveDatasource(string datasourceName)
        {
            T existingDatasource = Datasources.FirstOrDefault(datasource => datasource.Name.ToUpper() == datasourceName.ToUpper());
            if (existingDatasource != null)
                _datasources.Remove(existingDatasource);
        }

        /// <summary>
        /// Searches for datasources and populates the Datasources property.
        /// </summary>
        public virtual void PopulateDatasources()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for databases in all available datasources.
        /// </summary>
        public void PopulateDatabases()
        { 
            if (Datasources == null)
                return;

            foreach (IDatasource datasource in Datasources)
            {
                PopulateDatabases(datasource);
            }
        }

        /// <summary>
        /// Searches for databases in a specific datasource.
        /// </summary>
        /// <param name="datasource">Datasource</param>
        public virtual void PopulateDatabases(IDatasource datasource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads tables of a database.
        /// </summary>
        /// <param name="database">Database</param>
        /// <returns>Collection of ITables</returns>
        public virtual DatabaseObjectsCollection<ITable> LoadTables(IDatabase database)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads views of a database.
        /// </summary>
        /// <param name="database">Database</param>
        /// <returns>Collection of IViews</returns>
        public virtual DatabaseObjectsCollection<IView> LoadViews(IDatabase database)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads procedures of a database.
        /// </summary>
        /// <param name="database">Database</param>
        /// <returns>Collection of IProcedures</returns>
        public virtual DatabaseObjectsCollection<IProcedure> LoadProcedures(IDatabase database)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
