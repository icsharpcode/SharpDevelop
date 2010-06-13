using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonDock
{
    public class DeserializationCallbackEventArgs : EventArgs
    {
        public DeserializationCallbackEventArgs(string contentName)
        {
            Name = contentName;
        }

        /// <summary>
        /// Gets the name of the content to deserialize
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets/Sets the content manually deserialized
        /// </summary>
        public ManagedContent Content { get; set; }
    }
}
