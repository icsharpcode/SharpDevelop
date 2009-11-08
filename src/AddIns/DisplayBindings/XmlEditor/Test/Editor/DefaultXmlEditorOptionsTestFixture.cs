// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class DefaultXmlEditorOptionsTestFixture
	{
		XmlEditorOptions options;
		Properties properties;
		PropertyChangedEventArgs propertyChangedEventArgs;
		DefaultXmlSchemaFileAssociations defaultXmlSchemaFileExtensions;
		XmlSchemaCompletionDataCollection schemas;
		
		[SetUp]
		public void Init()
		{
			schemas = new XmlSchemaCompletionDataCollection();
			defaultXmlSchemaFileExtensions = CreateDefaultXmlSchemaFileExtensions();
			properties = new Properties();
			options = new XmlEditorOptions(properties, defaultXmlSchemaFileExtensions, schemas);
		}
		
		DefaultXmlSchemaFileAssociations CreateDefaultXmlSchemaFileExtensions()
		{
			string addinXml = "<AddIn name     = 'Xml Editor'\r\n" +
       								"author      = ''\r\n" +
       								"copyright   = 'prj:///doc/copyright.txt'\r\n" +
       								"description = ''\r\n" +
       								"addInManagerHidden = 'preinstalled'>\r\n" +
								"</AddIn>";

			using (StringReader reader = new StringReader(addinXml)) {
				AddIn addin = AddIn.Load(reader);
				
				AddInTreeNode addinTreeNode = new AddInTreeNode();

				Properties properties = new Properties();
				properties.Set<string>("id", ".xml");
				properties.Set<string>("namespaceUri", "http://example.com");
				properties.Set<string>("namespacePrefix", "e");
				addinTreeNode.AddCodons(
					new Codon[] {
						new Codon(addin, "SchemaAssociation", properties, new ICondition[0])
					});
				
				return new DefaultXmlSchemaFileAssociations(addinTreeNode);
			}
		}
		
		[Test]
		public void ShowAttributesWhenFoldedPropertyReturnsFalse()
		{
			Assert.IsFalse(options.ShowAttributesWhenFolded);
		}
		
		[Test]
		public void ShowSchemaAnnotationPropertyReturnsTrue()
		{
			Assert.IsTrue(options.ShowSchemaAnnotation);
		}
		
		[Test]
		public void SettingShowAttributesWhenFoldedToTrueUpdatesPropertiesObject()
		{
			options.ShowAttributesWhenFolded = true;
			Assert.IsTrue(properties.Get(XmlEditorOptions.ShowAttributesWhenFoldedPropertyName, false));
		}
		
		[Test]
		public void SettingSchemaAnnotationToTrueUpdatesPropertiesObject()
		{
			properties.Set(XmlEditorOptions.ShowSchemaAnnotationPropertyName, false);
			options.ShowSchemaAnnotation = true;
			Assert.IsTrue(properties.Get(XmlEditorOptions.ShowSchemaAnnotationPropertyName, false));
		}
		
		[Test]
		public void ShowAttributesWhenFoldedInPropertiesObjectUsedByXmlEditorAddInOptions()
		{
			properties.Set(XmlEditorOptions.ShowAttributesWhenFoldedPropertyName, true);
			Assert.IsTrue(options.ShowAttributesWhenFolded);
		}
		
		[Test]
		public void ShowSchemaAnnotationInPropertiesObjectUsedByXmlEditorAddInOptions()
		{
			properties.Set(XmlEditorOptions.ShowSchemaAnnotationPropertyName, false);
			Assert.IsFalse(options.ShowSchemaAnnotation);
		}

		[Test]
		public void PropertyChangedEventFiredWhenPropertiesObjectChanged()
		{
			options.PropertyChanged += OptionsPropertyChanged;
			properties.Set(XmlEditorOptions.ShowAttributesWhenFoldedPropertyName, true);
			Assert.AreEqual(XmlEditorOptions.ShowAttributesWhenFoldedPropertyName, propertyChangedEventArgs.Key);
		}
		
		void OptionsPropertyChanged(object source, PropertyChangedEventArgs e)
		{
			propertyChangedEventArgs = e;
		}
				
		[Test]
		public void PropertyChangedEventNotFiredAfterRemovingEventHandler()
		{
			PropertyChangedEventFiredWhenPropertiesObjectChanged();
			propertyChangedEventArgs = null;
			options.PropertyChanged -= OptionsPropertyChanged;
			properties.Set(XmlEditorOptions.ShowAttributesWhenFoldedPropertyName, true);
			Assert.IsNull(propertyChangedEventArgs);
		}
		
		[Test]
		public void UnknownFileExtensionReturnsEmptySchemaAssociation()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(String.Empty, String.Empty, String.Empty);
			Assert.AreEqual(expectedSchemaAssociation, options.GetSchemaFileAssociation(".unknown"));
		}
		
		[Test]
		public void RegisteredSchemaAssociationReturnedByXmlEditorOptions()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(".abc", "namespace-uri", "prefix");
			properties.Set("ext.abc", ".abc|namespace-uri|prefix");
			Assert.AreEqual(expectedSchemaAssociation, options.GetSchemaFileAssociation(".abc"));
		}
		
		[Test]
		public void RegisteredSchemaAssociationStoredInProperties()
		{
			XmlSchemaFileAssociation schemaAssociation = new XmlSchemaFileAssociation(".abc", "namespace-uri", "prefix");
			options.SetSchemaFileAssociation(schemaAssociation);
			Assert.AreEqual(".abc|namespace-uri|prefix", properties.Get("ext.abc", String.Empty));
		}
		
		[Test]
		public void RegisteredSchemaAssociationMatchedByFileExtensionIsCaseInsensitive()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(".abc", "namespace-uri", "prefix");
			properties.Set("ext.abc", ".abc|namespace-uri|prefix");
			Assert.AreEqual(expectedSchemaAssociation, options.GetSchemaFileAssociation(".ABC"));
		}
		
		[Test]
		public void DefaultXmlExtensionRevertedToIfNoFileExtensionSavedInXmlEditorOptionsProperties()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(".xml", "http://example.com", "e");
			Assert.AreEqual(expectedSchemaAssociation, options.GetSchemaFileAssociation(".xml"));
		}
		
		[Test]
		public void GetNamespacePrefixForRegisteredSchemaFileExtension()
		{
			Assert.AreEqual("e", options.GetNamespacePrefix(".xml"));
		}
		
		[Test]
		public void EmptyNamespacePrefixReturnedForUnknownSchemaFileExtension()
		{
			Assert.AreEqual(String.Empty, options.GetNamespacePrefix(".unknown"));
		}
		
		[Test]
		public void NullSchemaReturnedforUnknownSchemaFileExtension()
		{
			Assert.IsNull(options.GetSchemaCompletionData(".unknown"));
		}
		
		[Test]
		public void SchemaReturnedForKnownSchemaFileExtension()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://example.com' />";
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData(new StringReader(xml));
			schemas.Add(schema);
			
			Assert.AreSame(schema, options.GetSchemaCompletionData(".xml"));
		}
	}
}
