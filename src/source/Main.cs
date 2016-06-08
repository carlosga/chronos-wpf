/*
The MIT License

Copyright (c) 2009-2010. Carlos Guzmán Álvarez. http://chronoswpf.codeplex.com/

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using NLog;

namespace Chronos
{
    /// <summary>
    /// Application entry point class
    /// </summary>
    public static class ChronosMain
    {
        /// <summary>
        /// Entry Point
        /// </summary>
        /// <param name="args">The args.</param>
        [STAThread]
        public static void Main(params string[] args)
        {
            if (SingleInstance.InitializeAsFirstInstance("Chronos"))
            {
                var application = new App();

                application.InitializeComponent();

                if (args != null && args.Length > 0)
                {
                    application.Run(args);
                }
                else
                {
                    application.Run();
                }

                // Allow single instance code to perform cleanup operations
                SingleInstance.Cleanup();
            }
        }
    }
}