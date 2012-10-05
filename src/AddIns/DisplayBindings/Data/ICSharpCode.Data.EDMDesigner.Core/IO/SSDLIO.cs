// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class SSDLIO : IO
    {
        public static XDocument WriteXDocument(SSDLContainer ssdlContainer)
        {
            return new XDocument(new XDeclaration("1.0", "utf-8", null), WriteXElement(ssdlContainer));
        }

        public static XElement WriteXElement(SSDLContainer ssdlContainer)
        {
            // Instantiate Schema
            XElement schema = new XElement(ssdlNamespace + "Schema",
                new XAttribute("Namespace", ssdlContainer.Namespace), 
                new XAttribute("Alias", "Self"),
                new XAttribute(XNamespace.Xmlns + "store", storeNamespace.NamespaceName),
                new XAttribute("xmlns", ssdlNamespace.NamespaceName))
                .AddAttribute("Provider", ssdlContainer.Provider)
                .AddAttribute("ProviderManifestToken", ssdlContainer.ProviderManifestToken);
            
            // EntityContainer
            string entityContainerNamespace = string.Concat(ssdlContainer.Namespace, ".");
            XElement entityContainer = new XElement(ssdlNamespace + "EntityContainer", new XAttribute("Name", ssdlContainer.Name));
            schema.Add(entityContainer);

            // EntityContainer : EntitySets
            foreach (EntityType entityType in ssdlContainer.EntityTypes)
            {
                XElement entitySet = new XElement(ssdlNamespace + "EntitySet",
                    new XAttribute("Name", entityType.EntitySetName), new XAttribute("EntityType", string.Concat(entityContainerNamespace, entityType.Name)))
                        .AddAttribute(entityType.StoreType == StoreType.Views ? null : new XAttribute("Schema", entityType.Schema))
                        .AddAttribute("Table", entityType.Table)
                        .AddAttribute(storeNamespace, "Name", entityType.StoreName)
                        .AddAttribute(storeNamespace, "Schema", entityType.StoreSchema)
                        .AddAttribute(storeNamespace, "Type", entityType.StoreType)
                        .AddElement(string.IsNullOrEmpty(entityType.DefiningQuery) ? null : new XElement(ssdlNamespace + "DefiningQuery", entityType.DefiningQuery));

                entityContainer.Add(entitySet);
            }

            // EntityContainer : Associations
            foreach (Association association in ssdlContainer.AssociationSets)
            {
                XElement associationSet = new XElement(ssdlNamespace + "AssociationSet", 
                    new XAttribute("Name", association.AssociationSetName), new XAttribute("Association", string.Concat(entityContainerNamespace, association.Name)));

                string role2Name = association.Role2.Name;

                // If the association end properties are the same properties
                if (association.Role1.Name == association.Role2.Name && association.Role1.Type.Name == association.Role2.Type.Name)
                    role2Name += "1";

                associationSet.Add(
                        new XElement(ssdlNamespace + "End", new XAttribute("Role", association.Role1.Name), new XAttribute("EntitySet", association.Role1.Type.Name)),
                        new XElement(ssdlNamespace + "End", new XAttribute("Role", role2Name), new XAttribute("EntitySet", association.Role2.Type.Name)));
            
                entityContainer.Add(associationSet);
            }

            // EntityTypes
            foreach (EntityType entityType in ssdlContainer.EntityTypes)
            {
                XElement entityTypeElement = new XElement(ssdlNamespace + "EntityType", new XAttribute("Name", entityType.Name));

                XElement keys = new XElement(ssdlNamespace + "Key");

                foreach (Property property in entityType.Properties)
                {
                    // If we have a table then we set a key element if the current property is a primary key or part of a composite key.
                    // AND: If we have a view then we make a composite key of all non-nullable properties of the entity (VS2010 is also doing this).
                    if ((entityType.StoreType == StoreType.Tables && property.IsKey) || (entityType.StoreType == StoreType.Views && property.Nullable == false))
                        keys.Add(new XElement(ssdlNamespace + "PropertyRef", new XAttribute("Name", property.Name)));
                }

                if (!keys.IsEmpty)
                    entityTypeElement.Add(keys);

                foreach (Property property in entityType.Properties)
                {
                    entityTypeElement.Add(new XElement(ssdlNamespace + "Property", new XAttribute("Name", property.Name), new XAttribute("Type", property.Type))
                        .AddAttribute("Collation", property.Collation)
                        .AddAttribute("DefaultValue", property.DefaultValue)
                        .AddAttribute("FixedLength", property.FixedLength)
                        .AddAttribute("MaxLength", property.MaxLength)
                        .AddAttribute("Nullable", property.Nullable)
                        .AddAttribute("Precision", property.Precision)
                        .AddAttribute("Scale", property.Scale)
                        .AddAttribute("StoreGeneratedPattern", property.StoreGeneratedPattern)
                        .AddAttribute(storeNamespace, "Name", property.StoreName)
                        .AddAttribute(storeNamespace, "Schema", property.StoreSchema)
                        .AddAttribute(storeNamespace, "Type", property.StoreType)
                        .AddAttribute("Unicode", property.Unicode));
                }

                schema.AddElement(entityTypeElement);
            }

            foreach (Association association in ssdlContainer.AssociationSets)
            {
                string role2Name = association.Role2.Name;

                // If the association end properties are the same properties
                if (association.Role1.Name == association.Role2.Name && association.Role1.Type.Name == association.Role2.Type.Name)
                    role2Name += "1";
                
                XElement associationElement = new XElement(ssdlNamespace + "Association", new XAttribute("Name", association.Name),
                        new XElement(ssdlNamespace + "End", new XAttribute("Role", association.Role1.Name), new XAttribute("Type", string.Concat(entityContainerNamespace, association.Role1.Type.Name)), new XAttribute("Multiplicity", CardinalityStringConverter.CardinalityToString(association.Role1.Cardinality))),
                        new XElement(ssdlNamespace + "End", new XAttribute("Role", role2Name), new XAttribute("Type", string.Concat(entityContainerNamespace, association.Role2.Type.Name)), new XAttribute("Multiplicity", CardinalityStringConverter.CardinalityToString(association.Role2.Cardinality))));

                string dependentRoleName = association.DependantRole.Name;

                // If the association end properties are the same properties
                if (association.PrincipalRole.Name == association.DependantRole.Name && association.PrincipalRole.Type.Name == association.DependantRole.Type.Name)
                    dependentRoleName += "1";

                XElement principalRoleElement = new XElement(ssdlNamespace + "Principal", new XAttribute("Role", association.PrincipalRole.Name));
                foreach (Property property in association.PrincipalRole.Properties)
                    principalRoleElement.Add(new XElement(ssdlNamespace + "PropertyRef", new XAttribute("Name", property.Name)));

                XElement dependentRoleElement = new XElement(ssdlNamespace + "Dependent", new XAttribute("Role", dependentRoleName));
                foreach (Property property in association.DependantRole.Properties)
                    dependentRoleElement.Add(new XElement(ssdlNamespace + "PropertyRef", new XAttribute("Name", property.Name)));

                XElement referentialConstraintElement = new XElement(ssdlNamespace + "ReferentialConstraint", principalRoleElement, dependentRoleElement);
                associationElement.Add(referentialConstraintElement);

                schema.Add(associationElement);
            }

            foreach (Function function in ssdlContainer.Functions)
            {
                XElement functionElement = new XElement(ssdlNamespace + "Function", new XAttribute("Name", function.Name))
                    .AddAttribute("Aggregate", function.Aggregate)
                    .AddAttribute("BuiltIn", function.BuiltIn)
                    .AddAttribute("CommandText", function.CommandText)
                    .AddAttribute("IsComposable", function.IsComposable)
                    .AddAttribute("NiladicFunction", function.NiladicFunction)
                    .AddAttribute("ReturnType", function.ReturnType)
                    .AddAttribute("StoreFunctionName", function.StoreFunctionName)
                    .AddAttribute("ParameterTypeSemantics", function.ParameterTypeSemantics)
                    .AddAttribute("Schema", function.Schema)
                    .AddAttribute(storeNamespace, "Name", function.StoreName)
                    .AddAttribute(storeNamespace, "Schema", function.StoreSchema)
                    .AddAttribute(storeNamespace, "Type", function.StoreType);

                foreach (FunctionParameter functionParameter in function.Parameters)
                {
                    functionElement.Add(new XElement(ssdlNamespace + "Parameter", 
                        new XAttribute("Name", functionParameter.Name), new XAttribute("Type", functionParameter.Type), new XAttribute("Mode", functionParameter.Mode))
                            .AddAttribute("MaxLength", functionParameter.MaxLength)
                            .AddAttribute("Precision", functionParameter.Precision)
                            .AddAttribute("Scale", functionParameter.Scale)
                            .AddAttribute(storeNamespace, "Name", functionParameter.StoreName)
                            .AddAttribute(storeNamespace, "Schema", functionParameter.StoreSchema)
                            .AddAttribute(storeNamespace, "Type", functionParameter.StoreType));
                }

                schema.Add(functionElement);
            }

            return schema;
        }

        public static SSDLContainer ReadXElement(XElement ssdlXElement)
        {
            XElement schemaElement = ssdlXElement.Element(XName.Get("StorageModels", edmxNamespace.NamespaceName)).Element(XName.Get("Schema", ssdlNamespace.NamespaceName));
            
            if (schemaElement == null || schemaElement.IsEmpty)
            	return null;
            
            XElement entityContainerElement = schemaElement.Element(XName.Get("EntityContainer", ssdlNamespace.NamespaceName));

            var value = new SSDLContainer { Namespace = schemaElement.Attribute("Namespace").Value, Name = entityContainerElement.Attribute("Name").Value };
            SetStringValueFromAttribute(schemaElement, "Provider", provider => value.Provider = provider);
            SetStringValueFromAttribute(schemaElement, "ProviderManifestToken", provider => value.ProviderManifestToken = provider);

            #region EntitySets

            foreach (var entitySetElement in entityContainerElement.Elements(XName.Get("EntitySet", ssdlNamespace.NamespaceName)))
            {
                var entityType = new EntityType { EntitySetName = entitySetElement.Attribute("Name").Value };
                var entityTypeName = GetName(entitySetElement.Attribute("EntityType").Value);
                entityType.Name = entityTypeName;
                SetEnumValueFromAttribute<StoreType>(entitySetElement, "Type", storeNamespace.NamespaceName, storeType => entityType.StoreType = storeType);
                SetStringValueFromAttribute(entitySetElement, "Schema", schema => entityType.Schema = schema);
                SetStringValueFromAttribute(entitySetElement, "Name", storeNamespace.NamespaceName, storeName => entityType.StoreName = storeName);
                SetStringValueFromAttribute(entitySetElement, "Schema", storeNamespace.NamespaceName, storeSchema => entityType.StoreSchema = storeSchema);
                SetStringValueFromAttribute(entitySetElement, "Table", table => entityType.Table = table);
                SetStringValueFromElement(entitySetElement, "DefiningQuery", ssdlNamespace.NamespaceName, query => entityType.DefiningQuery = query);

                #region Properties
                var entityTypeElement = schemaElement.Elements(XName.Get("EntityType", ssdlNamespace.NamespaceName)).First(etElement => etElement.Attribute("Name").Value == entityTypeName);
                foreach (var propertyElement in entityTypeElement.Elements(XName.Get("Property", ssdlNamespace.NamespaceName)))
                {
                    var name = propertyElement.Attribute("Name").Value;
                    var property = new Property(entityType) { Name = name, Type = propertyElement.Attribute("Type").Value, IsKey = entityTypeElement.Element(XName.Get("Key", ssdlNamespace.NamespaceName)).Elements(XName.Get("PropertyRef", ssdlNamespace.NamespaceName)).Any(pr => pr.Attribute("Name").Value == name) };
                    SetBoolValueFromAttribute(propertyElement, "Nullable", nullable => property.Nullable = nullable);
                    SetIntValueFromAttribute(propertyElement, "MaxLength", maxLength => property.MaxLength = maxLength);
                    SetBoolValueFromAttribute(propertyElement, "FixedLength", fixedLength => property.FixedLength = fixedLength);
                    SetIntValueFromAttribute(propertyElement, "Precision", precision => property.Precision = precision);
                    SetIntValueFromAttribute(propertyElement, "Scale", scale => property.Scale = scale);
                    SetStringValueFromAttribute(propertyElement, "Collation", collation => property.Collation = collation);
                    SetStringValueFromAttribute(propertyElement, "DefaultValue", defaultValue => property.DefaultValue = defaultValue);
                    SetBoolValueFromAttribute(propertyElement, "Unicode", unicode => property.Unicode = unicode);
                    SetEnumValueFromAttribute<StoreGeneratedPattern>(propertyElement, "StoreGeneratedPattern", storeGeneratedPattern => property.StoreGeneratedPattern = storeGeneratedPattern);
                    SetStringValueFromAttribute(propertyElement, "Name", storeNamespace.NamespaceName, storeName => property.StoreName = storeName);
                    SetStringValueFromAttribute(propertyElement, "Schema", storeNamespace.NamespaceName, storeSchema => property.StoreSchema = storeSchema);
                    SetStringValueFromAttribute(propertyElement, "Type", storeNamespace.NamespaceName, storeType => property.StoreType = storeType);
                    entityType.Properties.Add(property);
                }
                #endregion Properties

                value.EntityTypes.Add(entityType);
            }

            #endregion EntitySets

            #region AssociationSets

            foreach (var associationSetElement in entityContainerElement.Elements(XName.Get("AssociationSet", ssdlNamespace.NamespaceName)))
            {
                var association = new Association { AssociationSetName = associationSetElement.Attribute("Name").Value };
                var associationName = GetName(associationSetElement.Attribute("Association").Value);
                association.Name = associationName;

                #region Roles
                var associationElement = schemaElement.Elements(XName.Get("Association", ssdlNamespace.NamespaceName)).First(aElement => aElement.Attribute("Name").Value == associationName);
                bool isPrincipal = true;
                foreach (var roleElement in associationSetElement.Elements(XName.Get("End", ssdlNamespace.NamespaceName)))
                {
                    var roleName = roleElement.Attribute("Role").Value;
                    var role = new Role { Name = roleName, Type = value.EntityTypes.GetByName(roleElement.Attribute("EntitySet").Value) };
                    SetCardinalityValueFromAttribute(associationElement.Elements(XName.Get("End", ssdlNamespace.NamespaceName)).First(are => are.Attribute("Role").Value == roleName), cardinality => role.Cardinality = cardinality);
                    var referentialConstraintElement = associationElement.Element(XName.Get("ReferentialConstraint", ssdlNamespace.NamespaceName));
                    if (referentialConstraintElement != null)
                    {
                        var principalElement = referentialConstraintElement.Element(XName.Get("Principal", ssdlNamespace.NamespaceName));
                        if (principalElement.Attribute("Role").Value == role.Name)
                        {
                            isPrincipal = true;
                            association.PrincipalRole = role;

                            EventedObservableCollection<Property> properties = new EventedObservableCollection<Property>();

                            foreach (XElement element in principalElement.Elements(XName.Get("PropertyRef", ssdlNamespace.NamespaceName)))
                            {
                                foreach (Property property in role.Type.Properties)
                                {
                                    if (property.Name == element.Attribute("Name").Value)
                                        properties.Add(property);
                                }
                            }

                            role.Properties = properties;
                        }
                        else
                        {
                            isPrincipal = false;
                            association.DependantRole = role;

                            EventedObservableCollection<Property> properties = new EventedObservableCollection<Property>();
                            XElement dependentElement = referentialConstraintElement.Element(XName.Get("Dependent", ssdlNamespace.NamespaceName));

                            foreach (XElement element in dependentElement.Elements(XName.Get("PropertyRef", ssdlNamespace.NamespaceName)))
                            {
                                foreach (Property property in role.Type.Properties)
                                {
                                    if (property.Name == element.Attribute("Name").Value)
                                        properties.Add(property);
                                }
                            }

                            role.Properties = properties;
                        }
                    }
                    if (isPrincipal)
                    {
                        association.Role1 = role;
                        isPrincipal = false;
                    }
                    else
                        association.Role2 = role;
                }

                #endregion Roles

                value.AssociationSets.Add(association);
            }

            #endregion AssociationSets

            #region Functions

            foreach (var functionElement in schemaElement.Elements(XName.Get("Function", ssdlNamespace.NamespaceName)))
            {
                var function = new Function { Name = functionElement.Attribute("Name").Value };
                SetBoolValueFromAttribute(functionElement, "Aggregate", aggregate => function.Aggregate = aggregate);
                SetBoolValueFromAttribute(functionElement, "BuiltIn", builtIn => function.BuiltIn = builtIn);
                SetBoolValueFromAttribute(functionElement, "IsComposable", isComposable => function.IsComposable = isComposable);
                SetBoolValueFromAttribute(functionElement, "NiladicFunction", niladicFunction => function.NiladicFunction = niladicFunction);
                SetStringValueFromAttribute(functionElement, "Schema", schema => function.Schema = schema);
                SetStringValueFromAttribute(functionElement, "Name", storeNamespace.NamespaceName, name => function.StoreName = name);
                SetStringValueFromAttribute(functionElement, "Schema", storeNamespace.NamespaceName, schema => function.StoreSchema = schema);
                SetStringValueFromAttribute(functionElement, "Type", storeNamespace.NamespaceName, type => function.StoreType = type);
                SetStringValueFromAttribute(functionElement, "FunctionName", storeNamespace.NamespaceName, functionName => function.StoreFunctionName = functionName);
                SetEnumValueFromAttribute<ParameterTypeSemantics>(functionElement, "ParameterTypeSemantics", parameterTypeSemantics => function.ParameterTypeSemantics = parameterTypeSemantics);

                #region Parameters

                foreach (var parameterElement in functionElement.Elements(XName.Get("Parameter", ssdlNamespace.NamespaceName)))
                {
                    var parameter = new FunctionParameter { Name = parameterElement.Attribute("Name").Value, Type = parameterElement.Attribute("Type").Value };
                    SetEnumValueFromAttribute<ParameterMode>(parameterElement, "Mode", mode => parameter.Mode = mode);
                    SetIntValueFromAttribute(parameterElement, "MaxLength", maxLength => parameter.MaxLength = maxLength);
                    SetIntValueFromAttribute(parameterElement, "Precision", precision => parameter.Precision = precision);
                    SetIntValueFromAttribute(parameterElement, "Scale", scale => parameter.Scale = scale);
                    SetStringValueFromAttribute(parameterElement, "Name", storeNamespace.NamespaceName, name => parameter.StoreName = name);
                    SetStringValueFromAttribute(parameterElement, "Schema", storeNamespace.NamespaceName, schema => parameter.StoreSchema = schema);
                    SetStringValueFromAttribute(parameterElement, "Type", storeNamespace.NamespaceName, type => parameter.StoreType = type);

                    function.Parameters.Add(parameter);
                }

                #endregion Parameters

                value.Functions.Add(function);
            }

            #endregion Functions

            return value;
        }
    }
}
