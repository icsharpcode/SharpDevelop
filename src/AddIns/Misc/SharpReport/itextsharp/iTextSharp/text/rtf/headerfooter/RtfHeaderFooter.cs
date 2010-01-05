using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using FD = iTextSharp.text.rtf.field;
/*
 * Created on Aug 10, 2004
 *
 * To change the template for this generated file go to
 * Window - Preferences - Java - Code Generation - Code and Comments
 */
namespace iTextSharp.text.rtf.headerfooter {

    /**
    * The RtfHeaderFooter represents one header or footer. This class can be used
    * directly.
    * 
    * @version $Id: RtfHeaderFooter.cs,v 1.7 2008/05/16 19:30:59 psoares33 Exp $
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfHeaderFooter : HeaderFooter, IRtfBasicElement {

        /**
        * Constant for the header type
        */
        public const int TYPE_HEADER = 1;
        /**
        * Constant for the footer type
        */
        public const int TYPE_FOOTER = 2;
        /**
        * Constant for displaying the header/footer on the first page
        */
        public const int DISPLAY_FIRST_PAGE = 0;
        /**
        * Constant for displaying the header/footer on all pages
        */
        public const int DISPLAY_ALL_PAGES = 1;
        /**
        * Constant for displaying the header/footer on all left hand pages
        */
        public const int DISPLAY_LEFT_PAGES = 2;
        /**
        * Constant for displaying the header/footer on all right hand pages
        */
        public const int DISPLAY_RIGHT_PAGES = 4;

        /**
        * Constant for a header on all pages
        */
        private static byte[] HEADER_ALL = DocWriter.GetISOBytes("\\header");
        /**
        * Constant for a header on the first page
        */
        private static byte[] HEADER_FIRST = DocWriter.GetISOBytes("\\headerf");
        /**
        * Constant for a header on all left hand pages
        */
        private static byte[] HEADER_LEFT = DocWriter.GetISOBytes("\\headerl");
        /**
        * Constant for a header on all right hand pages
        */
        private static byte[] HEADER_RIGHT = DocWriter.GetISOBytes("\\headerr");
        /**
        * Constant for a footer on all pages
        */
        private static byte[] FOOTER_ALL = DocWriter.GetISOBytes("\\footer");
        /**
        * Constant for a footer on the first page
        */
        private static byte[] FOOTER_FIRST = DocWriter.GetISOBytes("\\footerf");
        /**
        * Constnat for a footer on the left hand pages
        */
        private static byte[] FOOTER_LEFT = DocWriter.GetISOBytes("\\footerl");
        /**
        * Constant for a footer on the right hand pages
        */
        private static byte[] FOOTER_RIGHT = DocWriter.GetISOBytes("\\footerr");
        
        /**
        * The RtfDocument this RtfHeaderFooter belongs to
        */
        private RtfDocument document = null;
        /**
        * The content of this RtfHeaderFooter
        */
        private Object[] content = null;
        /**
        * The display type of this RtfHeaderFooter. TYPE_HEADER or TYPE_FOOTER
        */
        private int type = TYPE_HEADER;
        /**
        * The display location of this RtfHeaderFooter. DISPLAY_FIRST_PAGE,
        * DISPLAY_LEFT_PAGES, DISPLAY_RIGHT_PAGES or DISPLAY_ALL_PAGES
        */
        private int displayAt = DISPLAY_ALL_PAGES;
       
