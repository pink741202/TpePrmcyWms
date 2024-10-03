using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TpePrmcyWms.Models.DOM;

namespace TpePrmcyAlertBatch.Models.DOM
{
    public static class Extension
    {
        public static AlertNotification create(this AlertNotification alert, 
            int alertType, string sourceTable, int sourceFid, string sendTo, string alertTitle, string alertContent
            ) {
            alert.AlertType = alertType;
            alert.SourceTable = sourceTable;
            alert.SourceFid = sourceFid;
            alert.AlertTitle = alertTitle;
            alert.AlertContent = alertContent;
            alert.SendTo = sendTo;
            alert.adddate = DateTime.Now;
            return alert;
        }
    }
}
