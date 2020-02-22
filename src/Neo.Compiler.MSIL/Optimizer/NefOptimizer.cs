using Neo.VM;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler.Optimizer
{
    public class NefOptimizer
    {
        /// <summary>
        /// Instructions
        /// </summary>
        private List<INefItem> Items;
        private List<IOptimizeParser> OptimizeFunctions = new List<IOptimizeParser>();
        public NefOptimizer(byte[] script = null)
        {
            if (script != null)
            {
                using (var ms = new System.IO.MemoryStream(script))
                {
                    LoadNef(ms);
                }
            }
        }
        public void AddOptimizeParser(IOptimizeParser function)
        {
            if (OptimizeFunctions == null)
                OptimizeFunctions = new List<IOptimizeParser>();
            OptimizeFunctions.Add(function);
        }
        public void Optimize()
        {
            if (OptimizeFunctions == null || OptimizeFunctions.Count == 0)
                return;
            foreach (var func in OptimizeFunctions)
            {
                func.Parse(this.Items);
            }
        }
        //Step01 Load
        public void LoadNef(System.IO.Stream stream)
        {


            //read all Instruction to listInst
            List<NefInstruction> listInst = new List<NefInstruction>();
            //read all Address to listAddr
            Dictionary<int, NefLabel> mapLabel = new Dictionary<int, NefLabel>();
            int labelindex = 1;


            NefInstruction _inst = null;
            do
            {
                _inst = NefInstruction.ReadFrom(stream);
                if (_inst != null)
                {
                    listInst.Add(_inst);
                    if (_inst.AddressCountInData > 0)
                    {
                        for (var i = 0; i < _inst.AddressCountInData; i++)
                        {
                            var addr = _inst.GetAddressInData(i) + _inst.Offset;
                            if (!mapLabel.ContainsKey(addr))
                            {
                                var labelname = "label" + labelindex.ToString("D06");
                                labelindex++;
                                var label = new NefLabel(labelname, addr);
                                mapLabel.Add(addr, label);
                            }

                            _inst.labels[i] = mapLabel[addr].Name;
                        }
                    }
                }
            } while (_inst != null);

            //Add Labels
            if (Items == null)
                Items = new List<INefItem>();
            else
                Items.Clear();
            foreach (var instruction in listInst)
            {
                var curOffset = instruction.Offset;
                if (mapLabel.ContainsKey(curOffset))
                {
                    Items.Add(mapLabel[curOffset]);
                }
                Items.Add(instruction);
            }
        }

        //Step02

        //Step03 Link
        public void LinkNef(System.IO.Stream stream)
        {
            Dictionary<string, uint> mapLabel2Addr = new Dictionary<string, uint>();
            //Recalc Address
            //collection Labels and Resort Offset
            uint Offset = 0;
            foreach (var _item in this.Items)
            {
                NefInstruction _inst = _item as NefInstruction;
                NefLabel _label = _item as NefLabel;
                if (_inst != null)
                {
                    _inst.SetOffset((int)Offset);
                    Offset += _inst.CalcTotalSize;
                }
                else if (_label != null)
                {
                    _label.SetOffset((int)Offset);
                    mapLabel2Addr[_label.Name] = Offset;
                }
            }


            //ChangeAddress
            foreach (var _item in this.Items)
            {
                NefInstruction _inst = _item as NefInstruction;
                if (_inst != null)
                {
                    for (var i = 0; i < _inst.AddressCountInData; i++)
                    {
                        var label = _inst.labels[i];
                        var addr = (int)mapLabel2Addr[label] - _inst.Offset;
                        _inst.SetAddressInData(i, addr);
                    }
                }
            }
            //and Link
            foreach (var _inst in this.Items)
            {
                if (_inst is NefInstruction)
                    NefInstruction.WriteTo(_inst as NefInstruction, stream);
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="script">Script</param>

        //Step1.LoadInstructWithLabel
        //Step2.DoOptimze(blabla);





        /// <summary>
        /// Recalculate long jumps
        /// </summary>
        public void RecalculeLongJumps()
        {
            //for (int x = 0; x < Instructions.Count;)
            //{
            //    var ins = Instructions[x++];

            //    if (ins.OpCode == OpCode.PUSHA ||
            //        !(ins.Jump is JumpI32 jmp)) continue;

            //    if (jmp.Offset > sbyte.MaxValue) continue;
            //    if (jmp.Offset < sbyte.MinValue) continue;

            //    // Remove _L

            //    ins.OpCode = (OpCode)(((byte)ins.OpCode) - 1);
            //    ins.Operand = new byte[] { ins.Operand[0] };
            //    ins.Size -= 3;
            //    ins.Jump = new JumpI8(ins);

            //    // Recalculate offsets

            //    for (int index = x; index < Instructions.Count; index++)
            //    {
            //        Instructions[index].Offset -= 3;
            //    }

            //    // Recalculate jumps
            //    RecalculateJumpsForLongJump(Instructions, ins.Offset, 3, ins.Jump.Offset);
            //}
        }

    }
}
