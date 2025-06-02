﻿#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 *
 * Contributions by Andy Gill, James Talton and Georg Wächter.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace LibreLancer
{
    /// <summary>
    /// Contains common mathematical functions and constants.
    /// </summary>
    public static class MathHelper
    {
        #region Fields

        /// <summary>
        /// Defines the value of Pi as a <see cref="System.Single"/>.
        /// </summary>
        public const float Pi = 3.141592653589793238462643383279502884197169399375105820974944592307816406286208998628034825342117067982148086513282306647093844609550582231725359408128481117450284102701938521105559644622948954930382f;

        /// <summary>
        /// Defines the value of Pi divided by two as a <see cref="System.Single"/>.
        /// </summary>
        public const float PiOver2 = Pi / 2;

        /// <summary>
        /// Defines the value of Pi divided by three as a <see cref="System.Single"/>.
        /// </summary>
        public const float PiOver3 = Pi / 3;

        /// <summary>
        /// Definesthe value of  Pi divided by four as a <see cref="System.Single"/>.
        /// </summary>
        public const float PiOver4 = Pi / 4;

        /// <summary>
        /// Defines the value of Pi divided by six as a <see cref="System.Single"/>.
        /// </summary>
        public const float PiOver6 = Pi / 6;

        /// <summary>
        /// Defines the value of Pi multiplied by two as a <see cref="System.Single"/>.
        /// </summary>
        public const float TwoPi = 2 * Pi;

        /// <summary>
        /// Defines the value of Pi multiplied by 3 and divided by two as a <see cref="System.Single"/>.
        /// </summary>
        public const float ThreePiOver2 = 3 * Pi / 2;

        /// <summary>
        /// Defines the value of E as a <see cref="System.Single"/>.
        /// </summary>
        public const float E = 2.71828182845904523536f;

        /// <summary>
        /// Defines the base-10 logarithm of E.
        /// </summary>
        public const float Log10E = 0.434294482f;

        /// <summary>
        /// Defines the base-2 logarithm of E.
        /// </summary>
        public const float Log2E = 1.442695041f;

        #endregion

        #region Public Members

        #region NextPowerOfTwo

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static long NextPowerOfTwo(long n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n", "Must be positive.");
            return (long)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
        }

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static int NextPowerOfTwo(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n", "Must be positive.");
            return (int)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
        }

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static float NextPowerOfTwo(float n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n", "Must be positive.");
            return (float)System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
        }

        /// <summary>
        /// Returns the next power of two that is larger than the specified number.
        /// </summary>
        /// <param name="n">The specified number.</param>
        /// <returns>The next power of two.</returns>
        public static double NextPowerOfTwo(double n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException("n", "Must be positive.");
            return System.Math.Pow(2, System.Math.Ceiling(System.Math.Log((double)n, 2)));
        }

        #endregion

        #region Factorial

        /// <summary>Calculates the factorial of a given natural number.
        /// </summary>
        /// <param name="n">The number.</param>
        /// <returns>n!</returns>
        public static long Factorial(int n)
        {
            long result = 1;

            for (; n > 1; n--)
                result *= n;

            return result;
        }

        #endregion

        #region BinomialCoefficient

        /// <summary>
        /// Calculates the binomial coefficient <paramref name="n"/> above <paramref name="k"/>.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="k">The k.</param>
        /// <returns>n! / (k! * (n - k)!)</returns>
        public static long BinomialCoefficient(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        #endregion

        #region InverseSqrtFast

        /// <summary>
        /// Returns an approximation of the inverse square root of left number.
        /// </summary>
        /// <param name="x">A number.</param>
        /// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001</returns>
        /// <remarks>
        /// This is an improved implementation of the the method known as Carmack's inverse square root
        /// which is found in the Quake III source code. This implementation comes from
        /// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
        /// http://www.beyond3d.com/content/articles/8/
        /// </remarks>
        public static float InverseSqrtFast(float x)
        {
            unsafe
            {
                float xhalf = 0.5f * x;
                int i = *(int*)&x;              // Read bits as integer.
                i = 0x5f375a86 - (i >> 1);      // Make an initial guess for Newton-Raphson approximation
                x = *(float*)&i;                // Convert bits back to float
                x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
                return x;
            }
        }

        /// <summary>
        /// Returns an approximation of the inverse square root of left number.
        /// </summary>
        /// <param name="x">A number.</param>
        /// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001</returns>
        /// <remarks>
        /// This is an improved implementation of the the method known as Carmack's inverse square root
        /// which is found in the Quake III source code. This implementation comes from
        /// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
        /// http://www.beyond3d.com/content/articles/8/
        /// </remarks>
        public static double InverseSqrtFast(double x)
        {
            return InverseSqrtFast((float)x);
            // TODO: The following code is wrong. Fix it, to improve precision.
#if false
            unsafe
            {
                double xhalf = 0.5f * x;
                int i = *(int*)&x;              // Read bits as integer.
                i = 0x5f375a86 - (i >> 1);      // Make an initial guess for Newton-Raphson approximation
                x = *(float*)&i;                // Convert bits back to float
                x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
                return x;
            }
#endif
        }

        #endregion

        #region DegreesToRadians

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees">An angle in degrees</param>
        /// <returns>The angle expressed in radians</returns>
        public static float DegreesToRadians(float degrees)
        {
            const float degToRad = (float)System.Math.PI / 180.0f;
            return degrees * degToRad;
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">An angle in radians</param>
        /// <returns>The angle expressed in degrees</returns>
        public static float RadiansToDegrees(float radians)
        {
            const float radToDeg = 180.0f / (float)System.Math.PI;
            return radians * radToDeg;
        }

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees">An angle in degrees</param>
        /// <returns>The angle expressed in radians</returns>
        public static double DegreesToRadians(double degrees)
        {
            const double degToRad = System.Math.PI / 180.0;
            return degrees * degToRad;
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">An angle in radians</param>
        /// <returns>The angle expressed in degrees</returns>
        public static double RadiansToDegrees(double radians)
        {
            const double radToDeg = 180.0 / System.Math.PI;
            return radians * radToDeg;
        }

        #endregion

        #region Swap

        /// <summary>
        /// Swaps two double values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        public static void Swap(ref double a, ref double b)
        {
            double temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Swaps two float values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        public static void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Clamps a number between a minimum and a maximum.
        /// </summary>
        /// <param name="n">The number to clamp.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>min, if n is lower than min; max, if n is higher than max; n otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Clamp<T>(T n, T min, T max) where T : IBinaryNumber<T>
        {
            return n < min ? min : n > max ? max : n;
        }

        /// <summary>
        /// Clamps a number between a minimum and a maximum.
        /// </summary>
        /// <param name="n">The number to clamp.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>min, if n is lower than min; max, if n is higher than max; n otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float n, float min, float max)
        {
            if (Sse.IsSupported)
            {
                return Sse.MinScalar(
                        Sse.MaxScalar(Vector128.CreateScalarUnsafe(n), Vector128.CreateScalarUnsafe(min)),
                        Vector128.CreateScalarUnsafe(max))
                    .ToScalar();
            }
            if (AdvSimd.IsSupported)
            {
                return AdvSimd.MinNumberScalar(
                    AdvSimd.MaxNumberScalar(Vector64.CreateScalarUnsafe(n), Vector64.CreateScalarUnsafe(min)),
                    Vector64.CreateScalarUnsafe(max)).ToScalar();
            }
            return Clamp<float>(n, min, max);
        }

		#endregion

		#endregion

        public static float Lerp(float value1, float value2, float amount)
            => MathF.FusedMultiplyAdd(amount, value2 - value1, value1);

        public static float Snap(float s, float step)
        {
            if (step != 0f)
            {
                return MathF.Floor((s / step) + 0.5f) * step;
            }
            return s;
        }

        public static Vector2 Snap(Vector2 vector, Vector2 step) =>
            new Vector2(Snap(vector.X, step.X), Snap(vector.Y, step.Y));

        public static bool IsPowerOfTwo(int x)
        {
            if (x == 0) return false;
            return (x & (x - 1)) == 0;
        }

        public static float WrapF(float x, float max)
        {
            return (max + x % max) % max;
        }

        public static float QuatError(Quaternion a, Quaternion b)
        {
            if (a.W < 0) a = -a;
            if (b.W < 0) b = -b;
            var errorQuat = 1 - Quaternion.Dot(a, b);
            return errorQuat < float.Epsilon ? 0 : errorQuat;
        }

        public static float WrapF(float x, float min, float max)
        {
            return min + WrapF(x - min, max - min);
        }

        public static Vector3 ApplyEpsilon(Vector3 input, float epsilon = 0.0001f)
        {
            var output = input;
            if (Math.Abs(output.X) < epsilon) output.X = 0;
            if (Math.Abs(output.Y) < epsilon) output.Y = 0;
            if (Math.Abs(output.Z) < epsilon) output.Z = 0;
            return output;
        }

        public static Matrix4x4 MatrixFromEulerDegrees(Vector3 angles)
        {
            angles *= (MathF.PI / 180.0f);
            return  Matrix4x4.CreateRotationX(angles.X) *
                    Matrix4x4.CreateRotationY(angles.Y) *
                    Matrix4x4.CreateRotationZ(angles.Z);
        }

        public static Quaternion QuatFromEulerDegrees(Vector3 angles) =>
            MatrixFromEulerDegrees(angles).ExtractRotation();

        public static Quaternion QuatFromEulerDegrees(float x, float y, float z)
        {
            return MatrixFromEulerDegrees(x, y, z).ExtractRotation();
            //Not equivalent?
            /*x *= MathF.PI / 180.0f;
            y *= MathF.PI / 180.0f;
            z *= MathF.PI / 180.0f;
            return Quaternion.CreateFromAxisAngle(Vector3.UnitX, x) *
                   Quaternion.CreateFromAxisAngle(Vector3.UnitY, y) *
                   Quaternion.CreateFromAxisAngle(Vector3.UnitZ, z);*/
        }
        public static Matrix4x4 MatrixFromEulerDegrees(float x, float y, float z)
        {
            x *= MathF.PI / 180.0f;
            y *= MathF.PI / 180.0f;
            z *= MathF.PI / 180.0f;
            return  Matrix4x4.CreateRotationX(x) *
                    Matrix4x4.CreateRotationY(y) *
                    Matrix4x4.CreateRotationZ(z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetFlag(int flags, int idx) =>
            (flags & (1 << idx)) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFlag(ref int flags, int idx, bool value)
        {
            if (value)
                flags |= (1 << idx);
            else
                flags &= ~(1 << idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFlag<T>(ref T source, T flag) where T : struct, Enum
        {
            var underlying = typeof(T).GetEnumUnderlyingType();
            if (underlying == typeof(byte) || underlying == typeof(sbyte))
            {
                var flagB = Unsafe.BitCast<T, byte>(flag);
                ref var sourceB = ref Unsafe.As<T, byte>(ref source);
                sourceB |= flagB;
            }
            if (underlying == typeof(short) || underlying == typeof(ushort))
            {
                var flagS = Unsafe.BitCast<T, ushort>(flag);
                ref var sourceS = ref Unsafe.As<T, ushort>(ref source);
                sourceS |= flagS;
            }
            if (underlying == typeof(int) || underlying == typeof(uint))
            {
                var flagI = Unsafe.BitCast<T, uint>(flag);
                ref var sourceI = ref Unsafe.As<T, uint>(ref source);
                sourceI |= flagI;
            }
            if (underlying == typeof(long) || underlying == typeof(ulong))
            {
                var flagL = Unsafe.BitCast<T, ulong>(flag);
                ref var sourceL = ref Unsafe.As<T, ulong>(ref source);
                sourceL |= flagL;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnsetFlag<T>(ref T source, T flag) where T : struct, Enum
        {
            var underlying = typeof(T).GetEnumUnderlyingType();
            if (underlying == typeof(byte) || underlying == typeof(sbyte))
            {
                var flagB = Unsafe.BitCast<T, byte>(flag);
                ref var sourceB = ref Unsafe.As<T, byte>(ref source);
                sourceB &= (byte)~flagB;
            }
            if (underlying == typeof(short) || underlying == typeof(ushort))
            {
                var flagS = Unsafe.BitCast<T, ushort>(flag);
                ref var sourceS = ref Unsafe.As<T, ushort>(ref source);
                sourceS &= (ushort)~flagS;
            }
            if (underlying == typeof(int) || underlying == typeof(uint))
            {
                var flagI = Unsafe.BitCast<T, uint>(flag);
                ref var sourceI = ref Unsafe.As<T, uint>(ref source);
                sourceI &= ~flagI;
            }
            if (underlying == typeof(long) || underlying == typeof(ulong))
            {
                var flagL = Unsafe.BitCast<T, ulong>(flag);
                ref var sourceL = ref Unsafe.As<T, ulong>(ref source);
                sourceL &= ~flagL;
            }
        }

    }
}
