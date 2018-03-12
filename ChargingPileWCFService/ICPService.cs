using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ChargingPileWCFService
{
    [ServiceContract]
    public interface ICPService
    {
        [OperationContract]
        bool UploadBatteryTestingReport(BatteryTestringReport report);
    }


    [DataContract]
    public class BatteryTestringReport
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public string CustomerId { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [DataMember]
        public string VehicleNo { get; set; }

        /// <summary>
        /// 检查日期
        /// </summary>
        [DataMember]
        public DateTime CheckDate { get; set; }

        /// <summary>
        /// 检测结果
        /// </summary>
        [DataMember]
        public string Results { get; set; }

        [DataMember]
        public double Capacity { get; set; }

        [DataMember]
        public double DCR { get; set; }

        [DataMember]
        public string CAN { get; set; }

        [DataMember]
        public string IR { get; set; }

        [DataMember]
        public double Temp { get; set; }

        [DataMember]
        public double TempUp { get; set; }

        [DataMember]
        public double VDiff { get; set; }

        [DataMember]
        public double ADiff { get; set; }

        [DataMember]
        public string Proposal { get; set; }
    }
}
