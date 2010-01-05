using System;
using iTextSharp.text;
using iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.parser.ctrlwords;
/*
 * $Id: RtfDestinationStylesheetTable.cs,v 1.2 2008/05/13 11:26:00 psoares33 Exp $
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
    * <code>RtfDestinationStylesheetTable</code> handles data destined for the 
    * Stylesheet Table destination
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    *
    */
    public class RtfDestinationStylesheetTable : RtfDestination {
        private String styleName = "";
        /**
        * <code>RtfParagraphStyle</code> object for setting styleshee values
        * as they are parsed from the input.
        */
        //private RtfParagraphStyle rtfParagraphStyle = null;
        
        private String elementName = "";
        
        /**
        * RTF Style number from stylesheet table.
        */
        private int styleNr = 0;
        
        /**
        * What kind of style is this, Paragraph or Character or Table
        */
        private int styleType = RtfStyleTypes.PARAGRAPH;
        
        // Alignment
        /**
        * Alignment - page 85
        *  \qc, \qj, \ql, \qr, \qd, \qkN, \qt 
        */
        private int alignment = Element.ALIGN_LEFT;
        /**
        * Percentage of line occupied by Kashida justification (0 � low, 10 � medium, 20 � high).
        * \qkN
        */
        private int justificationPercentage = 0;
        
        // Indentation
        /**
        * First line indentation.
        */
        private int firstLineIndent = 0;
        /**
        * Left indentation
        */
        private int leftIndent = 0;
        /**
        * Right indentation
        */
        private int rightIndent = 0;
        /**
        *  Automatically adjust right indentation when docunent grid is defined 
        */
        private int adustRightIndent = 0;
        /**
        *  Mirror indents? 
        */
        private int mirrorIndent = 0;
        
        // Document Foratting Properties
        /**
        * Override orphan/widow control.
        */
        private int overrideWidowControl = -1;
        
        // Asian Typography
        /**
        * auto spacing betwee DBC and English
        */
        private int AutoSpaceBetweenDBCEnglish = 0;
        /**
        * auto spacing betwee DBC and numbers
        */
        private int AutoSpaceBetweenDBCNumbers = 0;
        /**
        * No Character wrapping
        */
        private int noCharacterWrapping = 0;
        /**
        * No Word wrapping
        */
        private int noWordWrapping = 0;
        /**
        * No overflow period and comma
        */
        private int noOverflowPeriodComma = 0;
        
        
        
        //////////////////////////////////////////////////////
        /**
        * The RtfImportHeader to add color mappings to.
        */
        private RtfImportMgr importHeader = null;
        private String type = "";
        
        public RtfDestinationStylesheetTable() : base(null) {
        }
        
        public RtfDestinationStylesheetTable(RtfParser parser, String type) : base(parser){
            this.importHeader = parser.GetImportManager();
            this.type = type;
        }
        public override void SetParser(RtfParser parser) {
            this.rtfParser = parser;
            this.importHeader = parser.GetImportManager();
        }
        public void SetType(String value) {
            this.type = value;
        }
        public void SetElementName(String value) {
            this.elementName = value;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
        */
        public override bool HandleOpeningSubGroup() {
            // TODO Auto-generated method stub
            return false;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
        */
        public override bool CloseDestination() {

            return true;
        }

        public override bool HandleControlWord(RtfCtrlWordData ctrlWordData) {
            bool result = true;
            this.OnCtrlWord(ctrlWordData);  // event handler
            
            if (this.rtfParser.IsImport()) {
                // information
                if (ctrlWordData.ctrlWord.Equals("s")) { }
                if (ctrlWordData.ctrlWord.Equals("cs")) {}
                if (ctrlWordData.ctrlWord.Equals("ds")) {}
                if (ctrlWordData.ctrlWord.Equals("ts")) {}
                if (ctrlWordData.ctrlWord.Equals("tsrowd")) {}
                
                if (ctrlWordData.ctrlWord.Equals("keycode")) {}
                if (ctrlWordData.ctrlWord.Equals("shift")) { }
                if (ctrlWordData.ctrlWord.Equals("ctrl")) { }
                if (ctrlWordData.ctrlWord.Equals("alt")) { }
                //cells
                if (ctrlWordData.ctrlWord.Equals("fn")) { }
                if (ctrlWordData.ctrlWord.Equals("additive")) { }
                if (ctrlWordData.ctrlWord.Equals("sbasedon")) { }
                if (ctrlWordData.ctrlWord.Equals("snext")) { }
                if (ctrlWordData.ctrlWord.Equals("sautoupd")) { }
                if (ctrlWordData.ctrlWord.Equals("shidden")) { }
                if (ctrlWordData.ctrlWord.Equals("slink")) { }
                if (ctrlWordData.ctrlWord.Equals("slocked")) { }
                if (ctrlWordData.ctrlWord.Equals("spersonal")) { }
                if (ctrlWordData.ctrlWord.Equals("scompose")) { }
                if (ctrlWordData.ctrlWord.Equals("sreply")) { }
                /* FORMATTING */
                // brdrdef/parfmt/apoctl/tabdef/shading/chrfmt
                
                
                
                if (ctrlWordData.ctrlWord.Equals("styrsid")) { }
                if (ctrlWordData.ctrlWord.Equals("ssemihidden")) { }
                if (ctrlWordData.ctrlWord.Equals("sqformat")) { }
                if (ctrlWordData.ctrlWord.Equals("spriority")) { }
                if (ctrlWordData.ctrlWord.Equals("sunhideused")) { }
                
                /* TABLE STYLES */
                if (ctrlWordData.ctrlWord.Equals("tscellwidth")) { }
                if (ctrlWordData.ctrlWord.Equals("tscellwidthfts")) { }
                if (ctrlWordData.ctrlWord.Equals("tscellpaddt")) { }
                if (ctrlWordData.ctrlWord.Equals("tscellpaddl")) { }
                if (ctrlWordData.ctrlWord.Equals("tscellpaddr")) { }
                if (ctrlWordData.ctrlWord.Equals("tscellpaddb")) { }
                if (ctrlWordData.ctrlWord.Equals("tscellpaddft"))/*0-auto, 3-twips*/ { }
                if (ctrlWordData.ctrlWord.Equals("tscellpaddfl"))/*0-auto, 3-twips*/ { }
                if (ctrlWordData.ctrlWord.Equals("tscellpaddfr"))/*0-auto, 3-twips*/ { }
                if (ctrlWordData.ctrlWord.Equals("tscellpaddfb"))/*0-auto, 3-twips*/ { }
                if (ctrlWordData.ctrlWord.Equals("tsvertalt")) { }
                if (ctrlWordData.ctrlWord.Equals("tsvertalc")) { }
                if (ctrlWordData.ctrlWord.Equals("tsvertalb")) { }
                if (ctrlWordData.ctrlWord.Equals("tsnowrap")) { }
                if (ctrlWordData.ctrlWord.Equals("tscellcfpat")) { }
                if (ctrlWordData.ctrlWord.Equals("tscellcbpat")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbgbdiag")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbgfdiag")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbgcross")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbgdcross")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbgdkcross ")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbgdkdcross")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbghoriz")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbgvert")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbgdkhor")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbgdkvert")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbrdrt")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbrdrb")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbrdrl")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbrdrr")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbrdrh")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbrdrv")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbrdrdgl")) { }
                if (ctrlWordData.ctrlWord.Equals("tsbrdrdgr")) { }
                if (ctrlWordData.ctrlWord.Equals("tscbandsh")) { }
                if (ctrlWordData.ctrlWord.Equals("tscbandsv")) { }
            }
            if (ctrlWordData.ctrlWordType == RtfCtrlWordType.FLAG || 
                    ctrlWordData.ctrlWordType == RtfCtrlWordType.TOGGLE ||
                    ctrlWordData.ctrlWordType == RtfCtrlWordType.VALUE) {
                this.rtfParser.GetState().properties.SetProperty(ctrlWordData);
            }
            
            switch (this.rtfParser.GetConversionType()) {
            case RtfParser.TYPE_IMPORT_FULL:
                result = true;
                break;      
            case RtfParser.TYPE_IMPORT_FRAGMENT:
                result = true;
                break;
            case RtfParser.TYPE_CONVERT:
                result = true;
                break;
            default:    // error because is should be an import or convert
                result = false;
                break;
            }
            return result;
        }
        
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
        */
        public override bool HandleCloseGroup() {

            return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
        */
        public override bool HandleOpenGroup() {

            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
        */
        public override bool HandleCharacter(int ch) {
            styleName += (char)ch;
            return true;
        }
        
        public void CreateNewStyle() {
            //public RtfParagraphStyle(String styleName, String fontName, int fontSize, int fontStyle, Color fontColor)
            //this.rtfParagraphStyle = new RtfParagraphStyle();
        }
        
        /**
        * Set the justification percentage from parsed value.
        * @param percent The justification percentage
        * @return The justification percentage
        */
        public int SetJustificationPercentage(int percent) {
            this.justificationPercentage = percent;
            return this.justificationPercentage;
        }
        /**
        * Get the justification percentage.
        * @return The justification percentage value.
        */
        public int GetJustificationPercentage() {
            return this.justificationPercentage;
        }
        /**
        * Set the alignment value from the parsed value.
        * @param alignment The alignment value.
        * @return The alignment value.
        */
        public int SetAlignment(int alignment) {
            this.alignment = alignment;
            return this.alignment;
        }
        /**
        * Get the alignment value.
        * @return The alignment value.
        */
        public int GetAlignment() {
            return this.alignment;
        }
        /**
        * Get the first line indent value.
        * 
        * @return the firstLineIndent
        */
        public int GetFirstLineIndent() {
            return firstLineIndent;
        }
        /**
        * Set the first line indent value.
        * @param firstLineIndent the firstLineIndent to set
        */
        public void SetFirstLineIndent(int firstLineIndent) {
            this.firstLineIndent = firstLineIndent;
        }
        /**
        * Get the left indent value
        * @return the left indent
        */
        public int GetIndent() {
            return leftIndent;
        }
        /**
        * Set the left indent value from the value parsed.
        * @param indent the left indent value.
        */
        public void SetIndent(int indent) {
            this.leftIndent = indent;
        }
        /**
        * Get the right indent adjustment value
        * @return the adustRightIndent value
        */
        public int GetAdustRightIndent() {
            return adustRightIndent;
        }
        /**
        * Set the right indent adjustment value
        * @param adustRightIndent the adustRightIndent to set
        */
        public void SetAdustRightIndent(int adustRightIndent) {
            this.adustRightIndent = adustRightIndent;
        }
        /**
        * Get the left indent value
        * @return the leftIndent
        */
        public int GetLeftIndent() {
            return leftIndent;
        }
        /**
        * Set the left indent value
        * @param leftIndent the leftIndent to set
        */
        public void SetLeftIndent(int leftIndent) {
            this.leftIndent = leftIndent;
        }
        /**
        * Get the value indicating if document has mirrored indents.
        * 
        * @return the mirrorIndent
        */
        public int GetMirrorIndent() {
            return mirrorIndent;
        }
        /**
        * Set the mirrored indent value from the parsed value.
        * 
        * @param mirrorIndent the mirrorIndent to set
        */
        public void SetMirrorIndent(int mirrorIndent) {
            this.mirrorIndent = mirrorIndent;
        }
        /**
        * Get the right indent value.
        * 
        * @return the rightIndent
        */
        public int GetRightIndent() {
            return rightIndent;
        }
        /**
        * Set the right indent value.
        * 
        * @param rightIndent the rightIndent to set
        */
        public void SetRightIndent(int rightIndent) {
            this.rightIndent = rightIndent;
        }
        /**
        * Get the ovirride widow control value.
        * 
        * @return the overrideWidowControl
        */
        public int GetOverrideWidowControl() {
            return overrideWidowControl;
        }
        /**
        * Set the override widow control.
        * 
        * @param overrideWidowControl the overrideWidowControl to set
        */
        public void SetOverrideWidowControl(int overrideWidowControl) {
            this.overrideWidowControl = overrideWidowControl;
        }
        /**
        * Get the auto space between DBC and English indicator.
        * 
        * @return the autoSpaceBetweenDBCEnglish
        */
        public int GetAutoSpaceBetweenDBCEnglish() {
            return AutoSpaceBetweenDBCEnglish;
        }
        /**
        * Set the auto space between DBC and English indicator.
        * 
        * @param autoSpaceBetweenDBCEnglish the autoSpaceBetweenDBCEnglish to set
        */
        public void SetAutoSpaceBetweenDBCEnglish(int autoSpaceBetweenDBCEnglish) {
            AutoSpaceBetweenDBCEnglish = autoSpaceBetweenDBCEnglish;
        }
        /**
        * Get the auto space between DBC and Numbers indicator.
        * @return the autoSpaceBetweenDBCNumbers
        */
        public int GetAutoSpaceBetweenDBCNumbers() {
            return AutoSpaceBetweenDBCNumbers;
        }
        /**
        * Set the auto space between DBC and Numbers indicator.
        * @param autoSpaceBetweenDBCNumbers the autoSpaceBetweenDBCNumbers to set
        */
        public void SetAutoSpaceBetweenDBCNumbers(int autoSpaceBetweenDBCNumbers) {
            AutoSpaceBetweenDBCNumbers = autoSpaceBetweenDBCNumbers;
        }
        /**
        * Get no character wrapping indicator.
        * 
        * @return the noCharacterWrapping
        */
        public int GetNoCharacterWrapping() {
            return noCharacterWrapping;
        }
        /**
        * Set the no character wrapping indicator from parsed value
        * 
        * @param noCharacterWrapping the noCharacterWrapping to set
        */
        public void SetNoCharacterWrapping(int noCharacterWrapping) {
            this.noCharacterWrapping = noCharacterWrapping;
        }
        /**
        * Get the no overflow period comma indicator.
        * 
        * @return the noOverflowPeriodComma
        */
        public int GetNoOverflowPeriodComma() {
            return noOverflowPeriodComma;
        }
        /**
        * Set the no overflow period comma indicator from the parsed value.
        * 
        * @param noOverflowPeriodComma the noOverflowPeriodComma to set
        */
        public void SetNoOverflowPeriodComma(int noOverflowPeriodComma) {
            this.noOverflowPeriodComma = noOverflowPeriodComma;
        }
        /**
        * Get the no word wrapping indicator.
        * 
        * @return the noWordWrapping
        */
        public int GetNoWordWrapping() {
            return noWordWrapping;
        }
        /**
        * Set the no word wrapping indicator from the parsed value.
        * 
        * @param noWordWrapping the noWordWrapping to set
        */
        public void SetNoWordWrapping(int noWordWrapping) {
            this.noWordWrapping = noWordWrapping;
        }
        /**
        * Get this style number.
        * 
        * @return the styleNr
        */
        public int GetStyleNr() {
            return styleNr;
        }
        /**
        * Set this style number from the parsed value.
        * 
        * @param styleNr the styleNr to set
        */
        public void SetStyleNr(int styleNr) {
            this.styleNr = styleNr;
        }
        /**
        * Get this style type.
        * For example Style, Character Style, etc.
        * 
        * @return the styleType
        */
        public int GetStyleType() {
            return styleType;
        }
        /**
        * Set the style type.
        * 
        * @param styleType the styleType to set
        */
        public void SetStyleType(int styleType) {
            this.styleType = styleType;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#setToDefaults()
        */
        public override void SetToDefaults() {
            styleName = "";
            styleNr = 0;
            alignment = Element.ALIGN_LEFT;
            justificationPercentage = 0;
            firstLineIndent = 0;
            leftIndent = 0;
            rightIndent = 0;
            adustRightIndent = 0;
            mirrorIndent = 0;
            overrideWidowControl = -1;
            AutoSpaceBetweenDBCEnglish = 0;
            AutoSpaceBetweenDBCNumbers = 0;
            noCharacterWrapping = 0;
            noWordWrapping = 0;
            noOverflowPeriodComma = 0;
        }
    }
}