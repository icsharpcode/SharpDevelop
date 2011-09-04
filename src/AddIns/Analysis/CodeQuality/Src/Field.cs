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
    public class Field : INode
    {
        /// <summary>
        /// Name of field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of field
        /// </summary>
        public Type FieldType { get; set; }

        /// <summary>
        /// Type which owns this field
        /// </summary>
        public Type DeclaringType { get; set; }

        /// <summary>
        /// Whether the field is event
        /// </summary>
        public bool IsEvent { get; set; }

        /// <summary>
        /// Whether the field is public
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Whether the field is private
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Whether the field is protected
        /// </summary>
        public bool IsProtected { get; set; }

        /// <summary>
        /// Whether the field is static
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Whether the field is constant
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        /// Whether the field is read only
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Whether the type of field is generic
        /// </summary>
        public bool IsGenericInstance { get; set; }

        /// <summary>
        /// If the field has generic instance so all types used in generic are presented in this set.
        /// </summary>
        public ISet<Type> GenericTypes { get; private set; }

        public Field()
        {
            FieldType = null;
            IsEvent = false;
            DeclaringType = null;
            GenericTypes = new HashSet<Type>();

            Dependency = null;
        }
        
        public Relationship GetRelationship(INode node)
        {
        	Relationship relationship = new Relationship();
        	
        	if (node == this) {
        		relationship.Relationships.Add(RelationshipType.Same);
        		return relationship;
        	}
        	
        	return relationship;
        }

        public override string ToString()
        {
            return Name;
        }

        public IDependency Dependency { get; set; }

        public string GetInfo()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Field Summary");
            builder.Append(Environment.NewLine);
            builder.AppendLine(String.Format("Name: {0}", Name));
            // more to come

            builder.Append(Environment.NewLine);

            if (IsConstant)
                builder.AppendLine("IsConstant");
            if (IsEvent)
                builder.AppendLine("IsEvent");
            if (IsPrivate)
                builder.AppendLine("IsPrivate");
            if (IsProtected)
                builder.AppendLine("IsProtected");
            if (IsPublic)
                builder.AppendLine("IsPublic");
            if (IsReadOnly)
                builder.AppendLine("IsReadOnly");
            if (IsStatic)
                builder.AppendLine("IsStatic");

            return builder.ToString();
        }
        
        public BitmapSource Icon { get { return NodeIconService.GetIcon(this); } }
    }
}
