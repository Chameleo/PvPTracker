using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoryIO
{
    // By Apoc of Mmowned
    internal static class Memory
    {
        internal static IntPtr ImageBase;

        private static T ReadInternal<T>(IntPtr address)
        {
            object ret;

            // Handle types that don't have a real typecode
            // and/or can be done without the ReadByte bullshit
            if (typeof (T) == typeof (IntPtr))
            {
                ret = (IntPtr) BitConverter.ToUInt32(ReadBytes(address, 4), 0);
                return (T) ret;
            }
            if (typeof(T) == typeof(string))
            {
                //System.Windows.Forms.MessageBox.Show("Getting string at " + address.ToString("X8"));
                // Simple enough right?
                List<byte> bytes = new List<byte>();
                // 1 more read...
                var bs = ReadBytes((IntPtr) ReadInternal<uint>(address), 255);
                for (int i = 0; i < bs.Length; i++)
                {
                    if (bs[i] == '\0')
                        break;
                    bytes.Add(bs[i]);
                }
                ret = Encoding.UTF8.GetString(bytes.ToArray());
                return (T) ret;
            }

            int size = Marshal.SizeOf(typeof (T));
            byte[] ba = ReadBytes(address, size);

            switch (Type.GetTypeCode(typeof (T)))
            {
                case TypeCode.Boolean:
                    ret = BitConverter.ToBoolean(ba, 0);
                    break;
                case TypeCode.Char:
                    ret = BitConverter.ToChar(ba, 0);
                    break;
                case TypeCode.Byte:
                    ret = ba[0];
                    break;
                case TypeCode.Int16:
                    ret = BitConverter.ToInt16(ba, 0);
                    break;
                case TypeCode.UInt16:
                    ret = BitConverter.ToUInt16(ba, 0);
                    break;
                case TypeCode.Int32:
                    ret = BitConverter.ToInt32(ba, 0);
                    break;
                case TypeCode.UInt32:
                    ret = BitConverter.ToUInt32(ba, 0);
                    break;
                case TypeCode.Int64:
                    ret = BitConverter.ToInt64(ba, 0);
                    break;
                case TypeCode.UInt64:
                    ret = BitConverter.ToUInt64(ba, 0);
                    break;
                case TypeCode.Single:
                    ret = BitConverter.ToSingle(ba, 0);
                    break;
                case TypeCode.Double:
                    ret = BitConverter.ToDouble(ba, 0);
                    break;
                default:
                    throw new NotSupportedException(typeof (T).FullName + " is not currently supported by Read<T>");
            }
            return (T) ret;
        }

        /// <summary>
        /// Reads a specific number of bytes from memory.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static byte[] ReadBytes(IntPtr address, int count)
        {
            return Native.ReadBytes(address, count);
        }

        public static T ReadRelative<T>(params IntPtr[] addresses)
        {
            if (addresses.Length >= 1)
            {
                addresses[0] = (IntPtr) (addresses[0].ToInt32() + ImageBase.ToInt32());
            }
            return Read<T>(addresses);
        }

        public static T ReadAtOffset<T>(IntPtr address, int offset)
        {
            return Read<T>(new IntPtr(address.ToInt32() + offset));
        }

        /// <summary>
        /// Reads a "T" from memory, using consecutive reading.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public static T Read<T>(params IntPtr[] addresses)
        {
            if (addresses.Length == 0)
            {
                return default(T);
            }
            if (addresses.Length == 1)
            {
                return ReadInternal<T>(addresses[0]);
            }

            IntPtr last = IntPtr.Zero;
            for (int i = 0; i < addresses.Length; i++)
            {
                if (i == addresses.Length - 1)
                {
                    return ReadInternal<T>((IntPtr) (addresses[i].ToInt64() + last.ToInt64()));
                }
                last = ReadInternal<IntPtr>(new IntPtr(last.ToInt64() + addresses[i].ToInt64()));
            }

            // Should never hit this.
            // The compiler just bitches.
            return default(T);
        }

        /// <summary>
        /// Writes a set of bytes to memory.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static bool WriteBytes(IntPtr address, byte[] bytes)
        {
            return Native.WriteBytes(address, bytes) == bytes.Length;
        }

        /// <summary>
        /// Writes a generic datatype to memory. (Note; only base datatypes are supported [int,float,uint,byte,sbyte,double,etc])
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Write<T>(IntPtr address, T value)
        {
            // We can't handle strings yet...
            // Maybe in the future. Since strings require special treatment
            // depending on the context. (Can we just write the string bytes? Do we need to write
            // them in a cave somewhere, and write the pointer to the cave somewhere else?
            // What type of string should be written? Etc.)

            // Just handle writing strings yourselves!
            if (typeof (T) == typeof (string))
            {
                throw new ArgumentException("Writing strings are not currently supported!", "value");
            }

            try
            {
                object val = value;

                byte[] bytes;

                // Handle types that don't have a real typecode
                // and/or can be done without the ReadByte bullshit
                if (typeof (T) == typeof (IntPtr))
                {
                    // Since we're already here...... we might as well do some stuffs.
                    WriteBytes(address, BitConverter.GetBytes((uint) val));
                    return true;
                }

                // Make sure we're handling passing in stuff as a byte array.
                if (typeof (T) == typeof (byte[]))
                {
                    bytes = (byte[]) val;
                    return Native.WriteBytes(address, bytes) == bytes.Length;
                }
                switch (Type.GetTypeCode(typeof (T)))
                {
                    case TypeCode.Boolean:
                        bytes = BitConverter.GetBytes((bool) val);
                        break;
                    case TypeCode.Char:
                        bytes = BitConverter.GetBytes((char) val);
                        break;
                    case TypeCode.Byte:
                        bytes = new[] {(byte) val};
                        break;
                    case TypeCode.Int16:
                        bytes = BitConverter.GetBytes((short) val);
                        break;
                    case TypeCode.UInt16:
                        bytes = BitConverter.GetBytes((ushort) val);
                        break;
                    case TypeCode.Int32:
                        bytes = BitConverter.GetBytes((int) val);
                        break;
                    case TypeCode.UInt32:
                        bytes = BitConverter.GetBytes((uint) val);
                        break;
                    case TypeCode.Int64:
                        bytes = BitConverter.GetBytes((long) val);
                        break;
                    case TypeCode.UInt64:
                        bytes = BitConverter.GetBytes((ulong) val);
                        break;
                    case TypeCode.Single:
                        bytes = BitConverter.GetBytes((float) val);
                        break;
                    case TypeCode.Double:
                        bytes = BitConverter.GetBytes((double) val);
                        break;
                    default:
                        throw new NotSupportedException(typeof (T).FullName + " is not currently supported by Write<T>");
                }
                return Native.WriteBytes(address, bytes) == bytes.Length;
            }
            catch
            {
                return false;
            }
        }
    }
}