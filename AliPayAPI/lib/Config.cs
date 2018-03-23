using System;
using System.Collections.Generic;
using System.Web;

namespace AliPayAPI
{
    /**
    * 	配置账号信息
    */
    public class AliPayConfig
    {
        //=======【基本信息设置】=====================================
        /* 支付宝信息配置
        * APPID：绑定支付的APPID（必须配置）
        * KEY：商户支付密钥，参考开户邮件设置（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */
        public const string ServerUrl = @"https://openapi.alipaydev.com/gateway.do";
        public const string APPID = @"2016081600258765";
        public const string APP_PRIVATE_KEY = @"MIIEowIBAAKCAQEAye3hyJqoJWUsVn7su0ndEhVuxlMHAX5RyCdOoLoGF4YKZ3NIS2mUVNoPNk/sl+Q0XiLIZCPrRsA3l4QS4mwOkyJJvfEY5/3DV2TCuPzFnqSahxd3tAg5VeKEBFyA9TrkEy4SGifYJFU2Smx+x400I8EzxGY8FJG6UC4k4AIZShfvDv8ufgK1R+Ub81JsTnx3ogtwpi3/7s7DKm+Mm1JCkt/gEijetmIflJMG6YviQU6XLTcM4xkrEvDttcQ9Z/TNxw3JAgVgUsTmT1dWjhqEuxtWpCvW+JfQu+sa1lZ+MD9dfyPqaG99X0DlQBsMKYxKqQAinhx+eXHGDrfUbGWBoQIDAQABAoIBAFvYkHb/KXYA477f8mtpuF0OVJlukGQ0gZxJjLD8i+LNPBQ70mlCt440tPCeP94ClXMv3Pf3gn9m1KJdF33XanWwBdyYOhzjRqOMmCkuB/EVq5fAq9i+WN3gru2Q6bMhOzYiIWe2MdCs1YnaeXvolQuiSBqP6cntUtI/etRNABW7qFDaQBPhXPHENYKyDTjRE93jpHZbvO0dutfgH1H2fn1Mi0Z41g0WtKF7fRFzd82+jAJAgVLx5tc8swLVfVhd7ZlRmoVQsc7zRHjTrnpb/ry9TemkoKmIb+ZIiEFsF+0GRwqSzfybuUpzufmmLX9opvNVSB8LQbMGKRzA18i7KkUCgYEA5Uvx/i7kZ4XjDzyDj4xton+KyHCUvv8VTaWbsmYrfzXZGyJ5EIQunBz9/oM5Dq+s1Mt9o4GWtqtrvlWWM6VST+zgvWvZr0mVL+gS61npHL1h9qekNUIJQN6fvxg8IqktVhSLrXwOyAnqKkiyBO4evgz/+GJfsnp17sRBYZho3+cCgYEA4XIHAYU8eZTTPhz3WNAfXgCO8cqbhLkMK6giFmRET4NNi/aZYK60Ja0EBzKpfGGul4d/njOumVKywm6ldiI1VppZHsx2VDGnBalMlVYYKkwQR67NvdiXHtk+5rA69pVIJmlrYMQjB7VCFMJrwgrGizFGg5jua6ulm4t5bIlmgTcCgYBEBNhfb6efsg7eKTRZs+2d47nWpdbqJZ87LmJWdIp4rQ+fRgWlyaBN/Se3hVO6sJBTe53kj/+WZpmKl7b70RHu1bUBW+nyXqCb2nsqR7yoIuHZmndSuSknjiLTPCwyl/7z5xpcN03nN1G4g2ITplOGSzLvircaqcssLhm7CswphwKBgDnTb6yaKjrdS5nBAEjNiV4pMoEegOl4NYD1LVkk+siSW0+tPwYniZmoWUInYoW+4HOJk9hWVVCKf8OTceltONUv3fAiba+G1NqE5FnhrW0b+YkJc0hgx9Jn0tSFG3qoK8t+esZlSL7vZTXB8LXi4a5OQ1H55h3D90SAb/LBA4PdAoGBALUWv7YJZQ3NWo3T1/PpqvdrjJA3/SCZpO6QSrqHuoWDPCyADKaMJyBJCVi3/fe+UHYp5Zt0z9TeM3nJte0meHFoBGx4AZpwZ0hKC0J3DlUT4YCH61UEl6oZVmew6dM+TBCdkSO9RaLrpymH2w8wGWqpH30ilEU0v1kUWRn0JV4I";
        public const string ALIPAY_PUBLIC_KEY = @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvStlrbYUILe6QVQNpZvQm2AIrLad92HCRawDrQT6lVSyssKE4jYep10vJeyOG9U2dQLVD/0ZANnIzeeER2RrFmOIEOcwCucRQGtgI7j/SCGlo2YT9MIUWWngx913cDtqvvE7DiMIOY9miKdjwNViSjVkVrSfv4oQxQ07ZKkNYGvwxglgtGnOhoBMTRmeF3qqKu+SY5Atip3M5VAaoLs6Rq6dlFaKyeVlhxaksHQvsfDNAsMA1uPZbGimg/sUhApJZkQ4QSjFz9XMChyCzxDMvVZYXWtaxaRqCOxZ6FYsL4F7Hdyw/51hLQIOcasABJ7jSCEUhRuE1/z4/20g5OQQSQIDAQAB";
        public const string CHARSET = @"utf-8";
        public const string SIGNTYPE = @"RSA2";

        public const string NOTIFY_URL = "http://39.104.66.176/Views/AliPay/ResultNotifyPage.aspx";

        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public const int LOG_LEVENL = 3;
    }
}