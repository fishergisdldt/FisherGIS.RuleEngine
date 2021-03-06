﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noesis.Javascript;

namespace RuleEngine
{
    /// <summary>
    /// 规则引擎
    /// </summary>
    internal static class Engine
    {
        private static JavascriptContext context = new JavascriptContext();
        //请求计算次数
        private static int usedCount = 0;
        //最大规则转换次数，多于此次数将重新新鲜js转换
        private const int reGenerateJSContextPeek = 1500;
        private static IJavascriptGenerator jsGenerator = new DefaultJavascriptGenerator();

        public static int GetResult_Int(string externalCode, params ParameterInfo[] parameters)
        {
            object o = GetResult(externalCode, parameters);

            if (o == null)
                throw new Exception("没有返回值");

            try
            {
                return (int)o;
            }
            catch
            {
                float f = float.Parse(o.ToString());
                return (int)f;
            }
        }
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="externalCode"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static float GetResult_Float(string externalCode, params ParameterInfo[] parameters)
        {
            object o = GetResult(externalCode, parameters);
            return float.Parse(o.ToString());
        }
        public static decimal GetResult_Decimal(string externalCode, params ParameterInfo[] parameters)
        {
            object o = GetResult(externalCode, parameters);
            return decimal.Parse(o.ToString());
        }
        public static Single GetResult_Single(string externalCode, params ParameterInfo[] parameters)
        {
            object o = GetResult(externalCode, parameters);
            return Single.Parse(o.ToString());
        }
        public static double GetResult_Double(string externalCode, params ParameterInfo[] parameters)
        {
            object o = GetResult(externalCode, parameters);
            return double.Parse(o.ToString());
        }
        public static bool GetResult_Bool(string externalCode, params ParameterInfo[] parameters)
        {
            object o = GetResult(externalCode, parameters);
            return bool.Parse(o.ToString());
        }
        public static string GetResult_String(string externalCode, params ParameterInfo[] parameters)
        {
            object o = GetResult(externalCode, parameters);
            return (string)o;
        }
        public static void GetResult_Void(string externalCode, params ParameterInfo[] parameters)
        {
            GetResult(externalCode, parameters);
        }
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <param name="externalCode"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object GetResult(string externalCode, params ParameterInfo[] parameters)
        {
            ResourceEnsurace();

            foreach (var item in parameters)
                context.SetParameter(item.ParameterName, item.ParameterValue);

            string realJs = jsGenerator.GenerateJS(externalCode);

            context.Run(realJs);

            object o = context.GetParameter("_RESULT_RETURN");
            return o;
        }

        private static void ResourceEnsurace()
        {
            usedCount++;

            if (usedCount >= reGenerateJSContextPeek)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }

                context = new JavascriptContext();

                usedCount = 1;
            }
        }
    }
}