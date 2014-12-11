﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalEditor.IO;
using UniversalEditor.ObjectModels.Icarus;

namespace UniversalEditor.DataFormats.Icarus
{
    public class IcarusTextDataFormat : DataFormat
    {
        private static DataFormatReference _dfr = null;
        protected override DataFormatReference MakeReferenceInternal()
        {
            if (_dfr == null)
            {
                _dfr = base.MakeReferenceInternal();
                _dfr.Capabilities.Add(typeof(IcarusScriptObjectModel), DataFormatCapabilities.All);
            }
            return _dfr;
        }

        protected override void LoadInternal(ref ObjectModel objectModel)
        {
            IcarusScriptObjectModel script = (objectModel as IcarusScriptObjectModel);
            if (script == null) return;

            
        }

        protected override void SaveInternal(ObjectModel objectModel)
        {
            IcarusScriptObjectModel script = (objectModel as IcarusScriptObjectModel);
            if (script == null) return;

            Writer tw = base.Accessor.Writer;
            tw.WriteLine("//Generated by BehavEd");

            foreach (IcarusCommand command in script.Commands)
            {
                WriteCommand(tw, command);
            }
        }

        private void WriteCommand(Writer tw, IcarusCommand command, int indentLength = 0)
        {
            string indent = new string(' ', indentLength * 4);
            /*
            if (command is IcarusCommandAffect)
            {
                IcarusCommandAffect cmd = (command as IcarusCommandAffect);
                tw.Write(indent);
                tw.WriteLine("affect ( \"" + cmd.Target + "\", " + cmd.AffectType.ToString() + " )");
                
                tw.Write(indent);
                tw.WriteLine("{");

                foreach (IcarusCommand command2 in cmd.Commands)
                {
                    WriteCommand(tw, command2, indentLength + 1);
                }

                tw.WriteLine();
                tw.Write(indent);
                tw.WriteLine("}");

            }
            else if (command is IcarusCommandSet)
            {
                IcarusCommandSet cmd = (command as IcarusCommandSet);
                tw.Write(indent);
                tw.WriteLine("set ( " + cmd.Property.ToString() + ", " + cmd.Value.ToString() + " );");
            }
            else if (command is IcarusCommandUse)
            {
                IcarusCommandUse cmd = (command as IcarusCommandUse);
                tw.Write(indent);
                tw.WriteLine("use ( " + cmd.Target.ToString() + " );");
            }
            else if (command is IcarusCommandWait)
            {
                IcarusCommandWait cmd = (command as IcarusCommandWait);
                tw.Write(indent);
                tw.WriteLine("wait ( " + cmd.Target.ToString() + " );");
            }
            */
            tw.Flush();
        }
    }
}
