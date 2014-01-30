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
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Function;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class MSLIO : IO
    {
        public static XDocument GenerateTypeMapping(XDocument mslDocument)
        {
            XElement mappingElement = mslDocument.Element(XName.Get("Mapping", mslNamespace.NamespaceName));

            if (mappingElement == null || mappingElement.IsEmpty)
                return null;

            XElement entityContainerMappingElement = mappingElement.Element(XName.Get("EntityContainerMapping", mslNamespace.NamespaceName));

            if (entityContainerMappingElement == null || entityContainerMappingElement.IsEmpty)
                return null;

            foreach (XElement entitySetMapping in entityContainerMappingElement.Elements(mslNamespace + "EntitySetMapping")) {
            	if (entitySetMapping.HasEntityTypeMappingChildElement())
            		continue;
            	
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

        public static XElement Write(EDM edm)
        {
            CSDLContainer csdlContainer = edm.CSDLContainer;
            string entityContainerNamespace = string.Concat(csdlContainer.Namespace, ".");

            // Instantiate Mapping
            XElement mapping = new XElement(mslNamespace + "Mapping",
                new XAttribute("Space", "C-S"));

            // EntityContainerMapping
            XElement entityContainerMapping = new XElement(mslNamespace + "EntityContainerMapping", 
                new XAttribute("StorageEntityContainer", edm.SSDLContainer.Name),
                new XAttribute("CdmEntityContainer", csdlContainer.Name));

            foreach (EntityType entitySet in csdlContainer.EntitySets)
            {
                IEnumerable<EntityType> entityTypes = csdlContainer.EntityTypes.Where(entityType => entityType.EntitySetName == entitySet.EntitySetName);

                // EntityContainerMapping : EntitySetMapping
                XElement entitySetMappingElement = new XElement(mslNamespace + "EntitySetMapping", 
                    new XAttribute("Name", entitySet.Name));

                // EntityContainerMapping : EntitySetMapping : EntityTypeMapping
                foreach (EntityType entityType in entityTypes)
                {
                    XElement entityTypeMappingElement = new XElement(mslNamespace + "EntityTypeMapping", 
                        new XAttribute("TypeName", string.Format("IsTypeOf({0}{1})", entityContainerNamespace, entityType.Name)));

                    // EntityContainerMapping : EntitySetMapping : EntityTypeMapping : MappingFragment
                    foreach (EDMObjects.SSDL.EntityType.EntityType table in entityType.Mapping.MappedSSDLTables)
                    { 
                        XElement mappingFragmentElement = new XElement(mslNamespace + "MappingFragment", 
                            new XAttribute("StoreEntitySet", table.EntitySetName));

                        IEnumerable<PropertyMapping> scalarMappings = entityType.Mapping.GetSpecificMappingForTable(table);

                        foreach (PropertyMapping scalarMapping in scalarMappings)
                        {
                            mappingFragmentElement.AddElement(new XElement(mslNamespace + "ScalarProperty",
                                new XAttribute("Name", scalarMapping.Property.Name),
                                new XAttribute("ColumnName", scalarMapping.Column.Name)));
                        }

                        mappingFragmentElement.Add(MappingComplexProperties(entityType, entityType.Mapping, table, entityContainerNamespace));

                        IEnumerable<ConditionMapping> conditionMappings = entityType.Mapping.ConditionsMapping.Where(condition => condition.Table == table);

                        foreach (ConditionMapping conditionMapping in conditionMappings)
                        {
                            XElement conditionElement = new XElement(mslNamespace + "Condition");

                            if (conditionMapping is ColumnConditionMapping)
                                conditionElement.AddAttribute("ColumnName", (conditionMapping as ColumnConditionMapping).Column.Name);
                            else if (conditionMapping is PropertyConditionMapping)
                                conditionElement.AddAttribute("Name", (conditionMapping as PropertyConditionMapping).CSDLProperty.Name);

                            mappingFragmentElement.Add(conditionElement.AddMappingConditionAttribute(conditionMapping));
                        }

                        entityTypeMappingElement.Add(mappingFragmentElement);
                    }

                    entitySetMappingElement.Add(entityTypeMappingElement);
                }

                // EntityContainerMapping : EntitySetMapping : CUDFunctionMapping
                foreach (EntityType entityType in entityTypes)
                {
                    entitySetMappingElement.Add(CUDFunctionMapping(entityType, entityContainerNamespace, string.Concat(edm.SSDLContainer.Namespace, ".")));
                }

                entityContainerMapping.Add(entitySetMappingElement);
            }

            // EntityContainerMapping : AssociationSetMappings
            IEnumerable<Association> associations = csdlContainer.Associations.Where(association => association.Mapping.SSDLTableMapped != null);

            foreach (Association association in associations)
            {
                XElement associationSetMappingElement = new XElement(mslNamespace + "AssociationSetMapping",
                    new XAttribute("Name", association.AssociationSetName),
                    new XAttribute("TypeName", string.Concat(entityContainerNamespace, association.Name)),
                    new XAttribute("StoreEntitySet", association.Mapping.SSDLTableMapped.Name));

                XElement endPropertyElement1 = new XElement(mslNamespace + "EndProperty",
                    new XAttribute("Name", association.PropertyEnd1Role));

                foreach (PropertyMapping navigationPropertyMapping in association.PropertyEnd1.Mapping)
                {
                    endPropertyElement1.AddElement(new XElement(mslNamespace + "ScalarProperty",
                        new XAttribute("Name", navigationPropertyMapping.Property.Name),
                        new XAttribute("ColumnName", navigationPropertyMapping.Column.Name)));
                }

                XElement endPropertyElement2 = new XElement(mslNamespace + "EndProperty",
                    new XAttribute("Name", association.PropertyEnd2Role));

                foreach (PropertyMapping navigationPropertyMapping in association.PropertyEnd2.Mapping)
                {
                    endPropertyElement2.AddElement(new XElement(mslNamespace + "ScalarProperty",
                        new XAttribute("Name", navigationPropertyMapping.Property.Name),
                        new XAttribute("ColumnName", navigationPropertyMapping.Column.Name)));
                }

                associationSetMappingElement.Add(endPropertyElement1);
                associationSetMappingElement.Add(endPropertyElement2);

                entityContainerMapping.Add(associationSetMappingElement);
            }

            // EntityContainerMapping : Conditions
            foreach (Function function in csdlContainer.Functions)
            {
                entityContainerMapping.Add(new XElement(mslNamespace + "FunctionImportMapping",
                    new XAttribute("FunctionImportName", function.Name),
                    new XAttribute("FunctionName", string.Format("{0}.{1}", edm.SSDLContainer.Namespace, function.SSDLFunction.Name))));
            }

            return mapping.AddElement(entityContainerMapping);
        }

        private static IEnumerable<XElement> MappingComplexProperties(TypeBase type, MappingBase mapping, EDMObjects.SSDL.EntityType.EntityType table, string entityContainerNamespace)
        {
            foreach (ComplexProperty complexProperty in type.AllComplexProperties)
            {
                ComplexPropertyMapping complexPropertyMapping = mapping.GetEntityTypeSpecificComplexPropertyMapping(complexProperty);

                if (complexPropertyMapping != null)
                {
                    XElement complexPropertyElement = new XElement(mslNamespace + "ComplexProperty",
                        new XAttribute("Name", complexProperty.Name));

                    IEnumerable<PropertyMapping> scalarMappings = complexPropertyMapping.GetSpecificMappingForTable(table);

                    foreach (PropertyMapping scalarMapping in scalarMappings)
                    {
                        complexPropertyElement.AddElement(new XElement(mslNamespace + "ScalarProperty",
                            new XAttribute("Name", scalarMapping.Property.Name),
                            new XAttribute("ColumnName", scalarMapping.Column.Name)));
                    }

                    foreach (ComplexProperty subComplexProperty in complexProperty.ComplexType.ComplexProperties)
                    {
                        complexPropertyElement.Add(MappingComplexProperties(complexProperty.ComplexType, complexPropertyMapping, table, entityContainerNamespace));
                    }

                    yield return complexPropertyElement;
                }
            }
        }

        private static XElement MappingColumnCondition(ColumnConditionMapping condition)
        {
            return new XElement(mslNamespace + "Condition", new XAttribute("ColumnName", condition.Column.Name)).AddMappingConditionAttribute(condition);
        }

        private static XElement CUDFunctionMapping(EntityType entityType, string entityContainerNamespace, string storeContainerNamespace)
        {
            EntityTypeMapping mapping = entityType.Mapping;

            if (mapping.InsertFunctionMapping == null || mapping.UpdateFunctionMapping == null || mapping.DeleteFunctionMapping == null)
                return null;

            XElement modificationFunctionMapping = new XElement(mslNamespace + "ModificationFunctionMapping");

            var insertFunction = mapping.InsertFunctionMapping;

            if (insertFunction != null)
            {
                XElement insertFunctionElement = new XElement(mslNamespace + "InsertFunction",
                    new XAttribute("FunctionName", string.Concat(storeContainerNamespace, insertFunction.SSDLFunction.Name)));

                insertFunctionElement.Add(CUDFunctionMappingAssociation(insertFunction));
                insertFunctionElement.Add(CUDFunctionMappingParameters(insertFunction));
                insertFunctionElement.Add(CUDFunctionMappingResults(insertFunction));

                modificationFunctionMapping.AddElement(insertFunctionElement);
            }

            var updateFunction = mapping.UpdateFunctionMapping;

            if (updateFunction != null)
            {
                XElement updateFunctionElement = new XElement(mslNamespace + "UpdateFunction",
                    new XAttribute("FunctionName", string.Concat(storeContainerNamespace, updateFunction.SSDLFunction.Name)));

                updateFunctionElement.Add(CUDFunctionMappingAssociation(updateFunction));
                updateFunctionElement.Add(CUDFunctionMappingParameters(updateFunction));
                updateFunctionElement.Add(CUDFunctionMappingResults(updateFunction));

                modificationFunctionMapping.AddElement(updateFunctionElement);
            }

            var deleteFunction = mapping.DeleteFunctionMapping;

            if (deleteFunction != null)
            {
                XElement deleteFunctionElement = new XElement(mslNamespace + "DeleteFunction",
                    new XAttribute("FunctionName", string.Concat(storeContainerNamespace, deleteFunction.SSDLFunction.Name)));

                deleteFunctionElement.Add(CUDFunctionMappingAssociation(deleteFunction));
                deleteFunctionElement.Add(CUDFunctionMappingParameters(deleteFunction));
                deleteFunctionElement.Add(CUDFunctionMappingResults(deleteFunction));

                modificationFunctionMapping.AddElement(deleteFunctionElement);
            }

            return new XElement(mslNamespace + "EntityTypeMapping",
                new XAttribute("TypeName", string.Concat(entityContainerNamespace, entityType.Name)),
                modificationFunctionMapping);
        }

        private static IEnumerable<XElement> CUDFunctionMappingAssociation(CUDFunctionMapping functionMapping)
        {
            foreach (CUDFunctionAssociationMapping cudFunctionAssiociation in functionMapping.AssociationMappings)
            {
                XElement associationEndElement = new XElement(mslNamespace + "AssociationEnd",
                    new XAttribute("AssociationSet", cudFunctionAssiociation.Association.AssociationSetName),
                    new XAttribute("From", cudFunctionAssiociation.FromRole),
                    new XAttribute("To", cudFunctionAssiociation.ToRole));

                associationEndElement.Add(CUDFunctionMappingScalarPropertiesParameters(cudFunctionAssiociation.AssociationPropertiesMapping));

                yield return associationEndElement;
            }
        }

        private static IEnumerable<XElement> CUDFunctionMappingParameters(CUDFunctionMapping functionMapping)
        {
            return CUDFunctionMappingParameters(functionMapping.ParametersMapping);
        }

        private static IEnumerable<XElement> CUDFunctionMappingParameters(EntityTypeCUDFunctionParametersMapping cudFunctionParameters)
        { 
            List<XElement> complexProperties = new List<XElement>();
                        
            foreach (var complexParameter in cudFunctionParameters.ComplexPropertiesMapping)
            {
                XElement complexProperty = new XElement(mslNamespace + "Complexproperty",
                    new XAttribute("Name", complexParameter.Key.Name));
                complexProperty.Add(CUDFunctionMappingParameters(complexParameter.Value));
            }

            return CUDFunctionMappingScalarPropertiesParameters(cudFunctionParameters).Union(complexProperties);
        }

        private static IEnumerable<XElement> CUDFunctionMappingScalarPropertiesParameters(CUDFunctionParametersMapping cudFunctionParameters)
        {
            foreach (var parameter in cudFunctionParameters)
            {
                yield return new XElement(mslNamespace + "ScalarProperty",
                    new XAttribute("Name", parameter.Key.Name),
                    new XAttribute("ParameterName", parameter.Value.SSDLFunctionParameter.Name))
                    .AddAttribute(null, "Version", parameter.Value.Version);
            }
        }

        private static IEnumerable<XElement> CUDFunctionMappingResults(CUDFunctionMapping functionMapping)
        {
            foreach (var result in functionMapping.ResultsMapping)
            {
                yield return new XElement(mslNamespace + "ResultBinding",
                    new XAttribute("Name", result.Key.Name),
                    new XAttribute("ColumnName", result.Value));
            }
        }
    }

    #region Extension methods

    internal static class MSLIOHelpers
    {
        public static XElement AddMappingConditionAttribute(this XElement element, ConditionMapping conditionMapping)
        {
            switch (conditionMapping.Operator)
            { 
                case ConditionOperator.IsNotNull:
                    element.Add(new XAttribute("IsNull", false));
                    break;
                case ConditionOperator.IsNull:
                    element.Add(new XAttribute("IsNull", true));
                    break;
                case ConditionOperator.Equals:
                    element.Add(new XAttribute("Value", conditionMapping.Value));
                    break;
            }

            return element;
        }
        
		public static bool HasEntityTypeMappingChildElement(this XElement entitySetMapping)
		{
			return entitySetMapping.Elements().Any(e => e.Name.LocalName == "EntityTypeMapping");
		}
    }

    #endregion
}
