using System;
using System.IO;
using System.Collections;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.style {

    /**
    * The RtfStylesheetList stores the RtfParagraphStyles that are used in the document.
    * 
    * @version $Revision$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfStylesheetList : RtfElement, IRtfExtendedElement {

        /**
        * The Hashtable containing the RtfParagraphStyles.
        */
        private Hashtable styleMap = null;
        /**
        * Whether the default settings have been loaded.
        */
        private bool defaultsLoaded = false;
        
        /**
        * Constructs a new RtfStylesheetList for the RtfDocument.
        * 
        * @param doc The RtfDocument this RtfStylesheetList belongs to.
        */
        public RtfStylesheetList(RtfDocument doc) : base(doc) {
            this.styleMap = new Hashtable();
        }

        /**
        * unused
        */
        public override void WriteContent(Stream outp) {       
        }
        
        /**
        * Register a RtfParagraphStyle with this RtfStylesheetList.
        * 
        * @param rtfParagraphStyle The RtfParagraphStyle to add.
        */
        public void RegisterParagraphStyle(RtfParagraphStyle rtfParagraphStyle) {
            RtfParagraphStyle tempStyle = new RtfParagraphStyle(this.document, rtfParagraphStyle);
            tempStyle.HandleInheritance();
            tempStyle.SetStyleNumber(this.styleMap.Count);
            this.styleMap[tempStyle.GetStyleName()] = tempStyle;
        }

        /**
        * Registers all default styles. If styles with the given name have already been registered,
        * then they are NOT overwritten.
        */
        private void RegisterDefaultStyles() {
            defaultsLoaded = true;
            if (!this.styleMap.ContainsKey(RtfParagraphStyle.STYLE_NORMAL.GetStyleName())) {
                RegisterParagraphStyle(RtfParagraphStyle.STYLE_NORMAL);
            }
            if (!this.styleMap.ContainsKey(RtfParagraphStyle.STYLE_HEADING_1.GetStyleName())) {
                RegisterParagraphStyle(RtfParagraphStyle.STYLE_HEADING_1);
            }
            if (!this.styleMap.ContainsKey(RtfParagraphStyle.STYLE_HEADING_2.GetStyleName())) {
                RegisterParagraphStyle(RtfParagraphStyle.STYLE_HEADING_2);
            }
            if (!this.styleMap.ContainsKey(RtfParagraphStyle.STYLE_HEADING_3.GetStyleName())) {
                RegisterParagraphStyle(RtfParagraphStyle.STYLE_HEADING_3);
            }
        }

        /**
        * Gets the RtfParagraphStyle with the given name. Makes sure that the defaults
        * have been loaded.
        * 
        * @param styleName The name of the RtfParagraphStyle to get.
        * @return The RtfParagraphStyle with the given name or null.
        */
        public RtfParagraphStyle GetRtfParagraphStyle(String styleName) {
            if (!defaultsLoaded) {
                RegisterDefaultStyles();
            }
            if (this.styleMap.ContainsKey(styleName)) {
                return (RtfParagraphStyle) this.styleMap[styleName];
            } else {
                return null;
            }
        }
        
        /**
        * Writes the definition of the stylesheet list.
        */
        public virtual void WriteDefinition(Stream result) {
            byte[] t;
            result.Write(t = DocWriter.GetISOBytes("{"), 0, t.Length);
            result.Write(t = DocWriter.GetISOBytes("\\stylesheet"), 0, t.Length);
            result.Write(t = RtfElement.DELIMITER, 0, t.Length);
            if (this.document.GetDocumentSettings().IsOutputDebugLineBreaks()) {
                result.Write(t = DocWriter.GetISOBytes("\n"), 0, t.Length);
            }
            foreach (RtfParagraphStyle rps in this.styleMap.Values)
                rps.WriteDefinition(result);
            result.Write(t = DocWriter.GetISOBytes("}"), 0, t.Length);
            if (this.document.GetDocumentSettings().IsOutputDebugLineBreaks()) {
                result.WriteByte((byte)'\n');
            }
        }
    }
}