using System;

namespace System.Drawing {
    /// <summary>
    /// The <code>Dimension2D</code> class is to encapsulate a width 
    /// and a height dimension.
    /// </summary>
    /// <remarks>
    /// This class is only the abstract baseclass for all objects that
    /// store a 2D dimension.
    /// The actual storage representation of the sizes is left to
    /// the subclass.
    /// </remarks>
    public abstract class Dimension2D : ICloneable {
        /// <summary>
        /// This is an abstract class that cannot be instantiated directly.
        /// Type-specific implementation subclasses are available for
        /// instantiation and provide a number of formats for storing
        /// the information necessary to satisfy the various accessor
        /// methods below.
        /// </summary>
        /// <seealso cref="T:System.Drawing.Dimension"/>
        protected Dimension2D() {
        }

        /// <summary>
        /// Returns the width of this <code>Dimension</code> in double 
        /// precision.
        /// </summary>
        ///    <value>the width</value>
        public abstract double Width {get;}

        /// <summary>
        /// Returns the height of this <code>Dimension</code> in double 
        /// precision.
        /// </summary>
        /// <value>the height</value>
        public abstract double Height {get;}

        /// <summary>
        /// Sets the size of this <code>Dimension</code> object to the 
        /// specified width and height.
        /// </summary>
        /// <param name="width">the new width for the <code>Dimension</code>
        /// object</param>
        /// <param name="height">the new height for the <code>Dimension</code> 
        /// object</param>
        public abstract void SetSize(double width, double height);

        /// <summary>
        /// Sets the size of this <code>Dimension2D</code> object to 
        /// match the specified size.
        /// </summary>
        /// <value>the size</value>
        public Dimension2D Size {
            set {
                SetSize(value.Width, value.Height);
            }
        }

        /// <summary>
        /// Creates a new object of the same class as this object.
        /// </summary>
        /// <returns>a clone of this instance</returns>
        public Object Clone() {
            throw new Exception("not implemented");
        }
    }
}
