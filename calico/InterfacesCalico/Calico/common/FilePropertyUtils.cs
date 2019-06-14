using Nini.Config;
using System;

namespace Calico.common
{
    public sealed class FilePropertyUtils
    {
        private static readonly object padlock = new object();
        private static FilePropertyUtils instance = null;
        private IConfigSource source = null;

        private FilePropertyUtils() { }

        // thread safe
        public static FilePropertyUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new FilePropertyUtils();
                        }
                    }
                }
                return instance;
            }
        }

        public bool ReadFile(String fileName)
        {
            try
            {
                source = new IniConfigSource(fileName);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public String GetValueString(String group, String key)
        {
            if (source != null && !String.IsNullOrWhiteSpace(group) && !String.IsNullOrWhiteSpace(key))
            {
                return source.Configs[group.Trim()].Get(key.Trim());
            }
            return String.Empty;
        }

        public int GetValueInt(String group, String key)
        {
            if (source != null && !String.IsNullOrWhiteSpace(group) && !String.IsNullOrWhiteSpace(key))
            {
                return source.Configs[group.Trim()].GetInt(key.Trim());
            }
            return 0;
        }

        public String[] GetValueArrayString(String group)
        {
            if (source != null && !String.IsNullOrWhiteSpace(group))
            {
                return source.Configs[group].GetValues();
            }
            return new String[] { };
        }

    }
}
