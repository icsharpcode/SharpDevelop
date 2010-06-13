using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace AvalonDock
{
    public class RequestDocumentCloseEventArgs : CancelEventArgs
    {
        public RequestDocumentCloseEventArgs(DocumentContent doc)
        {
            DocumentToClose = doc;
        }

        /// <summary>
        /// Document content that user wants to close
        /// </summary>
        public DocumentContent DocumentToClose { get; private set; }
    }
}
