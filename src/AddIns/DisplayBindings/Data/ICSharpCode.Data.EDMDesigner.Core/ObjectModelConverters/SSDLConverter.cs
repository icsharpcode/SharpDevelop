// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#region Usings

using System.Collections.Generic;

using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.ObjectModelConverters
{
    internal class SSDLConverter
    {
        #region Public methods

        public static SSDLContainer CreateSSDLContainer(IDatabase database, string modelNamespace)
        {
            SSDLContainer ssdlContainer = new SSDLContainer()
            {
                Name = database.Name.Replace(".", string.Empty) + "StoreContainer",
                Namespace = modelNamespace,
                Provider = database.Datasource.DatabaseDriver.ProviderName,
                ProviderManifestToken = database.Datasource.ProviderManifestToken
            };

            List<ITable> tablesAndViews = new List<ITable>();
            tablesAndViews.AddRange(database.Tables);
            tablesAndViews.AddRange(database.Views);

            foreach (ITable table in tablesAndViews)
            {
                if (!table.IsSelected)
                    continue;

                EntityType entityType = CreateSSDLEntityType(table);
                ssdlContainer.EntityTypes.Add(entityType);

                if (table.Constraints != null)
                {
                    foreach (IConstraint constraint in table.Constraints)
                    {
                        if (constraint.FKTableName != table.TableName)
                            continue;

                        Association association = CreateSSDLAssociation(constraint);
                        ssdlContainer.AssociationSets.Add(association);
                    }
                }
            }

            foreach (IProcedure procedure in database.Procedures)
            {
                if (!procedure.IsSelected)
                    continue;
                
                Function function = CreateSSDLFunction(procedure);
                ssdlContainer.Functions.Add(function);
            }

            return ssdlContainer;
        }

        #endregion

        #region Private methods

        private static EntityType CreateSSDLEntityType(ITable table)
        {
            EntityType entityType = new EntityType()
            {
                Name = table.TableName,
                EntitySetName = table.TableName,
                Schema = table.SchemaName,
                StoreName = table.TableName,
                StoreSchema = table.SchemaName
            };

            if (table is IView)
            {
                entityType.StoreType = StoreType.Views;
                entityType.DefiningQuery = (table as IView).DefiningQuery;
            }
            else
                entityType.StoreType = StoreType.Tables;

            foreach (IColumn column in table.Items)
            {
                entityType.Properties.Add(CreateSSDLProperty(column, entityType));
            }

            return entityType;
        }

        private static Function CreateSSDLFunction(IProcedure procedure)
        {
            Function function = new Function()
            {
                Name = procedure.Name,
                Schema = procedure.SchemaName,
                Aggregate = false,
                BuiltIn = false,
                NiladicFunction = false,
                IsComposable = false,
                ParameterTypeSemantics = ParameterTypeSemantics.AllowImplicitConversion
            };

            foreach (IProcedureParameter procedureParameter in procedure.Items)
            {
                FunctionParameter functionParameter = new FunctionParameter()
                {
                    Name = procedureParameter.Name,
                    Type = procedureParameter.DataType,
                    Mode = (ParameterMode)procedureParameter.ParameterMode
                };

                function.Parameters.Add(functionParameter);
            }

            return function;
        }

        private static Property CreateSSDLProperty(IColumn column, EntityType entityType)
        {
            Property property = new Property(entityType)
            {
                Name = column.Name,
                IsKey = column.IsPrimaryKey,
                Nullable = column.IsNullable
                // FixedLength
                // Collation
                // DefaultValue
                // Unicode
            };

            if (!column.IsUserDefinedDataType)
                property.Type = column.DataType;
            else
                property.Type = column.SystemType;

            if (column.Length > 0)
                property.MaxLength = column.Length;

            //if (column.Precision != 0)
            //    property.Precision = column.Precision;

            //if (column.Scale != 0)
            //    property.Scale = column.Scale;

            if (column.IsIdentity)
                property.StoreGeneratedPattern = StoreGeneratedPattern.Identity;
            else if (column.IsComputed)
                property.StoreGeneratedPattern = StoreGeneratedPattern.Computed;

            return property;
        }

		private static Association CreateSSDLAssociation(IConstraint constraint)
		{
			string associationName = constraint.Name.Replace('.', '_');
			
			Association association = new Association()
			{
				Name = associationName,
				AssociationSetName = associationName
			};

            Role role1 = new Role()
            {
                Name = constraint.PKTableName,
                Cardinality = (Cardinality)constraint.PKCardinality
            };

            foreach (IColumn pkColumn in constraint.PKColumns)
            {
                Property role1Property = CreateSSDLProperty(pkColumn, CreateSSDLEntityType(constraint.PKTable));
                role1.Properties.Add(role1Property);
                role1.Type = role1Property.EntityType;
            }

            association.Role1 = role1;
            association.PrincipalRole = role1;

            Role role2 = new Role()
            {
                Name = constraint.FKTableName,
                Cardinality = (Cardinality)constraint.FKCardinality
            };

            foreach (IColumn fkColumn in constraint.FKColumns)
            {
                Property role2Property = CreateSSDLProperty(fkColumn, CreateSSDLEntityType(constraint.FKTable));
                role2.Properties.Add(role2Property);
                role2.Type = role2Property.EntityType;
            }

            association.Role2 = role2;
            association.DependantRole = role2;

            return association;
        }

        #endregion
    }
}
