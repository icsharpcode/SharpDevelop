using System;
using System.IO;
using iTextSharp.text;
using ST = iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.document;
/*
 * $Id: RtfField.cs,v 1.7 2008/05/16 19:30:54 psoares33 Exp $
 * 
 *
 * Copyright 2004 by Mark Hall
 * Uses code Copyright 2002
 *   <a href="http://www.smb-tec.com">SMB</a> 
 *   <a href="mailto:Dirk.Weigenand@smb-tec.com">Dirk.Weigenand@smb-tec.com</a>
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

namespace iTextSharp.text.rtf.field {

    /**
    * The RtfField class is an abstract base class for all rtf field functionality.
    * Subclasses only need to implement the two abstract methods writeFieldInstContent
    * and writeFieldResultContent. All other field functionality is handled by the
    * RtfField class.
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author <a href="mailto:Dirk.Weigenand@smb-tec.com">Dirk Weigenand</a>
    */
    public abstract class RtfField : Chunk, iTextSharp.text.rtf.IRtfBasicElement {

        /**
        * Constant for the beginning of a rtf group
        */
        public static byte[] OPEN_GROUP = {(byte)'{'};
        /**
        * Constant for the end of an rtf group
        */
        public static byte[] CLOSE_GROUP = {(byte)'}'};
        /**
        * Constant for a delimiter in rtf
        */
        public static byte[] DELIMITER = {(byte)' '};
        /**
        * Constant for a comma delimiter in rtf
        */
        public static byte[] COMMA_DELIMITER = {(byte)';'};
        /**
        * The factor to use for translating from iText to rtf measurments
        */
        public const double TWIPS_FACTOR = 20;

        /**
        * Constant for a rtf field
        */
        private static byte[] FIELD = DocWriter.GetISOBytes("\\field");
        /**
        * Constant for a dirty field
        */
        private static byte[] FIELD_DIRTY = DocWriter.GetISOBytes("\\flddirty");
        /**
        * Constant for a private field
        */
        private static byte[] FIELD_PRIVATE = DocWriter.GetISOBytes("\\fldpriv");
        /**
        * Constant for a locked field
        */
        private static byte[] FIELD_LOCKED = DocWriter.GetISOBytes("\\fldlock");
        /**
        * Constant for a edited field
        */
        private static byte[] FIELD_EDIT = DocWriter.GetISOBytes("\\fldedit");
        /**
        * Constant for an alt field
        */
        private static byte[] FIELD_ALT = DocWriter.GetISOBytes("\\fldalt");
        /**
        * Constant for the field instructions
        */
        private static byte[] FIELD_INSTRUCTIONS = DocWriter.GetISOBytes("\\*\\fldinst");
        /**
        * Constant for the field result
        */
        private static byte[] FIELD_RESULT = DocWriter.GetISOBytes("\\fldrslt");

        /**
        * Is the field dirty
        */
        private bool fieldDirty = false;
        /**
        * Is the field edited
        */
        private bool fieldEdit = false;
        /**
        * Is the field locked
        */
        private bool fieldLocked = false;
        /**
        * Is the field private
        */
        private bool fieldPrivate = false;
        /**
        * Is it an alt field
        */
        private bool fieldAlt = false;
        /**
        * Whether this RtfField is in a table
        */
        private bool inTable = false;
        /**
        * Whether this RtfElement is in a header
        */
        private bool inHeader = false;
        /**
        * The RtfDocument this RtfField belongs to 
        */
        protected RtfDocument document = null;
        /**
        * The RtfFont of this RtfField
        */
        private new ST.RtfFont font = null;

        /**
        * Constructs a RtfField for a RtfDocument. This is not very usefull,
        * since the RtfField by itself does not do anything. Use one of the
        * subclasses instead.
        * 
        * @param doc The RtfDocument this RtfField belongs to.
        */
        protected RtfField(RtfDocument doc) : this(doc, new Font()) {
        }
        
        /**
        * Constructs a RtfField for a RtfDocument. This is not very usefull,
        * since the RtfField by itself does not do anything. Use one of the
        * subclasses instead.
        * 
        * @param doc The RtfDocument this RtfField belongs to.
        * @param font The Font this RtfField should use
        */
        protected RtfField(RtfDocument doc, Font font) : base("", font) {
            this.document = doc;
            this.font = new ST.RtfFont(this.document, font);
        }
        
        /**
        * Sets the RtfDocument this RtfElement belongs to
        * 
        * @param doc The RtfDocument to use
        */
        public void SetRtfDocument(RtfDocument doc) {
            this.document = doc;
            this.font.SetRtfDocument(this.document);
        }
        
        /**
        * Writes the field beginning. Also writes field properties.
        * 
        * @return A byte array with the field beginning.
        * @throws IOException
        */
        private void WriteFieldBegin(Stream result) {
            result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);
            result.Write(FIELD, 0, FIELD.Length);
            if (fieldDirty) result.Write(FIELD_DIRTY, 0, FIELD_DIRTY.Length);
            if (fieldEdit) result.Write(FIELD_EDIT, 0, FIELD_EDIT.Length);
            if (fieldLocked) result.Write(FIELD_LOCKED, 0, FIELD_LOCKED.Length);
            if (fieldPrivate) result.Write(FIELD_PRIVATE, 0, FIELD_PRIVATE.Length);
        }
        
        /**
        * Writes the beginning of the field instruction area.
        * 
        * @return The beginning of the field instruction area
        * @throws IOException
        */
        private void WriteFieldInstBegin(Stream result) {
            result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);        
            result.Write(FIELD_INSTRUCTIONS, 0, FIELD_INSTRUCTIONS.Length);
            result.Write(DELIMITER, 0, DELIMITER.Length);
        }
        
        /**
        * Writes the content of the field instruction area. Override this
        * method in your subclasses.
        */
        protected abstract void WriteFieldInstContent(Stream oupt);
        
        /**
        * Writes the end of the field instruction area.
        */
        private void WriteFieldInstEnd(Stream result) {
            if (fieldAlt) {
                result.Write(DELIMITER, 0, DELIMITER.Length);
                result.Write(FIELD_ALT, 0, FIELD_ALT.Length);
            }
            result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
        }
        
        /**
        * Writes the beginning of the field result area
        */
        private void WriteFieldResultBegin(Stream result) {
            result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);
            result.Write(FIELD_RESULT, 0, FIELD_RESULT.Length);
            result.Write(DELIMITER, 0, DELIMITER.Length);
        }
        
        /**
        * Writes the content of the pre-calculated field result. Override this
        * method in your subclasses.
        */ 
        protected abstract void WriteFieldResultContent(Stream oupt);
        
        /**
        * Writes the end of the field result area
        */ 
        private void WriteFieldResultEnd(Stream result) {
            result.Write(DELIMITER, 0, DELIMITER.Length);
            result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
        }
        
        /**
        * Writes the end of the field
        */
        private void WriteFieldEnd(Stream result) {
            result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
        }
        
        
        /**
        * Writes the field to the <code>OutputStream</code>.
        */    
        public virtual void WriteContent(Stream result) {
            font.WriteBegin(result);
            WriteFieldBegin(result);
            WriteFieldInstBegin(result);
            WriteFieldInstContent(result);
            WriteFieldInstEnd(result);
            WriteFieldResultBegin(result);
            WriteFieldResultContent(result);
            WriteFieldResultEnd(result);
            WriteFieldEnd(result);
            font.WriteEnd(result);
        }        
            
        /**
        * Get whether this field is an alt field
        * 
        * @return Returns whether this field is an alt field
        */
        public bool IsFieldAlt() {
            return fieldAlt;
        }
        
        /**
        * Set whether this field is an alt field
        * 
        * @param fieldAlt The value to use
        */
        public void SetFieldAlt(bool fieldAlt) {
            this.fieldAlt = fieldAlt;
        }
        
        /**
        * Get whether this field is dirty
        * 
        * @return Returns whether this field is dirty
        */
        public bool IsFieldDirty() {
            return fieldDirty;
        }
        
        /**
        * Set whether this field is dirty
        * 
        * @param fieldDirty The value to use
        */
        public void SetFieldDirty(bool fieldDirty) {
            this.fieldDirty = fieldDirty;
        }
        
        /**
        * Get whether this field is edited
        * 
        * @return Returns whether this field is edited
        */
        public bool IsFieldEdit() {
            return fieldEdit;
        }
        
        /**
        * Set whether this field is edited.
        * 
        * @param fieldEdit The value to use
        */
        public void SetFieldEdit(bool fieldEdit) {
            this.fieldEdit = fieldEdit;
        }
        
        /**
        * Get whether this field is locked
        * 
        * @return Returns the fieldLocked.
        */
        public bool IsFieldLocked() {
            return fieldLocked;
        }
        
        /**
        * Set whether this field is locked
        * @param fieldLocked The value to use
        */
        public void SetFieldLocked(bool fieldLocked) {
            this.fieldLocked = fieldLocked;
        }
        
        /**
        * Get whether this field is private
        * 
        * @return Returns the fieldPrivate.
        */
        public bool IsFieldPrivate() {
            return fieldPrivate;
        }
        
        /**
        * Set whether this field is private
        * 
        * @param fieldPrivate The value to use
        */
        public void SetFieldPrivate(bool fieldPrivate) {
            this.fieldPrivate = fieldPrivate;
        }

        /**
        * Sets whether this RtfField is in a table
        * 
        * @param inTable <code>True</code> if this RtfField is in a table, <code>false</code> otherwise
        */
        public void SetInTable(bool inTable) {
            this.inTable = inTable;
        }
        
        /**
        * Gets whether this <code>RtfField</code> is in a table.
        * 
        * @return <code>True</code> if this <code>RtfField</code> is in a table, <code>false</code> otherwise
        */
        public bool IsInTable() {
            return this.inTable;
        }
        
        /**
        * Sets whether this RtfField is in a header
        * 
        * @param inHeader <code>True</code> if this RtfField is in a header, <code>false</code> otherwise
        */
        public void SetInHeader(bool inHeader) {
            this.inHeader = inHeader;
        }
        
        /**
        * Gets whether this <code>RtfField</code> is in a header.
        * 
        * @return <code>True</code> if this <code>RtfField</code> is in a header, <code>false</code> otherwise
        */
        public bool IsInHeader() {
            return this.inHeader;
        }
        
        /**
        * An RtfField is never empty.
        */
        public override bool IsEmpty() {
            return false;
        }
    
        public override Font Font {
            set {
                base.Font = value;
                font = new ST.RtfFont(document, value);
            }
        }
    }
}