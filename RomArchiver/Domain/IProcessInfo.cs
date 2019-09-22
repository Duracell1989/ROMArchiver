namespace RomLister
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
