//This code came from https://github.com/LordMike/OffregLib
//Copyright (c) 2021 Michael Bisbjerg

/*
 * MIT License

Copyright (c) 2021 Michael Bisbjerg

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

---

Some files are the property of Microsoft, and are not distributed with the above license.
Those files are offreg.x64.dll and offreg.x86.dll.
 */
using System;
using System.Text;

namespace OffregLib
{
    internal static class OffregHelper
    {
        /// <summary>
        ///     Represents the encoding used by Windows Registry.
        ///     UTF-16 is valid for Windows 2000 and forward.
        /// </summary>
        public static Encoding StringEncoding
        {
            get { return Encoding.Unicode; }
        }

        /// <summary>
        ///     The number of bytes for a single character in UTF-16.
        ///     Commonly used to pad byte arrays / sizes with nulls
        /// </summary>
        public const int SingleCharBytes = 2;

        /// <summary>
        ///     Converts some binary data into the form used in the CLR.
        /// </summary>
        /// <param name="type">The type of data to convert from.</param>
        /// <param name="data">The data to convert.</param>
        /// <param name="parsedData">The parsed CLR object.</param>
        /// <returns>Indication if the parsed data could be parsed correctly as the specified type. If false, data is always a byte[].</returns>
        public static bool TryConvertValueDataToObject(RegValueType type, byte[] data, out object parsedData)
        {
            parsedData = data;

            switch (type)
            {
                case RegValueType.REG_NONE:
                    // NONE format shouldn't be specified, ever.
                    return false;
                case RegValueType.REG_LINK: // This is a unicode string
                case RegValueType.REG_EXPAND_SZ: // This is a unicode string
                case RegValueType.REG_SZ:
                    if (data.Length % 2 != 0)
                        // UTF-16 strings are always an even number of bytes
                        return false;

                    // Remove all the trailing nulls
                    int toIndex = 0;
                    while (data.Length > toIndex + 2 && (data[toIndex] != 0 || data[toIndex + 1] != 0))
                        toIndex += 2;

                    parsedData = StringEncoding.GetString(data, 0, toIndex);
                    return true;
                case RegValueType.REG_BINARY:
                    return true;
                case RegValueType.REG_DWORD:
                    if (data.Length != 4)
                        return false;

                    parsedData = BitConverter.ToInt32(data, 0);
                    return true;
                case RegValueType.REG_DWORD_BIG_ENDIAN:
                    if (data.Length != 4)
                        return false;

                    Array.Reverse(data);
                    parsedData = BitConverter.ToInt32(data, 0);
                    return true;
                case RegValueType.REG_MULTI_SZ:
                    // Get string without the ending null
                    if (data.Length % 2 != 0)
                        // Invalid string data, must always be even in length
                        return false;

                    if (data.Length == 0)
                        // A badly formatted list
                        return false;

                    if (data.Length == 2 && data[0] == 0 && data[1] == 0)
                    {
                        // An empty list is identified by: \0
                        parsedData = new string[0];
                        return true;
                    }

                    if (data[data.Length - 4] != 0 || data[data.Length - 3] != 0 || data[data.Length - 2] != 0 ||
                        data[data.Length - 1] != 0)
                        // Must always end with four nulls
                        return false;

                    string s2 = StringEncoding.GetString(data, 0, data.Length - 4);
                    parsedData = s2.Split(new[] { '\0' });
                    return true;
                case RegValueType.REG_RESOURCE_LIST:
                    return true;
                case RegValueType.REG_FULL_RESOURCE_DESCRIPTOR:
                    return true;
                case RegValueType.REG_RESOURCE_REQUIREMENTS_LIST:
                    return true;
                case RegValueType.REG_QWORD:
                    if (data.Length != 8)
                        return false;

                    parsedData = BitConverter.ToInt64(data, 0);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException("TryConvertValueDataToObject was given an invalid RegValueType: " + type);
            }
        }
    }
}