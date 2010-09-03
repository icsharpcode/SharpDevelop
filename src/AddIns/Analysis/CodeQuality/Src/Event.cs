// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Event : INode
    {
        /// <summary>
        /// Name of event
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of event
        /// </summary>
        public Type EventType { get; set; }

        /// <summary>
        /// Type which owns this event
        /// </summary>
        public Type DeclaringType { get; set; }

        public Event()
        {
            Dependency = null;
        }

        public override string ToString()
        {
            return Name;
        }
        
        public Relationship GetRelationship(INode node)
        {
        	Relationship relationship = new Relationship();
        	return relationship;
        }

        public IDependency Dependency { get; set; }

        public string GetInfo()
        {
            // Events aren't visible. They are showed like fields instead.
            return this.ToString();
        }
        
        public BitmapSource Icon { get { return null; } }
    }
}
