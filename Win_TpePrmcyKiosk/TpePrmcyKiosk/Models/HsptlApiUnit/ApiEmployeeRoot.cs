using System;
using Newtonsoft.Json;

namespace TpePrmcyKiosk.Models.HsptlApiUnit
{
    public class ApiEmployee
    {
        public string emp_no { get; set; }
        public string emp_name { get; set; }
        public string emp_dep_code { get; set; }
        public string emp_dept_name { get; set; }
        public string emp_pos_code { get; set; }
        public string emp_pos_name { get; set; }
        public string emp_location { get; set; }
        public string emp_cost_center { get; set; }
        public string upper_dep_code { get; set; }
        public string upper_dep_name { get; set; }
        public string emp_birth { get; set; }
        [JsonProperty("CardNo")]
        public string CardNo { get; set; }
        public string emp_ename { get; set; }
        public string mobile_tel { get; set; }
        public string mobile_code { get; set; }
    }
    public class ApiEmployeeRoot
    {
        public string status { get; set; }
        public List<ApiEmployee> rows { get; set; }
        public string msg { get; set; }
    }
}
