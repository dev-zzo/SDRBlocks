using System;
using System.Runtime.InteropServices;

namespace SDRBlocks.Core.Maths
{
    /// <summary>
    /// Implements a classic complex data type.
    /// It could've been made immutable, but there's no real reason to do so atm.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Complex
    {
        public float Re;
        public float Im;

        public static Complex Zero = new Complex(0.0f, 0.0f);
        public static Complex RealOne = new Complex(1.0f, 0.0f);
        public static Complex ImagOne = new Complex(0.0f, 1.0f);

        public Complex(float re, float im)
        {
            this.Re = re;
            this.Im = im;
        }

        #region Operators

        public static Complex operator +(Complex lhs, Complex rhs)
        {
            return new Complex(lhs.Re + rhs.Re, lhs.Im + rhs.Im);
        }

        public static Complex operator -(Complex lhs, Complex rhs)
        {
            return new Complex(lhs.Re - rhs.Re, lhs.Im - rhs.Im);
        }

        public static Complex operator *(Complex lhs, Complex rhs)
        {
            return new Complex(
                lhs.Re * rhs.Re - lhs.Im * rhs.Im,
                lhs.Im * rhs.Re + lhs.Re * rhs.Im);
        }

        public static Complex operator *(Complex lhs, float rhs)
        {
            return new Complex(lhs.Re * rhs, lhs.Im * rhs);
        }

        public static Complex operator /(Complex lhs, Complex rhs)
        {
            float dn = 1.0f / rhs.ModSq();
            float re = (lhs.Re * rhs.Re + lhs.Im * rhs.Im) * dn;
            float im = (lhs.Im * rhs.Re - lhs.Re * rhs.Im) * dn;
            return new Complex(re, im);
        }

        public static Complex operator /(Complex lhs, float rhs)
        {
            float rcp = 1.0f / rhs;
            return new Complex(lhs.Re * rcp, lhs.Im * rcp);
        }

        public static Complex operator ~(Complex rhs)
        {
            return new Complex(rhs.Re, -rhs.Im);
        }

        public static bool operator ==(Complex lhs, Complex rhs)
        {
            return lhs.Re == rhs.Re && lhs.Im == rhs.Im;
        }

        public static bool operator !=(Complex lhs, Complex rhs)
        {
            return lhs.Re != rhs.Re || lhs.Im != rhs.Im;
        }

        #endregion

        /// <summary>
        /// Take modulus of this complex number.
        /// </summary>
        /// <returns></returns>
        public float Mod()
        {
            return (float)Math.Sqrt(this.ModSq());
        }

        /// <summary>
        /// Modulus squared (quite faster).
        /// </summary>
        /// <returns></returns>
        public float ModSq()
        {
            return this.Re * this.Re + this.Im * this.Im;
        }

        /// <summary>
        /// Take argument of this complex number.
        /// </summary>
        /// <returns></returns>
        public float Arg()
        {
            // TODO: Optimize this. Atan2 takes double's and is slow.
            return (float)Math.Atan2(this.Im, this.Re);
        }

        /// <summary>
        /// Returns the normalized value.
        /// </summary>
        /// <returns></returns>
        public Complex Normalize()
        {
            float norm = this.Mod();
            if (norm > 1e-10)
            {
                return this / norm;
            }
            return Zero;
        }

        public override string ToString()
        {
            return String.Format("{{{0},{1}j}}", this.Re, this.Im);
        }
    }
}