        /**
        * Constructs a RtfHeaderFooter based on a HeaderFooter with a certain type and displayAt
        * location. For internal use only.
        * 
        * @param doc The RtfDocument this RtfHeaderFooter belongs to
        * @param headerFooter The HeaderFooter to base this RtfHeaderFooter on
        * @param type The type of RtfHeaderFooter
        * @param displayAt The display location of this RtfHeaderFooter
        */
        protected internal RtfHeaderFooter(RtfDocument doc, HeaderFooter headerFooter, int type, int displayAt) : base(new Phrase(""), false)  {
            this.document = doc;
            this.type = type;
            this.displayAt = displayAt;
            Paragraph par = new Paragraph();
            par.Alignment = headerFooter.Alignment;
            if (headerFooter.Before != null) {
                par.Add(headerFooter.Before);
            }
            if (headerFooter.IsNumbered()) {
                par.Add(new FD.RtfPageNumber(this.document));
            }
            if (headerFooter.After != null) {
                par.Add(headerFooter.After);
            }
            try {
                this.content = new Object[1];
                if (this.document != null) {
                    this.content[0] = this.document.GetMapper().MapElement(par)[0];
                    ((IRtfBasicElement) this.content[0]).SetInHeader(true);
                } else {
                    this.content[0] = par;
                }
            } catch (DocumentException) {
            }
        }
        
        /**
        * Constructs a RtfHeaderFooter as a copy of an existing RtfHeaderFooter.
        * For internal use only.
        * 
        * @param doc The RtfDocument this RtfHeaderFooter belongs to
        * @param headerFooter The RtfHeaderFooter to copy
        * @param displayAt The display location of this RtfHeaderFooter
        */
        protected internal RtfHeaderFooter(RtfDocument doc, RtfHeaderFooter headerFooter, int displayAt) : base(new Phrase(""), false) {
            this.document = doc;
            this.content = headerFooter.GetContent();
            this.displayAt = displayAt;
            for (int i = 0; i < this.content.Length; i++) {
                if (this.content[i] is IElement) {
                    try {
                        this.content[i] = this.document.GetMapper().MapElement((IElement) this.content[i])[0];
                    } catch (DocumentException) {
                    }
                }
                if (this.content[i] is IRtfBasicElement) {
                    ((IRtfBasicElement) this.content[i]).SetInHeader(true);
                }
            }
        }
        
        /**
        * Constructs a RtfHeaderFooter for a HeaderFooter.
        *  
        * @param doc The RtfDocument this RtfHeaderFooter belongs to
        * @param headerFooter The HeaderFooter to base this RtfHeaderFooter on
        */
        protected internal RtfHeaderFooter(RtfDocument doc, HeaderFooter headerFooter) : base(new Phrase(""), false) {
            this.document = doc;
            Paragraph par = new Paragraph();
            par.Alignment = headerFooter.Alignment;
            if (headerFooter.Before != null) {
                par.Add(headerFooter.Before);
            }
            if (headerFooter.IsNumbered()) {
                par.Add(new FD.RtfPageNumber(this.document));
            }
            if (headerFooter.After != null) {
                par.Add(headerFooter.After);
            }
            try {
                this.content = new Object[1];
                this.content[0] = doc.GetMapper().MapElement(par)[0];
                ((IRtfBasicElement) this.content[0]).SetInHeader(true);
            } catch (DocumentException) {
            }
        }
        
        /**
        * Constructs a RtfHeaderFooter for any Element.
        *
        * @param element The Element to display as content of this RtfHeaderFooter
        */
        public RtfHeaderFooter(IElement element) : this(new IElement[]{element}) {
        }

        /**
        * Constructs a RtfHeaderFooter for an array of Elements.
        * 
        * @param elements The Elements to display as the content of this RtfHeaderFooter.
        */
        public RtfHeaderFooter(IElement[] elements) : base(new Phrase(""), false){
            this.content = new Object[elements.Length];
            for (int i = 0; i < elements.Length; i++) {
                this.content[i] = elements[i];
            }
        }
        
        /**
        * Sets the RtfDocument this RtfElement belongs to
        * 
        * @param doc The RtfDocument to use
        */
        public void SetRtfDocument(RtfDocument doc) {
            this.document = doc;
            if (this.document != null) {
                for (int i = 0; i < this.content.Length; i++) {
                    try {
                        if (this.content[i] is Element) {
                            this.content[i] = this.document.GetMapper().MapElement((IElement) this.content[i])[0];
                            ((IRtfBasicElement) this.content[i]).SetInHeader(true);
                        } else if (this.content[i] is IRtfBasicElement){
                            ((IRtfBasicElement) this.content[i]).SetRtfDocument(this.document);
                            ((IRtfBasicElement) this.content[i]).SetInHeader(true);
                        }
                    } catch (DocumentException) {
                    }
                }
            }
        }
        
