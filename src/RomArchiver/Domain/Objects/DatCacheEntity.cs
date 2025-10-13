using System.Text.Json.Serialization;

namespace RomArchiver.Domain.Objects
{
    internal class DatCacheEntity
    {
        internal string FilePath { get; set; }

        internal string FilenameInArchive { get; set; }

        internal int CompressedFileSize { get; set; }

        internal uint ModifyNo { get; set; }

        [JsonPropertyName("CRC")]
        public string Crc { get; set; }

        internal string ArchiveType { get; set; }

        internal bool Cleaned { get; set; }

        internal string CleanedValue => Cleaned ? "clean" : string.Empty;

        internal bool IsArchived { get; set; }

        internal bool IsAltered { get; set; }

        internal ulong FileSize { get; set; }

        public DatCacheEntity()
        {
        }
    }
}