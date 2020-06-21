using System.Collections.Generic;
using System.Net;

namespace Hybrid.Ai.Updater.BLL.Services
{
    public static class NetMaskConverter
    {
        public static List<IpV4NetmaskToRange> IpV4NetmaskRangeParse(List<string> netmaskList)
        {
            var resultList = new List<IpV4NetmaskToRange>();
            
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var netmask in netmaskList)
            {
                var ipNetwork = IPNetwork.Parse(netmask);

                var cidr = ipNetwork.Cidr;
                var minIp = ipNetwork.FirstUsable.ToString().Split('.');
                var maxIp = ipNetwork.LastUsable.ToString().Split('.');
                
                if (minIp.Length < 4 || maxIp.Length < 4) 
                    continue;
                
                resultList.Add(new IpV4NetmaskToRange
                {
                    NetMask = netmask,
                    MinFirstSegment = int.Parse(minIp[0]),
                    MinSecondSegment = int.Parse(minIp[1]),
                    MinThirdSegment = int.Parse(minIp[2]),
                    MinLastSegment = int.Parse(minIp[3]),
                    MaxFirstSegment = int.Parse(maxIp[0]),
                    MaxSecondSegment = int.Parse(maxIp[1]),
                    MaxThirdSegment = int.Parse(maxIp[2]),
                    MaxLastSegment = int.Parse(maxIp[3]),
                    Cidr = cidr
                });
            }

            return resultList;
        }
    }

    public class IpV4NetmaskToRange
    {
        public string NetMask { get; set; }
        
        public int Cidr { get; set; }
        
        public int MinFirstSegment { get; set; }
        
        public int MinSecondSegment { get; set; }
        
        public int MinThirdSegment { get; set; }
        
        public int MinLastSegment { get; set; }
        
        public int MaxFirstSegment { get; set; }
        
        public int MaxSecondSegment { get; set; }
        
        public int MaxThirdSegment { get; set; }
        
        public int MaxLastSegment { get; set; }
        
    }
}