        /**
        * Write the content of this RtfHeaderFooter.
        */
        public virtual void WriteContent(Stream result) {       
            result.Write(RtfElement.OPEN_GROUP, 0, RtfElement.OPEN_GROUP.Length);
            if (this.type == TYPE_HEADER) {
                if (this.displayAt == DISPLAY_ALL_PAGES) {
                    result.Write(HEADER_ALL, 0, HEADER_ALL.Length);
                } else if (this.displayAt == DISPLAY_FIRST_PAGE) {
                    result.Write(HEADER_FIRST, 0, HEADER_FIRST.Length);
                } else if (this.displayAt == DISPLAY_LEFT_PAGES) {
                    result.Write(HEADER_LEFT, 0, HEADER_LEFT.Length);
                } else if (this.displayAt == DISPLAY_RIGHT_PAGES) {
                    result.Write(HEADER_RIGHT, 0, HEADER_RIGHT.Length);
                }
            } else {
                if (this.displayAt == DISPLAY_ALL_PAGES) {
                    result.Write(FOOTER_ALL, 0, FOOTER_ALL.Length);
                } else if (this.displayAt == DISPLAY_FIRST_PAGE) {
                    result.Write(FOOTER_FIRST, 0, FOOTER_FIRST.Length);
                } else if (this.displayAt == DISPLAY_LEFT_PAGES) {
                    result.Write(FOOTER_LEFT, 0, FOOTER_LEFT.Length);
                } else if (this.displayAt == DISPLAY_RIGHT_PAGES) {
                    result.Write(FOOTER_RIGHT, 0, FOOTER_RIGHT.Length);
                }
            }
            result.Write(RtfElement.DELIMITER, 0, RtfElement.DELIMITER.Length);
            for (int i = 0; i < this.content.Length; i++) {
                if (this.content[i] is IRtfBasicElement) {
                    IRtfBasicElement rbe = (IRtfBasicElement)this.content[i];
                    rbe.WriteContent(result);
                }
            }
            result.Write(RtfElement.CLOSE_GROUP, 0, RtfElement.CLOSE_GROUP.Length);
        }
        
        
        /**
        * Sets the display location of this RtfHeaderFooter
        * 
        * @param displayAt The display location to use.
        */
        public void SetDisplayAt(int displayAt) {
            this.displayAt = displayAt;
        }
        
        /**
        * Sets the type of this RtfHeaderFooter
        * 
        * @param type The type to use.
        */
        public void SetType(int type) {
            this.type = type;
        }
        
        /**
        * Gets the content of this RtfHeaderFooter
        * 
        * @return The content of this RtfHeaderFooter
        */
        private Object[] GetContent() {
            return this.content;
        }

        /**
        * Unused
        * @param inTable
        */
        public void SetInTable(bool inTable) {
        }
        
        /**
        * Unused
        * @param inHeader
        */
        public void SetInHeader(bool inHeader) {
        }

        /**
        * Set the alignment of this RtfHeaderFooter. Passes the setting
        * on to the contained element.
        */
        public void SetAlignment(int alignment) {
            base.Alignment = alignment;
            for (int i = 0; i < this.content.Length; i++) {
                if (this.content[i] is Paragraph) {
                    ((Paragraph) this.content[i]).Alignment = alignment;
                } else if (this.content[i] is Table) {
                    ((Table) this.content[i]).Alignment = alignment;
                } else if (this.content[i] is Image) {
                    ((Image) this.content[i]).Alignment = alignment;
                }     
            }
        }
    }
}