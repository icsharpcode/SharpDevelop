using System;
using System.IO;
using System.Xml;
using System.Collections;

using iTextSharp.text;
using iTextSharp.text.xml;

/*
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

namespace iTextSharp.text.html {

    /// <summary>
    /// This class can be used to parse an XML file.
    /// </summary>
    public class HtmlParser : XmlParser{
    
        /// <summary>
        /// Constructs an XmlParser.
        /// </summary>
        public HtmlParser() {
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        public override void Go(IDocListener document, XmlDocument xDoc) {
            parser = new ITextmyHtmlHandler(document);
            parser.Parse(xDoc);
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        public override void Go(IDocListener document, String file) {
            parser = new ITextmyHtmlHandler(document);
            parser.Parse(file);
        }
    
        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        public override void Go(IDocListener document, XmlTextReader reader) {
            parser = new ITextmyHtmlHandler(document);
            parser.Parse(reader);
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public override void Go(IDocListener document, XmlDocument xDoc, XmlDocument xTagmap) {
            parser = new ITextmyHtmlHandler(document, new TagMap(xTagmap));
            parser.Parse(xDoc);
        }
    
        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="tagmap"></param>
        public override void Go(IDocListener document, XmlTextReader reader, String tagmap) {
            parser = new ITextmyHtmlHandler(document, new TagMap(tagmap));
            parser.Parse(reader);
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public override void Go(IDocListener document, String file, String tagmap) {
            parser = new ITextmyHtmlHandler(document, new TagMap(tagmap));
            parser.Parse(file);
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public override void Go(IDocListener document, String file, Hashtable tagmap) {
            parser = new ITextmyHtmlHandler(document, tagmap);
            parser.Parse(file);
        }
    
        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="tagmap"></param>
        public override void Go(IDocListener document, XmlTextReader reader, Hashtable tagmap) {
            parser = new ITextmyHtmlHandler(document, tagmap);
            parser.Parse(reader);
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        public new static void Parse(IDocListener document, XmlDocument xDoc) {
            HtmlParser p = new HtmlParser();
            p.Go(document, xDoc);
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        public new static void Parse(IDocListener document, String file) {
            HtmlParser p = new HtmlParser();
            p.Go(document, file);
        }
    
        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        public new static void Parse(IDocListener document, XmlTextReader reader) {
            HtmlParser p = new HtmlParser();
            p.Go(document, reader);
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public new static void Parse(IDocListener document, XmlDocument xDoc, XmlDocument xTagmap) {
            HtmlParser p = new HtmlParser();
            p.Go(document, xDoc, xTagmap);
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public new static void Parse(IDocListener document, String file, String tagmap) {
            HtmlParser p = new HtmlParser();
            p.Go(document, file, tagmap);
        }
    
        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public new static void Parse(IDocListener document, String file, Hashtable tagmap) {
            HtmlParser p = new HtmlParser();
            p.Go(document, file, tagmap);
        }
    
        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="tagmap"></param>
        public new static void Parse(IDocListener document, XmlTextReader reader, String tagmap) {
            HtmlParser p = new HtmlParser();
            p.Go(document, reader, tagmap);
        }
    
        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="tagmap"></param>
        public new static void Parse(IDocListener document, XmlTextReader reader, Hashtable tagmap) {
            HtmlParser p = new HtmlParser();
            p.Go(document, reader, tagmap);
        }
    }
}