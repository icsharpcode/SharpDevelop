using System;
using System.Collections;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.direct;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.parser.ctrlwords;
using iTextSharp.text.rtf.parser.properties;
/*
 * $Id: RtfDestinationDocument.cs,v 1.4 2008/05/13 11:26:00 psoares33 Exp $
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
    * <code>RtfDestinationDocument</code> handles data destined for the document destination
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    *
    */
    public sealed class RtfDestinationDocument : RtfDestination, IRtfPropertyListener {


        /**
        * The RtfDocument object.
        * 
        * @see com.lowagie.text.rtf.document.RtfDocument
        */
        private RtfDocument rtfDoc = null;
        
        /**
        * The iText Document object.
        * 
        * @see com.lowagie.text.Document
        */
        private Document doc = null;
        
        private StringBuilder buffer = null;
        /**
        * Indicates the parser action. Import or Conversion.
        * 
        * @see com.lowagie.text.rtf.direct.RtfParser#TYPE_UNIDENTIFIED
        * @see com.lowagie.text.rtf.direct.RtfParser#TYPE_CONVERT
        * @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FRAGMENT
        * @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FULL
        */
        private int conversionType = 0;
        
        
        /**
        * Indicates the current table level being processed
        */
        private int tableLevel = 0;
        
        private static ArrayList IMPORT_IGNORED_CTRLWORDS = new ArrayList(new string[]{
            "rtf",
            "ansicpg",
            "deff",
            "ansi",
            "mac",
            "pca",
            "pc",
            "stshfdbch",
            "stshfloch",
            "stshfhich",
            "stshfbi",
            "deflang",
            "deflangfe",
            "adeflang",
            "adeflangfe"
            }
        );
        private static ArrayList CONVERT_IGNORED_CTRLWORDS = new ArrayList(new string[]{"rtf"});

        private Paragraph iTextParagraph = null;
        
        public RtfDestinationDocument() : base(null) {
        }
        /**
        * Constructs a new <code>RtfDestinationDocument</code> using
        * the parameters to initialize the object.
        * @param rtfDoc The <code>RtfDocument</code> this works with.
        * @param doc The iText <code>Document</code> this works with.
        * @param type The type of conversion being done.
        */
        public RtfDestinationDocument(RtfParser parser) : base (parser){
            this.rtfDoc = parser.GetRtfDocument();
            this.doc = parser.GetDocument();
            this.conversionType = parser.GetConversionType();
            SetToDefaults();
            if (this.rtfParser.IsConvert()) {
                this.rtfParser.GetState().properties.AddRtfPropertyListener(this);
            }
        }
        
        public override void SetParser(RtfParser parser) {
            this.rtfParser = parser;
            this.rtfDoc = parser.GetRtfDocument();
            this.doc = parser.GetDocument();
            this.conversionType = parser.GetConversionType();
            SetToDefaults();
            if (this.rtfParser.IsConvert()) {
                this.rtfParser.GetState().properties.AddRtfPropertyListener(this);
            }
        }
        

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
        */
        public override bool CloseDestination() {
            if (this.rtfParser.IsImport()) {
                if (this.buffer.Length>0) {
                    WriteBuffer();
                }
            }
            
            this.rtfParser.GetState().properties.RemoveRtfPropertyListener(this);

            return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
        */
        public override bool HandleOpenGroup() {
            this.OnOpenGroup(); // event handler
            
            if (this.rtfParser.IsImport()) {
            }
            if (this.rtfParser.IsConvert()) {
                if (this.iTextParagraph == null) this.iTextParagraph = new Paragraph();
            }
            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
        */
        public override bool HandleOpeningSubGroup() {
            if (this.rtfParser.IsImport()) {
                if (this.buffer.Length>0) {
                    WriteBuffer();
                }
            }
            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
        */
        public override bool HandleCloseGroup() {
            this.OnCloseGroup();    // event handler
            
            if (this.rtfParser.IsImport()) {
                if (this.buffer.Length>0) {
                    WriteBuffer();
                }
                WriteText("}");
            }
            if (this.rtfParser.IsConvert()) {
                if (this.buffer.Length > 0 && this.iTextParagraph == null) {
                    this.iTextParagraph = new Paragraph();
                }
                if (this.buffer.Length > 0 ) {
                    Chunk chunk = new Chunk();
                    chunk.Append(this.buffer.ToString());
                    this.iTextParagraph.Add(chunk);
                }
                if (this.iTextParagraph != null) {
                    AddParagraphToDocument();
                }
            }
            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
        */
        public override bool HandleCharacter(int ch) {
            bool result = true;
            this.OnCharacter(ch);   // event handler
            
            if (this.rtfParser.IsImport()) {
                if (buffer.Length > 254) {
                    this.WriteBuffer();
                }
                buffer.Append((char)ch);
            }
            if (this.rtfParser.IsConvert()) {
                buffer.Append((char)ch);
            }
            return result;
        }

        
        public override bool HandleControlWord(RtfCtrlWordData ctrlWordData) {
            bool result = false;
            this.OnCtrlWord(ctrlWordData);  // event handler
            
            if (this.rtfParser.IsImport()) {
                // map font information
                if (ctrlWordData.ctrlWord.Equals("f")) { ctrlWordData.param =  this.rtfParser.GetImportManager().MapFontNr(ctrlWordData.param);}
                
                // map color information
                //colors
                if (ctrlWordData.ctrlWord.Equals("cb")) { ctrlWordData.param = this.rtfParser.GetImportManager().MapColorNr(ctrlWordData.param);}
                if (ctrlWordData.ctrlWord.Equals("cf")) { ctrlWordData.param = this.rtfParser.GetImportManager().MapColorNr(ctrlWordData.param);}
                //cells
                if (ctrlWordData.ctrlWord.Equals("clcbpat")) { ctrlWordData.param = this.rtfParser.GetImportManager().MapColorNr(ctrlWordData.param);}
                if (ctrlWordData.ctrlWord.Equals("clcbpatraw")) { ctrlWordData.param = this.rtfParser.GetImportManager().MapColorNr(ctrlWordData.param);}
                if (ctrlWordData.ctrlWord.Equals("clcfpat")) { ctrlWordData.param = this.rtfParser.GetImportManager().MapColorNr(ctrlWordData.param);}
                if (ctrlWordData.ctrlWord.Equals("clcfpatraw")) { ctrlWordData.param = this.rtfParser.GetImportManager().MapColorNr(ctrlWordData.param);}
                //table rows
                if (ctrlWordData.ctrlWord.Equals("trcfpat")) { ctrlWordData.param = this.rtfParser.GetImportManager().MapColorNr(ctrlWordData.param);}
                if (ctrlWordData.ctrlWord.Equals("trcbpat")) { ctrlWordData.param = this.rtfParser.GetImportManager().MapColorNr(ctrlWordData.param);}
                //paragraph border
                if (ctrlWordData.ctrlWord.Equals("brdrcf")) { ctrlWordData.param = this.rtfParser.GetImportManager().MapColorNr(ctrlWordData.param);}
            }
            

            
            if (this.rtfParser.IsConvert()) {
                if (ctrlWordData.ctrlWord.Equals("par")) { AddParagraphToDocument(); }
                // Set Font
                if (ctrlWordData.ctrlWord.Equals("f")) {}
                
                // color information
                //colors
                if (ctrlWordData.ctrlWord.Equals("cb")) {}
                if (ctrlWordData.ctrlWord.Equals("cf")) {}
                //cells
                if (ctrlWordData.ctrlWord.Equals("clcbpat")) {}
                if (ctrlWordData.ctrlWord.Equals("clcbpatraw")) {}
                if (ctrlWordData.ctrlWord.Equals("clcfpat")) {}
                if (ctrlWordData.ctrlWord.Equals("clcfpatraw")) {}
                //table rows
                if (ctrlWordData.ctrlWord.Equals("trcfpat")) {}
                if (ctrlWordData.ctrlWord.Equals("trcbpat")) {}
                //paragraph border
                if (ctrlWordData.ctrlWord.Equals("brdrcf")) {}
                
                /* TABLES */
                if (ctrlWordData.ctrlWord.Equals("trowd")) /*Beginning of row*/ { tableLevel++;}
                if (ctrlWordData.ctrlWord.Equals("cell")) /*End of Cell Denotes the end of a table cell*/ {
    //              String ctl = ctrlWordData.ctrlWord;
    //              System.out.Print("cell found");
                }
                if (ctrlWordData.ctrlWord.Equals("row")) /*End of row*/ { tableLevel++;}
                if (ctrlWordData.ctrlWord.Equals("lastrow")) /*Last row of the table*/ {}
                if (ctrlWordData.ctrlWord.Equals("row")) /*End of row*/ { tableLevel++;}
                if (ctrlWordData.ctrlWord.Equals("irow")) /*param  is the row index of this row.*/ {}
                if (ctrlWordData.ctrlWord.Equals("irowband")) /*param is the row index of the row, adjusted to account for header rows. A header row has a value of -1.*/ {}
                if (ctrlWordData.ctrlWord.Equals("tcelld")) /*Sets table cell defaults*/ {}
                if (ctrlWordData.ctrlWord.Equals("nestcell")) /*Denotes the end of a nested cell.*/ {}
                if (ctrlWordData.ctrlWord.Equals("nestrow")) /*Denotes the end of a nested row*/ {}
                if (ctrlWordData.ctrlWord.Equals("nesttableprops")) /*Defines the properties of a nested table. This is a destination control word*/ {}
                if (ctrlWordData.ctrlWord.Equals("nonesttables")) /*Contains text for readers that do not understand nested tables. This destination should be ignored by readers that support nested tables.*/ {}
                if (ctrlWordData.ctrlWord.Equals("trgaph")) /*Half the space between the cells of a table row in twips.*/ {}
                if (ctrlWordData.ctrlWord.Equals("cellx")) /*param Defines the right boundary of a table cell, including its half of the space between cells.*/ {}
                if (ctrlWordData.ctrlWord.Equals("clmgf")) /*The first cell in a range of table cells to be merged.*/ {}
                if (ctrlWordData.ctrlWord.Equals("clmrg")) /*Contents of the table cell are merged with those of the preceding cell*/ {}
                if (ctrlWordData.ctrlWord.Equals("clvmgf")) /*The first cell in a range of table cells to be vertically merged.*/ {}
                if (ctrlWordData.ctrlWord.Equals("clvmrg")) /*Contents of the table cell are vertically merged with those of the preceding cell*/ {}
                /* TABLE: table row revision tracking */
                if (ctrlWordData.ctrlWord.Equals("trauth")) /*With revision tracking enabled, this control word identifies the author of changes to a table row's properties. N refers to a value in the revision table*/ {}
                if (ctrlWordData.ctrlWord.Equals("trdate")) /*With revision tracking enabled, this control word identifies the date of a revision*/ {}
                /* TABLE: Autoformatting flags */
                if (ctrlWordData.ctrlWord.Equals("tbllkborder")) /*Flag sets table autoformat to format borders*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllkshading")) /*Flag sets table autoformat to affect shading.*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllkfont")) /*Flag sets table autoformat to affect font*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllkcolor")) /*Flag sets table autoformat to affect color*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllkbestfit")) /*Flag sets table autoformat to apply best fit*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllkhdrrows")) /*Flag sets table autoformat to format the first (header) row*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllklastrow")) /*Flag sets table autoformat to format the last row.*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllkhdrcols")) /*Flag sets table autoformat to format the first (header) column*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllklastcol")) /*Flag sets table autoformat to format the last column*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllknorowband")) /*Specifies row banding conditional formatting shall not be applied*/ {}
                if (ctrlWordData.ctrlWord.Equals("tbllknocolband")) /*Specifies column banding conditional formatting shall not be applied.*/ {}
                /* TABLE: Row Formatting */
                if (ctrlWordData.ctrlWord.Equals("taprtl")) /*Table direction is right to left*/ {}
                if (ctrlWordData.ctrlWord.Equals("trautofit")) /*param = AutoFit:
    0   No AutoFit (default).
    1   AutoFit is on for the row. Overridden by \clwWidthN and \trwWidthN in any table row.
    */ {}
                if (ctrlWordData.ctrlWord.Equals("trhdr")) /*Table row header. This row should appear at the top of every page on which the current table appears*/ {}
                if (ctrlWordData.ctrlWord.Equals("trkeep")) /*Keep table row together. This row cannot be split by a page break. This property is assumed to be off unless the control word is present*/ {}
                if (ctrlWordData.ctrlWord.Equals("trkeepfollow")) /*Keep row in the same page as the following row.*/ {}
                if (ctrlWordData.ctrlWord.Equals("trleft")) /*Position in twips of the leftmost edge of the table with respect to the left edge of its column.*/ {}
                if (ctrlWordData.ctrlWord.Equals("trqc")) /*Centers a table row with respect to its containing column.*/ {}
                if (ctrlWordData.ctrlWord.Equals("trql")) /*Left-justifies a table row with respect to its containing column.*/ {}
                if (ctrlWordData.ctrlWord.Equals("trqr")) /*Right-justifies a table row with respect to its containing column*/ {}
                if (ctrlWordData.ctrlWord.Equals("trrh")) /*Height of a table row in twips. When 0, the height is sufficient for all the text in the line; when positive, the height is guaranteed to be at least the specified height; when negative, the absolute value of the height is used, regardless of the height of the text in the line*/ {}
                if (ctrlWordData.ctrlWord.Equals("trpaddb")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trpaddl")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trpaddr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trpaddt")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trpaddfb")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trpaddfl")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trpaddfr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trpaddft")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trspdl")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trspdt")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trspdb")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trspdr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trspdfl")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trspdft")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trspdfb")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trspdfr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trwWidth")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trftsWidth")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trwWidthB")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trftsWidthB")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trftsWidthB")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trwWidthA")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trftsWidthA")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tblind")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tblindtype")) /* */ {}
                /*TABLE: Row shading and Background Colors*/
                if (ctrlWordData.ctrlWord.Equals("trcbpat")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trcfpat")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trpat")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trshdng")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgbdiag")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgcross")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgdcross")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgdkbdiag")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgdkcross")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgdkdcross")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgdkfdiag")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgdkhor")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgdkvert")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgfdiag")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbghoriz")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbgvert")) /* */ {}
                /* TABLE: Cell Formatting*/
                if (ctrlWordData.ctrlWord.Equals("clFitText")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clNoWrap")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clpadl")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clpadt")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clpadb")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clpadr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clpadfl")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clpadft")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clpadfb")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clpadfr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clwWidth")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clftsWidth")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clhidemark")) /* */ {}
                /* TABLE: Compared Table Cells */
                if (ctrlWordData.ctrlWord.Equals("clins")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("cldel")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clmrgd")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clmrgdr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clsplit")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clsplitr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clinsauth")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clinsdttm")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("cldelauth")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("cldeldttm")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clmrgdauth")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clmrgddttm")) /* */ {}
                /*TABLE: Position Wrapped Tables (The following properties must be the same for all rows in the table.)*/
                if (ctrlWordData.ctrlWord.Equals("tdfrmtxtLeft")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tdfrmtxtRight")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tdfrmtxtTop")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tdfrmtxtBottom")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tabsnoovrlp")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tphcol")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tphmrg")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tphpg")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposnegx")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposnegy")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposx")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposxc")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposxi")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposxl")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposxo")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposxr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposy")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposyb")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposyc")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposyil")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposyin")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposyout")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tposyt")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tpvmrg")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tpvpara")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("tpvpg")) /* */ {}
                /* TABLE: Bidirectional Controls */
                if (ctrlWordData.ctrlWord.Equals("rtlrow")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("ltrrow")) /* */ {}
                /* TABLE: Row Borders */
                if (ctrlWordData.ctrlWord.Equals("trbrdrt")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbrdrl")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbrdrb")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbrdrr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbrdrh")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("trbrdrv")) /* */ {}
                /* TABLE: Cell Borders */
                if (ctrlWordData.ctrlWord.Equals("brdrnil")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clbrdrb")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clbrdrt")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clbrdrl")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("clbrdrr")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("cldglu")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("cldgll")) /* */ {}
                if (ctrlWordData.ctrlWord.Equals("")) /* */ {}
            }
            if (ctrlWordData.ctrlWordType == RtfCtrlWordType.TOGGLE) {
                this.rtfParser.GetState().properties.ToggleProperty(ctrlWordData);//ctrlWordData.specialHandler);
            }
            
            if (ctrlWordData.ctrlWordType == RtfCtrlWordType.FLAG || 
                    ctrlWordData.ctrlWordType == RtfCtrlWordType.VALUE) {
                this.rtfParser.GetState().properties.SetProperty(ctrlWordData);//ctrlWordData.specialHandler, ctrlWordData.param);
            }
            
            switch (conversionType) {
            case RtfParser.TYPE_IMPORT_FULL:
                if (!IMPORT_IGNORED_CTRLWORDS.Contains(ctrlWordData.ctrlWord)) {
                    WriteBuffer();
                    WriteText(ctrlWordData.ToString());
                }
                result = true;
                break;      
            case RtfParser.TYPE_IMPORT_FRAGMENT:
                if (!IMPORT_IGNORED_CTRLWORDS.Contains(ctrlWordData.ctrlWord)) {
                    WriteBuffer();
                    WriteText(ctrlWordData.ToString());
                }
                result = true;
                break;
            case RtfParser.TYPE_CONVERT:
                if (IMPORT_IGNORED_CTRLWORDS.Contains(ctrlWordData.ctrlWord) == false) {
                }
                result = true;
                break;
            default:    // error because is should be an import or convert
                result = false;
                break;
            }
            
            
            
            
            return result;
        }
        /**
        * Write the accumulated buffer to the destination.
        * Used for direct content
        */
        private void WriteBuffer() {
            WriteText(this.buffer.ToString());
            SetToDefaults();
        }
        /**
        * Write the string value to the destiation.
        * Used for direct content
        * @param value
        */
        private void WriteText(String value) {
            if (this.rtfParser.IsNewGroup()) {
                this.rtfDoc.Add(new RtfDirectContent("{"));
                this.rtfParser.SetNewGroup(false);
            }
            if (value.Length > 0) {
                this.rtfDoc.Add(new RtfDirectContent(value));
            }
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#setDefaults()
        */
        public override void SetToDefaults() {
            this.buffer = new StringBuilder();
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.properties.RtfPropertyListener#afterChange(java.lang.String)
        */
        public void AfterPropertyChange(String propertyName) {
            if (propertyName.StartsWith(RtfProperty.CHARACTER)) {
            } else {
                if (propertyName.StartsWith(RtfProperty.PARAGRAPH)) {
                } else {
                    if (propertyName.StartsWith(RtfProperty.SECTION)) {
                    } else {
                        if (propertyName.StartsWith(RtfProperty.DOCUMENT)) {

                        }
                    }
                }
            }       
        }
        
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.properties.RtfPropertyListener#beforeChange(java.lang.String)
        */
        public void BeforePropertyChange(String propertyName) {
            // do we have any text to do anything with?
            // if not, then just return without action.
            if (this.buffer.Length == 0) return;
            
            if (propertyName.StartsWith(RtfProperty.CHARACTER)) {
                // this is a character change,
                // add a new chunck to the current paragraph using current character settings.
                Chunk chunk = new Chunk();
                chunk.Append(this.buffer.ToString());
                this.buffer = new StringBuilder(255);
                Hashtable charProperties = this.rtfParser.GetState().properties.GetProperties(RtfProperty.CHARACTER);
                String defFont = (String)charProperties[RtfProperty.CHARACTER_FONT];
                if (defFont == null) defFont = "0";
                RtfDestinationFontTable fontTable = (RtfDestinationFontTable)this.rtfParser.GetDestination("fonttbl");
                Font currFont = fontTable.GetFont(defFont);
                int fs = Font.NORMAL;
                if (charProperties.ContainsKey(RtfProperty.CHARACTER_BOLD)) fs |= Font.BOLD; 
                if (charProperties.ContainsKey(RtfProperty.CHARACTER_ITALIC)) fs |= Font.ITALIC;
                if (charProperties.ContainsKey(RtfProperty.CHARACTER_UNDERLINE)) fs |= Font.UNDERLINE;
                Font useFont = FontFactory.GetFont(currFont.Familyname, 12, fs, new Color(0,0,0));
                
                
                chunk.Font = useFont;
                if (iTextParagraph == null) this.iTextParagraph = new Paragraph();
                this.iTextParagraph.Add(chunk);

            } else {
                if (propertyName.StartsWith(RtfProperty.PARAGRAPH)) {
                    // this is a paragraph change. what do we do?
                } else {
                    if (propertyName.StartsWith(RtfProperty.SECTION)) {
                        
                    } else {
                        if (propertyName.StartsWith(RtfProperty.DOCUMENT)) {

                        }
                    }
                }
            }       
        }
        
        private void AddParagraphToDocument() {
            if (this.iTextParagraph != null) {
                try {
                    this.rtfParser.GetDocument().Add(this.iTextParagraph);
                } catch {
                }
                this.iTextParagraph = null;
            }   
        }
    }
}