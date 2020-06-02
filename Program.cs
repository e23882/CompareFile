using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CheckFile
{
    class Program
    {
        static void Main(string[] args)
        {
#if (DEBUG)
            var sourcePath = @"D:\Backup\LeoYang\Desktop\ApServicesError";
            var targetPath = @"D:\Backup\LeoYang\Desktop\ApServices";
#else
            Console.WriteLine("來源路徑:");
            var sourcePath = Console.ReadLine();
            Console.WriteLine("目標路徑:");
            var targetPath = Console.ReadLine();
            Console.WriteLine("確認檔案數量...");
#endif

            Stopwatch st = new Stopwatch();
            st.Start();
            //Recursive rr = new Recursive(@"D:\Backup\LeoYang\Desktop\Source");
            Recursive rr = new Recursive(sourcePath);
            Console.WriteLine($"所有檔案共 : {rr.GetFileList().Count}");
            Console.WriteLine("開始比對檔案");
            //rr.GetFile(@"D:\Backup\LeoYang\Desktop\Target");
            rr.GetFile(targetPath);
            st.Stop();
            Console.WriteLine($"執行完畢，共花費{st.ElapsedMilliseconds / 1000} 秒");
            Console.Read();
        }
    }
    public class Recursive
    {
        #region Declarations
        List<string> data = new List<string>();
        private string _Source = string.Empty;
        private string _Target = string.Empty;
        private int _ThreadID;
        #endregion

        #region Property

        #endregion

        #region Memberfunction
        public Recursive(string topPath)
        {
            _Source = topPath;
            checkNMove(topPath);
        }

        /// <summary>
        /// 確認檔案並移動
        /// </summary>
        /// <param name="path"></param>
        private void checkNMove(string path)
        {

            foreach (var file in Directory.GetFiles(path))
            {
                if (!data.Contains(file))
                    data.Add(file);
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                foreach (var file in Directory.GetFiles(dir))
                {
                    if (!data.Contains(file))
                        data.Add(file);
                }
                checkNMove(dir);
            }
        }

        /// <summary>
        /// 取得檔案清單
        /// </summary>
        /// <returns></returns>
        public List<string> GetFileList()
        {
            return data;
        }

        /// <summary>
        /// 取得檔案
        /// </summary>
        /// <param name="target"></param>
        public void GetFile(string target)
        {
            _Target = target;
            if (data.Count > 0)
            {
                int tempCount = 0;
                string carType = string.Empty;
                foreach (var item in data)
                {
                    var subPath = item.Replace(_Source, "");
                    //檢查對應路徑是否存在
                    var fullPath = _Target + subPath;
                    FileStream fsSource = File.OpenRead(fullPath);
                    FileStream fsTarget = File.OpenRead(item);
                    //比對檔案長度
                    if(fsSource.Length != fsTarget.Length)
                    {
                        FileInfo fioSource = new FileInfo(fullPath);
                        FileInfo fioTarget = new FileInfo(item);
                        Console.WriteLine($"{subPath.Substring(1)}檔案大小有差異\r\n\t來源檔案 : {fsSource.Length} {fioSource.LastWriteTime}\r\n\t目標檔案 : {fsTarget.Length} {fioTarget.LastWriteTime}\r\n");
                        fsSource.Dispose();
                        fsTarget.Dispose();
                    }
                    else//比對檔案內容
                    {
                        var FileByteSource = File.ReadAllBytes(fullPath);
                        var FileByteTarget = File.ReadAllBytes(item);
                        var dataCount = 0;
                        do
                        {
                            if (FileByteSource[dataCount] != FileByteTarget[dataCount])
                            {
                                Console.WriteLine($"{subPath.Substring(1)}檔案內容有差異，檔案Bytes[] 第{dataCount}位置不同\r\n");
                                break;
                            }
                            dataCount++;
                        }
                        while (dataCount < FileByteSource.Length);
                    }
                    tempCount++;
                }
            }
        }
        #endregion
    }
}
