using System;
using System.Collections;
using System.util;
using iTextSharp.text.factories;

/*
 * $Id: Annotation.cs,v 1.12 2008/05/13 11:25:08 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text 
{
    /// <summary>
    /// An Annotation is a little note that can be added to a page
    /// on a document.
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Anchor"/>
    public class Annotation : IElement 
    {
    
        // membervariables
    
        /// <summary>This is a possible annotation type.</summary>
        public const int TEXT = 0;    
        /// <summary>This is a possible annotation type.</summary>
        public const int URL_NET = 1;    
        /// <summary>This is a possible annotation type.</summary>
        public const int URL_AS_STRING = 2;    
        /// <summary>This is a possible annotation type.</summary>
        public const int FILE_DEST = 3;    
        /// <summary>This is a possible annotation type.</summary>
        public const int FILE_PAGE = 4;    
        /// <summary>This is a possible annotation type.</summary>
        public const int NAMED_DEST = 5;    
        /// <summary>This is a possible annotation type.</summary>
        public const int LAUNCH = 6;
        /// <summary>This is a possible annotation type.</summary>
        public const int SCREEN = 7;
    
        /// <summary>This is a possible attribute.</summary>
        public const string TITLE = "title";
        /// <summary>This is a possible attribute.</summary>
        public const string CONTENT = "content";
        /// <summary>This is a possible attribute.</summary>
        public const string URL = "url";
        /// <summary>This is a possible attribute.</summary>
        public const string FILE = "file";
        /// <summary>This is a possible attribute.</summary>
        public const string DESTINATION = "destination";
        /// <summary>This is a possible attribute.</summary>
        public const string PAGE = "page";
        /// <summary>This is a possible attribute.</summary>
        public const string NAMED = "named";
        /// <summary>This is a possible attribute.</summary>
        public const string APPLICATION = "application";
        /// <summary>This is a possible attribute.</summary>
        public const string PARAMETERS = "parameters";
        /// <summary>This is a possible attribute.</summary>
        public const string OPERATION = "operation";
        /// <summary>This is a possible attribute.</summary>
        public const string DEFAULTDIR = "defaultdir";
        /// <summary>This is a possible attribute.</summary>
        public const string LLX = "llx";
        /// <summary>This is a possible attribute.</summary>
        public const string LLY = "lly";
        /// <summary>This is a possible attribute.</summary>
        public const string URX = "urx";
        /// <summary>This is a possible attribute.</summary>
        public const string URY = "ury";
        /// <summary>This is a possible attribute.</summary>
        public const string MIMETYPE = "mime";
    
        /// <summary>This is the type of annotation.</summary>
        protected int annotationtype;
    
        /// <summary>This is the title of the Annotation.</summary>
        protected Hashtable annotationAttributes = new Hashtable();

        /// <summary>This is the lower left x-value</summary>
        private float llx = float.NaN;
        /// <summary>This is the lower left y-value</summary>
        private float lly = float.NaN;
        /// <summary>This is the upper right x-value</summary>
        private float urx = float.NaN;
        /// <summary>This is the upper right y-value</summary>
        private float ury = float.NaN;
    
        // constructors
    
        /// <summary>
        /// Constructs an Annotation with a certain title and some text.
        /// </summary>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        private Annotation(float llx, float lly, float urx, float ury) 
        {
            this.llx = llx;
            this.lly = lly;
            this.urx = urx;
            this.ury = ury;
        }
    
        public Annotation(Annotation an) {
            annotationtype = an.annotationtype;
            annotationAttributes = an.annotationAttributes;
            llx = an.llx;
            lly = an.lly;
            urx = an.urx;
            ury = an.ury;
        }

        /// <summary>
        /// Constructs an Annotation with a certain title and some text.
        /// </summary>
        /// <param name="title">the title of the annotation</param>
        /// <param name="text">the content of the annotation</param>
        public Annotation(string title, string text) 
        {
            annotationtype = TEXT;
            annotationAttributes[TITLE] = title;
            annotationAttributes[CONTENT] = text;
        }
    
        /// <summary>
        /// Constructs an Annotation with a certain title and some text.
        /// </summary>
        /// <param name="title">the title of the annotation</param>
        /// <param name="text">the content of the annotation</param>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        public Annotation(string title, string text, float llx, float lly, float urx, float ury) : this(llx, lly, urx, ury) 
        {
            annotationtype = TEXT;
            annotationAttributes[TITLE] = title;
            annotationAttributes[CONTENT] = text;
        }
    
        /// <summary>
        /// Constructs an Annotation.
        /// </summary>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        /// <param name="url">the external reference</param>
        public Annotation(float llx, float lly, float urx, float ury, Uri url) : this(llx, lly, urx, ury) 
        {
            annotationtype = URL_NET;
            annotationAttributes[URL] = url;
        }
    
        /// <summary>
        /// Constructs an Annotation.
        /// </summary>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        /// <param name="url">the external reference</param>
        public Annotation(float llx, float lly, float urx, float ury, string url) : this(llx, lly, urx, ury) 
        {
            annotationtype = URL_AS_STRING;
            annotationAttributes[FILE] = url;
        }
    
        /// <summary>
        /// Constructs an Annotation.
        /// </summary>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        /// <param name="file">an external PDF file</param>
        /// <param name="dest">the destination in this file</param>
        public Annotation(float llx, float lly, float urx, float ury, string file, string dest) : this(llx, lly, urx, ury) 
        {
            annotationtype = FILE_DEST;
            annotationAttributes[FILE] = file;
            annotationAttributes[DESTINATION] = dest;
        }

        /// <summary>
        /// Creates a Screen anotation to embed media clips
        /// </summary>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        /// <param name="moviePath">path to the media clip file</param>
        /// <param name="mimeType">mime type of the media</param>
        /// <param name="showOnDisplay">if true play on display of the page</param>
        public Annotation(float llx, float lly, float urx, float ury,
            string moviePath, string mimeType, bool showOnDisplay) : this(llx, lly, urx, ury)
        {
            annotationtype = SCREEN;
            annotationAttributes[FILE] = moviePath;
            annotationAttributes[MIMETYPE] = mimeType;
            annotationAttributes[PARAMETERS] = new bool[] {false /* embedded */, showOnDisplay };
        }
    
        /// <summary>
        /// Constructs an Annotation.
        /// </summary>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        /// <param name="file">an external PDF file</param>
        /// <param name="page">a page number in this file</param>
        public Annotation(float llx, float lly, float urx, float ury, string file, int page) : this(llx, lly, urx, ury) 
        {
            annotationtype = FILE_PAGE;
            annotationAttributes[FILE] = file;
            annotationAttributes[PAGE] = page;
        }
    
        /// <summary>
        /// Constructs an Annotation.
        /// </summary>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        /// <param name="named">a named destination in this file</param>
        /// <overloads>
        /// Has nine overloads.
        /// </overloads>
        public Annotation(float llx, float lly, float urx, float ury, int named) : this(llx, lly, urx, ury) 
        {
            annotationtype = NAMED_DEST;
            annotationAttributes[NAMED] = named;
        }
    
        /// <summary>
        /// Constructs an Annotation.
        /// </summary>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        /// <param name="application">an external application</param>
        /// <param name="parameters">parameters to pass to this application</param>
        /// <param name="operation">the operation to pass to this application</param>
        /// <param name="defaultdir">the default directory to run this application in</param>
        public Annotation(float llx, float lly, float urx, float ury, string application, string parameters, string operation, string defaultdir) : this(llx, lly, urx, ury) 
        {
            annotationtype = LAUNCH;
            annotationAttributes[APPLICATION] = application;
            annotationAttributes[PARAMETERS] = parameters;
            annotationAttributes[OPERATION] = operation;
            annotationAttributes[DEFAULTDIR] = defaultdir;
        }
    
        // implementation of the Element-methods
    
        /// <summary>
        /// Gets the type of the text element
        /// </summary>
        public int Type 
        {
            get 
            {
                return Element.ANNOTATION;
            }
        }
    
        // methods
    
        /// <summary>
        /// Processes the element by adding it (or the different parts) to an
        /// IElementListener.
        /// </summary>
        /// <param name="listener">an IElementListener</param>
        /// <returns>true if the element was process successfully</returns>
        public bool Process(IElementListener listener) 
        {
            try 
            {
                return listener.Add(this);
            }
            catch (DocumentException) 
            {
                return false;
            }
        }
    
        /// <summary>
        /// Gets all the chunks in this element.
        /// </summary>
        /// <value>an ArrayList</value>
        public ArrayList Chunks 
        {
            get 
            {
                return new ArrayList();
            }
        }
   
        // methods
    
        /// <summary>
        /// Sets the dimensions of this annotation.
        /// </summary>
        /// <param name="llx">the lower left x-value</param>
        /// <param name="lly">the lower left y-value</param>
        /// <param name="urx">the upper right x-value</param>
        /// <param name="ury">the upper right y-value</param>
        public void SetDimensions (float llx, float lly, float urx, float ury) 
        {
            this.llx = llx;
            this.lly = lly;
            this.urx = urx;
            this.ury = ury;
        }
    
        // methods to retrieve information
    
        /// <summary>
        /// Returns the lower left x-value.
        /// </summary>
        /// <returns>a value</returns>
        public float GetLlx() 
        {
            return llx;
        }
    
        /// <summary>
        /// Returns the lower left y-value.
        /// </summary>
        /// <returns>a value</returns>
        public float GetLly() 
        {
            return lly;
        }
    
        /// <summary>
        /// Returns the uppper right x-value.
        /// </summary>
        /// <returns>a value</returns>
        public float GetUrx() 
        {
            return urx;
        }
    
        /// <summary>
        /// Returns the uppper right y-value.
        /// </summary>
        /// <returns>a value</returns>
        public float GetUry() 
        {
            return ury;
        }
    
        /// <summary>
        /// Returns the lower left x-value.
        /// </summary>
        /// <param name="def">the default value</param>
        /// <returns>a value</returns>
        public float GetLlx(float def) 
        {
            if (float.IsNaN(llx))
                return def;
            return llx;
        }
    
        /// <summary>
        /// Returns the lower left y-value.
        /// </summary>
        /// <param name="def">the default value</param>
        /// <returns>a value</returns>
        public float GetLly(float def) 
        {
            if (float.IsNaN(lly))
                return def;
            return lly;
        }
    
        /// <summary>
        /// Returns the upper right x-value.
        /// </summary>
        /// <param name="def">the default value</param>
        /// <returns>a value</returns>
        public float GetUrx(float def) 
        {
            if (float.IsNaN(urx))
                return def;
            return urx;
        }
    
        /// <summary>
        /// Returns the upper right y-value.
        /// </summary>
        /// <param name="def">the default value</param>
        /// <returns>a value</returns>
        public float GetUry(float def) 
        {
            if (float.IsNaN(ury))
                return def;
            return ury;
        }
    
        /// <summary>
        /// Returns the type of this Annotation.
        /// </summary>
        /// <value>a type</value>
        public int AnnotationType 
        {
            get 
            {
                return annotationtype;
            }
        }
    
        /// <summary>
        /// Returns the title of this Annotation.
        /// </summary>
        /// <value>a name</value>
        public string Title 
        {
            get 
            {
                string s = (string)annotationAttributes[TITLE];
                if (s == null) 
                    s = "";
                return s;
            }
        }
    
        /// <summary>
        /// Gets the content of this Annotation.
        /// </summary>
        /// <value>a reference</value>
        public string Content 
        {
            get 
            {
                string s = (string)annotationAttributes[CONTENT];
                if (s == null) s = "";
                return s;
            }
        }
    
        /// <summary>
        /// Gets the content of this Annotation.
        /// </summary>
        /// <value>a reference</value>
        public Hashtable Attributes 
        {
            get 
            {
                return annotationAttributes;
            }
        }
    
        /**
        * @see com.lowagie.text.Element#isContent()
        * @since   iText 2.0.8
        */
        public bool IsContent() {
            return true;
        }

        /**
        * @see com.lowagie.text.Element#isNestable()
        * @since   iText 2.0.8
        */
        public bool IsNestable() {
            return true;
        }

        public override string ToString() {
            return base.ToString();
        }
    }
}
