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
using System.ComponentModel;

namespace OffregLib
{
    public class OffregHive : OffregBase
    {
        /// <summary>
        ///     The Root key of this hive.
        /// </summary>
        public OffregKey Root { get; private set; }

        /// <summary>
        ///     Internal constructor to form an Offline Registry Hive from an open handle.
        /// </summary>
        /// <param name="hivePtr"></param>
        internal OffregHive(IntPtr hivePtr)
        {
            _intPtr = hivePtr;

            // Represent this as a key also
            Root = new OffregKey(null, _intPtr, null);
        }

        /// <summary>
        ///     Saves a hive to Disk.
        ///     See http://msdn.microsoft.com/en-us/library/ee210773(v=vs.85).aspx for more details.
        /// </summary>
        /// <remarks>The target file must not exist.</remarks>
        /// <param name="targetFile">The target file to write to.</param>
        /// <param name="majorVersionTarget">The compatibility version to save for, see the link in summary.</param>
        /// <param name="minorVersionTarget">The compatibility version to save for, see the link in summary.</param>
        public void SaveHive(string targetFile, uint majorVersionTarget, uint minorVersionTarget)
        {
            Win32Result res = OffregNative.SaveHive(_intPtr, targetFile, majorVersionTarget, minorVersionTarget);

            if (res != Win32Result.ERROR_SUCCESS)
                throw new Win32Exception((int)res);
        }

        /// <summary>
        ///     Creates a new hive in memory.
        /// </summary>
        /// <returns>The newly created hive.</returns>
        public static OffregHive Create()
        {
            IntPtr newHive;
            Win32Result res = OffregNative.CreateHive(out newHive);

            if (res != Win32Result.ERROR_SUCCESS)
                throw new Win32Exception((int)res);

            return new OffregHive(newHive);
        }

        /// <summary>
        ///     Opens an existing hive from the disk.
        /// </summary>
        /// <param name="hiveFile">The file to open.</param>
        /// <returns>The newly opened hive.</returns>
        public static OffregHive Open(string hiveFile)
        {
            IntPtr existingHive;
            Win32Result res = OffregNative.OpenHive(hiveFile, out existingHive);

            if (res != Win32Result.ERROR_SUCCESS)
                throw new Win32Exception((int)res);

            return new OffregHive(existingHive);
        }

        /// <summary>
        ///     Closes the hive and releases ressources used by it.
        /// </summary>
        public override void Close()
        {
            if (_intPtr != IntPtr.Zero)
            {
                Win32Result res = OffregNative.CloseHive(_intPtr);

                if (res != Win32Result.ERROR_SUCCESS)
                    throw new Win32Exception((int)res);
            }
        }

        /// <summary>
        ///     Disposes the hive object and releases ressources used by it.
        /// </summary>
        public override void Dispose()
        {
            Close();
        }
    }
}