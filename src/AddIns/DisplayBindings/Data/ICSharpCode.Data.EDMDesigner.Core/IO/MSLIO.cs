// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class MSLIO : IO
    {
        #region Methods
        
        public static XDocument GenerateTypeMapping(XDocument mslDocument)
        {
            XElement mappingElement = mslDocument.Element(XName.Get("Mapping", mslNamespace.NamespaceName));

            if (mappingElement == null || mappingElement.IsEmpty)
                return null;

            XElement entityContainerMappingElement = mappingElement.Element(XName.Get("EntityContainerMapping", mslNamespace.NamespaceName));

            if (entityContainerMappingElement == null || entityContainerMappingElement.IsEmpty)
                return null;

            foreach (XElement entitySetMapping in entityContainerMappingElement.Elements(mslNamespace + "EntitySetMapping"))
            {
                string name = entitySetMapping.Attribute("Name").Value;
                string storeEntitySet = entitySetMapping.Attribute("StoreEntitySet").Value;
                string typeName = entitySetMapping.Attribute("TypeName").Value;

                XElement entityTypeMapping = new XElement(mslNamespace + "EntityTypeMapping", new XAttribute("TypeName", string.Format("IsTypeOf({0})", typeName)));
                XElement mappingFragment = null;
                entityTypeMapping.Add(mappingFragment = new XElement(mslNamespace + "MappingFragment", new XAttribute("StoreEntitySet", storeEntitySet)));

                foreach (XElement property in entitySetMapping.Elements())
                    mappingFragment.Add(property);

                entitySetMapping.RemoveAll();
                entitySetMapping.AddAttribute("Name", name);
                entitySetMapping.Add(entityTypeMapping);
            }

            return mslDocument;
        }

        public static CSDLContainer IntegrateMSLInCSDLContainer(CSDLContainer csdlContainer, SSDLContainer ssdlContainer, XElement edmxRuntime)
        {
            XElement mappingsElement = edmxRuntime.Element(XName.Get("Mappings", edmxNamespace.NamespaceName));
            
            if (mappingsElement == null || mappingsElement.IsEmpty)
            	return null;
            
            XElement mappingElement = mappingsElement.Element(XName.Get("Mapping", mslNamespace.NamespaceName));
            
            if (mappingElement == null || mappingElement.IsEmpty)
            	return null;

            XElement entityContainerMappingElement = mappingElement.Element(XName.Get("EntityContainerMapping", mslNamespace.NamespaceName));
            
            if (entityContainerMappingElement == null || entityContainerMappingElement.IsEmpty)
            	return null;
            
            #region EntityTypes
            
            foreach (var entityTypeMappingElement in entityContainerMappingElement.Elements(XName.Get("EntitySetMapping", mslNamespace.NamespaceName)).SelectMany(entitySetElement => entitySetElement.Elements(XName.Get("EntityTypeMapping", mslNamespace.NamespaceName))))
            {
                var typeName = entityTypeMappingElement.Attribute("TypeName").Value;
                if (typeName.IndexOf("(") != -1)
                    typeName = typeName.Remove(typeName.IndexOf(")")).Substring(typeName.IndexOf("(") + 1);
                var entityType = csdlContainer.EntityTypes.GetByName(GetName(typeName));
                entityType.Mapping.BeginInit();
                var mapping = entityType.Mapping;
                foreach (var mappingFragmentElement in entityTypeMappingElement.Elements(XName.Get("MappingFragment", mslNamespace.NamespaceName)))
                {
                    var table = ssdlContainer.EntityTypes.GetByName(mappingFragmentElement.Attribute("StoreEntitySet").Value);
                    foreach (var scalarPropertyElement in mappingFragmentElement.Elements(XName.Get("ScalarProperty", mslNamespace.NamespaceName)))
                    {
                        var scalarProperty = entityType.AllScalarProperties.GetByName(scalarPropertyElement.Attribute("Name").Value);
                        var column = table.Properties.GetByName(scalarPropertyElement.Attribute("ColumnName").Value);
                        entityType.Mapping[scalarProperty, table] = column;
                    }

                    MapComplexProperties(mappingFragmentElement, mapping, entityType, table);

                    #region Conditions
                    
                    foreach (var conditionElement in mappingFragmentElement.Elements(XName.Get("Condition", mslNamespace.NamespaceName)))
                    {
                        var columnNameAttribute = conditionElement.Attribute("ColumnName");
                        ConditionMapping condition;
                        if (columnNameAttribute == null)
                            condition = new PropertyConditionMapping { CSDLProperty = entityType.ScalarProperties.GetByName(conditionElement.Attribute("Name").Value), Table = table };
                        else
                            condition = new ColumnConditionMapping { Column = table.Properties.GetByName(columnNameAttribute.Value) };
                        var valueAttribute = conditionElement.Attribute("Value");
                        if (valueAttribute == null)
                        {
                            if (conditionElement.Attribute("IsNull").Value == "false")
                                condition.Operator = ConditionOperator.IsNotNull;
                            else
                                condition.Operator = ConditionOperator.IsNull;
                        }
                        else
                        {
                            condition.Operator = ConditionOperator.Equals;
                            condition.Value = valueAttribute.Value;
                        }
                        mapping.ConditionsMapping.Add(condition);
                    }
                    
                    #endregion Conditions
                }
                mapping.Init = true;
                #region CUD Functions
                var modificationFunctionMappingElement = entityTypeMappingElement.Element(XName.Get("ModificationFunctionMapping", mslNamespace.NamespaceName));
                if (modificationFunctionMappingElement != null)
                {
                    var insertFunctionMappingElement = modificationFunctionMappingElement.Element(XName.Get("InsertFunction", mslNamespace.NamespaceName));
                    if (insertFunctionMappingElement != null)
                        mapping.InsertFunctionMapping = SetCUDFunctionMapping(ssdlContainer, entityType, insertFunctionMappingElement);
                    var updateFunctionMappingElement = modificationFunctionMappingElement.Element(XName.Get("UpdateFunction", mslNamespace.NamespaceName));
                    if (updateFunctionMappingElement != null)
                        mapping.UpdateFunctionMapping = SetCUDFunctionMapping(ssdlContainer, entityType, updateFunctionMappingElement);
                    var deleteFunctionMappingElement = modificationFunctionMappingElement.Element(XName.Get("DeleteFunction", mslNamespace.NamespaceName));
                    if (deleteFunctionMappingElement != null)
                        mapping.DeleteFunctionMapping = SetCUDFunctionMapping(ssdlContainer, entityType, deleteFunctionMappingElement);
                }
                #endregion CUD Functions

                entityType.Mapping.EndInit();
            }
            #endregion EntityTypes

            #region Associations
            foreach (var associationSetMappingElement in entityContainerMappingElement.Elements(XName.Get("AssociationSetMapping", mslNamespace.NamespaceName)))
            {
                var csdlAssociation = csdlContainer.AssociationsCreated.GetByName(GetName(associationSetMappingElement.Attribute("TypeName").Value));
                csdlAssociation.Mapping.BeginInit();
                var table = ssdlContainer.EntityTypes.GetByName(associationSetMappingElement.Attribute("StoreEntitySet").Value);
                foreach (var endPropertyElement in associationSetMappingElement.Elements(XName.Get("EndProperty", mslNamespace.NamespaceName)))
                {
                    NavigationProperty navigationProperty;
                    if (csdlAssociation.PropertyEnd1Role == endPropertyElement.Attribute("Name").Value)
                        navigationProperty = csdlAssociation.PropertyEnd1;
                    else
                        navigationProperty = csdlAssociation.PropertyEnd2;
                    var navigationPropertyMapping = navigationProperty.Mapping;
                    foreach (var scalarPropertyElement in endPropertyElement.Elements(XName.Get("ScalarProperty", mslNamespace.NamespaceName)))
                    {
                        var scalarProperty = navigationProperty.EntityType.AllScalarProperties.GetByName(scalarPropertyElement.Attribute("Name").Value);
                        var column = table.Properties.GetByName(scalarPropertyElement.Attribute("ColumnName").Value);
                        navigationPropertyMapping[scalarProperty] = column;
                    }
                }
                foreach (var conditionElement in associationSetMappingElement.Elements(XName.Get("Condition", mslNamespace.NamespaceName)))
                {
                    var columnNameAttribute = conditionElement.Attribute("ColumnName");
                    ColumnConditionMapping condition = new ColumnConditionMapping { Column = table.Properties.GetByName(columnNameAttribute.Value) }; ;
                    var valueAttribute = conditionElement.Attribute("Value");
                    if (valueAttribute == null)
                    {
                        if (conditionElement.Attribute("IsNull").Value == "false")
                            condition.Operator = ConditionOperator.IsNotNull;
                        else
                            condition.Operator = ConditionOperator.IsNull;
                    }
                    else
                    {
                        condition.Operator = ConditionOperator.Equals;
                        condition.Value = valueAttribute.Value;
                    }
                    csdlAssociation.Mapping.ConditionsMapping.Add(condition);
                }
                csdlAssociation.Mapping.SSDLTableMapped = table;
                csdlAssociation.Mapping.EndInit();
            }
            #endregion Associations

            #region Functions
            foreach (var functionMappingElement in entityContainerMappingElement.Elements(XName.Get("FunctionImportMapping", mslNamespace.NamespaceName)))
                csdlContainer.Functions.GetByName(functionMappingElement.Attribute("FunctionImportName").Value).SSDLFunction = ssdlContainer.Functions.GetByName(GetName(functionMappingElement.Attribute("FunctionName").Value));
            #endregion Funtions

            return csdlContainer;
        }

        private static void MapComplexProperties(XElement complexMappingParentElement, MappingBase complexMappingOwner, TypeBase type, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType table)
        {
            foreach (var complexPropertyElement in complexMappingParentElement.Elements(XName.Get("ComplexProperty", mslNamespace.NamespaceName)))
            {
                var complexProperty = type.AllComplexProperties.GetByName(complexPropertyElement.Attribute("Name").Value);
                var complexPropertyMapping = complexMappingOwner[complexProperty];
                foreach (var scalarPropertyElement in complexPropertyElement.Elements(XName.Get("ScalarProperty", mslNamespace.NamespaceName)))
                {
                    var scalarProperty = complexProperty.ComplexType.AllScalarProperties.GetByName(scalarPropertyElement.Attribute("Name").Value);
                    var column = table.Properties.First(c => c.Name == scalarPropertyElement.Attribute("ColumnName").Value);
                    complexPropertyMapping[scalarProperty, table] = column;
                }
                MapComplexProperties(complexPropertyElement, complexPropertyMapping, complexProperty.ComplexType, table);
            }
        }

        private static CUDFunctionMapping SetCUDFunctionMapping(SSDLContainer ssdlContainer, EntityType entityType, XElement functionMappingElement)
        {
            var cudFunctionMapping = new CUDFunctionMapping();
            var ssdlFunction = ssdlContainer.Functions.GetByName(GetName(functionMappingElement.Attribute("FunctionName").Value));
            cudFunctionMapping.SSDLFunction = ssdlFunction;
            SetCUDFunctionParametersMapping(entityType, functionMappingElement, cudFunctionMapping.ParametersMapping, ssdlFunction);
            foreach (var scalarPropertyElement in functionMappingElement.Elements(XName.Get("ResultBinding", mslNamespace.NamespaceName)))
                cudFunctionMapping.ResultsMapping[entityType.AllScalarProperties.GetByName(scalarPropertyElement.Attribute("Name").Value)] = scalarPropertyElement.Attribute("ColumnName").Value;
            foreach (var associationEndElement in functionMappingElement.Elements(XName.Get("AssociationEnd", mslNamespace.NamespaceName)))
            {
                var navigationProperty = entityType.NavigationProperties.First(np => np.Association.AssociationSetName == associationEndElement.Attribute("AssociationSet").Value);
                var cudFunctionAssociationMapping = new CUDFunctionAssociationMapping { Association = navigationProperty.Association, FromRole = associationEndElement.Attribute("From").Value, ToRole = associationEndElement.Attribute("To").Value };
                cudFunctionMapping.AssociationMappings.Add(cudFunctionAssociationMapping);
                SetCUDFunctionParametersScalarMapping(navigationProperty.RelatedEntityType, associationEndElement, cudFunctionAssociationMapping.AssociationPropertiesMapping, ssdlFunction);
            }
            return cudFunctionMapping;
        }

        private static void SetCUDFunctionParametersMapping(TypeBase entityType, XElement functionMappingElement, EntityTypeCUDFunctionParametersMapping cudFunctionParametersMapping, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function.Function ssdlFunction)
        {
            SetCUDFunctionParametersScalarMapping(entityType, functionMappingElement, cudFunctionParametersMapping, ssdlFunction);
            foreach (var complexPropertyElement in functionMappingElement.Elements(XName.Get("ComplexProperty", mslNamespace.NamespaceName)))
            {
                var cudFunctionComplexPropertyParametersMapping = new EntityTypeCUDFunctionParametersMapping();
                var complexProperty = entityType.AllComplexProperties.GetByName(complexPropertyElement.Attribute("Name").Value);
                cudFunctionParametersMapping.ComplexPropertiesMapping.Add(new KeyValuePair<ComplexProperty, EntityTypeCUDFunctionParametersMapping>(complexProperty, cudFunctionComplexPropertyParametersMapping));
                SetCUDFunctionParametersMapping(complexProperty.ComplexType, complexPropertyElement, cudFunctionComplexPropertyParametersMapping, ssdlFunction);
            }
        }

        private static void SetCUDFunctionParametersScalarMapping(TypeBase entityType, XElement functionMappingElement, CUDFunctionParametersMapping cudFunctionParametersMapping, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function.Function ssdlFunction)
        {
            foreach (var scalarPropertyElement in functionMappingElement.Elements(XName.Get("ScalarProperty", mslNamespace.NamespaceName)))
            {
                var scalarProperty = entityType.AllScalarProperties.GetByName(scalarPropertyElement.Attribute("Name").Value);
                var parameter = ssdlFunction.Parameters.GetByName(scalarPropertyElement.Attribute("ParameterName").Value);
                var versionAttribute = scalarPropertyElement.Attribute("Version");
                var functionParameterMapping = new FunctionParameterMapping { SSDLFunctionParameter = parameter };
                if (versionAttribute != null)
                    functionParameterMapping.Version = versionAttribute.Value.ToLower() == "current" ? FunctionParameterVersion.Current : FunctionParameterVersion.Original;
                cudFunctionParametersMapping[scalarProperty] = functionParameterMapping;
            }
        }

        #endregion
    }
}
