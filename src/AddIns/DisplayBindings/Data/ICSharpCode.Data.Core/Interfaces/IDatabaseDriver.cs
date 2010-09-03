// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ICSharpCode.Data.Core.DatabaseObjects;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface IDatabaseDriver : IDatabaseObjectBase
    {
        DatabaseObjectsCollection<IDatasource> IDatasources { get; }
        string ProviderName { get; }
        string ODBCProviderName { get; }

        IDatasource AddNewDatasource(string datasourceName);
        IDatasource CreateNewIDatasource(string name);
  
        void RemoveDatasource(string datasourceName);
        DatabaseObjectsCollection<ITable> LoadTables(IDatabase database);
        DatabaseObjectsCollection<IView> LoadViews(IDatabase database);
        DatabaseObjectsCollection<IProcedure> LoadProcedures(IDatabase database);
        void PopulateDatasources();
        void PopulateDatabases();
        void PopulateDatabases(IDatasource datasource);
    }

    public interface IDatabaseDriver<T> : IDatabaseDriver where T : IDatasource
    {
        T CreateNewDatasource(string name);
        DatabaseObjectsCollection<T> Datasources { get; }
    }
}
