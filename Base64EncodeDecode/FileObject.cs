using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64EncodeDecode
{
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
