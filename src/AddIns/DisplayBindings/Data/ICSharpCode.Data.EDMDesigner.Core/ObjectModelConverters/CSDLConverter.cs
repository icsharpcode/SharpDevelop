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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.Interfaces;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.ObjectModelConverters
{
    internal class CSDLConverter
    {
        /*internal static EntityType CreateCSDLEntityType(ITable table)
        {
            EntityType entityType = new EntityType()
            {
                Name = table.TableName,
                EntitySetName = table.TableName, 
                Abstract = false,
                EntitySetVisibility = Visibility.Public,
            };

            foreach (IColumn column in table.Items)
            {
                PropertyBase property = CreateCSDLProperty(column, entityType);

                if (property is ComplexProperty)
                    entityType.ComplexProperties.Add((ComplexProperty)CreateCSDLProperty(column, entityType));
                else
                    entityType.ScalarProperties.Add((ScalarProperty)CreateCSDLProperty(column, entityType));
            }

            return entityType;
        }

        private static PropertyType GetScalarPropertyType(IColumn column)
        {
            return PropertyType.Binary;
            //switch (column.DataType)
            //{ 
            //    case "
            //}
        }

        private static PropertyBase CreateCSDLProperty(IColumn column, EntityType entityType)
        {
            PropertyBase property = null;
            
            // Complex property
            /*
            property = new ScalarProperty()
            {
                Name = column.Name,
                Type = CreateCSDLProperty(column, entityType),
                IsKey = column.IsIdentity,
                Nullable = column.IsNullable,
                MaxLength = column.Length,
                // FixedLength
                Precision = column.Precision,
                Scale = column.Scale,
                // Collation
                // DefaultValue
                // Unicode
            };

            if (column.IsIdentity)
                property.StoreGeneratedPattern = StoreGeneratedPattern.Identity;
            else if (column.IsComputed)
                property.StoreGeneratedPattern = StoreGeneratedPattern.Computed;
            else
                property.StoreGeneratedPattern = StoreGeneratedPattern.None;

            return property;
        }
        
        public static CSDLContainer CreateCSDLContainer(IDatabase database, string modelNamespace)
        {
            CSDLContainer csdlContainer = new CSDLContainer()
            {
                Name = database.Name.Replace(".", string.Empty) + "StoreContainer",
                Namespace = modelNamespace,
                Alias = "Self"
            };

            List<ITable> tablesAndViews = new List<ITable>();
            tablesAndViews.AddRange(database.Tables);
            tablesAndViews.AddRange(database.Views);

            foreach (ITable table in tablesAndViews)
            {
                EntityType entityType = CreateCSDLEntityType(table);
                csdlContainer.EntityTypes.Add(entityType);
            }

            return csdlContainer;
        }*/
    }
}
