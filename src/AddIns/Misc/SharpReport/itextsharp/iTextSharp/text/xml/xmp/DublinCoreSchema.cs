using System;
/*
 * $Id: DublinCoreSchema.cs,v 1.3 2008/05/13 11:26:16 psoares33 Exp $
 * 
 *
 * Copyright 2005 by Bruno Lowagie.
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
 * the Initial Developer are Copyright (C) 1999-2005 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2005 by Paulo Soares. All Rights Reserved.
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
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE 
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU LIBRARY GENERAL PUBLIC LICENSE for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.xml.xmp {

    /**
    * An implementation of an XmpSchema.
    */
    public class DublinCoreSchema : XmpSchema {
        
        /** default namespace identifier*/
        public const String DEFAULT_XPATH_ID = "dc";
        /** default namespace uri*/
        public const String DEFAULT_XPATH_URI = "http://purl.org/dc/elements/1.1/";
        /** External Contributors to the resource (other than the authors). */
        public const String CONTRIBUTOR = "dc:contributor";
        /** The extent or scope of the resource. */
        public const String COVERAGE = "dc:coverage";
        /** The authors of the resource (listed in order of precedence, if significant). */
        public const String CREATOR = "dc:creator";
        /** Date(s) that something interesting happened to the resource. */
        public const String DATE = "dc:date";
        /** A textual description of the content of the resource. Multiple values may be present for different languages. */
        public const String DESCRIPTION = "dc:description";
        /** The file format used when saving the resource. Tools and applications should set this property to the save format of the data. It may include appropriate qualifiers. */
        public const String FORMAT = "dc:format";
        /** Unique identifier of the resource. */
        public const String IDENTIFIER = "dc:identifier";
        /** An unordered array specifying the languages used in the resource. */
        public const String LANGUAGE = "dc:language";
        /** Publishers. */
        public const String PUBLISHER = "dc:publisher";
        /** Relationships to other documents. */
        public const String RELATION = "dc:relation";
        /** Informal rights statement, selected by language. */
        public const String RIGHTS = "dc:rights";
        /** Unique identifier of the work from which this resource was derived. */
        public const String SOURCE = "dc:source";
        /** An unordered array of descriptive phrases or keywords that specify the topic of the content of the resource. */
        public const String SUBJECT = "dc:subject";
        /** The title of the document, or the name given to the resource. Typically, it will be a name by which the resource is formally known. */
        public const String TITLE = "dc:title";
        /** A document type; for example, novel, poem, or working paper. */
        public const String TYPE = "dc:type";
        
        /**
        * @param shorthand
        * @throws IOException
        */
        public DublinCoreSchema() : base("xmlns:" + DEFAULT_XPATH_ID + "=\"" + DEFAULT_XPATH_URI + "\"") {
            this[FORMAT] = "application/pdf";
        }
        
        /**
        * Adds a title.
        * @param title
        */
        public void AddTitle(String title) {
            this[TITLE] = title;
        }
        
        /**
        * Adds a description.
        * @param desc
        */
        public void AddDescription(String desc) {
            this[DESCRIPTION] = desc;
        }

        /**
        * Adds a subject.
        * @param subject
        */
        public void AddSubject(String subject) {
            XmpArray array = new XmpArray(XmpArray.UNORDERED);
            array.Add(subject);
            SetProperty(SUBJECT, array);
        }

        
        /**
        * Adds a subject.
        * @param subject array of subjects
        */
        public void addSubject(String[] subject) {
            XmpArray array = new XmpArray(XmpArray.UNORDERED);
            for (int i = 0; i < subject.Length; i++) {
                array.Add(subject[i]);
            }
            SetProperty(SUBJECT, array);
        }
        
        /**
        * Adds a single author.
        * @param author
        */
        public void AddAuthor(String author) {
            XmpArray array = new XmpArray(XmpArray.ORDERED);
            array.Add(author);
            SetProperty(CREATOR, array);
        }
        
        /**
        * Adds an array of authors.
        * @param author
        */
        public void AddAuthor(String[] author) {
            XmpArray array = new XmpArray(XmpArray.ORDERED);
            for (int i = 0; i < author.Length; i++) {
                array.Add(author[i]);
            }
            SetProperty(CREATOR, array);
        }

        /**
        * Adds a single publisher.
        * @param publisher
        */
        public void AddPublisher(String publisher) {
            XmpArray array = new XmpArray(XmpArray.ORDERED);
            array.Add(publisher);
            SetProperty(PUBLISHER, array);
        }

        /**
        * Adds an array of publishers.
        * @param publisher
        */
        public void AddPublisher(String[] publisher) {
            XmpArray array = new XmpArray(XmpArray.ORDERED);
            for (int i = 0; i < publisher.Length; i++) {
                array.Add(publisher[i]);
            }
            SetProperty(PUBLISHER, array);
        }
    }
}