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
using System.Runtime.InteropServices.ComTypes;

namespace OffregLib
{
    /// <summary>
    ///     Object representing all the results from the ORQueryInfoKey call
    /// </summary>
    internal class QueryInfoKeyData
    {
        public string Class { get; set; }
        public uint SubKeysCount { get; set; }

        /// <summary>
        ///     Chars size, excluding null char.
        /// </summary>
        public uint MaxSubKeyLen { get; set; }

        /// <summary>
        ///     Chars size, excluding null char.
        /// </summary>
        public uint MaxClassLen { get; set; }

        public uint ValuesCount { get; set; }

        /// <summary>
        ///     Chars size, excluding null char.
        /// </summary>
        public uint MaxValueNameLen { get; set; }

        public uint MaxValueLen { get; set; }
        public uint SizeSecurityDescriptor { get; set; }
        public FILETIME LastWriteTime { get; set; }
    }
}