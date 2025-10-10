namespace RomArchiver.Domain.Objects
{
    internal class DatRezEntity
    {
        private string _filePath;
        private uint _ModifyNo;
        private long _fileSize;
        private string _archiveType;
        private short _compressionLevel;

        internal string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        internal uint ModifyNo
        {
            get { return _ModifyNo; }
            set { _ModifyNo = value; }
        }

        internal long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        internal string ArchiveType
        {
            get { return _archiveType; }
            set { _archiveType = value; }
        }

        internal short CompressionLevel
        {
            get { return _compressionLevel; }
            set { _compressionLevel = value; }
        }

        internal DatRezEntity()
        {
        }
    }
}