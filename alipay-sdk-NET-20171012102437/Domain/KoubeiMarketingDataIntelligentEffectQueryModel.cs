using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingDataIntelligentEffectQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingDataIntelligentEffectQueryModel : AopObject
    {
        /// <summary>
        /// 操作人上下文信息
        /// </summary>
        [XmlElement("operator_context")]
        public PromoOperatorInfo OperatorContext { get; set; }

        /// <summary>
        /// 智能营销活动的详情，用于咨询的元数据
        /// </summary>
        [XmlElement("promo")]
        public IntelligentPromo Promo { get; set; }
    }
}