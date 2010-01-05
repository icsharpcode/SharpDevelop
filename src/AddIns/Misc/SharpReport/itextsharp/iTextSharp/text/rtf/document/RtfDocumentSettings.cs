using System;
using iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.document.output;

/*
 * $Id: RtfDocumentSettings.cs,v 1.10 2008/05/16 19:30:51 psoares33 Exp $
 * 
 *
 * Copyright 2003, 2004, 2005 by Mark Hall
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

namespace iTextSharp.text.rtf.document {

    /**
    * The RtfDocumentSettings contains output specific settings. These settings modify
    * how the actual document is then generated and some settings may mean that some
    * RTF readers can't read the document or render it wrongly.
    * 
    * @version $Id: RtfDocumentSettings.cs,v 1.10 2008/05/16 19:30:51 psoares33 Exp $
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Thomas Bickel (tmb99@inode.at)
    */
    public class RtfDocumentSettings {

        /**
        * The RtfDocument this RtfDocumentSettings belongs to.
        */
        private RtfDocument document = null;
        /**
        * Whether to also output the table row definition after the cell content.
        */
        private bool outputTableRowDefinitionAfter = true;
        /**
        * Whether to output the line breaks that make the rtf document source more readable.
        */
        private bool outputDebugLineBreaks = true;
        /**
        * Whether to always generate soft linebreaks for \n in Chunks.
        */
        private bool alwaysGenerateSoftLinebreaks = false;
        /**
        * Whether to always translate characters past 'z' into unicode representations.
        */
        private bool alwaysUseUnicode = true;
        /**
        * How to cache the document during generation. Defaults to RtfDataCache.CACHE_MEMORY;
        */
        private int dataCacheStyle = RtfDataCache.CACHE_MEMORY;
        /**
        * Whether to write image scaling information. This is required for Word 2000, 97 and Word for Mac
        */
        private bool writeImageScalingInformation = false;
        /**
        * Whether images should be written in order to mimick the PDF output. 
        */
        private bool imagePDFConformance = true;
        /**
        * Document protection level
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        private int protectionLevel = RtfProtection.LEVEL_NONE;
        /**
        * Document protection level password hash.
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        private String protectionHash = null;
        /**
        * Document read password hash
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        //private String writereservhash = null; //\*\writereservhash - not implemented
        /**
        * Document recommended to be opened in read only mode.
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        private bool readOnlyRecommended = false;
        /**
        * Images are written as binary data and not hex encoded.
        * @since 2.1.1
        * @author Mark Hall (Mark.Hall@mail.room3b.eu)
        */
        private bool imageWrittenAsBinary = true;
        
        /**
        * Constructs a new RtfDocumentSettings object.
        * 
        * @param document The RtfDocument this RtfDocumentSettings belong to.
        */
        public RtfDocumentSettings(RtfDocument document) {
            this.document = document;
        }

        /**
        * Gets whether to output the line breaks for increased rtf document readability.
        * 
        * @return Whether to output line breaks.
        */
        public bool IsOutputDebugLineBreaks() {
            return outputDebugLineBreaks;
        }
        
        /**
        * Sets whether to output the line breaks for increased rtf document readability.
        * Some line breaks may be added where the rtf specification demands it.
        * 
        * @param outputDebugLineBreaks The outputDebugLineBreaks to set.
        */
        public void SetOutputDebugLineBreaks(bool outputDebugLineBreaks) {
            this.outputDebugLineBreaks = outputDebugLineBreaks;
        }
        
        /**
        * Gets whether the table row definition should also be written after the cell content.
        * 
        * @return Returns the outputTableRowDefinitionAfter.
        */
        public bool IsOutputTableRowDefinitionAfter() {
            return outputTableRowDefinitionAfter;
        }
        
        /**
        * Sets whether the table row definition should also be written after the cell content.
        * This is recommended to be set to <code>true</code> if you need Word2000 compatiblity and
        * <code>false</code> if the document should be opened in OpenOffice.org Writer.
        * 
        * @param outputTableRowDefinitionAfter The outputTableRowDefinitionAfter to set.
        */
        public void SetOutputTableRowDefinitionAfter(
                bool outputTableRowDefinitionAfter) {
            this.outputTableRowDefinitionAfter = outputTableRowDefinitionAfter;
        }
    
        /**
        * Gets whether all linebreaks inside Chunks are generated as soft linebreaks.
        * 
        * @return <code>True</code> if soft linebreaks are generated, <code>false</code> for hard linebreaks.
        */
        public bool IsAlwaysGenerateSoftLinebreaks() {
            return this.alwaysGenerateSoftLinebreaks;
        }

        /**
        * Sets whether to always generate soft linebreaks.
        * 
        * @param alwaysGenerateSoftLinebreaks Whether to always generate soft linebreaks.
        */
        public void SetAlwaysGenerateSoftLinebreaks(bool alwaysGenerateSoftLinebreaks) {
            this.alwaysGenerateSoftLinebreaks = alwaysGenerateSoftLinebreaks;
        }

        /**
        * Gets whether all characters bigger than 'z' are represented as unicode.
        * 
        * @return <code>True</code> if unicode representation is used, <code>false</code> otherwise.
        */
        public bool IsAlwaysUseUnicode() {
            return this.alwaysUseUnicode;
        }
        
        /**
        * Sets whether to represent all characters bigger than 'z' as unicode.
        * 
        * @param alwaysUseUnicode <code>True</code> to use unicode representation, <code>false</code> otherwise.
        */
        public void SetAlwaysUseUnicode(bool alwaysUseUnicode) {
            this.alwaysUseUnicode = alwaysUseUnicode;
        }

        /**
        * Registers the RtfParagraphStyle for further use in the document. This does not need to be
        * done for the default styles in the RtfParagraphStyle object. Those are added automatically.
        * 
        * @param rtfParagraphStyle The RtfParagraphStyle to register.
        */
        public void RegisterParagraphStyle(RtfParagraphStyle rtfParagraphStyle) {
            this.document.GetDocumentHeader().RegisterParagraphStyle(rtfParagraphStyle);
        }

        /**
        * Sets the data cache style. This controls where the document is cached during
        * generation. Two cache styles are supported:
        * <ul>
        *   <li>RtfDataCache.CACHE_MEMORY: The document is cached in memory. This is fast,
        *     but places a limit on how big the document can get before causing
        *     OutOfMemoryExceptions.</li>
        *   <li>RtfDataCache.CACHE_DISK: The document is cached on disk. This is slower
        *     than the CACHE_MEMORY setting, but the document size is now only constrained
        *     by the amount of free disk space.</li>
        * </ul>
        * 
        * @param dataCacheStyle The data cache style to set. Valid constants can be found
        *  in RtfDataCache.
        * @see com.lowagie.text.rtf.document.output.output.RtfDataCache.
        */
        public void SetDataCacheStyle(int dataCacheStyle) {
            switch (dataCacheStyle) {
                case RtfDataCache.CACHE_MEMORY_EFFICIENT:   
                    this.dataCacheStyle = RtfDataCache.CACHE_MEMORY_EFFICIENT;
                    break;
                case RtfDataCache.CACHE_DISK:               
                    this.dataCacheStyle = RtfDataCache.CACHE_DISK;
                    break;
                default:
                //case RtfDataCache.CACHE_MEMORY:             
                    this.dataCacheStyle = RtfDataCache.CACHE_MEMORY;
                    break;
            }
        }
        
        /**
        * Gets the current data cache style.
        * 
        * @return The current data cache style.
        */
        public int GetDataCacheStyle() {
            return this.dataCacheStyle;
        }

        /**
        * Gets the current setting on image PDF conformance.
        * 
        * @return The current image PDF conformance.
        */
        public bool IsImagePDFConformance() {
            return this.imagePDFConformance;
        }

        
        /**
        * Sets the image PDF conformance setting. By default images will be added
        * as if they were displayed with 72dpi. Set this to <code>false</code>
        * if images should be generated with the Word default DPI setting.
        * 
        * @param imagePDFConformance <code>True</code> if PDF equivalence is desired, <code>false</code>
        *   for the default Word display.
        */
        public void SetImagePDFConformance(bool imagePDFConformance) {
            this.imagePDFConformance = imagePDFConformance;
        }

        
        /**
        * Gets whether to write scaling information for images.
        * 
        * @return Whether to write scaling information for images.
        */
        public bool IsWriteImageScalingInformation() {
            return this.writeImageScalingInformation;
        }

        
        /**
        * Sets whether image scaling information should be written. This needs to be set to <code>true</code>
        * MS Word 2000, MS Word 97 and Word for Mac.
        * 
        * @param writeImageScalingInformation Whether to write image scaling information.
        */
        public void SetWriteImageScalingInformation(bool writeImageScalingInformation) {
            this.writeImageScalingInformation = writeImageScalingInformation;
        }
        
        /**
        * Set the options required for RTF documents to display correctly in MS Word 2000
        * and MS Word 97.
        * Sets <code>outputTableRowDefinitionAfter = true</code> and <code>writeImageScalingInformation = true</code>.
        */
        public void SetOptionsForMSWord2000And97() {
            this.SetOutputTableRowDefinitionAfter(true);
            this.SetWriteImageScalingInformation(true);
        }
        
        /**
        * Set the options required for RTF documents to display correctly in MS Word for Mac.
        * Sets <code>writeImageScalingInformation = true</code>.
        */
        public void SetOptionsForMSWordForMac() {
            this.SetWriteImageScalingInformation(true);
        }
        
        /**
        * Set the options required for RTF documents to display correctly in MS Word XP (2002).
        * Sets <code>writeImageScalingInformation = false</code>.
        */
        public void SetOptionsForMSWordXP() {
            this.SetWriteImageScalingInformation(false);
        }

        /**
        * Set the options required for RTF documents to display correctly in OpenOffice.Org
        * Writer.
        * Sets <code>outputTableRowDefinitionAfter = false</code>.
        */
        public void SetOptionsForOpenOfficeOrg() {
            this.SetOutputTableRowDefinitionAfter(false);
        }
 
        /**
        * @param level Document protecton level
        * @param pwd Document password - clear text
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public bool SetProtection(int level, String pwd) {
            bool result = false;
            if (this.protectionHash == null) {
                if (!SetProtectionLevel(level)) {
                    result = false;
                }
                else
                {
                    protectionHash = RtfProtection.GenerateHash(pwd);
                    result = true;
                }
            }
            else {
                if (this.protectionHash.Equals(RtfProtection.GenerateHash(pwd))) {
                    if (!SetProtectionLevel(level)) {
                        result = false;
                    }
                    else
                    {
                        protectionHash = RtfProtection.GenerateHash(pwd);
                        result = true;
                    }
                }
            }
            return result;
        }
        
        /**
        * @param pwd Document password - clear text
        * @return true if document unprotected, false if protection is not removed.
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public bool UnprotectDocument(String pwd) {
            bool result = false;
            if (this.protectionHash.Equals(RtfProtection.GenerateHash(pwd))) {
                this.protectionLevel =  RtfProtection.LEVEL_NONE;
                this.protectionHash = null;
                result = true;
            }
            return result;
        }
        
        /**
        * @param level Document protection level
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public bool SetProtectionLevel(int level) {
            bool result = false;
            switch (level) {
            case RtfProtection.LEVEL_NONE:
                if (this.protectionHash == null) {
                    break;
                }
                    goto case RtfProtection.LEVEL_ANNOTPROT;
            case RtfProtection.LEVEL_ANNOTPROT:
            case RtfProtection.LEVEL_FORMPROT:
            case RtfProtection.LEVEL_REVPROT:
            case RtfProtection.LEVEL_READPROT:
                this.protectionLevel = level;
                result = true;
                break;
            }
            return result;
        }
        
        /**
        * This function is not intended for general use. Please see 'public bool SetProtection(int level, String pwd)'
        * @param pwd Password HASH to set the document password hash to.
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public void SetPasswordHash(String pwd) {
            if (pwd != null && pwd.Length != 8) return;
            this.protectionHash = pwd;
        }
        
        /**
        * Converts protection level from internal bitmap value to protlevel output value
        * @return <pre>
        * 0 = Revision protection
        * 1 = Annotation/Comment protection
        * 2 = Form protection
        * 3 = Read only protection
        * </pre>
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        private int ConvertProtectionLevel() {
            int level = 0;
            switch (this.protectionLevel) {
            case RtfProtection.LEVEL_NONE:
                break;
            case RtfProtection.LEVEL_REVPROT:
                level = 0;
                break;
            case RtfProtection.LEVEL_ANNOTPROT:
                level = 1;
                break;
            case RtfProtection.LEVEL_FORMPROT:
                level = 2;
                break;
            case RtfProtection.LEVEL_READPROT:
                level = 3;
                break;
            }
            return level;
            
        }
        
        /**
        * @return RTF document protection level
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public int GetProtectionLevelRaw() {
            return this.protectionLevel;
        }
        
        /**
        * @return RTF document protection level
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public int GetProtectionLevel() {
            return ConvertProtectionLevel();
        }
        
        /**
        * @return RTF document protection level as a byte array (byte[])
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public byte[] GetProtectionLevelBytes() {
            return DocWriter.GetISOBytes(ConvertProtectionLevel().ToString());
        }
        
        /**
        * @param oldPwd Old password - clear text
        * @param newPwd New password - clear text
        * @return true if password set, false if password not set
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public bool SetNewPassword(String oldPwd, String newPwd) {
            bool result = false;
            if (this.protectionHash.Equals(RtfProtection.GenerateHash(oldPwd))) {
                this.protectionHash = RtfProtection.GenerateHash(newPwd);
                result = true;
            }
            return result;
        }
        
        /**
        * Set the RTF flag that recommends the document be opened in read only mode.
        * @param value true if the flag is to be set, false if it is NOT to be set
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public void SetReadOnlyRecommended(bool value) {
            this.readOnlyRecommended = value;
        }
        
        /**
        * Get the RTF flag that recommends if the the document should be opened in read only mode.
        * @return true if flag is set, false if it is not set
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public bool GetReadOnlyRecommended() {
            return this.readOnlyRecommended;
        }
        
        /**
        * Determine if document has protection enabled.
        * @return true if protection is enabled, false if it is not enabled
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public bool IsDocumentProtected() {
            return !(this.protectionHash == null);
        }
        
        /**
        * Obtain the password has as a byte array.
        * @return The bytes of the password hash as a byte array (byte[])
        * @since 2.1.1
        * @author Howard Shank (hgshank@yahoo.com)
        */
        public byte[] GetProtectionHashBytes() {
            return DocWriter.GetISOBytes(this.protectionHash);
        }

        /**
        * Set whether images are written as binary data or are hex encoded.
        * 
        * @param imageWrittenAsBinary <code>True</code> to write images as binary data, <code>false</code> for hex encoding.
        * @since 2.1.1
        * @author Mark Hall (Mark.Hall@mail.room3b.eu)
        */
        public void SetImageWrittenAsBinary(bool imageWrittenAsBinary) {
            this.imageWrittenAsBinary = imageWrittenAsBinary;
        }
        
        /**
        * Gets whether images are written as binary data or are hex encoded. Defaults to <code>true</code>.
        * 
        * @since 2.1.1
        * @return <code>True</code> if images are written as binary data, <code>false</code> if hex encoded.
        * @author Mark Hall (Mark.Hall@mail.room3b.eu)
        */
        public bool IsImageWrittenAsBinary() {
            return this.imageWrittenAsBinary;
        }
    }
}