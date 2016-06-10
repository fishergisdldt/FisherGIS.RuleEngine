using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using RuleEngine.Parsers;

namespace RuleEngine
{
    public static class REngine
    {
        private static RConfig config;

        /// <summary>
        /// 构造函数
        /// </summary>
        static REngine()
        {
            LoadSettings();
        }

        /// <summary>
        /// 从appconfig中读出规则库加载规则配置
        /// </summary>
        private static void LoadSettings()
        {
            config = new RConfig();
            config.ThrowExceptionIfNotfoundRule = false;

            if (ConfigurationManager.AppSettings["REngine.ThrowExceptionIfNotfoundRule"] != null)
                config.ThrowExceptionIfNotfoundRule = Convert.ToString(ConfigurationManager.AppSettings["REngine.ThrowExceptionIfNotfoundRule"]) == "1";

            if (ConfigurationManager.AppSettings["REngine.RulefilesPath"] == null)
                throw new Exception("不存在REngine.RulefilesPath的key，在AppSetting中");

            config.RulefilesPath = ConfigurationManager.AppSettings["REngine.RulefilesPath"];
            config.RulefilesPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.RulefilesPath);

            if (!System.IO.Directory.Exists(config.RulefilesPath))
                throw new Exception("规则文件目录不存在");

            LoadRules();
        }
        /// <summary>
        /// 加载规则，将规则存储在config的规则字典中 key ：组+规则名称
        /// </summary>
        private static void LoadRules()
        {
            var files=System.IO.Directory.GetFiles(config.RulefilesPath, "*.rule");
            if (files == null || files.Length == 0)
                throw new Exception("rule文件不存在");

            StringBuilder text = new StringBuilder();
            files.ToList().ForEach(file => {
                var fileText = System.IO.File.ReadAllText(file);
                text.AppendLine(fileText);
            });
            //获取规则库规则组
            List<RuleRegion> regions=RegionParser.ParseRegions(text.ToString());
            if (regions == null || regions.Count == 0)
                throw new Exception("region不存在");

            regions.ForEach(region => {
                var rules = RuleParser.ParseRules(region.RegionContent);

                if (regions == null || regions.Count == 0)
                    throw new Exception(string.Format("region '{0}' 无法找到rule", region.RegionName));

                rules.ForEach(rule => {
                    rule.RegionName = region.RegionName;
                });

                rules.ForEach(rule =>{
                    var key=string.Format("{0}.{1}", rule.RegionName, rule.RuleName);
                    config.RuleDefinations[key] = rule;
                });
            });
        }
        /// <summary>
        /// 默认规则名
        /// </summary>
        private static string DefaultRuleName = "default";
        /// <summary>
        /// 通过规则组和规则标示找到规则
        /// </summary>
        /// <param name="ruleRegionId">规则组标示</param>
        /// <param name="ruleId">规则标示</param>
        /// <returns></returns>
        private static string LocateRuleContent(string ruleRegionId, string ruleId)
        {
            var key = string.Format("{0}.{1}", ruleRegionId, ruleId);
            if(config.RuleDefinations.ContainsKey(key))
                return config.RuleDefinations[key].RuleContent;

            if (config.ThrowExceptionIfNotfoundRule)
                throw new Exception("没有找到"+key);

            key = string.Format("{0}.{1}", ruleRegionId, DefaultRuleName);
            if (config.RuleDefinations.ContainsKey(key))
                return config.RuleDefinations[key].RuleContent;

            throw new Exception("没有找到" + key);
        }

        public static ParameterInfo CreateParameter(string name, object value)
        {
            ParameterInfo info = new ParameterInfo();
            info.ParameterName = name;
            info.ParameterValue = value;
            return info;
        }
        /// <summary>
        /// 通过规则组 规则标示获取该规则下的结果
        /// </summary>
        /// <param name="ruleRegionId"></param>
        /// <param name="ruleId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static float InvokeAsFloat(string ruleRegionId, string ruleId, params ParameterInfo[] parameters)
        {
            //获取规则编码
            var ruleCode = LocateRuleContent(ruleRegionId, ruleId);
            return Engine.GetResult_Float(ruleCode, parameters);
        }
        public static float InvokeAsFloat(string ruleRegionId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, DefaultRuleName);
            return Engine.GetResult_Float(ruleCode, parameters);
        }

        public static float InvokeAsInt(string ruleRegionId, string ruleId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, ruleId);
            return Engine.GetResult_Int(ruleCode, parameters);
        }
        public static float InvokeAsInt(string ruleRegionId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, DefaultRuleName);
            return Engine.GetResult_Int(ruleCode, parameters);
        }

        public static decimal InvokeAsDecimal(string ruleRegionId, string ruleId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, ruleId);
            return Engine.GetResult_Decimal(ruleCode, parameters);
        }
        public static decimal InvokeAsDecimal(string ruleRegionId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, DefaultRuleName);
            return Engine.GetResult_Decimal(ruleCode, parameters);
        }

        public static Single InvokeAsSingle(string ruleRegionId, string ruleId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, ruleId);
            return Engine.GetResult_Single(ruleCode, parameters);
        }
        public static Single InvokeAsSingle(string ruleRegionId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, DefaultRuleName);
            return Engine.GetResult_Single(ruleCode, parameters);
        }

        public static double InvokeAsDouble(string ruleRegionId, string ruleId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, ruleId);
            return Engine.GetResult_Double(ruleCode, parameters);
        }
        public static double InvokeAsDouble(string ruleRegionId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, DefaultRuleName);
            return Engine.GetResult_Double(ruleCode, parameters);
        }
        
        public static bool InvokeAsBool(string ruleRegionId, string ruleId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, ruleId);
            return Engine.GetResult_Bool(ruleCode, parameters);
        }
        public static bool InvokeAsBool(string ruleRegionId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, DefaultRuleName);
            return Engine.GetResult_Bool(ruleCode, parameters);
        }
        
        public static string InvokeAsString(string ruleRegionId, string ruleId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, ruleId);
            return Engine.GetResult_String(ruleCode, parameters);
        }
        public static string InvokeAsString(string ruleRegionId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, DefaultRuleName);
            return Engine.GetResult_String(ruleCode, parameters);
        }
        
        public static void InvokeAsVoid(string ruleRegionId, string ruleId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, ruleId);
            Engine.GetResult_Void(ruleCode, parameters);
        }
        public static void InvokeAsVoid(string ruleRegionId, params ParameterInfo[] parameters)
        {
            var ruleCode = LocateRuleContent(ruleRegionId, DefaultRuleName);
            Engine.GetResult_Void(ruleCode, parameters);
        }
    }
}
