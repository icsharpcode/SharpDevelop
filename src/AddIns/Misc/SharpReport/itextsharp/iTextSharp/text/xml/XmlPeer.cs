using System;
using System.Collections;
using System.util;

using iTextSharp.text;

/*
 * $Id: XmlPeer.cs,v 1.5 2008/05/13 11:26:12 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 by Bruno Lowagie.
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
 *
 * The Original Code is 'iText, a free JAVA-PDF library'.
 *
 * The Initial Developer of the Original Code is Bruno Lowagie. Portions created by
 * the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000, 2001, 2002 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), in which case the
 * provisions of LGPL are applicable instead of those above.  If you wish to
 * allow use of your version of this file only under the terms of the LGPL
 * License and not to allow others to use your version of this file under
 * the MPL, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the LGPL.
 * If you do not delete the provisions above, a recipient may use your version
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE.
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.xml {

	/// <summary>
	/// This interface is implemented by the peer of all the iText objects.
	/// </summary>
	public class XmlPeer {
    
		/// <summary> This is the name of the alias. </summary>
		protected String tagname;
    
		/// <summary> This is the name of the alias. </summary>
		protected String customTagname;
    
		/// <summary> This is the Map that contains the aliases of the attributes. </summary>
		protected Properties attributeAliases = new Properties();
    
		/// <summary> This is the Map that contains the default values of the attributes. </summary>
		protected Properties attributeValues = new Properties();
    
		/// <summary> This is String that contains the default content of the attributes. </summary>
		protected String defaultContent = null;
    
		/// <summary>
		/// Creates a XmlPeer.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="alias"></param>
		public XmlPeer(String name, String alias) {
			this.tagname = name;
			this.customTagname = alias;
		}
    
		/// <summary>
		/// Gets the tagname of the peer.
		/// </summary>
		/// <value>the tagname of the peer</value>
		public String Tag {
			get {
				return tagname;
			}
		}
    
		/// <summary>
		/// Gets the alias of the peer.
		/// </summary>
		/// <value>the alias of the peer</value>
		public String Alias {
			get {
				return customTagname;
			}
		}
    
		/// <summary> Gets the list of attributes of the peer. </summary>
		public virtual Properties GetAttributes(Hashtable attrs) {
			Properties attributes = new Properties();
			attributes.AddAll(attributeValues);
			if (defaultContent != null) {
				attributes.Add(ElementTags.ITEXT, defaultContent);
			}
			if (attrs != null) {
				foreach (string key in attrs.Keys) {
					attributes.Add(GetName(key), (string)attrs[key]);
				}
			}
			return attributes;
		}
    
		/// <summary>
		/// Sets an alias for an attribute.
		/// </summary>
		/// <param name="name">the iText tagname</param>
		/// <param name="alias">the custom tagname</param>
		public virtual void AddAlias(String name, String alias) {
			attributeAliases.Add(alias, name);
		}
    
		/// <summary>
		/// Sets a value for an attribute.
		/// </summary>
		/// <param name="name">the iText tagname</param>
		/// <param name="value">the default value for this tag</param>
		public void AddValue(String name, String value) {
			attributeValues.Add(name, value);
		}
    
		/// <summary>
		/// Sets the default content.
		/// </summary>
		/// <value>the default content</value>
		public string Content {
			set {
				this.defaultContent = value;
			}
		}
    
		/// <summary>
		/// Returns the iText attribute name.
		/// </summary>
		/// <param name="name">the custom attribute name</param>
		/// <returns>the iText attribute name</returns>
		public String GetName(String name) {
			String value;
			if ((value = attributeAliases[name]) != null) {
				return value;
			}
			return name;
		}
    
		/// <summary>
		/// Returns the default values.
		/// </summary>
		/// <value>the default values</value>
		public Properties DefaultValues {
			get {
				return attributeValues;
			}
		}
	}
}