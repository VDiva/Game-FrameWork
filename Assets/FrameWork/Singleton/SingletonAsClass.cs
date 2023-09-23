namespace FrameWork.Singleton
{
    public class SingletonAsClass<T> where T: class, new()
    {
        
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance==null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    }
}