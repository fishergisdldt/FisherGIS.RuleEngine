using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngine.Parsers
{
    /// <summary>
    /// 命令转换器辅助类
    /// </summary>
    internal class ParseHelper
    {
        /// <summary>
        /// 去除空白行
        /// </summary>
        /// <param name="lines">命令行</param>
        /// <returns></returns>
        public static List<string> FilterEmptyRow(string[] lines)
        {
            List<string> filteredLines = new List<string>();
            foreach (string line in lines)
                if (line.Trim().Length > 0)
                    filteredLines.Add(line.Trim());
            return filteredLines;
        }
        /// <summary>
        /// 规则内容
        /// </summary>
        /// <param name="lines">从规则容器中取出当前规则</param>
        /// <param name="startRowIndex">规则起始行</param>
        /// <param name="endRowIndex">规则结束行</param>
        /// <returns></returns>
        public static string GetInnerRawContent(List<string> lines, int startRowIndex, int endRowIndex)
        {
            string str = string.Empty;
            int i = startRowIndex;
            while (i <= endRowIndex)
                str += lines[i++] + "\r\n";
            return str;
        }
    }
}
