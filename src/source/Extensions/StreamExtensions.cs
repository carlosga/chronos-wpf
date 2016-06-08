using System;
using System.IO;
using System.Text;

namespace Chronos.Extensions
{
    /// <summary>
    /// Extension methods any kind of streams
    /// </summary>
    public static class StreamExtensions
    {
        #region · Extensions ·

        /// <summary>
        /// Opens a StreamReader using the default encoding.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The stream reader</returns>
        public static StreamReader GetReader(this Stream stream)
        {
            return stream.GetReader(null);
        }

        /// <summary>
        /// Opens a StreamReader using the specified encoding.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The stream reader</returns>
        public static StreamReader GetReader(this Stream stream, Encoding encoding)
        {
            if (stream.CanRead == false)
            {
                throw new InvalidOperationException("Stream does not support reading.");
            }

            encoding = (encoding ?? Encoding.Default);

            return new StreamReader(stream, encoding);
        }

        /// <summary>
        /// Opens a StreamWriter using the default encoding.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The stream writer</returns>
        public static StreamWriter GetWriter(this Stream stream)
        {
            return stream.GetWriter(null);
        }

        /// <summary>
        /// Opens a StreamWriter using the specified encoding.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The stream writer</returns>
        public static StreamWriter GetWriter(this Stream stream, Encoding encoding)
        {
            if (stream.CanWrite == false)
            {
                throw new InvalidOperationException("Stream does not support writing.");
            }

            encoding = (encoding ?? Encoding.Default);
            
            return new StreamWriter(stream, encoding);
        }

        /// <summary>
        /// Reads all text from the stream using the default encoding.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The result string.</returns>
        public static string ReadToEnd(this Stream stream)
        {
            return stream.ReadToEnd(null);
        }

        /// <summary>
        /// Reads all text from the stream using a specified encoding.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The result string.</returns>
        public static string ReadToEnd(this Stream stream, Encoding encoding)
        {
            using (var reader = stream.GetReader(encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Sets the stream cursor to the beginning of the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The stream</returns>
        public static Stream SeekToBegin(this Stream stream)
        {
            if (stream.CanSeek == false)
            {
                throw new InvalidOperationException("Stream does not support seeking.");
            }

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        /// <summary>
        /// Sets the stream cursor to the end of the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The stream</returns>
        public static Stream SeekToEnd(this Stream stream)
        {
            if (stream.CanSeek == false)
            {
                throw new InvalidOperationException("Stream does not support seeking.");
            }

            stream.Seek(0, SeekOrigin.End);
            
            return stream;
        }

        /// <summary>
        /// Copies any stream into a local MemoryStream
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <returns>The copied memory stream.</returns>
        public static MemoryStream CopyToMemory(this Stream stream)
        {
            var memoryStream = new MemoryStream((int)stream.Length);
            
            stream.CopyTo(memoryStream);
            
            return memoryStream;
        }

        /// <summary>
        /// Reads the entire stream and returns a byte array.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The byte array</returns>
        /// <remarks>Thanks to EsbenCarlsen  for providing an update to this method.</remarks>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            using (var memoryStream = stream.CopyToMemory())
            {
                return memoryStream.ToArray();
            }
        }

        #endregion
    }
}