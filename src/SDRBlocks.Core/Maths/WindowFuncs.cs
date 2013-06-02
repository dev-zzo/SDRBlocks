using System;

// Reference: http://zone.ni.com/reference/en-XX/help/371361E-01/lvanlsconcepts/char_smoothing_windows/

namespace SDRBlocks.Core.Maths
{
    public enum WindowType
    {
        Hanning,
        Hamming,
        BartlettHanning,
        Blackman,
        BlackmanHarris,
        HannPoisson,
    }

    public static class WindowFuncs
    {
        public static float[] Build(WindowType windowType, int length)
        {
            float[] h = new float[length];

            float f2 = 1.0f * FastMath.TWOPI / (length - 1);
            float f4 = 2.0f * FastMath.TWOPI / (length - 1);
            float f6 = 3.0f * FastMath.TWOPI / (length - 1);

            switch (windowType)
            {
                case WindowType.Hanning:
                    for (int i = 0; i < length; ++i)
                    {
                        h[i] = 0.50f
                            - 0.50f * (float)Math.Cos(f2 * i);
                    }
                    break;

                case WindowType.Hamming:
                    for (int i = 0; i < length; ++i)
                    {
                        h[i] = 0.54f 
                            - 0.46f * (float)Math.Cos(f2 * i);
                    }
                    break;

                case WindowType.BartlettHanning:
                    for (int i = 0; i < length; ++i)
                    {
                        float a = i / length - 0.5f;
                        h[i] = 0.62f
                            - 0.48f * (float)Math.Abs(a)
                            + 0.38f * (float)Math.Cos(FastMath.TWOPI * a);
                    }
                    break;

                case WindowType.Blackman:
                    for (int i = 0; i < length; ++i)
                    {
                        h[i] = 0.42f
                            - 0.50f * (float)Math.Cos(f2 * i)
                            + 0.08f * (float)Math.Cos(f4 * i);
                    }
                    break;

                case WindowType.BlackmanHarris:
                    for (int i = 0; i < length; ++i)
                    {
                        h[i] = 0.35875f
                            - 0.48829f * (float)Math.Cos(f2 * i)
                            + 0.14128f * (float)Math.Cos(f4 * i)
                            - 0.01168f * (float)Math.Cos(f6 * i);
                    }
                    break;

                case WindowType.HannPoisson:
                    float alpha = 0.005f; // Adjustable?
                    float e = -2.0f * alpha / (length - 1);
                    for (int i = 0; i < length; ++i)
                    {
                        float n = i - (length - 1) / 2.0f;
                        h[i] = 0.5f * (1.0f + (float)Math.Cos(f2 * n)) * (float)Math.Exp(e * Math.Abs(n));
                    }
                    break;
            }
            return h;
        }

    }
}
