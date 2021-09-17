﻿//This code came from https://github.com/LordMike/OffregLib
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
namespace OffregLib
{
    /// <summary>
    /// Basic tuple to replicate that of .NET 3 and later
    /// </summary>
    /// <typeparam name="K">Type item 1</typeparam>
    /// <typeparam name="V">Type item 2</typeparam>
    internal class Tuple<K, V>
    {
        public K Item1 { get; set; }
        public V Item2 { get; set; }

        public Tuple()
        {
            
        }

        public Tuple(K item1, V item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}
