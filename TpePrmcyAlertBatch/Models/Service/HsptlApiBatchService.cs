using System.Configuration;
using TpePrmcyAlertBatch.Models.DOM;
using TpePrmcyAlertBatch.Models.Unit;
using TpePrmcyWms.Models.DOM;

namespace TpePrmcyAlertBatch.Models.Service
{
    internal class HsptlApiBatchService
    {
        DBcPharmacy _db = new DBcPharmacy();
        public HsptlApiBatchService()
        {

        }

        public void saveBalanceDiffFile(DateTime thetime)
        {
            try
            {
                string diskPath = ConfigurationManager.AppSettings["BalanceDiffFile"]; //舊的 \\\\tpechnas03\\filetrans$\\PHA\\PHAS004

                string[] filePaths = Directory.GetFiles(diskPath, "*.txt",
                                     SearchOption.TopDirectoryOnly);

                foreach (var filePath in filePaths)
                {
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            var subFilePath = filePath.Split('\\');
                            var fileName = subFilePath[subFilePath.Length - 1];
                            var branchCode = fileName.Substring(0, 1);
                            var fileTimeStr = fileName.Substring(1, 14);
                            var fileTime = new DateTime(Convert.ToInt32(fileTimeStr.Substring(0, 4)),
                                                        Convert.ToInt32(fileTimeStr.Substring(4, 2)),
                                                        Convert.ToInt32(fileTimeStr.Substring(6, 2)),
                                                        Convert.ToInt32(fileTimeStr.Substring(8, 2)),
                                                        Convert.ToInt32(fileTimeStr.Substring(10, 2)),
                                                        Convert.ToInt32(fileTimeStr.Substring(12, 2)));

                            using (StreamReader sr = File.OpenText(filePath))
                            {
                                string dataStr = "";
                                while ((dataStr = sr.ReadLine()) != null)
                                {
                                    string[] data = dataStr.Split(',');
                                    if (data.Length == 13)
                                    {
                                        var his = new BalanceDiffFile()
                                        {
                                            BranchCode = branchCode,
                                            CreateTime = DateTime.Now,
                                            FileName = fileName,
                                            FileTime = fileTime,
                                            OPDER_SW = data[0],
                                            PAT_NO = data[1],
                                            ODR_CODE = data[2],
                                            TOTAL_QTY = data[3],
                                            PRICE_UNIT = data[4],
                                            REG_DATE = data[5],
                                            DELETE_DATE = data[6],
                                            DRUG_NO = data[7],
                                            ODR_SEQ = data[8],
                                            D_NUMBER = data[9],
                                            MED_TYPE = data[10],
                                            HR_SEQ = data[11],
                                            PAT_SEQ = data[12]
                                        };

                                        _db.BalanceDiffFile.Add(his);
                                    }
                                }
                            }

                            File.Delete(filePath);
                        }
                    }
                    catch (Exception ex) { qwFunc.logshow($"執行:結存量差異分析統計(檔:{filePath}) 出錯:{ex.Message}"); }
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                qwFunc.logshow($"執行:結存量差異分析統計 出錯:{ex.Message}");
            }

        }

    }
}
