using System.Runtime.CompilerServices;

namespace GOPH.FileManager
{
    public abstract class ObjectFolder  : IObjectFolder
    {

        public virtual string GetFileImage()
        {
          
            return this.GetType().Name;
            
        }

        public virtual string GetFolderRootDirectory()
        {  
                return "ImageManager";           
        }
    }
}
