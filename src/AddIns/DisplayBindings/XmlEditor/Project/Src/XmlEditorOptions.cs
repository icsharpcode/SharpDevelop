// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using ICSharpCode.Core;
using System.Globalization;

namespace ICSharpCode.XmlEditor
{
	public class XmlEditorOptions
	{
		public static readonly string OptionsProperty = "XmlEditor.AddIn.Options";
		public static readonly string ShowAttributesWhenFoldedPropertyName = "ShowAttributesWhenFolded";
		public static readonly string ShowSchemaAnnotationPropertyName = "ShowSchemaAnnotation";
		
		Properties properties;
		
		public XmlEditorOptions(Properties properties)
		{
			this.properties = properties;
		}

		public event PropertyChangedEventHandler PropertyChanged {
			add    { properties.PropertyChanged += value; }
			remove { properties.PropertyChanged -= value; }
		}

		public bool ShowAttributesWhenFolded {
			get { return properties.Get(ShowAttributesWhenFoldedPropertyName, false); }
			set {  properties.Set(ShowAttributesWhenFoldedPropertyName, value); }
		}
		
		public bool ShowSchemaAnnotation {
			get { return properties.Get(ShowSchemaAnnotationPropertyName, true); }
			set { properties.Set(ShowSchemaAnnotationPropertyName, value); }
		}		
	}
}
