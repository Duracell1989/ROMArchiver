using System;

namespace RomLister
{
    internal class DatCacheEntity
    {
        private string _filePath;
        private string _archivedFilePath;
        private Int32 _compressedFileSize;
        private UInt32 _ModifyNo;
        private string _crc;
        private string _archiveType;
        private Boolean _cleaned;
        private Boolean _isArchived;
        private Boolean _isAltered;
        private UInt64 _fileSize;

        internal string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        internal string FilenameInArchive
        {
            get { return _archivedFilePath; }
            set { _archivedFilePath = value; }
        }

        internal Int32 CompressedFileSize
        {
            get { return _compressedFileSize; }
            set { _compressedFileSize = value; }
        }

        internal UInt32 ModifyNo
        {
            get { return _ModifyNo; }
            set { _ModifyNo = value; }
        }

        internal string Crc
        {
            get { return _crc; }
            set { _crc = value; }
        }

        internal string ArchiveType
        {
            get { return _archiveType; }
            set { _archiveType = value; }
        }

        internal Boolean Cleaned
        {
            get { return _cleaned; }
            set { _cleaned = value; }
        }

        internal string CleanedValue
        {
            get { return _cleaned ? "clean" : string.Empty; }
        }

        internal Boolean IsArchived
        {
            get { return _isArchived; }
            set { _isArchived = value; }
        }

        internal Boolean IsAltered
        {
            get { return _isAltered; }
            set { _isAltered = value; }
        }

        internal UInt64 FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        internal DatCacheEntity()
        {
        }
    }
}