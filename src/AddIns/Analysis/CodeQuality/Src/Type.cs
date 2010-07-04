using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Type : IDependency
    {
        /// <summary>
        /// Nested types like inner classes, interfaces and so on.
        /// </summary>
        public ISet<Type> NestedTypes { get; set; }

        /// <summary>
        /// Type which owns this type. If this isn't nested type so Owner is null.
        /// </summary>
        public Type Owner { get; set; }

        /// <summary>
        /// Inherited type. If there is no inheritance so it is null.
        /// </summary>
        public Type BaseType { get; set; }

        /// <summary>
        /// Interfaces which are implemented by this type
        /// </summary>
        public ISet<Type> ImplementedInterfaces { get; set; }

        /// <summary>
        /// Methods within type
        /// </summary>
        public ISet<Method> Methods { get; set; }

        /// <summary>
        /// Fields within type
        /// </summary>
        public ISet<Field> Fields { get; set; }

        /// <summary>
        /// Events within type
        /// </summary>
        public ISet<Event> Events { get; set; }

        /// <summary>
        /// Name of type with a name of namespace.
        /// </summary>
        public string FullName
        {
            get
            {
                return Namespace.Name + "." + Name;
            }
        }

        /// <summary>
        /// Name of type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Namespace where type is located
        /// </summary>
        public Namespace Namespace { get; set; }

        /// <summary>
        /// Whether the type is interface
        /// </summary>
        public bool IsInterface { get; set; }

        /// <summary>
        /// Whether the type is enum
        /// </summary>
        public bool IsEnum { get; set; }

        /// <summary>
        /// Whether the type is class
        /// </summary>
        public bool IsClass { get; set; }

        /// <summary>
        /// Whether the type is sealed
        /// </summary>
        public bool IsSealed { get; set; }

        /// <summary>
        /// Whether the type is abstract
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Whether the type is public
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Whether the type is struct
        /// </summary>
        public bool IsStruct { get; set; }

        /// <summary>
        /// Whether the type is internal
        /// </summary>
        public bool IsInternal { get; set; }

        /// <summary>
        /// Whether the type is delagate
        /// </summary>
        public bool IsDelegate { get; set; }

        /// <summary>
        /// If it is nested type whether is private
        /// </summary>
        public bool IsNestedPrivate { get; set; }

        /// <summary>
        /// If it is nested type whether is public
        /// </summary>
        public bool IsNestedPublic { get; set; }

        /// <summary>
        /// If it is nested type whether is protected
        /// </summary>
        public bool IsNestedProtected { get; set; }


        public Type()
        {
            Methods = new HashSet<Method>();
            Fields = new HashSet<Field>();
            Events = new HashSet<Event>();
            NestedTypes = new HashSet<Type>();
            ImplementedInterfaces = new HashSet<Type>();

            Owner = null;
            BaseType = null;
        }

        /// <summary>
        /// Returns all types which are used in this type
        /// </summary>
        /// <returns>A set of types</returns>
        public ISet<Type> GetUses()
        {
            var set = new HashSet<Type>();

            foreach (var method in Methods)
            {
                set.UnionWith(method.TypeUses);
            }

            foreach (var field in Fields)
            {
                if (field.FieldType != null) // if it is null so it is type from outside of this assembly
                    set.Add(field.FieldType); // TODO: find solution to handle them
            }

            return set;
        }

        public BidirectionalGraph<object, IEdge<object>> BuildDependencyGraph()
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();

            foreach (var method in Methods)
            {
                g.AddVertex(method.Name);
            }

            foreach (var field in Fields)
            {
                g.AddVertex(field.Name);
            }

            foreach (var method in Methods)
            {
                foreach (var methodUse in method.MethodUses)
                {
                    g.AddEdge(new Edge<object>(method.Name, methodUse.Name));
                }

                foreach (var fieldUse in method.FieldUses)
                {
                    g.AddEdge(new Edge<object>(method.Name, fieldUse.Name));
                }
            }

            return g;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
