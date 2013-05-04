using System;
using System.Runtime.InteropServices;

namespace SDRBlocks.Core
{
    /// <summary>
    /// Implements a classic complex data type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Complex
    {
        public float Re;
        public float Im;

        public Complex(float re, float im)
        {
            this.Re = re;
            this.Im = im;
        }

        #region "Operators"

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

        public float Mod()
        {
            return (float) Math.Sqrt(ModSq());
        }

        /// <summary>
        /// Modulus squared.
        /// </summary>
        /// <returns></returns>
        public float ModSq()
        {
            return this.Re * this.Re + this.Im * this.Im;
        }

        public override string ToString()
        {
            return String.Format("{{{0},{1}}}", this.Re, this.Im);
        }
    }
}
