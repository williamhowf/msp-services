namespace Com.GGIT.Enumeration
{
    public enum DatetimeFormatEnum
    {
        /// <summary>
        /// i.e. : 23052019
        /// </summary>
        [EnumValue("ddMMyyyy")]
        [EnumDescription("ddMMyyyy")]
        ddMMyyyy,
        /// <summary>
        /// i.e. : 23.05.2019
        /// </summary>
        [EnumValue("dd.MM.yyyy")]
        [EnumDescription("dd.MM.yyyy")]
        ddMMyyyyWithDot,
        /// <summary>
        /// i.e. : 23-05-2019
        /// </summary>
        [EnumValue("dd-MM-yyyy")]
        [EnumDescription("dd-MM-yyyy")]
        ddMMyyyyWithHyphen,
        /// <summary>
        /// i.e. : 23/05/2019
        /// </summary>
        [EnumValue("dd/MM/yyyy")]
        [EnumDescription("dd/MM/yyyy")]
        ddMMyyyyWithSlash,
        /// <summary>
        /// i.e. : 20190523
        /// </summary>
        [EnumValue("yyyyMMdd")]
        [EnumDescription("yyyyMMdd")]
        yyyyMMdd,
        /// <summary>
        /// i.e. : 2019.05.23
        /// </summary>
        [EnumValue("yyyy.MM.dd")]
        [EnumDescription("yyyy.MM.dd")]
        yyyyMMddWithDot,
        /// <summary>
        /// i.e. : 2019-05-23
        /// </summary>
        [EnumValue("yyyy-MM-dd")]
        [EnumDescription("yyyy-MM-dd")]
        yyyyMMddWithHyphen,
        /// <summary>
        /// i.e. : 2019/05/23
        /// </summary>
        [EnumValue("yyyy/MM/dd")]
        [EnumDescription("yyyy/MM/dd")]
        yyyyMMddWithSlash,
        /// <summary>
        /// i.e. : 2019-05-23 15:01
        /// </summary>
        [EnumValue("yyyy-MM-dd HH:mm")]
        [EnumDescription("yyyy-MM-dd HH:mm")]
        yyyyMMddHHmmWithHyphen,
        /// <summary>
        /// i.e. : 2019-05-23T15:01
        /// </summary>
        [EnumValue("yyyy-MM-ddTHH:mm")]
        [EnumDescription("yyyy-MM-ddTHH:mm")]
        yyyyMMddTHHmmWithHyphen,
        /// <summary>
        /// i.e. : 2019-05-23 15:02:40
        /// </summary>
        [EnumValue("yyyy-MM-dd HH:mm:ss")]
        [EnumDescription("yyyy-MM-dd HH:mm:ss")]
        yyyyMMddHHmmssWithHyphen,
        /// <summary>
        /// i.e. : 2019-05-23 15:02:40.642
        /// </summary>
        [EnumValue("yyyy-MM-dd HH:mm:ss.fff")]
        [EnumDescription("yyyy-MM-dd HH:mm:ss.fff")]
        yyyyMMddHHmmssfffWithHyphen,
        /// <summary>
        /// i.e. : 201905231435
        /// </summary>
        [EnumValue("yyyyMMddHHmm")]
        [EnumDescription("yyyyMMddHHmm")]
        yyyyMMddHHmm,
        /// <summary>
        /// i.e. : 20190523143522
        /// </summary>
        [EnumValue("yyyyMMddHHmmss")]
        [EnumDescription("yyyyMMddHHmmss")]
        yyyyMMddHHmmss,
        /// <summary>
        /// i.e. : 20190523143622973
        /// </summary>
        [EnumValue("yyyyMMddHHmmssfff")]
        [EnumDescription("yyyyMMddHHmmssfff")]
        yyyyMMddHHmmssfff,
        /// <summary>
        /// i.e. : 2019-05-23T07:04:16.2807569Z
        /// </summary>
        [EnumValue("yyyy-MM-ddTHH:mm:ss.fffffffZ")]
        [EnumDescription("yyyy-MM-ddTHH:mm:ss.fffffffZ")]
        ISO8601,
        /// <summary>
        /// i.e. : 2019-05-23T07:04Z
        /// </summary>
        [EnumValue("yyyy-MM-ddTHH:mmZ")]
        [EnumDescription("yyyy-MM-ddTHH:mmZ")]
        ISO8601_Minutes,
        /// <summary>
        /// i.e. : 2019-05-23T14:38:02Z
        /// </summary>
        [EnumValue("yyyy-MM-ddTHH:mm:ssZ")]
        [EnumDescription("yyyy-MM-ddTHH:mm:ssZ")]
        ISO8601_Seconds,
        /// <summary>
        /// i.e. : 2019-05-23T14:38:05.572Z
        /// </summary>
        [EnumValue("yyyy-MM-ddTHH:mm:ss.fffZ")]
        [EnumDescription("yyyy-MM-ddTHH:mm:ss.fffZ")]
        ISO8601_Milliseconds,
        /// <summary>
        /// i.e. : 23May2019
        /// </summary>
        [EnumValue("ddMMMyyyy")]
        [EnumDescription("ddMMMyyyy")]
        ddMMMyyyy,
        /// <summary>
        /// i.e. : 210October2019
        /// </summary>
        [EnumValue("ddMMMMyyyy")]
        [EnumDescription("ddMMMMyyyy")]
        ddMMMMyyyy,
        /// <summary>
        /// i.e. : 23 May 2019
        /// </summary>
        [EnumValue("dd MMM yyyy")]
        [EnumDescription("dd MMM yyyy")]
        ddMMMyyyyWithSpace,
        /// <summary>
        /// i.e. : 23 September 2019
        /// </summary>
        [EnumValue("dd MMMM yyyy")]
        [EnumDescription("dd MMMM yyyy")]
        ddMMMMyyyyWithSpace,
        /// <summary>
        /// i.e. : 09:30 AM/PM
        /// </summary>
        [EnumValue("hh:mm tt")]
        [EnumDescription("hh:mm tt")]
        hhmmtt,
        /// <summary>
        /// i.e. : 17:36
        /// </summary>
        [EnumValue("HH:mm")]
        [EnumDescription("hh:mm tt")]
        HHmm,
    }
}
