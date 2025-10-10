namespace RomArchiver.Domain
{
    internal interface IProcessInfo
    {
        string Arguments
        {
            get;
        }

        string UserfriendlyProgress(int progress);
    }
}
