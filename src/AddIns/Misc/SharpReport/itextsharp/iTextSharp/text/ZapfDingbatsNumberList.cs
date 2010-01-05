using System;

namespace iTextSharp.text
{
    /**
    * 
    * A special-version of <CODE>LIST</CODE> whitch use zapfdingbats-numbers (1..10).
    * 
    * @see com.lowagie.text.List
    * @version 2003-06-22
    * @author Michael Niedermair
    */
    public class ZapfDingbatsNumberList : List {

        /**
        * which type
        */
        protected int type;

        /**
        * Creates a ZapdDingbatsNumberList
        * @param type the type of list
        * @param symbolIndent  indent
        */
        public ZapfDingbatsNumberList(int type) : base(true) {
            this.type = type;
            float fontsize = symbol.Font.Size;
            symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
            postSymbol = " ";
        }

        /**
        * Creates a ZapdDingbatsNumberList
        * @param type the type of list
        * @param symbolIndent    indent
        */
        public ZapfDingbatsNumberList(int type, int symbolIndent) : base(true, symbolIndent) {
            this.type = type;
            float fontsize = symbol.Font.Size;
            symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
            postSymbol = " ";
        }

        /**
        * get the type
        *
        * @return    char-number
        */
        public int NumberType {
            get {
                return type;
            }
            set {
                type = value;
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
                switch (type ) {
                    case 0:
                        chunk.Append(((char)(first + list.Count + 171)).ToString());
                        break;
                    case 1:
                        chunk.Append(((char)(first + list.Count + 181)).ToString());
                        break;
                    case 2:
                        chunk.Append(((char)(first + list.Count + 191)).ToString());
                        break;
                    default:
                        chunk.Append(((char)(first + list.Count + 201)).ToString());
                        break;
                }
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
