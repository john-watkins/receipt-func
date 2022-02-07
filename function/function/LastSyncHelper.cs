using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenFaaS
{
    internal class LastSyncHelper
    {
        private static readonly string LAST_SYNC_FILE = "last_sync.txt";
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        public static DateTime GetLastSync()
        {
            _readWriteLock.EnterReadLock();
            try
            {
                string sdt = File.ReadAllText(LAST_SYNC_FILE);
                if (DateTime.TryParse(sdt, out DateTime lastSync))
                {                    
                    return lastSync;
                }
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }        
            return DateTime.Now.AddDays(-1);
        }

        public static void WriteLastSync(DateTime dt)
        {
            _readWriteLock.EnterWriteLock();
            try
            {
                File.WriteAllText(LAST_SYNC_FILE, dt.ToString("yyyy-MM-dd HH:mm:ss"));                
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }                        
        }
    }
}
