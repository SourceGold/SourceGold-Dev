using System.Reflection;

namespace Assets.Script
{
    public abstract class DataPersistence : IDataPersistence
    {
        public DataPersistence(bool autoRegister = true)
        {
            if (autoRegister)
            {
                ((IDataPersistence)this).RegisterExistence();
            }
        }

        public abstract void Restart();

        public virtual void LoadData(string fileName)
        {
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

            MethodInfo methodInfo = DataPersistenceManager.Instance.GetType().GetMethod(nameof(DataPersistenceManager.LoadDataFile));
            methodInfo = methodInfo.MakeGenericMethod(GetType());
            var objFromFile = methodInfo.Invoke(null, new object[] { fileName });

            var dstPropertyInfo = this.GetType().GetProperties(bindingFlags);
            foreach (var dstProperty in dstPropertyInfo)
            {
                foreach (var srcProperty in objFromFile.GetType().GetProperties())
                {
                    if (dstProperty.CanWrite && dstProperty.Name == srcProperty.Name)
                    {
                        dstProperty.SetValue(this, srcProperty.GetValue(objFromFile));
                        break;
                    }
                }
            }
        }

        public virtual void SaveData(string fileName)
        {
            DataPersistenceManager.SaveDataFile(fileName, this);
        }

        public virtual string GetSaveFileName()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Placeholder for now, might need to be changed later when actual implementation is needed
        /// </summary>
        public virtual void Dispose()
        {
            ((IDataPersistence)this).Dispose();
        }
    }
}
