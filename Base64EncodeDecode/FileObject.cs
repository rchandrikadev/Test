using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64EncodeDecode
{
    /// <summary>
    /// Defines the file name and binary data for a file object
    /// </summary>
    public class FileObject
    {
        private string _fileName;
        private byte[] _fileData;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public byte [] FileData
        {
            get { return _fileData; }
            set { _fileData = value; }
        }
    }
}
