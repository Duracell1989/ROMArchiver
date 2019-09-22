using System;

namespace RomLister
{
    internal class DatRezEntity
    {
        private string _filePath;
        private UInt32 _ModifyNo;
        private Int64 _fileSize;
        private string _archiveType;
        private Int16 _compressionLevel;

        internal string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        internal UInt32 ModifyNo
        {
            get { return _ModifyNo; }
            set { _ModifyNo = value; }
        }

        internal Int64 FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        internal string ArchiveType
        {
            get { return _archiveType; }
            set { _archiveType = value; }
        }

        internal Int16 CompressionLevel
        {
            get { return _compressionLevel; }
            set { _compressionLevel = value; }
        }

        internal DatRezEntity()
        {
        }
    }
}