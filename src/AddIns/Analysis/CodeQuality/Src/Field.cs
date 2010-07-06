using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public Type Owner { get; set; }

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
        public ISet<Type> GenericTypes { get; set; }

        public Field()
        {
            FieldType = null;
            IsEvent = false;
            Owner = null;

            Dependency = null;
        }

        public override string ToString()
        {
            return Name;
        }

        public IDependency Dependency { get; set; }

        public string GetInfo()
        {
            return this.ToString();
        }
    }
}
