using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLibrary.Models.Unit
{
    public enum AlertTypeClass
    {
        //硬體
        DeviceNotWork = 101, //櫃子壞掉
        DoorOpend = 102, //門沒關

        //DrugGrid
        QtyOverGrid = 201, //數量超過藥格上限
        QtyLowerGrid = 202, //低於安全庫存        
        NearExpiryAlert = 203, //近效期  

        //StockBill
        StockTakeError = 301, //盤點錯誤
        NoStockTaked = 302, //盤點錯誤
        InTransitExpired = 303, //在途庫存超時

        //PrscptBill
        HisConnectFailed = 401, //api連線
        AdHocProcNotOnList = 402,  //臨採-不在清單中
        FreeTrialNotEnough = 403,  //贈藥-量不足
        FreeTrialNotOnList = 404,  //贈藥-不在清單中
        HisReturnFalse = 405,  //贈藥
        
    }
}
