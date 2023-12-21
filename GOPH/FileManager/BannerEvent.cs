namespace GOPH.FileManager
{
    public class BannerEvent : ObjectFolder
    {
        private BannerEvent() { }

        private static BannerEvent _instance = null;

        public static BannerEvent GetBannerEvent()
        {
            if (_instance is null)
            {
                return new BannerEvent();
            }

            return _instance;
        }

        public override string GetFileImage()
        {
            return base.GetFileImage();
        }

        public override string GetFolderRootDirectory()
        {
            return base.GetFolderRootDirectory();
        }
    }

}
