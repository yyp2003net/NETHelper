using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NETHelper
{
      public class Unzip
      {
            class FileEntity
            {
                  public string ZipFullname { get; set; }
                  public string UnzipPath { get; set; }
            }

            public static void Run(string zipPath, string unzipPath)
            {
                  List<Task> tasks = new List<Task>();
                  FileInfo[] fls = new System.IO.DirectoryInfo(zipPath).GetFiles("*.zip");
                  foreach (var fl in fls)
                  {
                        var entity = new FileEntity
                        {
                              ZipFullname = fl.FullName,
                              UnzipPath = Path.Combine(unzipPath, Path.GetFileNameWithoutExtension(fl.Name))
                        };
                        tasks.Add(Task.Factory.StartNew(Unzip_Task, entity));
                  }
                  Task.WaitAll(tasks.ToArray());
            }

            static void Unzip_Task(object state)
            {
                  FileEntity entity = state as FileEntity;
                  if (!Directory.Exists(entity.UnzipPath))
                        Directory.CreateDirectory(entity.UnzipPath);

                  int size = 1024;
                  byte[] data = new byte[size];
                  string fileName = null;
                  FileStream fs = null;
                  ZipInputStream zs = null;
                  List<string> files = new List<string>();
                  try
                  {
                        fs = new FileStream(entity.ZipFullname, FileMode.Open, FileAccess.ReadWrite);
                        zs = new ZipInputStream(fs);
                        ZipEntry theEntry;
                        while ((theEntry = zs.GetNextEntry()) != null)
                        {
                              fileName = theEntry.Name;

                              //not window system file
                              if (fileName.IndexOf(":") > -1)
                              {
                                    fileName = fileName.Replace(":", "_");
                              }
                              //not window system path
                              if (fileName.IndexOf("/") > -1)
                              {
                                    fileName = fileName.Replace("/", "\\");
                              }
                              if (theEntry.IsDirectory)
                              {
                                    //maybe empty folder
                                    if (!Directory.Exists(Path.Combine(entity.UnzipPath, fileName)))
                                          Directory.CreateDirectory(Path.Combine(entity.UnzipPath, fileName));
                              }
                              else
                              {
                                    string fullname = Path.Combine(entity.UnzipPath, fileName);
                                    //not repeated create directory
                                    if (files.Count(f => f.ToLower().StartsWith(Path.GetDirectoryName(fullname).ToLower())) == 0)
                                    {
                                          Directory.CreateDirectory(Path.GetDirectoryName(fullname));
                                    }
                                    files.Add(fullname);
                                    List<byte> bytes = new List<byte>();
                                    size = zs.Read(data, 0, data.Length);
                                    while (size > 0)
                                    {
                                          bytes.AddRange(data);
                                          size = zs.Read(data, 0, data.Length);
                                    }
                                    var ws = new FileStream(fullname, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 1024, true);
                                    ws.BeginWrite(bytes.ToArray(), 0, bytes.Count, new AsyncCallback(EndWriteCallback), ws);
                              }
                        }
                  }
                  catch
                  {
                        throw;
                  }
                  finally
                  {
                        if (zs != null) zs.Close();
                        if (fs != null) fs.Close();
                  }
            }

            static void EndWriteCallback(IAsyncResult result)
            {
                  FileStream stream = (FileStream)result.AsyncState;
                  stream.EndWrite(result);
                  stream.Flush();
                  stream.Close();
            }
      }
}
