# ROMArchiver

## General
This project was setup in 2013 after using another program called OfflineList.
The (re-)archiving possiblities within this application were limited (consisted of a very low compression rate) and didn't support the latest (better) archiving options.
This program works as follows:
- It reads the OfflineList .cache-files
- and re-archived the files which are present in the .cache-files but with a much better compression rate
- then updates the .cache-file to indicate the new file-size - so that OfflineList still thinks that nothing has changed and sees the files/cache as valid.

## Setup
Change the keys in the appsettings.json:

|Key|Value|
|---|---|
|OfflineListCacheDirectory|The cache folder which belongs to OfflineList.|
|RomDirectory|The directory in which the ROMs live, This Directory needs sub-directories for the different ROMs: 3DS, NDS, GBA.|
|WorkingDirectory|Temporary directory which will be used for extracting and (temporary) writing the new archive. Note: A fast SDD will improve performance during the re-archiving.|
|SevenZipExecutablePath|The full path towards the 7z.exe.|
|7zLocation|The full path towards the 7z.dll. This is needed by the SevenZipSharp NuGet Package.|
