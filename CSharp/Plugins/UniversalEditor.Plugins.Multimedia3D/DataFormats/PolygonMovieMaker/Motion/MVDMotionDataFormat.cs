﻿using System;
using UniversalEditor.Plugins.Multimedia3D.ObjectModels.Motion;

namespace UniversalEditor.Plugins.Multimedia3D.DataFormats.PolygonMovieMaker.Motion
{
    /// <summary>
    /// Motion data loads from and saves to MikuMikuMoving "Motion Vector Data" (MVD) file format.
    /// </summary>
    public class MVDMotionDataFormat : DataFormat
    {
        public override DataFormatReference MakeReference()
        {
            DataFormatReference dfr = base.MakeReference();
            dfr.Capabilities.Add(typeof(MotionObjectModel), DataFormatCapabilities.All);
            dfr.Filters.Add("Motion vector data file", new byte?[][] { new byte?[] { (byte)'M', (byte)'o', (byte)'t', (byte)'i', (byte)'o', (byte)'n', (byte)' ', (byte)'V', (byte)'e', (byte)'c', (byte)'t', (byte)'o', (byte)'r', (byte)' ', (byte)'D', (byte)'a', (byte)'t', (byte)'a', (byte)' ', (byte)'f', (byte)'i', (byte)'l', (byte)'e' } }, new string[] { "*.mvd" });
            return dfr;
        }
        protected override void LoadInternal(ref ObjectModel objectModel)
        {
            MotionObjectModel motion = (objectModel as MotionObjectModel);
            if (motion == null) return;

            System.Text.Encoding encoding = System.Text.Encoding.UTF8;

            IO.BinaryReader br = base.Stream.BinaryReader;
            string signature = br.ReadFixedLengthString(30);
            signature = signature.Substring(0, signature.IndexOf('\0'));
            if (signature != "Motion Vector Data file") throw new DataFormatException(Localization.StringTable.ErrorDataFormatInvalid);

            float version = br.ReadSingle();
            
            byte encodingType = br.ReadByte();
            if (encodingType == 0)
            {
                encoding = System.Text.Encoding.Unicode;
            }

            uint compatibleModelNameJLength = br.ReadUInt32();
            byte[] compatibleModelNameJBytes = br.ReadBytes(compatibleModelNameJLength);
            string compatibleModelNameJ = System.Text.Encoding.UTF8.GetString(compatibleModelNameJBytes);

            uint compatibleModelNameELength = br.ReadUInt32();
            byte[] compatibleModelNameEBytes = br.ReadBytes(compatibleModelNameELength);
            string compatibleModelNameE = encoding.GetString(compatibleModelNameEBytes);

            motion.CompatibleModelNames.Add(compatibleModelNameJ);
            motion.CompatibleModelNames.Add(compatibleModelNameE);

            ushort us000 = br.ReadUInt16();
            uint CRYPTO = br.ReadUInt32();

            byte[] unknown1 = br.ReadBytes(12);

            uint numBones = br.ReadUInt32();
            uint unknown2 = br.ReadUInt32();

            System.Collections.Generic.List<Internal.MVDBoneData> bones = new System.Collections.Generic.List<Internal.MVDBoneData>();
            for (uint i = 0; i < numBones; i++)
            {
                Internal.MVDBoneData mvd = new Internal.MVDBoneData();

                mvd.FrameIndex = br.ReadUInt32(); // idk, some index that always seems to be incremented...?
                
                uint boneNameLength = br.ReadUInt32();
                byte[] boneNameBytes = br.ReadBytes(boneNameLength);
                string boneNameInUTF8 = System.Text.Encoding.UTF8.GetString(boneNameBytes);

                mvd.BoneName = boneNameInUTF8;
                bones.Add(mvd);
            }

            System.Collections.Generic.List<UniversalEditor.Plugins.Multimedia3D.ObjectModels.Motion.Internal.RawMotionFrame> rawFrames = new System.Collections.Generic.List<UniversalEditor.Plugins.Multimedia3D.ObjectModels.Motion.Internal.RawMotionFrame>();

            bool panic = false; // HACK: delete this when the actual format is figured out!

            for (int i = 0; i < bones.Count; i++)
            {
                ushort us16 = br.ReadUInt16();              // 16   32
                uint uBoneIndex = br.ReadUInt32();          // 
                uint uFrameSize = br.ReadUInt32();          // 56   16
                uint uFrameCount = br.ReadUInt32();         // 
                
                uint extraDataLen = br.ReadUInt32();
                byte[] extraData = br.ReadBytes(extraDataLen);

                for (uint j = 0; j < uFrameCount; j++)
                {
                    UniversalEditor.Plugins.Multimedia3D.ObjectModels.Motion.Internal.RawMotionFrame rawFrame = new UniversalEditor.Plugins.Multimedia3D.ObjectModels.Motion.Internal.RawMotionFrame();
                    if (uFrameSize == 56)
                    {
                        // Total size of data for each frame in the bone: 56 bytes

                        rawFrame.BoneName = bones[(int)uBoneIndex].BoneName;
                        
                        uint u000 = br.ReadUInt32();

                        uint uFrameIndex = br.ReadUInt32();
                        rawFrame.Index = uFrameIndex;

                        uint u002 = br.ReadUInt32();

                        // POSITION
                        float posX = br.ReadSingle(); // pos Y
                        float posY = br.ReadSingle(); // pos Z
                        float posZ = br.ReadSingle(); // rot X
                        rawFrame.Position = new PositionVector3(posX, posY, posZ);

                        // ROTATION
                        float rotX = br.ReadSingle(); // rot Y
                        float rotY = br.ReadSingle();
                        float rotZ = br.ReadSingle();
                        float rotW = br.ReadSingle();
                        rawFrame.Rotation = new PositionVector4(rotX, rotY, rotZ, rotW);

                        float rotX1 = br.ReadSingle();
                        float rotY1 = br.ReadSingle();
                        float rotZ1 = br.ReadSingle();
                        float rotW1 = br.ReadSingle();
                    }
                    else if (uFrameSize == 16)
                    {
                        float rotX1 = br.ReadSingle();
                        float weight = br.ReadSingle();
                        uint rotZ1 = br.ReadUInt32();
                        uint rotW1 = br.ReadUInt32();

                    }
                    else
                    {
                        panic = true;
                        break;
                    }
                    rawFrames.Add(rawFrame);
                }
                if (panic) break;
            }

            uint index = 0;
            MotionFrame frame = new MotionFrame();
            foreach (UniversalEditor.Plugins.Multimedia3D.ObjectModels.Motion.Internal.RawMotionFrame rawFrame in rawFrames)
            {
                if (rawFrame.Index != index)
                {
                    frame.Index = index;
                    motion.Frames.Add(frame);

                    frame = new MotionFrame();
                    index = rawFrame.Index;
                }

                MotionBoneRepositionAction action = new MotionBoneRepositionAction();
                action.BoneName = rawFrame.BoneName;
                action.Position = rawFrame.Position;
                action.Rotation = rawFrame.Rotation;

				action.Interpolation.Rotation.X1 = rawFrame.Interpolation.Rotation.X1;
				action.Interpolation.Rotation.X2 = rawFrame.Interpolation.Rotation.X2;
				action.Interpolation.Rotation.Y1 = rawFrame.Interpolation.Rotation.Y1;
				action.Interpolation.Rotation.Y2 = rawFrame.Interpolation.Rotation.Y2;
				action.Interpolation.XAxis.X1 = rawFrame.Interpolation.XAxis.X1;
				action.Interpolation.XAxis.X2 = rawFrame.Interpolation.XAxis.X2;
				action.Interpolation.XAxis.Y1 = rawFrame.Interpolation.XAxis.Y1;
				action.Interpolation.XAxis.Y2 = rawFrame.Interpolation.XAxis.Y2;
				action.Interpolation.YAxis.X1 = rawFrame.Interpolation.YAxis.X1;
				action.Interpolation.YAxis.X2 = rawFrame.Interpolation.YAxis.X2;
				action.Interpolation.YAxis.Y1 = rawFrame.Interpolation.YAxis.Y1;
				action.Interpolation.YAxis.Y2 = rawFrame.Interpolation.YAxis.Y2;
				action.Interpolation.ZAxis.X1 = rawFrame.Interpolation.ZAxis.X1;
				action.Interpolation.ZAxis.X2 = rawFrame.Interpolation.ZAxis.X2;
				action.Interpolation.ZAxis.Y1 = rawFrame.Interpolation.ZAxis.Y1;
				action.Interpolation.ZAxis.Y2 = rawFrame.Interpolation.ZAxis.Y2;

				action.Interpolation2.Rotation.X1 = rawFrame.Interpolation2.Rotation.X1;
				action.Interpolation2.Rotation.X2 = rawFrame.Interpolation2.Rotation.X2;
				action.Interpolation2.Rotation.Y1 = rawFrame.Interpolation2.Rotation.Y1;
				action.Interpolation2.Rotation.Y2 = rawFrame.Interpolation2.Rotation.Y2;
				action.Interpolation2.XAxis.X1 = rawFrame.Interpolation2.XAxis.X1;
				action.Interpolation2.XAxis.X2 = rawFrame.Interpolation2.XAxis.X2;
				action.Interpolation2.XAxis.Y1 = rawFrame.Interpolation2.XAxis.Y1;
				action.Interpolation2.XAxis.Y2 = rawFrame.Interpolation2.XAxis.Y2;
				action.Interpolation2.YAxis.X1 = rawFrame.Interpolation2.YAxis.X1;
				action.Interpolation2.YAxis.X2 = rawFrame.Interpolation2.YAxis.X2;
				action.Interpolation2.YAxis.Y1 = rawFrame.Interpolation2.YAxis.Y1;
				action.Interpolation2.YAxis.Y2 = rawFrame.Interpolation2.YAxis.Y2;
				action.Interpolation2.ZAxis.X1 = rawFrame.Interpolation2.ZAxis.X1;
				action.Interpolation2.ZAxis.X2 = rawFrame.Interpolation2.ZAxis.X2;
				action.Interpolation2.ZAxis.Y1 = rawFrame.Interpolation2.ZAxis.Y1;
				action.Interpolation2.ZAxis.Y2 = rawFrame.Interpolation2.ZAxis.Y2;

				action.Interpolation3.Rotation.X1 = rawFrame.Interpolation3.Rotation.X1;
				action.Interpolation3.Rotation.X2 = rawFrame.Interpolation3.Rotation.X2;
				action.Interpolation3.Rotation.Y1 = rawFrame.Interpolation3.Rotation.Y1;
				action.Interpolation3.Rotation.Y2 = rawFrame.Interpolation3.Rotation.Y2;
				action.Interpolation3.XAxis.X1 = rawFrame.Interpolation3.XAxis.X1;
				action.Interpolation3.XAxis.X2 = rawFrame.Interpolation3.XAxis.X2;
				action.Interpolation3.XAxis.Y1 = rawFrame.Interpolation3.XAxis.Y1;
				action.Interpolation3.XAxis.Y2 = rawFrame.Interpolation3.XAxis.Y2;
				action.Interpolation3.YAxis.X1 = rawFrame.Interpolation3.YAxis.X1;
				action.Interpolation3.YAxis.X2 = rawFrame.Interpolation3.YAxis.X2;
				action.Interpolation3.YAxis.Y1 = rawFrame.Interpolation3.YAxis.Y1;
				action.Interpolation3.YAxis.Y2 = rawFrame.Interpolation3.YAxis.Y2;
				action.Interpolation3.ZAxis.X1 = rawFrame.Interpolation3.ZAxis.X1;
				action.Interpolation3.ZAxis.X2 = rawFrame.Interpolation3.ZAxis.X2;
				action.Interpolation3.ZAxis.Y1 = rawFrame.Interpolation3.ZAxis.Y1;
				action.Interpolation3.ZAxis.Y2 = rawFrame.Interpolation3.ZAxis.Y2;

				action.Interpolation4.Rotation.X1 = rawFrame.Interpolation4.Rotation.X1;
				action.Interpolation4.Rotation.X2 = rawFrame.Interpolation4.Rotation.X2;
				action.Interpolation4.Rotation.Y1 = rawFrame.Interpolation4.Rotation.Y1;
				action.Interpolation4.Rotation.Y2 = rawFrame.Interpolation4.Rotation.Y2;
				action.Interpolation4.XAxis.X1 = rawFrame.Interpolation4.XAxis.X1;
				action.Interpolation4.XAxis.X2 = rawFrame.Interpolation4.XAxis.X2;
				action.Interpolation4.XAxis.Y1 = rawFrame.Interpolation4.XAxis.Y1;
				action.Interpolation4.XAxis.Y2 = rawFrame.Interpolation4.XAxis.Y2;
				action.Interpolation4.YAxis.X1 = rawFrame.Interpolation4.YAxis.X1;
				action.Interpolation4.YAxis.X2 = rawFrame.Interpolation4.YAxis.X2;
				action.Interpolation4.YAxis.Y1 = rawFrame.Interpolation4.YAxis.Y1;
				action.Interpolation4.YAxis.Y2 = rawFrame.Interpolation4.YAxis.Y2;
				action.Interpolation4.ZAxis.X1 = rawFrame.Interpolation4.ZAxis.X1;
				action.Interpolation4.ZAxis.X2 = rawFrame.Interpolation4.ZAxis.X2;
				action.Interpolation4.ZAxis.Y1 = rawFrame.Interpolation4.ZAxis.Y1;
				action.Interpolation4.ZAxis.Y2 = rawFrame.Interpolation4.ZAxis.Y2;

                frame.Actions.Add(action);
            }
            motion.Frames.Add(frame);

            // TODO figure out footer: it's different

            #region Footer
            br.BaseStream.Seek(-65, System.IO.SeekOrigin.End);

            uint n = br.ReadUInt32();
            for (uint i = 0; i < n; i++)
            {
                uint nBoneIndex = br.ReadUInt32();
            }

            byte[] extraData0001 = br.ReadBytes(n);
            byte extraData0001a = br.ReadByte();
            byte[] extraData0002 = br.ReadBytes(n);
            byte extraData0002a = br.ReadByte();
            byte[] extraData0003 = br.ReadBytes(n);
            byte extraData0003a = br.ReadByte();
            byte[] extraData0004 = br.ReadBytes(n);
            byte extraData0004a = br.ReadByte();
            
            byte nulEnd = br.ReadByte();
            #endregion
        }
        protected override void SaveInternal(ObjectModel objectModel)
        {
            throw new NotImplementedException();
        }
    }
}