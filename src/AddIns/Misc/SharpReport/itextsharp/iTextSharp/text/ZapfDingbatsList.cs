using System;

namespace iTextSharp.text
{
    /**
    * 
    * A special-version of <CODE>LIST</CODE> whitch use zapfdingbats-letters.
    * 
    * @see com.lowagie.text.List
    * @author Michael Niedermair and Bruno Lowagie
    */
    public class ZapfDingbatsList : List {
        /**
        * char-number in zapfdingbats
        */
        protected int zn;

        /**
        * Creates a ZapfDingbatsList
        * 
        * @param zn a char-number
        */
        public ZapfDingbatsList(int zn) : base(true) {
            this.zn = zn;
            float fontsize = symbol.Font.Size;
            symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
            postSymbol = " ";
        }

        /**
        * Creates a ZapfDingbatsList
        * 
        * @param zn a char-number
        * @param symbolIndent    indent
        */
        public ZapfDingbatsList(int zn, int symbolIndent) : base(true, symbolIndent) {
            this.zn = zn;
            float fontsize = symbol.Font.Size;
            symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
            postSymbol = " ";
        }

        /**
        * set the char-number 
        * @param zn a char-number
        */
        public int CharNumber {
            set {
                this.zn = value;
            }
            get {
                return this.zn;
            }
        }

        /**
        * Adds an <CODE>Object</CODE> to the <CODE>List</CODE>.
        *
        * @param    o    the object to add.
        * @return true if adding the object succeeded
        */
        public override bool Add(Object o) {
            if (o is ListItem) {
                ListItem item = (ListItem) o;
                Chunk chunk = new Chunk(preSymbol, symbol.Font);
                chunk.Append(((char)zn).ToString());
                chunk.Append(postSymbol);
                item.ListSymbol = chunk;
                item.SetIndentationLeft(symbolIndent, autoindent);
                item.IndentationRight = 0;
                list.Add(item);
                return true;
            } else if (o is List) {
                List nested = (List) o;
                nested.IndentationLeft = nested.IndentationLeft + symbolIndent;
                first--;
                list.Add(nested);
                return true;
            } else if (o is String) {
                return this.Add(new ListItem((string) o));
            }
            return false;
        }
    }
}
