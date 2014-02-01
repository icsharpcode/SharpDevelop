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
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.Data.Core.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Function;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using System.Collections.Generic;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class CSDLIO : IO
    {
        #region Methods

        #region Read

        public static CSDLContainer ReadXElement(XElement edmxRuntime)
        {
            XElement schemaElement = edmxRuntime.Element(XName.Get("ConceptualModels", edmxNamespace.NamespaceName)).Element(XName.Get("Schema", csdlNamespace.NamespaceName));
            
            if (schemaElement == null || schemaElement.IsEmpty)
            	return null;
            
            XElement entityContainerElement = schemaElement.Element(XName.Get("EntityContainer", csdlNamespace.NamespaceName));

            CSDLContainer csdlContainer = new CSDLContainer { Namespace = schemaElement.Attribute("Namespace").Value, Alias = schemaElement.Attribute("Alias").Value, Name = entityContainerElement.Attribute("Name").Value };

            #region EntityTypes
            while (true)
            {
                var typesEnumerator = (from ete in schemaElement.Elements(XName.Get("EntityType", csdlNamespace.NamespaceName))
                                       let eteName = ete.Attribute("Name").Value
                                       where !csdlContainer.EntityTypes.Any(et => et.Name == eteName)
                                       let baseTypeAttribute = ete.Attribute("BaseType")
                                       let baseType = baseTypeAttribute == null ? null : csdlContainer.EntityTypes.GetByName(GetName(baseTypeAttribute.Value))
                                       where baseTypeAttribute == null || baseType != null
                                       select new { EntityTypeElement = ete, Name = eteName, BaseType = baseType }).GetEnumerator();
                if (!typesEnumerator.MoveNext())
                    break;
                do
                {
                    var current = typesEnumerator.Current;
                    csdlContainer.EntityTypes.Add(ReadCSDLEntityType(schemaElement, entityContainerElement, current.EntityTypeElement, csdlContainer, current.Name, current.BaseType));
                } while (typesEnumerator.MoveNext());
            }
            #endregion EntityTypes

            #region Associations
            foreach (var association in csdlContainer.AssociationsCreated)
            {
                association.AssociationSetName = entityContainerElement.Elements(XName.Get("AssociationSet", csdlNamespace.NamespaceName)).First(ae => GetName(ae.Attribute("Association").Value) == association.Name).Attribute("Name").Value;
                if (association.PropertyEnd2.EntityType == null)
                {
                    var entityTypeName = schemaElement.Elements(XName.Get("Association", csdlNamespace.NamespaceName)).First(ae => ae.Attribute("Name").Value == association.Name).Elements(XName.Get("End", csdlNamespace.NamespaceName)).First(er => er.Attribute("Role").Value == association.PropertyEnd2Role).Attribute("Type").Value;
                    int dotIndex = entityTypeName.IndexOf(".");
                    if (dotIndex != -1)
                        entityTypeName = entityTypeName.Substring(dotIndex + 1);
                    var entityType = csdlContainer.EntityTypes.First(et => et.Name == entityTypeName); ;
                    entityType.NavigationProperties.Add(association.PropertyEnd2);
                }
            }
            #endregion Associations

            #region ComplexTypes
            foreach (var complexTypeElement in schemaElement.Elements(XName.Get("ComplexType", csdlNamespace.NamespaceName)))
            {
                var complexType = new ComplexType { Name = complexTypeElement.Attribute("Name").Value };
                ReadCSDLType(schemaElement, complexTypeElement, csdlContainer, complexType);
                csdlContainer.ComplexTypes.Add(complexType);
            }
            #endregion ComplexTypes

            #region Functions
            foreach (var functionElement in entityContainerElement.Elements(XName.Get("FunctionImport", csdlNamespace.NamespaceName)))
            {
                var function = new Function { Name = functionElement.Attribute("Name").Value };
                var returnTypeAttribute = functionElement.Attribute("ReturnType");
                if (returnTypeAttribute != null)
                {
                    var returnTypeValue = returnTypeAttribute.Value;
                    returnTypeValue = returnTypeValue.Remove(returnTypeValue.IndexOf(")")).Substring(returnTypeValue.IndexOf("(") + 1);
                    function.ScalarReturnType = GetScalarPropertyTypeFromAttribute(returnTypeValue);
                    if (function.ScalarReturnType == null)
                        function.EntityType = csdlContainer.EntityTypes.GetByName(GetName(returnTypeValue));
                }
                SetVisibilityValueFromAttribute(functionElement, "methodAccess", visibility => function.Visibility = visibility);

                #region Function parameters
                foreach (var parameterElement in functionElement.Elements(XName.Get("Parameter", csdlNamespace.NamespaceName)))
                {
                    var parameter = new FunctionParameter { Name = parameterElement.Attribute("Name").Value, Type = GetScalarPropertyTypeFromAttribute(parameterElement).Value };
                    SetEnumValueFromAttribute<ParameterMode>(parameterElement, "Mode", mode => parameter.Mode = mode);
                    SetIntValueFromAttribute(parameterElement, "Precision", precision => parameter.Precision = precision);
                    SetIntValueFromAttribute(parameterElement, "Scale", scale => parameter.Scale = scale);
                    SetIntValueFromAttribute(parameterElement, "MaxLength", maxLength => parameter.MaxLength = maxLength);
                    function.Parameters.Add(parameter);
                }
                #endregion Function parameters

                csdlContainer.Functions.Add(function);
            }
            #endregion Functions

            return csdlContainer;
        }

        private static EntityType ReadCSDLEntityType(XElement schemaElement, XElement entityContainerElement, XElement entityTypeElement, CSDLContainer container, string typeName, EntityType baseType)
        {
            var entityType = new EntityType { Name = typeName, BaseType = baseType };
            SetBoolValueFromAttribute(entityTypeElement, "Abstract", isAbstract => entityType.Abstract = isAbstract);
            var entitySetElement = entityContainerElement.Elements(XName.Get("EntitySet", csdlNamespace.NamespaceName)).FirstOrDefault(ese => GetName(ese.Attribute("EntityType").Value) == entityType.Name);
            if (entitySetElement != null)
            {
                entityType.EntitySetName = entitySetElement.Attribute("Name").Value;
                SetVisibilityValueFromAttribute(entitySetElement, "GetterAccess", getterAccess => entityType.EntitySetVisibility = getterAccess);
            }
            ReadCSDLType(schemaElement, entityTypeElement, container, (TypeBase)entityType);
            return entityType;
        }

        private static void ReadCSDLType(XElement schemaElement, XElement entityTypeElement, CSDLContainer container, TypeBase baseType)
        {
            if (baseType.Name == null)
                baseType.Name = entityTypeElement.Attribute("Name").Value;
            SetVisibilityValueFromAttribute(entityTypeElement, "TypeAccess", typeAccess => baseType.Visibility = typeAccess);

            foreach (var propertyElement in entityTypeElement.Elements(XName.Get("Property", csdlNamespace.NamespaceName)))
            {
                var name = propertyElement.Attribute("Name").Value;
                var keyElement = entityTypeElement.Element(XName.Get("Key", csdlNamespace.NamespaceName));
                var propertyType = GetScalarPropertyTypeFromAttribute(propertyElement);
                PropertyBase property;
                if (propertyType == null)
                {
                    property = new ComplexProperty(GetName(propertyElement.Attribute("Type").Value)) { Name = name };
                    baseType.ComplexProperties.Add((ComplexProperty)property);
                }
                else
                {
                    property = new ScalarProperty() { Name = name, IsKey = keyElement != null && keyElement.Elements(XName.Get("PropertyRef", csdlNamespace.NamespaceName)).Any(pr => pr.Attribute("Name").Value == name), Type = propertyType.Value };
                    var scalarProp = (ScalarProperty)property;
                    SetBoolValueFromAttribute(propertyElement, "Nullable", nullable => scalarProp.Nullable = nullable);
                    SetVisibilityValueFromAttribute(propertyElement, "SetterAccess", setterAccess => scalarProp.SetVisibility = setterAccess);
                    SetIntValueFromAttribute(propertyElement, "MaxLength", maxLength => scalarProp.MaxLength = maxLength);
                    SetBoolValueFromAttribute(propertyElement, "Unicode", unicode => scalarProp.Unicode = unicode);
                    SetBoolValueFromAttribute(propertyElement, "FixedLength", fixedLength => scalarProp.FixedLength = fixedLength);
                    SetIntValueFromAttribute(propertyElement, "Precision", precision => scalarProp.Precision = precision);
                    SetIntValueFromAttribute(propertyElement, "Scale", scale => scalarProp.Scale = scale);
                    SetStringValueFromAttribute(propertyElement, "ConcurrencyMode", concurrencyMode => scalarProp.ConcurrencyMode = ConcurrencyMode.None);
                    SetStringValueFromAttribute(propertyElement, "DefaultValue", defaultValue => scalarProp.DefaultValue = defaultValue);
                    SetStringValueFromAttribute(propertyElement, "Collation", collation => scalarProp.Collation = collation);
                    baseType.ScalarProperties.Add(scalarProp);
                }
                SetVisibilityValueFromAttribute(propertyElement, "GetterAccess", getterAccess => property.GetVisibility = getterAccess);
            }
            var entityType = baseType as EntityType;
            if (entityType != null)
            {
                foreach (var navigationPropertyElement in entityTypeElement.Elements(XName.Get("NavigationProperty", csdlNamespace.NamespaceName)))
                {
                    var navigationPropertyname = navigationPropertyElement.Attribute("Name").Value;
                    var associationName = GetName(navigationPropertyElement.Attribute("Relationship").Value);
                    var associationElement = schemaElement.Elements(XName.Get("Association", csdlNamespace.NamespaceName)).First(ae => ae.Attribute("Name").Value == associationName);
                    Association association = container.AssociationsCreated.GetByName(associationName);
                    bool associationExisting = association != null;
                    if (!associationExisting)
                    {
                        association = new Association { Name = associationName };
                        container.AssociationsCreated.Add(association);
                    }
                    var navigationProperty = new NavigationProperty(association) { Name = navigationPropertyname };
                    var roleName = navigationPropertyElement.Attribute("FromRole").Value;
                    SetCardinalityValueFromAttribute(associationElement.Elements(XName.Get("End", csdlNamespace.NamespaceName)).First(ee => ee.Attribute("Role").Value == roleName), cardinality => navigationProperty.Cardinality = cardinality);
                    SetVisibilityValueFromAttribute(navigationPropertyElement, "GetterAccess", visibility => navigationProperty.GetVisibility = visibility);
                    SetVisibilityValueFromAttribute(navigationPropertyElement, "SetterAccess", visibility => navigationProperty.SetVisibility = visibility);
                    if (associationExisting)
                    {
                        association.PropertyEnd2 = navigationProperty;
                        association.PropertyEnd2Role = roleName;
                    }
                    else
                    {
                        association.PropertyEnd1 = navigationProperty;
                        association.PropertyEnd1Role = roleName;
                        string toRoleName = navigationPropertyElement.Attribute("ToRole").Value;
                        NavigationProperty fakeNavigationProperty = new NavigationProperty(association) { Name = roleName, Generate = false };
                        SetCardinalityValueFromAttribute(associationElement.Elements(XName.Get("End", csdlNamespace.NamespaceName)).First(ee => ee.Attribute("Role").Value == toRoleName), cardinality => fakeNavigationProperty.Cardinality = cardinality);
                        association.PropertyEnd2 = fakeNavigationProperty;
                        association.PropertyEnd2Role = toRoleName;
                    }
                    var referentialConstraintElement = associationElement.Element(XName.Get("ReferentialConstraint", csdlNamespace.NamespaceName));
                    if (referentialConstraintElement != null)
                    {
                        var referentialConstraintRoleElement = referentialConstraintElement.Elements().First(rce => rce.Attribute("Role").Value == roleName);
                        var scalarProperties = referentialConstraintRoleElement.Elements(XName.Get("PropertyRef", csdlNamespace.NamespaceName)).Select(e => entityType.AllScalarProperties.First(sp => sp.Name == e.Attribute("Name").Value));
                        switch (referentialConstraintRoleElement.Name.LocalName)
                        {
                            case "Principal":
                                association.PrincipalRole = roleName;
                                association.PrincipalProperties = scalarProperties;
                                break;
                            case "Dependent":
                                association.DependentRole = roleName;
                                association.DependentProperties = scalarProperties;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    entityType.NavigationProperties.Add(navigationProperty);
                }
            }
        }

        private static void SetVisibilityValueFromAttribute(XElement element, string attribute, Action<Visibility> setAction)
        {
            SetEnumValueFromAttribute<Visibility>(element, attribute, csdlNamespace.NamespaceName, setAction);
        }

        private static PropertyType? GetScalarPropertyTypeFromAttribute(XElement element)
        {
            return GetScalarPropertyTypeFromAttribute(element.Attribute("Type").Value);
        }

        private static PropertyType? GetScalarPropertyTypeFromAttribute(string propertyTypeValue)
        {
            var values = Enum.GetValues(typeof(PropertyType)).Cast<object>().Where(v => v.ToString() == propertyTypeValue).Select(v => (PropertyType)v).Take(1).ToArray();
            if (values.Length == 0)
                return null;
            return values[0];
        }

        #endregion

        #region Write

        public static XElement Write(CSDLContainer csdlContainer)
        {
            // Instantiate Schema
            XElement schema = new XElement(csdlNamespace + "Schema",
                new XAttribute("Namespace", csdlContainer.Namespace),
                new XAttribute("Alias", csdlContainer.Alias),
                new XAttribute(XNamespace.Xmlns + "annotation", csdlAnnotationNamespace.NamespaceName),
                new XAttribute("xmlns", csdlNamespace.NamespaceName));

            // EntityContainer
            string entityContainerNamespace = string.Concat(csdlContainer.Namespace, ".");
            XElement entityContainer = new XElement(csdlNamespace + "EntityContainer", new XAttribute("Name", csdlContainer.Name));
            schema.Add(entityContainer);

            // EntityContainer : EntitySets
            foreach (EntityType entityType in csdlContainer.EntitySets)
            {
                XElement entitySetElement = new XElement(csdlNamespace + "EntitySet",
                    new XAttribute("Name", entityType.EntitySetName), 
                    new XAttribute("EntityType", string.Concat(entityContainerNamespace, entityType.Name)));
                    //.AddAttribute(csdlCodeGenerationNamespace, "GetterAccess", entityType.EntitySetVisibility); // Not available in EF 4.0

                entityContainer.Add(entitySetElement);
            }

            // EntityContainer : AssociationSets
            foreach (Association association in csdlContainer.Associations)
            {
                XElement associationSetElement = new XElement(csdlNamespace + "AssociationSet",
                    new XAttribute("Name", association.AssociationSetName),
                    new XAttribute("Association", string.Concat(entityContainerNamespace, association.Name)));

                associationSetElement.Add(
                    new XElement(csdlNamespace + "End", new XAttribute("Role", association.PropertyEnd1Role), new XAttribute("EntitySet", association.PropertyEnd1.EntityType.EntitySetName)),
                    new XElement(csdlNamespace + "End", new XAttribute("Role", association.PropertyEnd2Role), new XAttribute("EntitySet", association.PropertyEnd2.EntityType.EntitySetName)));

                entityContainer.AddElement(associationSetElement);
            }

            // EntityContainer : FunctionImports
            foreach (Function function in csdlContainer.Functions)
            {
                XElement functionElement = new XElement(csdlNamespace + "FunctionImport",
                    new XAttribute("Name", function.Name))
                    .AddAttribute("EntitySet", function.EntityType == null ? null : function.EntityType.EntitySetName)
                    .AddAttribute("ReturnType", function.ReturnType);

                foreach (FunctionParameter functionParameter in function.Parameters)
                {
                    functionElement.AddElement(new XElement(csdlNamespace + "Paramter",
                        new XAttribute("Name", functionParameter.Name),
                        new XAttribute("Type", functionParameter.Type))
                        .AddAttribute("MaxLength", functionParameter.MaxLength)
                        .AddAttribute("Mode", functionParameter.Mode)
                        .AddAttribute("Precision", functionParameter.Precision)
                        .AddAttribute("Scale", functionParameter.Scale));
                }

                entityContainer.AddElement(functionElement);
            }

            // ComplexTypes
            foreach (ComplexType complexType in csdlContainer.ComplexTypes)
            {
                XElement complexTypeElement = new XElement(csdlNamespace + "ComplexType",
                    new XAttribute("Name", complexType.Name));
                    //.AddAttribute(new XAttribute(csdlCodeGenerationNamespace + "TypeAccess", complexType.Visibility)); // Not available in EF 4.0

                complexTypeElement.Add(WriteScalarProperties(complexType));
                complexTypeElement.Add(WriteComplexProperties(complexType, string.Concat(csdlContainer.Alias, ".")));

                schema.AddElement(complexTypeElement);
            }

            // EntityTypes
            foreach (EntityType entityType in csdlContainer.EntityTypes)
            {
                XElement entityTypeElement = new XElement(csdlNamespace + "EntityType")
                    .AddAttribute("Name", entityType.Name)
                    //.AddAttribute(csdlCodeGenerationNamespace, "TypeAccess", entityType.Visibility) // Not available in EF 4.0
                    .AddAttribute("BaseType", entityType.BaseType == null ? null : string.Concat(entityContainerNamespace, entityType.BaseType.Name))
                    .AddAttribute("Abstract", entityType.Abstract);

                if (entityType.SpecificKeys.Any())
                {
                    XElement keyElement = new XElement(csdlNamespace + "Key");
                    
                    entityType.ScalarProperties.Where(sp => sp.IsKey).ForEach(scalarProperty =>
                    {
                        keyElement.AddElement(new XElement(csdlNamespace + "PropertyRef")
                            .AddAttribute("Name", scalarProperty.Name));                        
                    });

                    entityTypeElement.AddElement(keyElement);
                }

                entityTypeElement.Add(WriteScalarProperties(entityType));
                entityTypeElement.Add(WriteComplexProperties(entityType, string.Concat(csdlContainer.Alias, ".")));

                // EntityType : NavigationProperties
                entityType.NavigationProperties.Where(np => np.Generate).ForEach(navigationProperty =>
                {
                    entityTypeElement.AddElement(new XElement(csdlNamespace + "NavigationProperty")
                        .AddAttribute("Name", navigationProperty.Name)
                        .AddAttribute("Relationship", string.Concat(entityContainerNamespace, navigationProperty.Association.Name))
                        .AddAttribute("FromRole", navigationProperty.Association.GetRoleName(navigationProperty))
                        .AddAttribute("ToRole", navigationProperty.Association.GetRoleName(navigationProperty.Association.PropertiesEnd.First(role => role != navigationProperty))));
                        //.AddAttribute(csdlCodeGenerationNamespace, "GetterAccess", navigationProperty.GetVisibility) // Not available in EF 4.0
                        //.AddAttribute(csdlCodeGenerationNamespace, "SetterAccess", navigationProperty.SetVisibility));
                });

                schema.AddElement(entityTypeElement);
            }

            // Associations
            foreach (Association association in csdlContainer.Associations)
            { 
                XElement associationElement = new XElement(csdlNamespace + "Association")
                    .AddAttribute("Name", association.Name);
                    
                associationElement.AddElement(new XElement(csdlNamespace + "End")
                    .AddAttribute("Role", association.PropertyEnd1Role)
                    .AddAttribute("Type", string.Concat(entityContainerNamespace, association.PropertyEnd1.EntityType.Name))
                    .AddAttribute("Multiplicity", CardinalityStringConverter.CardinalityToString(association.PropertyEnd1.Cardinality)));

                associationElement.AddElement(new XElement(csdlNamespace + "End")
                    .AddAttribute("Role", association.PropertyEnd2Role)
                    .AddAttribute("Type", string.Concat(entityContainerNamespace, association.PropertyEnd2.EntityType.Name))
                    .AddAttribute("Multiplicity", CardinalityStringConverter.CardinalityToString(association.PropertyEnd2.Cardinality)));

                if (association.PrincipalRole != null)
                { 
                    XElement referentialConstraintElement = new XElement(csdlNamespace + "ReferentialConstraint");

                    XElement principalElement = (new XElement(csdlNamespace + "Principal")
                        .AddAttribute("Role", association.PrincipalRole));

                    foreach (ScalarProperty propertyRef in association.PrincipalProperties)
                        principalElement.AddElement(new XElement(csdlNamespace + "PropertyRef").AddAttribute("Name", propertyRef.Name));

                    XElement dependentElement = (new XElement(csdlNamespace + "Dependent")
                        .AddAttribute("Role", association.DependentRole));

                    foreach (ScalarProperty propertyRef in association.DependentProperties)
                        dependentElement.AddElement(new XElement(csdlNamespace + "PropertyRef").AddAttribute("Name", propertyRef.Name));

                    referentialConstraintElement.AddElement(principalElement);
                    referentialConstraintElement.AddElement(dependentElement);
                    associationElement.AddElement(referentialConstraintElement);
                }

                schema.AddElement(associationElement);
            }

            return schema;
        }

        private static IEnumerable<XElement> WriteScalarProperties(TypeBase type)
        {
            foreach (ScalarProperty scalarProperty in type.ScalarProperties)
            {
                yield return new XElement(csdlNamespace + "Property")
                    .AddAttribute("Name", scalarProperty.Name)
                    .AddAttribute("Type", scalarProperty.Type)
                    .AddAttribute("Collation", scalarProperty.Collation)
                    .AddAttribute("ConcurrencyMode", scalarProperty.ConcurrencyMode)
                    .AddAttribute("DefaultValue", scalarProperty.DefaultValue)
                    .AddAttribute("FixedLength", scalarProperty.FixedLength)
                    //.AddAttribute(csdlCodeGenerationNamespace, "GetterAccess", scalarProperty.GetVisibility)  // Not available in EF 4.0
                    .AddAttribute("MaxLength", scalarProperty.MaxLength)
                    .AddAttribute("Nullable", scalarProperty.Nullable)
                    .AddAttribute("Precision", scalarProperty.Precision)
                    .AddAttribute("Scale", scalarProperty.Scale)
                    //.AddAttribute(csdlCodeGenerationNamespace, "SetterAccess", scalarProperty.SetVisibility)  // Not available in EF 4.0
                    .AddAttribute("Unicode", scalarProperty.Unicode);
            }
        }

        private static IEnumerable<XElement> WriteComplexProperties(TypeBase type, string csdlAlias)
        {
            foreach (ComplexProperty complexProperty in type.ComplexProperties)
            {
                yield return new XElement(csdlNamespace + "Property")
                    .AddAttribute("Name", complexProperty.Name)
                    .AddAttribute("Type", string.Concat(csdlAlias, complexProperty.ComplexType.Name))
                    .AddAttribute("Nullable", false);
                    //.AddAttribute(csdlCodeGenerationNamespace, "GetterAccess", complexProperty.GetVisibility);  // Not available in EF 4.0
            }
        }

        #endregion

        #endregion
    }
}
