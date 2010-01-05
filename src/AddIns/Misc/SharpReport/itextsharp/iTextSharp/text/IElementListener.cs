using System;

namespace iTextSharp.text {
    /// <summary>
    /// A class that implements ElementListener will perform some
    /// actions when an Element is added.
    /// </summary>
    public interface IElementListener {
        /// <summary>
        /// Signals that an Element was added to the Document.
        /// </summary>
        /// <param name="element">Element added</param>
        /// <returns>true if the element was added, false if not.</returns>
        bool Add(IElement element);
    }
}
