#region Usings

using System;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Function;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class CSDLIO : IO
    {
        #region Methods

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

        #endregion

        private static EntityType ReadCSDLEntityType(XElement schemaElement, XElement entityContainerElement, XElement entityTypeElement, CSDLContainer container, string typeName, EntityType baseType)
        {
            var entityType = new EntityType { Name = typeName, BaseType = baseType };
            SetBoolValueFromAttribute(entityTypeElement, "Abstract", isAbstract => entityType.Abstract = isAbstract);
            var entitySetElement = entityContainerElement.Elements(XName.Get("EntitySet", csdlNamespace.NamespaceName)).Where(ese => GetName(ese.Attribute("EntityType").Value) == entityType.Name).FirstOrDefault();
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
                    property = new ScalarProperty() { Name = name, IsKey = keyElement != null && keyElement.Elements(XName.Get("PropertyRef", csdlNamespace.NamespaceName)).Where(pr => pr.Attribute("Name").Value == name).Any(), Type = propertyType.Value };
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
    }
}
