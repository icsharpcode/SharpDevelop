using System;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.parser.ctrlwords;
using iTextSharp.text.rtf.parser.enumerations;
/*
 * $Id: RtfDestinationColorTable.cs,v 1.2 2008/05/13 11:26:00 psoares33 Exp $
 * 
 *
 * Copyright 2007 by Howard Shank (hgshank@yahoo.com)
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
 * the Initial Developer are Copyright (C) 1999-2006 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2006 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
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
namespace iTextSharp.text.rtf.parser.destinations {

    /**
    * <code>RtfDestinationColorTable</code> handles data destined for the color table destination
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    * 
    * @since 2.0.8
    */
    public class RtfDestinationColorTable : RtfDestination  {

        /**
        * The RtfImportHeader to add color mappings to.
        */
        private RtfImportMgr importHeader = null;
        /**
        * The number of the current color being parsed.
        */
        private int colorNr = 0;
        /**
        * The red component of the current color being parsed.
        */
        private int red = -1;
        /**
        * The green component of the current color being parsed.
        */
        private int green = -1;
        /**
        * The blue component of the current color being parsed.
        */
        private int blue = -1;
        /*
        * Color themes - Introduced Word 2007
        */
        /**
        * Specifies the tint when specifying a theme color.
        * RTF control word ctint
        * 
        * 0 - 255: 0 = full Tint(white), 255 = no tint. 
        * Default value: 255
        * 
        * If tint is specified and is less than 255, cshade must equal 255.
        * ctint/cshade are mutually exclusive
        * 
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#cshade
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#themeColor
        */
        private int ctint = 255;
        /**
        * Specifies the shade when specifying a theme color.
        * RTF control word cshade
        * 
        * 0 - 255: 0 = full Shade(black), 255 = no shade. 
        * Default value: 255
        * 
        * If shade is specified and is less than 255, ctint must equal 255.
        * cshade/ctint are mutually exclusive
        * 
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#ctint
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#themeColor
        */
        private int cshade = 255;
        /**
        * Specifies the use of a theme color.
        * 
        * @see com.lowagie.text.rtf.parser.enumerations.RtfColorThemes
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#ctint
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#cshade
        */
        private int themeColor = RtfColorThemes.THEME_UNDEFINED;
        /**
        * Color map object for conversions
        */
        private Hashtable colorMap = null;
        
        /**
        * Constructor.
        */
        public RtfDestinationColorTable() : base(null) {
            colorMap = new Hashtable();
            this.colorNr = 0;
        }
        
        /**
        * Constructs a new RtfColorTableParser.
        * 
        * @param importHeader The RtfImportHeader to add the color mappings to.
        */
        public RtfDestinationColorTable(RtfParser parser) : base(parser) {
            colorMap = new Hashtable();
            this.colorNr = 0;
            this.importHeader = parser.GetImportManager();
            this.SetToDefaults();
        }
        
        public override void SetParser(RtfParser parser) {
            this.rtfParser = parser;
            colorMap = new Hashtable();
            this.colorNr = 0;
            this.importHeader = parser.GetImportManager();
            this.SetToDefaults();
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
        */
        public override bool HandleOpeningSubGroup() {
            return true;
        }

        public override bool CloseDestination() {
            return true;
        }

        public override bool HandleCloseGroup() {
            ProcessColor();
            return true;
        }
        public override bool HandleOpenGroup() {
            return true;
        }
        
        public override bool HandleCharacter(int ch) {
            // color elements end with a semicolon (;)
            if ((char)ch == ';') {
                this.ProcessColor();
            }
            return true;
        }
        
        public override bool HandleControlWord(RtfCtrlWordData ctrlWordData) {
            if (ctrlWordData.ctrlWord.Equals("blue")) this.SetBlue(ctrlWordData.IntValue());
            if (ctrlWordData.ctrlWord.Equals("red")) this.SetRed(ctrlWordData.IntValue());
            if (ctrlWordData.ctrlWord.Equals("green")) this.SetGreen(ctrlWordData.IntValue());
            if (ctrlWordData.ctrlWord.Equals("cshade")) this.SetShade(ctrlWordData.IntValue());
            if (ctrlWordData.ctrlWord.Equals("ctint")) this.SetTint(ctrlWordData.IntValue());
            //if(ctrlWordData.ctrlWord.Equals("cmaindarkone")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("cmainlightone")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("cmaindarktwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("cmainlighttwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("caccentone")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("caccenttwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("caccentthree")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("caccentfour")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("caccentfive")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("caccentsix")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("chyperlink")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("cfollowedhyperlink")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("cbackgroundone")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("ctextone")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("cbacgroundtwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
            //if(ctrlWordData.ctrlWord.Equals("ctexttwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
            return true;
        }
        
        /**
        * Set default values.
        */
        public override void SetToDefaults() {
            this.red = -1;
            this.green = -1;
            this.blue = -1;
            this.ctint = 255;
            this.cshade = 255;
            this.themeColor = RtfColorThemes.THEME_UNDEFINED;
            // do not reset colorNr
        }
        /**
        * Processes the color triplet parsed from the document.
        * Add it to the import mapping so colors can be mapped when encountered
        * in the RTF import or conversion.
        */
        private void ProcessColor() {
            if (red != -1 && green != -1 && blue != -1) {
                if (this.rtfParser.IsImport()) {
                    this.importHeader.ImportColor(this.colorNr.ToString(), new Color(this.red, this.green, this.blue));
                }
            
                if (this.rtfParser.IsConvert()) {
                    colorMap[this.colorNr.ToString()] = new Color(this.red, this.green, this.blue);
                }
            }
            this.SetToDefaults();
            this.colorNr++;
        }
        /**
        * Set the red color to value.
        * @param value Value to set red to.
        */
        private void SetRed(int value) {
            if (value >= 0 && value <= 255) {
                this.red = value;
            }
        }
        /**
        * Set the green color value.
        * @param value Value to set green to.
        */
        private void SetGreen(int value) {
            if (value >= 0 && value <= 255) {
                this.green = value;
            }
        }
        /**
        * Set the blue color value.
        * @param value Value to set blue to.
        */
        private void SetBlue(int value) {
            if (value >= 0 && value <= 255) {
                this.blue = value;
            }
        }
        /**
        * Set the tint value
        * @param value Value to set the tint to
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#ctint
        */
        private void SetTint(int value) {
            if (value >= 0 && value <= 255) {
                this.ctint = value;
                if (value >= 0 && value <255) {
                    this.cshade = 255;
                }
            }
        }
        /**
        * Set the shade value
        * @param value Value to set the shade to
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#cshade
        */
        private void SetShade(int value) {
            if (value >= 0 && value <= 255) {
                this.cshade = value;
                if (value >= 0 && value <255) {
                    this.ctint = 255;
                }
            }
        }
        /**
        * Set the theme color value.
        * @param value Value to set the theme color to
        * @see com.lowagie.text.rtf.parser.enumerations.RtfColorThemes
        */
        private void SetThemeColor(int value) {
            if (value >= RtfColorThemes.THEME_UNDEFINED && value <= RtfColorThemes.THEME_MAX) {
                this.themeColor = value;
            } else {
                this.themeColor = RtfColorThemes.THEME_UNDEFINED;
            }
        }
        
        // conversion functions
        /**
        * Get the <code>Color</code> object that is mapped to the key.
        * @param key The map number.
        * *@return <code>Color</code> object from the map. null if key does not exist.
        */
        public Color GetColor(String key) {
            return (Color)colorMap[key];
        }
        
    }
}