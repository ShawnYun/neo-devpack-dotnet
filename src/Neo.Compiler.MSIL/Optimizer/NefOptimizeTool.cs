using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Compiler.Optimizer
{
    public static class NefOptimizeTool
    {
        /// <summary>
        /// Optimize
        /// </summary>
        /// <param name="script">Script</param>
        /// <returns>Optimized script</returns>
        public static byte[] Optimize(byte[] script)
        {
            return Optimize(script, new OptimizeParserType[]
            {
                OptimizeParserType.DELETE_DEAD_CODDE,
                OptimizeParserType.USE_SHORT_ADDRESS,
                OptimizeParserType.DELETE_USELESS_EQUAL
            }, new Dictionary<string, int>());
        }

        public static byte[] Optimize(byte[] script, Dictionary<string, int> funcAddrList)
        {
            return Optimize(script, new OptimizeParserType[]
            {
                OptimizeParserType.DELETE_DEAD_CODDE,
                OptimizeParserType.USE_SHORT_ADDRESS,
                OptimizeParserType.DELETE_USELESS_EQUAL
            }
            , funcAddrList);
        }

        public static byte[] Optimize(byte[] script, params OptimizeParserType[] parserTypes)
        {
            return Optimize(script, parserTypes, new Dictionary<string, int>());
        }

        /// <summary>
        /// Optimize
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parserTypes">Optmize parser, currently, there are four parsers:
        /// <para> DELETE_DEAD_CODDE -- delete dead code parser, default parser</para>
        /// <para> USE_SHORT_ADDRESS -- use short address parser. eg: JMP_L -> JMP, JMPIF_L -> JMPIF, default parser</para>
        /// <para> DELETE_NOP -- delete nop parser</para>
        /// <para> DELETE_USELESS_JMP -- delete useless jmp parser, eg: JPM 2</para>
        /// <para> DELETE_USELESS_EQUAL -- delete useless equal parser, eg: EQUAL 01 01 </para></param>
        /// <returns>Optimized script</returns>
        public static byte[] Optimize(byte[] script, OptimizeParserType[] parserTypes, Dictionary<string, int> funcAddrList)
        {
            var optimizer = new NefOptimizer();

            foreach (var parserType in parserTypes)
            {
                object[] objAttrs = parserType.GetType().GetField(parserType.ToString()).GetCustomAttributes(typeof(OptimizeParserAttribute), false);
                if (objAttrs is null || objAttrs.Length == 0) continue;

                var attribute = (OptimizeParserAttribute)objAttrs[0];
                var obj = Activator.CreateInstance(attribute.Type);
                if (obj is null) continue;
                IOptimizeParser parser = (IOptimizeParser)obj;
                optimizer.AddOptimizeParser(parser);
            }

            bool optimized;
            do
            {
                //step01 Load
                using (var ms = new MemoryStream(script))
                {
                    optimizer.LoadNef(ms, funcAddrList);
                }

                //step02 doOptimize
                optimizer.Optimize();

                //step03 link
                using (var ms = new MemoryStream())
                {
                    optimizer.LinkNef(ms);

                    optimizer.RebuildFuncAddrList(funcAddrList);

                    var bytes = ms.ToArray();

                    optimized = bytes.Length < script.Length;
                    if (optimized) { script = bytes; }
                }

                // Execute it while decrease the size
            }
            while (optimized);

            return script;
        }
    }
}
