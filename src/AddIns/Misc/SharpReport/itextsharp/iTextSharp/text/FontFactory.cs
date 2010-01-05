using System;
using System.IO;
using System.Collections;
using System.util;
using System.Globalization;

using iTextSharp.text.html;
using iTextSharp.text.pdf;
using iTextSharp.text;

/*
 * $Id: FontFactory.cs,v 1.16 2008/05/13 11:25:10 psoares33 Exp $
 * 
 *
 * Copyright 2002 by Bruno Lowagie.
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
namespace iTextSharp.text {
    /// <summary>
    /// If you are using True Type fonts, you can declare the paths of the different ttf- and ttc-files
    /// to this static class first and then create fonts in your code using one of the static getFont-method
    /// without having to enter a path as parameter.
    /// </summary>
    public sealed class FontFactory {
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string COURIER = BaseFont.COURIER;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string COURIER_BOLD = BaseFont.COURIER_BOLD;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string COURIER_OBLIQUE = BaseFont.COURIER_OBLIQUE;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string COURIER_BOLDOBLIQUE = BaseFont.COURIER_BOLDOBLIQUE;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string HELVETICA = BaseFont.HELVETICA;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string HELVETICA_BOLD = BaseFont.HELVETICA_BOLD;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string HELVETICA_OBLIQUE = BaseFont.HELVETICA_OBLIQUE;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string HELVETICA_BOLDOBLIQUE = BaseFont.HELVETICA_BOLDOBLIQUE;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string SYMBOL = BaseFont.SYMBOL;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string TIMES = "Times";
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string TIMES_ROMAN = BaseFont.TIMES_ROMAN;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string TIMES_BOLD = BaseFont.TIMES_BOLD;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string TIMES_ITALIC = BaseFont.TIMES_ITALIC;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string TIMES_BOLDITALIC = BaseFont.TIMES_BOLDITALIC;
    
        /// <summary> This is a possible value of a base 14 type 1 font </summary>
        public const string ZAPFDINGBATS = BaseFont.ZAPFDINGBATS;
        
        private static FontFactoryImp fontImp = new FontFactoryImp();

        /// <summary> This is the default encoding to use. </summary>
        private static string defaultEncoding = BaseFont.WINANSI;
    
        /// <summary> This is the default value of the <VAR>embedded</VAR> variable. </summary>
        private static bool defaultEmbedding = BaseFont.NOT_EMBEDDED;
    
        /// <summary> Creates new FontFactory </summary>
        private FontFactory() {
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, string encoding, bool embedded, float size, int style, Color color) {
            return fontImp.GetFont(fontname, encoding, embedded, size, style, color);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <param name="cached">true if the font comes from the cache or is added to the cache if new, false if the font is always created new</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, string encoding, bool embedded, float size, int style, Color color, bool cached) {
            return fontImp.GetFont(fontname, encoding, embedded, size, style, color, cached);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="attributes">the attributes of a Font object</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(Properties attributes) {
            fontImp.DefaultEmbedding = defaultEmbedding;
            fontImp.DefaultEncoding = defaultEncoding;
            return fontImp.GetFont(attributes);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, string encoding, bool embedded, float size, int style) {
            return GetFont(fontname, encoding, embedded, size, style, null);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <param name="size">the size of this font</param>
        /// <returns></returns>
        public static Font GetFont(string fontname, string encoding, bool embedded, float size) {
            return GetFont(fontname, encoding, embedded, size, Font.UNDEFINED, null);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, string encoding, bool embedded) {
            return GetFont(fontname, encoding, embedded, Font.UNDEFINED, Font.UNDEFINED, null);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, string encoding, float size, int style, Color color) {
            return GetFont(fontname, encoding, defaultEmbedding, size, style, color);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, string encoding, float size, int style) {
            return GetFont(fontname, encoding, defaultEmbedding, size, style, null);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="size">the size of this font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, string encoding, float size) {
            return GetFont(fontname, encoding, defaultEmbedding, size, Font.UNDEFINED, null);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, string encoding) {
            return GetFont(fontname, encoding, defaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, float size, int style, Color color) {
            return GetFont(fontname, defaultEncoding, defaultEmbedding, size, style, color);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, float size, Color color) {
            return GetFont(fontname, defaultEncoding, defaultEmbedding, size, Font.UNDEFINED, color);
        }
        
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, float size, int style) {
            return GetFont(fontname, defaultEncoding, defaultEmbedding, size, style, null);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="size">the size of this font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname, float size) {
            return GetFont(fontname, defaultEncoding, defaultEmbedding, size, Font.UNDEFINED, null);
        }
    
        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <returns>a Font object</returns>
        public static Font GetFont(string fontname) {
            return GetFont(fontname, defaultEncoding, defaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);
        }

        /**
        * Register a font by giving explicitly the font family and name.
        * @param familyName the font family
        * @param fullName the font name
        * @param path the font path
        */
        public void RegisterFamily(String familyName, String fullName, String path) {
            fontImp.RegisterFamily(familyName, fullName, path);
        }
        
        public static void Register(Properties attributes) {
            string path;
            string alias = null;

            path = attributes.Remove("path");
            alias = attributes.Remove("alias");

            fontImp.Register(path, alias);
        }
    
        /// <summary>
        /// Register a ttf- or a ttc-file.
        /// </summary>
        /// <param name="path">the path to a ttf- or ttc-file</param>
        public static void Register(string path) {
            Register(path, null);
        }
    
        /// <summary>
        /// Register a ttf- or a ttc-file and use an alias for the font contained in the ttf-file.
        /// </summary>
        /// <param name="path">the path to a ttf- or ttc-file</param>
        /// <param name="alias">the alias you want to use for the font</param>
        public static void Register(string path, string alias) {
            fontImp.Register(path, alias);
        }
    
        /** Register all the fonts in a directory.
        * @param dir the directory
        * @return the number of fonts registered
        */    
        public static int RegisterDirectory(String dir) {
            return fontImp.RegisterDirectory(dir);
        }

        /**
        * Register all the fonts in a directory and possibly its subdirectories.
        * @param dir the directory
        * @param scanSubdirectories recursively scan subdirectories if <code>true</true>
        * @return the number of fonts registered
        * @since 2.1.2
        */
        public static int RegisterDirectory(String dir, bool scanSubdirectories) {
            return fontImp.RegisterDirectory(dir, scanSubdirectories);
        }
            
        /** Register fonts in some probable directories. It usually works in Windows,
        * Linux and Solaris.
        * @return the number of fonts registered
        */    
        public static int RegisterDirectories() {
            return fontImp.RegisterDirectories();
        }

        /// <summary>
        /// Gets a set of registered fontnames.
        /// </summary>
        /// <value>a set of registered fontnames</value>
        public static ICollection RegisteredFonts {
            get {
                return fontImp.RegisteredFonts;
            }
        }
    
        /// <summary>
        /// Gets a set of registered font families.
        /// </summary>
        /// <value>a set of registered font families</value>
        public static ICollection RegisteredFamilies {
            get {
                return fontImp.RegisteredFamilies;
            }
        }
    
        /// <summary>
        /// Checks whether the given font is contained within the object
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <returns>true if font is contained within the object</returns>
        public static bool Contains(string fontname) {
            return fontImp.IsRegistered(fontname);
        }
    
        /// <summary>
        /// Checks if a certain font is registered.
        /// </summary>
        /// <param name="fontname">the name of the font that has to be checked</param>
        /// <returns>true if the font is found</returns>
        public static bool IsRegistered(string fontname) {
            return fontImp.IsRegistered(fontname);
        }

        public static string DefaultEncoding {
            get {
                return defaultEncoding;
            }
        }

        public static bool DefaultEmbedding {
            get {
                return defaultEmbedding;
            }
        }


        public static FontFactoryImp FontImp {
            get {
                return fontImp;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException("FontFactoryImp cannot be null.");
                fontImp = value;
            }
        }
    }
}