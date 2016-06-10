using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngine.Parsers
{
    /// <summary>
    /// 规则转换器
    /// </summary>
    internal class RegionParser
    {
        /// <summary>
        /// 转换规则
        /// </summary>
        /// <param name="text">所有规则函数</param>
        /// <returns></returns>
        public static List<RuleRegion> ParseRegions(string text)
        {
            List<RuleRegion> regions = new List<RuleRegion>();

            if (text == null)
                return regions;
            if (text.Trim().Length <= 0)
                return regions;
            //换行，回车作为函数行分隔
            string[] lines = text.Split("\r\n".ToCharArray());
            if (lines == null)
                return regions;
            if (lines.Length == 0)
                return regions;
            //去除空白行
            List<string> filteredLines = ParseHelper.FilterEmptyRow(lines);

            RuleRegion region = null;
            int regionIndex = -1;
            for (int i = 0; i < filteredLines.Count; i++)
            {
                string line = filteredLines[i];

                if (line.Trim().Length > 7 && line.Substring(0, 7) == "#region")
                {
                    regionIndex = i;

                    region = new RuleRegion();
                    region.RegionName = line.Substring(8).Trim('"');
                }
                else if (line.Trim().Length >= 10 && line.Substring(0, 10) == "#endregion")
                {
                    if(region==null)
                        throw new Exception("Region Parse error");

                    if (region != null)
                    {
                        region.RegionContent = ParseHelper.GetInnerRawContent(filteredLines, regionIndex + 1, i - 1);
                        regions.Add(region);

                        regionIndex = -1;
                    }   
                }
            }

            if (regions.Count == 0)
                throw new Exception("Region cannot be empty");

            return regions;
        }
    }
}
