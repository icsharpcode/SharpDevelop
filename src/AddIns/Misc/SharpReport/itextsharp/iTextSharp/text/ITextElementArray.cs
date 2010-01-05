using System;

namespace iTextSharp.text {
    /// <summary>
    /// Interface for a text element to which other objects can be added.
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Phrase"/>
    /// <seealso cref="T:iTextSharp.text.Paragraph"/>
    /// <seealso cref="T:iTextSharp.text.Section"/>
    /// <seealso cref="T:iTextSharp.text.ListItem"/>
    /// <seealso cref="T:iTextSharp.text.Chapter"/>
    /// <seealso cref="T:iTextSharp.text.Anchor"/>
    /// <seealso cref="T:iTextSharp.text.Cell"/>
    public interface ITextElementArray : IElement {
        /// <summary>
        /// Adds an object to the TextElementArray.
        /// </summary>
        /// <param name="o">an object that has to be added</param>
        /// <returns>true if the addition succeeded; false otherwise</returns>
        bool Add(Object o);
    }
}
