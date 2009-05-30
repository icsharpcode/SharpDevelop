// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// The Xml Editor add-in options.
	/// </summary>
	public static class XmlEditorAddInOptions
	{
		public static readonly string OptionsProperty = "XmlEditor.AddIn.Options";
		public static readonly string ShowAttributesWhenFoldedPropertyName = "ShowAttributesWhenFolded";
		public static readonly string ShowSchemaAnnotationPropertyName = "ShowSchemaAnnotation";
		
		static Properties properties;

		static XmlEditorAddInOptions()
 		{
			properties = PropertyService.Get(OptionsProperty, new Properties());
		}

 		static Properties Properties {
			get {
				Debug.Assert(properties != null);
				return properties;
 			}
		}
		
		public static event PropertyChangedEventHandler PropertyChanged {
			add    { Properties.PropertyChanged += value; }
			remove { Properties.PropertyChanged -= value; }
		}
		
		#region Properties
		/// <summary>
		/// Gets an association between a schema and a file extension.
		/// </summary>
		/// <remarks>
		/// <para>The property will be an xml element when the SharpDevelopProperties.xml
		/// is read on startup.  The property will be a schema association
		/// if the user changes the schema associated with the file
		/// extension in tools->options.</para>
		/// <para>The normal way of doing things is to
		/// pass the GetProperty method a default value which auto-magically
		/// turns the xml element into a schema association so we would not 
		/// have to check for both.  In this case, however, I do not want
		/// a default saved to the SharpDevelopProperties.xml file unless the user
		/// makes a change using Tools->Options.</para>
		/// <para>If we have a file extension that is currently missing a default 
		/// schema then if we  ship the schema at a later date the association will 
		/// be updated by the code if the user has not changed the settings themselves. 
		/// </para>
		/// <para>For example, the initial release of the xml editor add-in had
		/// no default schema for .xsl files, by default it was associated with
		/// no schema and this setting is saved if the user ever viewed the settings
		/// in the tools->options dialog.  Now, after the initial release the
		/// .xsl schema was created and shipped with SharpDevelop, there is
		/// no way to associate this schema to .xsl files by default since 
		/// the property exists in the SharpDevelopProperties.xml file.</para>
		/// <para>An alternative way of doing this might be to have the
		/// config info in the schema itself, which a special SharpDevelop 
		/// namespace.  I believe this is what Visual Studio does.  This
		/// way is not as flexible since it requires the user to locate
		/// the schema and change the association manually.</para>
		/// </remarks>
		public static XmlSchemaAssociation GetSchemaAssociation(string extension)
		{			
			extension = extension.ToLower();
			string property = Properties.Get("ext" + extension, String.Empty);
			XmlSchemaAssociation association = null;
			
			if (property.Length > 0) {
				association = XmlSchemaAssociation.ConvertFromString(property);
			}
			
			// Use default?
			if (association == null) {
				association = XmlSchemaAssociation.GetDefaultAssociation(extension);
			}
			
			return association;
		}
		
		public static void SetSchemaAssociation(XmlSchemaAssociation association)
		{
			Properties.Set("ext" + association.Extension, association.ConvertToString());
		}
		
		public static bool ShowAttributesWhenFolded {
			get {
				return Properties.Get(ShowAttributesWhenFoldedPropertyName, false);
			}
			
			set {
				Properties.Set(ShowAttributesWhenFoldedPropertyName, value);
			}
		}
		
		public static bool ShowSchemaAnnotation {
			get {
				return Properties.Get(ShowSchemaAnnotationPropertyName, true);
			}
			
			set {
				Properties.Set(ShowSchemaAnnotationPropertyName, value);
			}
		}
		
		#endregion
	}
}
